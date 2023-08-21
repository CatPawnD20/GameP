using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts
{
    /*
     * GameScene'i aktif olan (masaya giriş yapmış) playerların iki tip görüntüsü olabilir.
     * 1- NonLocalPlayerPrefab
     * 2- LocalPlayerPrefab
     * Bu görüntülerin spawn edilmesinden sorumludur.
     */
    public class GamePlayerSpawnManager : MonoBehaviour
    {
        ///////////////////////////////////////////////////Variables Section/////////////////////////////////////////////////////
        public static GamePlayerSpawnManager instance;
        [SerializeField] private string syncID;
        [Header("Player Prefabs")]
        [SerializeField] private GameObject nonLocalPlayerPrefab = null;
        [SerializeField] private GameObject localPlayerPrefab = null;
        
        private GameObject spawnedLocalPlayerPrefab = null;
        
        private IDictionary<int, Dictionary<int, int>> relocationMap = new Dictionary<int, Dictionary<int, int>>();
        private Dictionary<int, int> oneMap = new Dictionary<int, int>();
        private Dictionary<int, int> threeMap = new Dictionary<int, int>();
        private Dictionary<int, int> fourMap = new Dictionary<int, int>();
        private Dictionary<int, int> sixMap = new Dictionary<int, int>();
        private Dictionary<int, int> sevenMap = new Dictionary<int, int>();
        private Dictionary<int, int> nineMap = new Dictionary<int, int>();
        private Dictionary<int, int> tenMap = new Dictionary<int, int>();
        private Dictionary<int, int> twelveMap = new Dictionary<int, int>();
        ///////////////////////////////////////////////////Events Section/////////////////////////////////////////////////////
        private void Awake()
        {
            initializeRelocationMaps();
        }

        void Start()
        {
            instance = this;
        }
        ///////////////////////////////////////////////////GameScenePlayerUpdate Section/////////////////////////////////////////////////////
        
        public void UpdateLocalPlayer(List<Seat> seatList, string playerName, GameObject gameScene)
        {
            if (seatList.Count == 6)
            {
                foreach (var seat in seatList)
                {
                    if (string.Equals(seat.username,playerName))
                    {
                        gameScene.GetComponent<SixPlayerGameScene>().LocalPlayer.SetActive(true);
                        gameScene.GetComponent<SixPlayerGameScene>().LocalPlayer.GetComponent<LocalPlayer>().SetPlayerInfo(seat);
                    }
                }

            }

            if (seatList.Count == 8)
            {
                
            }
        }
        
        public void UpdateOtherPlayers(List<Seat> seatList, string localPlayerName, GameObject gameScene)
        {
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    if (seat.location != SeatLocations.BottomRight)
                    {
                        if (seatList.Count == 6)
                        {
                            gameScene.GetComponent<SixPlayerGameScene>().SitButtonMap[(int) seat.location].SetActive(false);
                            gameScene.GetComponent<SixPlayerGameScene>().NLPlayerMap[(int) seat.location].SetActive(true);
                            gameScene.GetComponent<SixPlayerGameScene>().NLPlayerMap[(int) seat.location].GetComponent<nonLocalPlayer>().SetPlayerInfo(seat);
                        }
                        if (seatList.Count == 8)
                        {
                            //find map
                            //Dictionary<int, int> dummyMap = relocationMap[localPlayerRealLocation];
                            //Relocation 
                            //int sitButtonLocation = dummyMap[(int) seat.location];
                        
                            //EightPlayerGameScene.instance.SitButtonMap[(int) seat.location].SetActive(false);
                            //EightPlayerGameScene.instance.NLPlayerMap[(int) seat.location].SetActive(true);
                            //EightPlayerGameScene.instance.NLPlayerMap[(int) seat.location].GetComponent<nonLocalPlayer>().PlayerNameText.text = seat.username;
                            //EightPlayerGameScene.instance.NLPlayerMap[(int) seat.location].GetComponent<nonLocalPlayer>().BalanceText.text = seat.balance.ToString();
                            //EightPlayerGameScene.instance.NLPlayerMap[(int) seat.location].GetComponent<nonLocalPlayer>().LastMoveText.text = seat.lastMove.ToString();
                        }
                    }
                    
                }
            }
        }
        
        public void UpdateNewPlayerForGameScene(string newPlayerName, List<Seat> seatList, GameObject gameScene)
        {
            
            if (seatList.Count == 6)
            {
                foreach (var seat in seatList)
                {
                    if (string.Equals(seat.username , newPlayerName))
                    {
                        gameScene.GetComponent<SixPlayerGameScene>().SitButtonMap[(int) seat.location].SetActive(false);
                        gameScene.GetComponent<SixPlayerGameScene>().NLPlayerMap[(int) seat.location].SetActive(true);
                        gameScene.GetComponent<SixPlayerGameScene>().NLPlayerMap[(int) seat.location].GetComponent<nonLocalPlayer>().SetPlayerInfo(seat);
                    }
                }
            }

            if (seatList.Count == 8)
            {
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        if (string.Equals(seat.username , newPlayerName))
                        {
                            //EightPlayerGameScene.instance.SitButtonMap[(int) seat.location].SetActive(false);
                            //EightPlayerGameScene.instance.NLPlayerMap[(int) seat.location].SetActive(true);
                            //EightPlayerGameScene.instance.NLPlayerMap[(int) seat.location].GetComponent<nonLocalPlayer>().PlayerNameText.text = seat.username;
                            //EightPlayerGameScene.instance.NLPlayerMap[(int) seat.location].GetComponent<nonLocalPlayer>().BalanceText.text = seat.balance.ToString();
                            //EightPlayerGameScene.instance.NLPlayerMap[(int) seat.location].GetComponent<nonLocalPlayer>().LastMoveText.text = seat.lastMove.ToString();
                        }
                    }
                }
            }
        
        }


        ///////////////////////////////////////////////////TableScenePlayerUpdate Section/////////////////////////////////////////////////////
        public void UpdateNonLocalPlayersOnTableScene(List<Seat> seatList, GameObject activeScene)
        {
            if (seatList.Count == 6)
            {
                SixPlayerTableScene sixPlayerTableScene = activeScene.GetComponent<SixPlayerTableScene>();
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        sixPlayerTableScene.SitButtonMap[(int) seat.location].SetActive(false);
                        sixPlayerTableScene.NLPlayerMap[(int) seat.location].SetActive(true);
                        sixPlayerTableScene.NLPlayerMap[(int) seat.location].GetComponent<nonLocalPlayer>().SetPlayerInfo(seat);
                        
                    }
                }
            }
            
        }
        
        ///////////////////////////////////////////////////TableScenePlayerSpawn Section/////////////////////////////////////////////////////
        public void SpawnNonLocalPlayersOnTableScene(List<Seat> seatList, GameObject activeScene)
        {
            if (seatList.Count == 6)
            {
                SixPlayerTableScene sixPlayerTableScene = activeScene.GetComponent<SixPlayerTableScene>();
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        sixPlayerTableScene.SitButtonMap[(int) seat.location].SetActive(false);
                        sixPlayerTableScene.NLPlayerMap[(int) seat.location].SetActive(true);
                        sixPlayerTableScene.NLPlayerMap[(int) seat.location].GetComponent<nonLocalPlayer>().SetPlayerInfo(seat);
                        
                    }
                }
            }
            
        }
        public void SpawnNewPlayerForTableScene(string newPlayerName, List<Seat> seatList, GameObject activeScene)
        {
            if (seatList.Count == 6)
            {
                foreach (var seat in seatList)
                {
                    if (string.Equals(seat.username, newPlayerName))
                    {
                        activeScene.GetComponent<SixPlayerTableScene>().SitButtonMap[(int) seat.location].SetActive(false);
                        activeScene.GetComponent<SixPlayerTableScene>().NLPlayerMap[(int) seat.location].SetActive(true);
                        activeScene.GetComponent<SixPlayerTableScene>().NLPlayerMap[(int) seat.location].GetComponent<nonLocalPlayer>().SetPlayerInfo(seat);
                    }
                }
                
            }

            if (seatList.Count == 8)
            {
                foreach (var seat in seatList)
                {
                    if (string.Equals(seat.username, newPlayerName))
                    {
                        //EightPlayerTableScene.instance.SitButtonMap[(int) seat.location].SetActive(false);
                        //EightPlayerTableScene.instance.NLPlayerMap[(int) seat.location].SetActive(true);
                        //EightPlayerTableScene.instance.NLPlayerMap[(int) seat.location].GetComponent<nonLocalPlayer>().PlayerNameText.text = seat.username;
                        //EightPlayerTableScene.instance.NLPlayerMap[(int) seat.location].GetComponent<nonLocalPlayer>().BalanceText.text = seat.balance.ToString();
                        //EightPlayerTableScene.instance.NLPlayerMap[(int) seat.location].GetComponent<nonLocalPlayer>().LastMoveText.text = seat.lastMove.ToString();
                    }
                }
            }
        }
        
        ///////////////////////////////////////////////////GameScenePlayerSpawn Section/////////////////////////////////////////////////////
        
        public void SpawnLocalPlayer(List<Seat> seatList, string newPlayerName, GameObject gameScene)
        {
            if (seatList.Count == 6)
            {
                foreach (var seat in seatList)
                {
                    if (string.Equals(seat.username,newPlayerName))
                    {
                        gameScene.GetComponent<SixPlayerGameScene>().LocalPlayer.SetActive(true);
                        gameScene.GetComponent<SixPlayerGameScene>().LocalPlayer.GetComponent<LocalPlayer>().SetPlayerInfo(seat);
                    }
                }

            }

            if (seatList.Count == 8)
            {
                
            }
        }
        public void SpawnOtherPlayers(List<Seat> seatList, string localPlayerName, GameObject gameScene)
        {
            
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    if (seat.location != SeatLocations.BottomRight)
                    {
                        if (seatList.Count == 6)
                        {
                            
                            gameScene.GetComponent<SixPlayerGameScene>().SitButtonMap[(int) seat.location].SetActive(false);
                            gameScene.GetComponent<SixPlayerGameScene>().NLPlayerMap[(int) seat.location].SetActive(true);
                            gameScene.GetComponent<SixPlayerGameScene>().NLPlayerMap[(int) seat.location].GetComponent<nonLocalPlayer>().SetPlayerInfo(seat);
                            
                        }
                        if (seatList.Count == 8)
                        {
                            //find map
                            //Dictionary<int, int> dummyMap = relocationMap[localPlayerRealLocation];
                            //Relocation 
                            //int sitButtonLocation = dummyMap[(int) seat.location];
                        
                            //EightPlayerGameScene.instance.SitButtonMap[(int) seat.location].SetActive(false);
                            //EightPlayerGameScene.instance.NLPlayerMap[(int) seat.location].SetActive(true);
                            //EightPlayerGameScene.instance.NLPlayerMap[(int) seat.location].GetComponent<nonLocalPlayer>().PlayerNameText.text = seat.username;
                            //EightPlayerGameScene.instance.NLPlayerMap[(int) seat.location].GetComponent<nonLocalPlayer>().BalanceText.text = seat.balance.ToString();
                            //EightPlayerGameScene.instance.NLPlayerMap[(int) seat.location].GetComponent<nonLocalPlayer>().LastMoveText.text = seat.lastMove.ToString();
                        }
                    }
                    
                }
            }
        }
        
        public void SpawnNewPlayerForGameScene(string newPlayerName, List<Seat> seatList, GameObject gameScene)
        {
            
            if (seatList.Count == 6)
            {
                foreach (var seat in seatList)
                {
                    if (string.Equals(seat.username , newPlayerName))
                    {
                        gameScene.GetComponent<SixPlayerGameScene>().SitButtonMap[(int) seat.location].SetActive(false);
                        gameScene.GetComponent<SixPlayerGameScene>().NLPlayerMap[(int) seat.location].SetActive(true);
                        gameScene.GetComponent<SixPlayerGameScene>().NLPlayerMap[(int) seat.location].GetComponent<nonLocalPlayer>().SetPlayerInfo(seat);
                    }
                }
            }

            if (seatList.Count == 8)
            {
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        if (string.Equals(seat.username , newPlayerName))
                        {
                            //EightPlayerGameScene.instance.SitButtonMap[(int) seat.location].SetActive(false);
                            //EightPlayerGameScene.instance.NLPlayerMap[(int) seat.location].SetActive(true);
                            //EightPlayerGameScene.instance.NLPlayerMap[(int) seat.location].GetComponent<nonLocalPlayer>().PlayerNameText.text = seat.username;
                            //EightPlayerGameScene.instance.NLPlayerMap[(int) seat.location].GetComponent<nonLocalPlayer>().BalanceText.text = seat.balance.ToString();
                            //EightPlayerGameScene.instance.NLPlayerMap[(int) seat.location].GetComponent<nonLocalPlayer>().LastMoveText.text = seat.lastMove.ToString();
                        }
                    }
                }
            }
        
        }
        
        //////////////////////////////////////////////////////RelocationMaps Section/////////////////////////////////////////////////////
        private void initializeRelocationMaps()
        {
            initializeRelocationMap();
            initializeOneMap();
            initializeThreeMap();
            initializeFourMap();
            initializeSixMap();
            initializeSevenMap();
            initializeNineMap();
            initializeTenMap();
            initializeTwelveMap();
        }
        private void initializeRelocationMap()
        {
            relocationMap.Add(1,oneMap);
            relocationMap.Add(3,threeMap);
            relocationMap.Add(4,fourMap);
            relocationMap.Add(6,sixMap);
            relocationMap.Add(7,sevenMap);
            relocationMap.Add(9,nineMap);
            relocationMap.Add(10,tenMap);
            relocationMap.Add(12,twelveMap);
        }

        private void initializeOneMap()
        {
            oneMap.Add(1,1);
            oneMap.Add(3,3);
            oneMap.Add(4,4);
            oneMap.Add(6,6);
            oneMap.Add(7,7);
            oneMap.Add(9,9);
            oneMap.Add(10,10);
            oneMap.Add(12,12);
        }

        private void initializeThreeMap()
        {
            threeMap.Add(1,12);
            threeMap.Add(3,1);
            threeMap.Add(4,3);
            threeMap.Add(6,4);
            threeMap.Add(7,6);
            threeMap.Add(9,7);
            threeMap.Add(10,9);
            threeMap.Add(12,10);
        }

        private void initializeFourMap()
        {
            fourMap.Add(1,10);
            fourMap.Add(3,12);
            fourMap.Add(4,1);
            fourMap.Add(6,3);
            fourMap.Add(7,4);
            fourMap.Add(9,6);
            fourMap.Add(10,7);
            fourMap.Add(12,9);
        }

        private void initializeSixMap()
        {
            sixMap.Add(1,9);
            sixMap.Add(3,10);
            sixMap.Add(4,12);
            sixMap.Add(6,1);
            sixMap.Add(7,3);
            sixMap.Add(9,4);
            sixMap.Add(10,6);
            sixMap.Add(12,7);
        }

        private void initializeSevenMap()
        {
            sevenMap.Add(1,7);
            sevenMap.Add(3,9);
            sevenMap.Add(4,10);
            sevenMap.Add(6,12);
            sevenMap.Add(7,1);
            sevenMap.Add(9,3);
            sevenMap.Add(10,4);
            sevenMap.Add(12,6);
        }

        private void initializeNineMap()
        {
            nineMap.Add(1,6);
            nineMap.Add(3,7);
            nineMap.Add(4,9);
            nineMap.Add(6,10);
            nineMap.Add(7,12);
            nineMap.Add(9,1);
            nineMap.Add(10,3);
            nineMap.Add(12,4);
        }

        private void initializeTenMap()
        {
            tenMap.Add(1,4);
            tenMap.Add(3,6);
            tenMap.Add(4,7);
            tenMap.Add(6,9);
            tenMap.Add(7,10);
            tenMap.Add(9,12);
            tenMap.Add(10,1);
            tenMap.Add(12,3);
        }

        private void initializeTwelveMap()
        {
            twelveMap.Add(1,3);
            twelveMap.Add(3,4);
            twelveMap.Add(4,6);
            twelveMap.Add(6,7);
            twelveMap.Add(7,9);
            twelveMap.Add(9,10);
            twelveMap.Add(10,12);
            twelveMap.Add(12,1);
        }
        //////////////////////////////////////////////////////Properties Section/////////////////////////////////////////////////////
        public IDictionary<int, Dictionary<int, int>> RelocationMap => relocationMap;

        public GameObject NonLocalPlayerPrefab => nonLocalPlayerPrefab;
        
        public GameObject LocalPlayerPrefab => localPlayerPrefab;
        public string SyncID
        {
            get => syncID;
            set => syncID = value;
        }


        public void RemoveLocalPlayerFromGameScene(string playerName, List<Seat> seatList, GameObject activeScene)
        {
            if (seatList.Count == 6)
            {
                foreach (var seat in seatList)
                {
                    if (string.Equals(seat.username , playerName))
                    {
                        activeScene.GetComponent<SixPlayerGameScene>().LocalPlayerHand.SetActive(false);
                        activeScene.GetComponent<SixPlayerGameScene>().BottomRightBet.SetActive(false);
                        activeScene.GetComponent<SixPlayerGameScene>().LocalPlayer.SetActive(false);
                    }
                }
            }

            if (seatList.Count == 8)
            {
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        if (string.Equals(seat.username , playerName))
                        {
                            //EightPlayerGameScene.instance.SitButtonMap[(int) seat.location].SetActive(true);
                            //EightPlayerGameScene.instance.NLPlayerMap[(int) seat.location].SetActive(false);
                        }
                    }
                }
            }
        }

        public void RemoveNonLocalPlayersFromGameScene(List<Seat> seatList, GameObject activeScene)
        {
            if (seatList.Count == 6)
            {
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        activeScene.GetComponent<SixPlayerGameScene>().NLPlayerMap[(int) seat.location].SetActive(false);
                        activeScene.GetComponent<SixPlayerGameScene>().BetMap[(int) seat.location].SetActive(false);
                    }
                }
            }

            
        }

        public void RemoveCardBacksFromGameScene(List<Seat> seatList, string playerName, GameObject activeScene)
        {
            if (seatList.Count == 6)
            {
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        activeScene.GetComponent<SixPlayerGameScene>().CardBackMap[(int) seat.location].SetActive(false);
                    }
                }
            }

            if (seatList.Count == 8)
            {
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        if (string.Equals(seat.username , playerName))
                        {
                            //EightPlayerGameScene.instance.CardBackMap[(int) seat.location].SetActive(false);
                        }
                    }
                }
            }
        }

        public void RemoveDealerTokenFromGameScene(List<Seat> seatList, GameObject activeScene)
        {
            if (seatList.Count == 6)
            {
                foreach (var seat in seatList)
                {
                    if (seat.isDealer)
                    {
                        activeScene.GetComponent<SixPlayerGameScene>().DealerTokenMap[(int) seat.location].SetActive(false);
                    }
                }
            }

            
        }

        public void RemovePlayerSeatFromScene(string leavedPlayerName, List<Seat> oldSeatList, bool inGameScene, GameObject activeScene)
        {
            if (inGameScene)
            {
                if (oldSeatList.Count == 6)
                {
                    foreach (var seat in oldSeatList)
                    {
                        if (string.Equals(seat.username , leavedPlayerName))
                        {
                            activeScene.GetComponent<SixPlayerGameScene>().NLPlayerMap[(int) seat.location].SetActive(false);
                            activeScene.GetComponent<SixPlayerGameScene>().BetMap[(int) seat.location].SetActive(false);
                            activeScene.GetComponent<SixPlayerGameScene>().CardBackMap[(int) seat.location].SetActive(false);
                            //activeScene.GetComponent<SixPlayerGameScene>().DealerTokenMap[(int) seat.location].SetActive(false);
                            activeScene.GetComponent<SixPlayerGameScene>().SitButtonMap[(int) seat.location].SetActive(true);
                        }
                    }
                }
            }
            else
            {
                if (oldSeatList.Count == 6)
                {
                    foreach (var seat in oldSeatList)
                    {
                        if (string.Equals(seat.username , leavedPlayerName))
                        {
                            activeScene.GetComponent<SixPlayerTableScene>().NLPlayerMap[(int) seat.location].SetActive(false);
                            activeScene.GetComponent<SixPlayerTableScene>().BetMap[(int) seat.location].SetActive(false);
                            activeScene.GetComponent<SixPlayerTableScene>().CardBackMap[(int) seat.location].SetActive(false);
                            //activeScene.GetComponent<SixPlayerTableScene>().DealerTokenMap[(int) seat.location].SetActive(false);
                            activeScene.GetComponent<SixPlayerTableScene>().SitButtonMap[(int) seat.location].SetActive(true);
                        }
                    }
                }
                
            }

        }
    }
}