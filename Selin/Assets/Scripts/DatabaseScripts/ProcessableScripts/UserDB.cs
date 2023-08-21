using System;
using System.Data;
using Npgsql;
using Unity.VisualScripting;
using UnityEngine;


namespace Assets.Scripts
{
    /*
     * User bilgileri ile ilgili database işlemlerini gerçekleştirir
     * getItem(int id) -->> database'den user çekmek
     * getItem(string surname) -->> Login işlemi başında elimizde id bilgisi olmadığı için kullanılan ek çözüm bize id verir
     * 
     */
    public class UserDB : IProcessable
    {
        private PostgreSQL postgreSQL;

        public UserDB()
        {
            
        }

        public UserDB(PostgreSQL postgreSql)
        {
            this.postgreSQL = postgreSql;
        }
        public ParentObject getItem(int id)
        {
            return null;
        }
        
        public ParentObject getItem(string username)
        {
            User user = null; 
            try
            {
                
                postgreSQL.connectDB();
                postgreSQL.openDB();
                string query = "SELECT * FROM users WHERE username=@username ";
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = postgreSQL.Connection;
                
                
                command.CommandText = query;
                command.Parameters.AddWithValue("username", username);
                command.Prepare();
                
                NpgsqlDataReader dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    int id = dataReader.GetInt32(0);
                    user = new User(id);
                    user.Username = dataReader.GetString(1);
                    user.Password = dataReader.GetString(2);
                    user.Balance = (float) dataReader.GetDecimal(3);
                    user.RakePercent = (float) dataReader.GetDecimal(4);
                    if (!dataReader.IsDBNull(5))
                    {
                        user.Parent = dataReader.GetString(5);
                    }
                   
                    else
                    {
                        user.Parent = null;
                    }

                    user.ParentPercent = (float) dataReader.GetDecimal(6);
                    user.Creator = dataReader.GetString(7);
                    user.CreationDate = dataReader.GetDateTime(8);
                    string userTypeString = dataReader.GetString(9);
                    switch (userTypeString)
                    {
                        case "Admin":
                            user.UserType = UserTypes.Admin;
                            break;
                        case "Operator":
                            user.UserType = UserTypes.Operator;
                            break;
                        case "Player":
                            user.UserType = UserTypes.Player;
                            break;
                    }
                    user.RakeBackAmount = (float) dataReader.GetDecimal(10);
                }
                dataReader.Close();
                postgreSQL.closeDB();
            }
            catch (NpgsqlException e)
            {
                Console.WriteLine(e);
                Debug.Log("UserDB -->>> getItem(username) -->>> catch -->>> user bilgilerine ulaşılamadı : ");
                throw;
            }
            return user;
        }

        public void deleteItem(int id)
        {
            throw new System.NotImplementedException();
        }

        public void updateItem(ParentObject parentObject)
        {
            try
            {
                postgreSQL.connectDB();
                postgreSQL.openDB();
                User user = (User) parentObject;
                string query =
                    "UPDATE users SET password = @password,balance = @balance,rakepercent = @rakepercent,parent = @parent,parentpercent = @parentpercent,creator = @creator,creationdate = @creationdate,usertype = @usertype,rakebackamount = @rakebackamount " +
                    "WHERE username = @username";
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = postgreSQL.Connection;
                command.CommandText = query;
                command.Parameters.AddWithValue("password",user.Password);
                command.Parameters.AddWithValue("balance",user.Balance);
                command.Parameters.AddWithValue("rakepercent",user.RakePercent);
                if (string.IsNullOrEmpty(user.Parent))
                {
                    command.Parameters.AddWithValue("parent",DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("parent",user.Parent);
                }
                
                command.Parameters.AddWithValue("parentpercent",user.ParentPercent);
                command.Parameters.AddWithValue("creator",user.Creator);
                command.Parameters.AddWithValue("creationdate",user.CreationDate);
                command.Parameters.AddWithValue("usertype",user.UserType.ToString());
                command.Parameters.AddWithValue("rakebackamount",user.RakeBackAmount);
                command.Parameters.AddWithValue("username",user.Username);
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

        public void putItem(ParentObject parentObject)
        {
            try
            {
                postgreSQL.connectDB();
                postgreSQL.openDB();
                User user = (User) parentObject;
                string query = "INSERT INTO users (username,password,balance,rakepercent,parent,parentpercent,creator,creationdate,usertype) " +
                               "VALUES (@username,@password,@balance,@rakepercent,@parent,@parentpercent,@creator,@creationdate,@usertype)";
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = postgreSQL.Connection;
                command.CommandText = query;
                command.Parameters.AddWithValue("username",user.Username);
                command.Parameters.AddWithValue("password",user.Password);
                command.Parameters.AddWithValue("balance",user.Balance);
                command.Parameters.AddWithValue("rakepercent",user.RakePercent);
                if (string.IsNullOrEmpty(user.Parent))
                {
                    command.Parameters.AddWithValue("parent",DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("parent",user.Parent);
                }
                
                command.Parameters.AddWithValue("parentpercent",user.ParentPercent);
                command.Parameters.AddWithValue("creator",user.Creator);
                command.Parameters.AddWithValue("creationdate",user.CreationDate);
                command.Parameters.AddWithValue("usertype",user.UserType.ToString());
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