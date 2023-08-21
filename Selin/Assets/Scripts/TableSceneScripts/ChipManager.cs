using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


/*
 * Bir oyun masasının chip işlemlerini kontrol edecek sınıftır.
 * ******************Tuttuğu Mapler******************
 * playerAndChipAmountMap -->> Oyun masasındaki oyuncuların chip miktarlarını tutar. Bu Map TurnPlayers'in yaratımında kullanılır.
 * seatLocationAndChipAmountMap -->> Oyun masasındaki koltukYerlerinin chip miktarlarını tutar. Bu Map TurnPlayers'in yaratımında kullanılır.
 * Alttaki 4 map birlikte GameMove yaratılırken kullanılır.
 * playerAndParentMap -->> Oyun masasındaki oyuncuların parent'larını tutar. Bu Map Yeni GameMove yaratılırken kullanılır.
 * playerAndParentRakePercentMap -->> Oyun masasındaki oyuncuların parent'larının rake oranlarını tutar. Bu Map Yeni GameMove yaratılırken kullanılır.
 * playerAndPlayerRakePercentMap -->> Oyun masasındaki oyuncuların rake oranlarını tutar. Bu Map Yeni GameMove yaratılırken kullanılır.
 * parentAndRakeAmountMap -->> Oyun masasındaki parent'ların rake miktarlarını tutar. Bu Map Yeni GameMove yaratılırken kullanılır.
 * **************************************************
 * totalPot
 */
namespace Assets.Scripts
{
    public class ChipManager : MonoBehaviour
    {
        ///////////////////////////////////////////////////Variables Section/////////////////////////////////////////////////////
        public static ChipManager instance;
        [SerializeField] private string syncID;
        [SerializeField] private float totalPot;
        [SerializeField] private float subMaxBet = 0;
        [Header("MainBet ")]
        [SerializeField] private float mainBet = 0;
        [SerializeField] private List<string> mainBetPossibleOwners = new List<string>();
        [Header("sideBet ")]
        [SerializeField] private float sideBet1 = 0;
        [SerializeField] private int sideBetCount = 0;
        [SerializeField] private List<string> sideBet1PossibleOwners = new List<string>();
        [SerializeField] private float sideBet2 = 0;
        [SerializeField] private List<string> sideBet2PossibleOwners = new List<string>();
        [SerializeField] private float sideBet3 = 0;
        [SerializeField] private List<string> sideBet3PossibleOwners = new List<string>();
        [SerializeField] private float sideBet4 = 0;
        [SerializeField] private List<string> sideBet4PossibleOwners = new List<string>();
        [SerializeField] private float sideBet5 = 0;
        [SerializeField] private List<string> sideBet5PossibleOwners = new List<string>();
        [SerializeField] private float sideBet6 = 0;
        [SerializeField] private List<string> sideBet6PossibleOwners = new List<string>();
        
        
        private List<int> takenSideBets = new List<int>();
        public IDictionary<int,float> sideBetNoAndSideBetAmountMap = new Dictionary<int,float>();
        public IDictionary<int, List<string>> sideBetNoAndPlayerMap = new Dictionary<int, List<string>>();
        private Dictionary<string,float> playerAndChipAmountMap = new Dictionary<string, float>();
        private Dictionary<SeatLocations,float> seatLocationAndChipAmountMap = new Dictionary<SeatLocations, float>();
        private Dictionary<string,string> playerAndParentMap = new Dictionary<string, string>();
        private Dictionary<string,float> playerAndParentRakePercentMap = new Dictionary<string, float>();
        private Dictionary<string, float> playerAndPlayerRakePercentMap = new Dictionary<string, float>();
        private Dictionary<string,float> parentAndParentRakeAmountMap = new Dictionary<string, float>();
        private Dictionary<string,float> playerAndPlayerRakeAmountMap = new Dictionary<string, float>();
        ///////////////////////////////////////////////////Events Section/////////////////////////////////////////////////////
        private void Awake()
        {
            Debug.Log("ChipManager Awake");
            sideBetNoAndSideBetAmountMap.Add(0,mainBet);
            sideBetNoAndSideBetAmountMap.Add(1,sideBet1);
            sideBetNoAndSideBetAmountMap.Add(2,sideBet2);
            sideBetNoAndSideBetAmountMap.Add(3,sideBet3);
            sideBetNoAndSideBetAmountMap.Add(4,sideBet4);
            sideBetNoAndSideBetAmountMap.Add(5,sideBet5);
            sideBetNoAndSideBetAmountMap.Add(6,sideBet6);
            
            
            sideBetNoAndPlayerMap.Add(0,mainBetPossibleOwners);
            sideBetNoAndPlayerMap.Add(1,sideBet1PossibleOwners);
            sideBetNoAndPlayerMap.Add(2,sideBet2PossibleOwners);
            sideBetNoAndPlayerMap.Add(3,sideBet3PossibleOwners);
            sideBetNoAndPlayerMap.Add(4,sideBet4PossibleOwners);
            sideBetNoAndPlayerMap.Add(5,sideBet5PossibleOwners);
            sideBetNoAndPlayerMap.Add(6,sideBet6PossibleOwners);
        }
        void Start()
        {
            instance = this;
            
        }

