using System;
using System.Collections;
using System.Collections.Generic;
using kcp2k;
using Mirror;
using Telepathy;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts
{
    /*
     * Bu class online kullanıcı listesini tutar ve bu liste ile ilgili işlemleri yapar
     * Tuttuğu değişkenler
     * -OnlinePlayerList<string>  --->> SyncList         ---->>> Bu sebeple NetworkBehaviour'dır
     * IDictionary<int, string> playerHashMap = new Dictionary<int, string>();  --->>> ConnectionID'ye karşılık playerName tutar
     * Böylece giriş çıkış işlemlerinin kontrolunu kolaylaştırır.
     * Fonksiyonları
     * 1- isOnline
     * 2- addPlayerToOnlinePlayerList
     * 3- AddPlayerToPlayerHashMap
     * 4- RemovePlayerFromPlayerHashMap
     * 5- GetPlayerNameFromPlayerHashMap
     * 6- getValues
     * 7- UpdateOnlinePlayerSyncList
     * 8- setOnlinePlayerList
     */
    public class VisitorArea : NetworkBehaviour
    {
        ///////////////////////////////////////////////////Variable Section/////////////////////////////////////////////////////
        public static VisitorArea instance;

        public readonly SyncList<string> onlinePlayerList = new SyncList<string>();
        private IDictionary<string, string> playerAndTableMap = new Dictionary<string, string>();

        [SerializeField] private List<string> adminList = new List<string>();
        [SerializeField] private List<string> operatorList = new List<string>();
        
        //ConnID vs PlayerName
        private IDictionary<NetworkConnection, string> playerHashMap = new Dictionary<NetworkConnection, string>();
        
        private NetworkConnectionToClient missingPlayerId;

        private string missingPlayerName;
        private NetworkMatch networkMatch;
        [Header("SyncData")]
        [SerializeField] private string syncID;
        [SerializeField] private string NetworkMatchID;
        ///////////////////////////////////////////////////Callback Section/////////////////////////////////////////////////////

        private void Awake()
        {
            Debug.Log("VisitorArea.cs -->>> awake");
            networkMatch = GetComponent<NetworkMatch>();
            syncID = "lobby";
            networkMatch.matchId = GuidGenerator.syncIDToGuid(syncID);
            NetworkMatchID = networkMatch.matchId.ToString();
        }
        
        public void Start()
        {
            Debug.Log("VisitorArea.cs -->>> onStart -->>> clientte başlayacakmı : "+isClient);    
            /*
             * Server tarafında server online olduğu ilk anda çalışır.
             * Bunun sebebi LoginScene'de kendi adında bir GameObject'e sahiptir. Bu nesne hayata geldiği ilk anda 
             */
            if (isServer)
            {
                Debug.Log("VisitorArea.cs -->>> onStart -->>> isServer : "+isServer);
                instance = this;
                
                Debug.Log("VisitorArea.cs -->>> onStart -->>> isServerOnly : "+instance.GetComponent<NetworkIdentity>().isServerOnly);
                
            }

            if (isClient && isLocalPlayer)
            {
                Debug.Log("VisitorArea.cs -->>> onStart -->>> isClient && isLocalPlayer : "+isClient + isLocalPlayer);
            }
            
            /*
             * Client tarafında LobbyScene'e geçildiği anda çalışır.
             * Bunun sebebi bu scene'de VisitorAreaUI adlı gameObject'e iliştirilmiş olmasıdır.
             */
            if (isClient)
            {
                instance = this;
                Debug.Log("VisitorArea.cs -->>> Start -->>> isClient  : "+isClient );
            }
            
        }
        
        ///////////////////////////////////////////////////Function Section/////////////////////////////////////////////////////
        public string GetFromPlayerAndTableMap(string playerName)
        {
            if (playerAndTableMap.ContainsKey(playerName))
            {
                return playerAndTableMap[playerName];
            }
            else
            {
                
                Debug.LogWarning("VisitorArea.cs -->>> GetFromPlayerAndTableMap -->>>  key :  " +playerName + 
                                 "bulunamadı. Null geri dönderildi!!!");
                return null;
            }
        }
        public bool RemoveFromPlayerAndTableMap(string playerName)
        {
            if (playerAndTableMap.ContainsKey(playerName))
            {
                return playerAndTableMap.Remove(playerName);
            }
            else
            {
                Debug.LogWarning("VisitorArea.cs -->>> RemoveFromPlayerAndTableMap -->>>  key :  " +playerName + 
                                 "bulunamadı. false geri dönderildi!!!");
                return false;
            }
        }
        public void AddUserToOperatorList(string username)
        {
            operatorList.Add(username);
        }
        public bool isOperator(string playerName)
        {
            if (operatorList.Contains(playerName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public void AddUserToAdminList(string Username)
        {
            adminList.Add(Username);
        }
        public bool isAdmin(string playerName)
        {
            if (adminList.Contains(playerName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        /*
         * verilen connectionID playerHashMap'te yok ise, o key, value çiftini map'e ekler
         */
        public void AddPlayerToPlayerHashMap(NetworkConnection conn, string playerName)
        {
            if (playerHashMap.ContainsKey(conn))
            {
                Debug.LogWarning("Add işlemi başarısız!!!\nConnection playerHashMap'te zaten var");
            }
            else
            {
                playerHashMap.Add(conn,playerName);
            }
        }

        /*
         *verilen connection playerHashMap'te var ise, o key, value çiftini map'ten siler
         */
        public void RemovePlayerFromPlayerHashMap(NetworkConnection conn)
        {
            if (playerHashMap.ContainsKey(conn))
            {
                playerHashMap.Remove(conn);
            }
            else
            {
                Debug.LogWarning("Remove işlemi başarısız!!!\nConnection playerHashMap'te yok");
            }
            
        }

        /*
         * playerHashMap içinde, verilen connectionID'ye ait bir value bulunuyorsa, bu value ile parametre olarak gelen playerName'i set eder
         * ve true değerini geri dönderir 
         * Eğer bu connectionID'ye ait bir value yok ise false değerini geri dönderir. playerName bu durumda null'dur.
         */
        public bool GetPlayerNameFromPlayerHashMap(NetworkConnection conn,out string playerName)
        {
             return playerHashMap.TryGetValue(conn, out playerName);
        }
        public NetworkConnection GetConnFromPlayerHashMap(string username)
        {
            foreach (var key in playerHashMap.Keys)
            {
                if (playerHashMap[key] == username)
                {
                    return key as NetworkConnection;
                }
            }
            Debug.LogWarning("VisitorArea.cs -->>> GetConnFromPlayerHashMap -->>>  key :  " +username + 
                             "bulunamadı. Null geri dönderildi!!!");
            return null;
        }

        /*
         * playerHashMap'teki Value'ları bir Collection'a çeker ve bu collection'u geri dönderir
         */
        private ICollection<string> getValues(IDictionary<NetworkConnection, string> playerHashMap)
        {
            return playerHashMap.Values;
        }

        /*
         * playerHashMap'i kullanarak onlinePlayerList'i günceller
         */
        public void UpdateOnlinePlayerSyncList()
        {
            ICollection<string> dummyList = getValues(playerHashMap);
            onlinePlayerList.Clear();
            foreach (var name in dummyList)
            {
                AddPlayerToOnlinePlayerList(name);
            }
        }

         /*
         * Verilen username onlinePlayerList'te bulunur mu ? Bu kontrolu yapar
         */
        public bool isOnline(string username)
        {
            if (onlinePlayerList.Contains(username))
            {
                return true;
            }
            else
            {
                return false;
            }
            
            
        }
        
        /*
         * verilen List<string> ile onlinePlayerList'i set eder
         */
        private void setOnlinePlayerList(List<string> playerNames)
        {
            onlinePlayerList.Clear();
            if (playerNames != null)
            {
                foreach (var playerName in playerNames)
                {
                    AddPlayerToOnlinePlayerList(playerName);
                }
            }

        }
        
        /*
         * Verilen username'i onlinePlayerListe'sine ekler
         */
        private void AddPlayerToOnlinePlayerList(string username)
        {
            onlinePlayerList.Add(username);
        }

        ///////////////////////////////////////////////////Properties Section/////////////////////////////////////////////////////
        public IDictionary<NetworkConnection, string> PlayerHashMap
        {
            get => playerHashMap;
            set => playerHashMap = value;
        }

        public IDictionary<string, string> PlayerAndTableMap
        {
            get => playerAndTableMap;
            set => playerAndTableMap = value;
        }

        public string MissingPlayerName
        {
            get => missingPlayerName;
            set => missingPlayerName = value;
        }

        public NetworkConnectionToClient MissingPlayerId
        {
            get => missingPlayerId;
            set => missingPlayerId = value;
        }
    }
}