using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

/*
 13.700 loc
 * Bir oyun masasında, Turn işlemlerini kontrol edecek sınıftır.
 * ****************************************************************
 * activeSeatLocations: Oyun masasında aktif olan oyuncuların konumlarını tutar.
Bu konumlar, oyuncuların koltuk yerine göre küçükten büyüğe göre sıralanır.
 * 
 */
namespace Assets.Scripts
{
    public class TurnManager : MonoBehaviour
    {
        ///////////////////////////////////////////////////Variables Section/////////////////////////////////////////////////////
        public static TurnManager instance;
        
        [SerializeField] private string syncID;
        private int[] activeSeatLocations = null;
        private DateTime currentMoveStartTime;
        private bool moveTimeCalculate = false;
        List<Tuple<string,List<CardData>>> winnersAndCombinationsList = new List<Tuple<string, List<CardData>>>();
        private TurnWinners turnWinners;
        [SerializeField] private int totalMoveTime = 30;
        [SerializeField] private int totalMoveTimeForEndGame = 5;
        [SerializeField] private bool isPlayerTurnEnd;
        [SerializeField] private bool isPlayerTimeOut;
        
        [Header("Turn Details")]
        [SerializeField] private SeatLocations currentHasTurnlocation;
        [SerializeField] private string currentHasTurnPlayer;
        [SerializeField] private int turnTime;
        private DateTime turnStartDate;
        [Header("BigBlind Details")]
        [SerializeField] private SeatLocations currentBigBlindlocation;
        [SerializeField] private string currentBigBlindPlayer;
        [Header("SmallBlind Details")]
        [SerializeField] private SeatLocations currentSmallBlindlocation;
        [SerializeField] private string currentSmallBlindPlayer;
        [Header("CurrentDealer")]
        [SerializeField] private SeatLocations currentDealerlocation;
        [SerializeField] private string currentDealerPlayer;
        List<GameMove> moveList = new List<GameMove>();
        List<GameMove> subMoveList = new List<GameMove>();
        [Header("MoveSequanceNo")] 
        [SerializeField] private int moveSequanceNo = 0;

        private GameMove correctionReturnMove;
        private float correctionReturnAmount;
        private float correctionBetValueForClientSide;

        [Header("TurnState")]
        [SerializeField] private TurnStage myTurnStage = TurnStage.None;
        [SerializeField] private bool isSubTurnStageEnded;
        [SerializeField] private bool ReadyForTurnEnded;
        [SerializeField] private bool isEndGame;
        [SerializeField] private TurnEndType turnEndType;

        [Header("SideBet")]
        [SerializeField] private bool isSubTurnStageCauseSideBet;
        
        [Header("AllinAndFoldedPlayers")]
        private List<string> allinPlayerList = new List<string>();
        [SerializeField]private int allinPlayerCount;
        private List<string> foldedPlayerList = new List<string>();
        [SerializeField]private int foldedPlayerCount;
        ///////////////////////////////////////////////////Events Section/////////////////////////////////////////////////////
        void Start()
        {
            instance = this;
        }

        private void Update()
        {
            TurnTimeCalculateIfConditionTrue(moveTimeCalculate);
        }
        ///////////////////////////////////////////////////NewTurnInitialSettings Section/////////////////////////////////////////////////////
        public void ActiveSeatLocationsArrayInitialize(List<Seat> seatList)
        {
            SetActiveSeatLocationsArray(seatList);
        }
        
        ///////////////////////////////////////////////////TurnMoves Section/////////////////////////////////////////////////////
        public bool CurrentHasTurnPlayerCheckByPlayerName(string playerName)
        {
            if (string.Equals(currentHasTurnPlayer, playerName))
            {
                return true;
            }
            else
            {
                Debug.LogWarning("TurnManager Says Player " + playerName + " has not turn");
                return false;
            }
            
        }
        
        public bool MoveControlMoveAmountIsValid(string playerName, Move myNextMove, float moveAmount, float subMaxBet,
            float maxRaiseLimit,
            float currentTotalPot, float currentBalance)
        {
            float myPreviousTotalBet = SubMoveListCalculateMyPreviousTotalBetAmount(playerName);
            float callAmount = subMaxBet - myPreviousTotalBet;
            switch (myNextMove)
            {
                case Move.Check:
                    if (myPreviousTotalBet == subMaxBet)
                    {
                        return true;
                    }
                    else
                    {
                        Debug.LogWarning("TurnManager Says PreviousTotalBet is not valid for Check");
                        return false;
                    }
                case Move.Call:
                    if (callAmount == moveAmount)
                    {
                        return true;
                    }
                    else
                    {
                        Debug.LogWarning("TurnManager Says MoveAmount is not valid for Call");
                        return false;
                    }
                case Move.Raise:
                    float raiseLimitAmount = maxRaiseLimit;
                    if (raiseLimitAmount >= moveAmount)
                    {
                        return true;
                    }
                    else
                    {
                        Debug.LogWarning("TurnManager Says MoveAmount is not valid for Raise");
                        return false;
                    }
                case Move.AllIn:
                    float raiseLimitAmount2 = maxRaiseLimit;
                    if (currentBalance <= raiseLimitAmount2)
                    {
                        return true;
                    }
                    else
                    {
                        Debug.LogWarning("TurnManager Says Balance is not valid for AllIn");
                        return false;
                    }
                
            }
            return false;
        }
        
