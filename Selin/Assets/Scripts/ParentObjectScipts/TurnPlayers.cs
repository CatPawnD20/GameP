using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class TurnPlayers : ParentObject
    {
        private int turnID;
        private string player1 = String.Empty;
        private float player1Balance = 0;
        private SeatLocations player1Seat = SeatLocations.None;
        private string player2 = String.Empty;
        private float player2Balance = 0;
        private SeatLocations player2Seat = SeatLocations.None;
        private string player3 = String.Empty;
        private float player3Balance = 0;
        private SeatLocations player3Seat = SeatLocations.None;
        private string player4 = String.Empty;
        private float player4Balance = 0;
        private SeatLocations player4Seat = SeatLocations.None;
        private string player5  = String.Empty;
        private float player5Balance = 0;
        private SeatLocations player5Seat = SeatLocations.None;
        private string player6 = String.Empty;
        private float player6Balance = 0;
        private SeatLocations player6Seat = SeatLocations.None;
        private string player7 = String.Empty;
        private float player7Balance = 0;
        private SeatLocations player7Seat = SeatLocations.None;
        private string player8 = String.Empty;
        private float player8Balance = 0;
        private SeatLocations player8Seat = SeatLocations.None;

        public TurnPlayers()
        {
            
        }

        public TurnPlayers(int turnID, Dictionary<string, float> playerAndChipAmountMap,
            Dictionary<SeatLocations, float> seatLocationAndChipAmountMap)
        {
            this.turnID = turnID;
            int playerCount = playerAndChipAmountMap.Keys.Count;
            if (playerCount == 2)
            {
                set2playerTurnPlayers(playerAndChipAmountMap, seatLocationAndChipAmountMap);
            }
            else if (playerCount == 3)
            {
                set3playerTurnPlayers(playerAndChipAmountMap, seatLocationAndChipAmountMap);
            }
            else if (playerCount == 4)
            {
                set4playerTurnPlayers(playerAndChipAmountMap, seatLocationAndChipAmountMap);
            }
            else if (playerCount == 5)
            {
                set5playerTurnPlayers(playerAndChipAmountMap, seatLocationAndChipAmountMap);
            }
            else if (playerCount == 6)
            {
                set6playerTurnPlayers(playerAndChipAmountMap, seatLocationAndChipAmountMap);
            }
            else if (playerCount == 7)
            {
                set7playerTurnPlayers(playerAndChipAmountMap, seatLocationAndChipAmountMap);
            }
            else if (playerCount == 8)
            {
                set8playerTurnPlayers(playerAndChipAmountMap, seatLocationAndChipAmountMap);
            }
            
        }

        private void set8playerTurnPlayers(Dictionary<string, float> playerAndChipAmountMap, Dictionary<SeatLocations, float> seatLocationAndChipAmountMap)
        {
            player1 = playerAndChipAmountMap.Keys.ElementAt(0);
            player1Balance = playerAndChipAmountMap.Values.ElementAt(0);
            player1Seat = seatLocationAndChipAmountMap.Keys.ElementAt(0);

            player2 = playerAndChipAmountMap.Keys.ElementAt(1);
            player2Balance = playerAndChipAmountMap.Values.ElementAt(1);
            player2Seat = seatLocationAndChipAmountMap.Keys.ElementAt(1);

            player3 = playerAndChipAmountMap.Keys.ElementAt(2);
            player3Balance = playerAndChipAmountMap.Values.ElementAt(2);
            player3Seat = seatLocationAndChipAmountMap.Keys.ElementAt(2);

            player4 = playerAndChipAmountMap.Keys.ElementAt(3);
            player4Balance = playerAndChipAmountMap.Values.ElementAt(3);
            player4Seat = seatLocationAndChipAmountMap.Keys.ElementAt(3);

            player5 = playerAndChipAmountMap.Keys.ElementAt(4);
            player5Balance = playerAndChipAmountMap.Values.ElementAt(4);
            player5Seat = seatLocationAndChipAmountMap.Keys.ElementAt(4);

            player6 = playerAndChipAmountMap.Keys.ElementAt(5);
            player6Balance = playerAndChipAmountMap.Values.ElementAt(5);
            player6Seat = seatLocationAndChipAmountMap.Keys.ElementAt(5);

            player7 = playerAndChipAmountMap.Keys.ElementAt(6);
            player7Balance = playerAndChipAmountMap.Values.ElementAt(6);
            player7Seat = seatLocationAndChipAmountMap.Keys.ElementAt(6);

            player8 = playerAndChipAmountMap.Keys.ElementAt(7);
            player8Balance = playerAndChipAmountMap.Values.ElementAt(7);
            player8Seat = seatLocationAndChipAmountMap.Keys.ElementAt(7);
        }

        private void set7playerTurnPlayers(Dictionary<string, float> playerAndChipAmountMap, Dictionary<SeatLocations, float> seatLocationAndChipAmountMap)
        {
            player1 = playerAndChipAmountMap.Keys.ElementAt(0);
            player1Balance = playerAndChipAmountMap.Values.ElementAt(0);
            player1Seat = seatLocationAndChipAmountMap.Keys.ElementAt(0);

            player2 = playerAndChipAmountMap.Keys.ElementAt(1);
            player2Balance = playerAndChipAmountMap.Values.ElementAt(1);
            player2Seat = seatLocationAndChipAmountMap.Keys.ElementAt(1);

            player3 = playerAndChipAmountMap.Keys.ElementAt(2);
            player3Balance = playerAndChipAmountMap.Values.ElementAt(2);
            player3Seat = seatLocationAndChipAmountMap.Keys.ElementAt(2);

            player4 = playerAndChipAmountMap.Keys.ElementAt(3);
            player4Balance = playerAndChipAmountMap.Values.ElementAt(3);
            player4Seat = seatLocationAndChipAmountMap.Keys.ElementAt(3);

            player5 = playerAndChipAmountMap.Keys.ElementAt(4);
            player5Balance = playerAndChipAmountMap.Values.ElementAt(4);
            player5Seat = seatLocationAndChipAmountMap.Keys.ElementAt(4);

            player6 = playerAndChipAmountMap.Keys.ElementAt(5);
            player6Balance = playerAndChipAmountMap.Values.ElementAt(5);
            player6Seat = seatLocationAndChipAmountMap.Keys.ElementAt(5);

            player7 = playerAndChipAmountMap.Keys.ElementAt(6);
            player7Balance = playerAndChipAmountMap.Values.ElementAt(6);
            player7Seat = seatLocationAndChipAmountMap.Keys.ElementAt(6);
        }

        private void set6playerTurnPlayers(Dictionary<string, float> playerAndChipAmountMap, Dictionary<SeatLocations, float> seatLocationAndChipAmountMap)
        {
            player1 = playerAndChipAmountMap.Keys.ElementAt(0);
            player1Balance = playerAndChipAmountMap.Values.ElementAt(0);
            player1Seat = seatLocationAndChipAmountMap.Keys.ElementAt(0);

            player2 = playerAndChipAmountMap.Keys.ElementAt(1);
            player2Balance = playerAndChipAmountMap.Values.ElementAt(1);
            player2Seat = seatLocationAndChipAmountMap.Keys.ElementAt(1);

            player3 = playerAndChipAmountMap.Keys.ElementAt(2);
            player3Balance = playerAndChipAmountMap.Values.ElementAt(2);
            player3Seat = seatLocationAndChipAmountMap.Keys.ElementAt(2);

            player4 = playerAndChipAmountMap.Keys.ElementAt(3);
            player4Balance = playerAndChipAmountMap.Values.ElementAt(3);
            player4Seat = seatLocationAndChipAmountMap.Keys.ElementAt(3);

            player5 = playerAndChipAmountMap.Keys.ElementAt(4);
            player5Balance = playerAndChipAmountMap.Values.ElementAt(4);
            player5Seat = seatLocationAndChipAmountMap.Keys.ElementAt(4);

            player6 = playerAndChipAmountMap.Keys.ElementAt(5);
            player6Balance = playerAndChipAmountMap.Values.ElementAt(5);
            player6Seat = seatLocationAndChipAmountMap.Keys.ElementAt(5);
        }

        private void set5playerTurnPlayers(Dictionary<string, float> playerAndChipAmountMap, Dictionary<SeatLocations, float> seatLocationAndChipAmountMap)
        {
            player1 = playerAndChipAmountMap.Keys.ElementAt(0);
            player1Balance = playerAndChipAmountMap.Values.ElementAt(0);
            player1Seat = seatLocationAndChipAmountMap.Keys.ElementAt(0);

            player2 = playerAndChipAmountMap.Keys.ElementAt(1);
            player2Balance = playerAndChipAmountMap.Values.ElementAt(1);
            player2Seat = seatLocationAndChipAmountMap.Keys.ElementAt(1);

            player3 = playerAndChipAmountMap.Keys.ElementAt(2);
            player3Balance = playerAndChipAmountMap.Values.ElementAt(2);
            player3Seat = seatLocationAndChipAmountMap.Keys.ElementAt(2);

            player4 = playerAndChipAmountMap.Keys.ElementAt(3);
            player4Balance = playerAndChipAmountMap.Values.ElementAt(3);
            player4Seat = seatLocationAndChipAmountMap.Keys.ElementAt(3);

            player5 = playerAndChipAmountMap.Keys.ElementAt(4);
            player5Balance = playerAndChipAmountMap.Values.ElementAt(4);
            player5Seat = seatLocationAndChipAmountMap.Keys.ElementAt(4);
        }

        private void set4playerTurnPlayers(Dictionary<string, float> playerAndChipAmountMap, Dictionary<SeatLocations, float> seatLocationAndChipAmountMap)
        {
            player1 = playerAndChipAmountMap.Keys.ElementAt(0);
            player1Balance = playerAndChipAmountMap.Values.ElementAt(0);
            player1Seat = seatLocationAndChipAmountMap.Keys.ElementAt(0);

            player2 = playerAndChipAmountMap.Keys.ElementAt(1);
            player2Balance = playerAndChipAmountMap.Values.ElementAt(1);
            player2Seat = seatLocationAndChipAmountMap.Keys.ElementAt(1);

            player3 = playerAndChipAmountMap.Keys.ElementAt(2);
            player3Balance = playerAndChipAmountMap.Values.ElementAt(2);
            player3Seat = seatLocationAndChipAmountMap.Keys.ElementAt(2);

            player4 = playerAndChipAmountMap.Keys.ElementAt(3);
            player4Balance = playerAndChipAmountMap.Values.ElementAt(3);
            player4Seat = seatLocationAndChipAmountMap.Keys.ElementAt(3);
        }

        private void set3playerTurnPlayers(Dictionary<string, float> playerAndChipAmountMap, Dictionary<SeatLocations, float> seatLocationAndChipAmountMap)
        {
            player1 = playerAndChipAmountMap.Keys.ElementAt(0);
            player1Balance = playerAndChipAmountMap.Values.ElementAt(0);
            player1Seat = seatLocationAndChipAmountMap.Keys.ElementAt(0);
            player2 = playerAndChipAmountMap.Keys.ElementAt(1);
            player2Balance = playerAndChipAmountMap.Values.ElementAt(1);
            player2Seat = seatLocationAndChipAmountMap.Keys.ElementAt(1);
            player3 = playerAndChipAmountMap.Keys.ElementAt(2);
            player3Balance = playerAndChipAmountMap.Values.ElementAt(2);
            player3Seat = seatLocationAndChipAmountMap.Keys.ElementAt(2);
        }


        private void set2playerTurnPlayers(Dictionary<string, float> playerAndChipAmountMap,
            Dictionary<SeatLocations, float> seatLocationAndChipAmountMap)
        {
            player1 = playerAndChipAmountMap.Keys.ElementAt(0);
            player1Balance = playerAndChipAmountMap.Values.ElementAt(0);
            player1Seat = seatLocationAndChipAmountMap.Keys.ElementAt(0);
            player2 = playerAndChipAmountMap.Keys.ElementAt(1);
            player2Balance = playerAndChipAmountMap.Values.ElementAt(1);
            player2Seat = seatLocationAndChipAmountMap.Keys.ElementAt(1);
        }
        

        public int TurnID
        {
            get => turnID;
            set => turnID = value;
        }

        public SeatLocations Player1Seat
        {
            get => player1Seat;
            set => player1Seat = value;
        }

        public SeatLocations Player2Seat
        {
            get => player2Seat;
            set => player2Seat = value;
        }

        public SeatLocations Player3Seat
        {
            get => player3Seat;
            set => player3Seat = value;
        }

        public SeatLocations Player4Seat
        {
            get => player4Seat;
            set => player4Seat = value;
        }

        public SeatLocations Player5Seat
        {
            get => player5Seat;
            set => player5Seat = value;
        }

        public SeatLocations Player6Seat
        {
            get => player6Seat;
            set => player6Seat = value;
        }

        public SeatLocations Player7Seat
        {
            get => player7Seat;
            set => player7Seat = value;
        }

        public SeatLocations Player8Seat
        {
            get => player8Seat;
            set => player8Seat = value;
        }

        public string Player1
        {
            get => player1;
            set => player1 = value;
        }

        public float Player1Balance
        {
            get => player1Balance;
            set => player1Balance = value;
        }

        public string Player2
        {
            get => player2;
            set => player2 = value;
        }

        public float Player2Balance
        {
            get => player2Balance;
            set => player2Balance = value;
        }

        public string Player3
        {
            get => player3;
            set => player3 = value;
        }

        public float Player3Balance
        {
            get => player3Balance;
            set => player3Balance = value;
        }

        public string Player4
        {
            get => player4;
            set => player4 = value;
        }

        public float Player4Balance
        {
            get => player4Balance;
            set => player4Balance = value;
        }

        public string Player5
        {
            get => player5;
            set => player5 = value;
        }

        public float Player5Balance
        {
            get => player5Balance;
            set => player5Balance = value;
        }

        public string Player6
        {
            get => player6;
            set => player6 = value;
        }

        public float Player6Balance
        {
            get => player6Balance;
            set => player6Balance = value;
        }

        public string Player7
        {
            get => player7;
            set => player7 = value;
        }

        public float Player7Balance
        {
            get => player7Balance;
            set => player7Balance = value;
        }

        public string Player8
        {
            get => player8;
            set => player8 = value;
        }

        public float Player8Balance
        {
            get => player8Balance;
            set => player8Balance = value;
        }
    }
}