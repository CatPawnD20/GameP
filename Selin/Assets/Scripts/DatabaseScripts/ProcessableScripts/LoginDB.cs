using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Npgsql;
using Object = UnityEngine.Object;

namespace Assets.Scripts
{
    /*
     * Login bilgileri ile ilgili database işlemlerini gerçekleştirir
     * getItem(int id) -->> database'den user çekmek
     * getItem(string surname) -->> Login işlemi başında elimizde id bilgisi olmadığı için kullanılan ek çözüm bize id verir
     * 
     */
    public class LoginDB : IProcessable
    {
        private PostgreSQL postgreSQL;
        
        
        public LoginDB()
        {
            
        }

        public LoginDB(PostgreSQL postgreSql)
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
                Login login = (Login) parentObject;
                string query = "INSERT INTO logins (username) VALUES (@username)";
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = postgreSQL.Connection;
                command.CommandText = query;
                command.Parameters.AddWithValue("username", login.LoginUsername);
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