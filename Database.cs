using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Collectorcord.Server.Global_Variables;
using MySql.Data.MySqlClient;
using System.IO;
using static Collectorcord.TextUtil;
using System.Data;

namespace Collectorcord {
    public static class Database {
        private static MySqlConnection conn;
        static string keyPath = DIR + "\\Server_Txt\\DBKey.txt";
        public enum Function 
            { Select, };

        public static Task ConnectToServer() {
            try {
                conn = new MySqlConnection();
                conn.ConnectionString = GET_AUTH();
                conn.Open();
                Console.WriteLine("Connected to database succesfully.");
            }catch(MySqlException e) {
                Console.WriteLine(e.Message);
            }
            return Task.CompletedTask;
        }

        private static string GET_AUTH() {
            if (!File.Exists(keyPath)) {
                Console.WriteLine("Error in retrieving server key. Terminating.");
                Environment.Exit(0);
            }
            using (StreamReader sr = new StreamReader(keyPath)) {
                string res = sr.ReadLine() ?? "";
                if (res != "") {
                    return res;
                }
            }
            Console.WriteLine("Error in retrieving key (value was null). Terminating.");
            Environment.Exit(0);
            return "";
        }

        public static void DisconnectFromServer() {
            conn.Close();
            Console.WriteLine("Database connection was terminated.");
            return;
        }
        
        public static bool ValueExists(string table, string column, string value) {
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT " + column + " FROM " + table + " WHERE " + column + " = '" + value + "'";
            using (MySqlDataReader reader = cmd.ExecuteReader()) {
                try {
                    reader.Read();
                    if(reader.GetString(0) == value) {
                        return true;
                    } else {
                        Console.WriteLine(reader.GetString(0));
                    }
                } catch (Exception e) {
                    Console.WriteLine(e.Message);
                    reader.Close();
                }
            }
            return false;
        }

        public static bool AddEntry(string table, string[] columns, string[] valsToAdd) {
            MySqlCommand cmd = conn.CreateCommand();
            try {
                cmd.CommandText = cmd.CommandText =
                    "INSERT INTO " + table + ArrayToString(columns) + " VALUES " + ArrayToString(valsToAdd, true) + ";";
                int rowsAffected = cmd.ExecuteNonQuery();
                if(rowsAffected != 0) {
                    return true;
                }
            } catch (Exception e) {
                throw e;
            }
            return false;
        }

        public static string GetValueByPrimaryKey(string table, string column, string primaryKeyCol, string primaryKeyVal) {
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT " + column + " FROM " + table + " WHERE " + primaryKeyCol + " = '" + primaryKeyVal + "'";
            using (MySqlDataReader reader = cmd.ExecuteReader()) {
                try {
                    reader.Read();
                    return reader.GetString(0);
                } catch (Exception e) {
                    Console.WriteLine(e.Message);
                    reader.Close();
                }
            }
            return null;
        }

        public static DataTable Query(string query = null, Function function = Function.Select) {
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = query;
	    //Console.WriteLine("gh");
            MySqlDataReader reader = cmd.ExecuteReader();
            try {
                DataTable dt = new DataTable();
                if (!reader.HasRows) { //possible update action
                    dt.Columns.Add(new DataColumn {
                        ColumnName = "RowsAffected"
                    });
                    dt.Rows.Add(dt.NewRow()[0] = "RecordsAffected: " + reader.RecordsAffected.ToString());
		    reader.Close();
                    return dt;
                }
		//Console.WriteLine("ghh");
                dt.Load(reader);
		reader.Close();
                return dt;
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                reader.Close();
            }
            return null;
        }

        public static void PrintTable(string table, string[] columns, string WHERE = "", int noOfRows = -1, int spacing = 30) {
            if(spacing%10 != 0) {
                return;
            }

            string ret = "";
            MySqlCommand cmd = conn.CreateCommand();
            if (WHERE == "") {
                cmd.CommandText = "SELECT " + string.Join(",", columns) + " FROM " + table;
            } else {
                cmd.CommandText = "SELECT " + string.Join(",", columns) + " FROM " + table + " WHERE " + WHERE;
            }
            int noOfCol = columns.Length;
            using (MySqlDataReader reader = cmd.ExecuteReader()) {
                for (int i = 0; i < columns.Length; i++) {
                    string name = columns[i];
                    Console.Write("{0," + spacing + "}", name);
                }

                DataTable dt = new DataTable();
                dt.Load(reader);
                PrintDataTable(dt, columns, noOfRows, spacing);
            }
            return;
        }

        public static void PrintDataTable(DataTable dt, string[] columns = null, int noOfRows = -1, int spacing = 30) {
            noOfRows = ((noOfRows == -1) && (dt.Rows.Count <= 1000)) ? dt.Rows.Count : noOfRows;
            if(columns != null) {
                foreach(var entry in columns) {
                    Console.Write("{0," + spacing + "}", entry);
                }
                Console.WriteLine();
            }
            for (int j = 0; j < noOfRows; j++) {
                var row = dt.Rows[j];
                for (int i = 0; i < row.ItemArray.Length; i++) {
                    var val = row.ItemArray[i];
                    Console.Write("{0," + spacing + "}", val);
                }
                Console.WriteLine();
            }
        }
    }
}
