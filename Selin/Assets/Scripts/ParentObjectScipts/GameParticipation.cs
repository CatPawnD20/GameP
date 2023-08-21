using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameParticipation : ParentObject
    {
        private string matchid;
        private string username;
        private DateTime joinDate;
        private float joinBalance;
        private DateTime leaveDate;
        private float leaveBalance;

        public GameParticipation(string matchid, string username, float joinBalance)
        {
            this.matchid = matchid;
            this.username = username;
            this.joinBalance = joinBalance;
            joinDate = DateTime.Now;
        }

        public GameParticipation(int id)
        {
            this.id = id;
        }

        public string Matchid
        {
            get => matchid;
            set => matchid = value;
        }

        public string Username
        {
            get => username;
            set => username = value;
        }

        public DateTime JoinDate
        {
            get => joinDate;
            set => joinDate = value;
        }

        public float JoinBalance
        {
            get => joinBalance;
            set => joinBalance = value;
        }

        public DateTime LeaveDate
        {
            get => leaveDate;
            set => leaveDate = value;
        }

        public float LeaveBalance
        {
            get => leaveBalance;
            set => leaveBalance = value;
        }
    }
}