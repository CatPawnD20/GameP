using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Npgsql;
using UnityEngine;

namespace Assets.Scripts
{
    public class TurnDB : IProcessable
    {
        private PostgreSQL postgreSQL;
        public TurnDB()
        {
            
        }
        public TurnDB(PostgreSQL postgreSQL)
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
            try
            {
                postgreSQL.connectDB();
                postgreSQL.openDB();
                Turn turn = (Turn) parentObject;
                string query =
                    "UPDATE turns SET enddate = @enddate,playtime = @playtime,totalpot = @totalpot,totalrakeback = @totalrakeback,profit = @profit " +
                    "WHERE id = @id";
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = postgreSQL.Connection;
                command.CommandText = query;
                command.Parameters.AddWithValue("id", turn.id);
                command.Parameters.AddWithValue("enddate", turn.EndDate);
                command.Parameters.AddWithValue("playtime", turn.PlayTime);
                command.Parameters.AddWithValue("totalpot", turn.TotalPot);
                command.Parameters.AddWithValue("totalrakeback", turn.TotalRakeBack);
                command.Parameters.AddWithValue("profit", turn.Profit);
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
                Turn turn = (Turn) parentObject;
                int turnid = 0;
                string query = "INSERT INTO turns (matchid,startdate,smallblind,dealer,smallb,bigb) " +
                               "VALUES (@matchid,@startdate,@smallblind,@dealer,@smallb,@bigb)" +
                               "RETURNING id";
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = postgreSQL.Connection;
                command.CommandText = query;
                command.Parameters.AddWithValue("matchid",turn.MatchID);
                command.Parameters.AddWithValue("startdate",turn.StartDate);
                command.Parameters.AddWithValue("smallblind",turn.SmallBlind);
                command.Parameters.AddWithValue("dealer",turn.Dealer);
                command.Parameters.AddWithValue("smallb",turn.SmallBlindPlayer);
                command.Parameters.AddWithValue("bigb",turn.BigBlindPlayer);
                command.Prepare();
                
                NpgsqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    turnid = dataReader.GetInt32(0);
                }
                postgreSQL.closeDB();
                return turnid;
            }
            catch (NpgsqlException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}