        private void initiateSideBetMaps()
        {
            
            
        }

        //////////////////////////////////////////////////SideBet Section/////////////////////////////////////////////////////
        

        private void AddSideBetToSideBetMaps(int sideBetCount, List<string> sideBetPossibleOwners, float sideBetAmount,
            List<string> playerList)
        {
            
            switch (sideBetCount)
            {
                case 1:
                    sideBet1PossibleOwners = FindRemainingPlayers(sideBetPossibleOwners, playerList);
                    sideBetNoAndPlayerMap[1] = sideBet1PossibleOwners;
                    mainBet = sideBetAmount;
                    sideBetNoAndSideBetAmountMap[0] = mainBet;
                    mainBetPossibleOwners = sideBetPossibleOwners;
                    sideBetNoAndPlayerMap[0] = mainBetPossibleOwners;
                    break;
                case 2:
                    sideBet2PossibleOwners = FindRemainingPlayers(sideBetPossibleOwners, sideBet1PossibleOwners);
                    sideBetNoAndPlayerMap[2] = sideBet2PossibleOwners;
                    sideBet1 = sideBetAmount - mainBet;
                    sideBetNoAndSideBetAmountMap[1] = sideBet1;
                    sideBet1PossibleOwners = sideBetPossibleOwners;
                    sideBetNoAndPlayerMap[1] = sideBet1PossibleOwners;
                    break;
                case 3:
                    sideBet3PossibleOwners = FindRemainingPlayers(sideBetPossibleOwners, sideBet2PossibleOwners);
                    sideBetNoAndPlayerMap[3] = sideBet3PossibleOwners;
                    sideBet2 = sideBetAmount - sideBet1 - mainBet;
                    sideBetNoAndSideBetAmountMap[2] = sideBet2;
                    sideBet2PossibleOwners = sideBetPossibleOwners;
                    sideBetNoAndPlayerMap[2] = sideBet2PossibleOwners;
                    break;
                case 4:
                    sideBet4PossibleOwners = FindRemainingPlayers(sideBetPossibleOwners, sideBet3PossibleOwners);
                    sideBetNoAndPlayerMap[4] = sideBet4PossibleOwners;
                    sideBet3 = sideBetAmount - sideBet2 - sideBet1 - mainBet;
                    sideBetNoAndSideBetAmountMap[3] = sideBet3;
                    sideBet3PossibleOwners = sideBetPossibleOwners;
                    sideBetNoAndPlayerMap[3] = sideBet3PossibleOwners;
                    break;
                case 5:
                    sideBet5PossibleOwners = FindRemainingPlayers(sideBetPossibleOwners, sideBet4PossibleOwners);
                    sideBetNoAndPlayerMap[5] = sideBet5PossibleOwners;
                    sideBet4 = sideBetAmount - sideBet3 - sideBet2 - sideBet1 - mainBet;
                    sideBetNoAndSideBetAmountMap[4] = sideBet4;
                    sideBet4PossibleOwners = sideBetPossibleOwners;
                    sideBetNoAndPlayerMap[4] = sideBet4PossibleOwners;
                    break;
                case 6:
                    sideBet6PossibleOwners = FindRemainingPlayers(sideBetPossibleOwners, sideBet5PossibleOwners);
                    sideBetNoAndPlayerMap[6] = sideBet6PossibleOwners;
                    sideBet5 = sideBetAmount - sideBet4 - sideBet3 - sideBet2 - sideBet1 - mainBet;
                    sideBetNoAndSideBetAmountMap[5] = sideBet5;
                    sideBet5PossibleOwners = sideBetPossibleOwners;
                    sideBetNoAndPlayerMap[5] = sideBet5PossibleOwners;
                    break;
                default:
                    Debug.LogError("SideBetCount is not valid");
                    break;
            }
            
        }

        
        private List<string> FindRemainingPlayers(List<string> sideBetPossibleOwners, List<string> gamePlayerList)
        {
            List<string> remainingPlayers = new List<string>();
            foreach (var player in gamePlayerList)
            {
                if (!sideBetPossibleOwners.Contains(player))
                {
                    remainingPlayers.Add(player);
                }
            }
            return remainingPlayers;
        }

