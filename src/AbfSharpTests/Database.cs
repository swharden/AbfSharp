using Microsoft.Data.Sqlite;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbfSharpTests
{
    class Database
    {
        [Test]
        public void Test_CreateDB()
        {
            string[] abfFilePaths = System.IO.Directory.GetFiles(SampleData.DATA_FOLDER, "*.abf").ToArray();
            Assert.IsNotNull(abfFilePaths);
            Assert.IsNotEmpty(abfFilePaths);

            // delete the old database if it exists
            string databaseFilename = "abfs.db";
            if (System.IO.File.Exists(databaseFilename))
                System.IO.File.Delete(databaseFilename);

            // open the database file (creating it if it doesn't exist)
            using var conn = new SqliteConnection($"Data Source={databaseFilename};");
            conn.Open();

            using var createTableCommand = conn.CreateCommand();
            createTableCommand.CommandText =
                "CREATE TABLE IF NOT EXISTS Abfs" +
                "(" +
                    "[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                    "[Folder] TEXT NOT NULL, " +
                    "[Filename] TEXT NOT NULL, " +
                    "[Date] CHAR(10) NOT NULL, " + // YYYY-MM-DD
                    "[Time] CHAR(12) NOT NULL " + // HH:MM:SS.SSS
                ")";
            createTableCommand.ExecuteNonQuery();

            // load ABF details into the table
            foreach (string abfFile in abfFilePaths)
            {
                string abfFilePath = System.IO.Path.GetFullPath(abfFile);
                var abf = new AbfSharp.ABF(abfFilePath);

                using var insertAbfCommand = new SqliteCommand("INSERT INTO Abfs " +
                    "(Folder, Filename, Date, Time) " +
                    "VALUES (@folder, @filename, @date, @time)", conn);
                insertAbfCommand.Parameters.AddWithValue("folder", System.IO.Path.GetDirectoryName(abf.FilePath));
                insertAbfCommand.Parameters.AddWithValue("filename", System.IO.Path.GetFileName(abf.FilePath));
                insertAbfCommand.Parameters.AddWithValue("date", abf.Header.StartDateTime.ToString("yyyy-MM-dd"));
                insertAbfCommand.Parameters.AddWithValue("time", abf.Header.StartDateTime.ToString("HH:mm:ss.fff"));
                insertAbfCommand.ExecuteNonQuery();
            }

            // read ABF details from the table
            using var readAbfsCommand = conn.CreateCommand();
            readAbfsCommand.CommandText = @"SELECT Filename, Date, Time FROM Abfs";
            SqliteDataReader reader = readAbfsCommand.ExecuteReader();
            int abfsReadFromDatabase = 0;
            while (reader.Read())
            {
                string filename = reader["Filename"].ToString();
                string date = reader["Date"].ToString();
                string time = reader["Time"].ToString();
                abfsReadFromDatabase += 1;
                Console.WriteLine($"read: {filename} ({date} {time})");
            }

            Assert.AreEqual(abfFilePaths.Length, abfsReadFromDatabase);

            // close the connection intentionally
            conn.Close();
        }
    }
}
