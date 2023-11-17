using DataHarbor.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        //创建表
        public static void CreateTable(string tableName)
        {
            using (var connection = GetSQLiteConnection())
            {
                string sql = $"CREATE TABLE IF NOT EXISTS {tableName} " +
                             "(Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                             "Name TEXT NOT NULL, " +
                             "UKey TEXT NOT NULL, " +
                             "Describe TEXT, " +
                             "WebLink TEXT, " +
                             "FileLocation TEXT, " +
                             "FileNum INTEGER, " +
                             "Date DATE)";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        //查询表某个值，如果存在则返回true，否则返回false
        public static bool IsProjectExist(string name, string projectName)
        {
            SQLiteConnection conn = GetSQLiteConnection();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = conn;
            cmd.CommandText = "SELECT * FROM " + name + " WHERE ProjectName = '" + projectName + "'";
            SQLiteDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                conn.Close();
                return true;
            }
            else
            {
                conn.Close();
                return false;
            }
        }

        //查询表，并返回全部数据
        public static ObservableCollection<DataSetProject> GetProjectTable(string name)
        {
            ObservableCollection<DataSetProject> list = new ObservableCollection<DataSetProject>();
            SQLiteConnection conn = GetSQLiteConnection();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = conn;
            cmd.CommandText = "SELECT * FROM " + name;
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DataSetProject project = new DataSetProject();
                project.ProjectName = reader["ProjectName"].ToString();
                project.ProjectDescribe = reader["ProjectDescribe"].ToString();
                project.DataTotal = Convert.ToInt32(reader["DataTotal"].ToString());
                list.Add(project);
            }
            conn.Close();
            return list;
        }

        //删除数据
        public static void DeleteData(string name, string projectName)
        {
            SQLiteConnection conn = GetSQLiteConnection();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = conn;
            cmd.CommandText = "DELETE FROM " + name + " WHERE ProjectName = '" + projectName + "'";
            cmd.ExecuteNonQuery();
            conn.Close();
        }


    }
}
