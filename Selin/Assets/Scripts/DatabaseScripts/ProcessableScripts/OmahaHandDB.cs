using System;
using System.Collections;
using System.Collections.Generic;
using Npgsql;
using UnityEngine;

namespace Assets.Scripts
{
    public class OmahaHandDB : IProcessable
    {
        private PostgreSQL postgreSQL;

        public OmahaHandDB()
        {
            
        }

        public OmahaHandDB(PostgreSQL postgreSQL)
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
                OmahaHand omahaHand = (OmahaHand) parentObject;
                string query = "INSERT INTO omahahands (turnid,playername,card1suit,card1value,card2suit,card2value,card3suit,card3value,card4suit,card4value) " +
                               "VALUES (@turnid,@playername,@card1suit,@card1value,@card2suit,@card2value,@card3suit,@card3value,@card4suit,@card4value)";
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = postgreSQL.Connection;
                command.CommandText = query;
                command.Parameters.AddWithValue("turnid",omahaHand.TurnId);
                command.Parameters.AddWithValue("playername",omahaHand.PlayerName);
                command.Parameters.AddWithValue("card1suit",omahaHand.Card1Suit.ToString());
                command.Parameters.AddWithValue("card1value",omahaHand.Card1Value.ToString());
                command.Parameters.AddWithValue("card2suit",omahaHand.Card2Suit.ToString());
                command.Parameters.AddWithValue("card2value",omahaHand.Card2Value.ToString());
                command.Parameters.AddWithValue("card3suit",omahaHand.Card3Suit.ToString());
                command.Parameters.AddWithValue("card3value",omahaHand.Card3Value.ToString());
                command.Parameters.AddWithValue("card4suit",omahaHand.Card4Suit.ToString());
                command.Parameters.AddWithValue("card4value",omahaHand.Card4Value.ToString());
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