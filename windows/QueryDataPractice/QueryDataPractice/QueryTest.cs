using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace QueryDataPractice
{
    public partial class QueryTest : Form
    {
        public QueryTest()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection())
            { 
                try
                {
                    conn.ConnectionString = string.Format(sendStr.Text, ServerList.Text, "QMS", "sdt", "SDT#7");
                    StringBuilder OutPutStr = new StringBuilder();
                    string input = string.Format("SN={0};$;station=QueryData;$;", SN.Text);
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        string cmd = "MonitorPortal";
                        SqlCommand sqlcmd = new SqlCommand(cmd, conn);
                        sqlcmd.CommandType = CommandType.StoredProcedure;
                        sqlcmd.Parameters.Add("@BU", SqlDbType.VarChar).Value = "BU4";
                        sqlcmd.Parameters.Add("@Station", SqlDbType.VarChar).Value = queryStation.Text;
                        sqlcmd.Parameters.Add("@Step", SqlDbType.VarChar).Value = queryStep.Text;
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
                        OriginData.Text = values;
                        string[] key_v = values.Replace(";$;", "\n").Split('\n');
                        foreach (string line in key_v)
                        {
                            if (line.Contains('='))
                            {
                                string[] data = line.Replace("SET ", "").Split('=');
                                string n_line = string.Format("\"{0}\"=\"{1}\"\n", data[0], data[1]);
                                ParsedData.AppendText(n_line);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {   

                    OriginData.Text = ex.Message;
                }
            }
        }
        private void QueryTest_Load(object sender, EventArgs e)
        {
            FactoryBox.Items.AddRange("QMB\nQCMC".Split('\n'));
            ServerList.Items.AddRange("10.97.1.40\n10.18.6.52".Split('\n'));

            /* Per setting*/
            FactoryBox.Text = "QMB";
            ServerList.Text = "10.97.1.40";
            queryStation.Text = "SWDL";
            cmdText.Text = "MonitorPortal";
            queryStep.Text = "querydata";
            sendStr.Text = "server={0};Database={1};Uid={2};Pwd={3};Integrated Security=False";
        }
    }
}