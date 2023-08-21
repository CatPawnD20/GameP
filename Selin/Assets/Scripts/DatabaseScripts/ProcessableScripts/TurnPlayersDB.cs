using System;
using System.Collections;
using System.Collections.Generic;
using Npgsql;
using UnityEngine;

namespace Assets.Scripts
{
    public class TurnPlayersDB : IProcessable
    {
        private PostgreSQL postgreSQL;

        public TurnPlayersDB()
        {
            
        }

        public TurnPlayersDB(PostgreSQL postgreSQL)
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
                TurnPlayers turnPlayers = (TurnPlayers) parentObject;
                string query = "INSERT INTO turnplayers (turnid,player1,player1balance,player1seat,player2,player2balance,player2seat,player3,player3balance,player3seat,player4,player4balance,player4seat,player5,player5balance,player5seat,player6,player6balance,player6seat,player7,player7balance,player7seat,player8,player8balance,player8seat) " +
                               "VALUES (@turnid,@player1,@player1balance,@player1seat,@player2,@player2balance,@player2seat,@player3,@player3balance,@player3seat,@player4,@player4balance,@player4seat,@player5,@player5balance,@player5seat,@player6,@player6balance,@player6seat,@player7,@player7balance,@player7seat,@player8,@player8balance,@player8seat)";
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = postgreSQL.Connection;
                command.CommandText = query;
                command.Parameters.AddWithValue("turnid",turnPlayers.TurnID);
                if (string.IsNullOrEmpty(turnPlayers.Player1))
                {
                    command.Parameters.AddWithValue("player1",DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("player1",turnPlayers.Player1);
                }
                command.Parameters.AddWithValue("player1balance",turnPlayers.Player1Balance);
                if (turnPlayers.Player1Seat == SeatLocations.None)
                {
                    command.Parameters.AddWithValue("player1seat",DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("player1seat",turnPlayers.Player1Seat.ToString());
                }
                if (string.IsNullOrEmpty(turnPlayers.Player2))
                {
                    command.Parameters.AddWithValue("player2",DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("player2",turnPlayers.Player2);
                }
                command.Parameters.AddWithValue("player2balance",turnPlayers.Player2Balance);
                if (turnPlayers.Player2Seat == SeatLocations.None)
                {
                    command.Parameters.AddWithValue("player2seat",DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("player2seat",turnPlayers.Player2Seat.ToString());
                }
                if (string.IsNullOrEmpty(turnPlayers.Player3))
                {
                    command.Parameters.AddWithValue("player3",DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("player3",turnPlayers.Player3);
                }
                command.Parameters.AddWithValue("player3balance",turnPlayers.Player3Balance);
                if (turnPlayers.Player3Seat == SeatLocations.None)
                {
                    command.Parameters.AddWithValue("player3seat",DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("player3seat",turnPlayers.Player3Seat.ToString());
                }
                if (string.IsNullOrEmpty(turnPlayers.Player4))
                {
                    command.Parameters.AddWithValue("player4",DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("player4",turnPlayers.Player4);
                }
                command.Parameters.AddWithValue("player4balance",turnPlayers.Player4Balance);
                if (turnPlayers.Player4Seat == SeatLocations.None)
                {
                    command.Parameters.AddWithValue("player4seat",DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("player4seat",turnPlayers.Player4Seat.ToString());
                }
                if (string.IsNullOrEmpty(turnPlayers.Player5))
                {
                    command.Parameters.AddWithValue("player5",DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("player5",turnPlayers.Player5);
                }
                command.Parameters.AddWithValue("player5balance",turnPlayers.Player5Balance);
                if (turnPlayers.Player5Seat == SeatLocations.None)
                {
                    command.Parameters.AddWithValue("player5seat",DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("player5seat",turnPlayers.Player5Seat.ToString());
                }
                if (string.IsNullOrEmpty(turnPlayers.Player6))
                {
                    command.Parameters.AddWithValue("player6",DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("player6",turnPlayers.Player6);
                }
                command.Parameters.AddWithValue("player6balance",turnPlayers.Player6Balance);
                if (turnPlayers.Player6Seat == SeatLocations.None)
                {
                    command.Parameters.AddWithValue("player6seat",DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("player6seat",turnPlayers.Player6Seat.ToString());
                }
                if (string.IsNullOrEmpty(turnPlayers.Player7))
                {
                    command.Parameters.AddWithValue("player7",DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("player7",turnPlayers.Player7);
                }
                command.Parameters.AddWithValue("player7balance",turnPlayers.Player7Balance);
                if (turnPlayers.Player7Seat == SeatLocations.None)
                {
                    command.Parameters.AddWithValue("player7seat",DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("player7seat",turnPlayers.Player7Seat.ToString());
                }
                if (string.IsNullOrEmpty(turnPlayers.Player8))
                {
                    command.Parameters.AddWithValue("player8",DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("player8",turnPlayers.Player8);
                }
                command.Parameters.AddWithValue("player8balance",turnPlayers.Player8Balance);
                if (turnPlayers.Player8Seat == SeatLocations.None)
                {
                    command.Parameters.AddWithValue("player8seat",DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("player8seat",turnPlayers.Player8Seat.ToString());
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