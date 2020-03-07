using LYL.Data.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYL.Data
{
    public class SqliteDbContext : SQLiteConnection, IDisposable
    {
        public static string SqlitePath
        {
            get
            {
                //var dbpath = System.AppDomain.CurrentDomain.BaseDirectory + "mysqlite.db";
                var dbpath = @"C:\lylremote.db";
                if (!File.Exists(dbpath))
                {
                    File.Create(dbpath);
                }
                return dbpath;
            }
        }
        public SqliteDbContext() : base(SqlitePath)
        {
            CreateTables();
        }


        void CreateTables()
        {
            this.CreateTable<MachineInfo>(); 

        }
        public TableQuery<MachineInfo> MachineInfos { get { return this.Table<MachineInfo>(); } }
        
        protected override void Dispose(bool disposing)
        { 
            base.Dispose(disposing);
        }
    }
}
