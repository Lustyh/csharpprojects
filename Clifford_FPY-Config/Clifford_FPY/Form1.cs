using Ini;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Clifford_FPY
{
    public partial class Form1 : Form
    {
        IniFile Getiniinfo1 = new IniFile(System.Windows.Forms.Application.StartupPath + "\\Clifford_FPY.ini");
        public string Server_FAIL_Item_Path = "", Server_FPY_Path = "", Server_Report_Path="", Server_FAIL_Item_Path_Station = "", Server_FPY_Path_Station = "";
        public DateTime Select_Start_Time, Select_End_Time;
        public string Time_path = "";
        public int SN_Length = 0, SN_Position = 0;
        public string Project_Type = "";
        public string DBserver = "10.18.6.53", DBname = "QMS", DBType = "MonitorPortal_SEL2";
        public string Open_file_path = "", pro_sta = "";
        public static string[] get_sn;

        public List<string> list_FPY { get; set; } = new List<string>();
        public List<string> DUT_SNs { get; set; } = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        Thread Test_p;
        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;
            pro_sta = comboBox1.Text + "_" + comboBox2.Text;
            if (Getiniinfo1.IniReadValue("Projects", "Quanta").Contains(comboBox1.Text))
            {
                Ini_Report();

                Test_p = new Thread(new ThreadStart(Start_Run));
                Test_p.Start();
            }
            else
            {
                if (checkBox3.Checked)
                {
                    string[] stations = Getiniinfo1.IniReadValue("Station", comboBox1.Text).Split(':');
                    Time_path = DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss");
                    bool fist_clear = true;
                    new Thread(new ThreadStart(() =>
                    {
                        foreach (var item in stations)
                        {
                            if (fist_clear) { Ini_Report(); } else { Ini_Report(false); }
                            fist_clear = false;
                            Application.DoEvents();
                            comboBox2.Text = item;
                            Start_Run_FPY();
                        //Test_p.Start();
                    }
                        combine_log();
                    })).Start();

                }
                else
                {
                    Ini_Report();
                    Test_p = new Thread(new ThreadStart(Start_Run_FPY));
                    Test_p.Start();
                }
            }
            /*DateTime dt1 = DateTime.Parse(dateTimePicker1.Value.ToString("yyyy-MM-dd"));
            DateTime dt2 = DateTime.Parse(dateTimePicker1.Value.ToString("yyyy_MM_dd"));
            TimeSpan ts = dt2.Subtract(dt1);

            MessageBox.Show(ts.ToString());
            string aaa = @"‪C:\Users\Simple\Desktop\h.txt";
            MessageBox.Show(aaa.Substring(aaa.LastIndexOf("\\") + 1));
            //dateTimePicker1.Format = DateTimePickerFormat.Custom;
            //dateTimePicker1.CustomFormat = "yyyy-MM-dd-HH-mm-ss";

            string date = dateTimePicker1.Value.ToString("yyyy-MM-ddTHH:mm:ssZ");
            MessageBox.Show(date);
            */
        }
        public void combine_log()
        {
            string[] all_logs = Directory.GetFiles(Application.StartupPath + "\\ALL_STATION\\" + Time_path);
            string result_name = DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss");
            string Title = "Station,Tata,D/N,Config,Test time,Line,Fixture,SN,Failure Mode,Failure Description,Retest,NTF,Fail,SCOF";
            
            WriFAILitem("All_Station-" + result_name, Title);
            foreach (var LOG in all_logs)
            {
                string file_name = LOG.Substring(LOG.LastIndexOf("\\") + 1);
                richTextBox2.AppendText(file_name + "\n");
                string station_mark = file_name.Substring(0,file_name.IndexOf("_"));
                string logs = File.ReadAllText(LOG);
                string[] lines = logs.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                for (int i = 1; i < lines.Length; i++)
                {
                    if (lines[i].Split(',')[0] != "")
                    {
                        WriFAILitem("All_Station-" + result_name, station_mark + "," + lines[i] + ",");
                    }
                }
                WriFAILitem("All_Station-" + result_name, "*********,************,************,********");
                WriFAILitem("All_Station-" + result_name, "*********,************,************,********");
                WriFAILitem("All_Station-" + result_name, "*********,************,************,********");
                WriFAILitem("All_Station-" + result_name, "*********,************,************,********");
                WriFAILitem("All_Station-" + result_name, "*********,************,************,********");
            }
            Reset_Report();
        }
        public void Ini_Report(bool a=true)
        {
            try
            {
                if (a)
                {
                    richTextBox1.Clear();
                    richTextBox2.Clear();
                }
                comboBox1.Enabled = false;
                comboBox2.Enabled = false;
                comboBox3.Enabled = false;
                button1.Enabled = false;
                button2.Enabled = false;
                try
                {
                    if (!Directory.Exists(Application.StartupPath + "\\FPY\\" + pro_sta))
                    {
                        Directory.CreateDirectory(Application.StartupPath + "\\FPY\\" + pro_sta);
                    }
                    if (!Directory.Exists(Application.StartupPath + "\\Fail_Item\\" + pro_sta))
                    {
                        Directory.CreateDirectory(Application.StartupPath + "\\Fail_Item\\" + pro_sta);
                    }
                }
                catch (Exception)
                {
                    Print_Error_Message("Creat Directory ");
                }
                string[] Local_Fail_Item_File_names = Directory.GetFiles(Application.StartupPath + "\\Fail_Item\\" + pro_sta);
                string[] Local_FPY_File_names = Directory.GetFiles(Application.StartupPath + "\\FPY\\" + pro_sta);
                string[] Local_all_File_names = Directory.GetFiles(Application.StartupPath + "\\FPY\\" + pro_sta);
                foreach (var item in Local_Fail_Item_File_names)
                {
                    try
                    {
                        File.Delete(item);
                    }
                    catch (Exception)
                    {
                        Print_Error_Message("Delete Local Fail Item File Error");
                    }
                }
                foreach (var item in Local_FPY_File_names)
                {
                    try
                    {
                        File.Delete(item);
                    }
                    catch (Exception)
                    {
                        Print_Error_Message("Delete Local FPY File Error");
                    }
                }
                foreach (var item in Local_all_File_names)
                {
                    try
                    {
                        File.Delete(item);
                    }
                    catch (Exception)
                    {
                        Print_Error_Message("Delete Local Fail Item File Error");
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void Reset_Report()
        {
            try
            {
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
                pictureBox1.Visible = false;
            }
            catch (Exception)
            {
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string project = comboBox1.Text;
            richTextBox1.AppendText(project);
            string station = Getiniinfo1.IniReadValue("Station", project);
            richTextBox1.AppendText("\n" + station);
            comboBox2.DataSource = null;
            comboBox2.Items.Clear();
            comboBox2.Text = station.Split(':')[0];
            comboBox2.Items.AddRange(station.Split(':'));
            string distinguish_Project = Getiniinfo1.IniReadValue("Project", project);
            string SEL_2 = Getiniinfo1.IniReadValue("FA", "SEL2");
            if (SEL_2.ToUpper().Contains(project.ToUpper()))
            {
                DBname = "QMS";
                DBserver = "10.18.6.53";
                DBType = "MonitorPortal_SEL2";
            }
            else
            {
                DBname = "QMS";
                DBserver = "10.18.6.52";
                DBType = "MonitorPortal";
            }
            try
            {
                SN_Length = Convert.ToInt32(distinguish_Project.Split(':')[0]);
                SN_Position = Convert.ToInt32(distinguish_Project.Split(':')[1]);
                Project_Type = distinguish_Project.Split(':')[2];
                richTextBox1.AppendText(SN_Length.ToString() + "\n");
                richTextBox1.AppendText(SN_Position.ToString() + "\n");
                richTextBox1.AppendText(Project_Type.ToString() + "\n");
            }
            catch (Exception)
            {
                Print_Error_Message("Convert SN_Length SN_Position Project_Type Error");
            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox2.Checked = false;
            }
            else
            {
                checkBox2.Checked = true;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                checkBox1.Checked = false;
            }
            else
            {
                checkBox1.Checked = true;
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                richTextBox1.Select(richTextBox1.Text.Length, 0);
                richTextBox1.ScrollToCaret();
            }
            catch (Exception sd) { richTextBox1.AppendText("\n" + sd.Message); }
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                richTextBox2.Select(richTextBox2.Text.Length, 0);
                richTextBox2.ScrollToCaret();
            }
            catch (Exception sd) { richTextBox2.AppendText("\n" + sd.Message); }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Open_file_path);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            button2.Enabled = false;
            if (Application.StartupPath.Contains(@"\\"))
            {
                MessageBox.Show("请不要直接从server上直接打开程序\n请点击Loader文件夹下的快捷方式");
                this.Close();
            }
            Server_FAIL_Item_Path = Getiniinfo1.IniReadValue("PATH", "Server_Fail_Item");
            Server_FPY_Path = Getiniinfo1.IniReadValue("PATH", "Server_FPY_LOG");
            Server_Report_Path = Getiniinfo1.IniReadValue("PATH", "Server_Yield_Report_LOG");
            if (!Directory.Exists(Application.StartupPath + "\\Fail_Item"))
            {
                Directory.CreateDirectory(Application.StartupPath + "\\Fail_Item");
            }
            if (!Directory.Exists(Application.StartupPath + "\\FPY"))
            {
                Directory.CreateDirectory(Application.StartupPath + "\\FPY");
            }
            if (!Directory.Exists(Application.StartupPath + "\\ALL_STATION\\"))
            {
                Directory.CreateDirectory(Application.StartupPath + "\\ALL_STATION\\");
            }
            if (!Directory.Exists(Application.StartupPath + "\\Report\\"))
            {
                Directory.CreateDirectory(Application.StartupPath + "\\Report\\");
            }

            string project = Getiniinfo1.IniReadValue("Projects", "Project");
            comboBox1.Items.AddRange(project.Split(':'));
            try
            {
                string distinguish_Project = Getiniinfo1.IniReadValue("Project", comboBox1.Text);
                SN_Length = Convert.ToInt32(distinguish_Project.Split(':')[0]);
                SN_Position = Convert.ToInt32(distinguish_Project.Split(':')[1]);
                Project_Type = distinguish_Project.Split(':')[2];
            }
            catch (Exception)
            {
                Print_Error_Message("Load Convert SN_Length SN_Position Project_Type Error");
            }
            if (File.Exists(Application.StartupPath + "\\Auto_Catch.ini"))
            {
                new Thread(new ThreadStart(() =>
                {
                    if (Auto_Catch.check_auto())
                    {
                        Auto_Catch.init();
                        Auto_Catch.Start_Catch();
                    }
                })).Start();
            }

        }
        public void Start_Run_FPY()
        {
            try
            {
                string start_time_yy = dateTimePicker1.Value.ToString("yyyy-MM-dd");
                string start_time_hh = textBox1.Text;
                string start_time_all = start_time_yy + " " + start_time_hh.Substring(0, 2) + ":" + start_time_hh.Substring(3, 2) + ":00";
                Select_Start_Time = DateTime.Parse(start_time_all);

                string end_time_yy = dateTimePicker2.Value.ToString("yyyy-MM-dd");
                string end_time_hh = textBox2.Text;
                string End_time_all = end_time_yy + " " + end_time_hh.Substring(0, 2) + ":" + end_time_hh.Substring(3, 2) + ":00";
                Select_End_Time = DateTime.Parse(End_time_all);
            }
            catch (Exception)
            {
                Error_Message("Please enter the corrent time!");
                Reset_Report();
                return;
            }
            Server_FAIL_Item_Path_Station = Server_FAIL_Item_Path + "\\" + comboBox2.Text;
            Server_FPY_Path_Station = Server_FPY_Path + "\\" + comboBox2.Text;
            if (!Directory.Exists(Server_FAIL_Item_Path_Station))
            {
                Error_Message("Server Fail item path don't exist!"); Reset_Report();
                return;
            }
            if (!Directory.Exists(Server_FPY_Path_Station))
            {
                Error_Message("Server FPT path don't exist!"); Reset_Report();
                return;
            }
            

            string[] Fail_Item_File_names = Directory.GetFiles(Server_FAIL_Item_Path_Station);
            string[] FPY_File_names = Directory.GetFiles(Server_FPY_Path_Station);
            foreach (var FPY_File_name in FPY_File_names)
            {
                string File_name = FPY_File_name.Substring(FPY_File_name.LastIndexOf("\\") + 1, 10).Replace("_","-");
                DateTime dt1 = DateTime.Parse(dateTimePicker1.Value.ToString("yyyy-MM-dd"));//Select Start time
                DateTime dt2 = DateTime.Parse(dateTimePicker2.Value.ToString("yyyy-MM-dd")); //Select End time
                DateTime dt3 = DateTime.Parse(File_name); //File time
                TimeSpan ts1 = dt2.Subtract(dt3);
                TimeSpan ts2 = dt3.Subtract(dt1);
                //richTextBox2.AppendText(ts1.TotalDays.ToString());
                //richTextBox2.AppendText(ts2.TotalDays.ToString());
                if (ts1.TotalDays >= 0 && ts2.TotalDays >= 0)
                {
                    try
                    {
                        File.Copy(FPY_File_name, Application.StartupPath + "\\FPY\\" + pro_sta + "\\" + File_name + ".csv",true);
                    }
                    catch (Exception)
                    {
                        Print_Error_Message("Copy Server FPY File Error");
                    }
                }
            }
            foreach (var Fail_Item_File_name in Fail_Item_File_names)
            {
                try
                {
                    string File_name = Fail_Item_File_name.Substring(Fail_Item_File_name.LastIndexOf("\\") + 1, 10).Replace("_", "-");
                    DateTime dt1 = DateTime.Parse(dateTimePicker1.Value.ToString("yyyy-MM-dd")); //start time
                    DateTime dt2 = DateTime.Parse(dateTimePicker2.Value.ToString("yyyy-MM-dd")); //end time
                    DateTime dt3 = DateTime.Parse(File_name);                                    //file time
                    TimeSpan ts1 = dt2.Subtract(dt3);
                    TimeSpan ts2 = dt3.Subtract(dt1);
                    if (ts1.TotalDays >= 0 && ts2.TotalDays >= -1)
                    {
                        try
                        {
                            File.Copy(Fail_Item_File_name, Application.StartupPath + "\\Fail_Item\\" + pro_sta + "\\" + File_name + ".csv",true);
                        }
                        catch (Exception)
                        {
                            Print_Error_Message("Copy Server Fail Item File Error");
                        }
                    }
                }
                catch (Exception)
                {
                    Print_Error_Message("Analyze Server Fail Item File Error");
                }
            }
            list_FPY.Clear();
            //string[] Local_Fail_Item_File_names2 = Directory.GetFiles(Application.StartupPath + "\\Fail_Item\\");
            string[] Local_FPY_File_names2 = Directory.GetFiles(Application.StartupPath + "\\FPY\\" + pro_sta);
            foreach (var item in Local_FPY_File_names2)
            {
                try
                {
                    string[] Content_Lines = File.ReadAllLines(item);
                    foreach (var Line in Content_Lines)
                    {
                        string[] line = Line.Split(',');
                        if (line.Length >= 5)
                        {
                            string test_time = line[0].Replace("T"," ").Replace("Z", "");
                            string FPY_Code = line[4].Replace(" ","");
                            DateTime dateTime_test = DateTime.Parse(test_time); //Test time
                            
                            TimeSpan timeSpan_start_test = dateTime_test.Subtract(Select_Start_Time); //Select start time
                            TimeSpan timeSpan_end_test = Select_End_Time.Subtract(dateTime_test);
                            
                            if (timeSpan_start_test.TotalSeconds >= 0 && timeSpan_end_test.TotalSeconds >= 0 && FPY_Code != "")
                            {
                                if (line[1].Length>=SN_Length)
                                {
                                    int Project_len = 1;
                                    if (Project_Type.Contains("&"))
                                    { Project_len = Project_Type.Split('&')[0].Length; }
                                    else
                                    { Project_len = Project_Type.Length; }
                                    if (Project_Type.ToUpper().Contains(line[1].Substring(SN_Position - 1, Project_len).ToUpper()))
                                    {
                                        list_FPY.Add(Line);
                                        //richTextBox1.AppendText(item);
                                        //richTextBox1.AppendText(Line);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    Print_Error_Message("Find Local Fail Item File Error");
                }
            }
            /*foreach (var FPY_CORD in list_FPY)
            {
                try
                {
                    File.Delete(FPY_CORD);
                }
                catch (Exception)
                {
                    Print_Error_Message("Delete Local FPY File Error");
                }
            }*/
            if (list_FPY.Count==0)
            {
                richTextBox1.AppendText(comboBox2.Text + " Station Don't Find SN\n");Reset_Report();
                return;
            }
            else
            {
                richTextBox2.AppendText(comboBox2.Text + " Station Find SN\n");
                richTextBox2.AppendText(list_FPY.Count.ToString() + " Station Find SN\n");
                foreach (var item in list_FPY)
                {
                    richTextBox2.AppendText(item.Split(',')[1] +" :"+ comboBox2.Text + "\n");
                }
                richTextBox2.AppendText( "************************\n");
            }
            start_find();
        }
        public string  get_D_N(string time)
        {
            
            return "";
        }
        public void start_find()
        {
            try
            {
                string result_name = DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss");
                string Title = "Tata,D/N,Config,Test time,Line,Fixture,SN,Failure Mode,Failure Description,Retest,NTF,Fail,SOF,OFFLINE";
                string station = comboBox2.Text;
                WriFAILitem(station + "_" + result_name, Title);
                Print_Error_Message("Start Find");
                Print_Error_Message("Total SN:" + list_FPY.Count);
                bool All_Fail_Item = false;
                if (checkBox4.Checked)
                {
                    All_Fail_Item = true;
                }
                foreach (var str_FPY in list_FPY)
                {
                    Print_Error_Message(str_FPY.Split(',')[1]);
                }

                string[] Local_Fail_Item_File_names2 = Directory.GetFiles(Application.StartupPath + "\\Fail_Item\\" + pro_sta);
                foreach (var str_FPY in list_FPY)
                {
                    string[] FPY_SN = str_FPY.Split(',');
                    string SN = FPY_SN[1];
                    richTextBox1.AppendText("Start find " +SN+ "\n");
                    Application.DoEvents();
                    string FPYCODE = ",0,0,0,0,0,";
                    string D_N = "D";
                    string DATE = "";
                    string content = "";
                    switch (FPY_SN[4])
                    {
                        case "RETEST":
                            FPYCODE = ",1,0,0,0,0,";
                            break;
                        case "NTF":
                            FPYCODE = ",0,1,0,0,0,";
                            break;
                        case "FAIL":
                            FPYCODE = ",0,0,1,0,0,";
                            break;
                        case "SCOF_PASS":
                            FPYCODE = ",0,0,0,1,0,";
                            break;
                        case "OFFLINE":
                            FPYCODE = ",0,0,0,0,1,";
                            break;
                        default:
                            continue;
                    }
                    DUT_SNs.Add(SN);
                    string SKU = "";
                    if (FPY_SN.Length>=6)
                    {
                        SKU = FPY_SN[5];
                    }
                    if (FPY_SN[4] == "SCOF_PASS")
                    {
                        if (checkBox1.Checked) { D_N = "D"; }
                        if (checkBox2.Checked) { D_N = "N"; }
                        string line_id = "";
                        try
                        { line_id = FPY_SN[3].Split('_')[1]; }
                        catch (Exception)
                        {
                            line_id = FPY_SN[3];
                        }
                        content = FPY_SN[0].Substring(0, 10) + "," + D_N + "," + SKU + "," + FPY_SN[0] + "," + line_id + "," + FPY_SN[3] + "," + FPY_SN[1] + ",,";
                        WriFAILitem(station + "_" + result_name, content + FPYCODE, true);
                        continue;
                    }
                    bool next_sn = false;
                    bool first = true;
                    foreach (var Local_Fail_Item_File in Local_Fail_Item_File_names2)
                    {
                        Application.DoEvents();
                        string lines_a = File.ReadAllText(Local_Fail_Item_File);
                        string[] lines = lines_a.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                        foreach (var line in lines)
                        {
                            Application.DoEvents();
                            string[] line_content = line.Split(',');
                            if (line.Contains(SN) && line_content.Length >= 6)
                            {
                                if (line_content[4].Length>32000)
                                {
                                    line_content[4] = line_content[4].Substring(0, 32000) + "\"";
                                }
                                else
                                {
                                    line_content[4] = line_content[4];
                                }
                                if (line_content[5].Length > 32000)
                                {
                                    line_content[5] = line_content[5].Substring(0, 32000) + "\"";
                                }
                                else
                                {
                                    line_content[5] = line_content[5];
                                }
                                if (FPY_SN[4] == "OFFLINE")
                                {
                                    line_content[4] = "device offline";
                                    line_content[5] = "device offline YES";
                                }
                                DATE = line_content[0].Substring(0, 10);
                                if (checkBox1.Checked) { D_N = "D"; }
                                if (checkBox2.Checked) { D_N = "N"; }
                                content = DATE + "," + D_N + "," + SKU + "," + line_content[0] + "," + line_content[1] + "," + line_content[2] + "," + line_content[3] + "," + line_content[4] + "," + line_content[5];
                                //richTextBox1.AppendText(content + "\n");
                                if (first)
                                {
                                    first = false;
                                    WriFAILitem(station + "_" + result_name, content + FPYCODE, false);
                                }
                                else
                                {
                                    if (checkBox4.Checked)
                                    {
                                        WriFAILitem(station + "_" + result_name, content + ",", false);
                                    }
                                }
                                if (!All_Fail_Item)
                                {
                                    next_sn = true;
                                    continue;
                                }
                            }
                        }
                        if (next_sn)
                        {
                            continue;
                        }
                    }
                    WriFAILitem(station + "_" + result_name, "");
                }
                string time = DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd");
                if (!Directory.Exists(Server_Report_Path + "\\" + station + "\\" + time))
                {
                    try
                    {
                        Directory.CreateDirectory(Server_Report_Path + "\\" + station + "\\" + time);
                    }
                    catch (Exception ee)
                    {
                        richTextBox1.AppendText("Create Directory Error:" + ee.Message);
                    }
                }
                /*if (File.Exists(Application.StartupPath + "\\Report\\" + station + "_" + result_name + ".csv"))
                {
                    try
                    {
                        File.Copy(Application.StartupPath + "\\Report\\" + station + "_" + result_name + ".csv", Server_Report_Path + "\\" + station + "\\" + time + "\\" + station + "_" + result_name + ".csv");
                    }
                    catch (Exception ww)
                    {
                        richTextBox1.AppendText("\nCopy Report to Server Error:" + ww.Message);
                    }
                    string results = File.ReadAllText(Application.StartupPath + "\\Report\\" + station + "_" + result_name + ".csv");
                    foreach (var sn in DUT_SNs)
                    {
                        if (!results.Contains(sn))
                        {
                            richTextBox2.AppendText(station+"未找到SN:" + sn + "请手动查看log确认\n");
                        }
                    }
                }*/
                
            }
            catch (Exception total)
            {
                richTextBox1.AppendText("\nTotal Error:" + total.Message);
            }
            if (!checkBox3.Checked)
            {
                Reset_Report();
            }
        }

        public void Print_Error_Message(string message)
        {
            richTextBox1.AppendText(message + "\n");
        }
        private string Querydata(string station, string step, string Inputstring)//同SF交换过站信息
        {
            //richTextBox1.AppendText("\n" + station + "\n" + step + "\n" + Inputstring + "\n" + DBserver + "\n" + DBname);

            try
            {
                richTextBox1.AppendText(DateTime.Now.ToString("yyyyMMddHHmmss"));
                SqlConnection a = new SqlConnection();
                a.ConnectionString = "server="+DBserver+";Database=" + DBname + ";Uid=sdt;Pwd=SDT#7;Integrated Security=False";
                try
                { a.Open(); }
                catch { label1.Text = "link DBfail"; }
                if (a.State == ConnectionState.Open)
                {
                    SqlCommand b = new SqlCommand(DBType, a);
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
                    string temp1 = ss.Replace(";$;", "*");
                    string[] temp2 = temp1.Split('*');
                    return ss;

                }
                richTextBox1.AppendText(DateTime.Now.ToString("yyyyMMddHHmmss"));
                return "连接数据服务器出现异常2";
            }
            catch (Exception ee)
            {
                richTextBox2.AppendText(DateTime.Now.ToString("yyyyMMddHHmmss\n") + ee.Message);
                return "连接数据服务器出现异常";
            }
        }
        private void WriFAILitem(string name, string content, bool enter=true)
        {
            try
            {
                //*******************************变量设置************************************************
                string WriLogPath2Ser = "";
                if (checkBox3.Checked)
                {
                    WriLogPath2Ser = Application.StartupPath + "\\ALL_STATION\\"+ Time_path;
                }
                else
                {
                    WriLogPath2Ser = Application.StartupPath + "\\Report\\";
                }
                        
                //*******************************变量设置************************************************
                //*************************确认是否有对应的文件夹******************************************
                if (Directory.Exists(@WriLogPath2Ser) == false)//如果不存在就创建file文件夹
                {
                    Directory.CreateDirectory(@WriLogPath2Ser);
                }
                //**************************************************************************************

                StreamWriter WriteLog2Ser = new StreamWriter(WriLogPath2Ser + "\\" + name + ".csv", true);
                WriteLog2Ser.AutoFlush = true;
                
                WriteLog2Ser.Write(content);
                if (enter)
                {
                    WriteLog2Ser.Write("\r\n");
                }
                WriteLog2Ser.Flush(); //*******关闭****************
                WriteLog2Ser.Close(); //*******关闭****************
                Open_file_path = WriLogPath2Ser + "\\" + name + ".csv";
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
                Get_3 = str_sample.ToUpper();
                Get_n_4 = Get_3.IndexOf(Get_1);
                Get_n_5 = Get_3.IndexOf(Get_2);
                if (Get_n_4 > -1)
                { }

                if (Get_n_5 > -1) { } else { }

                Get_4 = Get_3.Substring(Get_3.ToUpper().IndexOf(Get_1));

                Get_n_1 = Get_1.Length;
                Get_n_2 = Get_2.Length;
                Get_n_3 = Get_4.ToUpper().IndexOf(Get_2);
                Get_5 = Get_3.Substring(Get_3.ToUpper().IndexOf(Get_1) + Get_n_1, Get_n_3 - Get_n_1);
                return Get_5;

            }
            catch (Exception)
            {
                return "";
            }
        }
        public void Start_Run()
        {
            get_sn = null;
            Get_SN a = new Get_SN();
            a.Show();
            while (get_sn == null)
            {
                Thread.Sleep(200);
                Application.DoEvents();
            }
            foreach (var item in get_sn)
            {
                richTextBox1.AppendText(item + "\n");
            }
            richTextBox2.AppendText( "Total SN: "+get_sn.Length.ToString()+"\n");
            string path = "";
            if (comboBox1.Text == "Joplin")
            {
                path = Getiniinfo1.IniReadValue("Special_Path", "Joplin");
                //path = @"\\10.18.6.47\sel_monitor\Joplin_Test_Logs\Joplin-fail-log";
                if (comboBox2.Text == "SUB_LED")
                {
                    path = Getiniinfo1.IniReadValue("Special_Path", "Joplin_LED");
                    //path = @"\\10.18.6.47\sel_monitor\Joplin_Test_Logs\\Joplin_Process_log\\Joplin-fail-log";
                }
            }
            else if (comboBox1.Text == "Caprica")
            {
                path = Getiniinfo1.IniReadValue("Special_Path", "Caprica");
                if (comboBox2.Text == "FLASH_IMAGE")
                {
                    path = Getiniinfo1.IniReadValue("Special_Path", "Caprica_FLASH_IMAGE");
                    
                    DateTime dt1 = DateTime.Parse(dateTimePicker1.Value.ToString("yyyy-MM-dd")); //start time
                    DateTime dt2 = DateTime.Parse(dateTimePicker2.Value.ToString("yyyy-MM-dd")); //end time
                    StreamWriter streamWriter1 = new StreamWriter(Application.StartupPath + "\\Report\\" + comboBox3.Text + "-" + comboBox2.Text + "-" + DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss") + ".csv");
                    streamWriter1.WriteLine("SN,LINE,FIXTURE,FAIL_ITEM,DESCRIPTION,TIME");
                    foreach (var DUT_SN in get_sn)
                    {
                        if (DUT_SN =="")
                        {
                            continue;
                        }
                        string line_id = "";
                        string fixture_id = "";
                        string fail_item = "Can not get boot up image";
                        string des_fail = "Can not get boot up image";
                        TimeSpan ts1 = dt2.Subtract(dt1);
                        int aaas = (dt2 - dt1).Days;
                        bool Alread_find = false;
                        for (int i = 0; i <= aaas; i++)
                        {
                            Application.DoEvents();
                            int YEAR = dt2.AddDays(-i).Year;
                            int MONTH = dt2.AddDays(-i).Month;
                            int DAY = dt2.AddDays(-i).Day;
                            string ser_log_path = path + "\\" + YEAR.ToString("0000") + "\\" + MONTH.ToString("00") + "\\" + DAY.ToString("00");
                            richTextBox1.AppendText(ser_log_path + "\n");
                            if (Directory.Exists(ser_log_path))
                            {
                                Application.DoEvents();
                                string[] files_log = Directory.GetFiles(ser_log_path);
                                foreach (string file_log in files_log)
                                {
                                    if (file_log.Contains(DUT_SN))
                                    {
                                        try
                                        {
                                            string file_name = file_log.Substring(file_log.LastIndexOf("\\"));
                                            string[] names_file = file_name.Split('_');
                                            fixture_id = names_file[1];
                                            line_id = fixture_id.Split('-')[1];
                                        }
                                        catch (Exception)
                                        {
                                        }
                                        Alread_find = true;
                                        break;
                                    }
                                }
                                if (Alread_find)
                                {
                                    break;
                                }
                            }
                        }
                        if (line_id == "")
                        {
                            fail_item = des_fail = "";
                        }
                        streamWriter1.WriteLine(DUT_SN + "," + line_id + "," + fixture_id + ",\"" + fail_item + "\",\"" + des_fail + "\",");
                    }
                    streamWriter1.Close();
                    Reset_Report();
                    return;
                }
                //path = @"\\10.18.6.49\SEL_Monitor\Caprica_Test_Logs\Caprica_Fail_Log";
            }
            else
            {
                MessageBox.Show("Please check Project");
            }
            path = path + "\\" + comboBox2.Text;
            if (!Directory.Exists(path))
            {
                MessageBox.Show("请检查网络");
            }
            richTextBox1.AppendText(path + "\n");
            StreamWriter streamWriter = new StreamWriter(Application.StartupPath + "\\Report\\" + comboBox3.Text + "-" + comboBox2.Text + "-" + DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss") + ".csv");
            streamWriter.WriteLine("SN,LINE,FIXTURE,FAIL_ITEM,DESCRIPTION,TIME");
            string[] files = Directory.GetFiles(path);
            int aaaa = 1;
            foreach (var SN in get_sn)
            {
                try
                {
                    richTextBox2.AppendText(SN.ToString() + "  " + aaaa + "\n");
                    aaaa++;
                    if (SN.Length < 5)
                    {
                        continue;
                    }
                    Application.DoEvents();
                    string item = "";
                    string des = "";
                    string time = "";
                    string FIX = "";
                    string SF_line = "";
                    for (int i = 0; i < files.Length; i++)
                    {
                        Application.DoEvents();
                        if (files[i].Contains(SN))
                        {
                            time = File.GetCreationTime(files[i]).ToString("yyyy_MM_dd-HH_mm_ss");
                            string[] lines = File.ReadAllLines(files[i]);
                            foreach (var line in lines)
                            {
                                Application.DoEvents();
                                string[] danyuange = line.Split(',');
                                if (danyuange.Length >= 4)
                                {
                                    if (comboBox2.Text == "SUB_LED")
                                    {
                                        if (danyuange[3].ToUpper().Contains("FAIL"))
                                        {
                                            //richTextBox1.AppendText(danyuange[0]);
                                            string file_name = Path.GetFileName(files[i]);
                                            FIX = file_name.Substring(0, file_name.IndexOf("_"));
                                            SF_line = FIX.Substring(2, 3);
                                            item += danyuange[0].Replace("\"", "") + "\n";
                                            des += danyuange[0].Replace("\"", "") + "  " + danyuange[5].Replace("\"", "") + "\n";
                                        }

                                    }
                                    else if (comboBox2.Text == "BFT")
                                    {
                                        if (danyuange[1].ToUpper().Contains("FAIL"))
                                        {
                                            //richTextBox1.AppendText(danyuange[0]);
                                            item += danyuange[0].Replace("\"", "") + "\n";
                                            des += danyuange[0].Replace("\"", "") + "  " + danyuange[2].Replace("\"", "") + "\n";
                                        }
                                    }
                                    else
                                    {
                                        if (danyuange[2].ToUpper().Contains("FAIL"))
                                        {
                                            //richTextBox1.AppendText(danyuange[1]);
                                            item += danyuange[1].Replace("\"", "") + "\n";
                                            des += danyuange[1].Replace("\"", "") + "  " + danyuange[3].Replace("\"", "") + "\n";
                                        }
                                    }
                                }
                                else
                                {
                                    if (line.Contains("QDW_Fixture_Name="))
                                    {
                                        FIX = line.Substring(17);
                                    }
                                    if (line.Contains("QDW_Line="))
                                    {
                                        SF_line = line.Substring(9);
                                    }
                                }
                            }
                        }
                        if (FIX != "")
                        { break; }
                    }
                    streamWriter.WriteLine(SN + "," + SF_line + "," + FIX + ",\"" + item + "\",\"" + des + "\"," + time);
                }
                catch (Exception ex)
                {
                    richTextBox1.AppendText(ex.Message);
                }
            }
            streamWriter.Close();
            Reset_Report();
        }
        public void Error_Message(string Error_message)
        {
            textBox3.Text = Error_message;
        }
    }
}
