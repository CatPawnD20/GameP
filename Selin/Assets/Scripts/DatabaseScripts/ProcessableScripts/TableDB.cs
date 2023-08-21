using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Npgsql;
using UnityEngine;

namespace Assets.Scripts
{
    /*
     * TableCreation bilgileri ile ilgili database işlemlerini gerçekleştirir
     * putItem(parentObject) 
     */
    public class TableDB : IProcessable
    {
        private PostgreSQL postgreSQL;
        

        public TableDB()
        {
        
        }
        public TableDB(PostgreSQL postgreSql)
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
                Table table = (Table) parentObject;
                string query = "INSERT INTO gametables (admin,matchid,password,tablerakepercent) VALUES (@admin,@matchid,@password,@tablerakepercent)";
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = postgreSQL.Connection;
                command.CommandText = query;
                command.Parameters.AddWithValue("admin", table.CreatorName);
                command.Parameters.AddWithValue("matchid", table.MatchId);
                if (table.Password == null)
                {
                    command.Parameters.AddWithValue("password", "null");
                }
                else
                {
                    command.Parameters.AddWithValue("password", table.Password);    
                }
                command.Parameters.AddWithValue("tablerakepercent", table.TableRakePercent);
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