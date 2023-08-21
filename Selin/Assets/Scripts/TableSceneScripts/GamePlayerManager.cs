using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    /*
     * GameScene'i aktif olan(masaya giriş yapmış) playerların organizasyonundan sorumludur.
     * Bu işlemler için bir liste ve bir map tutar
     * Tuttuğu Map
     *      -->>> SeatAndPlayerMap -->>> koltuk konumuna karşılık player adını tutar.
     *      Server tarafında ADD-REMOVE işlemleri ile idare edilirken.
     *      Client tarafında UPDATE işlemi ile idare edilir. UPDATE işlemi TableGameManager'in SyncList'lerini temel alır
     * Tuttuğu List
     *      -->>> GamePlayerList -->>> oyunu aktif olarak oynayan player'ları tutar.
     *      Server tarafında ADD-REMOVE işlemleri ile idare edilirken.
     *      Client tarafında UPDATE işlemi ile idare edilir. UPDATE işlemi TableGameManager'in SyncList'lerini temel alır
     */
    public class GamePlayerManager : MonoBehaviour
    {
        public static GamePlayerManager instance;
        [SerializeField] private string syncID;
        [SerializeField] private List<string> gamePlayerList = new List<string>();
        private IDictionary<SeatLocations,string> seatAndPlayerMap = new Dictionary<SeatLocations,string>();
        private IDictionary<string,int> playerAndGameParticipationIDMap = new Dictionary<string, int>(); 
        [SerializeField] private List<string> leaverPlayerQueueList = new List<string>();
        ///////////////////////////////////////////////////Events Section/////////////////////////////////////////////////////
        // Start is called before the first frame update
        void Start()
        {
            instance = this;
        }
        ///////////////////////////////////////////////////playerAndGameParticipationIDMap Section/////////////////////////////////////////////////////
        public bool AddPlayerToPlayerAndGameParticipationIDMap(string playerName,int gameParticipationID)
        {
            if (playerAndGameParticipationIDMap.ContainsKey(playerName))
            {
                Debug.LogWarning("GamePlayerManager.cs -->>> SyncID : " + SyncID + "-->>>AddPlayerToPlayerAndGameParticipationIDMap" +
                                 "Player is already exist in playerAndGameParticipationIDMap!!! : PlayerName -> " + playerName);
                return false;
            }
            else
            {
                playerAndGameParticipationIDMap.Add(playerName,gameParticipationID);
                return true;
            }
        }
        ///////////////////////////////////////////////////GamePlayerList Section/////////////////////////////////////////////////////
        public void ClearUpdateGamePlayerList(List<Seat> seatList)
        {
            gamePlayerList.Clear();
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    gamePlayerList.Add(seat.username);
                }
            }
        }
        
        public bool AddPlayerToGamePlayerList(string playerName)
        {
            if (gamePlayerList.Contains(playerName))
            {
                Debug.LogWarning("GamePlayerManager.cs -->>> SyncID : " + SyncID + "-->>>AddPlayerToGamePlayerList" +
                                 "Player is already exist in playerList!!! : PlayerName -> " + playerName);
                return false;
            }
            else
            {
                gamePlayerList.Add(playerName);
                return true;
            }
        }
        
        public bool isPlayerOnGamePlayerList(string playerName)
        {
            if (gamePlayerList.Contains(playerName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        
        ///////////////////////////////////////////////////SeatAndPlayerMap Section/////////////////////////////////////////////////////
        public void ClearUpdateSeatAndPlayerMap(List<Seat> seatList)
        {
            seatAndPlayerMap.Clear();
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    seatAndPlayerMap.Add(seat.location,seat.username);
                }
            }
        }
        public bool AddPlayerToSeatAndPlayerMap(SeatLocations requestedSeat, string playerName)
        {
            if (seatAndPlayerMap.ContainsKey(requestedSeat))
            {
                Debug.LogWarning("GamePlayerManager.cs -->>> SyncID : " + SyncID + "-->>>AddPlayerToSeatAndPlayerMap" +
                                 "Seat already taken!!! : SeatLocation -> " + requestedSeat);
                return false;
            }
            else
            {
                seatAndPlayerMap.Add(requestedSeat,playerName);
                return true;
            }
        }
        
        public bool isSeatTaken(SeatLocations seatLocation)
        {
            if (SeatAndPlayerMap.ContainsKey(seatLocation))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public string GetPlayerBySeatLocationSeatAndPlayerMap(SeatLocations seatLocation)
        {
            if (seatAndPlayerMap.ContainsKey(seatLocation))
            {
                return seatAndPlayerMap[seatLocation];
            }
            else
            {
                Debug.LogWarning("GamePlayerManager.cs -->>> GetFromSeatAndPlayerMap -->>>  key :  " +seatLocation + 
                                 "bulunamadı. Null geri dönderildi!!!");
                return null;
            }
        }
        
        ///////////////////////////////////////////////////Properties Section/////////////////////////////////////////////////////
        public IDictionary<SeatLocations, string> SeatAndPlayerMap
        {
            get => seatAndPlayerMap;
            set => seatAndPlayerMap = value;
        }

        public List<string> GamePlayerList
        {
            get => gamePlayerList;
            set => gamePlayerList = value;
        }
        public string SyncID
        {
            get => syncID;
            set => syncID = value;
        }

        public int GetGameParticipationId(string playerName)
        {
            if (playerAndGameParticipationIDMap.ContainsKey(playerName))
            {
                return playerAndGameParticipationIDMap[playerName];
            }
            else
            {
                Debug.LogWarning("GamePlayerManager.cs -->>> GetGameParticipationId -->>>  key :  " +playerName + 
                                 "bulunamadı. -1 geri dönderildi!!!");
                return -1;
            }
        }

        public void RemovePlayerFromSeatAndPlayerMap(int seatNumber)
        {
            if (seatAndPlayerMap.ContainsKey((SeatLocations) seatNumber))
            {
                seatAndPlayerMap.Remove((SeatLocations) seatNumber);
            }
            else
            {
                Debug.LogWarning("GamePlayerManager.cs -->>> RemovePlayerFromSeatAndPlayerMap -->>>  key :  " +seatNumber + 
                                 "bulunamadı. Hiçbir işlem yapılmadı!!!");
            }
        }

        public void RemovePlayerFromGamePlayerList(string playerName)
        {
            if (gamePlayerList.Contains(playerName))
            {
                gamePlayerList.Remove(playerName);
            }
            else
            {
                Debug.LogWarning("GamePlayerManager.cs -->>> RemovePlayerFromGamePlayerList -->>>  key :  " +playerName + 
                                 "bulunamadı. Hiçbir işlem yapılmadı!!!");
            }
        }

        public void AddPlayerToLeaverPlayerQueue(string playerName)
        {
            if (leaverPlayerQueueList.Contains(playerName))
            {
                Debug.LogWarning("GamePlayerManager.cs -->>> AddPlayerToLeaverPlayerQueue -->>>  key :  " +playerName + 
                                 "zaten listede bulunuyor. Hiçbir işlem yapılmadı!!!");
            }
            else
            {
                leaverPlayerQueueList.Add(playerName);
            }
        }

        public void RemovePlayerFromGameParticipationIDMap(string playerName)
        {
            if (playerAndGameParticipationIDMap.ContainsKey(playerName))
            {
                playerAndGameParticipationIDMap.Remove(playerName);
            }
            else
            {
                Debug.LogWarning("GamePlayerManager.cs -->>> RemovePlayerFromGameParticipationIDMap -->>>  key :  " +playerName + 
                                 "bulunamadı. Hiçbir işlem yapılmadı!!!");
            }
        }

        public bool IsPlayerOnLeaverQueue(string playerName)
        {
            if (leaverPlayerQueueList.Contains(playerName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<string> GetLeaverQueuePlayerList()
        {
            return leaverPlayerQueueList;
        }

        public string GetAnotherPlayerName(string playerName)
        {
            List<string> currentGamePlayerList = new List<string>(gamePlayerList);
            List<string> currentLeaverPlayerQueueList = new List<string>(leaverPlayerQueueList);
            if (currentGamePlayerList.Count == 0)
            {
                Debug.LogWarning("GamePlayerManager.cs -->>> GetAnotherPlayerName -->>>  gamePlayerList.Count == 1 ");
                return playerName;
            }
            List<string> anotherPlayerListWhoIsNotInLeaverQueue = new List<string>();
            foreach (var anotherPlayer in currentGamePlayerList)
            {
                if (!currentLeaverPlayerQueueList.Contains(anotherPlayer))
                {
                    anotherPlayerListWhoIsNotInLeaverQueue.Add(anotherPlayer);
                }
            }
            foreach (var anotherPlayer in anotherPlayerListWhoIsNotInLeaverQueue)
            {
                if (!string.Equals(anotherPlayer,playerName))
                {
                    return anotherPlayer;
                }
            }
            return playerName;
        }

        public bool CheckPlayerIsOnLeaverQueue(string currentHasTurnPlayer)
        {
            if (leaverPlayerQueueList.Contains(currentHasTurnPlayer))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void RemovePlayerFromLeaverPlayerQueue(string playerName)
        {
            if (leaverPlayerQueueList.Contains(playerName))
            {
                leaverPlayerQueueList.Remove(playerName);
            }
            else
            {
                Debug.LogWarning("GamePlayerManager.cs -->>> RemovePlayerFromLeaverPlayerQueue -->>>  key :  " +playerName + 
                                 "bulunamadı. Hiçbir işlem yapılmadı!!!");
            }
        }
    }
}