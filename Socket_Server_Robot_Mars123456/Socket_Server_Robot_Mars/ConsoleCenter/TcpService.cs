using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleCenter
{
    static class TcpService
    {
        public delegate void TcpHandler(TcpClient c);
        private static List<TcpClient> connection_list = new List<TcpClient>();
        private static List<TcpListener> listener_list = new List<TcpListener>();
        private static object locker = new object();
        private static bool reloading = false;

        public static void Initialize()
        {
            int port = 37000;
            if (GetShadowPort(ref port))
            {
                //listen TM shadow port
                Listen(port);
            }
            if (port.ToString() != Setting.Port)
            {
                //listen mars/clifford port
                Listen(int.Parse(Setting.Port));
            }
            reloading = false;
        }

        private static bool GetShadowPort(ref int port)
        {
            string shadow = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Shadow.txt");
            if (!File.Exists(shadow)) return false;

            string[] lines = File.ReadAllLines(shadow);
            foreach (string l in lines)
            {
                if (!l.Contains("=")) continue;

                string[] arg = l.Split('=');
                if (arg[0].Trim().ToLower() == "port")
                {
                    int iport = 0;
                    if (int.TryParse(arg[1].Trim(), out iport))
                    {
                        port = iport;
                        Console.WriteLine($"Get Shadow Port Setting:{iport}");
                        return true;
                    }
                }
                Console.WriteLine(l.Trim());
            }
        }

        public static void Listen(int port)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            lock (locker)
            {
                listener_list.Add(listener);
                listener.Start();
            }
            Console.WriteLine($"Start listen port:{port}");
            Logger.WriteLine("SYS", $"Start listen port:{port}");
            Task.Run(() => { StartListenThread(listener); });
        }

        public static void StartListenThread(TcpListener listener)
        {
            while (!TesterData.IsStopRunning)
            {
                try
                {
                    if (!listener.Pending())
                    {
                        //空闲10ms
                        SpinWait.SpinUntil(() => false, 10);
                        continue;
                    }
                    TcpClient client = listener.AcceptTcpClient();
                    Logger.WriteLine("TCP", $"{GetTcpClientInfo(client)} <accept>");
                    lock (locker)
                    {
                        connection_list.Add(client);
                    }
                    Task.Run(() => { ReadSocketStream(client); });
                }catch(Exception x)
                {
                    Logger.WriteLine("TCP", x.Message);
                    if(reloading) break;
                }
            }
        }

        public static void ReadSocketStream(TcpClient client)
        {
            string socket_info = GetTcpClientInfo(client);
            try
            {
                while (!TesterData.IsStopRunning)
                {
                    //自旋等待,当条件为false就一直等待,为true时跳出循环,可有效降低CPU占用.
                    SpinWait.SpinUntil(() => {
                        return client.Available > 0 || TesterData.IsStopRunning;
                    }, -1);
                    if (client.Available > 0)
                    {
                        byte[] buffer = new byte[client.Available];
                        NetworkStream s = client.GetStream();
                        s.Read(buffer, 0, buffer.Length);
                        string msg = Encoding.UTF8.GetString(buffer);
                        Logger.WriteLine("TCP", $"{socket_info} <read> {msg}");
                        Task.Run(() =>
                        {
                            List<AutoMessage> msgs = Message.ParsingMessage(msg);
                            foreach (AutoMessage imsg in msgs)
                            {
                                string ret = Message.ProcessMessage(client, imsg);
                                WriteSocketStream(client, ret, socket_info);
                                Thread.Sleep(50);
                            }
                        });
                    }
                }
            }
            catch (Exception x)
            {
                Logger.WriteLine("TCP", $"{socket_info} <read error> {x.Message}");
            }
        }

        public static void WriteSocketStream(TcpClient client, string msgstr, string socket_info = null)
        {
            if (msgstr.Length > 0)
            {
                if ((socket_info == null) && (client != null))
                {
                    socket_info = GetTcpClientInfo(client);
                }
                try
                {
                    NetworkStream s = client.GetStream();
                    byte[] buffer = Encoding.UTF8.GetBytes(msgstr);
                    s.Write(buffer, 0, buffer.Length);
                    s.Flush();
                    Logger.WriteLine("TCP", $"{socket_info}  <send> {msgstr}");
                }
                catch (Exception x)
                {
                    Logger.WriteLine("TCP", $"{socket_info}  <send error> {msgstr} : {x.Message}");
                }
            }
        }

        public static string GetTcpClientInfo(TcpClient client)
        {
            string ip = (client.Client.RemoteEndPoint as IPEndPoint).Address.ToString();
            string port = (client.Client.RemoteEndPoint as IPEndPoint).Port.ToString();
            return $"{ip}:{port}";
        }

        public static void CloseAll()
        {
            lock (locker)
            {
                reloading = true;
                for(int i = 0; i < listener_list.Count; i++)
                {
                    try
                    {
                        listener_list[i].Stop();
                    }
                    catch { }
                }
                listener_list.Clear();
                for(int i = 0; i < connection_list.Count; i++)
                {
                    try
                    {
                        connection_list[i].Close();
                    }
                    catch { }
                }
                connection_list.Clear();
            }
        }
    }
}
