using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    /*
     * Tüm kullanıcılar için ağ üzerinde yapılacak işlemleri gerçekleştiren classtır.
     * Server ile haberleşme sadece bu class üzerinden gerçekleştirilir.
     * Yapılan işlemler:
     * 1- Login
     * 2- Logout
     * 3- OpenTableCreationOptionUI -------- Admin only
     * 4- CreateNewTable -------- Admin only
     * 5- JoinTable
     * 6- LeaveTable
     * 7- CreateNewUser -------- Admin and Operator(daha acılmadı)
     */
    public class Player : NetworkBehaviour
    {
        ///////////////////////////////////////////////////Variable Section/////////////////////////////////////////////////////
        [Header("Player Informations")]
        [SerializeField] private string localPlayerName = null;
        [SerializeField] private string password = null;
        [SerializeField] private float balance = 0;
        [SerializeField] private float rakePercent = 0;
        [SerializeField] private string parent = null;
        [SerializeField] private float parentPercent = 0;
        [SerializeField] private string creator = null;
        [SerializeField] private string creationDate = null;
        [SerializeField] private string userType = null;
        [Header("SyncData")]
        [SerializeField] private string syncID;
        [SerializeField] private string networkMatchID;
        [Header("Tools")]
        // bunun burda olması yanlış
        [SerializeField] private DBFacade dbFacade = null; 
        

        public static Player localPlayer;
        private NetworkMatch networkMatch;
        
        //alttaki ikisi olmasada olur yeri yanlış burda olmamalı
        private string usernamePlain;
        private string passwordPlain;
        ///////////////////////////////////////////////////Event Section/////////////////////////////////////////////////////
        private void Awake()
        {
            Debug.Log("Player.cs -->>> awake");
            networkMatch = GetComponent<NetworkMatch>();
            syncID = "lobby";
            networkMatch.matchId = GuidGenerator.syncIDToGuid(syncID);
            networkMatchID = networkMatch.matchId.ToString();
        }

        private void Start()
        {
            Debug.Log("Player.cs -->>> Start -->>> isLocalPlayer : "+isLocalPlayer);
        }

        /*
         * -------------------ClientSide-------------------
         * clientStart olduğunda localplayer'ı set eder
         */
        public override void OnStartClient () 
        {
            if (isLocalPlayer)
            {
                localPlayer = this;
                Debug.Log("Player.cs -->>> onStartClient -->>> isLocalPlayer : "+isLocalPlayer);
            } else {
                Debug.Log ($"Player.cs -->>> onStartClient -->>> isLocalPlayer : "+isLocalPlayer);
                
            }
        }

        /*
         * -------------------ServerSide-------------------
         * Bir client bağlantı kurduğunda tetiklenir
         */
        public override void OnStartServer()
        {
            Debug.Log("Player.cs -->>> OnStartServer -->>> Bir player bağlandı");
            base.OnStartServer();
        }

        public override void OnStopClient () 
        {
            Debug.Log("Player.cs -->>> OnStopClient -->>> Bir player ayrıldı");
            
        }
        ///////////////////////////////////////////////////ToggleMove Section/////////////////////////////////////////////////////
        
        public void ToggleMoves(ToggleMove toggleMove,bool toggleValue, float moveAmount)
        {
            CmdToggleMoves(toggleMove,toggleValue,moveAmount,localPlayer.localPlayerName);
        }

        [Command]
        private void CmdToggleMoves(ToggleMove toggleMove, bool toggleValue, float moveAmount, string playerName)
        {
            string matchId = VisitorArea.instance.GetFromPlayerAndTableMap(playerName);
            if (string.IsNullOrEmpty(matchId))
            {
                //matchID
                Debug.LogWarning("Player.cs -->>> CmdCheckGameState -->>> matchID'ye ait match bulunamadı");
                return;
            }
            if (!SpawnManager.instance.CheckAllTableTools(matchId))
            {
                Debug.LogWarning("Player.cs -->>> CmdCheckGameState -->>> CheckAllTableTools false döndü");
                return;
            }
            
            TableGameManager myTableGameManagerScript = SpawnManager.instance.GetTableGameManager(matchId);
            bool isToggleMoveLegit = myTableGameManagerScript.isToggleMoveLegit(toggleMove,toggleValue,moveAmount,playerName);
            if (!isToggleMoveLegit)
            {
                Debug.LogWarning("Player.cs -->>> CmdToggleMove -->>> isToggleMoveLegit false döndü");
                return;
            }
            Debug.Log("Player : "+playerName+" -->>> ToggleMove : "+toggleMove+" -->>> ToggleValue : "+toggleValue+" -->>> Amount : "+moveAmount);

            if (toggleValue)
            {
                AddToggleMoveToTheList(myTableGameManagerScript,toggleMove,moveAmount,playerName);
                Debug.Log("Player.cs -->>> CmdToggleMove -->>> AddToggleMoveToTheList");
            }
            else
            {
                RemoveToggleMoveFromTheList(myTableGameManagerScript,playerName);
                Debug.Log("Player.cs -->>> CmdToggleMove -->>> RemoveToggleMoveFromTheList");
            }
            
        }

        private void RemoveToggleMoveFromTheList(TableGameManager tableGameManagerScript, string playerName)
        {
            tableGameManagerScript.RemoveToggleMoveFromTheList(playerName);
        }

        private void AddToggleMoveToTheList(TableGameManager tableGameManagerScript, ToggleMove toggleMove, float moveAmount, string playerName)
        {
            tableGameManagerScript.AddToggleMoveToTheList(toggleMove,moveAmount,playerName);
        }

        ///////////////////////////////////////////////////NextMove Section/////////////////////////////////////////////////////
        public void NextMove(Move nextMove, float moveAmount)
        {
            CmdNextMove(nextMove,moveAmount,localPlayer.localPlayerName);
        }

        [Command]
        private void CmdNextMove(Move myNextMove, float moveAmount, string playerName)
        {
            StartCoroutine(WaitForLewKeyNextMove(myNextMove,moveAmount,playerName));
        }

        private IEnumerator WaitForLewKeyNextMove(Move myNextMove, float moveAmount, string playerName)
        {
            yield return new WaitForSeconds(TimeManager.instance.NextMoveDelayTime);
            LowKeyNextMove(myNextMove,moveAmount,playerName);
            yield return null;
        }


        private void LowKeyNextMove(Move myNextMove, float moveAmount, string playerName)
        {
            
            Debug.LogError("CMD Next Move Giriş");
            string matchId = VisitorArea.instance.GetFromPlayerAndTableMap(playerName);
            if (string.IsNullOrEmpty(matchId))
            {
                //matchID
                Debug.LogWarning("Player.cs -->>> CmdCheckGameState -->>> matchID'ye ait match bulunamadı");
                return;
            }
            if (!SpawnManager.instance.CheckAllTableTools(matchId))
            {
                Debug.LogWarning("Player.cs -->>> CmdCheckGameState -->>> CheckAllTableTools false döndü");
                return;
            }
            TableGameManager myTableGameManagerScript = SpawnManager.instance.GetTableGameManager(matchId);
            
            bool isMoveLegit = myTableGameManagerScript.MoveIsLegit(myNextMove,moveAmount,playerName);
            if (!isMoveLegit)
            {
                Debug.LogWarning("Player.cs -->>> CmdCheckGameState -->>> MoveIsLegit false döndü");
                return;
            }
            
            Debug.LogError("Player : "+playerName+" -->>> Move : "+myNextMove+" -->>> Amount : "+moveAmount);
            List<CardData> middleCards = GameMoveDoNew(myNextMove,moveAmount,playerName,myTableGameManagerScript);
            
            SendPlayerMoveToAllPlayers(playerName,myNextMove,myTableGameManagerScript,middleCards);
            
            KickPlayerIfHeIsFoldedAndPlayerOnLeaverQueue(myTableGameManagerScript,playerName,myNextMove);
            
            if (HasPreMove(myTableGameManagerScript))
            {
                string preMovePlayerName = myTableGameManagerScript.GetCurrentHasTurnPlayerName();
                ToggleMove toggleMove = myTableGameManagerScript.GetToggleMoveOfPlayer(preMovePlayerName);
                if (toggleMove != ToggleMove.None)
                {
                    float toggleMoveAmount = myTableGameManagerScript.GetToggleMoveAmountOfPlayer(preMovePlayerName);
                    Move preMove = myTableGameManagerScript.GeneratePreMoveFromToggleMove(toggleMove,preMovePlayerName);
                    RemovePreMovePlayerName(myTableGameManagerScript);
                    isMoveLegit = myTableGameManagerScript.MoveIsLegit(preMove,toggleMoveAmount,preMovePlayerName);
                    if (isMoveLegit)
                    {
                        SendToggleMoveIsDone(myTableGameManagerScript,preMovePlayerName,toggleMove,toggleMoveAmount,preMove);
                    }
                }
            }

            if (!HasPreMove(myTableGameManagerScript))
            {
                
                if (myNextMove == Move.HideCards || myNextMove == Move.ShowCards)
                {
                    Debug.LogError("CMD Next Move Boş ÇIKIŞ" + myTableGameManagerScript.MyOmahaEngineScript.MyTurnManagerScript.IsPlayerTurnEnd);
                    
                }
                else
                {
                    Debug.LogError("CMD Next Move Çıkış" + myTableGameManagerScript.MyOmahaEngineScript.MyTurnManagerScript.IsPlayerTurnEnd);
                    WaitForPlayerMoveUntilTurnEndsAndDoAutoMoveCheckOrFold(myTableGameManagerScript);
                }
                
            }
            
        }
        
        
        
        private void KickPlayerIfHeIsFoldedAndPlayerOnLeaverQueue(TableGameManager myTableGameManagerScript,
            string playerName, Move myNextMove)
        {
            List<Seat> oldSeatList = myTableGameManagerScript.MySeatList;
            if (myNextMove == Move.Fold)
            {
                if (myTableGameManagerScript.IsPlayerOnLeaverQueue(playerName))
                {
                    Debug.LogError("Buradan geldi");
                    LowKeyLeaveGame(playerName,oldSeatList,myTableGameManagerScript.SyncID);
                    LowKeyLeaveTable(playerName);
                }
            }
        }

        private void RemovePreMovePlayerName(TableGameManager myTableGameManagerScript)
        {
            myTableGameManagerScript.RemovePreMovePlayerName();
        }

        private void SendToggleMoveIsDone(TableGameManager myTableGameManagerScript, string preMovePlayerName,
            ToggleMove toggleMove, float toggleMoveAmount, Move preMove)
        {
            NetworkConnection myConnID = VisitorArea.instance.GetConnFromPlayerHashMap(preMovePlayerName);
            Debug.LogError("SendToggleMoveIsDone Giriş myConn equals null : "+ myConnID.Equals(null));
            TargetToggleMoveIsDone(myConnID,preMovePlayerName,toggleMove,myTableGameManagerScript.MySeatList,preMove,toggleMoveAmount);
        }

        [TargetRpc]
        private void TargetToggleMoveIsDone(NetworkConnection myConnID, string myName, ToggleMove toggleMove,
            List<Seat> seatList, Move preMove, float toggleMoveAmount)
        {
            TableGameManager.instance.ToggleMoveIsDone(myName,toggleMove,seatList);
            localPlayer.NextPreMove(preMove,toggleMoveAmount,myName);
        }
        private void NextPreMove(Move preMove, float toggleMoveAmount, string preMovePlayerName)
        {
            CmdNextMove(preMove,toggleMoveAmount,preMovePlayerName);
        }

        private bool HasPreMove(TableGameManager tableGameManagerScript)
        {
            return tableGameManagerScript.HasPreMove();
        }

        

        private void SendAnimationMovesToAllPlayers(TableGameManager myTableGameManagerScript)
        {
            TurnStage animationStartStage = myTableGameManagerScript.GetAnimationStartStage();
            if (animationStartStage == TurnStage.PreTurn)
            {
                SendTurnAndRiverAndWinnerAndGoNewTurn(myTableGameManagerScript);
            }
            if (animationStartStage == TurnStage.PreRiver)
            {
                SendRiverAndWinnerAndGoNewTurn(myTableGameManagerScript);
            }
            if (animationStartStage == TurnStage.PreShowDown)
            {
                SendWinnerAndGotoNewTurn(myTableGameManagerScript);
            }

        }

        private void SendWinnerAndGotoNewTurn(TableGameManager tableGameManagerScript)
        {
            SendWinnerInfoToAllPlayersAngGotoNewTurn(tableGameManagerScript);
        }

        private void SendWinnerInfoToAllPlayersAngGotoNewTurn(TableGameManager tableGameManagerScript)
        {
            List<string> inGamePlayerList = GetInGamePlayerList(tableGameManagerScript);
            List<List<CardData>> inGamePlayersCards = GetInGamePlayersCards(tableGameManagerScript);
            List<Tuple<string, List<CardData>>> winnerAndCombinations = GetWinnerAndCombinations(tableGameManagerScript);
            List<string> winnerList = GetWinnerListOrderByInGamePlayerList(inGamePlayerList,winnerAndCombinations,tableGameManagerScript);
            List<List<CardData>> winnerCombinations = GetWinnerCombinationsOrderByInGamePlayerList(inGamePlayerList,winnerAndCombinations,tableGameManagerScript);
            List<string> observerList = tableGameManagerScript.GetObserverList();
            List<float> winnerPotList = GetWinnerPotListOrderByInGamePlayerList(winnerList,tableGameManagerScript);
            
            AddWinAmountsToPlayers(tableGameManagerScript,winnerList,winnerPotList);
            ResetLastMoves(tableGameManagerScript);
            List<Seat> seatList1 = tableGameManagerScript.MySeatList;
            SendAllPlayersClash(seatList1,observerList,inGamePlayerList,inGamePlayersCards,winnerList,winnerCombinations,winnerPotList,tableGameManagerScript.SyncID);
        }

        private void SendRiverAndWinnerAndGoNewTurn(TableGameManager tableGameManagerScript)
        {
            CardData riverCard = tableGameManagerScript.GetRiverCard();
            SendRiverCardToAllPlayers(riverCard,tableGameManagerScript);
            
            IEnumerator coroutine = WaitForSendRiverCard(tableGameManagerScript);
            StartCoroutine(coroutine);
        }

        private void SendTurnAndRiverAndWinnerAndGoNewTurn(TableGameManager myTableGameManagerScript)
        {
            CardData turnCard = myTableGameManagerScript.GetTurnCard();
            SendTurnCardToAllPlayers(turnCard,myTableGameManagerScript);
            
            
            IEnumerator coroutine = WaitForSendTurnCard(myTableGameManagerScript);
            StartCoroutine(coroutine);
            
        }
        private IEnumerator WaitForSendTurnCard(TableGameManager tableGameManagerScript)
        {
            yield return new WaitForSeconds(1.0f);
            SendRiverAndWinnerAndGoNewTurn(tableGameManagerScript);
            
            yield return null;
        }

        private IEnumerator WaitForSendRiverCard(TableGameManager tableGameManagerScript)
        {
            yield return new WaitForSeconds(1.0f);
            SendWinnerAndGotoNewTurn(tableGameManagerScript);
            yield return null;
        }

        
        

        private void SendNewTurnToAllPlayers(TableGameManager tableGameManagerScript)
        {
            throw new NotImplementedException();
        }

        private void SendRiverCardToAllPlayers(CardData riverCard, TableGameManager tableGameManagerScript)
        {
            List<Seat> seatList = tableGameManagerScript.MySeatList;
            List<string> observerList = tableGameManagerScript.GetObserverList();
            NetworkConnection myConnID;
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    myConnID = VisitorArea.instance.GetConnFromPlayerHashMap(seat.username);
                    TargetSendRiverCard(myConnID,riverCard,seatList.Count);
                }
                                                
            }
            foreach (var observer in observerList)
            {
                myConnID = VisitorArea.instance.GetConnFromPlayerHashMap(observer);
                TargetSendRiverCard(myConnID,riverCard,seatList.Count);
            }
        }

        [TargetRpc]
        private void TargetSendRiverCard(NetworkConnection myConnID, CardData riverCard, int seatCount)
        {
            TableGameManager.instance.SetRiverCard(riverCard,seatCount);
        }

        private void SendTurnCardToAllPlayers(CardData turnCard, TableGameManager myTableGameManagerScript)
        {
            List<Seat> seatList = myTableGameManagerScript.MySeatList;
            List<string> observerList = myTableGameManagerScript.GetObserverList();
            NetworkConnection myConnID;
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    myConnID = VisitorArea.instance.GetConnFromPlayerHashMap(seat.username);
                    TargetSendTurnCard(myConnID,turnCard,seatList.Count);
                }
                                                
            }
            foreach (var observer in observerList)
            {
                myConnID = VisitorArea.instance.GetConnFromPlayerHashMap(observer);
                TargetSendTurnCard(myConnID,turnCard,seatList.Count);
            }
        }

        [TargetRpc]
        private void TargetSendTurnCard(NetworkConnection myConnID, CardData turnCard, int seatCount)
        {
            TableGameManager.instance.SetTurnCard(turnCard,seatCount);
        }

        private bool AnimationTillEnd(TableGameManager myTableGameManagerScript)
        {
            return myTableGameManagerScript.AnimationTillEnd();
        }

        private void SendPlayerMoveToAllPlayers(string lastMovePlayerName, Move move, TableGameManager tableGameManagerScript,
            List<CardData> middleCards)
        {
            int sideBetCount = tableGameManagerScript.GetSideBetCount();
            float middlePot = tableGameManagerScript.GetMainBet();
            List<float> sideBetList = new List<float>();

            if (sideBetCount != 0)
            {
                //main bet'i sideBet saymadan hesap yapılmıştır
                sideBetList = GetSideBetList(sideBetCount,sideBetList,tableGameManagerScript);
                Debug.Log("Player.cs -->>> SendPlayerMoveToAllPlayers -->>> sideBetList.Count: " + sideBetList.Count);
            }
            
            
            List<string> observerList = tableGameManagerScript.GetObserverList();
            List<Seat> seatList = tableGameManagerScript.MySeatList;
            bool isTurnStageEnd = tableGameManagerScript.SubTurnStageIsEnded();
            if (!isTurnStageEnd)
            {
                //TurnStage devam ediyor.
                SendAllPlayersNextMoveNoStageChange(lastMovePlayerName, seatList, observerList, middlePot,sideBetCount,sideBetList);
            }
            else
            {
                //TurnStage bitti.
                if (isReadyForTurnEnd(tableGameManagerScript))
                {
                    //TurnStage bitti ve turn bitmek için hazır
                    if (tableGameManagerScript.TurnEndisClash())
                    {
                        //TurnStage bitti ve turn bitmek için hazır. Ve Turn Clash ile bitiyor
                        if (isNeedCorrection(tableGameManagerScript))
                        {
                            Debug.Log("Player.cs -->>> SendPlayerMoveToAllPlayers -->>> isNeedCorrection yollanıyor");
                            DoNextMoveAndDoCorrectionAndMakeClash(seatList,observerList,lastMovePlayerName,middlePot,sideBetCount,sideBetList,tableGameManagerScript);
                        }
                        else
                        {
                            List<string> inGamePlayerList = GetInGamePlayerList(tableGameManagerScript);
                            List<List<CardData>> inGamePlayersCards = GetInGamePlayersCards(tableGameManagerScript);
                            List<Tuple<string, List<CardData>>> winnerAndCombinations = GetWinnerAndCombinations(tableGameManagerScript);
                            List<string> winnerList = GetWinnerListOrderByInGamePlayerList(inGamePlayerList,winnerAndCombinations,tableGameManagerScript);
                            List<List<CardData>> winnerCombinations = GetWinnerCombinationsOrderByInGamePlayerList(inGamePlayerList,winnerAndCombinations,tableGameManagerScript);
                            List<float> winnerPotList = GetWinnerPotListOrderByInGamePlayerList(winnerList,tableGameManagerScript);
            
                            SendAllPlayersNextMoveNoStageChange(lastMovePlayerName,seatList,observerList,middlePot,sideBetCount,sideBetList);
                            IEnumerator coroutine = WaitForLastMoveAndMoveChipsAndMakeClash(seatList, observerList,middlePot,sideBetCount,sideBetList,tableGameManagerScript,inGamePlayersCards,winnerCombinations,winnerPotList,winnerList,inGamePlayerList);
                            StartCoroutine(coroutine);
                            
                        }
                        
                    }
                    else 
                    {
                        //TurnStage bitti ve turn bitmek için hazır. Turn'u kazanan oyuncu sona kalan kişi 
                        // onun son kararının yollandığı yer
                        if (move == Move.HideCards)
                        {
                            List<string> inGamePlayerList = GetInGamePlayerList(tableGameManagerScript);
                            List<List<CardData>> inGamePlayersCards = GetInGamePlayersCards(tableGameManagerScript);
                            List<Tuple<string, List<CardData>>> winnerAndCombinations = GetWinnerAndCombinations(tableGameManagerScript);
                            List<string> winnerList = GetWinnerListOrderByInGamePlayerList(inGamePlayerList,winnerAndCombinations,tableGameManagerScript);
                            List<List<CardData>> winnerCombinations = GetWinnerCombinationsOrderByInGamePlayerList(inGamePlayerList,winnerAndCombinations,tableGameManagerScript);
                            List<float> winnerPotList = GetWinnerPotListOrderByInGamePlayerList(winnerList,tableGameManagerScript);
                            AddWinAmountsToPlayers(tableGameManagerScript,winnerList,winnerPotList);
                            ResetLastMoves(tableGameManagerScript);
                            //make rakeback arrangements
                            SendAllPlayerClearForNewTurn(seatList,observerList);
                            BeginNewTurn(syncID);
                        }
                        else if (move == Move.ShowCards)
                        {
                            List<string> inGamePlayerList = GetInGamePlayerList(tableGameManagerScript);
                            List<List<CardData>> inGamePlayersCards = GetInGamePlayersCards(tableGameManagerScript);
                            List<Tuple<string, List<CardData>>> winnerAndCombinations = GetWinnerAndCombinations(tableGameManagerScript);
                            List<string> winnerList = GetWinnerListOrderByInGamePlayerList(inGamePlayerList,winnerAndCombinations,tableGameManagerScript);
                            List<List<CardData>> winnerCombinations = GetWinnerCombinationsOrderByInGamePlayerList(inGamePlayerList,winnerAndCombinations,tableGameManagerScript);
                            List<float> winnerPotList = GetWinnerPotListOrderByInGamePlayerList(winnerList,tableGameManagerScript);
                            AddWinAmountsToPlayers(tableGameManagerScript,winnerList,winnerPotList);
                            ResetLastMoves(tableGameManagerScript);
                            string winnerName = winnerList[0];
                            List<CardData> winnerCards = winnerCombinations[0];
                            float winnerPot = winnerPotList[0];
                            SendAllPlayersShowCards(seatList,observerList,winnerName,winnerPot,winnerCards,tableGameManagerScript.SyncID);
                            
                        }
                        else
                        {
                            bool isEndGame = true;
                            tableGameManagerScript.SetEndGame(true);
                            if (isNeedCorrection(tableGameManagerScript))
                            {
                                SendNextMoveStageChangeAndDoCorrection(seatList,observerList,lastMovePlayerName,middleCards,middlePot,sideBetCount,sideBetList,tableGameManagerScript,isEndGame);
                                Debug.Log("Player.cs -->>> SendPlayerMoveToAllPlayers -->>> isNeedCorrection yollanıyor");
                            }
                            else
                            {
                                Debug.Log("Player.cs -->>> SendPlayerMoveToAllPlayers -->>> TurnEndisClash false döndü");
                                SendAllPlayersNextMoveStageChange(lastMovePlayerName,seatList,observerList,middleCards,middlePot,sideBetCount,sideBetList,isEndGame);
                            }
                            
                        }
                    }
                    
                }
                else
                {
                    //TurnStage bitti fakat turn bitmek için hazır değil
                    bool isEndGame = false;
                    if (isNeedCorrection(tableGameManagerScript))
                    {
                        if (AnimationTillEnd(tableGameManagerScript))
                        {
                            SendNextMoveStageChangeAndDoCorrectionAndStartAnimation(seatList,observerList,lastMovePlayerName,middleCards,middlePot,sideBetCount,sideBetList,tableGameManagerScript,isEndGame);
                        }else
                        {
                            SendNextMoveStageChangeAndDoCorrection(seatList,observerList,lastMovePlayerName,middleCards,middlePot,sideBetCount,sideBetList,tableGameManagerScript,isEndGame);
                            Debug.Log("Player.cs -->>> SendPlayerMoveToAllPlayers -->>> isNeedCorrection yollanıyor");
                        }
                    }
                    else
                    {
                        if (AnimationTillEnd(tableGameManagerScript))
                        {
                            SendAllPlayersNextMoveStageChangeAndStartAnimation(lastMovePlayerName,seatList,observerList,middleCards,middlePot,sideBetCount,sideBetList,isEndGame,tableGameManagerScript);
                        }
                        else
                        {
                            SendAllPlayersNextMoveStageChange(lastMovePlayerName,seatList,observerList,middleCards,middlePot,sideBetCount,sideBetList,isEndGame);
                        }
                        
                    }
                }
                
                
            }
            
        }

        private IEnumerator WaitForLastMoveAndMoveChipsAndMakeClash(List<Seat> seatList, List<string> observerList, float middlePot, int sideBetCount, List<float> sideBetList, TableGameManager tableGameManagerScript, List<List<CardData>> inGamePlayersCards, List<List<CardData>> winnerCombinations, List<float> winnerPotList, List<string> winnerList, List<string> inGamePlayerList)
        {
            yield return new WaitForSeconds(TimeManager.instance.LastMoveDelayTime);
            SendAllPlayersMoveChipsToPot(seatList,observerList,middlePot,sideBetCount,sideBetList);
            IEnumerator coroutine = WaitForMoveChipsToPotAndMakeClash(seatList, observerList, tableGameManagerScript,winnerList, winnerPotList, inGamePlayerList, inGamePlayersCards, winnerCombinations);
            StartCoroutine(coroutine);
            yield return null;
        }

        private void SendAllPlayersMoveChipsToPot(List<Seat> seatList, List<string> observerList, float middlePot, int sideBetCount, List<float> sideBetList)
        {
            NetworkConnection myConnID;
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    myConnID = VisitorArea.instance.GetConnFromPlayerHashMap(seat.username);
                    TargetMoveChipsToPot(myConnID,seatList,seat.username,middlePot,sideBetCount,sideBetList);
                }
                                                
            }
            foreach (var observer in observerList)
            {
                myConnID = VisitorArea.instance.GetConnFromPlayerHashMap(observer);
                TargetMoveChipsToPot(myConnID,seatList,observer,middlePot,sideBetCount,sideBetList);
            }
        }
        
        [TargetRpc]
        private void TargetMoveChipsToPot(NetworkConnection myConnID, List<Seat> seatList, string myName,
            float middlePot,
            int sideBetCount, List<float> sideBetList)
        {
            TableGameManager.instance.MoveChipsToPotSucces(seatList,myName,middlePot,sideBetCount,sideBetList);
        }

        private void DoNextMoveAndDoCorrectionAndMakeClash(List<Seat> seatList, List<string> observerList, string lastMovePlayerName, float middlePot, int sideBetCount, List<float> sideBetList, TableGameManager tableGameManagerScript)
        {
            List<string> inGamePlayerList = GetInGamePlayerList(tableGameManagerScript);
            List<List<CardData>> inGamePlayersCards = GetInGamePlayersCards(tableGameManagerScript);
            List<Tuple<string, List<CardData>>> winnerAndCombinations = GetWinnerAndCombinations(tableGameManagerScript);
            List<string> winnerList = GetWinnerListOrderByInGamePlayerList(inGamePlayerList,winnerAndCombinations,tableGameManagerScript);
            List<List<CardData>> winnerCombinations = GetWinnerCombinationsOrderByInGamePlayerList(inGamePlayerList,winnerAndCombinations,tableGameManagerScript);
            List<float> winnerPotList = GetWinnerPotListOrderByInGamePlayerList(winnerList,tableGameManagerScript);
            
            SendAllPlayersNextMoveNoStageChange(lastMovePlayerName,seatList,observerList,middlePot,sideBetCount,sideBetList);
            
            IEnumerator coroutine = WaitForLastMoveAndDoCorrectionAndMakeClash(seatList, observerList,middlePot,sideBetCount,sideBetList,tableGameManagerScript,inGamePlayersCards,winnerCombinations,winnerPotList,winnerList,inGamePlayerList);
            StartCoroutine(coroutine);
        }

        private IEnumerator WaitForLastMoveAndDoCorrectionAndMakeClash(List<Seat> seatList, List<string> observerList,
            float middlePot, int sideBetCount, List<float> sideBetList, TableGameManager tableGameManagerScript,
            List<List<CardData>> inGamePlayersCards, List<List<CardData>> winnerCombinations, List<float> winnerPotList,
            List<string> winnerList, List<string> inGamePlayerList)
        {
            yield return new WaitForSeconds(TimeManager.instance.LastMoveDelayTime);
            DoCorrection(seatList,observerList,tableGameManagerScript);
            IEnumerator coroutine = WaitForCorrectionAndMakeClash(seatList, observerList,middlePot,sideBetCount,sideBetList,tableGameManagerScript,inGamePlayersCards,winnerCombinations,winnerPotList,winnerList,inGamePlayerList);
            StartCoroutine(coroutine);
            yield return null;
            
            
        }

        private IEnumerator WaitForCorrectionAndMakeClash(List<Seat> seatList, List<string> observerList,
            float middlePot, int sideBetCount, List<float> sideBetList,
            TableGameManager tableGameManagerScript, List<List<CardData>> inGamePlayersCards,
            List<List<CardData>> winnerCombinations, List<float> winnerPotList, List<string> winnerList,
            List<string> inGamePlayerList)
        {
            yield return new WaitForSeconds(TimeManager.instance.CorrectionMoveDelayTime);
            SendAllPlayersMoveChipsToPot(seatList,observerList,middlePot,sideBetCount,sideBetList);
            IEnumerator coroutine = WaitForMoveChipsToPotAndMakeClash(seatList, observerList, tableGameManagerScript,winnerList, winnerPotList, inGamePlayerList, inGamePlayersCards, winnerCombinations);
            StartCoroutine(coroutine);
            yield return null;
        }

        private IEnumerator WaitForMoveChipsToPotAndMakeClash(List<Seat> seatList, List<string> observerList,
            TableGameManager tableGameManagerScript, List<string> winnerList, List<float> winnerPotList,
            List<string> inGamePlayerList, List<List<CardData>> inGamePlayersCards,
            List<List<CardData>> winnerCombinations)
        {
            yield return new WaitForSeconds(TimeManager.instance.MoveChipsToPotDelayTime);
            AddWinAmountsToPlayers(tableGameManagerScript,winnerList,winnerPotList);
            ResetLastMoves(tableGameManagerScript);
            //List<Seat> seatList1 = tableGameManagerScript.MySeatList;
            SendAllPlayersClash(seatList,observerList,inGamePlayerList,inGamePlayersCards,winnerList,winnerCombinations,winnerPotList,tableGameManagerScript.SyncID);
            yield return null;
        }

        private List<float> GetWinnerPotListOrderByInGamePlayerList(List<string> winnerList, TableGameManager tableGameManagerScript)
        {
            return tableGameManagerScript.GetWinnerPotListOrderByInGamePlayerList(winnerList);
        }


        private List<float> GetSideBetList(int sideBetCount, List<float> sideBetList, TableGameManager tableGameManagerScript)
        {
            float sideBet1 = 0;
            float sideBet2 = 0;
            float sideBet3 = 0;
            float sideBet4 = 0;
            float sideBet5 = 0;
            float sideBet6 = 0;
            
            switch (sideBetCount)
            {
                case 1:
                    sideBet1 = 0;
                    sideBet2 = 0;
                    sideBet3 = 0;
                    sideBet4 = 0;
                    sideBet5 = 0;
                    sideBet6 = 0;
                    break;
                case 2:
                    sideBet1 = tableGameManagerScript.GetSideBet(1);
                    sideBetList.Add(sideBet1);
                    break;
                case 3:
                    sideBet1 = tableGameManagerScript.GetSideBet(1);
                    sideBetList.Add(sideBet1);
                    sideBet2 = tableGameManagerScript.GetSideBet(2);
                    sideBetList.Add(sideBet2);
                    break;
                case 4:
                    sideBet1 = tableGameManagerScript.GetSideBet(1);
                    sideBetList.Add(sideBet1);
                    sideBet2 = tableGameManagerScript.GetSideBet(2);
                    sideBetList.Add(sideBet2);
                    sideBet3 = tableGameManagerScript.GetSideBet(3);
                    sideBetList.Add(sideBet3);
                    break;
                case 5:
                    sideBet1 = tableGameManagerScript.GetSideBet(1);
                    sideBetList.Add(sideBet1);
                    sideBet2 = tableGameManagerScript.GetSideBet(2);
                    sideBetList.Add(sideBet2);
                    sideBet3 = tableGameManagerScript.GetSideBet(3);
                    sideBetList.Add(sideBet3);
                    sideBet4 = tableGameManagerScript.GetSideBet(4);
                    sideBetList.Add(sideBet4);
                    break;
                case 6:
                    sideBet1 = tableGameManagerScript.GetSideBet(1);
                    sideBetList.Add(sideBet1);
                    sideBet2 = tableGameManagerScript.GetSideBet(2);
                    sideBetList.Add(sideBet2);
                    sideBet3 = tableGameManagerScript.GetSideBet(3);
                    sideBetList.Add(sideBet3);
                    sideBet4 = tableGameManagerScript.GetSideBet(4);
                    sideBetList.Add(sideBet4);
                    sideBet5 = tableGameManagerScript.GetSideBet(5);
                    sideBetList.Add(sideBet5);
                    break;
                case 7:
                    sideBet1 = tableGameManagerScript.GetSideBet(1);
                    sideBetList.Add(sideBet1);
                    sideBet2 = tableGameManagerScript.GetSideBet(2);
                    sideBetList.Add(sideBet2);
                    sideBet3 = tableGameManagerScript.GetSideBet(3);
                    sideBetList.Add(sideBet3);
                    sideBet4 = tableGameManagerScript.GetSideBet(4);
                    sideBetList.Add(sideBet4);
                    sideBet5 = tableGameManagerScript.GetSideBet(5);
                    sideBetList.Add(sideBet5);
                    sideBet6 = tableGameManagerScript.GetSideBet(6);
                    sideBetList.Add(sideBet6);
                    break;
                default:
                    Debug.LogError("Player.cs -->>> SendPlayerMoveToAllPlayers -->>> sideBetCount hatalı");
                    break;
            }

            return sideBetList;
        }

        private void SendNextMoveStageChangeAndDoCorrectionAndStartAnimation(List<Seat> seatList,
            List<string> observerList, string lastMovePlayerName, List<CardData> middleCards, float middlePot,
            int sideBetCount, List<float> sideBetList, TableGameManager tableGameManagerScript, bool isEndGame)
        {
            SendAllPlayersNextMoveStageChange(lastMovePlayerName,seatList,observerList,middleCards,middlePot,sideBetCount,sideBetList,isEndGame);
            
            IEnumerator coroutine = WaitForSendNextMoveAndAndDoCorrectionAndStartAnimation(seatList, observerList,tableGameManagerScript);
            StartCoroutine(coroutine);
            
        }

        private IEnumerator WaitForSendNextMoveAndAndDoCorrectionAndStartAnimation(List<Seat> seatList, List<string> observerList, TableGameManager tableGameManagerScript)
        {
            yield return new WaitForSeconds(TimeManager.instance.LastMoveDelayTime);
            DoCorrection(seatList,observerList,tableGameManagerScript);
            IEnumerator coroutine = WaitForCorrectionAndStartAnimation(tableGameManagerScript);
            StartCoroutine(coroutine);
            yield return null;
        }
        
        private IEnumerator WaitForCorrectionAndStartAnimation(TableGameManager tableGameManagerScript)
        {
            yield return new WaitForSeconds(TimeManager.instance.CorrectionMoveDelayTime);
            StartAnimationOnAllPlayers(tableGameManagerScript);
            yield return null;
        }

        

        private void SendAllPlayersNextMoveStageChangeAndStartAnimation(string lastMovePlayerName, List<Seat> seatList,
            List<string> observerList, List<CardData> middleCards, float middlePot, int sideBetCount,
            List<float> sideBetList, bool isEndGame,
            TableGameManager tableGameManagerScript)
        {
            SendAllPlayersNextMoveStageChange(lastMovePlayerName,seatList,observerList,middleCards,middlePot,sideBetCount,sideBetList,isEndGame);
            IEnumerator coroutine = WaitForEndSendAllPlayersNextMoveStageChangeAndStartAnimation(tableGameManagerScript);
            StartCoroutine(coroutine);
        }

        private IEnumerator WaitForEndSendAllPlayersNextMoveStageChangeAndStartAnimation(TableGameManager tableGameManagerScript)
        {
            yield return new WaitForSeconds(TimeManager.instance.AnimationStartDelayTime);
            StartAnimationOnAllPlayers(tableGameManagerScript);
            yield return null;
        }

        private void StartAnimationOnAllPlayers(TableGameManager tableGameManagerScript)
        {
            SendAnimationMovesToAllPlayers(tableGameManagerScript);
        }

        private void DoCorrection(List<Seat> seatList, List<string> observerList, TableGameManager tableGameManagerScript)
        {
            string correctionPlayerName = tableGameManagerScript.GetCorrectionMovePlayerName();
            float correctionBetAmountForClientSide = tableGameManagerScript.GetCorrectionBetAmountForClientSide();
            
            NetworkConnection myConnID;
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    myConnID = VisitorArea.instance.GetConnFromPlayerHashMap(seat.username);
                    TargetCorrectionMove(myConnID,seatList,correctionPlayerName,correctionBetAmountForClientSide,seat.username,true);
                }
                                                
            }
            foreach (var observer in observerList)
            {
                myConnID = VisitorArea.instance.GetConnFromPlayerHashMap(observer);
                TargetCorrectionMove(myConnID,seatList,correctionPlayerName,correctionBetAmountForClientSide,observer,false);
            }
        }

        private void SendNextMoveStageChangeAndDoCorrection(List<Seat> seatList, List<string> observerList,
            string lastMovePlayerName, List<CardData> middleCards, float middlePot,
            int sideBetCount,
            List<float> sideBetList,
            TableGameManager tableGameManagerScript, bool isEndGame)
        {
            SendAllPlayersNextMoveStageChange(lastMovePlayerName,seatList,observerList,middleCards,middlePot,sideBetCount,sideBetList,isEndGame);
            IEnumerator coroutine = WaitForSendNextMoveAndDoCorrection(seatList, observerList,tableGameManagerScript);
            StartCoroutine(coroutine);
        }

        

        private IEnumerator WaitForSendNextMoveAndDoCorrection(List<Seat> seatList, List<string> observerList,
            TableGameManager tableGameManagerScript)
        {
            yield return new WaitForSeconds(TimeManager.instance.LastMoveDelayTime);
            DoCorrection(seatList,observerList,tableGameManagerScript);
            yield return null;
        }

        [TargetRpc]
        private void TargetCorrectionMove(NetworkConnection myConnID, List<Seat> seatList, string correctionPlayerName,
            float correctionBetAmountForClientSide,
            string myName, bool isPlayerInGameScene)
        {
            TableGameManager.instance.CorrectionMove(seatList,correctionPlayerName,correctionBetAmountForClientSide,myName,isPlayerInGameScene);
        }

        private bool isNeedCorrection(TableGameManager tableGameManagerScript)
        {
            return tableGameManagerScript.isNeedCorrection();
        }

        

        private void SendAllPlayersShowCards(List<Seat> seatList, List<string> observerList, string winnerName, float winnerPot, List<CardData> winnerCards, string SyncID)
        {
            NetworkConnection myConnID;
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    myConnID = VisitorArea.instance.GetConnFromPlayerHashMap(seat.username);
                    TargetShowCards(myConnID,seatList,seat.username,true,winnerName,winnerPot,winnerCards,SyncID);
                }
                                                
            }
            foreach (var observer in observerList)
            {
                myConnID = VisitorArea.instance.GetConnFromPlayerHashMap(observer);
                TargetShowCards(myConnID,seatList,observer,false,winnerName,winnerPot,winnerCards,SyncID);
            }
            
            IEnumerator coroutine = WaitForShowCards(seatList, observerList, SyncID);
            StartCoroutine(coroutine);
        }

        private IEnumerator WaitForShowCards(List<Seat> seatList, List<string> observerList, string SyncID)
        {
            yield return new WaitForSeconds(TimeManager.instance.ShowCardsDelayTime);
            SendAllPlayerClearForNewTurn(seatList,observerList);
            BeginNewTurn(SyncID);
            yield return null;
            
        }
        [TargetRpc]
        private void TargetShowCards(NetworkConnection myConnID, List<Seat> seatList, string myName, bool isPlayerInGame, string winnerName, float winnerPot, List<CardData> winnerCards, string SyncID)
        {
            TableGameManager.instance.ShowCards(seatList,myName,isPlayerInGame,winnerName,winnerPot,winnerCards,SyncID);
        }
        


        private void ResetLastMoves(TableGameManager tableGameManagerScript)
        {
            tableGameManagerScript.ResetLastMoves();
        }

        private void SendAllPlayerClearForNewTurn(List<Seat> seatList, List<string> observerList)
        {
            NetworkConnection myConnID;
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    myConnID = VisitorArea.instance.GetConnFromPlayerHashMap(seat.username);
                    TargetClearForNewTurn(myConnID,seatList,seat.username,true);
                }
                                                
            }
            foreach (var observer in observerList)
            {
                myConnID = VisitorArea.instance.GetConnFromPlayerHashMap(observer);
                TargetClearForNewTurn(myConnID,seatList,observer,false);
            }
        }

        [TargetRpc]
        private void TargetClearForNewTurn(NetworkConnection myConnID, List<Seat> seatList, string myName,
            bool isPlayerInGame)
        {
            
            TableGameManager.instance.ClearForNewTurn(seatList,myName,isPlayerInGame);
        }

        private void BeginNewTurn(string matchId)
        {
            
            if (!SpawnManager.instance.CheckAllTableTools(matchId))
            {
                Debug.LogWarning("Player.cs -->>> BeginNewTurn£ -->>> CheckAllTableTools false döndü");
                return;
            }
            TableGameManager myTableGameManagerScript = SpawnManager.instance.GetTableGameManager(matchId);
            GamePlayerManager myGamePlayerManagerScript = myTableGameManagerScript.MyGamePlayerManagerScript;
            
            myTableGameManagerScript.MyGameState = GameState.Ready;
            List<string> zeroBalancePlayers = GetZeroBalancePlayersAndSetIsPlayerInGame(myTableGameManagerScript);
            
            if (zeroBalancePlayers.Count != 0)
            {
                KickZeroBalancePlayers(zeroBalancePlayers,myTableGameManagerScript);
            }else
            {
                StartCoroutine(WaitForBeginNewTurnControls(myTableGameManagerScript));
            }
            
        }

        private void KickZeroBalancePlayers(List<string> zeroBalancePlayers, TableGameManager myTableGameManagerScript)
        {
            List<Tuple<string,SeatLocations>> playerAndSeatLocationTupleList = new List<Tuple<string, SeatLocations>>();
            List<Seat> oldSeatList = myTableGameManagerScript.MySeatList;
            
            if (zeroBalancePlayers.Count != 0)
            {
                foreach (var player in zeroBalancePlayers)
                {
                    SeatLocations requestedSeat = (SeatLocations) ISeatList.GetPlayerLocationByName(myTableGameManagerScript.MySeatList, player);
                    playerAndSeatLocationTupleList.Add(new Tuple<string, SeatLocations>(player,requestedSeat));
                    LowKeyLeaveGame(player,oldSeatList,myTableGameManagerScript.SyncID);
                }
            }
            StartCoroutine(WaitForLeaveGameFunctions(myTableGameManagerScript,zeroBalancePlayers,playerAndSeatLocationTupleList));
        }

        IEnumerator WaitForLeaveGameFunctions(TableGameManager myTableGameManagerScript,
            List<string> zeroBalancePlayers, List<Tuple<string, SeatLocations>> playerAndSeatLocationTupleList)
        {
            yield return new WaitForSeconds(TimeManager.instance.LeaveGameDelayTime);
            if (zeroBalancePlayers.Count != 0)
            {
                foreach (var player in zeroBalancePlayers)
                {
                    float minDeposit = myTableGameManagerScript.MinDeposit;
                    SeatLocations requestedSeat = playerAndSeatLocationTupleList.Find(x => x.Item1 == player).Item2;
                    AskPlayersToBuyIn(player,minDeposit,requestedSeat);
                }
            }
            StartCoroutine(WaitForBeginNewTurnControls(myTableGameManagerScript));
        }

        IEnumerator WaitForBeginNewTurnControls(TableGameManager myTableGameManagerScript)
        {
            yield return new WaitForSeconds(TimeManager.instance.BeginNewTurnDelayTime);
            
            DoNextTurn(myTableGameManagerScript);
            yield return null;
        }

        private void AskPlayersToBuyIn(string playerName, float minDeposit, SeatLocations requestedSeat)
        {
            NetworkConnection myConnID = VisitorArea.instance.GetConnFromPlayerHashMap(playerName);
            TargetAskPlayerToBuyIn(myConnID,minDeposit,requestedSeat);
        }
        

        [TargetRpc]
        private void TargetAskPlayerToBuyIn(NetworkConnection myConnID, float minDeposit, SeatLocations requestedSeat)
        {
            TableGameManager.instance.DepositOptionsOpenSucces(minDeposit,requestedSeat);
        }

        private void DoNextTurn(TableGameManager myTableGameManagerScript)
        {
            Debug.LogError("Do Next Turn Giriş");
            KickPlayersOnLeaversQueue(myTableGameManagerScript);
            
            if (ISeatList.GetActivePlayerCount(myTableGameManagerScript.MySeatList) <= 1)
            {
                Debug.LogWarning("Player.cs -->>> DoNextTurn -->>> Yeterli player yok");
                myTableGameManagerScript.MyGameState = GameState.Pending;
                myTableGameManagerScript.MyOmahaEngineScript.ClearTableToolsGoData();
                SendAllPlayerClearForNewTurn(myTableGameManagerScript.MySeatList,myTableGameManagerScript.GetObserverList());
                return;
            }
            
            
            SetGameState(myTableGameManagerScript,GameState.Playing);
            InitializeNewTurn(myTableGameManagerScript);
            PrepareSmallAndBigBlindGODataByDealer(myTableGameManagerScript);
            WriteNewTurnToDatabase(myTableGameManagerScript,dbFacade);
            
            DealCards(myTableGameManagerScript);
            GiveTurnToPlayer(myTableGameManagerScript);
            StartTurn(myTableGameManagerScript);
            WriteNewTurnsTurnPlayersToDatabase(myTableGameManagerScript,dbFacade);
            WriteOmahaHandsToDatabase(myTableGameManagerScript,dbFacade);
            
            
            
            SendPlayerHandToSeatedPlayers(myTableGameManagerScript);
            SendTurnDetailsToObserverPlayers(myTableGameManagerScript);
            
            Debug.LogError("Do Next Turn Çıkış");
            WaitForPlayerMoveUntilTurnEndsAndDoAutoMoveCheckOrFold(myTableGameManagerScript);
        }

        private void PrepareSmallAndBigBlindGODataByDealer(TableGameManager myTableGameManagerScript)
        {
            myTableGameManagerScript.PrepareSmallAndBigBlindGODataByDealer();
        }


        private void KickPlayersOnLeaversQueue(TableGameManager myTableGameManagerScript)
        {
            List<Seat> oldSeatList = myTableGameManagerScript.MySeatList;
            int playerCount = myTableGameManagerScript.MyGamePlayerManagerScript.GetLeaverQueuePlayerList().Count;
            if (playerCount == 0)
            {
                return;
            }
            List<string> leaverQueuePlayerList = new List<string>(myTableGameManagerScript.MyGamePlayerManagerScript.GetLeaverQueuePlayerList());
            foreach (var playerName in leaverQueuePlayerList)
            {
                leaveGameFunctions(playerName,oldSeatList, myTableGameManagerScript);
                LowKeyLeaveTable(playerName);
            }
            
        }

        private List<string> GetZeroBalancePlayersAndSetIsPlayerInGame(TableGameManager tableGameManagerScript)
        {
            List<string> zeroBalancePlayerList = ISeatList.GetPlayerNameListWithZeroBalance(tableGameManagerScript.MySeatList);
            tableGameManagerScript.MySeatList = ISeatList.SetSeatVarIsPlayerInGameByBalance(tableGameManagerScript.MySeatList);
            return zeroBalancePlayerList;
        }

        private void AddWinAmountsToPlayers(TableGameManager tableGameManagerScript, List<string> winnerList,
            List<float> winnerPotList)
        {
            tableGameManagerScript.AddWinAmountsToPlayers(winnerList,winnerPotList);
        }


        private void SendAllPlayersClash(List<Seat> seatList, List<string> observerList, List<string> inGamePlayerList,
            List<List<CardData>> inGamePlayersCards, List<string> winnerList, List<List<CardData>> winnerCombinations,
            List<float> winAmountList, string SyncID)
        {
            NetworkConnection myConnID;
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    myConnID = VisitorArea.instance.GetConnFromPlayerHashMap(seat.username);
                    TargetClash(myConnID,seatList,seat.username,inGamePlayerList,inGamePlayersCards,winnerList,winnerCombinations,winAmountList);
                }
                                                
            }
            foreach (var observer in observerList)
            {
                myConnID = VisitorArea.instance.GetConnFromPlayerHashMap(observer);
                TargetClash(myConnID,seatList,observer,inGamePlayerList,inGamePlayersCards,winnerList,winnerCombinations,winAmountList);
            }
            IEnumerator coroutine = WaitForEndClashAnimation(seatList, observerList, SyncID);
            StartCoroutine(coroutine);
            
        }

        IEnumerator WaitForEndClashAnimation(List<Seat> seatList, List<string> observerList, string syncID)
        {
            yield return new WaitForSeconds(TimeManager.instance.ClashAnimationDelayTime);
            SendAllPlayerClearForNewTurn(seatList,observerList);
            BeginNewTurn(syncID);
            yield return null;
            
        }
        [TargetRpc]
        private void TargetClash(NetworkConnection myConnID, List<Seat> seatList, string myName,
            List<string> inGamePlayerList, List<List<CardData>> inGamePlayersCards, List<string> winnerList,
            List<List<CardData>> winnerCombinations, List<float> winAmountList)
        {
            TableGameManager.instance.AnimateClash(seatList,myName,inGamePlayerList,inGamePlayersCards,winnerList,winnerCombinations,winAmountList);
        }

        private List<List<CardData>> GetWinnerCombinationsOrderByInGamePlayerList(List<string> inGamePlayerList, List<Tuple<string, List<CardData>>> winnerAndCombinations, TableGameManager tableGameManagerScript)
        {
            return tableGameManagerScript.GetWinnerCombinationsOrderByInGamePlayerList(inGamePlayerList,winnerAndCombinations);
        }

        private List<string> GetWinnerListOrderByInGamePlayerList(List<string> inGamePlayerList,
            List<Tuple<string, List<CardData>>> winnerAndCombinations, TableGameManager tableGameManagerScript)
        {
            return tableGameManagerScript.GetWinnerListOrderByInGamePlayerList(inGamePlayerList,winnerAndCombinations);
        }

        
        
        

        private List<Tuple<string, List<CardData>>> GetWinnerAndCombinations(TableGameManager tableGameManagerScript)
        {
            return tableGameManagerScript.GetWinnerAndCombinations();
        }

        private List<List<CardData>> GetInGamePlayersCards(TableGameManager tableGameManagerScript)
        {
            List<List<CardData>> inGamePlayersCards = new List<List<CardData>>();
            List<string> inGamePlayerList = GetInGamePlayerList(tableGameManagerScript);
            foreach (string playerName in inGamePlayerList)
            {
                List<CardData> playerCards = tableGameManagerScript.GetPlayerCards(playerName);
                inGamePlayersCards.Add(playerCards);
            }

            return inGamePlayersCards;
        }

        private List<string> GetInGamePlayerList(TableGameManager tableGameManagerScript)
        {
            return ISeatList.GetInGamePlayerList(tableGameManagerScript.MySeatList);
        }


        private void SendAllPlayersNextMoveStageChange(string playerName, List<Seat> seatList,
            List<string> observerList, List<CardData> middleCards, float middlePot, int sideBetCount,
            List<float> sideBetList, bool isEndGame)
        {
            NetworkConnection myConnID;
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    myConnID = VisitorArea.instance.GetConnFromPlayerHashMap(seat.username);
                    TargetNextMoveStageChange(myConnID,seatList,playerName,seat.username,middleCards,middlePot,sideBetCount,sideBetList,isEndGame);
                }
                                                
            }
            foreach (var observer in observerList)
            {
                myConnID = VisitorArea.instance.GetConnFromPlayerHashMap(observer);
                TargetNextMoveStageChange(myConnID,seatList,playerName,observer,middleCards,middlePot,sideBetCount,sideBetList,isEndGame);
            }
        }

        private void SendAllPlayersNextMoveNoStageChange(string lastMovePlayerName, List<Seat> seatList,
            List<string> observerList, float middlePot, int sideBetCount, List<float> sideBetList)
        {
            NetworkConnection myConnID;
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    myConnID = VisitorArea.instance.GetConnFromPlayerHashMap(seat.username);
                    TargetNextMoveNoStageChange(myConnID,seatList,lastMovePlayerName,seat.username,middlePot,sideBetCount,sideBetList);
                }
                                                
            }
            foreach (var observer in observerList)
            {
                myConnID = VisitorArea.instance.GetConnFromPlayerHashMap(observer);
                TargetNextMoveNoStageChange(myConnID,seatList,lastMovePlayerName,observer,middlePot,sideBetCount,sideBetList);
            }
        }

        private bool isReadyForTurnEnd(TableGameManager tableGameManagerScript)
        {
            return tableGameManagerScript.isReadyForTurnEnd();
        }


        [TargetRpc]
        private void TargetNextMoveStageChange(NetworkConnection myConnID, List<Seat> seatList,
            string lastMovePlayerName, string myName, List<CardData> middleCards, float middlePot, int sideBetCount,
            List<float> sideBetList,
            bool isEndGame)
        {
            TableGameManager.instance.NextMoveStageChangeSucces(seatList,lastMovePlayerName,myName,middleCards,middlePot,sideBetCount,sideBetList,isEndGame);
        }

        [TargetRpc]
        private void TargetNextMoveNoStageChange(NetworkConnection myConnID, List<Seat> seatList,
            string lastMovePlayerName, string myName, float middlePot, int sideBetCount, List<float> sideBetList)
        {
            TableGameManager.instance.NextMoveNoStageChangeSucces(seatList,lastMovePlayerName,myName,middlePot,sideBetCount,sideBetList);
        }


        private List<CardData> GameMoveDoNew(Move myNextMove, float moveAmount, string playerName,
            TableGameManager myTableGameManagerScript)
        {
            return myTableGameManagerScript.GameMoveDoNew(myNextMove,moveAmount,playerName,dbFacade);
        }


        ///////////////////////////////////////////////////DoAutoMoveCheckOrFold Section/////////////////////////////////////////////////////
        
        ///////////////////////////////////////////////////CheckGameState Section/////////////////////////////////////////////////////
        private void CheckGameState()
        {
            CmdCheckGameState(LocalPlayerName);
        }
        
        [Command]
        private void CmdCheckGameState(string playerName)
        {
            Debug.LogError("Cmd Check GameState Giriş");
            string matchId = VisitorArea.instance.GetFromPlayerAndTableMap(playerName);
            if (string.IsNullOrEmpty(matchId))
            {
                //matchID
                Debug.LogWarning("Player.cs -->>> CmdCheckGameState -->>> matchID'ye ait match bulunamadı");
                return;
            }
            if (!SpawnManager.instance.CheckAllTableTools(matchId))
            {
                Debug.LogWarning("Player.cs -->>> CmdCheckGameState -->>> CheckAllTableTools false döndü");
                return;
            }
            TableGameManager myTableGameManagerScript = SpawnManager.instance.GetTableGameManager(matchId);
            GamePlayerManager myGamePlayerManagerScript = myTableGameManagerScript.MyGamePlayerManagerScript;
            
            if (myGamePlayerManagerScript.GamePlayerList.Count <= 1)
            {
                Debug.LogWarning("Player.cs -->>> CmdCheckGameState -->>> Yeterli player yok");
                myTableGameManagerScript.MyGameState = GameState.Pending;
                return;
            }
            if (ISeatList.GetActivePlayerCount(myTableGameManagerScript.MySeatList) <= 1)
            {
                Debug.LogWarning("Player.cs -->>> CmdCheckGameState -->>> Yeterli player yok");
                myTableGameManagerScript.MyGameState = GameState.Pending;
                return;
            }
            if (myTableGameManagerScript.MyGameState == GameState.Playing)
            {
                //wait for next turn
                Debug.LogWarning("Player.cs -->>> CmdCheckGameState -->>> Waiting for next turn");
                return;
            }

            SetGameState(myTableGameManagerScript,GameState.Playing);
            InitializeNewTurn(myTableGameManagerScript);
            PrepareSmallAndBigBlindGODataByDealer(myTableGameManagerScript);
            WriteNewTurnToDatabase(myTableGameManagerScript,dbFacade);

            DealCards(myTableGameManagerScript);
            GiveTurnToPlayer(myTableGameManagerScript);
            StartTurn(myTableGameManagerScript);
            
            WriteNewTurnsTurnPlayersToDatabase(myTableGameManagerScript,dbFacade);
            WriteOmahaHandsToDatabase(myTableGameManagerScript,dbFacade);
            
            SendPlayerHandToSeatedPlayers(myTableGameManagerScript);
            SendTurnDetailsToObserverPlayers(myTableGameManagerScript);

            Debug.LogError("Cmd Check GameState Çıkış");
            WaitForPlayerMoveUntilTurnEndsAndDoAutoMoveCheckOrFold(myTableGameManagerScript);
        }

        private void DealCards(TableGameManager tableGameManagerScript)
        {
            tableGameManagerScript.DealCards();
        }

        private void SendTurnDetailsToObserverPlayers(TableGameManager tableGameManagerScript)
        {
            NetworkConnection myConnID;
            int timeSpan = tableGameManagerScript.TimeSpanGet();
            TablePlayerManager myTablePlayerManagerScript = SpawnManager.instance.GetTablePlayerManager(tableGameManagerScript.SyncID);
            foreach (var observerName in myTablePlayerManagerScript.ObserverList)
            {
                myConnID = VisitorArea.instance.GetConnFromPlayerHashMap(observerName);
                TargetBuildTurnForObserver(myConnID,tableGameManagerScript.MySeatList,observerName,timeSpan);
            }
        }

        private void SendPlayerHandToSeatedPlayers(TableGameManager tableGameManagerScript)
        {
            
            Dealer myDealerScript = tableGameManagerScript.MyOmahaEngineScript.MyDealerScript;
            int timeSpan = tableGameManagerScript.TimeSpanGet();
            NetworkConnection myConnID;
            foreach (var seat in tableGameManagerScript.MySeatList)
            {
                if (seat.isActive)
                {
                    int[] mySuitArr = new int[4];
                    mySuitArr = myDealerScript.handToSuitArray(myDealerScript.GetHandFromPlayerHandMap(seat.username));
                    int[] myValueArr = new int[4];
                    myValueArr = myDealerScript.handToValueArray(myDealerScript.GetHandFromPlayerHandMap(seat.username));

                    myConnID = VisitorArea.instance.GetConnFromPlayerHashMap(seat.username);
                                        
                    TargetBuildTurnForSeatedPlayers(myConnID,mySuitArr,myValueArr,tableGameManagerScript.MySeatList,seat.username,timeSpan);
                }
                                                
            }
        }

        private void WriteNewMoveToDatabase(TableGameManager tableGameManagerScript, DBFacade dbFacade)
        {
            List<GameMove> moveList = tableGameManagerScript.MyOmahaEngineScript.MyTurnManagerScript.MoveList;
            foreach (GameMove move in moveList)
            {
                dbFacade.put(move);
            }
        }

        private void WriteOmahaHandsToDatabase(TableGameManager tableGameManagerScript, DBFacade dbFacade)
        {
            List<OmahaHand> omahaHandList = tableGameManagerScript.MyOmahaEngineScript.MyDealerScript.OmahaHandList;
            foreach (OmahaHand omahaHand in omahaHandList)
            {
                dbFacade.put(omahaHand);
            }
        }

        private void StartTurn(TableGameManager tableGameManagerScript)
        {
            tableGameManagerScript.StartTurn();
        }

        private void GiveTurnToPlayer(TableGameManager tableGameManagerScript)
        {
            tableGameManagerScript.GiveTurnToPlayer();
        }

        private void WriteNewTurnsTurnPlayersToDatabase(TableGameManager tableGameManagerScript, DBFacade dbFacade)
        {
            TurnPlayers newTurnPlayers = tableGameManagerScript.TurnPlayersCreateNew();
            dbFacade.put(newTurnPlayers);
        }

        private void WriteNewTurnToDatabase(TableGameManager tableGameManagerScript, DBFacade dbFacade)
        {
            Turn newTurn = tableGameManagerScript.TurnCreateNew();
            tableGameManagerScript.CurrentTurnID = dbFacade.putAndReturnID(newTurn);
        }

        private void InitializeNewTurn(TableGameManager tableGameManagerScript)
        {
            tableGameManagerScript.TurnInitializeNew();
        }

        private void SetGameState(TableGameManager tableGameManagerScript, GameState gameState)
        {
            tableGameManagerScript.GameStateSet(gameState);
        }

        [TargetRpc]
        private void TargetBuildTurnForObserver(NetworkConnection myConnID, List<Seat> seatList, string observerName,
            int timeSpan)
        {
            TableGameManager.instance.TargetBuildTurnForObserverSuccess(seatList,observerName,timeSpan);
            
        }

        [TargetRpc]
        //call dealer's buildMyHand method
        private void TargetBuildTurnForSeatedPlayers(NetworkConnection myConn, int[] mySuitArr, int[] myValueArr,
            List<Seat> seatList, string username, int timeSpan)
        {
            TableGameManager.instance.TargetBuildTurnForSeatedPlayersSucces(mySuitArr,myValueArr,seatList,username,timeSpan);
            
        }

        private void WaitForPlayerMoveUntilTurnEndsAndDoAutoMoveCheckOrFold(TableGameManager myTableGameManagerScript)
        {
            myTableGameManagerScript.StopCurrentCoroutine = false;
            myTableGameManagerScript.CoroutineMaster = gameObject.GetComponent<Player>().localPlayerName;
            Debug.LogError("Player.cs -->>> CoroutineMaster: " + myTableGameManagerScript.CoroutineMaster);
            
            if (myTableGameManagerScript.GetWaitForPlayerMoveCoroutine() != null) {
                myTableGameManagerScript.StopCurrentCoroutine = true;
                string currentHasTurnPlayer = myTableGameManagerScript.GetCurrentHasTurnPlayer();
                Debug.LogError("Player.cs -->>> WaitForPlayerMoveUntilTurnEndsAndDoAutoMoveCheckOrFold -->>> waitForPlayerMoveCoroutine sonlandırıldı, currentHasTurnPlayer: " + currentHasTurnPlayer );
                //StopCoroutine(myTableGameManagerScript.GetWaitForPlayerMoveCoroutine());
                StartCoroutine(waitForStopCoruutine(myTableGameManagerScript));
                
            }
            else
            {
                bool isEndGame = myTableGameManagerScript.GetReadyForTurnEnd();
            
                myTableGameManagerScript.SetWaitForPlayerMoveCoroutine(waitForPlayerMove(myTableGameManagerScript,isEndGame));
                StartCoroutine(myTableGameManagerScript.GetWaitForPlayerMoveCoroutine());
            }
        }
        
        IEnumerator waitForStopCoruutine(TableGameManager myTableGameManagerScript)
        {
            yield return new WaitWhile(myTableGameManagerScript.GetIsStopCoroutine);
            bool isEndGame = myTableGameManagerScript.GetReadyForTurnEnd();
            
            myTableGameManagerScript.SetWaitForPlayerMoveCoroutine(waitForPlayerMove(myTableGameManagerScript,isEndGame));
            StartCoroutine(myTableGameManagerScript.GetWaitForPlayerMoveCoroutine());
            
        }

        IEnumerator waitForPlayerMove(TableGameManager myTableGameManagerScript, bool isEndGame)
        {
            TurnEndType turnEndType = myTableGameManagerScript.GetTurnEndType();
            bool animationTillEndFlag = myTableGameManagerScript.GetAnimationTillEndFlag();
            if (turnEndType == TurnEndType.Clash || animationTillEndFlag)
            {
                string currentHasTurnPlayer2 = myTableGameManagerScript.GetCurrentHasTurnPlayer();
                Debug.LogError(
                    "Player.cs -->>> waitForPlayerMove -->>> waitForPlayerMoveCoroutine Null ATandı,currentHasTurnPlayer1 : " + currentHasTurnPlayer2);
                Debug.LogError(
                    "Player.cs -->>> waitForPlayerMove -->>> waitForPlayerMoveCoroutine bitti,currentHasTurnPlayer : " );
                myTableGameManagerScript.SetWaitForPlayerMoveCoroutine(null);
                yield return new WaitForSeconds(0.1f);
            }else
            {
                
                string currentHasTurnPlayer1 = myTableGameManagerScript.GetCurrentHasTurnPlayer();
                Debug.LogError(
                    "Player.cs -->>> waitForPlayerMove -->>> waitForPlayerMoveCoroutine Player'in Turn Bitişi Bekleniyor,currentHasTurnPlayer1 : " + currentHasTurnPlayer1);
                yield return new WaitUntil(() => myTableGameManagerScript.MyOmahaEngineScript.MyTurnManagerScript.GetIsPlayerTurnEnd() || myTableGameManagerScript.GetIsStopCoroutine());
                Debug.LogError(
                    "Player.cs -->>> waitForPlayerMove -->>> waitForPlayerMoveCoroutine baslatıldı,currentHasTurnPlayer : " );

                if (!myTableGameManagerScript.GetIsStopCoroutine())
                {
                    string currentHasTurnPlayer = myTableGameManagerScript.GetCurrentHasTurnPlayer();
                    ToggleMove toggleMove = ToggleMove.FoldCheck;
                    Move preMove = myTableGameManagerScript.GeneratePreMoveFromToggleMove(toggleMove,currentHasTurnPlayer);
                    Debug.LogError(
                        "Player.cs -->>> waitForPlayerMove -->>> waitForPlayerMoveCoroutine CoroutineSonlandırılmadı Devam ediliyor,currentHasTurnPlayer : " + currentHasTurnPlayer);
                    
                    if (isEndGame)
                    {
                        preMove = Move.HideCards;
                    }
                    int seatCount = myTableGameManagerScript.MySeatList.Count;
                    myTableGameManagerScript.SetMoveTimeCalculate(false);
                    myTableGameManagerScript.SetWaitForPlayerMoveCoroutine(null);
                    yield return new WaitUntil(myTableGameManagerScript.isWaitForPlayerCoroutineNULL);
                    Debug.LogError(
                        "Player.cs -->>> waitForPlayerMove -->>> waitForPlayerMoveCoroutine Null ATandı,currentHasTurnPlayer1 : " + currentHasTurnPlayer);
                    myTableGameManagerScript.SetPlayerTimeOut(false);
                    yield return new WaitWhile(myTableGameManagerScript.GetIsPlayerTimeOut);
                    bool moveNeedTransfer = false;
                    string transferPlayer = "";
                    NetworkConnection myConnID = null;
                    if (CheckPlayerIsOnLeaverQueue(currentHasTurnPlayer,myTableGameManagerScript))
                    {
                        transferPlayer = GetAnotherPlayer(currentHasTurnPlayer,myTableGameManagerScript);
                        moveNeedTransfer = true;
                    }
                    if (!moveNeedTransfer)
                    {
                        myConnID = VisitorArea.instance.GetConnFromPlayerHashMap(currentHasTurnPlayer);
                    }

                    if (moveNeedTransfer)
                    {
                        myConnID = VisitorArea.instance.GetConnFromPlayerHashMap(transferPlayer);
                    }
                    TargetTimeOutMoveSucces(myConnID,preMove,0,currentHasTurnPlayer,seatCount,moveNeedTransfer);
                    
                    //StartCoroutine(WaitForLewKeyNextMove(preMove,0,currentHasTurnPlayer,myTableGameManagerScript.SyncID));
                    Debug.LogError("TargetTimeOutMoveSucces WaitUntil isPlayerTurnEnd :" + " preMove : " + preMove+" isEndGame : " + isEndGame+" currentHasTurnPlayer : " + currentHasTurnPlayer);
                    Debug.LogError("Player.cs -->>> waitForPlayerMove -->>> waitForPlayerMoveCoroutine bitti,currentHasTurnPlayer : " + currentHasTurnPlayer);
                }
                else
                {
                    Debug.LogError("Coroutine Sonlandırıldı");
                }
            }
            myTableGameManagerScript.StopCurrentCoroutine = false;
            
        }

        private string GetAnotherPlayer(string player, TableGameManager myTableGameManagerScript)
        {
            return myTableGameManagerScript.GetAnotherPlayer(player);
        }

        private bool CheckPlayerIsOnLeaverQueue(string currentHasTurnPlayer, TableGameManager myTableGameManagerScript)
        {
            return myTableGameManagerScript.CheckPlayerIsOnLeaverQueue(currentHasTurnPlayer);
        }

        
        
        [TargetRpc]
        private void TargetTimeOutMoveSucces(NetworkConnection myConnID, Move preMove, int moveAmount,
            string currentHasTurnPlayer, int seatCount, bool isMoveTransfered)
        {
            if (!isMoveTransfered)
            {
                if (preMove ==Move.HideCards)
                {
                    TableGameManager.instance.HideEndGameButtons(seatCount);
                }
                localPlayer.NextMove(preMove, moveAmount);
            }
            else
            {
                localPlayer.NextMove(preMove, moveAmount,currentHasTurnPlayer);
            }
            
        }

        private void NextMove(Move nextMove, int moveAmount, string currentHasTurnPlayer)
        {
            CmdNextMove(nextMove,moveAmount,currentHasTurnPlayer);
        }


        ///////////////////////////////////////////////////JoinGame Section/////////////////////////////////////////////////////
        public void JoinGame(SeatLocations requestedSeat, int deposit)
        {
            float depositFloat = deposit;
            CmdJoinGame(localPlayerName, requestedSeat,depositFloat);
        }
        [Command]
        private void CmdJoinGame(string playerName, SeatLocations requestedSeat, float deposit)
        {
            //findPlayer
            string matchID = VisitorArea.instance.GetFromPlayerAndTableMap(playerName);
            if (string.IsNullOrEmpty(matchID) )
            {
                //player masadan çıkış yapmış
                Debug.LogWarning("player masadan çıkış yapmış");
                TargetJoinGameFail();
                return;
            }
            if (!SpawnManager.instance.CheckAllTableTools(matchID))
            {
                //tableToolsNotWorking
                Debug.LogWarning("tableToolsNotWorking");
                TargetJoinGameFail();
                return;
            }
            
            TableGameManager myTableGameManagerScript = SpawnManager.instance.GetTableGameManager(matchID);
            if (myTableGameManagerScript.ContainSeatByUsername(playerName))
            {
                Debug.LogWarning("Çapraz geçiş yapmak isteniyor");
                //capraz geçiş yapmak isteyen player
                //özel ilgi lazım
                return;
            }
            
            //check seat available
            if (myTableGameManagerScript.isSeatActiveByLocation(requestedSeat))
            { 
                // seat taken
                TargetJoinGameFailErrorSeatTaken();
                return;
            }
            
            //checkBalanceEnough
            User dummyUser = (User) dbFacade.get(usernamePlain, typeof(User)); 
            
            if (dummyUser.Balance < deposit) 
            {
                //yetersiz bakiye
                TargetJoinGameFailErrorBalanceNotEnough();
                return;
            }
            if (myTableGameManagerScript.MinDeposit > deposit)
            {
                //Minimum limitin altında yatırım yapılamaz
                TargetJoinGameFailErrorBelowMinimum(myTableGameManagerScript.MinDeposit);
                return;
            }
            if (!myTableGameManagerScript.PlayerListContain(playerName))
            { 
                Debug.LogWarning("Player.cs -->>> CmdJoinGame -->>> player tableGameMAnager PlayerList'inde bulunmuyor. Masa MatchID : " + matchID);
                //player tableGameMAnager PlayerList'inde bulunmuyor
                TargetJoinGameFailErrorPlayerNotFoundOnList(playerName);
                return;
            }

            TablePlayerManager myTablePlayerManagerScript = myTableGameManagerScript.MyTablePlayerManagerScript;             
            
            if (!myTablePlayerManagerScript.isPlayerOnObserverList(playerName)) 
            {
                Debug.LogWarning("Player.cs -->>> CmdJoinGame -->>> player tablePlayerManager observerListTe bulunmuyor. Masa MatchID : " + matchID);
                //player tablePlayerManager observerListTe bulunmuyor
                TargetJoinGameFailErrorPlayerNotFoundOnList(playerName);
                return;
                
            }
            if (!myTablePlayerManagerScript.isPlayerOnTablePlayerList(playerName))
            {
                Debug.LogWarning("Player.cs -->>> CmdJoinGame -->>> player tablePlayerManager tablePlayerListTe bulunmuyor. Masa MatchID : " + matchID);
                //player tablePlayerManager tablePlayerListTe bulunmuyor
                TargetJoinGameFailErrorPlayerNotFoundOnList(playerName);
                return;
            }
            GamePlayerManager myGamePlayerManagerScript = myTableGameManagerScript.MyGamePlayerManagerScript;
            
            RemovePlayerFromObserverList(myTableGameManagerScript,playerName);
            Seat newSeat = CreateNewSeat(myTableGameManagerScript,playerName,requestedSeat,deposit);
            
            
            if (dummyUser.hasParent())
            {
                myTableGameManagerScript.PlayerAndParentMapAdd(dummyUser.Username, dummyUser.Parent);
                myTableGameManagerScript.PlayerAndParentRakePercentMapAdd(dummyUser.Username, dummyUser.ParentPercent);
                myTableGameManagerScript.ParentAndParentRakeAmountMapAdd(dummyUser.Parent, 0);
            }
            myTableGameManagerScript.PlayerAndRakePercentMapAdd(dummyUser.Username, dummyUser.RakePercent);
            myTableGameManagerScript.PlayerAndRakeAmountMapAdd(dummyUser.Username, 0);
            myGamePlayerManagerScript.AddPlayerToGamePlayerList(playerName);
            myGamePlayerManagerScript.AddPlayerToSeatAndPlayerMap(newSeat.location, playerName);
            myTableGameManagerScript.UpdateSeatByLocation(newSeat);
                                            
            // kullanıcnın bakiyesi düşülecek
            dummyUser.Balance = dummyUser.Balance - deposit;
            //update database
            dbFacade.update(dummyUser);
            //update PlayerGameObject
            balance = dummyUser.Balance;
                                         
            //create new JoinGameInDataBase
            bool isOkey = CreateGameParticipation(myTableGameManagerScript, dummyUser.Username, deposit,dbFacade);
            if (!isOkey)
            {
                Debug.LogWarning("Player.cs -->>> CmdJoinGame -->>> CreateGameParticipation failed. Masa MatchID : " + matchID);
                return;
            }
            
            //playerPrefab LocalPlayer tarafında spawn olmalı ve diğer playerlarda NonLocalPlayer spawn olmalı
            // ve localplayer gameScene'e geçiş yapmalı
            int timeSpan = myTableGameManagerScript.TimeSpanGet();
            RpcJoinGame(dummyUser.Username,myTableGameManagerScript.MySeatList,PlayerGameState.InGame,timeSpan);
        }

        private bool CreateGameParticipation(TableGameManager tableGameManagerScript, string username, float deposit, DBFacade dbFacade)
        {
            GameParticipation gameParticipation = new GameParticipation(tableGameManagerScript.SyncID, username, deposit);
            int gameParticipationID = dbFacade.putAndReturnID(gameParticipation);
            return tableGameManagerScript.AddGameParticipationIdToMap(gameParticipationID, username);

        }

        private Seat CreateNewSeat(TableGameManager tableGameManagerScript, string playerName, SeatLocations requestedSeat, float deposit)
        {
            return tableGameManagerScript.SeatCreateNew(playerName, requestedSeat, deposit);
        }

        private void RemovePlayerFromObserverList(TableGameManager tableGameManagerScript, string playerName)
        {
            tableGameManagerScript.MyTablePlayerManagerScript.RemovePlayerFromObserverList(playerName);
        }

        [ClientRpc]
        private void RpcJoinGame(string newPlayerName, List<Seat> seatList, PlayerGameState playerGameState,
            int timeSpan)
        {
            if (isLocalPlayer)
            {
                TableGameManager.instance.JoinGameSuccesForLocalPlayer(newPlayerName,seatList,playerGameState,timeSpan);
                Player.localPlayer.CheckGameState();
            }
            else
            {
                //diğer oyuncuların gameScene'inde yeni katılan player spawn olacak
                TableGameManager.instance.JoinGameSuccesForNonLocalPlayers(newPlayerName, seatList);
            }
        }

        [TargetRpc]
        private void TargetJoinGameFailErrorTableToolsNotFound(string toolName)
        {
            Debug.LogWarning("Player.cs -->>> TargetJoinGameFailErrorTableToolsNotFound! ToolName :" + toolName + "\nKickPlayer...");
        }

        [TargetRpc]
        private void TargetJoinGameFailErrorPlayerNotFoundOnList(string playerName)
        {
            Debug.LogWarning("Player.cs -->>> TargetJoinGameFailErrorPlayerNotFoundOnList! PlayerName :" + playerName + "\nKickPlayer...");
        }

        [TargetRpc]
        private void TargetJoinGameFailErrorBalanceNotEnough()
        {
            TableGameManager.instance.JoinGameFail("Yetersiz Bakiye");
        }
        [TargetRpc]
        private void TargetJoinGameFailErrorBelowMinimum(float minDeposit)
        {
            string msg = String.Concat("Minimum yatırım : ", minDeposit.ToString());
            TableGameManager.instance.JoinGameFail(msg);
        }

        [TargetRpc]
        private void TargetJoinGameFailErrorSeatTaken()
        {
            TableGameManager.instance.JoinGameFail("Oturmak istediğiniz yer doldu");
        }

        [TargetRpc]
        private void TargetJoinGameFail()
        {
            //bu hata doldurulacak
        }

        ///////////////////////////////////////////////////OpenDepositOptions Section/////////////////////////////////////////////////////
        public void OpenDepositOptions(SeatLocations requestedSeatLocation)
        {
            CmdOpenDepositOptions(localPlayerName,requestedSeatLocation);
        }

        [Command]
        private void CmdOpenDepositOptions(string playerName, SeatLocations requestedSeatLocation)
        {
            string matchID = VisitorArea.instance.GetFromPlayerAndTableMap(playerName);
            if (!string.IsNullOrEmpty(matchID))
            {
                if (SpawnManager.instance.CheckAllTableTools(matchID))
                {
                    TableGameManager myTableGameManagerScript = SpawnManager.instance.GetTableGameManager(matchID);
                    float minDeposit = myTableGameManagerScript.MinDeposit;
                    if (!myTableGameManagerScript.isSeatActiveByLocation(requestedSeatLocation))
                    {
                        TargetOpenDepositOptions(minDeposit, requestedSeatLocation);
                    }
                    else
                    {
                        //kullanıcı deposit yapmaddan başka kullanıcı masaya oturmuş durumudur.
                        TargetOpenDepositOptionsFailErrorSeatTaken();
                    }
                }
                else
                {
                    //tableToolsNotWorking
                    TargetOpenDepositOptionsFail();
                }
                
            }
            else
            {
                //player'ın bulundugu Masa bulunamadı
                //buraya girmesini beklemiyorum
                Debug.LogWarning("Player.cs -->>> CmdOpenDepositOptions -->>> Player bir masaya kayıtlı değil lakin o masadan istek yolluyor");
            }
        }
    
        [TargetRpc]
        private void TargetOpenDepositOptionsFail()
        {
            Debug.LogWarning("Player.cs -->>> TargetOpenDepositOptionsFail");
        }

        [TargetRpc]
        private void TargetOpenDepositOptions(float minDeposit, SeatLocations requestedSeat)
        {
            
            TableGameManager.instance.DepositOptionsOpenSucces(minDeposit,requestedSeat);
        }

        [TargetRpc]
        private void TargetOpenDepositOptionsFailErrorSeatTaken()
        {
            TableGameManager.instance.DepositOptionsOpenFailErrorSeatTaken();
        }

        

        ///////////////////////////////////////////////////CreateNewUser Section/////////////////////////////////////////////////////
        /*
         * Çağıran -> UserCreationOptionUI
         */
        public void createNewUser(string username, string password, float  deposit,float  rakePercent, string friend,float  friendPercent, UserTypes userType)
        {
            CmdCreateNewUser(username, password,deposit,rakePercent,friend,friendPercent,LocalPlayerName,userType);
        }

        /*
         * 
         */
        [Command]
        private void CmdCreateNewUser(string username, string password, float  deposit,float  rakePercent, string friend,float  friendPercent,string adminName, UserTypes userType)
        {
            if (VisitorArea.instance.isAdmin(adminName))
            {
                if (!string.IsNullOrEmpty(username))
                {
                    
                    //check for username available
                    var dummyUser = (User) dbFacade.get(username, typeof(User));
                    if (dummyUser == null)
                    {
                        if (string.IsNullOrEmpty(friend))
                        {
                            if (password != null)
                            {
                                DateTime creationDate = DateTime.Now;
                                var newUser = new User(username, password, deposit,rakePercent,friend,friendPercent,adminName,creationDate ,userType);
                                dbFacade.put(newUser);
                                TargetCreateNewUser();
                            }
                            else
                            {
                                //password girilmemiş
                                TargetCreateNewUserFailErrorNoPassword();
                            }
                        }
                        else
                        {
                            //check for friend exist
                            dummyUser = (User) dbFacade.get(friend, typeof(User));
                            if (dummyUser != null)
                            {
                                if (password != null)
                                {
                                    DateTime creationDate = DateTime.Now;
                                    var newUser = new User(username, password, deposit,rakePercent,friend,friendPercent,adminName,creationDate ,userType);
                                    dbFacade.put(newUser);
                                    TargetCreateNewUser();
                                }
                                else
                                {
                                    //password girilmemiş
                                    TargetCreateNewUserFailErrorNoPassword();
                                }
                            }
                            else
                            {
                                //friend name yanlış
                                TargetCreateNewUserFailErrorWrongParentName();
                            }
                        }
                    
                    }
                    else
                    {
                        // username onceden alınmış veya uygun değil
                        TargetCreateNewUserFailErrorInvalidUsername();
                    }
                }
                else
                {
                    //username null
                    TargetCreateNewUserFailErrorInvalidUsername();
                }
            }
            else
            {
                //kickPlayer izinsiz işlem
            }
        }

        [TargetRpc]
        private void TargetCreateNewUser()
        {
           UserCreationOptionUI.instance.createNewUserSucces();
        }

        [TargetRpc]
        private void TargetCreateNewUserFailErrorInvalidUsername()
        {
            UserCreationOptionUI.instance.createNewUserFail("Invalid Username");
        }

        [TargetRpc]
        private void TargetCreateNewUserFailErrorWrongParentName()
        {
            UserCreationOptionUI.instance.createNewUserFail("Invalid Parent");
        }

        [TargetRpc]
        private void TargetCreateNewUserFailErrorNoPassword()
        {
            UserCreationOptionUI.instance.createNewUserFail("Invalid Password");
        }

        ///////////////////////////////////////////////////OpenUserCreationOption Section/////////////////////////////////////////////////////
        /*
         * çağıran -->> ControlAreaUI 
         */
        public void OpenUserCreationOptionUI()
        {
            CmdOpenUserCreationOptionUI(LocalPlayerName);
        }
        
        [Command]
        private void CmdOpenUserCreationOptionUI(string playerName)
        {
            if (VisitorArea.instance.isAdmin(playerName))
            {
                TargetOpenUserCreationOptionUI();
            }
            else
            {
                //Kick Player
            }
        }
        [TargetRpc]
        private void TargetOpenUserCreationOptionUI()
        {
            ControlAreaUI.instance.OpenUserCreationOptionUISucces();
        }

        ///////////////////////////////////////////////////LeaveTable Section/////////////////////////////////////////////////////
        public void LeaveTable()
        {
            CmdLeaveTable(LocalPlayerName);
        }

        [Command]
        private void CmdLeaveTable(string playerName)
        {
            string matchID = VisitorArea.instance.GetFromPlayerAndTableMap(playerName);
            TableGameManager myTableGameManagerScript = SpawnManager.instance.GetTableGameManager(matchID);
            List<Seat> oldSeatList = myTableGameManagerScript.MySeatList;
            
            LowKeyLeaveGame(playerName,oldSeatList,matchID);
            StartCoroutine(WaitForLeaveGame(playerName,matchID, myTableGameManagerScript));
        }

        public IEnumerator WaitForLeaveGame(string playerName,string matchID, TableGameManager myTableGameManagerScript)
        {
            yield return new WaitForSeconds(TimeManager.instance.LeaveGameDelayTime);
            if (myTableGameManagerScript.MyGameState == GameState.Pending)
            {
                LowKeyLeaveTable(playerName);
                User dummyUser = (User) dbFacade.get(usernamePlain, typeof(User));
                float currentBalance = dummyUser.Balance;
                TargetLeaveTable(matchID,currentBalance);
            }else
            {
                if(IsCoroutineTransferNeeded(playerName, myTableGameManagerScript))
                {
                    myTableGameManagerScript.IsCoroutineNeedTransfer = true;
                    myTableGameManagerScript.IsCoroutineTransferCompleted = false;
                    StartCoroutine(WaitForTransferCoroutineAnotherPlayer(playerName,matchID, myTableGameManagerScript));
                
                }else
                {
                    LowKeyLeaveTable(playerName);
                    User dummyUser = (User) dbFacade.get(usernamePlain, typeof(User));
                    float currentBalance = dummyUser.Balance;
                    TargetLeaveTable(matchID,currentBalance);
                }
            }
        }

        IEnumerator WaitForTransferCoroutineAnotherPlayer(string playerName, string matchID, TableGameManager myTableGameManagerScript)
        {
            TransferCoroutineAnotherPlayer(playerName,matchID, myTableGameManagerScript);
            yield return new WaitUntil(myTableGameManagerScript.GetIsCoroutineTransferCompleted);
            LowKeyLeaveTable(playerName);
            User dummyUser = (User) dbFacade.get(usernamePlain, typeof(User));
            float currentBalance = dummyUser.Balance;
            TargetLeaveTable(matchID,currentBalance);
        }

        private void TransferCoroutineAnotherPlayer(string playerName, string matchID, TableGameManager myTableGameManagerScript)
        {
            string anotherPlayerName = myTableGameManagerScript.GetAnotherPlayerName(playerName);
            GameObject anotherPlayerGameObject = FindPlayerGameObjectByLocalPlayerName(anotherPlayerName);
            if (anotherPlayerGameObject == null)
            {
               Debug.LogWarning("Player.cs -->>> CmdCheckGameState -->>> anotherPlayerGameObject null");
                    return;
            }
            anotherPlayerGameObject.GetComponent<Player>().TranferCoroutineToMe(playerName,anotherPlayerName,matchID, myTableGameManagerScript);
        }

        private void TranferCoroutineToMe(string playerName,string anotherPlayer, string matchID, TableGameManager myTableGameManagerScript)
        {
            Debug.LogWarning("Player.cs -->>> CorotineTransferToMe -->>> CoroutineAktarılıyor");
            WaitForPlayerMoveUntilTurnEndsAndDoAutoMoveCheckOrFold(myTableGameManagerScript);
            
            StartCoroutine(WaitForCoroutineTransferCompleted(playerName,anotherPlayer,matchID, myTableGameManagerScript));

        }

        IEnumerator  WaitForCoroutineTransferCompleted(string playerName,string anotherPlayer, string matchID, TableGameManager myTableGameManagerScript)
        {
            yield return new  WaitUntil(() => myTableGameManagerScript.GetCoroutineMaster() != playerName);
            Debug.LogWarning("Player.cs -->>> CorotineTransferToMe -->>> CoroutineAktarıldı");
            myTableGameManagerScript.IsCoroutineTransferCompleted = true;
            myTableGameManagerScript.IsCoroutineNeedTransfer = false;
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

        public void LowKeyLeaveGame(string playerName, List<Seat> oldSeatList, string matchId)
        {
            
            if (string.IsNullOrEmpty(matchId))
            {
                //matchID
                Debug.LogWarning("Player.cs -->>> CmdCheckGameState -->>> matchID'ye ait match bulunamadı");
                return;
            }
            if (!SpawnManager.instance.CheckAllTableTools(matchId))
            {
                Debug.LogWarning("Player.cs -->>> CmdCheckGameState -->>> CheckAllTableTools false döndü");
                return;
            }
            TableGameManager myTableGameManagerScript = SpawnManager.instance.GetTableGameManager(matchId);

            if (!IsPlayerOnGamePlayerList(playerName, myTableGameManagerScript))
            {
                Debug.LogWarning("Player.cs -->>> CmdCheckGameState -->>> IsPlayerOnGamePlayerList false döndü");
                return;
            }
            if (myTableGameManagerScript.MyGameState == GameState.Pending)
            {
                leaveGameFunctions(playerName,oldSeatList, myTableGameManagerScript);
                return;
            }
            
            //if player is in the game, add him to the list of players who will left the game
            //checkthatlist on fold move and if player is in the list, remove him from the game
            if (CheckPlayerIsInGame(playerName, myTableGameManagerScript))
            {
                bool hasTurn = CheckPlayerHasTurn(playerName, myTableGameManagerScript);
                if (hasTurn)
                {
                    if (isPlayerCanSayFold(playerName, myTableGameManagerScript))
                    {
                        LowKeyNextMove(Move.Fold,0,playerName);
                        leaveGameFunctions(playerName,oldSeatList, myTableGameManagerScript);
                    }
                    else
                    {
                        LowKeyNextMove(Move.Check,0,playerName);
                        AddPlayerToLeaverPlayerQueue(playerName, myTableGameManagerScript);
                    }
                    
                }else
                {
                    
                    AddPlayerToLeaverPlayerQueue(playerName, myTableGameManagerScript);
                    RemoveToggleMoveFromTheList(myTableGameManagerScript,playerName);
                }
            }
            else
            {
                //player fold demiş olabilir.
                //tum oyundan çıkarma işlemlerini yap
                leaveGameFunctions(playerName,oldSeatList, myTableGameManagerScript);
            }
        }

        private bool isPlayerCanSayFold(string playerName, TableGameManager myTableGameManagerScript)
        {
            float subMaxBet = myTableGameManagerScript.GetSubMaxBet();
            float playerSubMaxBet = myTableGameManagerScript.GetPlayerSubMaxBet(playerName);
            if (subMaxBet > playerSubMaxBet)
            {
                return true;
            }else
            {
                return false;
            }
        }

        private bool CheckPlayerHasTurn(string playerName, TableGameManager myTableGameManagerScript)
        {
            return ISeatList.GetSeatVarIsMyTurnByPlayerName(playerName, myTableGameManagerScript.MySeatList);
        }
        

        private void leaveGameFunctions(string playerName,List<Seat> oldSeatList, TableGameManager myTableGameManagerScript)
        {
            
            UpdateGameParticipationOnDatabase(playerName, myTableGameManagerScript,dbFacade);
            RemovePlayerFromGameParticipationIDMap(playerName, myTableGameManagerScript);
            UpdatePlayerBalanceAndRakeBackOnDatabaseAndPlayerGameObject(playerName, myTableGameManagerScript,dbFacade);
            UpdatePlayerParentRakeBackOnDatabase(playerName, myTableGameManagerScript,dbFacade);
            
            RemovePlayerFromLeaverPlayerQueue(playerName, myTableGameManagerScript);
            RemovePlayerFromSeatAndPlayerMap(playerName, myTableGameManagerScript);
            RemovePlayerFromGamePlayerList(playerName, myTableGameManagerScript);
            AddPlayerToObserverList(playerName, myTableGameManagerScript);
            
            RemovePlayerFromPlayerAndParentRakePercentMap(playerName, myTableGameManagerScript);
            RemoveParentFromParentAndParentRakeAmountMap(playerName, myTableGameManagerScript);
            RemovePlayerFromPlayerAndPlayerRakePercentMap(playerName, myTableGameManagerScript);
            RemovePlayerFromPlayerAndPlayerRakeAmountMap(playerName, myTableGameManagerScript);
            RemovePlayerFromPlayerAndParentMap(playerName, myTableGameManagerScript);

            
            ClearSeatFromSeatList(playerName, myTableGameManagerScript);
            SetActiveSeatLocations(myTableGameManagerScript);
            SendAllPlayersPlayerLeaveGame(playerName, myTableGameManagerScript,oldSeatList);
        }

        private void RemovePlayerFromLeaverPlayerQueue(string playerName, TableGameManager myTableGameManagerScript)
        {
            myTableGameManagerScript.RemovePlayerFromLeaverPlayerQueue(playerName);
        }


        private bool IsCoroutineTransferNeeded(string playerName, TableGameManager myTableGameManagerScript)
        {
            return myTableGameManagerScript.IsCoroutineTransferNeeded(playerName);
        }

        private void RemovePlayerFromGameParticipationIDMap(string playerName, TableGameManager myTableGameManagerScript)
        {
            myTableGameManagerScript.RemovePlayerFromGameParticipationIDMap(playerName);
        }

        private void SetActiveSeatLocations(TableGameManager myTableGameManagerScript)
        {
            myTableGameManagerScript.SetActiveSeatLocations();
        }

        private void ClearSeatFromSeatList(string playerName, TableGameManager myTableGameManagerScript)
        {
            myTableGameManagerScript.ClearSeatFromSeatList(playerName);
        }


        private bool CheckPlayerIsInGame(string playerName, TableGameManager tableGameManagerScript)
        {
            return tableGameManagerScript.CheckPlayerIsInGame(playerName);
        }

        private void AddPlayerToLeaverPlayerQueue(string playerName, TableGameManager tableGameManagerScript)
        {
            tableGameManagerScript.AddPlayerToLeaverPlayerQueue(playerName);
        }

        

        private void SendAllPlayersPlayerLeaveGame(string leavedPlayerName, TableGameManager myTableGameManagerScript,
            List<Seat> oldSeatList)
        {
            NetworkConnection myConnID;
            myConnID = VisitorArea.instance.GetConnFromPlayerHashMap(leavedPlayerName);
            List<Seat> newSeatList = myTableGameManagerScript.GetSeatList();
            List<string> observerList = myTableGameManagerScript.GetObserverList();
            int timeSpan = myTableGameManagerScript.TimeSpanGet();
            if (!myTableGameManagerScript.IsPlayerOnLeaverQueue(leavedPlayerName))
            {
                TargetPlayerSwapTableScene(myConnID,leavedPlayerName,newSeatList,timeSpan);
            }
            foreach (var seat in newSeatList)
            {
                if (seat.isActive)
                {
                    myConnID = VisitorArea.instance.GetConnFromPlayerHashMap(seat.username);
                    TargetPlayerLeaveGame(myConnID,leavedPlayerName,newSeatList,seat.username,oldSeatList,true);
                }
                                                
            }
            foreach (var observer in observerList)
            {
                if (!string.Equals(leavedPlayerName, observer))
                {
                    myConnID = VisitorArea.instance.GetConnFromPlayerHashMap(observer);
                    TargetPlayerLeaveGame(myConnID,leavedPlayerName,newSeatList,observer,oldSeatList,false);
                }
            }
        }

        [TargetRpc]
        private void TargetPlayerLeaveGame(NetworkConnection myConnID, string leavedPlayerName, List<Seat> newSeatList, string myName, List<Seat> oldSeatList, bool inGameScene)
        {
            TableGameManager.instance.PlayerLeaveGameSucces(leavedPlayerName,newSeatList,myName,oldSeatList,inGameScene);
        }

        [TargetRpc]
        private void TargetPlayerSwapTableScene(NetworkConnection myConnID, string playerName, List<Seat> seatList,
            int timeSpan)
        {
            TableGameManager.instance.SwapTableSceneFromGameSceneSucces(playerName,seatList,timeSpan);
        }

        private void RemovePlayerFromPlayerAndPlayerRakeAmountMap(string playerName, TableGameManager myTableGameManagerScript)
        {
            myTableGameManagerScript.RemovePlayerFromPlayerAndPlayerRakeAmountMap(playerName);
        }

        private void RemovePlayerFromPlayerAndPlayerRakePercentMap(string playerName, TableGameManager tableGameManagerScript)
        {
            tableGameManagerScript.RemovePlayerFromPlayerAndPlayerRakePercentMap(playerName);
        }

        private void RemoveParentFromParentAndParentRakeAmountMap(string playerName, TableGameManager tableGameManagerScript)
        {
            tableGameManagerScript.RemoveParentFromParentAndParentRakeAmountMap(playerName);
        }

        private void RemovePlayerFromPlayerAndParentRakePercentMap(string playerName, TableGameManager tableGameManagerScript)
        {
            tableGameManagerScript.RemovePlayerFromPlayerAndParentRakePercentMap(playerName);
        }

        private void RemovePlayerFromPlayerAndParentMap(string playerName, TableGameManager tableGameManagerScript)
        {
            tableGameManagerScript.RemovePlayerFromPlayerAndParentMap(playerName);
        }

        private void AddPlayerToObserverList(string playerName, TableGameManager tableGameManagerScript)
        {
            tableGameManagerScript.AddPlayerToObserverList(playerName);
        }

        private void RemovePlayerFromGamePlayerList(string playerName, TableGameManager tableGameManagerScript)
        {
            tableGameManagerScript.RemovePlayerFromGamePlayerList(playerName);
        }

        private void RemovePlayerFromSeatAndPlayerMap(string playerName, TableGameManager tableGameManagerScript)
        {
            int seatNumber = tableGameManagerScript.GetPlayerSeatNumber(playerName);
            tableGameManagerScript.RemovePlayerFromSeatAndPlayerMap(seatNumber);
        }

        private void UpdatePlayerParentRakeBackOnDatabase(string playerName, TableGameManager tableGameManagerScript, DBFacade dbFacade)
        {
            string parentName = tableGameManagerScript.GetPlayerParentName(playerName);
            if (!string.IsNullOrEmpty(parentName))
            {
                User parentUser = (User) dbFacade.get(parentName, typeof(User));
                if (parentUser != null)
                {
                    parentUser.RakeBackAmount += tableGameManagerScript.GetPlayerParentRakeBackAmount(parentUser.Username);
                    dbFacade.update(parentUser);
                }
            }
        }

        private void UpdatePlayerBalanceAndRakeBackOnDatabaseAndPlayerGameObject(string playerName, TableGameManager myTableGameManagerScript, DBFacade dbFacade1)
        {
            User dummyUser = (User) dbFacade1.get(playerName, typeof(User));
            if (dummyUser != null)
            {
                dummyUser = UpdateUserRakeBack(dummyUser,myTableGameManagerScript);
                dummyUser = UpdateUserBalance(dummyUser,myTableGameManagerScript);
                dbFacade1.update(dummyUser);
                balance = dummyUser.Balance;
            }
            else
            {
                Debug.LogWarning("Player does not exist in database! Player name: " + playerName + "UpdatePlayerBalanceAndRakeBack failed!");
            }
            
        }

        private User UpdateUserBalance(User user, TableGameManager myTableGameManagerScript)
        {
            user.Balance += myTableGameManagerScript.GetPlayerBalance(user.Username);
            return user;
        }

        private User UpdateUserRakeBack(User user, TableGameManager tableGameManagerScript)
        {
            user.RakeBackAmount += tableGameManagerScript.GetPlayerRakeBackAmount(user.Username);
            return user;
        }

        private void UpdateGameParticipationOnDatabase(string playerName, TableGameManager tableGameManagerScript,
            DBFacade facade)
        {
            int gameParticipationID = tableGameManagerScript.GetGameParticipationIdByPlayerName(playerName);
            if (gameParticipationID != -1)
            {
                GameParticipation gameParticipation = (GameParticipation) facade.get(gameParticipationID, typeof(GameParticipation));
                if (gameParticipation != null)
                {
                    gameParticipation.LeaveDate = DateTime.Now;
                    gameParticipation.LeaveBalance = tableGameManagerScript.GetPlayerBalance(playerName);
                    facade.update(gameParticipation);
                }
            }
        }

        private bool IsPlayerOnGamePlayerList(string playerName, TableGameManager myTableGameManagerScript)
        {
            return myTableGameManagerScript.MyGamePlayerManagerScript.isPlayerOnGamePlayerList(playerName);
        }

        /*
         * Logout işleminde tekrarlanabilir oldugu iiçin bu kısmı ayırdım
         */
        private string LowKeyLeaveTable(string playerName)
        {
            string matchID = VisitorArea.instance.GetFromPlayerAndTableMap(playerName);
            if (string.IsNullOrEmpty(matchID))
            {
                Debug.LogWarning("Player.cs -->>> LowKeyLeaveTable -->>> Player masada görünmüyor");
                return null;
                
            }
            if (!SpawnManager.instance.CheckAllTableTools(matchID))
            {
                Debug.LogWarning("Player.cs -->>> LowKeyLeaveTable -->>> TableTools null");
                return null;
            }
            TableGameManager myTableGameManagerScript = SpawnManager.instance.GetTableGameManager(matchID);
            if (!CheckPlayerIsInGame(playerName, myTableGameManagerScript))
            {
                //masanın player listini bul ve çıkart
                VisitorArea.instance.RemoveFromPlayerAndTableMap(playerName);
                
                    
                myTableGameManagerScript.MyTablePlayerManagerScript.RemovePlayerFromObserverList(playerName);
                myTableGameManagerScript.MyTablePlayerManagerScript.RemovePlayerFromTablePlayerList(playerName);
                    
                myTableGameManagerScript.PlayerListRemove(playerName);
                myTableGameManagerScript.RemoveToggleMoveFromTheList(playerName);
                //remove player from penalty list
            }
            else
            {
               //addplayerToVisitorArea Penalty list. 
            }
            
            return matchID;
            
        }

        [TargetRpc]
        private void TargetLeaveTable(string matchID, float currentBalance)
        {
            SceneManager.UnloadSceneAsync(3);
            SyncToLobby();
            GameObject dummyTableUIClone = SpawnManager.instance.GetFromTableUICloneMap(matchID);
            if (dummyTableUIClone != null)
            {
                TableUI myTableUIScript = SpawnManager.instance.GetTableUI(matchID);
                myTableUIScript.OpenPasswordOrJoinTableButton.interactable = true;
                myTableUIScript.PasswordCanvas.SetActive(false);
                myTableUIScript.TableDetailsCanvas.SetActive(true);
                LobbyUI.instance.Balance = currentBalance;
                IEnumerator coroutine = waitForSyncToLobby(0.5f);
                StartCoroutine(coroutine);
            }
            else
            {
                Debug.LogWarning("Player.cs -->>> TargetLeaveTable -->>> dummyTableUIClone null");
            }
            
            
        }
        IEnumerator waitForSyncToLobby( float second)
        {
            
            yield return new WaitForSeconds(second);
            SpawnManager.instance.SyncTables();
            
            yield return null;
        }
        
        ///////////////////////////////////////////////////JoinTable Section/////////////////////////////////////////////////////
        
        /*
         * Parametre olarak aldığı bilgileri
         * işleme konulmak üzere server'a yollar
         */
        public void JoinTable(string encryptedInputPassword, string matchID)
        {
            Debug.Log("Player.cs -->>> JoinTable -->>> ");
            CmdJoinTable(LocalPlayerName,matchID,encryptedInputPassword);
        }

        /*
         * Server üzerinde, player'in masaya giriş işlemini yapar ardından client tarafında işlemlerin gözükmesi için target fonksiyonunu cagırır
         * 1- Password'ü decrpyt eder
         * 2- Password'ü valide eder
         * 3- Giriş yapılmak istenen masanın varlığı ile ilgili gerekli kontrolleri yapar
         * 4- Masanın şifresini getirir. Ve password'ün uyuşup uyuşmadığını kontrol eder.
         * 5- Masaya ait TableUIDataStruct'a ulaşır.
         * 6- Masanın PlayerManager tool'una ulaşır. Ve playeri masanın PlayerNameList'ine ekler
         * 7- Gerekli işlemleri Client tarafında tamamlamak için tableUIdataStruct'u targetRPC ile client'e yollar
         */
        [Command]
        private void CmdJoinTable(string playerName, string matchID, string encryptedInputPassword)
        {
            string decryptedInputPassword = Decryptor.decrypt(encryptedInputPassword);
            if (!Validator.validate(decryptedInputPassword))
            {
                TargetJoinTableFailErrorWrongPassword(matchID);
                return;
            }
            if (!TableManager.instance.isTableExist(matchID))
            {
                Debug.LogWarning("Player.cs -->>> JoinTable -->>> Giriş yapılmak istenen masa bulunamadı!!!");
                return;
            }
            if (!SpawnManager.instance.isCloneAlive(matchID))
            {
                Debug.LogWarning("Player.cs -->>> JoinTable -->>> Giriş yapılmak istenen masa aktif değil!!!");
                return;
            }
            
            string tablePassword = TableManager.instance.GetTablePassword(matchID);
            
            if (!PasswordChecker.isPasswordCorrect(decryptedInputPassword,tablePassword))
            {
                TargetJoinTableFailErrorWrongPassword(matchID);
                return;
            }
            if (!SpawnManager.instance.CheckAllTableTools(matchID))
            {
                Debug.LogWarning("Player.cs -->>> CmdJoinTable -->>> SpawnManager.instance.CheckAllTableTools(matchID) false");
                return;
            }
            
            //Client'e yollanıyor
            TableUIDataStruct dummyStruct = SpawnManager.instance.GetFromTableUIDataStructMap(matchID);
            VisitorArea.instance.PlayerAndTableMap.Add(playerName,matchID);
            TableGameManager dummyTableGameManagerScript = SpawnManager.instance.GetTableGameManager(matchID);
            
            bool isPlayerOnRightLists = dummyTableGameManagerScript.MyTablePlayerManagerScript.AddPlayerToTablePlayerList(playerName)
                                        && dummyTableGameManagerScript.PlayerListAdd(playerName)
                                        && dummyTableGameManagerScript.MyTablePlayerManagerScript.AddPlayerToObserverList(playerName);
            
            if (!isPlayerOnRightLists)
            {
                Debug.LogWarning("Player.cs -->>> JoinTable -->>> Player zaten masada görünüyor. Giriş yapılamadı");
                //hatanın client tarafı burda yazılacak
                return;
            }

            PlayerGameState myGameState = PlayerGameState.Observer;
            int timeSpan = dummyTableGameManagerScript.TimeSpanGet();
            TargetJoinTable(dummyStruct,dummyTableGameManagerScript.MySeatList,myGameState,timeSpan,dummyTableGameManagerScript.MyGameState);
        }

        
        /*
         * Join table'ın client tarafını halleder.
         * 1- TableScene'i yükler
         * 2- Ardından yeni bir thread başlatır. Belirtilen süre kadar bekledikten sonra
         * 3- Table Toolları spawn eder.
         * 4- Kendine gelen dataStruct ile TableGameManager'in değerlerini set eder.
         * 5- Bu TableGameManageri kullanarak, GameUISpawnManager'in doğru sayıda player için koltuk barındıran Sahneyi aktif etmesini sağlar
         */
        [TargetRpc]
        private void TargetJoinTable(TableUIDataStruct tableDataStruct, List<Seat> seatList,
            PlayerGameState playerGameState, int timeSpan, GameState gameState)
        {
            SceneManager.LoadScene(3, LoadSceneMode.Additive);
            IEnumerator coroutine = waitForLoadTableScene(tableDataStruct,seatList,playerGameState,gameState,timeSpan,TimeManager.instance.LoadTableSceneDelayTime);
            StartCoroutine(coroutine);
        }
        IEnumerator waitForLoadTableScene(TableUIDataStruct tableUIDataStruct, List<Seat> seatList,
            PlayerGameState playerGameState, GameState gameState, int timeSpan, float second)
        {
            yield return new WaitForSeconds(second);
            
            //Diğer playerlerin ve masanın güncel durumunun spawn işlemi burda yapılacak
            SpawnManager.instance.SpawnTableTools(tableUIDataStruct);
            IEnumerator coroutine = WaitForSpawnTableTools(tableUIDataStruct,seatList,playerGameState,gameState,timeSpan,TimeManager.instance.SpawnTableToolsDelayTime);
            StartCoroutine(coroutine);
            yield return null;
        }

        private IEnumerator WaitForSpawnTableTools(TableUIDataStruct tableUIDataStruct, List<Seat> seatList,
            PlayerGameState playerGameState, GameState gameState, int timeSpan, float second)
        {
            yield return new WaitForSeconds(second);
            TableGameManager.instance.TableJoinSucces(seatList,localPlayerName,timeSpan,playerGameState,gameState);
            TableUI.instance.SuccesJoinTable(tableUIDataStruct.matchId);
            yield return null;
        }

        [TargetRpc]
        private void TargetJoinTableFailErrorWrongPassword(string matchID)
        {
            GameObject currentTableUIClone = SpawnManager.instance.GetFromTableUICloneMap(matchID);
            currentTableUIClone.GetComponent<TableUI>().OpenErrorCanvas();
        }
        ///////////////////////////////////////////////////Sync Section/////////////////////////////////////////////////////

        public void SyncToLobby()
        {
            Sync("lobby");
        }
        public void Sync(string matchID)
        {
            CmdSync(matchID);
        }
        
        /*
         * Server Side player game objecti etkiler
         * Player'ın lobby ile arasındaki bağlantıyı kopartır.
         * Lobby'nin networkable toolları artık kullanılamaz. Cunku Lobby networkunden Table Networkune gecılmıstır.
         */
        [Command]
        private void CmdSync(string matchID)
        {
            syncID = matchID;
            networkMatch.matchId = GuidGenerator.syncIDToGuid(syncID);
            networkMatchID = networkMatch.matchId.ToString();
            TargetSync(matchID);
        }

        /*
        * Client Side player game objecti etkiler.
         * !!!Bir yarari varmı bilemiyorum ileride görücez!!!
        * Player'ın lobby ile arasındaki bağlantıyı kopartır.
        * Lobby'nin networkable toolları artık kullanılamaz. Cunku Lobby networkunden Table Networkune gecılmıstır.
        */
        [TargetRpc]
        private void TargetSync(string matchID)
        {
            syncID = matchID;
            networkMatch.matchId = GuidGenerator.syncIDToGuid(syncID);
            networkMatchID = networkMatch.matchId.ToString();
        }
        ///////////////////////////////////////////////////CreateNewTable Section/////////////////////////////////////////////////////
        /*
         * Admin kullanıcıdan gelen password girdisi şifrelenmedi veya valide edilmedi ilerde sorun yaratabilir.!!!
         */
        public void createNewTable(string adminName, int seatCount, int smallBind,string password)
        {
            
            CmdCreateNewTable(adminName,seatCount,smallBind,password);
            
        }

        
        /*
         * yeni bir masa yaratma işlemini server uzerinde işletir.
         * öncelikle networkMatchID üretir, ardından parametre olarak gelen bilgilerle bir masa yaratır ve bu masayı
         * server üzerinde tutulan map'e ekler.
         * Masayı spawn etmek için, spawn bilgilerini tutan "TableUIDataStruct" örneği oluşturur ve bilgilerini set eder
         * Server üzerinde bulunan TableUIDataStructMap'i günceller ve CloneMatchIDSyncList'i günceller
         * ARdından dataStructta bulunan bilgiler ile Masayı server üzerinde Spawn eder.
         * oluşturduğu masayı kayıt eder ve masanın tüm playerlarda spawn edilebilmesi için masanın "spawn" bilgilerini tutan
         * dataStruct öğesini ClientRpc ile yollar
         *
         *
         *
         * işlemi yapan admin mi kontrol edilecek!!!
         */
        [Command]
        private void CmdCreateNewTable(string adminName, int seatCount, int smallBind,string password)
        {
            
            if (!TableManager.instance.isTableListFull())
            {
                //burda iki aynı guid olma durumu incelenebilir.
                string matchId = TableManager.instance.GenerateRandomMatchID();
                Guid networkMatchId = GuidGenerator.GenerateNewNetworkMatchId(matchId);
                
                Table newTable =TableManager.instance.CreateNewTable(adminName, matchId, seatCount,smallBind, networkMatchId, password);
                TableManager.instance.AddTableToTableMap(newTable);

                TableUIDataStruct newDataStruct = SpawnManager.instance.setDataStruct(newTable.MatchId, newTable.SeatCount,
                    newTable.SmallBlind, newTable.MinDeposit, newTable.HasPassword,newTable.TableRakePercent);
                
                SpawnManager.instance.UpdateTableUIDataStructMap(newDataStruct);
                SpawnManager.instance.UpdateCloneMatchIDSyncList(newTable.MatchId);
                
                // set tableCreation variables (date-time)
                dbFacade.put(newTable);
                
                
                //TableToollar spawn olmalı : Masa işlevsel olarak hazır hale gelmeli
                SpawnManager.instance.SpawnTableTools(newDataStruct);
                //JoinTable'da client side da buranın işlevselliğini orda göster
                //SpawnTable  : görseli playerda spawn ediyor
                RpcCreateNewTable(newDataStruct);
                
            }
            else
            {
                TargetCreateNewTableFailErrorTableListFull();
            }
        }
        
        [ClientRpc]
        private void RpcCreateNewTable(TableUIDataStruct tableUIDataStruct)
        {
            TableCreationOptionUI.instance.CreateNewTableSucces(tableUIDataStruct);
        }
        
        /*
         * LocalPlayer'üzerinde çalışır ve yeni masa kurma işleminin fail olduğunu bildirir.
         * Cunku Table List full'dür
         */
        [TargetRpc]
        private void TargetCreateNewTableFailErrorTableListFull()
        {
            TableCreationOptionUI.instance.CreateNewTableFailErrorTableListFull();
        }
        
        ///////////////////////////////////////////////////OpenTableCreationOptionUI Section/////////////////////////////////////////////////////
        public void GetTableStruct(string matchId)
        {
            CmdGetTableStruct(matchId);
        }
        
        [Command]
        private void CmdGetTableStruct(string matchId)
        {
            TableUIDataStruct dummyStruct = SpawnManager.instance.GetFromTableUIDataStructMap(matchId);
            if (dummyStruct.minDeposit != 0)
            {
                TargetGetTableStruct(dummyStruct);
            }
            else
            {
                Debug.LogWarning("Player.cs -->>> CmdGetTableStruct -->>> dataStruct'a ulaşılamadı ");
            }
            
        }

        [TargetRpc]
        private void TargetGetTableStruct(TableUIDataStruct dummyStruct)
        {
            SpawnManager.instance.GetTableStructSucces(dummyStruct);
        }

        ///////////////////////////////////////////////////OpenTableCreationOptionUI Section/////////////////////////////////////////////////////
        public void OpenTableCreationOptionUI()
        {
            CmdOpenTableCreationOptionUI(LocalPlayerName);
        }

        /*
         * TableCreationOptionUI'nın açılmasını kontrol ediyor.
         * Admin mi değil mi bu isteği yollayan
         */
        [Command]
        private void CmdOpenTableCreationOptionUI(string playerName)
        {
            if (VisitorArea.instance.isAdmin(playerName))
            {
                TargetOpenTableCreationOptionUI();
            }
            else
            {
                //Kick Player
            }
        }

        [TargetRpc]
        private void TargetOpenTableCreationOptionUI()
        {
            ControlAreaUI.instance.OpenTableCreationOptionUISucces();
        }
        ///////////////////////////////////////////////////Logout Section/////////////////////////////////////////////////////

        /*
        * Logout veya Disconnect işlemi Burdan başlayabilir <<<<<<<ApplicationFocus>>>>>.
        * Bu fonksiyon En son Build edilirken denenip farklı cihazlar için bir çözüm olablir
        
       private void OnApplicationFocus(bool hasFocus)
       {
           if (isLocalPlayer)
           {
               Debug.Log("Player.cs -->>> OnApplicationFocus -->>> hasFocus " + hasFocus);
               if (!hasFocus)
               {
                   if (Player.localPlayer.LocalPlayerName.Equals(null) || Player.localPlayer.LocalPlayerName.Equals(""))
                   {
                       Debug.Log("Player.cs -->>> OnApplicationFocus -->>> Login olmamış kullanıcı çıkış yaptı ");
                   }
                   else
                   {
                       Logout(localPlayer.LocalPlayerName);
                   }
               
               }
           }
       }
       */
        
        /*
        * Logout veya Disconnect işlemi burdan başlıyor
        * Player çıkış yaptıgında, server üzerinde tetiklediği event
         * İşlem sırası
         * 1 - VisitorArea'nın missingPlayerId değişkeni ile bu işlemi tetikleyen kullanıcının username'ine ulasmaya calısır.
         * 2 - Eğer username mevcut ise : Bu username ve connectionID ile Logout işlemini başlatır
         * 3 - username mevcut değilse! Bu durum kullanıcının login olmadığına işaret eder ve uygun debug.log gösterilir.
        */
        public override void OnStopServer()
        {
            Debug.Log($"Player.cs -->>> OnStopServer -->>> Bir player çıkış yaptı");
            bool isLoggedIn =
                VisitorArea.instance.GetPlayerNameFromPlayerHashMap(VisitorArea.instance.MissingPlayerId,
                    out var missingPlayer);
            
            if (isLoggedIn)
            {
                Logout(VisitorArea.instance.MissingPlayerId,missingPlayer);
                Debug.Log ($"Player.cs -->>> OnStopServer -->>> Ayrılan player tespit edildi : " + missingPlayer + "\nVe çıkış işlemi kayıt edildi");
            }
            else
            {
                Debug.Log("Player.cs -->>> Logout -->>> Ayrılan player tespit edilemedi");
            }
            base.OnStopServer();
           
        }
        
        /*
         * Parametre olarak, bilgileri gelen kullanıcı için Logout işlemini kayıt eder
         * Ardından playerHashmap'ten kullanıcıyı siler
         * Son olarak onlinePlayerList'i günceller
         */
        public void Logout(NetworkConnectionToClient connID, string username)
        {
            
            if (VisitorArea.instance.onlinePlayerList.Contains(username))
            {
                // dummy logout nesnesi oluştur. bilgilerini doldur ve kayıt et
                Logout newLogout = new Logout(username);
                dbFacade.put(newLogout);
                
                VisitorArea.instance.RemovePlayerFromPlayerHashMap(connID);
                VisitorArea.instance.UpdateOnlinePlayerSyncList();
                Debug.Log("Player.cs -->>> Logout -->>> onlinePlayerList'ten çıkarıldı :  " + username);
            }
            else
            {
                Debug.Log("Player.cs -->>> CmdLogout -->>> zaten çıkış yapmış " + username);
            }

        }
        
        //Bu command kalsın belki cağırmak lazım olur
        [Command]
        void CmdLogout(string username)
        {
            if (VisitorArea.instance.onlinePlayerList.Contains(username))
            {
                // dummy logout nesnesi oluştur. bilgilerini doldur ve kayıt et
                Logout newLogout = new Logout(username);
                dbFacade.put(newLogout);
                
                VisitorArea.instance.onlinePlayerList.Remove(username);
                Debug.Log("Player.cs -->>> CmdLogout -->>> onlinePlayerListten çıkarıldı :  " + username);
            }
            else
            {
                Debug.Log("Player.cs -->>> CmdLogout -->>> zaten çıkış yapmış " + username);
            }
        }
        
        
        ///////////////////////////////////////////////////Login Section/////////////////////////////////////////////////////
        /*
         * Bu fonksiyon LoginUI'dan çağırılıyor. Dolayısı ile user'ın local makinesinden yapılan bir fonksiyon çağrısıdır.
         * 1- Encrypt şekilde gelen username ve password'ü Server'a iletecek.
         */
        public void Login(string usernameCypher, string passwordCypher)
        {
            CmdLogin(usernameCypher, passwordCypher);
        }

        /*
         * Server üzerinde çalışan bir fonksiyondur.
         * Çeşitli kontrollerden sonra kendine parametre olarak gelen username password ile login işlemini yürütür
         * Ardından TargetLogin'i tetikler
         */
        [Command]
        void CmdLogin(string usernameCypher, string passwordCypher)
        {
            usernamePlain = Decryptor.decrypt(usernameCypher);
            passwordPlain = Decryptor.decrypt(passwordCypher);

            bool isUsernameValid =Validator.validate(usernamePlain);
            bool isPasswordValid =Validator.validate(passwordPlain);
            if (isUsernameValid  && isPasswordValid)
            {
                /*
                 * username bilgisini kullanarak öncelikle user bilgilerini fetch etmeliyiz...
                 * Bunun için önce dummy bir user nesnesi yaratmalıyım
                 */
                User dummyUser;
                dummyUser = (User) dbFacade.get(usernamePlain, typeof(User));
                
                
                if (dummyUser != null)
                {
                    if (PasswordChecker.isPasswordCorrect(passwordPlain,dummyUser.Password ))
                    {
                        if (!VisitorArea.instance.isOnline(dummyUser.Username))
                        {
                            localPlayerName = dummyUser.Username;
                            password = dummyUser.Password;
                            balance = dummyUser.Balance;
                            rakePercent = dummyUser.RakePercent;
                            parent = dummyUser.Parent;
                            parentPercent = dummyUser.ParentPercent;
                            creator = dummyUser.Creator;
                            creationDate = dummyUser.CreationDate.ToString();
                            userType = dummyUser.UserType.ToString();
                            if (dummyUser.UserType == UserTypes.Admin)
                            {
                                VisitorArea.instance.AddUserToAdminList(dummyUser.Username);
                            }
                            if (dummyUser.UserType == UserTypes.Operator)
                            {
                                VisitorArea.instance.AddUserToOperatorList(dummyUser.Username);
                            }

                            
                            NetworkIdentity playerIdentity = gameObject.GetComponent<NetworkIdentity>();
                            NetworkConnectionToClient connID = playerIdentity.connectionToClient;
                            
                            VisitorArea.instance.AddPlayerToPlayerHashMap(connID,dummyUser.Username);
                            VisitorArea.instance.UpdateOnlinePlayerSyncList();
                            
                            // dummy login nesnesi oluştur. bilgilerini doldur ve kayıt et
                            Login newLogin = new Login(dummyUser.Username);
                            dbFacade.put(newLogin);
                            
                            //ilgili user bilgilerini targetCliente yolla
                            TargetLogin(dummyUser.Username,dummyUser.Balance,dummyUser.UserType);
                            
                        }
                        else
                        {
                            //User already ONLİNE
                            TargetLoginErrorUserAlreadyOnline();
                        }
                    }
                    else
                    {
                        //Username or password WRONG
                        TargetLoginErrorInvalidPasswordOrUsername();
                    }
                }
                else
                {
                    //User doesn't EXİST
                    TargetLoginErrorInvalidPasswordOrUsername();
                }
            }
            else
            {
                //Username or password INVALİD
                TargetLoginErrorInvalidPasswordOrUsername();
            }
        }

        /*
         * Client'in lobby ekranına geçişini yapar.
         * Bu Load işlemi sırasında zaman kaybı olduğu için load ekranına bilgiler doğrudan yollanamaz. Çünkü bu zaman kaybı sebebiyle LobbyScene henüz start olmamıştır.
         * Bu sebeple yeni bir thread oluşturur. LobbyScene'e bilgiler bu thread içinde ufak bir delay'in ardından yollanır
         * Ardından LoginUI'ın loginSucces fonksiyonu çağrılır.
         */
        [TargetRpc]
        private void TargetLogin(string username, float balance, UserTypes userTypes)
        {
            SceneManager.LoadScene(2, LoadSceneMode.Additive);
            IEnumerator coroutine = waitForLoadLobbyScene(username,balance,userTypes,2.0f);
            StartCoroutine(coroutine);
            LoginUI.instance.loginSucces(username);
        }

        /*
         * tüm masaları spawn etmek için server üzerinde çalıştırılır.
         * Server kendi üzerindeki synclistten kontrol eder eğer var ise spawn edilecek masa
         * her biri için dataSTruct oluşturur ve bunları tek tek spawn edilmek üzere TargetRpc ile yollar
         * işlem tüm masalar spawn edilene kadar devam eder
         */
        [Command]
        private void CmdSpawnAllTableUI()
        {
            TableUIDataStruct dummyStruct = new TableUIDataStruct();
            dummyStruct = default;
            foreach (var matchID in SpawnManager.instance.tableCloneMatchIDSyncList)
            {
                dummyStruct = SpawnManager.instance.GetFromTableUIDataStructMap(matchID);
                TargetSpawnTableUI(dummyStruct);
            }
        }
        
        [TargetRpc]
        private void TargetSpawnTableUI(TableUIDataStruct tableUIDataStruct)
        {
            LobbyUI.instance.SpawnTableUISucces(tableUIDataStruct);
        }

        /*
         * Yeni bir thread oluşturulur.
         * Bu thread belirtilen saniye kadar wait işlemi yapar.
         * Aktif masaları spawn eder
         * Ardından atamaları yapar.
         * Ve main threade geri döner
         */
        IEnumerator waitForLoadLobbyScene(string username, float balance, UserTypes userTypes, float second)
        {
            Debug.Log("Player.cs -->>> TargetLogin -->>> waitForSeconds : " + second);
            yield return new WaitForSeconds(second);
            Player.localPlayer.CmdSpawnAllTableUI();
            LobbyUI.instance.loginSucces(username,balance,userTypes);
           
            yield return null;
        }

        /*
         * LocalPlayer'üzerinde çalışır ve giriş işleminin fail olduğunu bildirir.
         */
        [TargetRpc]
        private void TargetLoginErrorInvalidPasswordOrUsername()
        {
            LoginUI.instance.loginFailErrorInvalidPasswordOrUsername();
        }
        
        /*
         * LocalPlayer'üzerinde çalışır ve giriş işleminin fail olduğunu bildirir.
         */
        [TargetRpc]
        private void TargetLoginErrorUserAlreadyOnline()
        {
            LoginUI.instance.loginFailErrorUserAlreadyOnline();
        }
        
        ///////////////////////////////////////////////////Properties Section/////////////////////////////////////////////////////
        public DBFacade DBFacade
        {
            get => dbFacade;
            set => dbFacade = value;
        }

        public string LocalPlayerName
        {
            get => localPlayerName;
            set => localPlayerName = value;
        }


        
    }
    
    public enum PlayerGameState
    {
        Observer,
        InGame
    }

}