        ///////////////////////////////////////////////////SubMoveList Section/////////////////////////////////////////////////////
        public List<GameMove> SubMoveListGet()
        {
            return subMoveList;
        }
        private float SubMoveListCalculateMyPreviousTotalBetAmount(string playerName)
        {
            float myPreviousTotalBet = 0;
            foreach (var move in subMoveList)
            {
                if (string.Equals(move.PlayerName, playerName))
                {
                    myPreviousTotalBet += move.Amount;
                }
            }
            return myPreviousTotalBet;
        }
        ///////////////////////////////////////////////////SubBet Section/////////////////////////////////////////////////////
        private float SubMoveListFindCurrentHasPlayerTotalSubBet()
        {
            float playerTotalSubMoveBets = 0;
            foreach (var subMove in subMoveList)
            {
                if (subMove.PlayerName == currentHasTurnPlayer)
                {
                    playerTotalSubMoveBets += subMove.Amount;
                }
            }

            return playerTotalSubMoveBets;
        }
        ///////////////////////////////////////////////////TurnMovesCheckOrFold Section/////////////////////////////////////////////////////
        public bool DecideCheckOrFoldReturnTrueIfItsCheck(float subMaxBet)
        {
            float playerTotalSubMoveBets = SubMoveListFindCurrentHasPlayerTotalSubBet();
            if (playerTotalSubMoveBets == subMaxBet)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        ///////////////////////////////////////////////////TurnTimer Section/////////////////////////////////////////////////////
        public void MoveTimerStart()
        {
            turnTime = 0;
            currentMoveStartTime = DateTime.Now;
            isPlayerTurnEnd = false;
            isPlayerTimeOut = false;
            moveTimeCalculate = true;
            
        }
        public bool GetIsPlayerTurnEnd()
        {
            return isPlayerTurnEnd;
        }
        public bool GetIsPlayerTimeOut()
        {
            return isPlayerTimeOut;
        }
        private void TurnTimeCalculateIfConditionTrue(bool calculateTurnTime)
        {
           int totalMoveTimeWithGap = totalMoveTime + 2;
           int totalMoveTimeForEndGameWithGap = totalMoveTimeForEndGame + 2;
            
           if (calculateTurnTime)
           {
               TimeSpan timeSpan = DateTime.Now - currentMoveStartTime;
               turnTime = (int)timeSpan.TotalSeconds;
               if (isEndGame)
               {
                   if (turnTime > totalMoveTimeForEndGameWithGap)
                   {
                       isPlayerTurnEnd = true;
                       isPlayerTimeOut = true;
                        
                   }
               }
               else
               {
                   if (turnTime  > totalMoveTimeWithGap)
                   {
                       isPlayerTurnEnd = true;
                       isPlayerTimeOut = true;
                   }
               }
                
           }
        }
        
        public void TimerAnimationActivate(List<Seat> seatList, GameObject activeScene, bool isGameScene, int timeSpan,
            int totalMoveTime)
        {
            
            int remainingTime = CalculateRemainingTime(timeSpan, totalMoveTime);
            
            TurnTimerAnimate(remainingTime,totalMoveTime,seatList,activeScene,isGameScene);
            
        }
        
        private void TurnTimerAnimate(int remainingTime, int totalMoveTime, List<Seat> seatList, GameObject activeScene,
            bool isGameScene)
        {
            SeatLocations animationSeatLocation = ISeatList.GetSeatLocationByIsMyTurn(seatList);
            if (animationSeatLocation != SeatLocations.None)
            {
                if (seatList.Count == 6)
                {
                    if (isGameScene)
                    {
                        activeScene.GetComponent<SixPlayerGameScene>().DisableAllTimers();
                        activeScene.GetComponent<SixPlayerGameScene>().AnimateTurnTimer(animationSeatLocation, remainingTime,
                            totalMoveTime,activeScene,seatList);
                    }
                    else
                    {
                        activeScene.GetComponent<SixPlayerTableScene>().DisableAllTimers();
                        activeScene.GetComponent<SixPlayerTableScene>().AnimateTurnTimer(animationSeatLocation, remainingTime,
                            totalMoveTime);
                    }
                }
            }
            
        }
        
        private int CalculateRemainingTime(int timeSpan, int totalMoveTime)
        {
            int remainingTime = totalMoveTime - timeSpan;
            if (remainingTime < 0)
            {
                remainingTime = 0;
            }
            return remainingTime;
            
        }
        
        public int TimeSpanGet()
        {
            return turnTime;
        }

       ///////////////////////////////////////////////////GameMoveCreation Section/////////////////////////////////////////////////////
        public GameMove CreateNewGameMove(List<Seat> seatList, int currentTurnID,float currentBalance, Move move, float moveAmount,float totalRake,
            float userRakeBack,float parentRakeBack, float profit)
        {
            GameMove newGameMove = null;
            if (move == Move.SmallBlind)
            {
                moveList.Clear();
                subMoveList.Clear();
                newGameMove = CreateMoveSmallBlind(currentTurnID,currentBalance,moveAmount,totalRake,userRakeBack,parentRakeBack,profit);
                moveList.Add(newGameMove);
                subMoveList.Add(newGameMove);
                return newGameMove;
            }
            if (move == Move.BigBlind)
            {
                newGameMove = CreateMoveBigBlind(currentTurnID,currentBalance,moveAmount,totalRake,userRakeBack,parentRakeBack,profit);
                moveList.Add(newGameMove);
                subMoveList.Add(newGameMove);
                return newGameMove;
            }

            if (move == Move.Call)
            {
                newGameMove = GameMoveCreate(seatList,currentTurnID,currentBalance,move,moveAmount,totalRake,userRakeBack,parentRakeBack,profit);
                moveList.Add(newGameMove);
                subMoveList.Add(newGameMove);
                isSubTurnStageEnded = CheckStageEnd(seatList);
                if (isSubTurnStageEnded)
                {
                    isSubTurnStageCauseSideBet = CheckSubTurnStageCauseSideBet();
                }
            }
            if (move == Move.Check)
            {
                newGameMove = GameMoveCreate(seatList,currentTurnID,currentBalance,move,moveAmount,totalRake,userRakeBack,parentRakeBack,profit);
                moveList.Add(newGameMove);
                subMoveList.Add(newGameMove);
                isSubTurnStageEnded = CheckStageEnd(seatList);
                if (isSubTurnStageEnded)
                {
                    isSubTurnStageCauseSideBet = CheckSubTurnStageCauseSideBet();
                }
            }
            if (move == Move.Raise)
            {
                newGameMove = GameMoveCreate(seatList,currentTurnID,currentBalance,move,moveAmount,totalRake,userRakeBack,parentRakeBack,profit);
                moveList.Add(newGameMove);
                subMoveList.Add(newGameMove);
                //isSubTurnStageEnded = CheckStageEnd(seatList);
            }
            if (move == Move.AllIn)
            {
                newGameMove = GameMoveCreate(seatList,currentTurnID,currentBalance,move,moveAmount,totalRake,userRakeBack,parentRakeBack,profit);
                moveList.Add(newGameMove);
                subMoveList.Add(newGameMove);
                isSubTurnStageEnded = CheckStageEnd(seatList);
                //yapılan bu allin hamlesi, içinde bulundugumuz subturnde yapılmış, başka bir hamleyede düzeltme yapılmasına sebep oluyor mu

                if (isSubTurnStageEnded)
                {
                    int correctionMoveSeqNo = CheckMoveCauseCorrection(seatList);
                    if (correctionMoveSeqNo != -1)
                    {
                        SetCorrectionMove(correctionMoveSeqNo);
                    }
                }
                if (isSubTurnStageEnded)
                {
                    isSubTurnStageCauseSideBet = CheckSubTurnStageCauseSideBet();
                }
            }
            if (move == Move.Fold)
            {
                newGameMove = GameMoveCreate(seatList,currentTurnID,currentBalance,move,moveAmount,totalRake,userRakeBack,parentRakeBack,profit);
                moveList.Add(newGameMove);
                subMoveList.Add(newGameMove);
                isSubTurnStageEnded = CheckStageEnd(seatList);
                //yapılan bu fold hamlesi, içinde bulundugumuz subturnde yapılmış, başka bir hamleyede düzeltme yapılmasına sebep oluyor mu
                if (isSubTurnStageEnded)
                {
                    int correctionMoveSeqNo = CheckMoveCauseCorrection(seatList);
                    if (correctionMoveSeqNo != -1)
                    {
                        SetCorrectionMove(correctionMoveSeqNo);
                    }
                }
                if (isSubTurnStageEnded)
                {
                    isSubTurnStageCauseSideBet = CheckSubTurnStageCauseSideBet();
                }
                
            }
            if (move == Move.HideCards)
            {
                newGameMove = GameMoveCreate(seatList,currentTurnID,currentBalance,move,moveAmount,totalRake,userRakeBack,parentRakeBack,profit);
                moveList.Add(newGameMove);
                subMoveList.Add(newGameMove);
                isSubTurnStageEnded = true;
            }
            if (move == Move.ShowCards)
            {
                newGameMove = GameMoveCreate(seatList,currentTurnID,currentBalance,move,moveAmount,totalRake,userRakeBack,parentRakeBack,profit);
                moveList.Add(newGameMove);
                subMoveList.Add(newGameMove);
                isSubTurnStageEnded = true;
            }
            return newGameMove;
        }

        private bool CheckSubTurnStageCauseSideBet()
        {
            if (!IGameMoveList.HasAllin(subMoveList))
            {
                return false;
            }
            if (IGameMoveList.GenerateInGamePlayerList(subMoveList).Count < 3)
            {
                return false;
            }

            return true;
        }

        private int CheckMoveCauseCorrection(List<Seat> seatList)
        {
            int correctionMoveSeqNo = -1;
            int foldCount = IGameMoveList.GetMoveCountByMove(subMoveList,Move.Fold);
            int allInCount = IGameMoveList.GetMoveCountByMove(subMoveList,Move.AllIn);
            int playerCount = IGameMoveList.GetPlayerCount(subMoveList);
            List<string> inGamePlayerList = ISeatList.GenerateInGamePlayerList(seatList);
            List<GameMove> subMoveListWithOutSmallAndBigBlind = subMoveList.Where(x => x.move != Move.SmallBlind && x.move != Move.BigBlind).ToList();
            
            //exclude allin players and folded players from inGamePlayerList
            inGamePlayerList = RemoveAllinAndFoldedPlayersFromInGamePlayerList(inGamePlayerList);
            
            //subTurnde herkes hamle yapmamış ise
            if (!allPlayerHasMovedInSubTurnStage(inGamePlayerList, subMoveListWithOutSmallAndBigBlind))
            { 
                return -1;
            }
            
            //Turn'e devam edebilecek en az iki oyuncu var ise
            if (playerCount - foldCount - allInCount >= 2)
            {
                return -1;
            }
            
            //Turn'e devam edebilecek bir oyuncu var ise
            if (playerCount - foldCount - allInCount == 1)
            {
                //find that player and return his last move sequence no
                string correctionPlayerName = ISeatList.GetLastPlayerIncludeAllin(seatList);
                if (subMoveList.Where(x => x.PlayerName == correctionPlayerName).Count() != 0)
                {
                    return  subMoveList.Where(x => x.PlayerName == correctionPlayerName).Max(x => x.MoveSequenceNo);
                }

                return -1;
            }
            
            //Turn'e devam edebilecek oyuncu kalmamış ise
            if (playerCount - foldCount - allInCount == 0)
            {
                //all'in playerlar arasından find who has bigger total bet and return his last move sequence no
                string correctionPlayerName = IGameMoveList.WhichPlayerHasBiggerTotalBetByMove(subMoveList,Move.AllIn);
                if (subMoveList.Where(x => x.PlayerName == correctionPlayerName).Count() != 0)
                {
                    return  subMoveList.Where(x => x.PlayerName == correctionPlayerName).Max(x => x.MoveSequenceNo);
                }

                return -1;
            }
            
            Debug.LogWarning("TurnManager--->>CheckMoveCauseCorrection: Hatalı durum");
            return -1;
        }

        private List<string> RemoveAllinAndFoldedPlayersFromInGamePlayerList(List<string> inGamePlayerList)
        {
            List<string> inGamePlayerListWithOutAllinAndFoldedPlayers = new List<string>();
            List<string> allinPlayerList = IGameMoveList.GeneratePlayerListByMove(moveList,Move.AllIn);
            List<string> foldedPlayerList = IGameMoveList.GeneratePlayerListByMove(moveList,Move.Fold);
            
            foreach (var playerName in inGamePlayerList)
            {
                if (!allinPlayerList.Contains(playerName) && !foldedPlayerList.Contains(playerName))
                {
                    inGamePlayerListWithOutAllinAndFoldedPlayers.Add(playerName);
                }
            }

            return inGamePlayerListWithOutAllinAndFoldedPlayers;
        }

        private void SetCorrectionMove(int correctionMoveSeqNo)
        {
            if (correctionMoveSeqNo != -1)
            {
                foreach (var gameMove in moveList)
                {
                    if (gameMove.MoveSequenceNo == correctionMoveSeqNo)
                    {
                        correctionReturnMove = gameMove;
                    }
                }
            }
        }

        

       

        private GameMove GameMoveCreate(List<Seat> seatList, int currentTurnID, float currentBalance, Move move,
            float moveAmount,float totalRake,float userRakeBack,float parentRakeBack, float profit)
        {
            MoveSequanceNo = MoveSequanceNo + 1;
            string playerName = ISeatList.GetPlayerNameByIsMyTurn(seatList);
            float moveTime = ISeatList.GetMoveTimeByPlayerName(seatList, playerName);
            GameMove newGameMove = new GameMove(currentTurnID,MoveSequanceNo,playerName,currentBalance,moveTime,move,moveAmount,totalRake,userRakeBack,parentRakeBack,profit);
            return newGameMove;
        }
        
        private GameMove CreateMoveBigBlind(int currentTurnID, float currentBalance, float moveAmount,float totalRake,float userRakeBack,float parentRakeBack, float profit)
        {
            MoveSequanceNo = 2;
            string bigBlindPlayerName = CurrentBigBlindPlayer;
            float moveTime = 0;
            Move move = Move.BigBlind;
            GameMove newGameMove = new GameMove(currentTurnID,MoveSequanceNo,bigBlindPlayerName,currentBalance,moveTime,move,moveAmount,totalRake,userRakeBack,parentRakeBack,profit);
            return newGameMove;
        }

        private GameMove CreateMoveSmallBlind(int currentTurnID,float currentBalance, float moveAmount,float totalRake,float userRakeBack,float parentRakeBack, float profit)
        {
            MoveSequanceNo = 1;
            string smallBlindPlayerName = CurrentSmallBlindPlayer;
            float moveTime = 0;
            Move move = Move.SmallBlind;
            GameMove newGameMove = new GameMove(currentTurnID,MoveSequanceNo,smallBlindPlayerName,currentBalance,moveTime,move,moveAmount,totalRake,userRakeBack,parentRakeBack,profit);
            return newGameMove;
        }

        ///////////////////////////////////////////////////TurnSequance Section/////////////////////////////////////////////////////
        public List<Seat> GiveTurnToPlayer(List<Seat> seatList)
        {
            if (CurrentHasTurnlocation == SeatLocations.None)
            {
                seatList = initialzeHasTurn(seatList);
                return seatList;
            }
            seatList = MoveHasTurnToNextPlayer(seatList);
            return seatList;
        }

        private bool HasTurnPlayerIsFolded(List<Seat> seatList)
        {
            if (ISeatList.CheckFoldedPlayerHasTurnByIsMyTurn(seatList))
            {
                return true;
            }
            return false;
        }

        private bool HasTurnPlayerIsAllIn(List<Seat> seatList)
        {
            if (ISeatList.CheckAllInPlayerHasTurnByIsMyTurn(seatList))
            {
                return true;
            }
            return false;
        }

        private List<Seat> MoveHasTurnToNextPlayer(List<Seat> seatList)
        {
            int HasTurnLocation = (int)currentHasTurnlocation;
            seatList = ISeatList.SetSeatVarIsMyTurnByLocation(seatList,HasTurnLocation,false,this);

            if (isSubTurnStageEnded)
            {
                currentHasTurnlocation = SetDealerOrNextActiveSeatLocations();
            }
            HasTurnLocation = (int)currentHasTurnlocation;
            int whileLoopMaxCount = activeSeatLocations.Length + 1;
            int whileLoopCounter = 0;
            do
            {
                whileLoopCounter++;
                seatList = ISeatList.SetSeatVarIsMyTurnByLocation(seatList,HasTurnLocation,false,this);
                HasTurnLocation = ISeatList.ActiveSeatLocationsFindNextActiveLocation(HasTurnLocation, activeSeatLocations);
                seatList = ISeatList.SetSeatVarIsMyTurnByLocation(seatList, HasTurnLocation, true, this);
                if (whileLoopCounter > whileLoopMaxCount)
                {
                    Debug.LogError("TurnManager--->>MoveHasTurnToNextPlayer: While loop max counter a ulaştı");
                    break;
                }
            } while (HasTurnPlayerIsFolded(seatList) || HasTurnPlayerIsAllIn(seatList));
            currentHasTurnlocation = (SeatLocations) HasTurnLocation;
            currentHasTurnPlayer = ISeatList.GetPlayerNameByLocation(seatList,currentHasTurnlocation);
            return seatList;
        }

        private SeatLocations SetDealerOrNextActiveSeatLocations()
        {
            if (isDealerLocationInGame())
            {
                return currentDealerlocation;
            }else
            {
                return ISeatList.ActiveSeatLocationsFindPreviousActiveLocation((int) currentDealerlocation,activeSeatLocations);
            }
        }

        private bool isDealerLocationInGame()
        {
            foreach (var activeSeatLocation in activeSeatLocations)
            {
                if (activeSeatLocation == (int) currentDealerlocation)
                {
                    return true;
                }
            }
            return false;
        }

        private List<Seat> initialzeHasTurn(List<Seat> seatList)
        {
            int HasTurnLocation = ISeatList.ActiveSeatLocationsFindNextActiveLocation((int) currentBigBlindlocation,activeSeatLocations);
            seatList = ISeatList.SetSeatVarIsMyTurnByLocation(seatList,HasTurnLocation,true,this);
            return seatList;
        }
        
        public void SetCurrentHasTurnGOData(Seat dummySeat)
        {
            currentHasTurnlocation = dummySeat.location;
            currentHasTurnPlayer = dummySeat.username;
        }
        
        public bool SubTurnStageIsEnded()
        {
            return isSubTurnStageEnded;
        }
        
        

        ///////////////////////////////////////////////////ActiveSeatLocations Section/////////////////////////////////////////////////////
        

        public void SetActiveSeatLocationsArray(List<Seat> seatList)
        {
            int activePlayerCount = ISeatList.GetActivePlayerCount(seatList);
            int[] activeSeatLocations = new int[activePlayerCount];
            int index = 0;
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    activeSeatLocations[index] = (int) seat.location;
                    index++;
                }
            }
            
