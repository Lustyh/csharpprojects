using Ini;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Clifford_FPY
{
    class Auto_Catch 
    {
        static IniFile Getiniinfo = new IniFile(System.Windows.Forms.Application.StartupPath + "\\Auto_Catch.ini");
        static IniFile Getiniinfo1 = new IniFile(System.Windows.Forms.Application.StartupPath + "\\Clifford_FPY.ini");
        public static string Server_FAIL_Item_Path = "", Server_FPY_Path = "";
        public static string D_N_str = "", File_name = "";
        //public int SN_Length = 0, SN_Position = 0;
        //public string Project_Type = "";
        

        public static void init()
        {
            Server_FAIL_Item_Path = Getiniinfo1.IniReadValue("PATH", "Server_Fail_Item");
            Server_FPY_Path = Getiniinfo1.IniReadValue("PATH", "Server_FPY_LOG");
        }
        public static bool check_auto()
        {
            string Auto_mode = Getiniinfo.IniReadValue("FA", "Auto_mode");
            //form1.Print_Error_Message(Auto_mode);        
            if (Auto_mode.ToUpper() == "TRUE")
            {
                return true;
            }
            return false;
        }
        public static string[] get_stations(string station)
        {
            return Getiniinfo.IniReadValue("Station", station).Split(':');
        }
        public static string[] get_project()
        {
            return Getiniinfo.IniReadValue("Projects", "Project").Split(':');
        }

        public static void Start_Catch()
        {
            while (true)
            {
                while (true)
                {
                    if (1 < Convert.ToInt32(DateTime.Now.ToString("mm")) && Convert.ToInt32(DateTime.Now.ToString("mm"))<30)
                    {
                        break;
                    }
                    for (int i = 0; i < 60; i++)
                    {
                        Thread.Sleep(1000);
                        Application.DoEvents();
                    }
                }
                string[] projects = get_project();
                foreach (string project in projects)
                {
                    string[] Stations = get_stations(project);
                    foreach (string station in Stations)
                    {
                        new Thread(new ThreadStart(() =>
                        {
                            try
                            {
                                Start_Run_FPY_auto(station, project);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        })).Start();
                    }
                }
                for (int i = 0; i < 1800; i++)
                {
                    Thread.Sleep(1000);
                    Application.DoEvents();
                }
            }
        }
        public static void Start_Run_FPY_auto(string station, string project)
        {
            string pro_sta_name = project + "_" + station;
            try
            {
                if (!Directory.Exists(Application.StartupPath + "\\FPY\\" + pro_sta_name))
                {
                    Directory.CreateDirectory(Application.StartupPath + "\\FPY\\" + pro_sta_name);
                }
                if (!Directory.Exists(Application.StartupPath + "\\Fail_Item\\" + pro_sta_name))
                {
                    Directory.CreateDirectory(Application.StartupPath + "\\Fail_Item\\" + pro_sta_name);
                }
                string[] Local_Fail_Item_File_names = Directory.GetFiles(Application.StartupPath + "\\Fail_Item\\" + pro_sta_name);
                string[] Local_FPY_File_names = Directory.GetFiles(Application.StartupPath + "\\FPY\\" + pro_sta_name);
                string[] Local_all_File_names = Directory.GetFiles(Application.StartupPath + "\\FPY\\" + pro_sta_name);
                foreach (var item in Local_Fail_Item_File_names)
                {
                    try
                    {
                        File.Delete(item);
                    }
                    catch (Exception)
                    {
                        //form1.Print_Error_Message("Delete Local Fail Item File Error");
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
                        //form1.Print_Error_Message("Delete Local FPY File Error");
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
                        //form1.Print_Error_Message("Delete Local Fail Item File Error");
                    }
                }
            }
            catch (Exception)
            {
                //form1.Print_Error_Message("Creat Directory ");
            }
            string distinguish_Project = Getiniinfo1.IniReadValue("Project", project);
            int SN_Length = Convert.ToInt32(distinguish_Project.Split(':')[0]);
            int SN_Position = Convert.ToInt32(distinguish_Project.Split(':')[1]);
            string Project_Type = distinguish_Project.Split(':')[2];
            int HH = Convert.ToInt32(DateTime.Now.ToString("HH"));
            if (HH> 8 && HH <= 15)
            { D_N_str = "D";  }
            else
            { D_N_str = "N"; File_name = DateTime.Now.ToString("yyyy-MM-dd"); }
            if (HH > 8 && HH <= 15)
            { File_name = DateTime.Now.ToString("yyyy-MM-dd");}
            else if (HH > 15)
            { File_name = DateTime.Now.ToString("yyyy-MM-dd");}
            else if (HH <= 8)
            { File_name = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");}
            string start_time_all = DateTime.Now.ToString("yyyy-MM-dd HH") + ":00:00";
            DateTime Select_End_Time = DateTime.Parse(start_time_all);
            DateTime Select_Start_Time = Select_End_Time.AddHours(-1); //DateTime.Parse(End_time_all);
            string Server_FAIL_Item_Path_Station = Server_FAIL_Item_Path + "\\" + station;
            string Server_FPY_Path_Station = Server_FPY_Path + "\\" + station;
            if (!Directory.Exists(Server_FAIL_Item_Path_Station))
            {
                //form1.Error_Message("Server Fail item path don't exist!"); form1.Reset_Report();
                return;
            }
            if (!Directory.Exists(Server_FPY_Path_Station))
            {
                //form1.Error_Message("Server FPT path don't exist!"); form1.Reset_Report();
                return;
            }


            string[] Fail_Item_File_names = Directory.GetFiles(Server_FAIL_Item_Path_Station);
            string[] FPY_File_names = Directory.GetFiles(Server_FPY_Path_Station);
            foreach (var FPY_File_name in FPY_File_names)
            {
                string File_name = FPY_File_name.Substring(FPY_File_name.LastIndexOf("\\") + 1, 10).Replace("_", "-");
                DateTime dt1 = DateTime.Parse(Select_Start_Time.ToString("yyyy-MM-dd"));//Select Start time
                DateTime dt2 = DateTime.Parse(Select_End_Time.ToString("yyyy-MM-dd")); //Select End time
                DateTime dt3 = DateTime.Parse(File_name); //File time
                TimeSpan ts1 = dt2.Subtract(dt3);
                TimeSpan ts2 = dt3.Subtract(dt1);
                //richTextBox2.AppendText(ts1.TotalDays.ToString());
                //richTextBox2.AppendText(ts2.TotalDays.ToString());
                if (ts1.TotalDays >= 0 && ts2.TotalDays >= 0)
                {
                    try
                    {
                        File.Copy(FPY_File_name, Application.StartupPath + "\\FPY\\" + pro_sta_name + "\\" + File_name + ".csv", true);
                    }
                    catch (Exception)
                    {
                        //form1.Print_Error_Message("Copy Server FPY File Error");
                    }
                }
            }
            foreach (var Fail_Item_File_name in Fail_Item_File_names)
            {
                try
                {
                    string File_name = Fail_Item_File_name.Substring(Fail_Item_File_name.LastIndexOf("\\") + 1, 10).Replace("_", "-");
                    DateTime dt1 = DateTime.Parse(Select_Start_Time.ToString("yyyy-MM-dd")); //start time
                    DateTime dt2 = DateTime.Parse(Select_End_Time.ToString("yyyy-MM-dd")); //end time
                    DateTime dt3 = DateTime.Parse(File_name);                                    //file time
                    TimeSpan ts1 = dt2.Subtract(dt3);
                    TimeSpan ts2 = dt3.Subtract(dt1);
                    if (ts1.TotalDays >= 0 && ts2.TotalDays >= -1)
                    {
                        try
                        {
                            File.Copy(Fail_Item_File_name, Application.StartupPath + "\\Fail_Item\\" + pro_sta_name + "\\" + File_name + ".csv", true);
                        }
                        catch (Exception)
                        {
                            //form1.Print_Error_Message("Copy Server Fail Item File Error");
                        }
                    }
                }
                catch (Exception)
                {
                    //form1.Print_Error_Message("Analyze Server Fail Item File Error");
                }
            }
            List<string> list_FPY = new List<string>();
            list_FPY.Clear();
            //string[] Local_Fail_Item_File_names2 = Directory.GetFiles(Application.StartupPath + "\\Fail_Item\\");
            string[] Local_FPY_File_names2 = Directory.GetFiles(Application.StartupPath + "\\FPY\\" + pro_sta_name);
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
                            string test_time = line[0].Replace("T", " ").Replace("Z", "");
                            string FPY_Code = line[4].Replace(" ", "");
                            DateTime dateTime_test = DateTime.Parse(test_time); //Test time

                            TimeSpan timeSpan_start_test = dateTime_test.Subtract(Select_Start_Time); //Select start time
                            TimeSpan timeSpan_end_test = Select_End_Time.Subtract(dateTime_test);

                            if (timeSpan_start_test.TotalSeconds >= 0 && timeSpan_end_test.TotalSeconds >= 0 && FPY_Code != "")
                            {
                                if (line[1].Length >= SN_Length)
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
                    //form1.Print_Error_Message("Find Local Fail Item File Error");
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
            if (list_FPY.Count == 0)
            {
                //form1.Print_Error_Message(station + " Station Don't Find SN\n"); form1.Reset_Report();
                return;
            }
            else
            {
                //form1.Print_Error_Message(station + " Station Find SN\n");
                //form1.Print_Error_Message(list_FPY.Count.ToString() + " Station Find SN\n");
                foreach (var item in list_FPY)
                {
                    //form1.Print_Error_Message(item.Split(',')[1] + " :" + station + "\n");
                }
                //form1.Print_Error_Message("************************\n");
            }
            start_find_auto(station, pro_sta_name, list_FPY);
        }
        public static void start_find_auto(string station,string pro_sta_name, List<string> list_FPY)
        {
            try
            {
                //string result_name = DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss");
                string Title = "Tata,D/N,Config,Test time,Line,Fixture,SN,Failure Mode,Failure Description,Retest,NTF,Fail,SOF,OFFLINE";
                //string station = comboBox2.Text;
                WriFAILitem(pro_sta_name, Title);
                //form1.Print_Error_Message("Start Find");
                //form1.Print_Error_Message("Total SN:" + list_FPY.Count);
                bool All_Fail_Item = false;
                /*if (checkBox4.Checked)
                {
                    All_Fail_Item = true;
                }*/
                //foreach (var str_FPY in list_FPY)
                //{
                //    form1.Print_Error_Message(str_FPY.Split(',')[1]);
                //}

                string[] Local_Fail_Item_File_names2 = Directory.GetFiles(Application.StartupPath + "\\Fail_Item\\" + pro_sta_name);
                foreach (var str_FPY in list_FPY)
                {
                    string[] FPY_SN = str_FPY.Split(',');
                    string SN = FPY_SN[1];
                    //form1.Print_Error_Message("Start find " + SN + "\n");
                    Application.DoEvents();
                    string FPYCODE = ",0,0,0,0,0,";
                    string D_N = D_N_str;
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
                    
                    string SKU = "";
                    if (FPY_SN.Length >= 6)
                    {
                        SKU = FPY_SN[5];
                    }
                    if (FPY_SN[4] == "SCOF_PASS")
                    {
                        D_N = D_N_str;
                        string line_id = "";
                        try
                        { line_id = FPY_SN[3].Split('_')[1]; }
                        catch (Exception)
                        {
                            line_id = FPY_SN[3];
                        }
                        content = FPY_SN[0].Substring(0, 10) + "," + D_N + "," + SKU + "," + FPY_SN[0] + "," + line_id + "," + FPY_SN[3] + "," + FPY_SN[1] + ",,";
                        WriFAILitem(pro_sta_name, content + FPYCODE, true);
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
                                if (line_content[4].Length > 32000)
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
                                D_N = D_N_str;
                                content = DATE + "," + D_N + "," + SKU + "," + line_content[0] + "," + line_content[1] + "," + line_content[2] + "," + line_content[3] + "," + line_content[4] + "," + line_content[5];
                                //richTextBox1.AppendText(content + "\n");
                                if (first)
                                {
                                    first = false;
                                    WriFAILitem(pro_sta_name, content + FPYCODE, false);
                                }
                                else
                                {
                                    if (false)
                                    {
                                        WriFAILitem(pro_sta_name, content + ",", false);
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
                    WriFAILitem(pro_sta_name, "");
                }
                string time = DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd");
                /*if (!Directory.Exists(Server_Report_Path + "\\" + station + "\\" + time))
                {
                    try
                    {
                        Directory.CreateDirectory(Server_Report_Path + "\\" + station + "\\" + time);
                    }
                    catch (Exception ee)
                    {
                        richTextBox1.AppendText("Create Directory Error:" + ee.Message);
                    }
                }*/

            }
            catch (Exception total)
            {
                //form1.Print_Error_Message("\nTotal Error:" + total.Message);
            }
            //if (!checkBox3.Checked)
            //{
                //form1.Reset_Report();
            //}
        }
        private static void WriFAILitem(string name, string content, bool enter = true)
        {
            try
            {
                //*******************************变量设置************************************************
                string WriLogPath2Ser = "";
                if (false)
                {
                    //WriLogPath2Ser = Application.StartupPath + "\\ALL_STATION\\" + Time_path;
                }
                else
                {
                    WriLogPath2Ser = Application.StartupPath + "\\Report\\"+ File_name;
                }

                //*******************************变量设置************************************************
                //*************************确认是否有对应的文件夹******************************************
                if (Directory.Exists(@WriLogPath2Ser) == false)//如果不存在就创建file文件夹
                {
                    Directory.CreateDirectory(@WriLogPath2Ser);
                }
                //**************************************************************************************
                if (content.Contains("Retest,NTF,Fail,SOF,OFFLINE"))
                {
                    if (File.Exists(WriLogPath2Ser + "\\" + name + "_" + File_name + "_" + D_N_str + ".csv"))
                    {
                        return;
                    }
                }
                StreamWriter WriteLog2Ser = new StreamWriter(WriLogPath2Ser + "\\" + name + "_" + File_name + "_" + D_N_str + ".csv", true);
                WriteLog2Ser.AutoFlush = true;

                WriteLog2Ser.Write(content);
                if (enter)
                {
                    WriteLog2Ser.Write("\r\n");
                }
                WriteLog2Ser.Flush(); //*******关闭****************
                WriteLog2Ser.Close(); //*******关闭****************
                //Open_file_path = WriLogPath2Ser + "\\" + name + ".csv";
            }
            catch (Exception)
            {
            }
        }
    }
}
