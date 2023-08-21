
using System;
using Npgsql;

namespace Assets.Scripts
{
    public class SharedCardsDB : IProcessable
    {
        private PostgreSQL postgreSQL;

        public SharedCardsDB()
        {
            
        }
        public SharedCardsDB(PostgreSQL postgreSQL)
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
                SharedCards sharedCards = (SharedCards) parentObject;
                string query = "INSERT INTO sharedcards (turnid,flop1suit,flop1value,flop2suit,flop2value,flop3suit,flop3value,turnsuit,turnvalue,riversuit,rivervalue) VALUES (@turnid,@flop1suit,@flop1value,@flop2suit,@flop2value,@flop3suit,@flop3value,@turnsuit,@turnvalue,@riversuit,@rivervalue)";
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = postgreSQL.Connection;
                command.CommandText = query;
                command.Parameters.AddWithValue("turnid", sharedCards.TurnId);
                if (sharedCards.Flop1Suit == CardData.Suit.None)
                {
                    command.Parameters.AddWithValue("flop1suit", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("flop1suit", sharedCards.Flop1Suit.ToString());
                }
                if (sharedCards.Flop1Value == CardData.Value.None)
                {
                    command.Parameters.AddWithValue("flop1value", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("flop1value", sharedCards.Flop1Value.ToString());
                }
                if (sharedCards.Flop2Suit == CardData.Suit.None)
                {
                    command.Parameters.AddWithValue("flop2suit", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("flop2suit", sharedCards.Flop2Suit.ToString());
                }
                if (sharedCards.Flop2Value == CardData.Value.None)
                {
                    command.Parameters.AddWithValue("flop2value", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("flop2value", sharedCards.Flop2Value.ToString());
                }
                if (sharedCards.Flop3Suit == CardData.Suit.None)
                {
                    command.Parameters.AddWithValue("flop3suit", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("flop3suit", sharedCards.Flop3Suit.ToString());
                }
                if (sharedCards.Flop3Value == CardData.Value.None)
                {
                    command.Parameters.AddWithValue("flop3value", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("flop3value", sharedCards.Flop3Value.ToString());
                }
                if (sharedCards.TurnSuit == CardData.Suit.None)
                {
                    command.Parameters.AddWithValue("turnsuit", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("turnsuit", sharedCards.TurnSuit.ToString());
                }
                if (sharedCards.TurnValue == CardData.Value.None)
                {
                    command.Parameters.AddWithValue("turnvalue", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("turnvalue", sharedCards.TurnValue.ToString());
                }
                if (sharedCards.RiverSuit == CardData.Suit.None)
                {
                    command.Parameters.AddWithValue("riversuit", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("riversuit", sharedCards.RiverSuit.ToString());
                }
                if (sharedCards.RiverValue == CardData.Value.None)
                {
                    command.Parameters.AddWithValue("rivervalue", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("rivervalue", sharedCards.RiverValue.ToString());
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