            activeSeatLocations = activeSeatLocations.OrderBy(x => x).ToArray();
            this.activeSeatLocations = activeSeatLocations;
        }
        ///////////////////////////////////////////////////TurnStage Section/////////////////////////////////////////////////////
        public void SetTurnStage(TurnStage turnStage)
        {
            myTurnStage = turnStage;
        }
        
        public bool CheckStageEnd(List<Seat> seatList)
        {
            List<string> inGamePlayerList = ISeatList.GenerateInGamePlayerList(seatList);
            if (inGamePlayerList.Count <= 1)
            {
                return true;
            }
            //son hamlesi allin veya fold olmayan oyuncuların listesi
            List<string> playerListWhoHasNotFoldedOrAllin = ISeatList.GeneratePlayerListWhoHasNotFoldedOrAllin(seatList);
            
            if (IGameMoveList.HasEveryPlayerMoved(subMoveList,playerListWhoHasNotFoldedOrAllin))
            {
                if (IGameMoveList.HasAllin(subMoveList))
                {
                    if (IGameMoveList.HasFold(subMoveList))
                    {
                        //allin var fold var
                        float maxTotalBetForAllinPlayers = IGameMoveList.CalculateMaxTotalBetAmongPlayersByMove(subMoveList,Move.AllIn);
                        List<GameMove> withOutFoldedPlayersMoves = IGameMoveList.RemoveGameMovesOfPlayersByMove(subMoveList,Move.Fold);
                        List<GameMove> withOutAllInAndFoldedPlayersMoves = IGameMoveList.RemoveGameMovesOfPlayersByMove(withOutFoldedPlayersMoves,Move.AllIn);
                        if (IGameMoveList.isTotalBetsEqual(withOutAllInAndFoldedPlayersMoves) && IGameMoveList.HasEveryPlayerEnoughBet(withOutAllInAndFoldedPlayersMoves,maxTotalBetForAllinPlayers))
                        {
                            return true;
                        }
                        return false;
                    }else
                    {
                        //allin var fold yok
                        float maxTotalBetForAllinPlayers = IGameMoveList.CalculateMaxTotalBetAmongPlayersByMove(subMoveList,Move.AllIn);
                        List<GameMove> withOutAllInPlayersMoves = IGameMoveList.RemoveGameMovesOfPlayersByMove(subMoveList,Move.AllIn);
                        if (IGameMoveList.isTotalBetsEqual(withOutAllInPlayersMoves) && IGameMoveList.HasEveryPlayerEnoughBet(withOutAllInPlayersMoves,maxTotalBetForAllinPlayers))
                        {
                            return true;
                        }

                        return false;
                    }
                }else
                {
                    //allin yok
                    if (IGameMoveList.HasFold(subMoveList))
                    {
                        //allin yok fold var
                        List<GameMove> withOutFoldedPlayersMoves = IGameMoveList.RemoveGameMovesOfPlayersByMove(subMoveList,Move.Fold);
                        if (IGameMoveList.isTotalBetsEqual(withOutFoldedPlayersMoves))
                        {
                            return true;
                        }
                        return false;

                    }else
                    {
                        //allin yok fold yok
                        if (IGameMoveList.isTotalBetsEqual(subMoveList))
                        {
                            return true;
                        }
                        return false;
                    }
                }

            }
            // Alt tur aşaması bitmemiştir
            return false;
        }

        
        
