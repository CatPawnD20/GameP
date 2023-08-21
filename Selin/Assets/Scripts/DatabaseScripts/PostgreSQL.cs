using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Npgsql;

namespace Assets.Scripts
{
    /*
     * Bu sınıf database bağlantısına ait bilgileri içeriyor ayrıca
     * Database bağlantısını oluşturup diğer sınıfların kullanabileceği sekilde servis ediyor
     */
    public class PostgreSQL 
    {
        private NpgsqlConnection connection;
        private NpgsqlCommand command;
        
        public void connectDB()
        {
            try
            {
                connection =
                    new NpgsqlConnection("Server=127.0.0.1;User Id=postgres;Password=database123;Database=Deneme1;");
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Debug.Log("Database'e bağlanılamadı!!!");
                throw;
            }
            
        }
        
        public void openDB()
        {
            try
            {
                connection.Open();
            }
            catch (NpgsqlException e)
            {
                Console.WriteLine(e);
                Debug.Log("Database bağlantısı açılamadı!!!");
                throw;
            }
        }

        public void closeDB()
        {
            try
            {
                connection.Close();
            }
            catch (NpgsqlException e)
            {
                Console.WriteLine(e);
                Debug.Log("Database bağlantısı kapatılamadı!!!");
                throw;
            }
        }

        public NpgsqlConnection Connection
        {
            get => connection;
            set => connection = value;
        }

        public NpgsqlCommand Command
        {
            get => command;
            set => command = value;
        }
    }
}