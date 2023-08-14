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
        internal static DataTable PostgreSQL(string sqlQuery)
        {
            try
            {
                string connect = $@"sqlConnectData";
                NpgsqlConnection nc = new NpgsqlConnection(connect);
                nc.Open();
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(sqlQuery, nc);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                nc.Close();
                return dt;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.Message);
                return null;
            }
        }
    }
}