        private bool allPlayerHasMovedInSubTurnStage(List<string> inGamePlayerList, List<GameMove> subMoveListWithOutSmallAndBigBlind)
        {
            foreach (var playerName in inGamePlayerList)
            {
                if (!subMoveListWithOutSmallAndBigBlind.Exists(x => x.PlayerName == playerName))
                {
                    return false;
                }
            }

            return true;
        }
        
        public bool IsTurnEnded(List<Seat> seatList)
        {
            List<string> inGamePlayerList = ISeatList.GenerateInGamePlayerList(seatList);
            if(inGamePlayerList.Count <= 1)
            {
                turnEndType = TurnEndType.AllPlayersFolded;
                return true;
            }
            if (myTurnStage == TurnStage.PreFlop)
            {
                return false;
            }
            if (myTurnStage == TurnStage.PreTurn)
            {
                return false;
            }
            if (myTurnStage == TurnStage.PreRiver)
            {
                return false;
            }
            turnEndType = TurnEndType.Clash;
            return true;
        }
        
        public void TurnStageEnd()
        {
            //Go Next TurnStage
            myTurnStage = myTurnStage + 1;
            isSubTurnStageEnded = true;
        }
        
        ///////////////////////////////////////////////////Dealer Section/////////////////////////////////////////////////////
        public List<Seat> PrepareDealerToken(List<Seat> seatList)
        {
            if (CurrentDealerlocation == SeatLocations.None)
            {
                seatList =initializeDealer(seatList);
            }
            else
            {
                seatList = MoveDealerToNextPlayer(seatList,CurrentDealerlocation,this);
            }

            return seatList;
        }
        public List<Seat> MoveDealerToNextPlayer(List<Seat> seatList, SeatLocations currentDealerLocation,
            TurnManager turnManager)
        {
            int dealerLocation = (int) currentDealerLocation;
            seatList = ISeatList.SetSeatVarIsDealerByLocation(seatList, dealerLocation, false,turnManager);
            int nextDealerLocation =ISeatList.ActiveSeatLocationsFindNextActiveLocation(dealerLocation,turnManager.ActiveSeatLocations);
            seatList = ISeatList.SetSeatVarIsDealerByLocation(seatList, nextDealerLocation, true,turnManager);
            return seatList;
        }

