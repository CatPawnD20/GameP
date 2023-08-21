using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Table nesnesi için temel sınıftır. Parent object'i extend eder. Bu sayede ID değişkeni olması sağlanır.
 * 2 constructor'u bulunur
 * Tuttuğu değişkenler :
 * - id         --> from parentObject
 * - seatCount
 * - smallBlind
 * - bigBlind
 * - creatorName -->>> dataBase'e admin olarak işlenmiştir
 * - matchId
 * - networkMatchId -->>
 * - minDeposit
 * - password
 * - hasPassword
 */

namespace Assets.Scripts
{
    public class Table : ParentObject
    {
        private int seatCount;
        private float smallBlind;
        private float bigBlind;
        private string creatorName;
        private Guid networkMatchId;
        private string matchId;
        private float minDeposit;
        private string password;
        private bool hasPassword;
        private float tableRakePercent;

        public Table()
        {
            
        }

        public Table(string creatorName,string matchId,int seatCount,float smallBlind,Guid networkMatchId,string password)
        {
            tableRakePercent = 4;
            this.creatorName = creatorName;
            this.matchId = matchId;
            this.seatCount = seatCount;
            this.smallBlind = smallBlind;
            bigBlind = smallBlind * 2;
            this.networkMatchId = networkMatchId;
            minDeposit = smallBlind * 50;
            if (password == null)
            {
                this.hasPassword = false;
                this.password = password;
            }
            else
            {
                this.hasPassword = true;
                this.password = password;
            }
        }


        public float TableRakePercent
        {
            get => tableRakePercent;
            set => tableRakePercent = value;
        }

        public bool HasPassword
        {
            get => hasPassword;
            set => hasPassword = value;
        }

        public string Password
        {
            get => password;
            set => password = value;
        }

        public float MinDeposit
        {
            get => minDeposit;
            set => minDeposit = value;
        }

        public string MatchId
        {
            get => matchId;
            set => matchId = value;
        }

        public Guid NetworkMatchId
        {
            get => networkMatchId;
            set => networkMatchId = value;
        }

        public int SeatCount
        {
            get => seatCount;
            set => seatCount = value;
        }

        public float SmallBlind
        {
            get => smallBlind;
            set => smallBlind = value;
        }

        public float BigBlind
        {
            get => bigBlind;
            set => bigBlind = value;
        }

        public string CreatorName
        {
            get => creatorName;
            set => creatorName = value;
        }
        
    }
    public enum SeatLocations
    {
        BottomRight = 1,
        Bottom = 2,
        BottomLeft = 3,
        LeftBottom = 4,
        Left = 5,
        LeftTop = 6,
        TopLeft = 7,
        Top = 8,
        TopRight = 9,
        RightTop = 10,
        Right = 11,
        RightBottom = 12,
        None = 0
    }
    
}