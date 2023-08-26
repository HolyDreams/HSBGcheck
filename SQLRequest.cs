using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newHSBGcheck
{
    internal class SQLRequest
    {
        static NpgsqlConnection nc;
        internal static void ConnectToDB()
        {
            string connect = $@"sqlServer";
            nc = new NpgsqlConnection(connect);
            nc.Open();
        }
        internal static DataTable PostgreSQL(string sqlQuery)
        {
            try
            {
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(sqlQuery, nc);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.Message);
                return null;
            }
        }
        internal static void DisconnectDB()
        {
            nc.Close();
        }
    }
}
