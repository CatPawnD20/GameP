using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts
{
    public class OmahaEngine : MonoBehaviour
    {
        ///////////////////////////////////////////////////Variables Section/////////////////////////////////////////////////////
        public static OmahaEngine instance;
        [Header("TableDetails")]
        [SerializeField] private string syncID;
        [SerializeField] private float minDeposit;
        [SerializeField] private float smallBlind;
        [SerializeField] private float tableRakePercent;
        [Header("PotDetails")]
        [SerializeField] private float subMaxBet;
        [SerializeField] private float maxRaiseLimit;
        [SerializeField] private float currentTotalPot;
        
        [Header("AnimationDetails")]
        [SerializeField] private bool animationTillEnd;
        [SerializeField] private TurnStage animationStartTurnStage;
        Dealer myDealerScript = null;
        TurnManager myTurnManagerScript = null;
        ChipManager myChipManagerScript = null;
        
        
        ///////////////////////////////////////////////////Events Section/////////////////////////////////////////////////////
        void Start()
        {
            instance = this;
        }
        ///////////////////////////////////////////////////InitializeTurn Section/////////////////////////////////////////////////////
        /*
         * Bu iki Map TurnPlayers'in oluşturulabilmesi için gerekli.
         * PlayerAndChipMap
         * SeatLocationAndChipMap
         * ***********************************************
         */
        private void InitializeTurn(List<Seat> seatList)
        {
            InitializeActiveSeatLocationsArray(seatList);
            InitializePlayerAndChipMap(seatList);
            InitializeSeatLocationAndChipMap(seatList);
            
        }

        private void InitializeSeatLocationAndChipMap(List<Seat> seatList)
        {
            myChipManagerScript.InitializeSeatLocationAndChipMap(seatList);
        }

        private void InitializePlayerAndChipMap(List<Seat> seatList)
        {
            myChipManagerScript.InitializePlayerAndChipMap(seatList);
        }

        private void InitializeActiveSeatLocationsArray(List<Seat> seatList)
        {
            myTurnManagerScript.ActiveSeatLocationsArrayInitialize(seatList);
        }

        
        ///////////////////////////////////////////////////GameMoveCreateNew Section/////////////////////////////////////////////////////
        public GameMove GameMoveCreateNew(List<Seat> seatList, string playerName, int currentTurnID, Move lastMove,
            float lastMoveAmount)
        {
            switch (lastMove)
            {
                case Move.Call:
                    return GameMoveCreateNewForMoveCall(seatList, playerName, currentTurnID, lastMove, lastMoveAmount);
                case Move.Check:
                    return GameMoveCreateNewForMoveCheck(seatList, playerName, currentTurnID, lastMove, lastMoveAmount);
                case Move.Raise:
                    return GameMoveCreateNewForMoveRaise(seatList, playerName, currentTurnID, lastMove, lastMoveAmount);
                case Move.AllIn:
                    return GameMoveCreateNewForMoveAllIn(seatList, playerName, currentTurnID, lastMove, lastMoveAmount);
                case Move.Fold:
                    return GameMoveCreateNewForMoveFold(seatList, playerName, currentTurnID, lastMove, lastMoveAmount);
                case Move.HideCards:
                    return GameMoveCreateNewForMoveHideCards(seatList, playerName, currentTurnID, lastMove, lastMoveAmount);
                case Move.ShowCards:
                    return GameMoveCreateNewForMoveShowCards(seatList, playerName, currentTurnID, lastMove, lastMoveAmount);
            }
            return null;
        }

        private GameMove GameMoveCreateNewForMoveShowCards(List<Seat> seatList, string playerName, int currentTurnID, Move lastMove, float lastMoveAmount)
        {
            float currentBalance = ISeatList.GetBalanceByPlayerName(seatList, playerName);
            float totalRake = myChipManagerScript.CalculateTotalRake(lastMoveAmount, tableRakePercent);
            float userRakeBack = myChipManagerScript.CalculateUserRakeBack(playerName,totalRake);
            float parentRakeBack = myChipManagerScript.CalculateParentRakeBack(playerName,userRakeBack);
            float profit = myChipManagerScript.CalculateProfit(totalRake, userRakeBack, parentRakeBack);
            GameMove gameMove = myTurnManagerScript.CreateNewGameMove(seatList,currentTurnID,currentBalance,lastMove,lastMoveAmount,totalRake,userRakeBack,parentRakeBack,profit);
            return gameMove;
        }

        private GameMove GameMoveCreateNewForMoveHideCards(List<Seat> seatList, string playerName, int currentTurnID, Move lastMove, float lastMoveAmount)
        {
            float currentBalance = ISeatList.GetBalanceByPlayerName(seatList, playerName);
            float totalRake = myChipManagerScript.CalculateTotalRake(lastMoveAmount, tableRakePercent);
            float userRakeBack = myChipManagerScript.CalculateUserRakeBack(playerName,totalRake);
            float parentRakeBack = myChipManagerScript.CalculateParentRakeBack(playerName,userRakeBack);
            float profit = myChipManagerScript.CalculateProfit(totalRake, userRakeBack, parentRakeBack);
            GameMove gameMove = myTurnManagerScript.CreateNewGameMove(seatList,currentTurnID,currentBalance,lastMove,lastMoveAmount,totalRake,userRakeBack,parentRakeBack,profit);
            return gameMove;
        }

        private GameMove GameMoveCreateNewForMoveFold(List<Seat> seatList, string playerName, int currentTurnID, Move lastMove, float lastMoveAmount)
        {
            
            float currentBalance = ISeatList.GetBalanceByPlayerName(seatList, playerName);
            float totalRake = myChipManagerScript.CalculateTotalRake(lastMoveAmount, tableRakePercent);
            float userRakeBack = myChipManagerScript.CalculateUserRakeBack(playerName,totalRake);
            float parentRakeBack = myChipManagerScript.CalculateParentRakeBack(playerName,userRakeBack);
            float profit = myChipManagerScript.CalculateProfit(totalRake, userRakeBack, parentRakeBack);
            GameMove gameMove = myTurnManagerScript.CreateNewGameMove(seatList,currentTurnID,currentBalance,lastMove,lastMoveAmount,totalRake,userRakeBack,parentRakeBack,profit);
            SetPotDetailsForMoveFold(lastMoveAmount,seatList,playerName);
            return gameMove;
        }

        private void SetPotDetailsForMoveFold(float lastMoveAmount, List<Seat> seatList, string playerName)
        {
            //subMaxBet = smallBlind * 2;
            currentTotalPot = currentTotalPot + lastMoveAmount;
            
            
        }

        private GameMove GameMoveCreateNewForMoveAllIn(List<Seat> seatList, string playerName, int currentTurnID, Move lastMove, float lastMoveAmount)
        {
            
            float currentBalance = ISeatList.GetBalanceByPlayerName(seatList, playerName);
            float totalRake = myChipManagerScript.CalculateTotalRake(lastMoveAmount, tableRakePercent);
            float userRakeBack = myChipManagerScript.CalculateUserRakeBack(playerName,totalRake);
            float parentRakeBack = myChipManagerScript.CalculateParentRakeBack(playerName,userRakeBack);
            float profit = myChipManagerScript.CalculateProfit(totalRake, userRakeBack, parentRakeBack);
            GameMove gameMove = myTurnManagerScript.CreateNewGameMove(seatList,currentTurnID,currentBalance,lastMove,lastMoveAmount,totalRake,userRakeBack,parentRakeBack,profit);
            SetPotDetailsForMoveAllin(lastMoveAmount,seatList,playerName);
            return gameMove;
        }

        private void SetPotDetailsForMoveAllin(float lastMoveAmount, List<Seat> seatList, string myName)
        {
            float mySubMaxBet = ISeatList.GetTotalBetInSubTurnByPlayerName(seatList, myName);
            float subMaxBet = myChipManagerScript.SubMaxBet;
            if (subMaxBet < mySubMaxBet)
            {
                myChipManagerScript.SetSubMaxBet(mySubMaxBet);
            }
            currentTotalPot = currentTotalPot + lastMoveAmount;
            
            
            
        }

        private GameMove GameMoveCreateNewForMoveRaise(List<Seat> seatList, string playerName, int currentTurnID, Move lastMove, float lastMoveAmount)
        {
            float currentBalance = ISeatList.GetBalanceByPlayerName(seatList, playerName);
            float totalRake = myChipManagerScript.CalculateTotalRake(lastMoveAmount, tableRakePercent);
            float userRakeBack = myChipManagerScript.CalculateUserRakeBack(playerName,totalRake);
            float parentRakeBack = myChipManagerScript.CalculateParentRakeBack(playerName,userRakeBack);
            float profit = myChipManagerScript.CalculateProfit(totalRake, userRakeBack, parentRakeBack);
            GameMove gameMove = myTurnManagerScript.CreateNewGameMove(seatList,currentTurnID,currentBalance,lastMove,lastMoveAmount,totalRake,userRakeBack,parentRakeBack,profit);
            
            //myChipManagerScript.SetSubMaxBet(smallBlind*2);
            
            SetPotDetailsForMoveRaise(lastMoveAmount,seatList,playerName);
            return gameMove;
        }

        private void SetPotDetailsForMoveRaise(float lastMoveAmount, List<Seat> seatList, string myName)
        {
            
            float subMaxBet = 0;
            subMaxBet = ISeatList.GetTotalBetInSubTurnByPlayerName(seatList, myName);
            myChipManagerScript.SetSubMaxBet(subMaxBet);
            currentTotalPot = currentTotalPot + lastMoveAmount;
            
        }

        private GameMove GameMoveCreateNewForMoveCheck(List<Seat> seatList, string playerName, int currentTurnID, Move lastMove, float lastMoveAmount)
        {
            float currentBalance = ISeatList.GetBalanceByPlayerName(seatList, playerName);
            float totalRake = myChipManagerScript.CalculateTotalRake(lastMoveAmount, tableRakePercent);
            float userRakeBack = myChipManagerScript.CalculateUserRakeBack(playerName,totalRake);
            float parentRakeBack = myChipManagerScript.CalculateParentRakeBack(playerName,userRakeBack);
            float profit = myChipManagerScript.CalculateProfit(totalRake, userRakeBack, parentRakeBack);
            GameMove gameMove = myTurnManagerScript.CreateNewGameMove(seatList,currentTurnID,currentBalance,lastMove,lastMoveAmount,totalRake,userRakeBack,parentRakeBack,profit);
            
            //myChipManagerScript.SetSubMaxBet(smallBlind*2);
            
            SetPotDetailsForMoveCheck(lastMoveAmount);
            return gameMove;
        }

        private void SetPotDetailsForMoveCheck(float lastMoveAmount)
        {
            //subMaxBet = smallBlind * 2;
            //currentMiddlePot = currentMiddlePot;
            //currentTotalPot = currentTotalPot + lastMoveAmount;
            //maxRaiseLimit = currentTotalPot + lastMoveAmount * 2 + currentTotalPot;
        }

        private GameMove GameMoveCreateNewForMoveCall(List<Seat> seatList, string playerName, int currentTurnID, Move lastMove, float lastMoveAmount)
        {
            float currentBalance = ISeatList.GetBalanceByPlayerName(seatList, playerName);
            float totalRake = myChipManagerScript.CalculateTotalRake(lastMoveAmount, tableRakePercent);
            float userRakeBack = myChipManagerScript.CalculateUserRakeBack(playerName,totalRake);
            float parentRakeBack = myChipManagerScript.CalculateParentRakeBack(playerName,userRakeBack);
            float profit = myChipManagerScript.CalculateProfit(totalRake, userRakeBack, parentRakeBack);
            GameMove gameMove = myTurnManagerScript.CreateNewGameMove(seatList,currentTurnID,currentBalance,lastMove,lastMoveAmount,totalRake,userRakeBack,parentRakeBack,profit);
            
            //myChipManagerScript.SetSubMaxBet(smallBlind*2);
            
            
            
            SetPotDetailsForMoveCall(lastMoveAmount,seatList,playerName);
            return gameMove;
        }

        private void SetPotDetailsForMoveCall(float lastMoveAmount, List<Seat> seatList, string playerName)
        {
            //subMaxBet = smallBlind * 2;
            currentTotalPot = currentTotalPot + lastMoveAmount;
            
            
        }

        public bool MoveCheckMoveIsLegit(List<Seat> seatList, Move nextMove, float moveAmount, string playerName)
        {
            bool playerMoveLegit =  false;
            switch (nextMove)
            {
                case Move.Call:
                    playerMoveLegit = MoveIsCallLegit(seatList, nextMove, moveAmount, playerName);
                    break;
                case Move.Check:
                    playerMoveLegit = MoveIsCheckLegit(seatList, nextMove, moveAmount, playerName);
                    break;
                case Move.Raise:
                    playerMoveLegit = MoveIsRaiseLegit(seatList, nextMove, moveAmount, playerName);
                    break;
                case Move.AllIn:
                    playerMoveLegit = MoveIsAllInLegit(seatList, nextMove, moveAmount, playerName);
                    break;
                case Move.Fold:
                    playerMoveLegit = MoveIsFoldLegit(seatList, nextMove, moveAmount, playerName);
                    break;
                case Move.ShowCards:
                    playerMoveLegit = MoveIsShowCardsLegit(seatList, nextMove, moveAmount, playerName);
                    break;
                case Move.HideCards:
                    playerMoveLegit = MoveIsHideCardsLegit(seatList, nextMove, moveAmount, playerName);
                    break;
            }
            return playerMoveLegit;
        }

        private bool MoveIsHideCardsLegit(List<Seat> seatList, Move nextMove, float moveAmount, string playerName)
        {
            bool isPlayerTimeOut = myTurnManagerScript.IsPlayerTimeOut;
            bool isPlayerTurn = ISeatList.CheckIsMyTurnByPlayerName(seatList, playerName,nextMove);
            bool isTurnManagerSaysThisPlayerHasTurn = myTurnManagerScript.CurrentHasTurnPlayerCheckByPlayerName(playerName);
            bool isNotFolded = ISeatList.CheckLastMoveIsNotFoldByPlayerName(seatList, playerName);
            bool isPlayerWinner = ISeatList.CheckPlayerIsWinnerByPlayerName(seatList, playerName);
            if (isPlayerTurn && isTurnManagerSaysThisPlayerHasTurn  && isNotFolded && isPlayerWinner && !isPlayerTimeOut)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        private bool MoveIsShowCardsLegit(List<Seat> seatList, Move nextMove, float moveAmount, string playerName)
        {
            bool isPlayerTimeOut = myTurnManagerScript.IsPlayerTimeOut;
            bool isPlayerTurn = ISeatList.CheckIsMyTurnByPlayerName(seatList, playerName,nextMove);
            bool isTurnManagerSaysThisPlayerHasTurn = myTurnManagerScript.CurrentHasTurnPlayerCheckByPlayerName(playerName);
            bool isNotFolded = ISeatList.CheckLastMoveIsNotFoldByPlayerName(seatList, playerName);
            bool isPlayerWinner = ISeatList.CheckPlayerIsWinnerByPlayerName(seatList, playerName);
            if (isPlayerTurn && isTurnManagerSaysThisPlayerHasTurn  && isNotFolded && isPlayerWinner && !isPlayerTimeOut)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        private bool MoveIsFoldLegit(List<Seat> seatList, Move myNextMove, float moveAmount, string playerName)
        {
            bool isPlayerTimeOut = myTurnManagerScript.IsPlayerTimeOut;
            bool isPlayerTurn = ISeatList.CheckIsMyTurnByPlayerName(seatList, playerName,myNextMove);
            bool isTurnManagerSaysThisPlayerHasTurn = myTurnManagerScript.CurrentHasTurnPlayerCheckByPlayerName(playerName);
            bool isNotFolded = ISeatList.CheckLastMoveIsNotFoldByPlayerName(seatList, playerName);
            if (isPlayerTurn && isTurnManagerSaysThisPlayerHasTurn && isNotFolded && !isPlayerTimeOut)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool MoveIsAllInLegit(List<Seat> seatList, Move myNextMove, float moveAmount, string playerName)
        {
            float subMaxBet = myChipManagerScript.SubMaxBet;
            bool isPlayerTimeOut = myTurnManagerScript.IsPlayerTimeOut;
            bool isPlayerTurn = ISeatList.CheckIsMyTurnByPlayerName(seatList, playerName,myNextMove);
            bool isTurnManagerSaysThisPlayerHasTurn = myTurnManagerScript.CurrentHasTurnPlayerCheckByPlayerName(playerName);
            bool hasPlayerEnoughBalance = ISeatList.CheckPlayerHasEnoughBalance(seatList, playerName, moveAmount);
            float currentBalance = ISeatList.GetBalanceByPlayerName(seatList, playerName);
            bool isNotFolded = ISeatList.CheckLastMoveIsNotFoldByPlayerName(seatList, playerName);
            bool isMoveAmountValid = myTurnManagerScript.MoveControlMoveAmountIsValid(playerName,myNextMove,moveAmount,subMaxBet,maxRaiseLimit,currentTotalPot,currentBalance);
            if (isPlayerTurn && isTurnManagerSaysThisPlayerHasTurn && hasPlayerEnoughBalance && isNotFolded && isMoveAmountValid && !isPlayerTimeOut)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        private bool MoveIsRaiseLegit(List<Seat> seatList, Move myNextMove, float moveAmount, string playerName)
        {
            float subMaxBet = myChipManagerScript.SubMaxBet;
            bool isPlayerTimeOut = myTurnManagerScript.IsPlayerTimeOut;
            bool isPlayerTurn = ISeatList.CheckIsMyTurnByPlayerName(seatList, playerName,myNextMove);
            bool isTurnManagerSaysThisPlayerHasTurn = myTurnManagerScript.CurrentHasTurnPlayerCheckByPlayerName(playerName);
            bool hasPlayerEnoughBalance = ISeatList.CheckPlayerHasEnoughBalance(seatList, playerName, moveAmount);
            float currentBalance = ISeatList.GetBalanceByPlayerName(seatList, playerName);
            bool isNotFolded = ISeatList.CheckLastMoveIsNotFoldByPlayerName(seatList, playerName);
            bool isMoveAmountValid = myTurnManagerScript.MoveControlMoveAmountIsValid(playerName,myNextMove,moveAmount,subMaxBet,maxRaiseLimit,currentTotalPot,currentBalance);
            if (isPlayerTurn && isTurnManagerSaysThisPlayerHasTurn && hasPlayerEnoughBalance && isNotFolded && isMoveAmountValid && !isPlayerTimeOut)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        private bool MoveIsCheckLegit(List<Seat> seatList, Move myNextMove, float moveAmount, string playerName)
        {
            float subMaxBet = myChipManagerScript.SubMaxBet;
            bool isPlayerTimeOut = myTurnManagerScript.IsPlayerTimeOut;
            bool isPlayerTurn = ISeatList.CheckIsMyTurnByPlayerName(seatList, playerName,myNextMove);
            bool isTurnManagerSaysThisPlayerHasTurn = myTurnManagerScript.CurrentHasTurnPlayerCheckByPlayerName(playerName);
            bool hasPlayerEnoughBalance = ISeatList.CheckPlayerHasEnoughBalance(seatList, playerName, moveAmount);
            float currentBalance = ISeatList.GetBalanceByPlayerName(seatList, playerName);
            bool isNotFolded = ISeatList.CheckLastMoveIsNotFoldByPlayerName(seatList, playerName);
            bool isMoveAmountValid = myTurnManagerScript.MoveControlMoveAmountIsValid(playerName,myNextMove,moveAmount,subMaxBet,maxRaiseLimit,currentTotalPot,currentBalance);
            if (isPlayerTurn && isTurnManagerSaysThisPlayerHasTurn && hasPlayerEnoughBalance && isNotFolded && isMoveAmountValid && !isPlayerTimeOut)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool MoveIsCallLegit(List<Seat> seatList, Move myNextMove, float moveAmount, string playerName)
        {
            float subMaxBet = myChipManagerScript.SubMaxBet;
            bool isPlayerTimeOut = myTurnManagerScript.IsPlayerTimeOut;
            bool isPlayerTurn = ISeatList.CheckIsMyTurnByPlayerName(seatList, playerName,myNextMove);
            bool isTurnManagerSaysThisPlayerHasTurn = myTurnManagerScript.CurrentHasTurnPlayerCheckByPlayerName(playerName);
            bool hasPlayerEnoughBalance = ISeatList.CheckPlayerHasEnoughBalance(seatList, playerName, moveAmount);
            float currentBalance = ISeatList.GetBalanceByPlayerName(seatList, playerName);
            bool isNotFolded = ISeatList.CheckLastMoveIsNotFoldByPlayerName(seatList, playerName);
            bool isMoveAmountValid = myTurnManagerScript.MoveControlMoveAmountIsValid(playerName,myNextMove,moveAmount,subMaxBet,maxRaiseLimit,currentTotalPot,currentBalance);
            if (isPlayerTurn && isTurnManagerSaysThisPlayerHasTurn && hasPlayerEnoughBalance && isNotFolded && isMoveAmountValid && !isPlayerTimeOut)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
        
        
        public List<Seat> GiveTurnToNextPlayer(List<Seat> seatList)
        {
            return myTurnManagerScript.GiveTurnToPlayer(seatList);
        }
        public void MoveTimerStart()
        {
            myTurnManagerScript.MoveTimerStart();
        }
        ///////////////////////////////////////////////////ISeatList Section/////////////////////////////////////////////////////
        public List<Seat> SeatListSetMoveDetails(List<Seat> seatList, Move myNextMove, float moveAmount, string playerName)
        {
            seatList = ISeatList.SetSeatVarMoveTimeByPlayerName(seatList, playerName,myTurnManagerScript.TurnTime);
            seatList = ISeatList.SetLastMoveByPlayerName(seatList, myNextMove, playerName);
            seatList = ISeatList.SetSeatVarLastMoveAmountByPlayerName(seatList, moveAmount, playerName);
            seatList = ISeatList.UpdateSeatVarTotalBetInSubTurnByPlayerName(seatList,playerName,moveAmount);
            seatList = ISeatList.SetSeatVarIsPlayerMovedInSubTurnByPlayerName(seatList, playerName,true);
            seatList = ISeatList.UpdateSeatVarIsPlayerInGameByMove(seatList, playerName, myNextMove);
            return seatList;
        }
        ///////////////////////////////////////////////////PotCalculation Section/////////////////////////////////////////////////////
        public void SetBetAmountForSmallAndBigBlind(List<Seat> seatList)
        {
            ChipManager.instance.SetBetAmountForSmallAndBigBlind(seatList);
        }
        ///////////////////////////////////////////////////TurnMoves Section/////////////////////////////////////////////////////
        public void DoAutoMove(List<Seat> seatList, int currentTurnID, string playerName, float moveAmount, Move move)
        {
            float totalRake = myChipManagerScript.CalculateTotalRake(moveAmount, tableRakePercent);
            float userRakeBack = myChipManagerScript.CalculateUserRakeBack(playerName,totalRake);
            float parentRakeBack = myChipManagerScript.CalculateParentRakeBack(playerName, userRakeBack);
            float profit = myChipManagerScript.CalculateProfit(totalRake, userRakeBack, parentRakeBack);
            
            float currentBalance = ISeatList.GetBalanceByPlayerName(seatList,playerName);
            myTurnManagerScript.CreateNewGameMove(seatList, currentTurnID, currentBalance, move, moveAmount, totalRake,
                userRakeBack, parentRakeBack, profit);
        }
        ///////////////////////////////////////////////////SubMoveList Section/////////////////////////////////////////////////////
        public List<GameMove> GetSubMoveList()
        {
            return myTurnManagerScript.SubMoveListGet();
        }
        ///////////////////////////////////////////////////TurnMovesCheckOrFold Section/////////////////////////////////////////////////////
        public bool DecideCheckOrFoldReturnTrueIfItsCheck()
        {
            return MyTurnManagerScript.DecideCheckOrFoldReturnTrueIfItsCheck(myChipManagerScript.SubMaxBet);
        }
        ///////////////////////////////////////////////////TimerAnimation Section/////////////////////////////////////////////////////
        public void ActivateTimerAnimation(List<Seat> seatList, GameObject activeScene, bool isGameScene, int timeSpan,
            int totalMoveTime)
        {
            TurnManager.instance.TimerAnimationActivate(seatList,activeScene,isGameScene,timeSpan,totalMoveTime);
        }
        public int GetTimeSpan()
        {
            return myTurnManagerScript.TimeSpanGet();
        }
        ///////////////////////////////////////////////////JoinGame Section/////////////////////////////////////////////////////
        public void AddPlayerToPlayerAndRakePercentMap(string dummyUserUsername, float rakePercent)
        {
            myChipManagerScript.AddPlayerToPlayerAndPlayerRakePercentMap(dummyUserUsername, rakePercent);
        }
        public void AddPlayerToPlayerAndParentRakePercentMap(string username, float parentPercent)
        {
            myChipManagerScript.AddPlayerToPlayerAndParentRakePercentMap(username, parentPercent);
        }
        public void AddPlayerToPlayerAndParentMap(string username, string parent)
        {
            myChipManagerScript.AddPlayerToPlayerAndParentMap(username, parent);
        }
        ///////////////////////////////////////////////////StartTurn Section/////////////////////////////////////////////////////
        public List<Seat> StartTurn(List<Seat> seatList, int currentTurnID)
        {
           seatList = DoAutoMoves(seatList,currentTurnID);
           SetSideBetsToZero();
           SetTurnStageEndFlag(false);
           StartPlayerMoveTimer();
           return seatList;
        }

        private void SetTurnStageEndFlag(bool flag)
        {
            myTurnManagerScript.IsSubTurnStageEnded = flag;
        }
        
        private void SetSideBetsToZero()
        {
            myChipManagerScript.SetSideBetsToZero();
        }
        private void StartPlayerMoveTimer()
        {
            myTurnManagerScript.MoveTimerStart();
            
        }
        private List<Seat> DoAutoMoves(List<Seat> seatList, int currentTurnID)
        {
            seatList = DoAutoMoveSmallBlind(seatList,currentTurnID);
            seatList = DoAutoMoveBigBlind(seatList,currentTurnID);
            
            return seatList;
        }
        private List<Seat> DoAutoMoveBigBlind(List<Seat> seatList, int currentTurnID)
        {
            string bigBlindPlayer = myTurnManagerScript.CurrentBigBlindPlayer;
            
            float totalRake = myChipManagerScript.CalculateTotalRake(smallBlind*2, tableRakePercent);
            float userRakeBack = myChipManagerScript.CalculateUserRakeBack(bigBlindPlayer,totalRake);
            float parentRakeBack = myChipManagerScript.CalculateParentRakeBack(bigBlindPlayer, userRakeBack);
            float profit = myChipManagerScript.CalculateProfit(totalRake, userRakeBack, parentRakeBack);
            
            float currentBalance = ISeatList.GetBalanceByPlayerName(seatList,bigBlindPlayer);
            
            GameMove gameMove = myTurnManagerScript.CreateNewGameMove(seatList,currentTurnID,currentBalance,Move.BigBlind,smallBlind * 2,totalRake,userRakeBack,parentRakeBack,profit);
            myChipManagerScript.SetSubMaxBet(smallBlind*2);
            seatList = ISeatList.UpdateSeatVarBalanceByGameMove(seatList,gameMove);
            seatList = ISeatList.SetSeatVarLastMoveAmountByGameMove(seatList,gameMove);
            seatList = ISeatList.UpdateSeatVarTotalBetInSubTurnByMove(seatList,gameMove);
            seatList = ISeatList.SetSeatVarLastMoveByMove(seatList,gameMove);
            seatList = ISeatList.SetSeatVarMoveTimeByMove(seatList,gameMove);
            SetPotDetailsForBigBlind();
            return seatList;
            
        }
        private void SetPotDetailsForBigBlind()
        {
            myChipManagerScript.SetSubMaxBet(smallBlind * 2);
            myChipManagerScript.SetMainBet(0);
            
            currentTotalPot = smallBlind * 3;
            maxRaiseLimit = smallBlind * 7;
        }
        private List<Seat> DoAutoMoveSmallBlind(List<Seat> seatList, int currentTurnID)
        {
            string smallBlindPlayer = myTurnManagerScript.CurrentSmallBlindPlayer;
            
            float totalRake = myChipManagerScript.CalculateTotalRake(smallBlind, tableRakePercent);
            float userRakeBack = myChipManagerScript.CalculateUserRakeBack(smallBlindPlayer,totalRake);
            float parentRakeBack = myChipManagerScript.CalculateParentRakeBack(smallBlindPlayer, userRakeBack);
            float profit = myChipManagerScript.CalculateProfit(totalRake, userRakeBack, parentRakeBack);
            
            float currentBalance = ISeatList.GetBalanceByPlayerName(seatList,smallBlindPlayer);
            
            GameMove gameMove = myTurnManagerScript.CreateNewGameMove(seatList,currentTurnID,currentBalance,Move.SmallBlind,smallBlind,totalRake,userRakeBack,parentRakeBack,profit);
            myChipManagerScript.SetSubMaxBet(smallBlind);
            
            seatList = ISeatList.UpdateSeatVarBalanceByGameMove(seatList,gameMove);
            seatList = ISeatList.SetSeatVarLastMoveAmountByGameMove(seatList,gameMove);
            seatList = ISeatList.UpdateSeatVarTotalBetInSubTurnByMove(seatList,gameMove);
            seatList = ISeatList.SetSeatVarLastMoveByMove(seatList,gameMove);
            seatList = ISeatList.SetSeatVarMoveTimeByMove(seatList,gameMove);
            return seatList;
        }
        public bool SubTurnStageIsEnded()
        {
            return myTurnManagerScript.SubTurnStageIsEnded();
        }
        ///////////////////////////////////////////////////GiveTurntoPlayer Section/////////////////////////////////////////////////////
        public List<Seat> GiveTurntoPlayer(List<Seat> seatList)
        {
            return myTurnManagerScript.GiveTurnToPlayer(seatList);
        }
        ///////////////////////////////////////////////////TurnInitializeNew Section/////////////////////////////////////////////////////
        public List<Seat> InitializeNewTurn(List<Seat> seatList, List<string> inGamePlayerList)
        {
            ClearTableToolsGoData();
            seatList = SetInitialSeatListValuesForNewTurn(seatList, inGamePlayerList);
            InitializeTurn(seatList);
            seatList = PrepareDealerTokenLocation(seatList);
            SetTurnStage(TurnStage.PreFlop);
            
            return seatList;
        }

        public void ClearTableToolsGoData()
        {
            ClearOmahaEngineGoData();
            ClearTurnManagerGOData();
            ClearDealerGoData();
            ClearChipManagerGOData();
        }

        private void ClearChipManagerGOData()
        {
            myChipManagerScript.ClearChipManagerGOData();
        }

        private void ClearDealerGoData()
        {
            myDealerScript.ClearDealerGOData();
        }

        private void ClearOmahaEngineGoData()
        {
            maxRaiseLimit = 0;
            currentTotalPot = 0;
            
            animationTillEnd = false;
            animationStartTurnStage = TurnStage.None;
        }

        private void ClearTurnManagerGOData()
        {
            myTurnManagerScript.ClearTurnManagerGOData();
        }

        private List<Seat> SetInitialSeatListValuesForNewTurn(List<Seat> seatList, List<string> inGamePlayerList)
        {
            seatList = ISeatList.ClearMoveTime(seatList);
            seatList = ISeatList.ClearLastMove(seatList);
            seatList = ISeatList.ClearLastMoveAmount(seatList);
            seatList = ISeatList.ClearTotalBetInSubTurn(seatList);
            seatList = ISeatList.ClearIsPlayerMovedInSubTurn(seatList);
            seatList = ISeatList.ClearIsMyTurn(seatList);
            seatList = ISeatList.ClearIsPlayerInGame(seatList);
            seatList = ISeatList.SetSeatVarIsPlayerInGameByGamePlayerList(seatList,inGamePlayerList);
            return seatList;
        }

        private void SetTurnStage(TurnStage turnStage)
        {
            myTurnManagerScript.SetReadyForTurnEnded(false);
            myTurnManagerScript.SetTurnStage(turnStage);
        }

        
        public List<Seat> PrepareDealerTokenLocation(List<Seat> seatList)
        {
            return myTurnManagerScript.PrepareDealerToken(seatList);
        }
        public void DealCards(List<Seat> mySeatList, int currentTurnID)
        {
           myTurnManagerScript.ActiveSeatLocations = myDealerScript.DealCards(mySeatList,myTurnManagerScript.CurrentDealerlocation,myTurnManagerScript.ActiveSeatLocations,currentTurnID);
        }
        public void PrepareSmallAndBigBlindGODataByDealer(List<Seat> mySeatList)
        {
            myTurnManagerScript.PrepareSmallAndBigBlindGODataByDealer(mySeatList);
        }
        ///////////////////////////////////////////////////CreateNewTurnParts Section/////////////////////////////////////////////////////
        public Turn CreateNewTurn(string matchId, float smallBlind)
        {
            Turn newTurn = new Turn(syncID,smallBlind,myTurnManagerScript.CurrentDealerPlayer,myTurnManagerScript.CurrentSmallBlindPlayer,myTurnManagerScript.CurrentBigBlindPlayer);
            myTurnManagerScript.TurnStartTime(newTurn.StartDate);
            return newTurn;
        }
        
        public TurnPlayers CreateNewTurnPlayers(int currentTurnID)
        {
            TurnPlayers newTurnPlayers = new TurnPlayers(currentTurnID,myChipManagerScript.PlayerAndChipAmountMap,myChipManagerScript.SeatLocationAndChipAmountMap);
            return newTurnPlayers;
        }
        ///////////////////////////////////////////////////OmahaEngineTools Section/////////////////////////////////////////////////////
        public void SetOmahaEngineTools(GameObject newDealer, GameObject newChipManager, GameObject newTurnManager)
        {
            myDealerScript = newDealer.GetComponent<Dealer>();
            myChipManagerScript = newChipManager.GetComponent<ChipManager>();
            myTurnManagerScript = newTurnManager.GetComponent<TurnManager>();
            
        }

        ///////////////////////////////////////////////////Properties Section/////////////////////////////////////////////////////
        

       

        public float MinDeposit
        {
            get => minDeposit;
            set => minDeposit = value;
        }

        public float SmallBlind
        {
            get => smallBlind;
            set => smallBlind = value;
        }

        public float TableRakePercent
        {
            get => tableRakePercent;
            set => tableRakePercent = value;
        }

        public Dealer MyDealerScript => myDealerScript;

        public TurnManager MyTurnManagerScript => myTurnManagerScript;

        public ChipManager MyChipManagerScript => myChipManagerScript;

        public string SyncID
        {
            get => syncID;
            set => syncID = value;
        }


        public List<GameMove> GameMoveListGet()
        {
            return myTurnManagerScript.MoveList;
        }

        public void SetBetAmountCall(List<Seat> seatList, string lastMovePlayerName)
        {
            ChipManager.instance.SetBetAmountForMoveCall(seatList,lastMovePlayerName);
        }

        public bool IsTurnEnded(List<Seat> mySeatList)
        {
            return myTurnManagerScript.IsTurnEnded(mySeatList); 
        }

        public List<Seat> TurnStageEnd(List<Seat> mySeatList)
        {
            myTurnManagerScript.TurnStageEnd();
            
            mySeatList = ISeatList.ResetForNewTurnStage(mySeatList);
            return mySeatList;
        }

        public void CalculatePot(List<Seat> seatList)
        {
            int sideBetCount = myChipManagerScript.SideBetCount;
            if (!myTurnManagerScript.IsSubTurnStageCauseSideBet)
            {
                switch (sideBetCount)
                {
                    case 0:
                        myChipManagerScript.MainBet = currentTotalPot;
                        myChipManagerScript.sideBetNoAndSideBetAmountMap[0] = myChipManagerScript.MainBet;
                        break;
                    case 1:
                        myChipManagerScript.MainBet = currentTotalPot;
                        myChipManagerScript.sideBetNoAndSideBetAmountMap[0] = myChipManagerScript.MainBet;
                        
                        break;
                    case 2:
                        myChipManagerScript.SideBet1 = currentTotalPot - myChipManagerScript.MainBet;
                        myChipManagerScript.sideBetNoAndSideBetAmountMap[1] = myChipManagerScript.SideBet1;
                        
                        break;
                    case 3:
                        myChipManagerScript.SideBet2 = currentTotalPot - myChipManagerScript.MainBet - myChipManagerScript.SideBet1;
                        myChipManagerScript.sideBetNoAndSideBetAmountMap[2] = myChipManagerScript.SideBet2;
                        
                        break;
                    case 4:
                        myChipManagerScript.SideBet3 = currentTotalPot - myChipManagerScript.MainBet - myChipManagerScript.SideBet1 - myChipManagerScript.SideBet2;
                        myChipManagerScript.sideBetNoAndSideBetAmountMap[3] = myChipManagerScript.SideBet3;
                        
                        break;
                    case 5:
                        myChipManagerScript.SideBet4 = currentTotalPot - myChipManagerScript.MainBet - myChipManagerScript.SideBet1 - myChipManagerScript.SideBet2 - myChipManagerScript.SideBet3;
                        myChipManagerScript.sideBetNoAndSideBetAmountMap[4] = myChipManagerScript.SideBet4;
                        
                        break;
                    case 6:
                        myChipManagerScript.SideBet5 = currentTotalPot - myChipManagerScript.MainBet - myChipManagerScript.SideBet1 - myChipManagerScript.SideBet2 - myChipManagerScript.SideBet3 - myChipManagerScript.SideBet4;
                        myChipManagerScript.sideBetNoAndSideBetAmountMap[5] = myChipManagerScript.SideBet5;
                        
                        break;
                    case 7:
                        myChipManagerScript.SideBet6 = currentTotalPot - myChipManagerScript.MainBet - myChipManagerScript.SideBet1 - myChipManagerScript.SideBet2 - myChipManagerScript.SideBet3 - myChipManagerScript.SideBet4 - myChipManagerScript.SideBet5;
                        myChipManagerScript.sideBetNoAndSideBetAmountMap[6] = myChipManagerScript.SideBet6;
                        break;
                    default:
                        Debug.LogError("Side Bet Count is bigger than 7");
                        break;
                }
            }
            else
            {
                List<Tuple<List<string>, float>> sideBetOwnersAndAmountTupleList = myTurnManagerScript.CalculateSubTurnSideBetOwnersAndAmountTupleList();

                List<string> gamePlayerList = ISeatList.GenerateInGamePlayerList(seatList);
                myChipManagerScript.SetSideBetInformations(sideBetOwnersAndAmountTupleList,gamePlayerList);
            }
        }
        
        public List<CardData> GetMiddleCards()
        {
            return myDealerScript.GetMiddleCards(myTurnManagerScript.MyTurnStage);
        }

        public void SetBetAmountCheck(List<Seat> seatList, string lastMovePlayerName)
        {
            ChipManager.instance.SetBetAmountForMoveCheck(seatList,lastMovePlayerName);
        }

        public void StartNewSubTurn(bool value)
        {
            myTurnManagerScript.StartNewSubTurn(value);
        }

        public void ClearSubMoveList()
        {
            myTurnManagerScript.ClearSubMoveList();
        }

        public void ClearSubMaxBet()
        {
            myChipManagerScript.ClearSubMaxBet();
        }

        public void SetBetAmountRaise(List<Seat> seatList, string lastMovePlayerName)
        {
            ChipManager.instance.SetBetAmountForMoveRaise(seatList,lastMovePlayerName);
        }

        public void SetBetAmountAllin(List<Seat> seatList, string lastMovePlayerName)
        {
            ChipManager.instance.SetBetAmountForMoveAllin(seatList,lastMovePlayerName);
        }

        public List<CardData> SharedCards()
        {
            TurnStage CurrentTurnStage = myTurnManagerScript.MyTurnStage;
            return myDealerScript.GetSharedCards(CurrentTurnStage);
        }

        public List<Tuple<string,List<CardData>>> DecideWhoWins(List<Seat> mySeatList, List<CardData> sharedCards)
        {
            int sideBetCount = myChipManagerScript.SideBetCount;
            List<string> inGamePlayerList = ISeatList.GetInGamePlayerList(mySeatList);
            //TopTier Winners
            List<Tuple<string,List<CardData>>> winnersAndCombinations = FindWinnerHands(inGamePlayerList,sharedCards);
            if (sideBetCount == 0)
            {
                return winnersAndCombinations;
            }

            List<Tuple<string, List<CardData>>> winnersAndCombinationsFinal = new List<Tuple<string, List<CardData>>>();
            
            //find max tier among winners
            for (int i = 0; i < sideBetCount; i++)
            {
                List<Tuple<string,int>> winnersAndTiersTupleList =  myChipManagerScript.FindWinnersTier(winnersAndCombinations);
                int remainingTiers = CalculateRemainingTiers(winnersAndTiersTupleList,sideBetCount);
                winnersAndCombinations = CreateNewWinnersAndCombinations(winnersAndTiersTupleList,winnersAndCombinations);
                winnersAndCombinationsFinal = MergeThisListToFinalList(winnersAndCombinationsFinal,winnersAndCombinations);
                if (remainingTiers == 0)
                {
                    return winnersAndCombinationsFinal;
                }
                List<string> remainingPlayers = myChipManagerScript.FindRemainingTierPlayers(remainingTiers,sideBetCount);
                winnersAndCombinations = FindWinnerHands(remainingPlayers,sharedCards);
            }
            
            return winnersAndCombinations;
        }

        private List<Tuple<string, List<CardData>>> MergeThisListToFinalList(List<Tuple<string, List<CardData>>> winnersAndCombinationsFinal, List<Tuple<string, List<CardData>>> winnersAndCombinations)
        {
            foreach (var winnerAndCombination in winnersAndCombinations)
            {
                winnersAndCombinationsFinal.Add(winnerAndCombination);
            }

            return winnersAndCombinationsFinal;
        }

        private List<Tuple<string, List<CardData>>> CreateNewWinnersAndCombinations(List<Tuple<string, int>> winnersAndTiersTupleList, List<Tuple<string, List<CardData>>> winnersAndCombinations)
        {
            //findMaxTier players
            int maxTier = 0;
            List<string> maxTierPlayers = new List<string>();
            foreach (var winnerAndTierTuple in winnersAndTiersTupleList)
            {
                if (winnerAndTierTuple.Item2 > maxTier)
                {
                    maxTier = winnerAndTierTuple.Item2;
                }
            }
            foreach (var winnerAndTierTuple in winnersAndTiersTupleList)
            {
                if (winnerAndTierTuple.Item2 == maxTier)
                {
                    maxTierPlayers.Add(winnerAndTierTuple.Item1);
                }
            }
            //find maxTierPlayers in winnersAndCombinations
            List<Tuple<string, List<CardData>>> winnersAndCombinationsWithMaxTier = new List<Tuple<string, List<CardData>>>();
            foreach (var winnerAndCombinationTuple in winnersAndCombinations)
            {
                if (maxTierPlayers.Contains(winnerAndCombinationTuple.Item1))
                {
                    winnersAndCombinationsWithMaxTier.Add(winnerAndCombinationTuple);
                }
            }
            return winnersAndCombinationsWithMaxTier;
        }

        private int CalculateRemainingTiers(List<Tuple<string, int>> winnersAndTiersTupleList, int sideBetCount)
        {
            List<int> tierList = new List<int>();
            int remainingTiers = 0;
            foreach (var winnerAndTierTuple in winnersAndTiersTupleList)
            {
                tierList.Add(winnerAndTierTuple.Item2);
            }
            //reOrder tierList from max to min
            tierList.Sort();
            tierList.Reverse();
            if (tierList[0] == (sideBetCount -1))
            {
                remainingTiers = 0;
            }
            else
            {
                remainingTiers = tierList[0] + 1;
            }
            return remainingTiers;
        }

        private List<Tuple<string,List<CardData>>> FindWinnerHands(List<string> iInGamePlayerList, List<CardData> sharedCards)
        {
            Dictionary<string,Tuple<int, List<CardData>>> playerAndHandRankAndWinningCombinations = new Dictionary<string, Tuple<int, List<CardData>>>();
            Tuple<int, List<CardData>> maxValuesAndCombination = new Tuple<int, List<CardData>>(0,new List<CardData>());
            foreach (string playerName in iInGamePlayerList)
            {
                List<CardData> playerCards = myDealerScript.GetPlayerCards(playerName);
                maxValuesAndCombination = CalculateHandRank(playerCards,sharedCards);
                playerAndHandRankAndWinningCombinations.Add(playerName,maxValuesAndCombination);
            }
            return FindWinnerPlayer(playerAndHandRankAndWinningCombinations);
        }

        private Tuple<int, List<CardData>> CalculateHandRank(List<CardData> playerCards, List<CardData> sharedCards)
        {
            return myDealerScript.CalculateHandRank(playerCards,sharedCards);
        }

        private List<Tuple<string,List<CardData>>> FindWinnerPlayer(Dictionary<string,Tuple<int, List<CardData>>> playerAndHandRankAndWinningCombinations)
        {
            List<Tuple<string,List<CardData>>> winnersAndCombinations = new List<Tuple<string, List<CardData>>>();
            int minHandRank = int.MaxValue;
            
            foreach (KeyValuePair<string,Tuple<int, List<CardData>>> playerAndHandRankAndWinningCombination in playerAndHandRankAndWinningCombinations)
            {
                if (playerAndHandRankAndWinningCombination.Value.Item1 < minHandRank)
                {
                    minHandRank = playerAndHandRankAndWinningCombination.Value.Item1;
                }
            }
            foreach (KeyValuePair<string,Tuple<int, List<CardData>>> playerAndHandRankAndWinningCombination in playerAndHandRankAndWinningCombinations)
            {
                if (playerAndHandRankAndWinningCombination.Value.Item1 == minHandRank)
                {
                    winnersAndCombinations.Add(new Tuple<string, List<CardData>>(playerAndHandRankAndWinningCombination.Key,playerAndHandRankAndWinningCombination.Value.Item2));
                }
            }
            return winnersAndCombinations;
        }

        public void SetReadyForTurnEnd(bool value)
        {
            myTurnManagerScript.SetReadyForTurnEnded(value);
        }

        public void SetWinners(List<Tuple<string, List<CardData>>> winnersAndCombinations)
        {
            myTurnManagerScript.SetWinners(winnersAndCombinations);
        }

        public DateTime GetTurnStartDate()
        {
            return myTurnManagerScript.GetTurnStartDate();
        }

        public float GetTotalPot()
        {
            return myTurnManagerScript.GetTotalPot();
        }

        public float GetTotalRakeBack()
        {
            return myTurnManagerScript.GetTotalRakeBack();
        }

        public float GetTotalProfit()
        {
            return myTurnManagerScript.GetTotalProfit();
        }

        public List<GameMove> GetMoveList()
        {
            return myTurnManagerScript.GetMoveList();
        }


        public TurnEndType GetTurnEndType()
        {
            return myTurnManagerScript.GetTurnEndType();
        }

        public List<CardData> GetPlayerCards(string playerName)
        {
            return myDealerScript.GetPlayerCards(playerName);
        }

        public List<Tuple<string, List<CardData>>> GetWinnerAndCombinations()
        {
            return myTurnManagerScript.GetWinnerAndCombinations();
        }

        public List<string> GetWinnerListOrderByInGamePlayerList(List<string> inGamePlayerList, List<Tuple<string, List<CardData>>> winnerAndCombinations)
        {
            return myTurnManagerScript.GetWinnerListOrderByInGamePlayerList(inGamePlayerList,winnerAndCombinations);
        }

        public List<List<CardData>> GetWinnerCombinationsOrderByInGamePlayerList(List<string> inGamePlayerList, List<Tuple<string, List<CardData>>> winnerAndCombinations)
        {
            return myTurnManagerScript.GetWinnerCombinationsOrderByInGamePlayerList(inGamePlayerList,winnerAndCombinations);
        }

        public bool isReadyForTurnEnd()
        {
            return myTurnManagerScript.isReadyForTurnEnd();
        }

        public float GetWinAmount(float tableRakePercent)
        {
            return myTurnManagerScript.GetWinAmount(tableRakePercent);
        }

        public void GiveTurnToWinner(string winnerName, List<Seat> mySeatList)
        {
            myTurnManagerScript.GiveTurnToWinner(winnerName,mySeatList);
        }

        public void SetTurnWinners(List<Tuple<string, List<CardData>>> winnersAndCombinations, int currentTurnID)
        {
            TurnWinners turnWinners = myChipManagerScript.GenerateTurnWinners(winnersAndCombinations,currentTurnID,currentTotalPot,tableRakePercent);
            myTurnManagerScript.SetTurnWinners(turnWinners);
        }

        public TurnWinners GetTurnWinners()
        {
            return myTurnManagerScript.GetTurnWinners();
        }

        public List<Seat> AddWinAmountsToPlayers(List<Seat> seatList, List<string> winnerList, List<float> winnerPotList)
        {
            return myDealerScript.AddWinAmountsToPlayers(seatList,winnerList,winnerPotList);
        }

        public string GetWinnerName()
        {
            return myTurnManagerScript.GetWinnerName();
        }

        public List<CardData> GetWinnerHand(string winnerName)
        {
            return myDealerScript.GetPlayerCards(winnerName);
        }

        public GameMove GetCorrectionMove()
        {
            return myTurnManagerScript.GetCorrectionMove();
        }

        public void MakeCorrection()
        {
            string correctionPlayerName = myTurnManagerScript.CreateCorrectionMove();
            float correctionMoveAmount = CalculateCorrectionMoveAmount();
            
            
            
            
            float totalRake = myChipManagerScript.CalculateTotalRake(correctionMoveAmount, tableRakePercent);
            float userRakeBack = myChipManagerScript.CalculateUserRakeBack(correctionPlayerName,totalRake);
            float parentRakeBack = myChipManagerScript.CalculateParentRakeBack(correctionPlayerName,userRakeBack);
            float profit = myChipManagerScript.CalculateProfit(totalRake, userRakeBack, parentRakeBack);
            
            myTurnManagerScript.CreateCorrectionGameMove(correctionMoveAmount,totalRake,userRakeBack,parentRakeBack,profit);
            
            float correctionReturnAmount = myTurnManagerScript.GetCorrectionReturnAmount();
            SetPotDetailsForMoveCorrection(correctionReturnAmount);
        }

        private void SetPotDetailsForMoveCorrection(float correctionMoveAmount)
        {
            
            currentTotalPot = currentTotalPot - correctionMoveAmount;
            
        }

        private float CalculateCorrectionMoveAmount()
        {
            return myTurnManagerScript.CalculateCorrectionMoveAmount();
        }

        public float GetCorrectionMoveAmount()
        {
           return myTurnManagerScript.GetCorrectionMoveAmount();
        }

        public string GetCorrectionMovePlayerName()
        {
            return myTurnManagerScript.GetCorrectionMovePlayerName();
        }

        public float GetCorrectionBetAmountForClientSide()
        {
            return myTurnManagerScript.GetCorrectionBetAmountForClientSide();
        }

        public void SetGameAnimationTillEnd(bool value)
        {
            animationTillEnd = value;
        }

        public void SetTurnStageForAnimation(TurnStage preShowDown)
        {
            myTurnManagerScript.SetTurnStageForAnimation(preShowDown);
        }

        public void SetAnimationStartStage()
        {
            animationStartTurnStage = myTurnManagerScript.GetTurnStage();
        }

        public bool GetAnimationTillEnd()
        {
            return animationTillEnd;
        }

        public TurnStage GetAnimationStartStage()
        {
            return animationStartTurnStage;
        }


        public List<CardData> GetSharedCards(TurnStage turnStage)
        {
            return myDealerScript.GetSharedCards(turnStage);
        }

        public CardData GetTurnCard()
        {
            return myDealerScript.GetTurnCardOnly();
        }

        public CardData GetRiverCard()
        {
            return myDealerScript.GetRiverCardOnly();
        }

        public void MakeRakeBackCalculations()
        {
            List<GameMove> gameMoveList = myTurnManagerScript.GetMoveList();
            myChipManagerScript.MakeRakeBackCalculations(gameMoveList);
            
         }

        public float GetPlayerRakeBackAmount(string username)
        {
            return myChipManagerScript.GetFromPlayerAndPlayerRakeAmountMap(username);
        }

        public string GetPlayerParentName(string playerName)
        {
            return myChipManagerScript.GetPlayerParentName(playerName);
        }

        public float GetPlayerParentRakeBackAmount(string playerName)
        {
            return myChipManagerScript.GetParentRakeBackAmount(playerName);
        }


        public void RemovePlayerFromPlayerAndParentMap(string playerName)
        {
            myChipManagerScript.RemovePlayerFromPlayerAndParentMap(playerName);
        }

        public void RemovePlayerFromPlayerAndParentRakePercentMap(string playerName)
        {
            myChipManagerScript.RemovePlayerFromPlayerAndParentRakePercentMap(playerName);
        }

        public void RemoveParentFromParentAndParentRakeAmountMap(string playerName)
        {
            myChipManagerScript.RemoveParentFromParentAndParentRakeAmountMap(playerName);
        }

        public void RemovePlayerFromPlayerAndPlayerRakePercentMap(string playerName)
        {
            myChipManagerScript.RemovePlayerFromPlayerAndPlayerRakePercentMap(playerName);
        }

        public void RemovePlayerFromPlayerAndPlayerRakeAmountMap(string playerName)
        {
            myChipManagerScript.RemovePlayerFromPlayerAndPlayerRakeAmountMap(playerName);
        }


        public bool isToggleMoveExistInPlayerToggleMoveTupleList(string playerName, ToggleMove foldCheck)
        {
            return myDealerScript.isToggleMoveExistInPlayerToggleMoveTupleList(playerName,foldCheck);
        }

        public bool isCallAmountCorrect(string playerName, float moveAmount, List<Seat> mySeatList)
        {
            float subMaxBetAmount = MyChipManagerScript.SubMaxBet;
            float myPreviousTotalBetAmountInSubTurn = ISeatList.GetTotalBetInSubTurnByPlayerName(mySeatList, playerName);

            if ((subMaxBetAmount - myPreviousTotalBetAmountInSubTurn) == moveAmount)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddToggleMoveToTheList(ToggleMove toggleMove, float moveAmount, string playerName)
        {
            myDealerScript.AddToggleMoveToTheList(toggleMove,moveAmount,playerName);
        }

        public void RemoveToggleMoveFromTheList(string playerName)
        {
            myDealerScript.RemoveToggleMoveFromTheList(playerName);
        }

        public bool NextPlayerHasPreMove()
        {
            string nextPlayerName = MyTurnManagerScript.CurrentHasTurnPlayer;
            return myDealerScript.PlayerHasPreMove(nextPlayerName);
        }

        private string GetNextPreMovePlayerName()
        {
            return myDealerScript.GetPreMovePlayerName();
        }

        public string GetCurrentHasTurnPlayerName()
        {
            return myTurnManagerScript.CurrentHasTurnPlayer;
        }

        public ToggleMove GetToggleMoveOfPlayer(string preMovePlayerName)
        {
            return myDealerScript.GetToggleMoveOfPlayer(preMovePlayerName);
        }

        public float GetToggleMoveAmountOfPlayer(string preMovePlayerName)
        {
            return myDealerScript.GetToggleMoveAmountOfPlayer(preMovePlayerName);
        }

        public Move GeneratePreMoveFromToggleMove(ToggleMove toggleMove, string playerName, List<Seat> mySeatList)
        {
            Move preMove = Move.None;
            switch (toggleMove)
            {
                case ToggleMove.Call:
                    preMove =  Move.Call;break;
                case ToggleMove.Check:
                    preMove = Move.Check;break;
                case ToggleMove.FoldCheck:
                    bool isPlayerAllowedToCheck = IsPlayerAllowedToSayCheck(playerName, mySeatList);
                    if (isPlayerAllowedToCheck)
                    {
                        preMove = Move.Check;
                    }
                    else
                    {
                        preMove = Move.Fold;
                    }break;
            }
            return preMove;
        }

        public bool IsPlayerAllowedToSayCheck(string playerName, List<Seat> mySeatList)
        {
            float subMaxBetAmount = MyChipManagerScript.SubMaxBet;
            float myPreviousTotalBetAmountInSubTurn = ISeatList.GetTotalBetInSubTurnByPlayerName(mySeatList, playerName);

            if ((subMaxBetAmount - myPreviousTotalBetAmountInSubTurn) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetPreMovePlayerName()
        {
            string preMovePlayerName = myTurnManagerScript.CurrentHasTurnPlayer;
            myDealerScript.SetPreMovePlayerName(preMovePlayerName);
        }

        public void RemovePreMovePlayerName()
        {
            myDealerScript.RemovePreMovePlayerName();
        }

        public bool HasPreMove()
        {
            return myDealerScript.HasPreMove();
        }

        public int GetTurnTime()
        {
            return myTurnManagerScript.GetTurnTime();
        }

        public int GetTotalMoveTimeForEndGame()
        {
            return myTurnManagerScript.GetTotalMoveTimeForEndGame();
        }

        public string GetCurrentHasTurnPlayer()
        {
            return myTurnManagerScript.CurrentHasTurnPlayer;
        }

        public void SetActiveSeatLocations(List<Seat> mySeatList)
        {
            myTurnManagerScript.SetActiveSeatLocationsArray(mySeatList);
        }

        public bool GetReadyForTurnEnd()
        {
            return myTurnManagerScript.GetReadyForTurnEnd();
        }

        public bool GetAnimationTillEndFlag()
        {
            return animationTillEnd;
        }

        public void SetIsEndGame(bool isEndGame)
        {
            myTurnManagerScript.SetIsEndGame(isEndGame);
        }

        public void SetPlayerTimeOut(bool value)
        {
            myTurnManagerScript.SetPlayerTimeOut(value);
        }

        public bool GetIsPlayerTimeOut()
        {
            return myTurnManagerScript.GetIsPlayerTimeOut();
        }

        public void SetMoveTimeCalculate(bool value)
        {
            myTurnManagerScript.SetMoveTimeCalculate(value);
        }

        public void ParentAndParentRakeAmountMapAdd(string parent, float amount)
        {
            myChipManagerScript.AddParentToParentAndRakeAmountMap(parent, amount);
        }

        public void PlayerAndRakeAmountMapAdd(string username, float amount)
        {
            myChipManagerScript.AddPlayerToPlayerAndRakeAmountMap(username, amount);
        }

        public float GetSubMaxBet()
        {
            return myChipManagerScript.SubMaxBet;
        }

        public float GetPlayerSubMaxBet(string playerName)
        {
            return myTurnManagerScript.GetPlayerSubMaxBet(playerName);
        }

        public float GetMainBet()
        {
            return myChipManagerScript.MainBet;
        }

        public int GetSideBetCount()
        {
            return myChipManagerScript.GetSideBetCount();
        }

        public float GetSideBet(int index)
        {
            return myChipManagerScript.GetSideBet(index);
        }
       
        public List<float> GetWinnerPotListOrderByInGamePlayerList(List<string> winnerList)
        {
            return myTurnManagerScript.GetWinnerPotListOrderByInGamePlayerList(winnerList);
        }

        public void CalculateMaxRaiseLimit(List<Seat> seatList)
        {
            if (SubTurnStageIsEnded())
            {
                maxRaiseLimit = currentTotalPot;
            }
            else
            {
                string hasturnPlayerName = ISeatList.GetPlayerNameByIsMyTurn(seatList);
                float playerPreviosTotalBet = ISeatList.GetTotalBetInSubTurnByPlayerName(seatList, hasturnPlayerName);
                float myOpponentsMaxTotalBetInSubTurn = ISeatList.GetOpponenetsMaxTotalBetInSubTurn(seatList, hasturnPlayerName);
                float callAmount= myOpponentsMaxTotalBetInSubTurn - playerPreviosTotalBet;
                maxRaiseLimit = currentTotalPot + callAmount * 2 - playerPreviosTotalBet;
            }
        }
    }
}