        public void SetSideBetInformations(List<Tuple<List<string>, float>> sideBetOwnersAndAmountTupleList,
            List<string> gamePlayerList)
        {
            
            foreach (var tuple in sideBetOwnersAndAmountTupleList)
            {
                //5e kadar cıktıgını test et mutlaka
                sideBetCount++;
                AddSideBetToSideBetMaps(sideBetCount,tuple.Item1,tuple.Item2,gamePlayerList);
                
            }
        }

        

        ///////////////////////////////////////////////////NewTurnInitialSettings Section/////////////////////////////////////////////////////
        public void InitializePlayerAndChipMap(List<Seat> seatList)
        {
            SetPlayerAndChipMap(seatList);
        }
        
        public void InitializeSeatLocationAndChipMap(List<Seat> seatList)
        {
            
            SetSeatLocationAndChipMap(seatList);
        }

        ///////////////////////////////////////////////////PotCalculation Section/////////////////////////////////////////////////////
        public void SetBetAmountForMoveCall(List<Seat> seatList, string lastMovePlayerName)
        {
            subMaxBet = CalculateSubMaxBetForMoveCall(seatList, lastMovePlayerName);
            totalPot = CalculateTotalBetAmountForMoveCall(seatList,lastMovePlayerName);
        }

        private float CalculateTotalBetAmountForMoveCall(List<Seat> seatList, string lastMovePlayerName)
        {
            float totalBetAmount = totalPot;
            foreach (Seat seat in seatList)
            {
                if (seat.username == lastMovePlayerName)
                {
                    totalBetAmount += seat.lastMoveAmount;
                }
            }
            return totalBetAmount;
        }

        private float CalculateSubMaxBetForMoveCall(List<Seat> seatList, string lastMovePlayerName)
        {
            float maxBet = subMaxBet;
            foreach (Seat seat in seatList)
            {
                if (seat.isActive )
                {
                    if (seat.username == lastMovePlayerName)
                    {
                        if (seat.totalBetInSubTurn > maxBet)
                        {
                            maxBet = seat.totalBetInSubTurn;
                        }
                    }
                }
            }
            return maxBet;
        }

        public void SetBetAmountForSmallAndBigBlind(List<Seat> seatList)
        {
            subMaxBet = CalculateSubMaxBetForSmallAndBigBlind(seatList);
            totalPot = CalculateTotalBetAmountForSmallAndBigBlind(seatList);
        }

        private float CalculateTotalBetAmountForSmallAndBigBlind(List<Seat> seatList)
        {
            float totalBetAmount = 0;
            foreach (Seat seat in seatList)
            {
                totalBetAmount += seat.lastMoveAmount;
            }
            return totalBetAmount;
        }

        private float CalculateSubMaxBetForSmallAndBigBlind(List<Seat> seatList)
        {
            float maxBet = 0;
            foreach (Seat seat in seatList)
            {
                if (seat.isActive )
                {
                    if (seat.lastMoveAmount > maxBet)
                    {
                        maxBet = seat.lastMoveAmount;
                    }
                }
            }

            return maxBet;
        }
        

        ///////////////////////////////////////////////////calculateUserRakeBack Section/////////////////////////////////////////////////////
        
        public float CalculateUserRakeBack(string playerName, float totalRake)
        {
            float userRakePercent = GetFromPlayerAndPlayerRakePercentMap(playerName);
            return (totalRake * userRakePercent) / 100;
        }
        ///////////////////////////////////////////////////calculateProfit Section/////////////////////////////////////////////////////
        public float CalculateProfit(float totalRake, float userRakeBack,float parentRakeBack)
        {
            return totalRake - userRakeBack - parentRakeBack;
        }
        ///////////////////////////////////////////////////calculateParentGain Section/////////////////////////////////////////////////////
        public float CalculateParentRakeBack(string playerName, float childRakeBack)
        {
            float parentRakePercent = GetFromPlayerAndParentRakePercentMap(playerName);
            if (parentRakePercent != 0)
            {
                return parentRakePercent * childRakeBack / 100;
            }
            return 0;
        }
        
        ///////////////////////////////////////////////////calculateTotalRake Section/////////////////////////////////////////////////////
        public float CalculateTotalRake(float moveAmount, float tableRakePercent)
        {
            return (moveAmount * tableRakePercent) / 100;
        }
        ///////////////////////////////////////////////////PlayerAndRakeAmountMap/////////////////////////////////////////////////////
        public void AddPlayerToPlayerAndRakeAmountMap(string playerName, float rakeAmount)
        {
            if (playerAndPlayerRakeAmountMap.ContainsKey(playerName))
            {
                UpdatePlayerRakeAmount(playerName, rakeAmount);
            }
            else
            {
                playerAndPlayerRakeAmountMap.Add(playerName, rakeAmount);
            }
        }

