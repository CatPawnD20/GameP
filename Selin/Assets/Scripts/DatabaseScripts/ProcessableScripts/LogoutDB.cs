using System;
using System.Collections;
using System.Collections.Generic;
using Npgsql;
using UnityEngine;

namespace Assets.Scripts
{
    /*
     * Logout bilgileri ile ilgili database işlemlerini gerçekleştirir
     * getItem(int id) -->> database'den user çekmek
     * getItem(string surname) -->> Login işlemi başında elimizde id bilgisi olmadığı için kullanılan ek çözüm bize id verir
     * 
     */
    public class LogoutDB : IProcessable
    {
        private PostgreSQL postgreSQL;
        

        public LogoutDB()
        {
            
        }

        public LogoutDB(PostgreSQL postgreSql)
        {
            this.postgreSQL = postgreSql;
        }
        
        public ParentObject getItem(int id)
        {
            throw new System.NotImplementedException();
        }

        public ParentObject getItem(string username)
        {
            throw new System.NotImplementedException();
        }

        public void deleteItem(int id)
        {
            throw new System.NotImplementedException();
        }

        public void updateItem(ParentObject parentObject)
        {
            throw new System.NotImplementedException();
        }

        public void putItem(ParentObject parentObject)
        {
            try
            {
                postgreSQL.connectDB();
                postgreSQL.openDB();
                Logout logout = (Logout) parentObject;
                string query = "INSERT INTO logouts (username) VALUES (@username)";
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = postgreSQL.Connection;
                command.CommandText = query;
                command.Parameters.AddWithValue("username", logout.LogoutUsername);
                command.Prepare();
                command.ExecuteNonQuery();
                postgreSQL.closeDB();
            }
            catch (NpgsqlException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public int putItemAndReturnId(ParentObject parentObject)
        {
            throw new NotImplementedException();
        }
    }
}