using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameMove : ParentObject
    {
        private int turnId;
        private int moveSequenceNo;
        private string playerName;
        private float currentBalance;
        private float moveTime;
        public Move move;
        private float amount;
        private float totalRake;
        private float userRakeBack;
        private float parentRakeBack;
        private float profit;

        public GameMove()
        {
            
        }
        public GameMove(int turnId,int moveSequenceNo,string playerName,float currentBalance,float moveTime, Move move, float amount,float totalRake,float userRakeBack,float parentRakeBack, float profit)
        {
            this.turnId = turnId;
            this.moveSequenceNo = moveSequenceNo;
            this.playerName = playerName;
            this.currentBalance = currentBalance;
            this.moveTime = moveTime;
            this.move = move;
            this.amount = amount;
            this.totalRake = totalRake;
            this.userRakeBack = userRakeBack;
            this.parentRakeBack = parentRakeBack;
            this.profit = profit;
        }
        

        
        public float CurrentBalance
        {
            get => currentBalance;
            set => currentBalance = value;
        }

        public int TurnId
        {
            get => turnId;
            set => turnId = value;
        }

        public int MoveSequenceNo
        {
            get => moveSequenceNo;
            set => moveSequenceNo = value;
        }

        public string PlayerName
        {
            get => playerName;
            set => playerName = value;
        }

        public float MoveTime
        {
            get => moveTime;
            set => moveTime = value;
        }

        public Move Move
        {
            get => move;
            set => move = value;
        }

        public float Amount
        {
            get => amount;
            set => amount = value;
        }

        public float Profit
        {
            get => profit;
            set => profit = value;
        }

        public float TotalRake
        {
            get => totalRake;
            set => totalRake = value;
        }

        public float UserRakeBack
        {
            get => userRakeBack;
            set => userRakeBack = value;
        }

        public float ParentRakeBack
        {
            get => parentRakeBack;
            set => parentRakeBack = value;
        }
    }
    
    public enum Move
    {
        None = 0,
        Fold = 1,
        Check = 2,
        Call = 3,
        Raise = 4,
        AllIn = 5,
        SmallBlind = 6,
        BigBlind = 7,
        ShowCards = 8,
        HideCards = 9
    }
    public enum ToggleMove
    {
        None = 0,
        FoldCheck = 1,
        Check = 2,
        Call = 3
    }
}