        public void UpdatePlayerRakeAmount(string playerName, float rakeAmount)
        {
            float currentParentRakeAmount = GetFromPlayerAndPlayerRakeAmountMap(playerName);
            RemovePlayerFromPlayerAndPlayerRakeAmountMap(playerName);
            AddPlayerToPlayerAndRakeAmountMap(playerName, currentParentRakeAmount + rakeAmount);
        }

        public float GetFromPlayerAndPlayerRakeAmountMap(string playerName)
        {
            if (playerAndPlayerRakeAmountMap.ContainsKey(playerName))
            {
                return playerAndPlayerRakeAmountMap[playerName];
            }
            Debug.LogWarning("Get process Failed! Player does not exists in playerAndPlayerRakeAmountMap : " + playerName);
            return 0;
        }
        
        public void RemovePlayerFromPlayerAndPlayerRakeAmountMap(string playerName)
        {
            if (playerAndPlayerRakeAmountMap.ContainsKey(playerName))
            {
                playerAndPlayerRakeAmountMap.Remove(playerName);
            }
            else
            {
                Debug.LogWarning("Remove process Failed! Player does not exists in playerAndPlayerRakeAmountMap : " + playerName);
            }
        }
        ///////////////////////////////////////////////////ParentAndParentRakeAmountMap/////////////////////////////////////////////////////
        public void AddParentToParentAndRakeAmountMap(string parentName, float rakeAmount)
        {
            if (parentAndParentRakeAmountMap.ContainsKey(parentName))
            {
                UpdateParentRakeAmount(parentName,rakeAmount);
            }
            else
            {
                parentAndParentRakeAmountMap.Add(parentName, rakeAmount);
            }
        }

        public void UpdateParentRakeAmount(string parentName, float rakeAmount)
        {
            float currentParentRakeAmount = GetFromParentAndParentRakeAmountMap(parentName);
            RemoveParentFromParentAndRakeAmountMap(parentName);
            AddParentToParentAndRakeAmountMap(parentName, currentParentRakeAmount + rakeAmount);
        }

        public float GetFromParentAndParentRakeAmountMap(string parentName)
        {
            return parentAndParentRakeAmountMap[parentName];
        }

        public void RemoveParentFromParentAndRakeAmountMap(string parentName)
        {
            if (parentAndParentRakeAmountMap.ContainsKey(parentName))
            {
                parentAndParentRakeAmountMap.Remove(parentName);
            }
            Debug.LogWarning("Parent does not exists in parentAndParentRakeAmountMap : " + parentName);
        }
        
        public void RemoveParentFromParentAndParentRakeAmountMap(string playerName)
        {
            string parentName = GetPlayerParentName(playerName);
            if (!string.IsNullOrEmpty(parentName))
            {
                RemoveParentFromParentAndRakeAmountMap(parentName);
            }
        }


        ///////////////////////////////////////////////////PlayerAndRakePercentMap/////////////////////////////////////////////////////
        public void AddPlayerToPlayerAndPlayerRakePercentMap(string username, float rakePercent)
        {
            if (!playerAndPlayerRakePercentMap.ContainsKey(username))
            {
                playerAndPlayerRakePercentMap.Add(username, rakePercent);
            }
            else
            {
                Debug.LogWarning("Player already exists in playerAndPlayerRakePercentMap : "+username);
            }
        }
        public float GetFromPlayerAndPlayerRakePercentMap(string username)
        {
            if (playerAndPlayerRakePercentMap.ContainsKey(username))
            {
                return playerAndPlayerRakePercentMap[username];
            }
            else
            {
                Debug.LogWarning("Player does not exists in playerAndPlayerRakePercentMap : " + username);
                return 0;
            }
        }
        
        public void RemovePlayerFromPlayerAndPlayerRakePercentMap(string playerName)
        {
            if (playerAndPlayerRakePercentMap.ContainsKey(playerName))
            {
                playerAndPlayerRakePercentMap.Remove(playerName);
            }
            else
            {
                Debug.LogWarning("PlayerAndPlayerRakePercentMap does not contains this tier : " + playerName);
            }
        }
        ///////////////////////////////////////////////////playerAndParentRakePercentMap Section/////////////////////////////////////////////////////
        public void AddPlayerToPlayerAndParentRakePercentMap(string playerName,float parentRakePercent)
        {
            if (!playerAndParentRakePercentMap.ContainsKey(playerName))
            {
                playerAndParentRakePercentMap.Add(playerName,parentRakePercent);
            }
            else
            {
                Debug.LogWarning("Player already exist in playerAndParentRakePercentMap : " + playerName);
            }
        }
        public float GetFromPlayerAndParentRakePercentMap(string playerName)
        {
            if (playerAndParentRakePercentMap.ContainsKey(playerName))
            {
                return playerAndParentRakePercentMap[playerName];
            }
            else
            {
                return 0;
            }
        }
        
