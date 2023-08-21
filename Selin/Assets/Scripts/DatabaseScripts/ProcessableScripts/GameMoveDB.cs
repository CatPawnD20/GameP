using System;
using System.Collections;
using System.Collections.Generic;
using Npgsql;
using UnityEngine;


namespace Assets.Scripts
{
    public class GameMoveDB : IProcessable
    {
        private PostgreSQL postgreSQL;
        public GameMoveDB()
        {
            
        }

        public GameMoveDB(PostgreSQL postgreSQL)
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
                GameMove gameMove = (GameMove) parentObject;
                string query = "INSERT INTO moves (turnid,moveseqno,playername,currentbalance,movetime,move,amount,totalrake,userrakeback,parentrakeback,profit) " +
                               "VALUES (@turnid,@moveseqno,@playername,@currentbalance,@movetime,@move,@amount,@totalrake,@userrakeback,@parentrakeback,@profit)";
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = postgreSQL.Connection;
                command.CommandText = query;
                command.Parameters.AddWithValue("turnid",gameMove.TurnId);
                command.Parameters.AddWithValue("moveseqno",gameMove.MoveSequenceNo);
                command.Parameters.AddWithValue("playername",gameMove.PlayerName);
                command.Parameters.AddWithValue("currentbalance",gameMove.CurrentBalance);
                command.Parameters.AddWithValue("movetime",gameMove.MoveTime);
                command.Parameters.AddWithValue("move",gameMove.Move.ToString());
                command.Parameters.AddWithValue("amount",gameMove.Amount);
                command.Parameters.AddWithValue("totalrake",gameMove.TotalRake);
                command.Parameters.AddWithValue("userrakeback",gameMove.UserRakeBack);
                command.Parameters.AddWithValue("parentrakeback",gameMove.ParentRakeBack);
                command.Parameters.AddWithValue("profit",gameMove.Profit);
                
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