using CsvHelper;
using MiniExcelLibs;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SamsaraSFTP
{
    internal class Program
    {
        static void Upload(string filename,string dest)
        {
            var keyFile = new PrivateKeyFile("./samsara_ftp_key.ppk");
            var keyFiles = new[] { keyFile };
            var username = "samsaraftp";

            var methods = new List<AuthenticationMethod>();
            //methods.Add(new PasswordAuthenticationMethod(username, "samsara_ftp_key"));
            methods.Add(new PrivateKeyAuthenticationMethod(username, keyFiles));

            var con = new ConnectionInfo("s-09b9f0b38ea74363a.server.transfer.us-west-1.amazonaws.com", 22, username, methods.ToArray());
            string name = new FileInfo(filename).Name;
            using (var client = new SftpClient(con))
            {
                client.Connect();
                using (var r = new FileStream(filename,FileMode.Open))
                {
                    Console.WriteLine($"Uploading To:{dest + name}");
                    client.UploadFile(r, dest + name);
                }
            }
            if (!Directory.Exists("D:\\samsaraftp\\")) Directory.CreateDirectory("D:\\samsaraftp\\");
            File.Move(filename, "D:\\samsaraftp\\" + name);
        }

        static void Main(string[] args)
        {
            string date = DateTime.Now.ToString("yyyyMMdd");
            if (args.Length == 1)
            {
                Console.WriteLine(args[0]);
                if(args[0].Length != 8)
                {
                    Console.WriteLine("Parameter error!");
                    Console.WriteLine("Format: 20220213");
                    return;
                }
                date = args[0];
            }
            if(!Directory.Exists(date)) Directory.CreateDirectory(date);
            //{0}:FT1-FT4,{1}:date,{2}:Fixture,{3}:dual/single
            string[] dirs = new string[]
            {
                // Path for FT1 - FT4
                "\\\\10.97.1.49\\SEL_Monitor\\Z7B_Test_Logs\\Z7B\\{0}\\{1}",
                "\\\\10.97.1.49\\SEL_Monitor\\Z7B_Test_Logs\\Z7B\\{0}\\{1}\\{2}\\{3}\\CSV",
                // Path for FT5
                // FT5 path include the IP folder after {3}
                //"\\\\10.97.1.135\\Sel_monitor\\Z7B_Test_Logs\\Z7B\\{0}\\{1}",
                //"\\\\10.97.1.135\\Sel_monitor\\Z7B_Test_Logs\\Z7B\\{0}\\{1}\\{2}\\{3}\\{4}\\CSV",
                // Update for path internal share folder
                "\\\\10.97.1.49\\SEL_Monitor\\Z7B_Test_Logs\\Z7B\\{0}\\{1}",
                "\\\\10.97.1.49\\SEL_Monitor\\Z7B_Test_Logs\\Z7B\\{0}\\{1}\\{2}\\{3}\\{4}\\CSV"
            };
            string[] stations = new string[] { "FT1", "FT2", "FT3", "FT4", "FT5" };
            string[] config = new string[] { "dual", "single" };
            string[] ips = new string[] { "10.0.1.10" };
            string[] destin = new string[] {
                "/samsara-jdm-ftp/quanta/Brigid/PVT/FATP/CM33/MFG test data/",//single
                "/samsara-jdm-ftp/quanta/Brigid/PVT/FATP/CM34/MFG test data/",//dual
            };
            DataTable tb = new DataTable();
            int current_row = 0;
            foreach (string cfg in config)
            {
                foreach(string station in stations)
                {
                    string wos = string.Empty;
                    string dir_station = string.Format(dirs[0], station, date);
                    if (station == "FT5")
                    {
                        dir_station = string.Format(dirs[2], station, date);
                    }
                    tb.Clear();
                    tb.Columns.Clear();
                    current_row = 0;
                    tb.Columns.Add("PO", typeof(string));
                    tb.Columns.Add("WO", typeof(string));
                    tb.Columns.Add("SN", typeof(string));
                    tb.Columns.Add("DATE", typeof(string));
                    tb.Columns.Add("Fixture_ID", typeof(string));
                    if (!Directory.Exists(dir_station)) continue;
                    string[] fixtures = Directory.GetDirectories(dir_station);
                    List<string> station_files = new List<string>();
                    string template = string.Empty;
                    foreach (string fixture in fixtures)
                    {
                        string current_dir = string.Format(dirs[1], station, date, fixture.Substring(fixture.LastIndexOf('\\')+1), cfg);
                        if (station == "FT5")
                        {
                            foreach (string ip in ips)
                            {
                                current_dir = string.Format(dirs[3], station, date, fixture.Substring(fixture.LastIndexOf('\\') + 1), cfg, ip);
                            }
                        }
                        Console.WriteLine($"Dir:{current_dir}");
                        if (!Directory.Exists(current_dir)) continue;
                        
                        foreach (string f in Directory.GetFiles(current_dir))
                        {
                            if (f.Contains("PRODUCTION"))
                            {
                                station_files.Add(f);
                            }
                            if (f.Contains("PRODUCTION") && f.Contains("PRODUCTION"))
                            {
                                template = f;
                            }
                        }
                    }
                    //Console.WriteLine(template);
                    if (template.Length == 0) continue;
                    List<string> titles = new List<string>() { "PO", "WO", "SN", "DATE", "Fixture_ID" }; //hard define not belong csv file
                    List<string> redundant = new List<string>() { "LineID", "OPID", "SN", "Slot", "Start_Time", "Cycle_Time", "FixtureID", "Result" };  //No Need in csv log file
                    using (StreamReader reader = new StreamReader(template))
                    {
                        using (CsvReader t = new CsvReader(reader, CultureInfo.InvariantCulture))
                        {
                            var records = t.GetRecords<dynamic>();
                            foreach (var r in records)
                            {
                                try
                                {

                                    string key = string.Format("{0}", r.Description);
                                    if (!titles.Contains(key) && !redundant.Contains(key))
                                    {
                                        titles.Add(string.Format("{0}", r.Description));
                                        titles.Add(string.Format("{0}_UPPER", r.Description));
                                        titles.Add(string.Format("{0}_LOWER", r.Description));
                                        titles.Add(string.Format("{0}_RESULT", r.Description));
                                        tb.Columns.Add(string.Format("{0}", r.Description), typeof(string));
                                        if (r.Description != "Result" )//& r.Description != "save_result_in_sfcs")
                                        {
                                            tb.Columns.Add(string.Format("{0}_UPPER", r.Description), typeof(string));
                                            tb.Columns.Add(string.Format("{0}_LOWER", r.Description), typeof(string));
                                            tb.Columns.Add(string.Format("{0}_RESULT", r.Description), typeof(string));
                                        }
                                    }
                                }
                                catch (Exception x)
                                {
                                    Console.WriteLine(string.Format("{0}:{1}", r.Description, x.Message));
                                }
                            }
                        }
                    }

                    foreach (string f in station_files)
                    {
                        using (StreamReader reader = new StreamReader(f))
                        {
                            using (CsvReader t = new CsvReader(reader, CultureInfo.InvariantCulture))
                            {
                                var records = t.GetRecords<dynamic>().ToList();
                                tb.Rows.Add(tb.NewRow());
                                current_row++;
                                foreach (var r in records)
                                {
                                    try
                                    {
                                        tb.Rows[current_row - 1][3] = date; // write the date time
                                        string title = string.Format("{0}", r.Description);
                                        int i = titles.FindIndex(d => d == title);
                                        string v = string.Format("{0}", r.Value);
                                        if (title == "get_info_from_sf")
                                        {
                                            string wok = Regex.Match(v, "'WO': '\\d{9}'").Value.ToString();
                                            string wo = Regex.Match(wok, "\\d{9}").Value.ToString();
                                            int iwo = titles.FindIndex(d => d == "WO");
                                            
                                            tb.Rows[current_row - 1][iwo] = wo;
                                            if (wos.Length == 0)
                                            {
                                                wos = wo;
                                            }
                                        }
                                        if (r.Description == "FixtureID")
                                        {
                                            int fixid = titles.FindIndex(d => d == "Fixture_ID");
                                            tb.Rows[current_row - 1][fixid] = r.Value;
                                        }
                                        if (v.Length == 0)
                                        {
                                            v = string.Format("{0}", r.ErrorCode);
                                        }
                                        tb.Rows[current_row - 1][i] = v;
                                        if (i >= 5 && i <= titles.Count)
                                        {
                                            tb.Rows[current_row - 1][i + 1] = r.HighLimit;
                                            tb.Rows[current_row - 1][i + 2] = r.LowLimit;
                                            tb.Rows[current_row - 1][i + 3] = r.ErrorCode;
                                        }
                                    }
                                    catch (Exception x)
                                    {
                                        //Console.WriteLine(x.Message);
                                    }
                                }
                            }
                        }
                    }
                    string xlsx_name = $"{date}\\BRIGID_{wos}_MFG_{station}_{(cfg == "single" ? "CM33" : "CM34")}_{date}.xlsx";
                    Console.WriteLine(xlsx_name);
                    MiniExcel.SaveAs(xlsx_name, tb);

                    //if (cfg == "single")
                    //{
                    //    Upload(xlsx_name, destin[0]);
                    //}
                    //else
                    //{
                    //    Upload(xlsx_name, destin[1]);
                    //}

                    Console.WriteLine(xlsx_name);
                }
            }
        }
    }
}