        public void RemovePlayerFromPlayerAndParentRakePercentMap(string playerName)
        {
            if (playerAndParentRakePercentMap.ContainsKey(playerName))
            {
                playerAndParentRakePercentMap.Remove(playerName);
            }
        }
        
        ///////////////////////////////////////////////////playerAndParentMap Section/////////////////////////////////////////////////////
        public void AddPlayerToPlayerAndParentMap(string playerName,string parentName)
        {
            if (!playerAndParentMap.ContainsKey(playerName))
            {
                playerAndParentMap.Add(playerName,parentName);
            }
            else
            {
                Debug.LogWarning("PlayerAndParentMap already contains this tier : " + playerName);
            }
        }
        
        public void RemovePlayerFromPlayerAndParentMap(string playerName)
        {
            if (playerAndParentMap.ContainsKey(playerName))
            {
                playerAndParentMap.Remove(playerName);
            }
        }
        public string GetFromPlayerAndParentMap(string playerName)
        {
            if (playerAndParentMap.ContainsKey(playerName))
            {
                return playerAndParentMap[playerName];
            }
            else
            {
                Debug.LogWarning("PlayerAndParentMap does not contains this tier : " + playerName);
                return null;
            }
        }

        public bool hasUserParent(string playerName)
        {
            return playerAndParentMap.ContainsKey(playerName);
        }

        ///////////////////////////////////////////////////PlayerAndChipMap Section/////////////////////////////////////////////////////
        public void SetPlayerAndChipMap(List<Seat> mySeatList)
        {
            playerAndChipAmountMap.Clear();
            foreach (var seat in mySeatList)
            {
                if (seat.isActive)
                {
                    if (seat.isPlayerInGame)
                    {
                        playerAndChipAmountMap.Add(seat.username, seat.balance);
                    }
                }
            }
        }
        
        ///////////////////////////////////////////////////SeatLocationAndChipMap Section/////////////////////////////////////////////////////
        public void SetSeatLocationAndChipMap(List<Seat> mySeatList)
        {
            seatLocationAndChipAmountMap.Clear();
            foreach (var seat in mySeatList)
            {
                if (seat.isActive)
                {
                    if (seat.isPlayerInGame)
                    {
                        seatLocationAndChipAmountMap.Add(seat.location, seat.balance);
                    }
                }
            }
        }

        ///////////////////////////////////////////////////Properties Section/////////////////////////////////////////////////////
        public Dictionary<SeatLocations, float> SeatLocationAndChipAmountMap => seatLocationAndChipAmountMap;

        public Dictionary<string, float> PlayerAndChipAmountMap => playerAndChipAmountMap;

        public string SyncID
        {
            get => syncID;
            set => syncID = value;
        }

        public void SetSubMaxBet(float betAmount)
        {
            if (betAmount > subMaxBet)
            {
                subMaxBet = betAmount;
            }
        }

        public float SubMaxBet
        {
            get => subMaxBet;
            set => subMaxBet = value;
        }

        public float TotalPot
        {
            get => totalPot;
            set => totalPot = value;
        }


        public void SetSideBetsToZero()
        {
            sideBet1 = 0;
            sideBet2 = 0;
            sideBet3 = 0;
            sideBet4 = 0;
            sideBet5 = 0;
            sideBet6 = 0;
            
        }


        public void SetBetAmountForMoveCheck(List<Seat> seatList, string lastMovePlayerName)
        {
            subMaxBet = CalculateSubMaxBetForMoveCheck(seatList, lastMovePlayerName);
            totalPot = CalculateTotalBetAmountForMoveCheck(seatList,lastMovePlayerName);
        }

        private float CalculateTotalBetAmountForMoveCheck(List<Seat> seatList, string lastMovePlayerName)
        {
            float totalBetAmount = totalPot;
            foreach (Seat seat in seatList)
            {
                if (seat.username == lastMovePlayerName)
                {
                    totalBetAmount += seat.lastMoveAmount;
                }
            }
            return totalBetAmount;
        }

