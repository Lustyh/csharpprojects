using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using Ini;

namespace Screw_Check
{
    public partial class Form1 : Form
    {
        IniFile Getiniinfo = new IniFile(Application.StartupPath + "\\Screw_Setting.ini");
        public int ax = 0, ay = 315, t = 0, S_Count = 0, Total_Count = 0;
        public bool haso = false;
        SerialPort serialPort1;
        //public StreamWriter write_log, write_log_server;
        public string SN = null, WIPSN = null, filename = null, DateTime_log=null;
        public string SF_SERVER = "", SF_JK = "", DB_NAME = "";
        public double TORSION = 0;
        public string log_path_server = "", log_path_local = "";
        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            textBox1.Enabled = true;
            textBox1.Clear();
            textBox1.Focus();
            richTextBox1.Clear();
            richTextBox2.Clear();
            S_Count = 0;
            label1.Text = S_Count.ToString();
            label2.Visible = false;
            try
            {
                try { serialPort1.Close(); } catch (Exception sd) { richTextBox2.AppendText(sd.Message); }
                button1.Text = "Disconnect";


            }
            catch (Exception sd) { richTextBox2.AppendText(sd.Message); }
        }
        
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (textBox1.Text.Length == 0)
                    {
                        MessageBox.Show("SN 格式错误，请重新输入"); textBox1.Enabled = true; textBox1.Clear();
                        textBox1.Focus(); return;
                    }
                    richTextBox3.Clear();
                    richTextBox1.Clear();
                    richTextBox2.Clear();
                    richTextBox3.BackColor = SystemColors.Control;
                   // label2.Visible = false;
                    textBox1.Enabled = false;
                    string B_SN = textBox1.Text.ToUpper();
                    if ("B" == B_SN.Substring(0, 1).ToUpper())
                    {
                        B_SN = B_SN.Substring(1);
                    }
                    SN = B_SN;
                    DateTime_log = DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\" + DateTime.Now.ToString("dd");
                    filename = SN + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");

                    string SN_Info = Querydata(comboBox4.Text, "SCREW", "MASTER:SN=" + SN + ";*;MASTER:STATION=" + comboBox4.Text + ";*;MASTER:STEP=1;*;MASTER:MonitorAgentVer=VW20151102.01;*;");
                    richTextBox3.AppendText("\n" + SN_Info);
                    try
                    {
                        write_screw_log(SN_Info); //write_log.WriteLine(re); write_log_server.WriteLine(re);
                    }
                    catch (Exception sd) { richTextBox2.AppendText("\n" + sd.Message); }
                    //string SN_Status = Get_str("status=", ";$;", SN_Info);
                    //label4.Text = SN_Status;
                    if (!SN_Info.Contains("=PASS"))
                    {
                        label4.Text = "FAIL";
                        label4.Text = "FAIL";
                        richTextBox3.BackColor = Color.Red;
                        textBox1.Enabled = true;
                        textBox1.Clear();
                        textBox1.Focus();return;
                    }
                    label4.Text = "PASS";
                    //SN = textBox1.Text;
                    /*try { write_log.Close(); write_log_server.Close(); } catch { }
                    write_log = new StreamWriter(@"d:\Screw_log\"+SN+"-"+DateTime.Now.ToString("yyyyMMddHHmmss")+".log");
                    write_log.AutoFlush = true;*/
                    S_Count = 0;
                    
