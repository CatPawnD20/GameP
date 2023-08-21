using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    /*
     * User nesnesi için temel sınıftır. Parent object'i extend eder. Bu sayede ID değişkeni olması sağlanır.
     * Bir tane parent user olabilirken birden çok child user'ının olabilmesine imkan verilmiştir.
     * 2 constructor'u bulunur
     * Tuttuğu değişkenler :
     * - id         --> from parentObject --> done
     * - username   --> done
     * - password   --> done
     * - balance
     * - userType
     * - parentName
     * - childName<List>
     */
     
    
    public class User : ParentObject
    {
        private string username;
        private string password;
        private float balance;
        private float rakePercent;
        private float rakeBackAmount;
        private string parent;
        private float parentPercent;
        private string creator;
        private UserTypes userType;
        private DateTime creationDate;
        
        
        public User()
        {
            
        }
        public User(int userID)
        {
            this.id = userID;
        }
        public User(string username, string password, float balance,float rakePercent, string parent,float parentPercent, string creator,DateTime creationDate,UserTypes userType)
        {
            this.username = username;
            this.password = password;
            this.balance = balance;
            this.rakePercent = rakePercent;
            this.parent = parent;
            this.parentPercent = parentPercent;
            this.creator = creator;
            this.creationDate = creationDate;
            this.userType = userType;
            this.rakeBackAmount = 0;
            
        }

        public bool hasParent()
        {
            if(string.IsNullOrEmpty(parent))
            {
                return false;
            }
            return true;
        }

        public float RakePercent
        {
            get => rakePercent;
            set => rakePercent = value;
        }

        public float ParentPercent
        {
            get => parentPercent;
            set => parentPercent = value;
        }

        public DateTime CreationDate
        {
            get => creationDate;
            set => creationDate = value;
        }

        public string Parent
        {
            get => parent;
            set => parent = value;
        }

        public string Creator
        {
            get => creator;
            set => creator = value;
        }

        public UserTypes UserType
        {
            get => userType;
            set => userType = value;
        }
        
        public string Username
        {
            get => username;
            set => username = value;
        }

        public string Password
        {
            get => password;
            set => password = value;
        }

        public float Balance
        {
            get => balance;
            set => balance = value;
        }

        public float RakeBackAmount
        {
            get => rakeBackAmount;
            set => rakeBackAmount = value;
        }
    }
    public enum UserTypes
    {
        Admin,
        Player,
        Operator
    }
}