        private float CalculateSubMaxBetForMoveCheck(List<Seat> seatList, string lastMovePlayerName)
        {
            float maxBet = subMaxBet;
            foreach (Seat seat in seatList)
            {
                if (seat.isActive )
                {
                    if (seat.username == lastMovePlayerName)
                    {
                        if (seat.totalBetInSubTurn > maxBet)
                        {
                            maxBet = seat.totalBetInSubTurn;
                        }
                    }
                }
            }
            return maxBet;
        }

        public void ClearSubMaxBet()
        {
            subMaxBet = 0;
        }

        public void SetBetAmountForMoveRaise(List<Seat> seatList, string lastMovePlayerName)
        {
            subMaxBet = CalculateSubMaxBetForMoveRaise(seatList, lastMovePlayerName);
            totalPot = CalculateTotalBetAmountForMoveRaise(seatList,lastMovePlayerName);
        }

        private float CalculateTotalBetAmountForMoveRaise(List<Seat> seatList, string lastMovePlayerName)
        {
            float totalBetAmount = totalPot;
            foreach (Seat seat in seatList)
            {
                if (seat.username == lastMovePlayerName)
                {
                    totalBetAmount += seat.lastMoveAmount;
                }
            }
            return totalBetAmount;
        }

        private float CalculateSubMaxBetForMoveRaise(List<Seat> seatList, string lastMovePlayerName)
        {
            float maxBet = subMaxBet;
            foreach (Seat seat in seatList)
            {
                if (seat.isActive )
                {
                    if (seat.username == lastMovePlayerName)
                    {
                        if (seat.totalBetInSubTurn > maxBet)
                        {
                            maxBet = seat.totalBetInSubTurn;
                        }
                    }
                }
            }
            return maxBet;
        }

        public void SetBetAmountForMoveAllin(List<Seat> seatList, string lastMovePlayerName)
        {
            subMaxBet = CalculateSubMaxBetForMoveAllin(seatList, lastMovePlayerName);
            totalPot = CalculateTotalBetAmountForMoveAllin(seatList,lastMovePlayerName);
        }

        private float CalculateTotalBetAmountForMoveAllin(List<Seat> seatList, string lastMovePlayerName)
        {
            float totalBetAmount = totalPot;
            foreach (Seat seat in seatList)
            {
                if (seat.username == lastMovePlayerName)
                {
                    totalBetAmount += seat.lastMoveAmount;
                }
            }
            return totalBetAmount;
        }

        private float CalculateSubMaxBetForMoveAllin(List<Seat> seatList, string lastMovePlayerName)
        {
            float maxBet = subMaxBet;
            foreach (Seat seat in seatList)
            {
                if (seat.isActive )
                {
                    if (seat.username == lastMovePlayerName)
                    {
                        if (seat.totalBetInSubTurn > maxBet)
                        {
                            maxBet = seat.totalBetInSubTurn;
                        }
                    }
                }
            }
            return maxBet;
        }

        public float GetPlayerRakeBackAmount(string userUsername)
        {
            if (playerAndPlayerRakeAmountMap.ContainsKey(userUsername))
            {
                return playerAndPlayerRakeAmountMap[userUsername];
            }
            else
            {
                Debug.LogWarning("PlayerAndRakeBackAmountMap does not contains this tier : " + userUsername);
                return 0;
            }
        }

        public string GetPlayerParentName(string playerName)
        {
            if (playerAndParentMap.ContainsKey(playerName))
            {
                return playerAndParentMap[playerName];
            }
            else
            {
                return string.Empty;
            }
        }

        public float GetParentRakeBackAmount(string playerName)
        {
            if (parentAndParentRakeAmountMap.ContainsKey(playerName))
            {
                return parentAndParentRakeAmountMap[playerName];
            }
            else
            {
                Debug.LogWarning("PlayerAndParentRakeAmountMap does not contains this tier : " + playerName);
                return 0;
            }
        }

        

        

        

        

        

        public void ClearChipManagerGOData()
        {
            mainBet = 0;
            totalPot = 0;
            subMaxBet = 0;
            sideBetCount = 0;
            sideBet1 = 0;
            sideBet2 = 0;
            sideBet3 = 0;
            sideBet4 = 0;
            sideBet5 = 0;
            sideBet6 = 0;
            mainBetPossibleOwners.Clear();
            sideBet1PossibleOwners.Clear();
            sideBet2PossibleOwners.Clear();
            sideBet3PossibleOwners.Clear();
            sideBet4PossibleOwners.Clear();
            sideBet5PossibleOwners.Clear();
            sideBet6PossibleOwners.Clear();
            
            playerAndChipAmountMap.Clear();
            seatLocationAndChipAmountMap.Clear();
        }

        public void MakeRakeBackCalculations(List<GameMove> gameMoveList)
        {
            AddPlayerRakeAmountsToPlayerAndPlayerRakeAmountMap(gameMoveList);
            AddParentRakeAmountsToParentAndParentRakeAmountMap(gameMoveList);
        }

