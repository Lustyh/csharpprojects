using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShadowTM
{
    class SocketServer
    {
        private static Dictionary<string, Socket> socketList = new Dictionary<string, Socket>();
        
        public void Start()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint IEP = new IPEndPoint(IPAddress.Parse("0.0.0.0"), int.Parse("9090"));
            Console.WriteLine("controller port:9090");

            Socket socket2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint IEP2 = new IPEndPoint(IPAddress.Parse("0.0.0.0"), int.Parse("9091"));
            Console.WriteLine("controller port:9091");
            try
            {
                string data = DateTime.Now.AddDays(-5).ToString("yyyyMMdd");
                DirectoryInfo cur = new DirectoryInfo(Environment.CurrentDirectory);
                foreach (FileInfo file in cur.GetFiles())
                {
                    if (!(file.Name.EndsWith(".log") && file.Name.Contains("-")))
                    {
                        continue;
                    }
                    string file_time = file.Name.Substring(file.Name.Length - 12, 8);
                    if (string.Compare(file_time,data) != 1)
                    {
                        Console.WriteLine("remove file:" + file.Name);
                        save_log("Delete log", "remove file:" + file.Name);
                        File.Delete(file.FullName);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            //IPEndPoint IEP = new IPEndPoint(AddressFamily.InterNetwork, int.Parse("8090"));
            //绑定ip和端口
            socket.Bind(IEP);
            //开启监听
            socket.Listen(10);

            //richTextBox1.Invoke(new Action(() => { richTextBox1.AppendText("开始监听" + "\r\n"); }));
            save_log("STA","Start server");

            Thread thread = new Thread(new ParameterizedThreadStart(StartServer));
            thread.IsBackground = true;
            thread.Start(socket);




            socket2.Bind(IEP2);
            //开启监听
            socket2.Listen(10);
            save_log("STA", "Start server");

            Thread thread2 = new Thread(new ParameterizedThreadStart(StartServer));
            thread2.IsBackground = true;
            thread2.Start(socket2);

        }
        public void StartServer(object obj)
        {
            while (true)
            {
                Thread.Sleep(50);
                //等待接收客户端连接 Accept方法返回一个用于和该客户端通信的Socket
                Socket recviceSocket = ((Socket)obj).Accept();
                //获取客户端ip和端口号
                //str = recviceSocket.RemoteEndPoint.ToString();
                //socketList.Add(str, recviceSocket);
                //控件调用invoke方法 解决"从不是创建控件的线程访问它"的异常
                //cmb_socketlist.Invoke(new Action(() => { cmb_socketlist.Items.Add(str); }));
                //richTextBox1.Invoke(new Action(() => { richTextBox1.AppendText(str + "已连接" + "\r\n"); }));
                //Console.WriteLine(str + "已连接");
                //Accept()执行过后 当前线程会阻塞 只有在有客户端连接时才会继续执行
                //创建新线程,监控接收新客户端的请求数据
                Thread thread = new Thread(startRecive);
                thread.IsBackground = true;
                thread.Start(recviceSocket);
            }
        }
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="obj">客户端socket</param>
        public void startRecive(object obj)
        {
            string str1,str;
            string ip; 
            try
            {
                while (true)
                {

                    byte[] buffer = new byte[2048];
                    int count;
                    Thread.Sleep(50);
                    //Receive(Byte[]) 从绑定的 Socket 套接字接收数据，将数据存入接收缓冲区。
                    //该方法执行过后同Accept()方法一样  当前线程会阻塞 等到客户端下一次发来数据时继续执行
                    count = ((Socket)obj).Receive(buffer);
                    ip = ((Socket)obj).RemoteEndPoint.ToString();
                    if (count == 0)
                    {
                        //save_log("ERE", string.Format("{0} {1}","Disconnect",ip));
                        //break;
                    }
                    else
                    {
                        new Thread(new ThreadStart(() =>
                        {
                            str1 = Encoding.Default.GetString(buffer, 0, count);
                            string[] strs = str1.Split('^');
                            foreach (string strb in strs)
                            {
                                if (strb.Trim().Length > 0)
                                {
                                    str = strb + "^";
                                    save_log("TCP recv", "IP=" + ip + " " + str);
                                    string response = UserCommand.ProcessCommand(str, ip, obj);
                                    if (response.ToLower() != "n/a")
                                    {
                                        Send(response, ip, obj);
                                    }
                                }
                            }
                        })).Start();
                    }
                }
            }
            catch (Exception ex)
            {
                save_log("ERE", string.Format("{0} {1}", "line 91", ex.Message));
            }
        }
        public static void Send(string send_mes, string ip, object obj)
        {
            string str = send_mes;
            save_log("TCP send", "IP=" + ip + " " + send_mes);
            byte[] bytes = new byte[2048];
            bytes = Encoding.Default.GetBytes(str);
            ((Socket)obj).Send(bytes);
            
        }
        public static void Send_client(string send_mes, string ip)
        {
            try
            {
                IPAddress localAdd = IPAddress.Parse(ip);
                TcpListener listener = new TcpListener(localAdd, 8080);
                listener.Start();
                TcpClient client = listener.AcceptTcpClient();
                NetworkStream cstream = client.GetStream();
                byte[] data = Encoding.ASCII.GetBytes(send_mes);
                cstream.Write(data, 0, data.Length);
                Console.WriteLine("ok");
               
            }
            catch (Exception EE)
            {
                save_log("TCP ERE", "IP=" + ip + " " + EE.Message);
            }
        }
        public static void save_log(string type, string log)
        {
            try
            {
                string path = Path.Combine(".\\", "ShadowTM-" + DateTime.Now.ToString("yyyyMMdd") + ".log");
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine(string.Format("[{0}][{1}]{2}", type, DateTime.Now.ToString("HH:mm:ss:fff"), log));
                }
            }
            catch (Exception)
            {
            }
        }

    }
}
