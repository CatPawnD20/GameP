using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Turn : ParentObject
    {
        private string matchID;
        private DateTime startDate;
        private DateTime endDate;
        private float playTime;
        private float smallBlind;
        private float totalRakeBack;
        private float totalPot;
        private float profit;
        private string dealer;
        private string smallBlindPlayer;
        private string bigBlindPlayer;

        public Turn()
        {
            
        }

        public Turn(string matchID, float smallBlind, string dealer, string smallBlindPlayer, string bigBlindPlayer)
        {
            this.matchID = matchID;
            startDate = DateTime.Now;
            this.smallBlind = smallBlind;
            this.dealer = dealer;
            this.smallBlindPlayer = smallBlindPlayer;
            this.bigBlindPlayer = bigBlindPlayer;
        }

        public Turn(int turnID, DateTime turnEndDate, float playTime, float totalPot, float totalRakeBack, float totalProfit)
        {
            this.id = turnID;
            this.endDate = turnEndDate;
            this.playTime = playTime;
            this.totalPot = totalPot;
            this.totalRakeBack = totalRakeBack;
            this.profit = totalProfit;
        }

        public string MatchID
        {
            get => matchID;
            set => matchID = value;
        }

        public DateTime StartDate
        {
            get => startDate;
            set => startDate = value;
        }

        public DateTime EndDate
        {
            get => endDate;
            set => endDate = value;
        }

        public float PlayTime
        {
            get => playTime;
            set => playTime = value;
        }

        public float SmallBlind
        {
            get => smallBlind;
            set => smallBlind = value;
        }

        public float TotalRakeBack
        {
            get => totalRakeBack;
            set => totalRakeBack = value;
        }

        public float TotalPot
        {
            get => totalPot;
            set => totalPot = value;
        }

        public float Profit
        {
            get => profit;
            set => profit = value;
        }

        public string Dealer
        {
            get => dealer;
            set => dealer = value;
        }

        public string SmallBlindPlayer
        {
            get => smallBlindPlayer;
            set => smallBlindPlayer = value;
        }

        public string BigBlindPlayer
        {
            get => bigBlindPlayer;
            set => bigBlindPlayer = value;
        }
    }
}