        private List<Seat> initializeDealer(List<Seat> seatList)
        {
            int minLocation = ISeatList.ActiveSeatLocationsFindMin(activeSeatLocations);
            seatList = ISeatList.SetSeatVarIsDealerByLocation(seatList, minLocation, true,this);
            return seatList;
        }
        public void SetCurrentDealerGOData(Seat seat)
        {
            currentDealerlocation = seat.location;
            currentDealerPlayer = seat.username;
        }
        ///////////////////////////////////////////////////SmallBlindAndBigBlind Section/////////////////////////////////////////////////////
        public void PrepareSmallAndBigBlindGODataByDealer(List<Seat> mySeatList)
        {
            SetSmallBlindLocation(mySeatList);
            SetBigBlindLocation(mySeatList);
        }

        private void SetBigBlindLocation(List<Seat> seatList)
        {
            int bigBlindLocation = ISeatList.ActiveSeatLocationsFindNextActiveLocation((int) currentSmallBlindlocation,activeSeatLocations);
            SetBigBlindGOData(seatList, bigBlindLocation);
        }

        private void SetBigBlindGOData(List<Seat> seatList, int bigBlindLocation)
        {
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    if (seat.location == (SeatLocations) bigBlindLocation)
                    {
                        currentBigBlindlocation = seat.location;
                        currentBigBlindPlayer = seat.username;
                        if (string.IsNullOrEmpty(seat.username))
                        {
                            Debug.LogError("TurnManager -->>> SetBigBlindGoData -->>> seat.username is Empty");
                        }
                        if (seat.location == SeatLocations.None)
                        {
                            Debug.LogError("TurnManager -->>> SetBigBlindGoData -->>> seat.location is None");
                        }
                    }
                }

