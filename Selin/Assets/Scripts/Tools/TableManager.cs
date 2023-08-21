using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Mirror;
using UnityEngine;

namespace Assets.Scripts
{
    /*
     * Yaratılan masaları manage eden sınıftır.
     * IDictionary<string,Table> tableMap = new Dictionary<string,Table>(); -->>> matchID'ye karşılık table sınıfını tutan map barındırır.
     * 
     */
    public class TableManager : MonoBehaviour
    {
        public static TableManager instance;

        public void Start()
        {
            instance = this;
        }
        
        private IDictionary<string,Table> tableMap = new Dictionary<string,Table>();
        
        public string GetTablePassword(string matchID)
        {
            return tableMap[matchID].Password;
        }
        
        public bool isTableExist(string matchID)
        {
            return tableMap.ContainsKey(matchID);
        }
        
        public void AddTableToTableMap(Table newTable)
        {
            tableMap.Add(newTable.MatchId,newTable);
        }
        public Table CreateNewTable(string creatorName,string matchId,int seatCount,int smallBlind,Guid networkMatchId,string password)
        {
            Table newTable = new Table(creatorName,matchId, seatCount, smallBlind, networkMatchId,password);
            return newTable;
        }

        

        public string GenerateRandomMatchID()
        {
            string _id = string.Empty;
            for (int i = 0; i < 5; i++) {
                int random = UnityEngine.Random.Range (0, 36);
                if (random < 26) {
                    _id += (char) (random + 65);
                } else {
                    _id += (random - 26).ToString ();
                }
            }
            Debug.Log ($"Random Match ID: {_id}");
            return _id;

        }
        
        public bool isTableListFull()
        {
            if (tableMap.Count < 9)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public IDictionary<string, Table> TableMap
        {
            get => tableMap;
            set => tableMap = value;
        }


        
    }
}