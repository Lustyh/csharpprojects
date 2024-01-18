using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace SqlTest 
{
    public class Query
    {
        public static void Main(string[] args)
        {
            
            using (SqlConnection conn = new SqlConnection())
            {
                string QueryTarget = "server={0};Database={1};Uid={2};Pwd={3};Integrated Security=False";
                string QueryIP = "10.97.1.40";
                string DataBase = "QMS";
                string User = "sdt";
                string PW = "SDT#7";
                string inputInfo = "SN={0};$;station=QueryData;$;";
                string SN = "WIP25061HFAKA1W6F";
                try
                {
                    conn.ConnectionString = string.Format(QueryTarget, QueryIP, DataBase, User, PW);
                    StringBuilder OutPutStr = new StringBuilder();
                    string input = string.Format(inputInfo, SN);
                    Console.WriteLine(input);
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        string cmd = "MonitorPortal";
                        SqlCommand sqlcmd = new SqlCommand(cmd, conn);
                        sqlcmd.CommandType = CommandType.StoredProcedure;
                        sqlcmd.Parameters.Add("@BU", SqlDbType.VarChar).Value = "BU4";
                        sqlcmd.Parameters.Add("@Station", SqlDbType.VarChar).Value = "SWDL";
                        sqlcmd.Parameters.Add("@Step", SqlDbType.VarChar).Value = "querydata";
                        sqlcmd.Parameters.Add("@InPutStr", SqlDbType.VarChar).Value = input;
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
                        conn.Close();
                        string values = OutPutStr.ToString().Replace("\r", "\\r").Replace("\n", "\\n");
                        Console.WriteLine(values);
                        string[] key_v = values.Replace(";$;", "\n").Split('\n');
                        Console.WriteLine(key_v[0]);
                        foreach (string line in key_v)
                        {
                            Console.WriteLine((line));
                            if (line.Contains('='))
                            {
                                string[] data = line.Replace("SET ", "").Split('=');
                                string n_line = string.Format("\"{0}\"=\"{1}\"\n", data[0], data[1]);
                                Console.WriteLine(n_line);
                            }
                        }
                    }
                }
                catch (Exception ex) 
                {
                    
                    Console.WriteLine(ex);
                }
                
            }
        }
    }
}