                if (seat.location == (SeatLocations) bigBlindLocation && !seat.isActive)
                {
                    Debug.LogError("TurnManager -->>> SetBigBlindGoData -->>> seat.isActive is False");
                }
            }
        }

        private void SetSmallBlindLocation(List<Seat> seatList)
        {
            int smallBlindLocation = ISeatList.ActiveSeatLocationsFindNextActiveLocation((int) currentDealerlocation, activeSeatLocations);
            SetSmallBlindGOData(smallBlindLocation, seatList);
        }

        private void SetSmallBlindGOData(int smallBlindLocation, List<Seat> seatList)
        {
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    if (seat.location == (SeatLocations) smallBlindLocation)
                    {
                        currentSmallBlindPlayer = seat.username;
                        currentSmallBlindlocation = seat.location;
                        if (string.IsNullOrEmpty(seat.username))
                        {
                            Debug.LogError("TurnManager -->>> SetSmallBlindGoData -->>> seat.username is Empty");
                        }
                        if (seat.location == SeatLocations.None)
                        {
                            Debug.LogError("TurnManager -->>> SetSmallBlindGoData -->>> seat.location is None");
                        }
                    }
                }
                if (seat.location == (SeatLocations) smallBlindLocation && !seat.isActive)
                {
                    Debug.LogError("TurnManager -->>> SetSmallBlindGoData -->>> seat.isActive is false");
                }
            }
        }
        ///////////////////////////////////////////////////Properties Section/////////////////////////////////////////////////////

        public TurnStage MyTurnStage
        {
            get => myTurnStage;
            set => myTurnStage = value;
        }
        public int TurnTime
        {
            get => turnTime;
            set => turnTime = value;
        }

        public bool IsSubTurnStageEnded
        {
            get => isSubTurnStageEnded;
            set => isSubTurnStageEnded = value;
        }
        public int TotalMoveTime
        {
            get => totalMoveTime;
            set => totalMoveTime = value;
        }

        public List<GameMove> MoveList => moveList;

        public int MoveSequanceNo
        {
            get => moveSequanceNo;
            set => moveSequanceNo = value;
        }

        public SeatLocations CurrentHasTurnlocation
        {
            get => currentHasTurnlocation;
            set => currentHasTurnlocation = value;
        }

        public string CurrentHasTurnPlayer
        {
            get => currentHasTurnPlayer;
            set => currentHasTurnPlayer = value;
        }

        public SeatLocations CurrentBigBlindlocation
        {
            get => currentBigBlindlocation;
            set => currentBigBlindlocation = value;
        }

        public string CurrentBigBlindPlayer
        {
            get => currentBigBlindPlayer;
            set => currentBigBlindPlayer = value;
        }

        public SeatLocations CurrentSmallBlindlocation
        {
            get => currentSmallBlindlocation;
            set => currentSmallBlindlocation = value;
        }

        public string CurrentSmallBlindPlayer
        {
            get => currentSmallBlindPlayer;
            set => currentSmallBlindPlayer = value;
        }

        public int[] ActiveSeatLocations
        {
            get => activeSeatLocations;
            set => activeSeatLocations = value;
        }

        public SeatLocations CurrentDealerlocation
        {
            get => currentDealerlocation;
            set => currentDealerlocation = value;
        }

        public string CurrentDealerPlayer
        {
            get => currentDealerPlayer;
            set => currentDealerPlayer = value;
        }

        public string SyncID
        {
            get => syncID;
            set => syncID = value;
        }

        public void StartNewSubTurn(bool value)
        {
            isSubTurnStageEnded = !value;
        }

        public void ClearSubMoveList()
        {
            subMoveList.Clear();
        }


        public void SetReadyForTurnEnded(bool value)
        {
            ReadyForTurnEnded = value;
        }

        public void SetWinners(List<Tuple<string, List<CardData>>> winnersAndCombinations)
        {
            winnersAndCombinationsList = winnersAndCombinations;
        }

        public void TurnStartTime(DateTime turnStartDate)
        {
            this.turnStartDate = turnStartDate;
        }

        public DateTime GetTurnStartDate()
        {
            return turnStartDate;
        }

        public float GetTotalPot()
        {
            return CalculateTotalPot();
        }

        private float CalculateTotalPot()
        {
            float totalPot = 0;
            foreach (var gameMove in moveList)
            {
                totalPot += gameMove.Amount;
            }
            return totalPot;
        }

        public float GetTotalRakeBack()
        {
            return CalculateTotalRakeBack();
        }

        private float CalculateTotalRakeBack()
        {
            float totalRake = 0;
            foreach (var gameMove in moveList)
            {
                totalRake += gameMove.UserRakeBack + gameMove.ParentRakeBack;
            }
            return totalRake;
        }

        public float GetTotalProfit()
        {
            return CalculateTotalProfit();
        }

        private float CalculateTotalProfit()
        {
            float totalProfit = 0;
            foreach (var gameMove in moveList)
            {
                totalProfit += gameMove.Profit;
            }
            return totalProfit;
        }

        public List<GameMove> GetMoveList()
        {
            return moveList;
        }

        public void DecideTurnEndType(List<Seat> seatList)
        {
            throw new NotImplementedException();
        }

        public TurnEndType GetTurnEndType()
        {
            return turnEndType;
        }

        public List<Tuple<string, List<CardData>>> GetWinnerAndCombinations()
        {
            return winnersAndCombinationsList;
        }

        public List<string> GetWinnerListOrderByInGamePlayerList(List<string> inGamePlayerList, List<Tuple<string, List<CardData>>> winnerAndCombinations)
        {
            List<string> winnerList = new List<string>();
            foreach (var inGamePlayer in inGamePlayerList)
            {
                foreach (var winnerAndCombination in winnerAndCombinations)
                {
                    if (inGamePlayer == winnerAndCombination.Item1)
                    {
                        winnerList.Add(inGamePlayer);
                    }
                }
            }
            return winnerList;
        }

        public List<List<CardData>> GetWinnerCombinationsOrderByInGamePlayerList(List<string> inGamePlayerList, List<Tuple<string, List<CardData>>> winnerAndCombinations)
        {
            List<List<CardData>> winnerCombinations = new List<List<CardData>>();
            foreach (var inGamePlayer in inGamePlayerList)
            {
                foreach (var winnerAndCombination in winnerAndCombinations)
                {
                    if (inGamePlayer == winnerAndCombination.Item1)
                    {
                        winnerCombinations.Add(winnerAndCombination.Item2);
                    }
                }
            }
            return winnerCombinations;
        }

        public bool isReadyForTurnEnd()
        {
            return ReadyForTurnEnded;
        }

        public float GetWinAmount(float tableRakePercent)
        {
            return CalculateWinAmount(tableRakePercent);
        }

        private float CalculateWinAmount(float tableRakePercent)
        {
            float totalPot = CalculateTotalPot();
            return totalPot - (totalPot * tableRakePercent/100);
        }

        public void GiveTurnToWinner(string winnerName, List<Seat> seatList)
        {
            currentHasTurnlocation = (SeatLocations) ISeatList.GetLocationByIsMyTurn(seatList);
            currentHasTurnPlayer = winnerName;
        }

        

        private List<string> GetWinnerList(List<Tuple<string, List<CardData>>> winnersAndCombinations)
        {
            List<string> winnerList = new List<string>();
            foreach (var winnerAndCombination in winnersAndCombinations)
            {
                winnerList.Add(winnerAndCombination.Item1);
            }
            return winnerList;
        }

        public TurnWinners GetTurnWinners()
        {
            return turnWinners;
        }

        public string GetWinnerName()
        {
            return turnWinners.Winner1;
        }

        

        public GameMove GetCorrectionMove()
        {
            return correctionReturnMove;
        }

        public void CreateCorrectionGameMove(float correctionMoveAmount, float totalRake, float userRakeBack,
            float parentRakeBack, float profit)
        {
            GameMove correctionMove = new GameMove
            {
                TurnId = correctionReturnMove.TurnId,
                MoveSequenceNo = correctionReturnMove.MoveSequenceNo,
                PlayerName = correctionReturnMove.PlayerName,
                CurrentBalance = correctionReturnMove.CurrentBalance,
                Move = correctionReturnMove.Move,
                Amount = correctionMoveAmount,
                Profit = profit,
                UserRakeBack = userRakeBack,
                ParentRakeBack = parentRakeBack,
                TotalRake = totalRake,
                MoveTime = correctionReturnMove.MoveTime,
                id = correctionReturnMove.id,
            };
            List<GameMove> tempMoveList = new List<GameMove>();
            foreach (var gameMove in moveList)
            {
                if (correctionReturnMove.MoveSequenceNo != gameMove.MoveSequenceNo)
                {
                    tempMoveList.Add(gameMove);
                }
            }
            tempMoveList.Add(correctionMove);
            moveList = tempMoveList;
            
            List<GameMove> temSubMoveList = new List<GameMove>();
            foreach (var gameMove in subMoveList)
            {
                if (correctionReturnMove.MoveSequenceNo != gameMove.MoveSequenceNo)
                {
                    temSubMoveList.Add(gameMove);
                }
            }
            temSubMoveList.Add(correctionMove);
            subMoveList = temSubMoveList;
            
        }

        public float GetCorrectionMoveAmount()
        {
            return correctionReturnAmount;
        }

        public string GetCorrectionMovePlayerName()
        {
            return correctionReturnMove.PlayerName;
        }

        public string CreateCorrectionMove()
        {
            string correctionPlayerName = correctionReturnMove.PlayerName;
            float correctionPlayerTotalBet = IGameMoveList.GetTotalBetByPlayerName(subMoveList, correctionPlayerName);
            List<GameMove> withOutCorrectionPlayerMoves = IGameMoveList.RemoveGameMovesOfPlayersByPlayerNames(subMoveList, correctionPlayerName);
            float maxTotalBet = IGameMoveList.CalculateMaxTotalBet(withOutCorrectionPlayerMoves);
            correctionReturnAmount = correctionPlayerTotalBet - maxTotalBet;
            correctionBetValueForClientSide = correctionPlayerTotalBet - correctionReturnAmount;
            return correctionPlayerName;
        }

        public float CalculateCorrectionMoveAmount()
        {
            return correctionReturnMove.Amount - correctionReturnAmount;
        }

        public float GetCorrectionBetAmountForClientSide()
        {
            return correctionBetValueForClientSide;
        }

        public void SetTurnStageForAnimation(TurnStage preShowDown)
        {
            myTurnStage = preShowDown;
        }


        public TurnStage GetTurnStage()
        {
            return myTurnStage;
        }

        public float GetCorrectionReturnAmount()
        {
            return correctionReturnAmount;
        }
        
        public int TotalMoveTimeForEndGame
        {
            get => totalMoveTimeForEndGame;
            set => totalMoveTimeForEndGame = value;
        }

        public bool IsPlayerTurnEnd
        {
            get => isPlayerTurnEnd;
            set => isPlayerTurnEnd = value;
        }

        public bool IsPlayerTimeOut
        {
            get => isPlayerTimeOut;
            set => isPlayerTimeOut = value;
        }


        public void ClearTurnManagerGOData()
        {
            activeSeatLocations = null;
            moveTimeCalculate = false;
            winnersAndCombinationsList.Clear();
            turnWinners = null;
            currentHasTurnlocation = SeatLocations.None;
            currentHasTurnPlayer = string.Empty;
            turnTime = 0;
            isPlayerTurnEnd = false;
            isPlayerTimeOut = false;
            currentBigBlindlocation = SeatLocations.None;
            currentBigBlindPlayer = string.Empty;
            currentSmallBlindlocation = SeatLocations.None;
            currentSmallBlindPlayer = string.Empty;
            currentDealerlocation = SeatLocations.None;
            currentDealerPlayer = string.Empty;
            moveList.Clear();
            subMoveList.Clear();
            moveSequanceNo = 0;
            correctionReturnMove = null;
            correctionReturnAmount = 0;
            correctionBetValueForClientSide = 0;

            myTurnStage = TurnStage.None;
            isSubTurnStageEnded = false;
            ReadyForTurnEnded = false;
            turnEndType = TurnEndType.None;
            isEndGame = false;
            
        }

        public int GetTurnTime()
        {
            return totalMoveTime;
        }

        public int GetTotalMoveTimeForEndGame()
        {
            return totalMoveTimeForEndGame;
        }

        public bool GetReadyForTurnEnd()
        {
            return ReadyForTurnEnded;
        }


        public void SetIsEndGame(bool isEndGame)
        {
            this.isEndGame = isEndGame;
        }


        public void SetPlayerTimeOut(bool value)
        {
            isPlayerTimeOut = value;
        }

        public void SetMoveTimeCalculate(bool value)
        {
            moveTimeCalculate = value;
        }

        public float GetPlayerSubMaxBet(string playerName)
        {
            return IGameMoveList.GetTotalBetByPlayerName( subMoveList, playerName);
        }

        public bool IsSubTurnStageCauseSideBet
        {
            get => isSubTurnStageCauseSideBet;
            set => isSubTurnStageCauseSideBet = value;
        }


        
        

        

        public List<Tuple<List<string>, float>> CalculateSubTurnSideBetOwnersAndAmountTupleList()
        {
            List<Tuple<List<string>,float>> sideBetPossibleOwnerTupleList = IGameMoveList.GenerateSideBetPossibleOwnersAndAmountsTupleList(subMoveList,moveList);
            return sideBetPossibleOwnerTupleList;
        }

        public void SetTurnWinners(TurnWinners turnWinners)
        {
            this.turnWinners = turnWinners;
        }

        public List<float> GetWinnerPotListOrderByInGamePlayerList(List<string> winnerList)
        {
            return GenerateWinAmountListOrderByInGamePlayerList(winnerList);
        }

        private List<float> GenerateWinAmountListOrderByInGamePlayerList(List<string> winnerList)
        {
            List<float> winAmountList = new List<float>();
            foreach (var playerName in winnerList)
            {
                float amount = turnWinners.GetWinAmountByPlayerName(playerName);
                winAmountList.Add(amount);
            }
            return winAmountList;
        }
    }
    
    public enum TurnStage
    {
        PreFlop = 2,
        PreTurn = 3,
        PreRiver = 4,
        PreShowDown = 5,
        PreEnd = 6,
        PreStart = 1,
        None = 0
    }

    public enum TurnEndType
    {
        None = 0,
        Clash = 1,
        AllPlayersFolded = 2
    }
}