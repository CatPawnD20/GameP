using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public static class ISeat 
    {
        public static Seat TransferSeatData(Seat receiverSeat, Seat seat)
        {
            receiverSeat.balance = seat.balance;
            receiverSeat.username = seat.username;
            receiverSeat.location = seat.location;
            receiverSeat.isActive = seat.isActive;
            receiverSeat.moveTime = seat.moveTime;
            receiverSeat.isMyTurn = seat.isMyTurn;
            receiverSeat.isDealer = seat.isDealer;
            receiverSeat.lastMove = seat.lastMove;
            receiverSeat.lastMoveAmount = seat.lastMoveAmount;
            receiverSeat.isPlayerMovedInSubTurn = seat.isPlayerMovedInSubTurn;
            receiverSeat.totalBetInSubTurn = seat.totalBetInSubTurn;
            receiverSeat.isPlayerInGame = seat.isPlayerInGame;
            return receiverSeat;
        }
        public static  List<Seat> SummonSeatByCount(int seatCount, List<Seat> seatList)
        {
            if (seatCount == 6)
            {
                for (int i = 1; i <= 12; i++)
                {
                    if (i == 2 || i == 4 || i == 6 || i == 8 || i == 10 || i == 12)
                    {
                        continue;
                    }
                    Seat newSeat = CreateNewSeatByLocation((SeatLocations) i);
                    seatList.Add(newSeat);
                }
            }
            if (seatCount == 8)
            {
                for (int i = 1; i <= 12; i++)
                {
                    if (i == 2 || i == 5 || i == 8 || i == 11)
                    {
                        continue;
                    }
                    Seat newSeat = CreateNewSeatByLocation((SeatLocations) i);
                    seatList.Add(newSeat);
                }
            }
            return seatList;
        }
        public static Seat CreateNewSeatByLocation(SeatLocations location)
        {
            Seat newSeat = new Seat();
            newSeat.location = location;
            newSeat.username = String.Empty;
            newSeat.balance = 0;
            newSeat.isActive = false;
            newSeat.isMyTurn = false;
            newSeat.isDealer = false;
            newSeat.moveTime = 0;
            newSeat.lastMove = Move.None;
            newSeat.lastMoveAmount = 0;
            newSeat.isPlayerMovedInSubTurn = false;
            newSeat.totalBetInSubTurn = 0;
            newSeat.isPlayerInGame = false;
            return newSeat;
        }

        public static Seat TransferSeatLocation(Seat receiverSeat, Seat removedSeat)
        {
            receiverSeat.location = removedSeat.location;
            return receiverSeat;
        }
    }
    public struct Seat
    {
        public bool isActive;
        public string username;
        public float balance;
        public SeatLocations location;
        public bool isDealer;
        public bool isMyTurn;
        public float moveTime;
        public Move lastMove;
        public float lastMoveAmount;
        public float totalBetInSubTurn;
        public bool isPlayerMovedInSubTurn;
        public bool isPlayerInGame;

    }
}