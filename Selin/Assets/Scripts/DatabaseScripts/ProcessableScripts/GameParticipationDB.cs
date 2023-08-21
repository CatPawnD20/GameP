using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Npgsql;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameParticipationDB : IProcessable
    {
        private PostgreSQL postgreSQL;
        public GameParticipationDB()
        {
        
        }
        public GameParticipationDB(PostgreSQL postgreSQL)
        {
            this.postgreSQL = postgreSQL;
        }
        public ParentObject getItem(int id)
        {
            GameParticipation gameParticipation = new GameParticipation(id);
            try 
            {
                
                postgreSQL.connectDB();
                postgreSQL.openDB();
                string query = "SELECT * FROM gameparticipations WHERE id=@id ";
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = postgreSQL.Connection;
                command.CommandText = query;
                command.Parameters.AddWithValue("id", id);
                command.Prepare();
                
                NpgsqlDataReader dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    gameParticipation.id = dataReader.GetInt32(0);
                    gameParticipation.Matchid = dataReader.GetString(1);
                    gameParticipation.Username = dataReader.GetString(2);
                    gameParticipation.JoinDate = dataReader.GetDateTime(3);
                    gameParticipation.JoinBalance = (float) dataReader.GetDecimal(4);
                    if (!dataReader.IsDBNull(5))
                    {
                        gameParticipation.LeaveDate = dataReader.GetDateTime(5);
                    }
                    else
                    {
                        gameParticipation.LeaveDate = DateTime.Now;
                    }
                    if (!dataReader.IsDBNull(6))
                    {
                        gameParticipation.LeaveBalance = (float) dataReader.GetDecimal(6);
                    }
                    else
                    {
                        gameParticipation.LeaveBalance = 0;
                    }
                }
                dataReader.Close();
                postgreSQL.closeDB();
            }
            catch (NpgsqlException e)
            {
                Console.WriteLine(e);
                Debug.Log("Error in GameParticipationDB getItem");
                throw;
            }
            return gameParticipation;
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
            try
            {
                postgreSQL.connectDB();
                postgreSQL.openDB();
                GameParticipation gameParticipation = (GameParticipation) parentObject;
                string query =
                    "UPDATE gameparticipations SET leavedate = @leavedate,leavebalance = @leavebalance " +
                    "WHERE id = @id";
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = postgreSQL.Connection;
                command.CommandText = query;
                command.Parameters.AddWithValue("id", gameParticipation.id);
                command.Parameters.AddWithValue("leavedate", gameParticipation.LeaveDate);
                command.Parameters.AddWithValue("leavebalance", gameParticipation.LeaveBalance);
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
            throw new System.NotImplementedException();
        }

        public int putItemAndReturnId(ParentObject parentObject)
        {
            try
            {
                postgreSQL.connectDB();
                postgreSQL.openDB();
                GameParticipation gameParticipation = (GameParticipation) parentObject;
                int id = 0;
                string query = "INSERT INTO gameparticipations (matchid,username,joindate,joinbalance) " +
                               "VALUES (@matchid,@username,@joindate,@joinbalance)" +
                               "RETURNING id";
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = postgreSQL.Connection;
                command.CommandText = query;
                command.Parameters.AddWithValue("matchid",gameParticipation.Matchid);
                command.Parameters.AddWithValue("username",gameParticipation.Username);
                command.Parameters.AddWithValue("joindate",gameParticipation.JoinDate);
                command.Parameters.AddWithValue("joinbalance",gameParticipation.JoinBalance);
                command.Prepare();
                
                NpgsqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    id = dataReader.GetInt32(0);
                }
                postgreSQL.closeDB();
                return id;
            }
            catch (NpgsqlException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}