                    /*if (!Directory.Exists(@"\\10.18.6.49\sel_monitor\Caprica_Test_Logs\NH9_Screw_Logs\"+ DateTime_log))
                    {
                        Directory.CreateDirectory(@"\\10.18.6.49\sel_monitor\Caprica_Test_Logs\NH9_Screw_Logs\" + DateTime_log);
                    }
                    write_log_server = new StreamWriter(@"\\10.18.6.49\sel_monitor\Caprica_Test_Logs\NH9_Screw_Logs\" + DateTime_log+"\\" + SN + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".log");
                    write_log_server.AutoFlush = true;*/
                }

            }
            catch (Exception sd) { richTextBox2.AppendText("\n" + sd.Message); }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        public void write_screw_log(string logs)
        {
            try
            {
                if (!Directory.Exists(log_path_local + "\\" + DateTime_log))
                {
                    Directory.CreateDirectory(log_path_local + "\\" + DateTime_log);
                }
                StreamWriter write_log = new StreamWriter(log_path_local + "\\" + DateTime_log + "\\" + filename + ".log", true);
                write_log.AutoFlush = true;
                write_log.Write(logs);
                write_log.Close();

                if (log_path_server != "")
                {
                    try
                    {
                        if (!Directory.Exists(log_path_server + "\\" + DateTime_log))
                        {
                            Directory.CreateDirectory(log_path_server + "\\" + DateTime_log);
                        }
                        StreamWriter write_log_server = new StreamWriter(log_path_server + "\\" + DateTime_log + "\\" + filename + ".log", true);
                        write_log_server.AutoFlush = true;
                        write_log_server.Write(logs);
                        write_log_server.Close();
                    }
                    catch (Exception)
                    {
                    }
                }

            }
            catch (Exception)
            {
            }
        }

        private string Get_str(string name, string separation, string str_sample)
        {
            try
            {

                string Get_1, Get_2, Get_3, Get_4, Get_5;
                int Get_n_1, Get_n_2, Get_n_3, Get_n_4, Get_n_5;
                Get_1 = name.ToUpper();
                Get_2 = separation.ToUpper();
                Get_3 = str_sample;
                Get_n_4 = Get_3.IndexOf(Get_1);
                Get_n_5 = Get_3.IndexOf(Get_2);
                if (Get_n_4 > -1)
                { }
                else
                {
                    //richTextBox1.SelectionBackColor = Color.Red;
                    //richTextBox1.SelectionColor = Color.White;
                    //richTextBox1.AppendText("\n请检查QMS是否上传\"" + Get_1 + "\"");
                }
                if (Get_n_5 > -1) { } else { };// richTextBox1.SelectionBackColor = Color.Red; richTextBox1.SelectionColor = Color.White; richTextBox1.AppendText("\n请检查是否有\"" + Get_2 + "\""); }
                Get_4 = Get_3.Substring(Get_3.ToUpper().IndexOf(Get_1));
                Get_n_1 = Get_1.Length;
                Get_n_2 = Get_2.Length;
                Get_n_3 = Get_4.ToUpper().IndexOf(Get_2);
                Get_5 = Get_3.Substring(Get_3.ToUpper().IndexOf(Get_1) + Get_n_1, Get_n_3 - Get_n_1);
                return Get_5;

            }
            catch (Exception ww)
            {
                //richTextBox1.AppendText("ww:" + ww.Message);
                return "null";
            }
        }
        private string Querydata(string station, string step, string Inputstring)//同SF交换过站信息
        {
            try
            {
                SqlConnection a = new SqlConnection();
                a.ConnectionString = "server=" + SF_SERVER + ";Database=" + DB_NAME + ";Uid=sdt;Pwd=SDT#7;Integrated Security=False";
                try
                { a.Open(); }
                catch (Exception e)
                {
                    richTextBox1.AppendText("\n" + e.ToString());
                    Application.DoEvents();
                    Application.DoEvents();

                    //goto Label_112;
                }
                if (a.State == ConnectionState.Open)
                {
                    richTextBox1.AppendText("\n" + "Conect DB ok");
                    SqlCommand b = new SqlCommand(SF_JK, a);
                    b.CommandType = CommandType.StoredProcedure;
                    b.Parameters.Add("@BU", SqlDbType.VarChar).Value = "NB4";
                    b.Parameters.Add("@Station", SqlDbType.VarChar).Value = station;
                    b.Parameters.Add("@Step", SqlDbType.VarChar).Value = step;
                    b.Parameters.Add("@InPutStr", SqlDbType.VarChar).Value = Inputstring;
                    b.Parameters.Add("@OutPutStr", SqlDbType.VarChar, 8000);
                    b.Parameters["@BU"].Direction = ParameterDirection.Input;
                    b.Parameters["@BU"].DbType = DbType.String;
                    b.Parameters["@Station"].Direction = ParameterDirection.Input;
                    b.Parameters["@Station"].DbType = DbType.String;
                    b.Parameters["@Step"].Direction = ParameterDirection.Input;
                    b.Parameters["@Step"].DbType = DbType.String;
                    b.Parameters["@InPutStr"].Direction = ParameterDirection.Input;
                    b.Parameters["@InPutStr"].DbType = DbType.String;
                    b.Parameters["@OutPutStr"].DbType = DbType.String;
                    b.Parameters["@OutPutStr"].Direction = ParameterDirection.Output;
                    b.ExecuteScalar();
                    string ss = (string)b.Parameters["@OutPutStr"].Value;
                    a.Close();
                    return ss;

                }
                return "连接数据服务器出现异常2";
            }
            catch(Exception ex)
            {
                //
                return "连接数据服务器出现异常:" + ex.Message;
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            // 设置焦点
            textBox1.Focus();
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.AddRange(SerialPort.GetPortNames());
            if (SerialPort.GetPortNames().Length > 0)
            { comboBox1.Text = SerialPort.GetPortNames()[0]; }
            if (!Directory.Exists(@"d:\Screw_log\"))
            {
                Directory.CreateDirectory(@"d:\Screw_log\");
            }
            label3.Text = "Version:" + this.ProductVersion.ToString();
            if (Application.StartupPath.Contains(@"\\"))
            {
                MessageBox.Show("请不要直接从server上直接打开程序\n请点击Loader文件夹下的快捷方式");
                this.Close();
            }
            comboBox3.Text = Getiniinfo.IniReadValue("SETTING", "NUM");
            comboBox4.Text = Getiniinfo.IniReadValue("SETTING", "STATION");
            comboBox2.Text = Getiniinfo.IniReadValue("SETTING", "Port");
            log_path_server = Getiniinfo.IniReadValue("PATH", "LogPathServer");
            log_path_local = Getiniinfo.IniReadValue("PATH", "LogPathLocal");

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            comboBox1.Items.AddRange(SerialPort.GetPortNames());
            if (SerialPort.GetPortNames().Length > 0)
            { comboBox1.Text = SerialPort.GetPortNames()[0]; }
        }
        private string DBInfo(string station, string step, string Inputstring)//同SF交换过站信息
        {
            SqlConnection a = new SqlConnection();
            a.ConnectionString = "server=" + SF_SERVER + ";Database=" + DB_NAME + ";Uid=sdt;Pwd=SDT#7;Integrated Security=False";
            try
            { a.Open(); }
            catch { richTextBox3.AppendText( "\nlink DBfail，网络连接异常，请检查！"); }
            if (a.State == ConnectionState.Open)
            {
                SqlCommand b = new SqlCommand(SF_JK, a);
                b.CommandType = CommandType.StoredProcedure;
                b.Parameters.Add("@BU", SqlDbType.VarChar).Value = "NB4";
                b.Parameters.Add("@Station", SqlDbType.VarChar).Value = station;
                b.Parameters.Add("@Step", SqlDbType.VarChar).Value = step;
                b.Parameters.Add("@InPutStr", SqlDbType.VarChar).Value = Inputstring;
                b.Parameters.Add("@OutPutStr", SqlDbType.VarChar, 2000);
                b.Parameters["@BU"].Direction = ParameterDirection.Input;
                b.Parameters["@BU"].DbType = DbType.String;
                b.Parameters["@Station"].Direction = ParameterDirection.Input;
                b.Parameters["@Station"].DbType = DbType.String;
                b.Parameters["@Step"].Direction = ParameterDirection.Input;
                b.Parameters["@Step"].DbType = DbType.String;
                b.Parameters["@InPutStr"].Direction = ParameterDirection.Input;
                b.Parameters["@InPutStr"].DbType = DbType.String;
                b.Parameters["@OutPutStr"].DbType = DbType.String;
                b.Parameters["@OutPutStr"].Direction = ParameterDirection.Output;
                b.ExecuteScalar();
                string ss = (string)b.Parameters["@OutPutStr"].Value;
                a.Close();
                return ss;
            }
            return "123";

        }
        void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {   //  Thread.Sleep(Convert.ToInt32(comboBox3.Text));
                byte firstByte = Convert.ToByte(serialPort1.ReadByte());
                Thread.Sleep(80);
                // 防止页面停止响应，允许在耗时操作的内部调用它，而去处理消息队列中的消息，启到实时响应的效果
                Application.DoEvents();
                byte[] Resoursedata = new byte[serialPort1.BytesToRead];

                serialPort1.Read(Resoursedata, 0, Resoursedata.Length);//在此就可以读取到当前缓冲区内的数据
                                                                       //执行数据操作

                string re = System.Text.Encoding.Default.GetString(Resoursedata);



                try
                {
                    write_screw_log(re); //write_log.WriteLine(re); write_log_server.WriteLine(re);
                }
                catch(Exception sd) { richTextBox2.AppendText("\n"+sd.Message); }
                    

                richTextBox2.AppendText(re);
                //if (re.Contains("OK") || re.Substring(0, 45).Contains("O") || re.Substring(0, 45).Contains("K"))
                try {
                    if (Convert.ToDouble(re.Split(',')[9]) >= TORSION)
                    {
                        if (!textBox1.Enabled)
                        {
                            S_Count++;
                            label1.Text = S_Count.ToString();
                        }
                        
                             try
                        { WavPlayer.Play(Application.StartupPath + @"\ok.wav"); } catch { }
                    }
                }
                catch (Exception sd) { richTextBox2.AppendText("\n"+sd.Message); }
                
                if (S_Count == Convert.ToInt32(comboBox3.Text))
                {
                    //try { serialPort1.Close(); } catch (Exception sd) { richTextBox1.AppendText(sd.Message); }
                    //string inputs = "MASTER:SN=" + SN + ";*;" + "MASTER:STATION=" + comboBox4.Text + ";*;MASTER:ERRORCODE=PASS;*;MASTER:MonitorAgentVer=VW20151102.01;*;";
                    string inputs = "MASTER:SN=" + SN + ";*;MASTER:STATION=" + comboBox4.Text + ";*;MASTER:STEP=2;*;MASTER:MonitorAgentVer=VW20151102.01;*;";
                    try
                    {

                        new Thread(new ThreadStart(() =>
                        {
                            string result = null;
                            string inputs1 = inputs;
                            richTextBox3.AppendText("\n开始过站，请确保网线连接正常");
                            result = DBInfo(comboBox4.Text, "SCREW", inputs1);
                            try
                            {
                                write_screw_log(inputs1);
                                write_screw_log(result);//write_log_server.WriteLine(inputs1); write_log_server.WriteLine(result);
                            }
                            catch (Exception sd) { richTextBox2.AppendText("\n" + sd.Message); }
                            if (result.ToUpper().Contains("=PASS"))
                            {
                                richTextBox3.BackColor = Color.LimeGreen;
                                //label2.Visible = true;
                                //label2.BackColor = Color.LimeGreen; label2.ForeColor = Color.White; label2.Text = "PASS";
                            }
                            else
                            {
                                richTextBox3.BackColor = Color.Red;
                                //label2.Visible = true;
                                //label2.BackColor = Color.Red; label2.ForeColor = Color.White; label2.Text = "FAIL"; 
                            }
                            /*try
                            { write_log.Close(); write_log_server.Close(); }
                            catch (Exception sd) { richTextBox2.AppendText("\n" + sd.Message); }*/
                            richTextBox3.AppendText(result);
                        })).Start();
                         
                      
                        
                        textBox1.Enabled = true; textBox1.Clear(); textBox1.Focus();
                        S_Count = 0; label1.Text = S_Count.ToString();
                        S_Count = 0;
                        //sw.Close();
                    }
                    catch (Exception sd) { richTextBox2.AppendText("\n" + sd.Message); S_Count = 0; }

                }



                serialPort1.DiscardInBuffer();//丢弃传输缓冲区数据
                serialPort1.DiscardOutBuffer();//每次丢弃接收缓冲区的数据
            }
            catch (Exception sd) { richTextBox1.AppendText(sd.Message); }
        }
        private void button1_Click(object sender, EventArgs e)
        {
           
            if (button1.Text == "Connect")
            {

                try
                {
                    serialPort1 = new SerialPort();
                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);
                    serialPort1.Parity = Parity.None;
                    serialPort1.DataBits = 8;
                    serialPort1.StopBits = StopBits.One;
                    serialPort1.Open();
                    serialPort1.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
                    serialPort1.ReceivedBytesThreshold = 1;
                    button1.Text = "Disconnect";
                    timer1.Enabled = true;
                    Total_Count = Convert.ToInt32(comboBox3.Text);

                    SF_SERVER = Getiniinfo.IniReadValue("FATP", "DBserver");
                    SF_JK = Getiniinfo.IniReadValue("JK", "JK");
                    DB_NAME = Getiniinfo.IniReadValue("FATP", "DBname");
                    string string_TORSION = Getiniinfo.IniReadValue("TORSION", comboBox3.Text);
                    
                    if (double.TryParse(string_TORSION,out TORSION))
                    {
                        richTextBox3.AppendText("TORSION:" + string_TORSION + "\n");
                    }
                    else
                    {
                        richTextBox3.AppendText("TORSION_ERROR:" + string_TORSION + "\n");
                    }
                    richTextBox3.AppendText("SF SERVER:" + SF_SERVER + "\n");
                    richTextBox3.AppendText("SF JK:" + SF_JK + "\n");
                    richTextBox3.AppendText("SF DB_NAME:" + DB_NAME + "\n");
                    
                }
                catch (Exception ss) { richTextBox2.AppendText("\n" + ss.Message); }
            }
            else
            {
                serialPort1.Close();
                button1.Text = "Connect";
                timer1.Enabled = false;
            }
        }
    }
    public static class WavPlayer
    {
        [DllImport("winmm.dll", SetLastError = true)]
        static extern bool PlaySound(string pszSound, UIntPtr hmod, uint fdwSound);
        [DllImport("winmm.dll", SetLastError = true)]
        static extern long mciSendString(string strCommand,
            StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);
        [DllImport("winmm.dll")]
        private static extern long sndPlaySound(string lpszSoundName, long uFlags);

        [Flags]
        public enum SoundFlags
        {
            /// <summary>play synchronously (default)</summary>
            SND_SYNC = 0x0000,
            /// <summary>play asynchronously</summary>
            SND_ASYNC = 0x0001,
            /// <summary>silence (!default) if sound not found</summary>
            SND_NODEFAULT = 0x0002,
            /// <summary>pszSound points to a memory file</summary>
            SND_MEMORY = 0x0004,
            /// <summary>loop the sound until next sndPlaySound</summary>
            SND_LOOP = 0x0008,
            /// <summary>don’t stop any currently playing sound</summary>
            SND_NOSTOP = 0x0010,
            /// <summary>Stop Playing Wave</summary>
            SND_PURGE = 0x40,
            /// <summary>don’t wait if the driver is busy</summary>
            SND_NOWAIT = 0x00002000,
            /// <summary>name is a registry alias</summary>
            SND_ALIAS = 0x00010000,
            /// <summary>alias is a predefined id</summary>
            SND_ALIAS_ID = 0x00110000,
            /// <summary>name is file name</summary>
            SND_FILENAME = 0x00020000,
            /// <summary>name is resource name or atom</summary>
            SND_RESOURCE = 0x00040004
        }

        public static void Play(string strFileName)
        {
            PlaySound(strFileName, UIntPtr.Zero,
               (uint)(SoundFlags.SND_FILENAME | SoundFlags.SND_SYNC | SoundFlags.SND_NOSTOP));
        }
        public static void mciPlay(string strFileName)
        {
            string playCommand = "open " + strFileName + " type WAVEAudio alias MyWav";
            mciSendString(playCommand, null, 0, IntPtr.Zero);
            mciSendString("play MyWav", null, 0, IntPtr.Zero);

        }
        public static void sndPlay(string strFileName)
        {
            sndPlaySound(strFileName, (long)SoundFlags.SND_SYNC);
        }
    }

}
