using System;
using System.Collections;
using System.Collections.Generic;
using Npgsql;
using UnityEngine;

namespace Assets.Scripts
{
    public class TurnWinnersDB : IProcessable
    {
        private PostgreSQL postgreSQL;

        public TurnWinnersDB()
        {
            
        }

        public TurnWinnersDB(PostgreSQL postgreSQL)
        {
            this.postgreSQL = postgreSQL;
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
                TurnWinners turnWinners = (TurnWinners) parentObject;
                string query = "INSERT INTO turnwinners (turnid,winner1,win1amount,winner2,win2amount,winner3,win3amount,winner4,win4amount,winner5,win5amount,winner6,win6amount,winner7,win7amount,winner8,win8amount) VALUES (@turnid,@winner1,@win1amount,@winner2,@win2amount,@winner3,@win3amount,@winner4,@win4amount,@winner5,@win5amount,@winner6,@win6amount,@winner7,@win7amount,@winner8,@win8amount)";
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = postgreSQL.Connection;
                command.CommandText = query;
                command.Parameters.AddWithValue("turnid", turnWinners.TurnId);
                if (string.IsNullOrEmpty(turnWinners.Winner1))
                {
                    command.Parameters.AddWithValue("winner1", DBNull.Value);
                    command.Parameters.AddWithValue("win1amount", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("winner1", turnWinners.Winner1);
                    command.Parameters.AddWithValue("win1amount", turnWinners.Win1Amount);
                }
                if (string.IsNullOrEmpty(turnWinners.Winner2))
                {
                    command.Parameters.AddWithValue("winner2", DBNull.Value);
                    command.Parameters.AddWithValue("win2amount", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("winner2", turnWinners.Winner2);
                    command.Parameters.AddWithValue("win2amount", turnWinners.Win2Amount);
                }
                if (string.IsNullOrEmpty(turnWinners.Winner3))
                {
                    command.Parameters.AddWithValue("winner3", DBNull.Value);
                    command.Parameters.AddWithValue("win3amount", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("winner3", turnWinners.Winner3);
                    command.Parameters.AddWithValue("win3amount", turnWinners.Win3Amount);
                }
                if (string.IsNullOrEmpty(turnWinners.Winner4))
                {
                    command.Parameters.AddWithValue("winner4", DBNull.Value);
                    command.Parameters.AddWithValue("win4amount", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("winner4", turnWinners.Winner4);
                    command.Parameters.AddWithValue("win4amount", turnWinners.Win4Amount);
                }
                if (string.IsNullOrEmpty(turnWinners.Winner5))
                {
                    command.Parameters.AddWithValue("winner5", DBNull.Value);
                    command.Parameters.AddWithValue("win5amount", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("winner5", turnWinners.Winner5);
                    command.Parameters.AddWithValue("win5amount", turnWinners.Win5Amount);
                }
                if (string.IsNullOrEmpty(turnWinners.Winner6))
                {
                    command.Parameters.AddWithValue("winner6", DBNull.Value);
                    command.Parameters.AddWithValue("win6amount", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("winner6", turnWinners.Winner6);
                    command.Parameters.AddWithValue("win6amount", turnWinners.Win6Amount);
                }
                if (string.IsNullOrEmpty(turnWinners.Winner7))
                {
                    command.Parameters.AddWithValue("winner7", DBNull.Value);
                    command.Parameters.AddWithValue("win7amount", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("winner7", turnWinners.Winner7);
                    command.Parameters.AddWithValue("win7amount", turnWinners.Win7Amount);
                }
                if (string.IsNullOrEmpty(turnWinners.Winner8))
                {
                    command.Parameters.AddWithValue("winner8", DBNull.Value);
                    command.Parameters.AddWithValue("win8amount", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("winner8", turnWinners.Winner8);
                    command.Parameters.AddWithValue("win8amount", turnWinners.Win8Amount);
                }
                
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
            throw new System.NotImplementedException();
        }
    }
}