        private void AddParentRakeAmountsToParentAndParentRakeAmountMap(List<GameMove> gameMoveList)
        {
            List<string> playerList = new List<string>(playerAndParentMap.Keys);
            foreach (var playerName in playerList)
            {
                float parentTotalRakeAmount = IGameMoveList.GetParentRakeBackAmountTotalByPlayerName(gameMoveList, playerName);
                if (parentTotalRakeAmount > 0)
                {
                    UpdateParentRakeAmount(playerAndParentMap[playerName], parentTotalRakeAmount);
                }
                
            }
        }

        private void AddPlayerRakeAmountsToPlayerAndPlayerRakeAmountMap(List<GameMove> gameMoveList)
        {
            List<string> playerList = new List<string>(playerAndPlayerRakeAmountMap.Keys);
            foreach (var playerName in playerList)
            {
                float playerTotalRakeAmount = IGameMoveList.GetUserRakeBackAmountTotalByPlayerName(gameMoveList, playerName);
                if (playerTotalRakeAmount > 0)
                {
                    UpdatePlayerRakeAmount(playerName, playerTotalRakeAmount);
                }
            }
        }
        
        public int GetSideBetCount()
        {
            return sideBetCount;
        }
        public int SideBetCount
        {
            get => sideBetCount;
            set => sideBetCount = value;
        }

        public float MainBet
        {
            get => mainBet;
            set => mainBet = value;
        }

        public void SetMainBet(float amount)
        {
            mainBet = amount;
        }

        public float SideBet1
        {
            get => sideBet1;
            set => sideBet1 = value;
        }

        public float SideBet2
        {
            get => sideBet2;
            set => sideBet2 = value;
        }

        public float SideBet3
        {
            get => sideBet3;
            set => sideBet3 = value;
        }

        public float SideBet4
        {
            get => sideBet4;
            set => sideBet4 = value;
        }

        public float SideBet5
        {
            get => sideBet5;
            set => sideBet5 = value;
        }

        public float SideBet6
        {
            get => sideBet6;
            set => sideBet6 = value;
        }

        public List<Tuple<string,int>> FindWinnersTier(List<Tuple<string, List<CardData>>> winnersAndCombinations)
        {
            List<Tuple<string, int>> winnersTierTupleList = new List<Tuple<string, int>>();
            int tier = 0;
            foreach (var winnerAndCombination in winnersAndCombinations)
            {
                tier = FindWinnerTier(winnerAndCombination.Item1);
                winnersTierTupleList.Add(new Tuple<string, int>(winnerAndCombination.Item1, tier));
            }
            return winnersTierTupleList;
        }

        private int FindWinnerTier(string playerName)
        {
            for (int i = sideBetCount-1; i >= 0; i--)
            {
                bool condition = isPlayerInThisTier(sideBetNoAndPlayerMap[i], playerName);
                if (condition)
                {
                    return i;
                }
            }
            return 0;
        }

        private bool isPlayerInThisTier(List<string> possibleOwnerList, string playerName)
        {
            foreach (var possibleOwner in possibleOwnerList)
            {
                if (possibleOwner == playerName)
                {
                    return true;
                }
            }
            return false;
        }

        public List<string> FindRemainingTierPlayers(int current, int max)
        {
            List<string> remainingTierPlayers = new List<string>();
            for (int i = max; i >= current; i--)
            {
                remainingTierPlayers.AddRange(sideBetNoAndPlayerMap[i]);
            }
            return remainingTierPlayers;
        }

