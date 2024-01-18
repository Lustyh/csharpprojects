using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Socket_Server_Robot
{
    public class Shopfloor
    {

        public static string sfclogpath = Application.StartupPath + "\\logs\\Shopfloor\\shopfloor_";
        /// <summary>
        /// 从SF里面查询SN的在某个站的状态和信息
        /// </summary>
        /// <param name="SN">Serial Number</param>
        /// <param name="station">站别名</param>
        /// <returns>查询结果</returns>
        public static string QueryData(string SN, string station)//同SF交换过站信息
        {
            try
            {
                DateTime dateTime = DateTime.Now;
                string Inputstring = string.Format(
                    "SN={0};$;station=QueryData;$;nextstation={1};$;MonitorAgentVer=VW20151102.01;$;",
                    SN, station);
                save_SF_DATA("Input", Inputstring);
                SqlConnection a = new SqlConnection();
                a.ConnectionString = "server=" + Socket_Server.ServerIP + ";Database=" + Socket_Server.DBName + ";Uid=sdt;Pwd=SDT#7;Integrated Security=False";
                try
                { a.Open(); }
                catch (Exception e)
                {
                    return "Open Error:" + e.Message;
                }
                if (a.State == ConnectionState.Open)
                {
                    SqlCommand b = new SqlCommand(Socket_Server.Portal, a);
                    b.CommandType = CommandType.StoredProcedure;
                    b.Parameters.Add("@BU", SqlDbType.VarChar).Value = Socket_Server.BU;
                    b.Parameters.Add("@Station", SqlDbType.VarChar).Value = Socket_Server.Station;
                    b.Parameters.Add("@Step", SqlDbType.VarChar).Value = Socket_Server.Step;
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
                    save_SF_DATA("Output", ss);
                    if ((DateTime.Now - dateTime).TotalSeconds > 1)
                    {
                        saveTimeout_SF_DATA("Timeout", dateTime.ToString("HH:mm:ss:fff") + Inputstring);
                    }
                    return ss;
                }
                return "Error: SQL is Closed";
            }
            catch(Exception exc)
            {
                return exc.Message;
            }
        }

        public static void save_SF_DATA(string datatype, string data)
        {
            try
            {
                string content = string.Format("[{0}][{1}]:{2}", datatype, DateTime.Now.ToString("HH:mm:ss:fff"), data);
                StreamWriter WriteLog2Ser = new StreamWriter(sfclogpath + DateTime.Now.ToString("yyyyMMdd") + ".log", true);
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
        public static void saveTimeout_SF_DATA(string datatype, string data)
        {
            try
            {
                string content = string.Format("[{0}][{1}]:{2}", datatype, DateTime.Now.ToString("HH:mm:ss:fff"), data);
                StreamWriter WriteLog2Ser = new StreamWriter(sfclogpath + "_Timeout" + DateTime.Now.ToString("yyyyMMdd") + ".log", true);
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
