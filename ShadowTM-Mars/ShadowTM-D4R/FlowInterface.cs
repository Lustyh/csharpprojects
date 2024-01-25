using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace ShadowTM
{
    /// <summary>
    /// This interface is a TCP interface to communicate with HMI
    /// </summary>
    class FlowInterface
    {
        public delegate void ReceiveCommand(object sender, string parameter);
        public event ReceiveCommand event_ReceiveCommand;

        /*
            HMI內建Shadows: 36900~36999
            客戶專屬 Shadows: 37000~37099
        */
        const uint MIN_CUSTOM_PORT = 37000;
        const uint MAX_CUSTOM_PORT = 37099;
        const uint MIN_HMI_PORT = 36900;
        const uint MAX_HMI_PORT = 36999;

        const uint DEFAULT_TCP_PORT = MIN_CUSTOM_PORT + 1;
        private uint _Port;   // TCP port number
        private bool is_tcp_running = false;

        private TcpListener Listener = null;
        
        public FlowInterface(uint port = DEFAULT_TCP_PORT)
        {
            _Port = (port <= MAX_CUSTOM_PORT && port >= MIN_CUSTOM_PORT) ? port : DEFAULT_TCP_PORT;
            if (port == 502) _Port = 502;
        }

        public uint Port { get { return _Port; } }
        
        public void Start()
        {
            if (Listener != null) return;
            Listener = new TcpListener(IPAddress.Any, (int)_Port);
            is_tcp_running = true;
            Task.Run(() => { TcpListenThread(); });
        }

        public bool Stop()
        {
            is_tcp_running = false;
            return true;
        }

        public void TcpListenThread()
        {
            List<Socket> list_connections = new List<Socket>();
            try
            {
                Listener.Start();
                while (is_tcp_running)
                {
                    // check whether is any connection request
                    if (!Listener.Pending())
                    {
                        SpinWait.SpinUntil(() => false, 10);
                        continue;
                    }

                    var client = Listener.AcceptSocket();
                    Logger.Record("TCP","listen accepted");
                    list_connections.Add(client);
                    Task.Run(() => { TcpService(client); });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("TCP Listen Exception:" + ex.Message);
                is_tcp_running = false;
            }

            // close & remove all connections
            while (list_connections.Count > 0)
            {
                // it is possible that the socket is closed in the other thread
                try { list_connections[0].Disconnect(false); }
                catch { }
                try { list_connections[0].Close(); }
                catch { }
                try { list_connections.RemoveAt(0); }
                catch { }
            }
            // close listener
            try { Listener.Stop(); Listener = null; }
            catch { }
        }
        
        private void TcpService(Socket client)
        {
            const int time_socket_alive = 90 * 1000;//milliseconds
            const int time_alive_interval = 1 * 1000; // milliseconds
            SetKeepAlive(client, time_socket_alive, time_alive_interval);
            try
            {
                EndPoint remote_ep = client.RemoteEndPoint;

                while (is_tcp_running)
                {
                    SpinWait.SpinUntil(() => { return client.Available > 0 || !is_tcp_running; }, -1);
                    if (!is_tcp_running) break;
                    int length = client.Available;
                    if (length == 0) continue;
                    byte[] buffer = new byte[length];
                    client.ReceiveFrom(buffer, ref remote_ep);
                    string result = Encoding.UTF8.GetString(buffer);
                    if (length > 100)
                    {
                        Console.WriteLine("err detected, close the connection");
                        break;
                    }
                    event_ReceiveCommand?.Invoke(client, result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Socket Exception:" + ex.Message);
            }
            try { client.Shutdown(SocketShutdown.Both); }catch { }
            try { client.Disconnect(false); }catch { }
            try { client.Close(); }catch { }
            client = null;
        }
        /// <summary>
        /// SetKeepAlive
        /// this function is used to set keepalive option of socket
        /// </summary>
        /// <param name="sock"></param>
        /// works on which socket
        /// <param name="time"></param>
        /// if there is no any conmunication in this scoket, after "time", starting sending beacon to check whether the remote site is alive
        /// "time" is millisecond.
        /// <param name="interval"></param>
        ///  time interval between each two detection beacons
        /// <returns></returns>
        private bool SetKeepAlive(Socket sock, ulong time, ulong interval)
        {
            try
            {
                // resulting structure
                byte[] SIO_KEEPALIVE_VALS = new byte[12];

                // array to hold input values
                ulong[] input = new ulong[3] { (time == 0 || interval == 0) ? 0UL : 1UL, time, interval };
                // pack input into byte struct
                for (int i = 0; i < SIO_KEEPALIVE_VALS.Length; i++)
                {
                    SIO_KEEPALIVE_VALS[i] = (byte)(input[i / 4] >> ((3 - i % 4) * 8) & 0xff);
                }
                // create bytestruct for result (bytes pending on server socket)
                byte[] result = BitConverter.GetBytes(0);
                // write SIO_VALS to Socket IOControl
                sock.IOControl(IOControlCode.KeepAliveValues, SIO_KEEPALIVE_VALS, result);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    } 
}