        public TurnWinners GenerateTurnWinners(List<Tuple<string, List<CardData>>> winnersAndCombinations,
            int currentTurnID, float currentTotalPot, float tableRakePercent)
        {
            TurnWinners turnWinners;
            if (sideBetCount == 0)
            {
                if (winnersAndCombinations.Count == 1)
                {
                    
                    float winAmount = CalculateWinAmount(currentTotalPot, tableRakePercent, winnersAndCombinations.Count);
                    turnWinners = new TurnWinners(currentTurnID,winnersAndCombinations[0].Item1, winAmount);
                    return turnWinners;
                }
                else
                {
                    float winAmount = CalculateWinAmount(currentTotalPot, tableRakePercent, winnersAndCombinations.Count);
                    List<string> winnerList = GenerateWinnerListFromWinnersAndCombinationsTupleList(winnersAndCombinations);
                    turnWinners = new TurnWinners(currentTurnID,winnerList, winAmount);
                    return turnWinners;
                }
            }
            else
            {
                //sideBet olan durum 
                List<string> winnerList = GenerateWinnerListFromWinnersAndCombinationsTupleList(winnersAndCombinations);
                
                //get each winners winamounts
                List<Tuple<string, float>> winnersTierTupleList = new List<Tuple<string, float>>();

                foreach (var winner in winnerList)
                {
                    foreach (var sideBetWinners in sideBetNoAndPlayerMap.Values)
                    {
                        if (sideBetWinners.Contains(winner))
                        {
                            //get tier of sideBetWinners
                            int key = sideBetNoAndPlayerMap.FirstOrDefault(x => x.Value == sideBetWinners).Key;
                            int sideBetWinnerCount = CalculateSideBetPartnersCount(key, winnerList);
                            float sideBetAmount = GetSideBetAmountBySideBetNo(key);
                            float winAmount = CalculateWinAmount(sideBetAmount, tableRakePercent, sideBetWinnerCount);
                            winnersTierTupleList.Add(new Tuple<string, float>(winner, winAmount));
                        }
                        
                    }
                }
                
                turnWinners = new TurnWinners(currentTurnID,winnersTierTupleList);
                return turnWinners;
            }
            
        }

        private float GetSideBetAmountBySideBetNo(int tier)
        {
            float totalAmount = 0;

            for (int i = tier; i >= 0; i--)
            {
                if (sideBetNoAndSideBetAmountMap.ContainsKey(i))
                {
                    if (!takenSideBets.Contains(i))
                    {
                        totalAmount += sideBetNoAndSideBetAmountMap[i];
                        takenSideBets.Add(i);
                    }
                    
                }
                else
                {
                    Debug.LogError("SideBetAmount not found");
                    return 0;
                }
            }
            return totalAmount;
            
        }

        private void SetSideBetToZero(int tier)
        {
            switch (tier)
            {
                case 0:
                    mainBet = 0;
                    sideBetNoAndSideBetAmountMap[0] = mainBet;
                    break;
                case 1:
                    sideBet1 = 0;
                    sideBetNoAndSideBetAmountMap[1] = sideBet1;
                    break;
                case 2:
                    sideBet2 = 0;
                    sideBetNoAndSideBetAmountMap[2] = sideBet2;
                    break;
                case 3:
                    sideBet3 = 0;
                    sideBetNoAndSideBetAmountMap[3] = sideBet3;
                    break;
                case 4:
                    sideBet4 = 0;
                    sideBetNoAndSideBetAmountMap[4] = sideBet4;
                    break;
                case 5:
                    sideBet5 = 0;
                    sideBetNoAndSideBetAmountMap[5] = sideBet5;
                    break;
                case 6:
                    sideBet6 = 0;
                    sideBetNoAndSideBetAmountMap[6] = sideBet6;
                    break;
                default:  
                    Debug.LogError("SideBetAmount not found --->>>  SetSideBetToZero");
                    break;
            }
        }
        

        private float CalculateWinAmount(float amount, float tableRakePercent, int count)
        {
            float totalRakeBackAmount = (amount * tableRakePercent) / 100;
            float winAmount = (amount - totalRakeBackAmount) / count;
            return winAmount;
        }

        private int CalculateSideBetPartnersCount(int sideBetNo, List<string> winnerList)
        {
            //calculate how many players in winner list has this sidebet
            int count = 0;
            foreach (var winner in winnerList)
            {
                if (sideBetNoAndPlayerMap[sideBetNo].Contains(winner))
                {
                    count++;
                }
            }
            return count;
        }

        private List<string> GenerateWinnerListFromWinnersAndCombinationsTupleList(List<Tuple<string, List<CardData>>> winnersAndCombinations)
        {
            List<string> winnerList = new List<string>();
            foreach (var winnerAndCombination in winnersAndCombinations)
            {
                winnerList.Add(winnerAndCombination.Item1);
            }
            return winnerList;
        }

        public IDictionary<int, float> SideBetNoAndSideBetAmountMap
        {
            get => sideBetNoAndSideBetAmountMap;
            set => sideBetNoAndSideBetAmountMap = value;
        }

        public IDictionary<int, List<string>> SideBetNoAndPlayerMap
        {
            get => sideBetNoAndPlayerMap;
            set => sideBetNoAndPlayerMap = value;
        }

        public float GetSideBet(int index)
        {
            switch (index)
            {
                case 0:
                    return mainBet;
                case 1:
                    return sideBet1;
                case 2:
                    return sideBet2;
                case 3:
                    return sideBet3;
                case 4:
                    return sideBet4;
                case 5:
                    return sideBet5;
                case 6:
                    return sideBet6;
                default:
                    Debug.LogError("SideBetAmount not found");
                    return 0;
            }
        }
    }
}