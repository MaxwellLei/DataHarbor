using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataHarbor.Services
{
    internal class DatabaseService
    {
        
        //连接sqlite数据库
        public static SQLiteConnection GetSQLiteConnection()
        {
            //获取当前程序的相对路径
            string path = System.Environment.CurrentDirectory + "\\Resources\\MainData.db";
            SQLiteConnection conn = new SQLiteConnection("Data Source=" + path + ";Version=3;");
            conn.Open();
            return conn;
        }

        //创建sqlite数据库
        public static void CreateSQLiteDB(string name)
        {
            SQLiteConnection.CreateFile(name +".db");
        }

        //插入数据
        public static void InsertData(string tableName, string projectName, string projectDescribe,int num)
        {
            SQLiteConnection conn = GetSQLiteConnection();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = conn;
            cmd.CommandText = "INSERT INTO " + tableName + " (ProjectName, ProjectDescribe, DataTotal) VALUES ('" + projectName + "', '" + projectDescribe + "', '"+ num + "')";
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        //查询表，并返回全部数据
        public static List<string> SelectTable(string name)
        {
            List<string> list = new List<string>();
            SQLiteConnection conn = GetSQLiteConnection();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = conn;
            cmd.CommandText = "SELECT * FROM " + name;
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(reader["ProjectName"].ToString());
                list.Add(reader["ProjectDescribe"].ToString());
                list.Add(reader["DataTotal"].ToString());
            }
            conn.Close();
            return list;
        }

        //删除数据
        public static void DeleteData(string name, string url)
        {
            SQLiteConnection conn = GetSQLiteConnection();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = conn;
            cmd.CommandText = "DELETE FROM " + name + " WHERE url = '" + url + "'";
            cmd.ExecuteNonQuery();
            conn.Close();
        }


    }
}
