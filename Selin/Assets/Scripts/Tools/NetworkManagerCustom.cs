using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts
{
    /*
     * Network Manager görevlerini özelleştirmek için yaratılmıştır.
     * Özelleştirilen görevler :
     * 1- OnServerDisconnect
     */
    public class NetworkManagerCustom : NetworkManager
    {
        
        public override void Start()
        {
            base.Start();
        }

        /*
         *Burda yapılan özelleştirme :
         * Bir player çıkış işlemi yaptıgında, işlemler player'in connectionID'si üzerinden yürüyor.
         * Burda yapılan, bu connection id'yi orjinal işlemi bozmadan bir değişkene atamaktan ibarettir.
         */
        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            VisitorArea.instance.MissingPlayerId = conn;
            string playerName;
            VisitorArea.instance.GetPlayerNameFromPlayerHashMap(conn, out playerName);
            GameObject playerGameObject = FindPlayerGameObjectByLocalPlayerName(playerName);
            Player player = playerGameObject.GetComponent<Player>();
            string matchID = VisitorArea.instance.GetFromPlayerAndTableMap(playerName);
            if (string.IsNullOrEmpty(matchID))
            {
                Debug.LogError("NetworkManagerCustom : matchID null veya boş geldi.");
                base.OnServerDisconnect(conn);
                
            }
            else
            {
                TableGameManager myTableGameManagerScript = SpawnManager.instance.GetTableGameManager(matchID);
                List<Seat> oldSeatList = myTableGameManagerScript.MySeatList;
                player.LowKeyLeaveGame(playerName,oldSeatList,matchID);
                StartCoroutine(player.WaitForLeaveGame(playerName,matchID, myTableGameManagerScript));
                StartCoroutine(WaitForPlayerLeaveTable(playerName,conn));
            }
            
        }
        private bool IsPlayerOnAnyTable(string playerName)
        {
            if (VisitorArea.instance.GetFromPlayerAndTableMap(playerName) == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        
        IEnumerator WaitForPlayerLeaveTable(string playerName,NetworkConnectionToClient conn)
        {
            Debug.LogError("Player'in Masadan Cıkması bekleniyor");
            yield return new WaitWhile(() => IsPlayerOnAnyTable(playerName));
            Debug.LogError("Player Masadan Cıktı");
            base.OnServerDisconnect(conn);
        }
        
        private GameObject FindPlayerGameObjectByLocalPlayerName(string anotherPlayerName)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                if (player.GetComponent<Player>().LocalPlayerName == anotherPlayerName)
                {
                    return player;
                }
            }
            return null;
        }
        
    }
}