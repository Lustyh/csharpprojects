using Ini;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Socket_Server_Robot
{
    public partial class Socket_Server : Form
    {
        public static string ServerIP = "10.18.6.52";
        public static string DBName = "QMS";
        public static string BU = "NB4";
        public static string Portal = "MonitorPortal";
        public static string Station = "SWDL";
        public static string Step = "querydata";
        public static string test_environment = "";
        public static int Count = 1;
        public static int Fix_Holder_No = 8;
        public static string defau = "FAIL";
        public static string test_mode = "";
        public static string exe_path = "";
        public static string auto_fill = "N";
        public static string SFStation = "";
        public static string RetestRule = "";
        public static string Port = "";
        public static bool Send_SN = true;

        public static string robotlogpath = Application.StartupPath + "\\logs\\Robot\\Robot_";
        public static string marslogpath = Application.StartupPath + "\\logs\\Mars\\mars_";
        UserCommands userCommands = new UserCommands();

        /*
         * 1.0.1.1 初版
         1.0.1.2 修复TYPR=SFC时，传送的站别是错误的bug
         1.0.1.3 不保存TYPR=result/status时的log  多加一个checkbox 查看result/status的log
             
             */


        public Socket_Server()
        {
            InitializeComponent();
        }
        private void Socket_Server_Load(object sender, EventArgs e)
        {
            Get_ini();
            userCommands.Init(Count, Fix_Holder_No, test_mode);
            userCommands.uploadData = upload_data;
            userCommands.printMes = Print_message;
            userCommands.sendToClifford = Send;
            ini_dgvdata();
            Message_show();
            IPEndPoint IEP = new IPEndPoint(IPAddress.Parse("0.0.0.0"), int.Parse(Port));
            Start_Socket(IEP);
            
        }
        /// <summary>
        /// 获取配置文件里面的信息
        /// </summary>
        public void Get_ini()
        {
            IniFile Getiniinfo = new IniFile(Application.StartupPath + "\\Setting.ini");
            ServerIP = Getiniinfo.IniReadValue("DataBase", "ServerIP");
            DBName = Getiniinfo.IniReadValue("DataBase", "DBName");
            BU = Getiniinfo.IniReadValue("DataBase", "BU");
            Portal = Getiniinfo.IniReadValue("DataBase", "Portal");
            Step = Getiniinfo.IniReadValue("DataBase", "Step");
            Station = Getiniinfo.IniReadValue("DataBase", "Station");

            test_environment = Getiniinfo.IniReadValue("HOLDER", "Test_Type");
            string F = Getiniinfo.IniReadValue("HOLDER", "Fix_Count");
            string H = Getiniinfo.IniReadValue("HOLDER", "Fix_Holder_Num");
            test_mode = Getiniinfo.IniReadValue("HOLDER", "Test_Mode");
            auto_fill = Getiniinfo.IniReadValue("HOLDER", "Auto_Fill");
            RetestRule = Getiniinfo.IniReadValue("HOLDER", "RetestRule");
            Count = int.Parse(F);
            Fix_Holder_No = int.Parse(H);

            SFStation = Getiniinfo.IniReadValue("STATION", "Station");
            defau = Getiniinfo.IniReadValue("STATION", "STATUS_XX");
            Port = Getiniinfo.IniReadValue("STATION", "Port");
            if (Getiniinfo.IniReadValue("HOLDER", "Send_SN").ToUpper() == "N")
            { Send_SN = false; }
            labelVersion.Text = "Version : " + Application.ProductVersion;

            if (!Directory.Exists(Application.StartupPath + "//logs//"))
            {
                Directory.CreateDirectory(Application.StartupPath + "//logs//");
            }
            if (!Directory.Exists(Application.StartupPath + "//logs//Mars"))
            {
                Directory.CreateDirectory(Application.StartupPath + "//logs//Mars");
            }
            if (!Directory.Exists(Application.StartupPath + "//logs//Robot"))
            {
                Directory.CreateDirectory(Application.StartupPath + "//logs//Robot");
            }
            if (!Directory.Exists(Application.StartupPath + "//logs//Shopfloor"))
            {
                Directory.CreateDirectory(Application.StartupPath + "//logs//Shopfloor");
            }
        }

        /// <summary>
        /// 显示基本信息到UI上面
        /// </summary>
        public void Message_show()
        {
            this.txtIP.Text = "0.0.0.0";
            this.label_Holders.Text = (Count * Fix_Holder_No).ToString();
            this.label_Port.Text = Port;
            this.label_Station.Text = SFStation;
            if (auto_fill.ToUpper() == "Y")
            {
                this.label_AutoFillSN.Text = "True";
            }
            else
            {
                this.label_AutoFillSN.Text = "False";
            }
            this.radioButton1.Visible = false;
            this.radioButton2.Visible = false;
            this.radioButton3.Visible = false;
            this.radioButton4.Visible = false;
        }

        /// <summary>
        /// 显示fixture id
        /// </summary>
        /// <param name="fix_name">station id</param>
        public void check_radioButton(string fix_name)
        {
            if (this.radioButton1.Text == fix_name || this.radioButton2.Text == fix_name || this.radioButton3.Text == fix_name || this.radioButton4.Text == fix_name)
            {
                return;
            }
            if (!this.radioButton1.Visible)
            {
                this.radioButton1.Visible = true;
                this.radioButton1.Text = fix_name;
            }
            if (!this.radioButton2.Visible)
            {
                this.radioButton2.Visible = true;
                this.radioButton2.Text = fix_name;
            }
            if (!this.radioButton3.Visible)
            {
                this.radioButton3.Visible = true;
                this.radioButton3.Text = fix_name;
            }
            if (!this.radioButton4.Visible)
            {
                this.radioButton4.Visible = true;
                this.radioButton4.Text = fix_name;
            }
        }

        /// <summary>
        /// 定义UI表里面的信息
        /// </summary>
        public void ini_dgvdata()
        {
            dgvData.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            string[] items = new string[] { "IP", "SN", "Fix_ID", "Slot", "Status", "Fail_Count", "Fail_Fix", "upload_cli" };
            DataTable dt = new DataTable();
            for (int i = 0; i < items.Length; i++)
            {
                dt.Columns.Add(items[i], typeof(string));
            }

            for (int i = 0; i < userCommands.Robots.Count; i++)
            {
                dt.Rows.Add(new string[] {
                    "0.0.0.0", //UserCommands.Robots[i].IP,
                    userCommands.Robots[i].SN,
                    userCommands.Robots[i].Fix_ID,
                    userCommands.Robots[i].slot,
                    userCommands.Robots[i].status.ToString(),
                    userCommands.Robots[i].Fail_Count.ToString(),
                    userCommands.Robots[i].Fail_Fix,
                    "",
                });
            }
            dt.Rows.Add();
            // readstatus();
            for (int i = 0; i < userCommands.holders.Count; i++)
            {
                dt.Rows.Add(new string[] {
                    userCommands.holders[i].IP,
                    userCommands.holders[i].SN,
                    userCommands.holders[i].Fix_ID,
                    userCommands.holders[i].slot,
                    userCommands.holders[i].status.ToString(),
                    userCommands.holders[i].Fail_Count.ToString(),
                    userCommands.holders[i].Fail_Fix,
                    userCommands.holders[i].upload_cli,
                });
            }
            this.dgvData.DataSource = dt;

            dgvData.AllowUserToAddRows = false;
            dgvData.ReadOnly = true;
            for (int i = 0; i < this.dgvData.Columns.Count; i++)
            { this.dgvData.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable; }
        }
        /// <summary>
        /// 打开TCP/IP的端口
        /// </summary>
        /// <param name="iPEndPoint">TCP/IPIP和端口</param>
        /// <returns>是否开启成功</returns>
        public bool Start_Socket(IPEndPoint iPEndPoint)
        {
            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(iPEndPoint);
                socket.Listen(10);
                Print_message("Start Server", "Server already open, IP:0.0.0.0 Port:" + Port);
                Thread thread = new Thread(new ParameterizedThreadStart(StartServer));
                thread.IsBackground = true;
                thread.Start(socket);
                return true;
            }
            catch (Exception E)
            {
                Print_message("Error", E.Message);
                return false;
            }
        }

        /// <summary>
        /// 开始监听client端的连接
        /// </summary>
        /// <param name="socket">socket</param>
        public void StartServer(object socket)
        {
            while (true)
            {
                Thread.Sleep(10);
                //等待接收客户端连接 Accept方法返回一个用于和该客户端通信的Socket
                Socket recviceSocket = ((Socket)socket).Accept();
                Thread thread = new Thread(startRecive);
                thread.IsBackground = true;
                thread.Start(recviceSocket);
            }
        }

        /// <summary>
        /// 处理client端的请求
        /// </summary>
        /// <param name="recviceSocket">socket</param>
        public void startRecive(object recviceSocket)
        {
            string str;
            string ip;
            while (true)
            {
                byte[] buffer = new byte[2048];
                int count;
                try
                {
                    //Receive(Byte[]) 从绑定的 Socket 套接字接收数据，将数据存入接收缓冲区。
                    //该方法执行过后同Accept()方法一样  当前线程会阻塞 等到客户端下一次发来数据时继续执行
                    count = ((Socket)recviceSocket).Receive(buffer);
                    ip = ((Socket)recviceSocket).RemoteEndPoint.ToString();


                    if (count == 0)
                    {
                        //save_log("ERE", string.Format("{0} {1}","Disconnect",ip));
                        Thread.Sleep(10);
                        break;
                    }
                    else
                    {
                        new Thread(new ThreadStart(() =>
                        {
                            Thread.Sleep(10);
                            Application.DoEvents();
                            str = Encoding.Default.GetString(buffer, 0, count);
                            Print_message("TCP recv", "IP=" + ip + " " + str);
                            //save_log("TCP", string.Format("{0} {1}:{2}", "Request:", ip,str));
                            //Console.WriteLine("收到" + ip + "数据  " + str + "\r\n");
                            System.Net.IPAddress ipAdd;
                            int port;
                            ipAdd = (((Socket)recviceSocket).RemoteEndPoint as IPEndPoint).Address;
                            port = (((Socket)recviceSocket).RemoteEndPoint as IPEndPoint).Port;

                            string response = "";
                            if (str.Contains("^") && str.Contains("{"))
                            {
                                string[] request_list = str.Split('^');
                                for (int i = 0; i < request_list.Length; i++)
                                {
                                    if (request_list[i] != "")
                                    {
                                        Print_message("Split", request_list[i] + "^");
                                        response = userCommands.ProcessCommand(request_list[i] + "^", ip, recviceSocket);
                                        //save_log("TCP", string.Format("{0} {1}:{2}", "Response:", ip, str));
                                        if (response.ToLower() != "n/a")
                                        {
                                            Send(response, ip, recviceSocket);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                response = userCommands.ProcessCommand(str, ip, recviceSocket);
                                //save_log("TCP", string.Format("{0} {1}:{2}", "Response:", ip, str));
                                if (response.ToLower() != "n/a")
                                {
                                    Send(response, ip, recviceSocket);
                                }
                            }
                            /*try
                            {
                                if (str.Contains("ready") && RetestRule == "AAB")
                                {
                                    Print_message("debug", RetestRule);
                                    for (int m = 0; m < userCommands.holders.Count; m++)
                                    {
                                        if (userCommands.holders[m].status == 1 && ip.Contains(userCommands.holders[m].IP.Split(':')[0]) && userCommands.holders[m].IP.Split(':')[0].Length > 8)
                                        {
                                            userCommands.Robot_Start(userCommands.holders[m].SN, m + 1);
                                        }
                                    }
                                }

                            }
                            catch (Exception errormessage)
                            {
                                Print_message("Error", errormessage.Message);
                            }*/
                        })).Start();
                    }
                }
                catch (Exception ex)
                {
                    Print_message("ERE", string.Format("{0} {1}", "line 301", ex.Message));
                    break;
                }
            }
        }

        /// <summary>
        /// Send message 给
        /// </summary>
        /// <param name="send_mes"></param>
        /// <param name="ip"></param>
        /// <param name="obj"></param>
        public void Send(string send_mes, string ip, object obj)
        {
            string str = send_mes;
            Print_message("TCP send", "IP=" + ip + " " + send_mes);
            byte[] bytes = new byte[2048];
            bytes = Encoding.Default.GetBytes(str);
            ((Socket)obj).Send(bytes);

        }

        /// <summary>
        /// 使richTextBoxLogs显示的信息一致处于最末端 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void richTextBoxLogs_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this.richTextBoxLogs.Select(richTextBoxLogs.Text.Length, 0);
                this.richTextBoxLogs.ScrollToCaret();
            }
            catch (Exception sd) { richTextBoxLogs.AppendText("\n" + sd.Message); }
        }
        
        
        /// <summary>
        /// 向richTextBoxLogs里面打印信息并保存log
        /// </summary>
        /// <param name="Type">信息类型</param>
        /// <param name="Txt">信息内容</param>
        public void Print_message(string Type, string Txt)
        {
            string content = string.Format("[{0}][{1}]:{2}", Type, DateTime.Now.ToString("HH:mm:ss:fff"), Txt);
            if (checkBox1.Checked)
            {
                this.richTextBoxLogs.AppendText(content + "\n");
            }
            try
            {
                if (Txt.Contains("RESULT") || Txt.Contains("STATUS") || Txt.Length <= 4)
                {
                    return;  //don't recode RESULT/STATUS api logs
                }
                string logpath = "";
                if (Txt.Contains("{"))
                {
                    logpath = marslogpath + DateTime.Now.ToString("yyyyMMdd") + ".log";
                }
                else
                {
                    logpath = robotlogpath + DateTime.Now.ToString("yyyyMMdd") + ".log";
                }
                /*string path = Path.Combine(".\\logs\\", "ShadowTM-" + DateTime.Now.ToString("yyyyMMdd") + ".log");
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine(content);
                }*/
                StreamWriter WriteLog2Ser = new StreamWriter(logpath, true);
                WriteLog2Ser.AutoFlush = true;
                WriteLog2Ser.WriteLine(content);
                WriteLog2Ser.Flush();
                WriteLog2Ser.Close();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 更新UI表上面的数据
        /// </summary>
        /// <param name="row">行</param>
        /// <param name="cell">列</param>
        /// <param name="txt">内容</param>
        /// <returns>更新结果</returns>
        public bool upload_data(int row, int cell, string txt)
        {
            try
            {
                //if (cell == 1 || cell == 2 || cell == 4)
                //{
                this.dgvData.Rows[row].Cells[cell].Value = txt;
                //}
                if (cell == 4)
                {
                    writestatus();
                }
            }
            catch (Exception)
            {
                Print_message("Error", "Updata UI error");
            }
            return true;
        }

        public void writestatus()
        {
            try
            {
                string status_file = Application.StartupPath + "\\status.txt";
                string status_log = "";

                for (int i = 0; i < dgvData.RowCount; i++)
                {
                    status_log += dgvData.Rows[i].Cells[4].Value + ",";
                }
                StreamWriter WriteLog2Ser = new StreamWriter(Application.StartupPath + "\\status.txt", false);
                WriteLog2Ser.AutoFlush = true;
                WriteLog2Ser.WriteLine(status_log);
                WriteLog2Ser.Flush();
                WriteLog2Ser.Close();
            }
            catch (Exception)
            {

            }
        }

        public void readstatus()
        {
            try
            {
                string status_file = Application.StartupPath + "\\status.txt";
                if (!File.Exists(status_file))
                {
                    return;
                }
                string[] content = File.ReadAllText(status_file).Split(',');
                for (int i = 0; i < userCommands.holders.Count; i++)
                {
                    try
                    {
                        userCommands.holders[i].status = int.Parse(content[i+3]);
                    }
                    catch (Exception)
                    {
                        userCommands.holders[i].status = 0;
                    }
                }
            }
            catch (Exception)
            {

            }

        }
        /// <summary>
        /// 向client端发送信息事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Click(object sender, EventArgs e)
        {
            /*string aaa = "{\"message_id\": \"test_complete\", \"project_id\": \"BOREAL\", \"station_id\": \"NHL_F6-R46_FATP-PROV_01\", \"group_id\": 3, \"slot_id\": 0, \"dut_id\": \"\", \"result\": \"NO_SN\", \"extra_info\": \"\"}^";
            object a = null;
            upload_data(3 + 3, 4, "5");
            upload_data(3 + 3, 1, "wiptest");
            upload_data(3 + 3, 3, "3");
            upload_data(3 + 3, 2, "NHL_F6-R46_FATP-PROV_01");
            MessageBox.Show("OK");
            string ab = userCommands.ProcessCommand(aaa, "123", a);
            Print_message("DEBUGAA", ab);*/

            /*string str1 = Process.GetCurrentProcess().MainModule.FileName;//可获得当前执行的exe的文件名。   ********
            string str2 = Environment.CurrentDirectory;//获取和设置当前目录（即该进程从中启动的目录）的完全限定路径。
                                                       //备注按照定义，如果该进程在本地或网络驱动器的根目录中启动，则此属性的值为驱动器名称后跟一个尾部反斜杠（如“C:\”）。如果该进程在子目录中启动，则此属性的值为不带尾部反斜杠的驱动器和子目录路径（如“C:\mySubDirectory”）。
            string str3 = Directory.GetCurrentDirectory();//获取应用程序的当前工作目录。
            string str4 = AppDomain.CurrentDomain.BaseDirectory;//获取基目录，它由程序集冲突解决程序用来探测程序集。
            string str5 = Application.StartupPath;//获取启动了应用程序的可执行文件的路径，不包括可执行文件的名称。
            string str6 = Application.ExecutablePath;//获取启动了应用程序的可执行文件的路径，包括可执行文件的名称。   *****
            string str7 = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;//获取或设置包含该应用程序的目录的名称。
            MessageBox.Show(str1 + "\r\n" + str2 + "\r\n" + str3 + "\r\n" + str4 + "\r\n" + str5 + "\r\n" + str6 + "\r\n" + str7 + "\r\n");*/
        }

        /// <summary>
        /// 给datagridview标注行号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /*private void dgvData_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;
            var rowIdx = (e.RowIndex + 1).ToString();

            var centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }*/

        public static void mesage()
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                timer_logPrint.Enabled = true;
            }
        }

        private void timer_logPrint_Tick(object sender, EventArgs e)
        {
            // checkBox1.Checked = false;
            // checkBox_printstatus.Checked = false;
            //richTextBoxLogs.Clear();
            timer_logPrint.Enabled = false;
        }

        private void checkBox_printstatus_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_printstatus.Checked)
            {
                timer_logPrint.Enabled = true;
            }
        }
    }
}
