using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ShadowTM
{
    public class PluginSF
    {
        private static string ServerIP= "10.18.6.52";
        private static string DBName  = "QMS";
        private static string BU      = "NB4";
        private static string Portal  = "MonitorPortal";
        private static string Station = "SWDL";
        private static string Step    = "querydata";
        public static string Line_type = "FATP";
        private static string STATION_NAME = "";
        public static int Count = 1;
        public static int Fix_Holder_No = 8;
        public static string defau = "FAIL";
        public static string test_mode = "";
        public static string exe_path = "";
        public static string Send_sn = "Y";
        public static string Retest_Rule = "ABC";
        public static bool Auto_Fill = false;
        public static int Group_Num = 1;

        private class Initializer
        {
            private const string CONFIG = ".\\Setting.ini";
            [DllImport("kernel32")]
            private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

            /// <summary>
            /// 读取配置
            /// </summary>
            /// <param name="section"></param>
            /// <param name="key"></param>
            /// <returns></returns>
            public static string ReadItem(string section, string key)
            {
                StringBuilder value = new StringBuilder(255);
                int i = GetPrivateProfileString(section, key, "", value, 255, CONFIG);
                return value.ToString();
            }
        }

        private class Logger
        {
            private static object locker = new object();
            public static void Record(string type, string log)
            {
                lock (locker)
                {
                    string path = Path.Combine(".\\", "Shopfloor-" + DateTime.Now.ToString("yyyyMMdd") + ".log");
                    using (StreamWriter writer = new StreamWriter(path, true))
                    {
                        writer.WriteLine(string.Format("[{0}][{1}]{2}", type, DateTime.Now.ToString("HH:mm:ss:fff"), log));
                    }
                }
            }
        }

        public static bool Init()
        {
            try
            {
                ServerIP= Initializer.ReadItem("DataBase", "ServerIP");
                DBName  = Initializer.ReadItem("DataBase", "DBName");
                BU      = Initializer.ReadItem("DataBase", "BU");
                Portal  = Initializer.ReadItem("DataBase", "Portal");
                Station = Initializer.ReadItem("DataBase", "Station");
                Step    = Initializer.ReadItem("DataBase", "Step");
                Line_type = Initializer.ReadItem("DataBase", "LineType");
                string F= Initializer.ReadItem("HOLDER"  , "Fix_Count");
                string H= Initializer.ReadItem("HOLDER", "Fix_Holder_Num");
                string G = Initializer.ReadItem("HOLDER", "Group_Num");
                test_mode = Initializer.ReadItem("HOLDER", "Test_Mode");
                defau   = Initializer.ReadItem("STATION", "STATUS_XX");
                Send_sn = Initializer.ReadItem("HOLDER", "Send_SN");
                Retest_Rule = Initializer.ReadItem("HOLDER", "RetestRule");
                STATION_NAME = Initializer.ReadItem("STATION", "STATION_NAME");
                string auto_fill = Initializer.ReadItem("HOLDER", "Auto_Fill");
                if (auto_fill.ToLower() == "true")
                {
                    Auto_Fill = true;
                }
                else
                {
                    Auto_Fill = false;
                }

                Console.Write("Fix_Count=" + F);
                Count = int.Parse(F);
                Console.Write("count=" + Count.ToString());
                Fix_Holder_No = int.Parse(H);
                Console.WriteLine("Group_Count=" + G);
                Group_Num = int.Parse(G);
                Console.Write("FixHolderNum=" + H.ToString());
                Console.Write("Fix_Holder_No=" + Fix_Holder_No.ToString());
                string info = string.Format(
                    "ServerIP={0} DBName={1} BU={2} Portal={3} Station={4} Step={5}", 
                    ServerIP, 
                    DBName, 
                    BU, 
                    Portal, 
                    Station, 
                    Step);
                Logger.Record("INT", info);
                Console.WriteLine(info);
                string cinfo = string.Format(
                    "server={0};Database={1};Uid=sdt;Pwd=SDT#7;Integrated Security=False",
                    ServerIP,
                    DBName
                    );
                Console.WriteLine(cinfo);
            }
            catch (Exception x)
            {
                Logger.Record("INT", "ERROR:" + x.Message);
                return false;
            }
            return true;
        }
        public static string get_status(string status)
        {
            try
            {
                if (status == "")
                {
                    return "FAIL";
                }
                return Initializer.ReadItem("STATION", "STATUS_" + status);
            }
            catch (Exception)
            {
                return "FAIL";
            }
        }
        public static string Query(string SN)
        {
            if (Line_type == "SMT")
            {
                return SMT_Query(SN);
            }
            else
            {
                return FATP_Query(SN);
            }
        }
        public static string FATP_Query(string SN)
        {
            using (SqlConnection sf = new SqlConnection())
            {
                DateTime dateTime = DateTime.Now;
                string query = string.Format(
                            "SN={0};$;station=QueryData;$;nextstation={1};$;MonitorAgentVer=VW20151102.01;$;",
                            SN.Replace("BWIP", "WIP").Trim(),
                            STATION_NAME
                            );
                sf.ConnectionString = string.Format(
                    "server={0};Database={1};Uid=sdt;Pwd=SDT#7;Integrated Security=False",
                    ServerIP,
                    DBName
                    );
                try { sf.Open(); } catch (Exception) { }
                StringBuilder OutPutStr = new StringBuilder();
                if (sf.State != ConnectionState.Open) return string.Empty;
                // Console.WriteLine(query);
                try
                {
                    SqlCommand sqlcmd = new SqlCommand(Portal, sf);
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.Add("@BU", SqlDbType.VarChar).Value = BU;
                    sqlcmd.Parameters.Add("@Station", SqlDbType.VarChar).Value = Station;
                    sqlcmd.Parameters.Add("@Step", SqlDbType.VarChar).Value = Step;
                    sqlcmd.Parameters.Add("@InPutStr", SqlDbType.VarChar).Value = query;
                    sqlcmd.Parameters.Add("@OutPutStr", SqlDbType.VarChar, 8000);
                    sqlcmd.Parameters["@BU"].Direction = ParameterDirection.Input;
                    sqlcmd.Parameters["@BU"].DbType = DbType.String;
                    sqlcmd.Parameters["@Station"].Direction = ParameterDirection.Input;
                    sqlcmd.Parameters["@Station"].DbType = DbType.String;
                    sqlcmd.Parameters["@Step"].Direction = ParameterDirection.Input;
                    sqlcmd.Parameters["@Step"].DbType = DbType.String;
                    sqlcmd.Parameters["@InPutStr"].Direction = ParameterDirection.Input;
                    sqlcmd.Parameters["@InPutStr"].DbType = DbType.String;
                    sqlcmd.Parameters["@OutPutStr"].DbType = DbType.String;
                    sqlcmd.Parameters["@OutPutStr"].Direction = ParameterDirection.Output;
                    sqlcmd.ExecuteScalar();
                    OutPutStr.Append(sqlcmd.Parameters["@OutPutStr"].Value.ToString());
                    if (sf.State == ConnectionState.Open) sf.Close();
                    string retstr = OutPutStr.ToString().ToUpper();
                    //Console.WriteLine(retstr);
                    Logger.Record("SQL", "OutPut:" + retstr);
                    if ((DateTime.Now - dateTime).TotalSeconds > 1)
                    {
                        save_SF_DATA("Timeout_", dateTime.ToString("HH:mm:ss:fff") + query);
                    }
                    return retstr;
                    
                }
                catch (Exception x)
                {
                    Logger.Record("SQL", "Query:" + x.Message);
                }
                try { sf.Close(); } catch (Exception) { }
            }
            return  string.Empty;
        }
        public static string SMT_Query(string SN)
        {
            using (SqlConnection sf = new SqlConnection())
            {
                string query = string.Format(
                    "MB_NUM={0};$;Station={1};$;FixNO=",
                            SN.ToUpper(),
                            STATION_NAME
                            );
                sf.ConnectionString = string.Format(
                    "server={0};Database={1};Uid=execuser;Pwd=exec7*user;Integrated Security=False",
                    ServerIP,
                    DBName
                    );
                try { sf.Open(); } catch (Exception) { }
                StringBuilder OutPutStr = new StringBuilder();
                if (sf.State != ConnectionState.Open) return string.Empty;
                //Console.WriteLine(query);
                try
                {
                    SqlCommand sqlcmd = new SqlCommand(Portal, sf);
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.Add("@BU", SqlDbType.VarChar).Value = BU;
                    sqlcmd.Parameters.Add("@Station", SqlDbType.VarChar).Value = Station;
                    sqlcmd.Parameters.Add("@Step", SqlDbType.VarChar).Value = Step;
                    sqlcmd.Parameters.Add("@InPutStr", SqlDbType.VarChar).Value = query;
                    sqlcmd.Parameters.Add("@OutPutStr", SqlDbType.VarChar, 8000);
                    sqlcmd.Parameters["@BU"].Direction = ParameterDirection.Input;
                    sqlcmd.Parameters["@BU"].DbType = DbType.String;
                    sqlcmd.Parameters["@Station"].Direction = ParameterDirection.Input;
                    sqlcmd.Parameters["@Station"].DbType = DbType.String;
                    sqlcmd.Parameters["@Step"].Direction = ParameterDirection.Input;
                    sqlcmd.Parameters["@Step"].DbType = DbType.String;
                    sqlcmd.Parameters["@InPutStr"].Direction = ParameterDirection.Input;
                    sqlcmd.Parameters["@InPutStr"].DbType = DbType.String;
                    sqlcmd.Parameters["@OutPutStr"].DbType = DbType.String;
                    sqlcmd.Parameters["@OutPutStr"].Direction = ParameterDirection.Output;
                    sqlcmd.ExecuteScalar();
                    OutPutStr.Append(sqlcmd.Parameters["@OutPutStr"].Value.ToString());
                    if (sf.State == ConnectionState.Open) sf.Close();
                    string retstr = OutPutStr.ToString().ToUpper();
                    Logger.Record("SQL", "OutPut:" + retstr);
                    return retstr;
                }
                catch (Exception x)
                {
                    Logger.Record("SQL", "Query:" + x.Message);
                }
                try { sf.Close(); } catch (Exception) { }
            }
            return string.Empty;
        }
        public static bool Close()
        {
            Console.WriteLine("Close shopfloor");
            return true;
        }

        public static void save_SF_DATA(string datatype, string data)
        {
            try
            {
                string content = string.Format("[{0}][{1}]:{2}", datatype, DateTime.Now.ToString("HH:mm:ss:fff"), data);
                StreamWriter WriteLog2Ser = new StreamWriter(".\\SFC_TIMEOUT_"+ DateTime.Now.ToString("yyyyMMdd") + ".log", true);
                WriteLog2Ser.AutoFlush = true;
                WriteLog2Ser.WriteLine(content);
                WriteLog2Ser.Flush();
                WriteLog2Ser.Close();
                /*string path = Path.Combine(".\\logs\\", "logs_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log");
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine();
                }*/
            }
            catch (Exception)
            {
            }
        }
    }
}
