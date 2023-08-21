using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    /*
     * TableScene'de kullanılan toolları yönecetecek sınıftır
     * Tuttuğu Listeler
     * seatList -->> GameScene'de bulunan koltukları ifade eder. 
     * tablePlayerList -->> TableScene'e geçiş yapmış tüm userları ifade eder. (masadakiler ve observerlar)
     * Bu listeler
     *              -->> server tarafında ADD-REMOVE functionları ile idare edilirken
     *              -->> client tarafında UPDATE functionları ile idare edilir. Tüm update işlemleri TableGameManager'in List'lerini temel alır .
     */
    public class TableGameManager : MonoBehaviour
    {
        ///////////////////////////////////////////////////Variables Section/////////////////////////////////////////////////////
        public static TableGameManager instance;

        [SerializeField] private string syncID;
        [SerializeField] private int seatCount;
        [SerializeField] private float minDeposit;
        [SerializeField] private float smallBlind;
        [SerializeField] private float tableRakePercent;
        [SerializeField] private string activeUI;
        [SerializeField] private GameState myGameState;
        [SerializeField] private SeatLocations requestedSeat;
        [SerializeField] private int currentTurnID;

        [Header("CoroutineManagement")]
        [SerializeField] private string coroutineMaster;
        [SerializeField] private string leaverPlayer;
        [SerializeField] private bool isCoroutineNeedTransfer;
        [SerializeField] private bool isCoroutineTransferCompleted;
        
        TablePlayerManager myTablePlayerManagerScript = null;
        GamePlayerManager myGamePlayerManagerScript = null;
        GamePlayerSpawnManager myGamePlayerSpawnManagerScript = null;
        GameUISpawnManager myGameUISpawnManagerScript = null;
        OmahaEngine myOmahaEngineScript = null;

        private List<Seat> mySeatList = new List<Seat>();
        private List<string> myPlayerList = new List<string>();

        private GameObject inActiveScene;
        private Seat defaultSeat;
        private GameObject activeScene;
        private GameObject tableScene;
        private GameObject gameScene;
        private string errorSeatTakenString = "Oturmak istediğiniz koltuk doldu";
        private int deposit;

        private bool isEndGame;
        
        private IEnumerator waitForPlayerMoveCoroutine;

        private bool stopCurrentCoroutine;
        ///////////////////////////////////////////////////Events Section/////////////////////////////////////////////////////
        private void Awake()
        {
            defaultSeat = new Seat();
            Debug.Log("TableGameManager -->>> awake");
            syncID = "lobby";
        }

        // Start is called before the first frame update
        void Start()
        {
            instance = this;

        }

        private void Update()
        {

        }
        
        ///////////////////////////////////////////////////NextMoveSucces Section/////////////////////////////////////////////////////
        
        
        public void NextMoveStageChangeSucces(List<Seat> seatList, string lastMovePlayerName, string myName,
            List<CardData> middleCards, float middlePot, int sideBetCount, List<float> sideBetList, bool isEndGame)
        {
            //FindLastMove
            Move lastMove = ISeatList.GetLastMoveByPlayerName(seatList, lastMovePlayerName);
            switch (lastMove)
            {
                case Move.Check:
                    NextMoveCheckStageChange(seatList, lastMovePlayerName, myName, middleCards, middlePot,sideBetCount,sideBetList);
                    break;
                case Move.Call:
                    NextMoveCallStageChange(seatList, lastMovePlayerName, myName, middleCards, middlePot,sideBetCount,sideBetList);
                    break;
                case Move.Fold:
                    NextMoveFoldStageChange(seatList, lastMovePlayerName, myName, middleCards, middlePot,sideBetCount,sideBetList,isEndGame);
                    break;
                case Move.AllIn:
                    NextMoveAllInStageChange(seatList, lastMovePlayerName, myName, middleCards, middlePot,sideBetCount,sideBetList);
                    break;
            }
        }

        private void NextMoveAllInStageChange(List<Seat> seatList, string lastMovePlayerName, string myName,
            List<CardData> middleCards, float middlePot, int sideBetCount, List<float> sideBetList)
        {
            int totalMoveTime = TurnManager.instance.TotalMoveTime;
            if (ActiveScene == gameScene)
            {
                seatList = ISeatList.ReArrange(seatList, myName);
                DoNextMoveAllin(seatList, myName,lastMovePlayerName);
                GameControllerActivate(seatList, myName);
                if (string.Equals(myName, lastMovePlayerName))
                {
                    GameControllerDeactivate(seatList);
                }
                BetAmountSetForMoveAllin(seatList, lastMovePlayerName);
                GameControllerValuesForMoveAllin(seatList, myName, lastMovePlayerName,true,middlePot,sideBetCount,sideBetList);
                TimerAnimationActivate(seatList,0,totalMoveTime);
                SetMiddleCards(middleCards);
                SetMiddlePot(middlePot,sideBetCount,sideBetList);
                ClearPreviousSubTurnBets(seatList);
            }
            if (activeScene == tableScene)
            {
                DoNextMoveAllin(seatList, myName,lastMovePlayerName);
                TimerAnimationActivate(seatList,0,totalMoveTime);
                SetMiddleCards(middleCards);
                SetMiddlePot(middlePot,sideBetCount,sideBetList);
                
            }
        }

        private void NextMoveFoldStageChange(List<Seat> seatList, string lastMovePlayerName, string myName,
            List<CardData> middleCards, float middlePot, int sideBetCount, List<float> sideBetList, bool isEndGame)
        {
            int totalMoveTime = TurnManager.instance.TotalMoveTime;
            int totalMoveTimeForEndGame = TurnManager.instance.TotalMoveTimeForEndGame;
            if (ActiveScene == gameScene && gameScene != null)
            {
                seatList = ISeatList.ReArrange(seatList, myName);
                DoNextMoveFold(seatList, myName,lastMovePlayerName);
                GameControllerActivate(seatList, myName);
                CloseHand(seatList, lastMovePlayerName,myName);
                if (string.Equals(myName, lastMovePlayerName))
                {
                    GameControllerDeactivate(seatList);
                }
                GameControllerValuesForMoveFold(seatList, myName, lastMovePlayerName,middlePot,sideBetCount,sideBetList,true);
                SetMiddleCards(middleCards);
                SetMiddlePot(middlePot,sideBetCount,sideBetList);
                if (!isEndGame)
                {
                    TimerAnimationActivate(seatList,0,totalMoveTime);
                    ClearPreviousSubTurnBets(seatList);
                }
                //if player is winner
                ActivatePreShowDown(seatList, lastMovePlayerName,myName);
                TimerAnimationActivate(seatList,0,totalMoveTimeForEndGame);
                
            }
            if (activeScene == tableScene && tableScene != null)
            {
                DoNextMoveFold(seatList, myName,lastMovePlayerName);
                CloseHand(seatList, lastMovePlayerName,myName);
                SetMiddleCards(middleCards);
                SetMiddlePot(middlePot,sideBetCount,sideBetList);
                if (!isEndGame)
                {
                    TimerAnimationActivate(seatList,0,totalMoveTime);
                    ClearPreviousSubTurnBets(seatList);
                }
                TimerAnimationActivate(seatList,0,totalMoveTimeForEndGame);
            }
        }

        private void ActivatePreShowDown(List<Seat> seatList, string lastMovePlayerName, string myName)
        {
            GameUISpawnManager.instance.ActivatePreShowDown(seatList, lastMovePlayerName,myName, activeScene);
        }

        

        private void NextMoveCallStageChange(List<Seat> seatList, string lastMovePlayerName, string myName,
            List<CardData> middleCards, float middlePot, int sideBetCount, List<float> sideBetList)
        {
            int totalMoveTime = TurnManager.instance.TotalMoveTime;
            if (ActiveScene == gameScene)
            {
                seatList = ISeatList.ReArrange(seatList, myName);
                DoNextMoveCall(seatList, myName,lastMovePlayerName);
                GameControllerActivate(seatList, myName);
                BetAmountSetForMoveCall(seatList, lastMovePlayerName);
                GameControllerValuesForMoveCall(seatList, myName, lastMovePlayerName,true,middlePot,sideBetCount,sideBetList);
                TimerAnimationActivate(seatList,0,totalMoveTime);
                SetMiddleCards(middleCards);
                SetMiddlePot(middlePot,sideBetCount,sideBetList);
                ClearPreviousSubTurnBets(seatList);
            }
            if (activeScene == tableScene)
            {
                DoNextMoveCall(seatList, myName,lastMovePlayerName);
                TimerAnimationActivate(seatList,0,totalMoveTime);
                SetMiddleCards(middleCards);
                SetMiddlePot(middlePot,sideBetCount,sideBetList);
                ClearPreviousSubTurnBets(seatList);
            }
        }

        private void NextMoveCheckStageChange(List<Seat> seatList, string lastMovePlayerName, string myName,
            List<CardData> middleCards, float middlePot, int sideBetCount, List<float> sideBetList)
        {
            int totalMoveTime = TurnManager.instance.TotalMoveTime;
            if (ActiveScene == gameScene)
            {
                seatList = ISeatList.ReArrange(seatList, myName);
                DoNextMoveCheck(seatList, myName);
                GameControllerActivate(seatList, myName);
                BetAmountSetForMoveCheck(seatList, lastMovePlayerName);
                GameControllerValuesForMoveCheck(seatList, myName, lastMovePlayerName,true,middlePot,sideBetCount,sideBetList);
                TimerAnimationActivate(seatList,0,totalMoveTime);
                SetMiddleCards(middleCards);
                SetMiddlePot(middlePot,sideBetCount,sideBetList);
                ClearPreviousSubTurnBets(seatList);
            }
            if (activeScene == tableScene)
            {
                DoNextMoveCheck(seatList, myName);
                TimerAnimationActivate(seatList,0,totalMoveTime);
                SetMiddleCards(middleCards);
                SetMiddlePot(middlePot,sideBetCount,sideBetList);
                ClearPreviousSubTurnBets(seatList);
            }
        }

        private void ClearPreviousSubTurnBets(List<Seat> seatList)
        {
            if (activeScene == gameScene)
            {
                GameUISpawnManager.instance.ClearPreviousSubTurnBets(seatList,activeScene,true);
            }

            if (activeScene == tableScene)
            {
                GameUISpawnManager.instance.ClearPreviousSubTurnBets(seatList,activeScene,false);
            }
        }

        private void SetMiddlePot(float middlePot, int sideBetCount, List<float> sideBetList)
        {
            if (activeScene == gameScene)
            {
                GameUISpawnManager.instance.SetMiddlePot(middlePot,sideBetCount,sideBetList, activeScene, true);
            }

            if (activeScene == tableScene)
            {
                GameUISpawnManager.instance.SetMiddlePot(middlePot,sideBetCount,sideBetList, activeScene, false);
            }
        }

        private void SetMiddleCards(List<CardData> middleCards)
        {
            if (activeScene == gameScene)
            {
                GameUISpawnManager.instance.SetMiddleCards(middleCards, activeScene, true);
            }

            if (activeScene == tableScene)
            {
                GameUISpawnManager.instance.SetMiddleCards(middleCards, activeScene, false);
            }
        }

        private void GameControllerValuesForMoveCheck(List<Seat> seatList, string myName, string lastMovePlayerName,
            bool isStageChange, float middlePot, int sideBetCount, List<float> sideBetList)
        {
            GameUISpawnManager.instance.SetGameControllerValuesForMoveCheck(seatList, myName,lastMovePlayerName, activeScene, isStageChange,middlePot,smallBlind,sideBetCount,sideBetList);
        }

        private void BetAmountSetForMoveCheck(List<Seat> seatList, string lastMovePlayerName)
        {
            OmahaEngine.instance.SetBetAmountCheck(seatList, lastMovePlayerName);
        }

        private void DoNextMoveCheck(List<Seat> seatList, string myName)
        {
            if (activeScene == gameScene)
            {
                GamePlayerSpawnManager.instance.UpdateOtherPlayers(seatList, myName, activeScene);
                GamePlayerSpawnManager.instance.UpdateLocalPlayer(seatList, myName, activeScene);
            }

            if (activeScene == tableScene)
            {
                GamePlayerSpawnManager.instance.UpdateNonLocalPlayersOnTableScene(seatList, activeScene);
            }
        }

        public void NextMoveNoStageChangeSucces(List<Seat> seatList, string lastMovePlayerName, string myName,
            float middlePot, int sideBetCount, List<float> sideBetList)
        {
            
            //FindLastMove
            Move lastMove = ISeatList.GetLastMoveByPlayerName(seatList, lastMovePlayerName);
            switch (lastMove)
            {
                case Move.Call:
                    NextMoveCallNoStageChange(seatList, lastMovePlayerName, myName,middlePot,sideBetCount,sideBetList);
                    break;
                case Move.Check:
                    NextMoveCheckNoStageChange(seatList, lastMovePlayerName, myName,middlePot,sideBetCount,sideBetList);
                    break;
                case Move.Raise:
                    NextMoveRaiseNoStageChange(seatList, lastMovePlayerName, myName,middlePot,sideBetCount,sideBetList);
                    break;
                case Move.AllIn:
                    NextMoveAllInNoStageChange(seatList, lastMovePlayerName, myName,middlePot,sideBetCount,sideBetList);
                    break;
                case Move.Fold:
                    NextMoveFoldNoStageChange(seatList, lastMovePlayerName, myName,middlePot,sideBetCount,sideBetList);
                    break;
            }
        }

        private void NextMoveFoldNoStageChange(List<Seat> seatList, string lastMovePlayerName, string myName,
            float middlePot, int sideBetCount, List<float> sideBetList)
        {
            int totalMoveTime = TurnManager.instance.TotalMoveTime;
            if (ActiveScene == gameScene)
            {
                seatList = ISeatList.ReArrange(seatList, myName);
                DoNextMoveFold(seatList, myName,lastMovePlayerName);
                GameControllerActivate(seatList, myName);
                CloseHand(seatList, lastMovePlayerName,myName);
                if (string.Equals(myName, lastMovePlayerName))
                {
                    GameControllerDeactivate(seatList);
                }
                GameControllerValuesForMoveFold(seatList, myName, lastMovePlayerName,middlePot,sideBetCount,sideBetList,false);
                TimerAnimationActivate(seatList,0,totalMoveTime);
            }
            if (activeScene == tableScene)
            {
                DoNextMoveFold(seatList, myName,lastMovePlayerName);
                CloseHand(seatList, lastMovePlayerName,myName);
                TimerAnimationActivate(seatList,0,totalMoveTime);
            }
        }

        private void GameControllerValuesForMoveFold(List<Seat> seatList, string myName, string lastMovePlayerName,
            float middlePot, int sideBetCount, List<float> sideBetList, bool isStageChange)
        {
            GameUISpawnManager.instance.SetGameControllerValuesForMoveFold(seatList, myName,lastMovePlayerName, activeScene,middlePot,sideBetCount,sideBetList,smallBlind, isStageChange);
        }


        private void CloseHand(List<Seat> seatList, string lastMovePlayerName, string myName)
        {
            if (activeScene == gameScene)
            {
                GameUISpawnManager.instance.CloseHand(seatList, lastMovePlayerName,myName, activeScene,true);
            }
            if (activeScene == tableScene)
            {
                GameUISpawnManager.instance.CloseHand(seatList, lastMovePlayerName,myName, activeScene,false);
            }
        }

        private void DoNextMoveFold(List<Seat> seatList, string myName, string lastMovePlayerName)
        {
            if (activeScene == gameScene)
            {
                GamePlayerSpawnManager.instance.UpdateOtherPlayers(seatList, myName, activeScene);
                GamePlayerSpawnManager.instance.UpdateLocalPlayer(seatList, myName, activeScene);
            }

            if (activeScene == tableScene)
            {
                GamePlayerSpawnManager.instance.UpdateNonLocalPlayersOnTableScene(seatList, activeScene);
            }
        }

        private void NextMoveAllInNoStageChange(List<Seat> seatList, string lastMovePlayerName, string myName,
            float middlePot, int sideBetCount, List<float> sideBetList)
        {
            int totalMoveTime = TurnManager.instance.TotalMoveTime;
            if (ActiveScene == gameScene)
            {
                seatList = ISeatList.ReArrange(seatList, myName);
                DoNextMoveAllin(seatList, myName,lastMovePlayerName);
                GameControllerActivate(seatList, myName);
                if (string.Equals(myName, lastMovePlayerName))
                {
                    GameControllerDeactivate(seatList);
                }
                BetAmountSetForMoveAllin(seatList, lastMovePlayerName);
                GameControllerValuesForMoveAllin(seatList, myName, lastMovePlayerName,false,middlePot,sideBetCount,sideBetList);
                TimerAnimationActivate(seatList,0,totalMoveTime);
            }
            if (activeScene == tableScene)
            {
                DoNextMoveAllin(seatList, myName,lastMovePlayerName);
                TimerAnimationActivate(seatList,0,totalMoveTime);
            }
        }

        private void GameControllerValuesForMoveAllin(List<Seat> seatList, string myName, string lastMovePlayerName,
            bool isStageChange, float middlePot, int sideBetCount, List<float> sideBetList)
        {
            GameUISpawnManager.instance.SetGameControllerValuesForMoveAllin(seatList, myName,lastMovePlayerName, activeScene,isStageChange,middlePot,sideBetCount,sideBetList,smallBlind);
        }

        private void BetAmountSetForMoveAllin(List<Seat> seatList, string lastMovePlayerName)
        {
            OmahaEngine.instance.SetBetAmountAllin(seatList, lastMovePlayerName);
        }

        private void DoNextMoveAllin(List<Seat> seatList, string myName, string lastMovePlayerName)
        {
            if (activeScene == gameScene)
            {
                GameUISpawnManager.instance.SpawnAllinBetNoStageChangeForSeatedPlayers(seatList,lastMovePlayerName,activeScene);
                GamePlayerSpawnManager.instance.UpdateOtherPlayers(seatList, myName, activeScene);
                GamePlayerSpawnManager.instance.UpdateLocalPlayer(seatList, myName, activeScene);
            }

            if (activeScene == tableScene)
            {
                GameUISpawnManager.instance.SpawnRaiseBetNoStageChangeForObservers(seatList,lastMovePlayerName,activeScene);
                GamePlayerSpawnManager.instance.UpdateNonLocalPlayersOnTableScene(seatList, activeScene);
            }
        }

        private void NextMoveRaiseNoStageChange(List<Seat> seatList, string lastMovePlayerName, string myName,
            float middlePot, int sideBetCount, List<float> sideBetList)
        {
            int totalMoveTime = TurnManager.instance.TotalMoveTime;
            if (ActiveScene == gameScene)
            {
                seatList = ISeatList.ReArrange(seatList, myName);
                DoNextMoveRaise(seatList, myName,lastMovePlayerName);
                GameControllerActivate(seatList, myName);
                BetAmountSetForMoveRaise(seatList, lastMovePlayerName);
                GameControllerValuesForMoveRaise(seatList, myName, lastMovePlayerName,middlePot,sideBetCount,sideBetList);
                TimerAnimationActivate(seatList,0,totalMoveTime);
            }
            if (activeScene == tableScene)
            {
                DoNextMoveRaise(seatList, myName,lastMovePlayerName);
                TimerAnimationActivate(seatList,0,totalMoveTime);
            }
        }

        private void GameControllerValuesForMoveRaise(List<Seat> seatList, string myName, string lastMovePlayerName,
            float middlePot, int sideBetCount, List<float> sideBetList)
        {
            GameUISpawnManager.instance.SetGameControllerValuesForMoveRaise(seatList, myName,lastMovePlayerName, activeScene,middlePot,sideBetCount,sideBetList,smallBlind);
        }

        private void BetAmountSetForMoveRaise(List<Seat> seatList, string lastMovePlayerName)
        {
            OmahaEngine.instance.SetBetAmountRaise(seatList, lastMovePlayerName);
        }

        private void DoNextMoveRaise(List<Seat> seatList, string myName, string lastMovePlayerName)
        {
            if (activeScene == gameScene)
            {
                GameUISpawnManager.instance.SpawnRaiseBetNoStageChangeForSeatedPlayers(seatList,lastMovePlayerName,activeScene);
                GamePlayerSpawnManager.instance.UpdateOtherPlayers(seatList, myName, activeScene);
                GamePlayerSpawnManager.instance.UpdateLocalPlayer(seatList, myName, activeScene);
            }

            if (activeScene == tableScene)
            {
                GameUISpawnManager.instance.SpawnRaiseBetNoStageChangeForObservers(seatList,lastMovePlayerName,activeScene);
                GamePlayerSpawnManager.instance.UpdateNonLocalPlayersOnTableScene(seatList, activeScene);
            }
        }

        private void NextMoveCheckNoStageChange(List<Seat> seatList, string lastMovePlayerName, string myName,
            float middlePot, int sideBetCount, List<float> sideBetList)
        {
            int totalMoveTime = TurnManager.instance.TotalMoveTime;
            if (ActiveScene == gameScene)
            {
                seatList = ISeatList.ReArrange(seatList, myName);
                DoNextMoveCheck(seatList, myName);
                GameControllerActivate(seatList, myName);
                BetAmountSetForMoveCheck(seatList, lastMovePlayerName);
                GameControllerValuesForMoveCheck(seatList, myName, lastMovePlayerName,false,middlePot,sideBetCount,sideBetList);
                TimerAnimationActivate(seatList,0,totalMoveTime);
            }
            if (activeScene == tableScene)
            {
                DoNextMoveCheck(seatList, myName);
                TimerAnimationActivate(seatList,0,totalMoveTime);
            }
        }

        private void NextMoveCallNoStageChange(List<Seat> seatList, string lastMovePlayerName, string myName,
            float middlePot, int sideBetCount, List<float> sideBetList)
        {
            int totalMoveTime = TurnManager.instance.TotalMoveTime;
            if (ActiveScene == gameScene)
            {
                seatList = ISeatList.ReArrange(seatList, myName);
                DoNextMoveCall(seatList, myName,lastMovePlayerName);
                GameControllerActivate(seatList, myName);
                BetAmountSetForMoveCall(seatList, lastMovePlayerName);
                GameControllerValuesForMoveCall(seatList, myName, lastMovePlayerName,false,middlePot,sideBetCount,sideBetList);
                TimerAnimationActivate(seatList,0,totalMoveTime);
            }
            if (activeScene == tableScene)
            {
                DoNextMoveCall(seatList, myName,lastMovePlayerName);
                TimerAnimationActivate(seatList,0,totalMoveTime);
            }
        }
        private void GameControllerValuesForMoveCall(List<Seat> seatList, string myName, string lastMovePlayerName,
            bool isStageChange, float middlePot, int sideBetCount, List<float> sideBetList)
        {
            GameUISpawnManager.instance.SetGameControllerValuesForMoveCall(seatList, myName,lastMovePlayerName, activeScene,isStageChange,middlePot,smallBlind,sideBetCount,sideBetList);
        }

        private void BetAmountSetForMoveCall(List<Seat> seatList, string lastMovePlayerName)
        {
            OmahaEngine.instance.SetBetAmountCall(seatList, lastMovePlayerName);
        }

        private void DoNextMoveCall(List<Seat> seatList,string myName, string lastMovePlayerName)
        {
            if (activeScene == gameScene)
            {
                GameUISpawnManager.instance.SpawnCallBetNoStageChangeForSeatedPlayers(seatList,lastMovePlayerName,activeScene);
                GamePlayerSpawnManager.instance.UpdateOtherPlayers(seatList, myName, activeScene);
                GamePlayerSpawnManager.instance.UpdateLocalPlayer(seatList, myName, activeScene);
            }

            if (activeScene == tableScene)
            {
                GameUISpawnManager.instance.SpawnCallBetNoStageChangeForObservers(seatList,lastMovePlayerName,activeScene);
                GamePlayerSpawnManager.instance.UpdateNonLocalPlayersOnTableScene(seatList, activeScene);
            }
            
        }

        ///////////////////////////////////////////////////TurnMoves Section/////////////////////////////////////////////////////
        public void GameMoveDoAuto(string playerName, float moveAmount, Move move)
        {
            myOmahaEngineScript.DoAutoMove(mySeatList, currentTurnID, playerName, moveAmount, move);
        }

        public List<CardData> GameMoveDoNew(Move move, float moveAmount, string playerName, DBFacade dbFacade)
        {
            List<CardData> middleCards = new List<CardData>();
            ClearPreviousSubTurnValuesIfPreviousOneEnded();
            StartNewSubTurnIfPreviousOneEnded();
            SeatListSetMoveDetails(move, moveAmount, playerName);
            GameMove newGameMove = GameMoveCreateNew(move, moveAmount, playerName);
            UpdateSeatList(newGameMove);
            if (!SubTurnStageIsEnded())
            {
                GiveTurnToNextPlayer();
                CalculateMaxRaiseLimit();
                if (NextPlayerHasPreMove())
                {
                    SetPreMovePlayer();
                }
                MoveTimerStart();
            }
            if (SubTurnStageIsEnded())
            {
                if (isTurnEnded())
                {
                    if (move != Move.HideCards && move != Move.ShowCards)
                    {
                        MakeCorrectionIfNeed();
                        myOmahaEngineScript.CalculatePot(mySeatList);
                        PrepareForTurnEnd();
                        MoveTimerStart();
                        WriteSharedCardsToDB(dbFacade);
                        WriteWinnersToDB(dbFacade);
                        UpdateTurnDetailsToDB(dbFacade);
                        WriteMoveListToDB(dbFacade);
                    }
                    else
                    {
                        Debug.Log("Turn is ended but move is hide or show cards");
                    }
                    
                }
                else
                {
                    //herkesin allin dediği durum
                    if (!hasEnoughPlayers())
                    {
                        middleCards = TurnStageEnd();
                        MakeCorrectionIfNeed();
                        myOmahaEngineScript.CalculatePot(mySeatList);
                        SetGameAnimationTillEndFlag(true);
                        PrepareForAnimationTurnEnd();
                        WriteSharedCardsToDB(dbFacade);
                        WriteWinnersToDB(dbFacade);
                        UpdateTurnDetailsToDB(dbFacade);
                        WriteMoveListToDB(dbFacade);
                    }else
                    {
                        middleCards = TurnStageEnd();
                        myOmahaEngineScript.CalculatePot(mySeatList);
                        GiveTurnToNextPlayer();
                        CalculateMaxRaiseLimit();
                        if (NextPlayerHasPreMove())
                        {
                            SetPreMovePlayer();
                        }
                        MoveTimerStart();
                    }
                }
            }
            return middleCards;
            //SubBetCheckForSubBets();
            //
            //CheckAndUpdateStageStatus();
            //
            //
            //writeMoveListToDB();
        }

        private void CalculateMaxRaiseLimit()
        {
            myOmahaEngineScript.CalculateMaxRaiseLimit(mySeatList);
        }

        private void SetPreMovePlayer()
        {
            myOmahaEngineScript.SetPreMovePlayerName();
        }

        public bool NextPlayerHasPreMove()
        {
            return myOmahaEngineScript.NextPlayerHasPreMove();
        }

        private void StartNewSubTurnIfPreviousOneEnded()
        {
            if (SubTurnStageIsEnded())
            {
                StartNewSubTurn();
            }
        }

        private void PrepareForAnimationTurnEnd()
        {
            
            SetAnimationStartStage();
            List<CardData> sharedCards = GetSharedCards(TurnStage.PreShowDown);
            //sidebet olma durumu henüz ele alınmadı
            List<Tuple<string,List<CardData>>> winnersAndCombinations = DecideWhoWins(sharedCards);
            SetWinners(winnersAndCombinations);
            SetTurnWinners(winnersAndCombinations,currentTurnID);
            MakeRakeBackCalculations();
            //SetReadyForTurnEnd(true);   ?????
            GiveTurnToNoBody();
            
        }

        private List<CardData> GetSharedCards(TurnStage turnStage)
        {
           return myOmahaEngineScript.GetSharedCards(turnStage);
        }

        private void SetAnimationStartStage()
        {
            myOmahaEngineScript.SetAnimationStartStage();
        }

        private void SetTurnStageForAnimation(TurnStage preShowDown)
        {
            myOmahaEngineScript.SetTurnStageForAnimation(preShowDown);
        }


        private void WriteMoveListToDB(DBFacade dbFacade)
        {
            List<GameMove> turnMoves = GetMoveList();
            List<GameMove> turnMovesOrdered = new List<GameMove>();
            turnMovesOrdered = IGameMoveList.OrderMoveListByMoveSeqNo(turnMoves);
            
            for (int i = 0; i < turnMovesOrdered.Count; i++)
            {
                dbFacade.put(turnMovesOrdered[i]);
            }
        }

        private List<GameMove> GetMoveList()
        {
            return myOmahaEngineScript.GetMoveList();
        }

        private void UpdateTurnDetailsToDB(DBFacade dbFacade)
        {
            DateTime turnEndDate = DateTime.Now;
            DateTime turnStartDate = myOmahaEngineScript.GetTurnStartDate();
            float turnDuration = (float) (turnEndDate - turnStartDate).TotalSeconds;
            float totalPot = myOmahaEngineScript.GetTotalPot();
            float totalRakeBack = myOmahaEngineScript.GetTotalRakeBack();
            float totalProfit = myOmahaEngineScript.GetTotalProfit();
            Turn turn = new Turn(currentTurnID,turnEndDate,turnDuration,totalPot,totalRakeBack,totalProfit);
            dbFacade.update(turn);
        }

        private void WriteWinnersToDB(DBFacade dbFacade)
        {
            TurnWinners turnWinners = GetTurnWinners();
            dbFacade.put(turnWinners);
        }

        private TurnWinners GetTurnWinners()
        {
            return myOmahaEngineScript.GetTurnWinners();
        }

        private void WriteSharedCardsToDB(DBFacade dbFacade)
        {
            List<CardData> sharedCardsData = GetSharedCards();
            
            SharedCards sharedCards = new SharedCards(currentTurnID,sharedCardsData);
            dbFacade.put(sharedCards);
        }
        

        private void SetGameAnimationTillEndFlag(bool flag)
        {
            myOmahaEngineScript.SetGameAnimationTillEnd(flag);
        }

        //Oyuna devam edebilecek yeterli player'ın olup olmadığını kontrol eder.
        //Kalan playerların arasında birden fazla all'in dememiş player varsa true döner
        private bool hasEnoughPlayers()
        {
            return ISeatList.HasEnoughPlayersForTurn(mySeatList);
        }

        private void ClearPreviousSubTurnValuesIfPreviousOneEnded()
        {
            if (SubTurnStageIsEnded())
            {
                ClearSubTurnValues();
            }
        }

        private void ClearSubTurnValues()
        {
            mySeatList = ISeatList.ClearAllPlayersSubTurnTotalBets(mySeatList);
            myOmahaEngineScript.ClearSubMoveList();
            myOmahaEngineScript.ClearSubMaxBet();
            
        }

        private void StartNewSubTurn()
        {
            myOmahaEngineScript.StartNewSubTurn(true);
        }
        

        private List<CardData> TurnStageEnd()
        {
            
            List<CardData> middleCards = myOmahaEngineScript.GetMiddleCards();
            mySeatList = myOmahaEngineScript.TurnStageEnd(mySeatList);
            
            return middleCards;
            
        }
        public void CalculatePot()
        {
            myOmahaEngineScript.CalculatePot(mySeatList);
        }

        private void PrepareForTurnEnd()
        {
            
            if (TurnEndisClash())
            {
                
                List<CardData> sharedCards = GetSharedCards();
                //sidebet olma durumu henüz ele alınmadı
                List<Tuple<string,List<CardData>>> winnersAndCombinations = DecideWhoWins(sharedCards);
                SetWinners(winnersAndCombinations);
                SetTurnWinners(winnersAndCombinations,currentTurnID);
                SetReadyForTurnEnd(true);
                //make rakebackCalculations
                MakeRakeBackCalculations();
                GiveTurnToNoBody();
                
            }else
            {
                
                //Bir oyuncu haric herkesin fold dediği oyun sonu durumunu ifade eder
                Debug.Log("Turn end is not clash");
                List<Tuple<string,List<CardData>>> winnersAndCombinations = SetWinnerIsLastPlayer();
                SetWinners(winnersAndCombinations);
                SetTurnWinners(winnersAndCombinations,currentTurnID);
                SetReadyForTurnEnd(true);
                MakeRakeBackCalculations();
                GiveTurnToWinner();
                
            }
        }

        public void MakeCorrectionIfNeed()
        {
            if (isNeedCorrection())
            {
                Debug.Log("Correction is needed");
                MakeCorrection();
            }
        }

        private void MakeRakeBackCalculations()
        {
            myOmahaEngineScript.MakeRakeBackCalculations();
        }

        private void MakeCorrection()
        {
            //BALANCE İLE İLGİLİ BİR SIKINTI VAR PLAYER'İN BALANCI AZALMAYIP ARTIYOR
            myOmahaEngineScript.MakeCorrection();
            float returnAmount = GetCorrectionMoveAmount();
            string playerName = GetCorrectionMovePlayerName();
            mySeatList = ISeatList.UpdateSeatVarAddBalanceByPlayerName(mySeatList, playerName, returnAmount);

        }

        public string GetCorrectionMovePlayerName()
        {
            return myOmahaEngineScript.GetCorrectionMovePlayerName();
        }

        private float GetCorrectionMoveAmount()
        {
            return myOmahaEngineScript.GetCorrectionMoveAmount();
        }

        public bool isNeedCorrection()
        {
            GameMove correctionMove = myOmahaEngineScript.GetCorrectionMove(); 
            if (correctionMove != null)
            {
                return true;
            }
            return false;
        }

        private void SetTurnWinners(List<Tuple<string, List<CardData>>> winnersAndCombinations, int currentTurnID)
        {
            myOmahaEngineScript.SetTurnWinners(winnersAndCombinations, currentTurnID);
        }

        private void SetWinners(List<Tuple<string, List<CardData>>> winnersAndCombinations)
        {
            myOmahaEngineScript.SetWinners(winnersAndCombinations);
        }

        private void GiveTurnToNoBody()
        {
            mySeatList = ISeatList.GiveTurnToNoBody(mySeatList);
        }

        private void GiveTurnToWinner()
        {
            string winnerName = ISeatList.GetLastPlayer(mySeatList);
            mySeatList = ISeatList.SetSeatVarIsMyTurnJustForWinner(mySeatList, winnerName);
            myOmahaEngineScript.GiveTurnToWinner(winnerName, mySeatList);
        }

        private List<Tuple<string, List<CardData>>> SetWinnerIsLastPlayer()
        {
            List<Tuple<string, List<CardData>>> winnersAndCombinations = new List<Tuple<string, List<CardData>>>();
            string winnerName = ISeatList.GetLastPlayer(mySeatList);
            List<CardData> winnerCombination = GetLastPlayerHand(winnerName);
            Tuple<string, List<CardData>> winnerAndCombination = new Tuple<string, List<CardData>>(winnerName, winnerCombination);
            winnersAndCombinations.Add(winnerAndCombination);
            return winnersAndCombinations;
        }

        private List<CardData> GetLastPlayerHand(string winnerName)
        {
            return myOmahaEngineScript.GetPlayerCards(winnerName);
        }

        public bool TurnEndisClash()
        {
            TurnEndType  turnEndType =  myOmahaEngineScript.GetTurnEndType();
            if (turnEndType == TurnEndType.Clash)
            {
                return true;
            }
            return false;
        }

        private void SetReadyForTurnEnd(bool condition)
        {
            myOmahaEngineScript.SetReadyForTurnEnd(condition);
        }

        private List<CardData> GetSharedCards()
        {
            return myOmahaEngineScript.SharedCards();
        }

        private List<Tuple<string, List<CardData>>> DecideWhoWins(List<CardData> sharedCards)
        {
            return myOmahaEngineScript.DecideWhoWins(mySeatList, sharedCards);
        }

        public bool isTurnEnded()
        {
            return myOmahaEngineScript.IsTurnEnded(mySeatList);
        }

        private void UpdateSeatList(GameMove newGameMove)
        {
            mySeatList = ISeatList.UpdateSeatVarBalanceByGameMove(mySeatList,newGameMove);
        }

        private void MoveTimerStart()
        {
            myOmahaEngineScript.MoveTimerStart();
        }

        private void GiveTurnToNextPlayer()
        {
            mySeatList = myOmahaEngineScript.GiveTurnToNextPlayer(mySeatList);
        }

        public bool SubTurnStageIsEnded()
        {
            return MyOmahaEngineScript.SubTurnStageIsEnded();
        }

        private GameMove GameMoveCreateNew(Move move, float moveAmount, string playerName)
        {
            GameMove newGameMove = GameMoveCreateNew(playerName, move, moveAmount);
            return newGameMove;
        }

        private GameMove GameMoveCreateNew(string playerName, Move lastMove, float moveAmount)
        {
            return myOmahaEngineScript.GameMoveCreateNew(mySeatList, playerName, currentTurnID, lastMove, moveAmount);
        }

        ///////////////////////////////////////////////////CheckMoveLegit Section/////////////////////////////////////////////////////
        public bool MoveIsLegit(Move myNextMove, float moveAmount, string playerName)
        {
            return myOmahaEngineScript.MoveCheckMoveIsLegit(mySeatList, myNextMove, moveAmount, playerName);
        }
        
        ///////////////////////////////////////////////////SubBet Section////////////////////////////////////////////////////

        ///////////////////////////////////////////////////TurnStage Section/////////////////////////////////////////////////////
        public void TurnStageCheck(Move lastMove)
        {


        }
        

        ///////////////////////////////////////////////////TurnMovesCheckOrFold Section/////////////////////////////////////////////////////
        public bool DecideCheckOrFoldReturnTrueIfItsCheck()
        {
            return myOmahaEngineScript.DecideCheckOrFoldReturnTrueIfItsCheck();
        }

        ///////////////////////////////////////////////////TimeSpan Section/////////////////////////////////////////////////////
        public int TimeSpanGet()
        {
            return myOmahaEngineScript.GetTimeSpan();
        }

        ///////////////////////////////////////////////////SubMoveList Section/////////////////////////////////////////////////////
        public List<GameMove> GetSubMoveList()
        {
            return myOmahaEngineScript.GetSubMoveList();
        }

        ///////////////////////////////////////////////////StartTurn Section/////////////////////////////////////////////////////
        public void StartTurn()
        {
            mySeatList = myOmahaEngineScript.StartTurn(mySeatList, currentTurnID);
        }

        ///////////////////////////////////////////////////GiveTurntoPlayer Section/////////////////////////////////////////////////////
        public void GiveTurnToPlayer()
        {
            mySeatList = myOmahaEngineScript.GiveTurntoPlayer(mySeatList);
        }

        ///////////////////////////////////////////////////TurnInitializeNew Section/////////////////////////////////////////////////////
        public void TurnInitializeNew()
        {
            isEndGame = false;
            stopCurrentCoroutine = false;
            myOmahaEngineScript.SetIsEndGame(false);
            List<string> playerNames = myGamePlayerManagerScript.GamePlayerList;
            mySeatList = myOmahaEngineScript.InitializeNewTurn(mySeatList, playerNames);

        }

        public bool GetIsTurnEnd()
        {
            return isEndGame;
        }

        public bool IsEndGame
        {
            get => isEndGame;
            set => isEndGame = value;
        }
        ///////////////////////////////////////////////////CreateTurnParts Section/////////////////////////////////////////////////////

        public Turn TurnCreateNew()
        {
            Turn newTurn = myOmahaEngineScript.CreateNewTurn(syncID, smallBlind);
            return newTurn;
        }

        public TurnPlayers TurnPlayersCreateNew()
        {
            TurnPlayers newTurnPlayers = myOmahaEngineScript.CreateNewTurnPlayers(currentTurnID);
            return newTurnPlayers;
        }

        ///////////////////////////////////////////////////DealCards Section/////////////////////////////////////////////////////
        public void TargetBuildTurnForObserverSuccess(List<Seat> seatList, string playerName, int timeSpan)
        {
            TurnBuild(seatList, playerName, timeSpan);

        }

        private void TurnBuild(List<Seat> seatList, string playerName, int timeSpan)
        {
            int totalMoveTime = TurnManager.instance.TotalMoveTime;
            DealerTokenSpawn(seatList);
            CardBacksSpawn(seatList, playerName);
            DoSmallAndBigBlind(seatList, playerName);
            TimerAnimationActivate(seatList, timeSpan, totalMoveTime);
        }

        private void DoSmallAndBigBlind(List<Seat> seatList, string playerName)
        {
            if (activeScene == gameScene)
            {
                GameUISpawnManager.instance.SpawnSmallBlindBetForSeatedPlayers(seatList, activeScene);
                GameUISpawnManager.instance.SpawnBigBlindBetForSeatedPlayers(seatList, activeScene);
                GamePlayerSpawnManager.instance.UpdateOtherPlayers(seatList, playerName, activeScene);
                GamePlayerSpawnManager.instance.UpdateLocalPlayer(seatList, playerName, activeScene);
            }

            if (activeScene == tableScene)
            {
                GameUISpawnManager.instance.SpawnSmallBlindBetForObservers(seatList, activeScene);
                GameUISpawnManager.instance.SpawnBigBlindBetForObservers(seatList, activeScene);
                GamePlayerSpawnManager.instance.UpdateNonLocalPlayersOnTableScene(seatList, activeScene);
            }
        }

        public void TargetBuildTurnForSeatedPlayersSucces(int[] mySuitArr, int[] myValueArr, List<Seat> seatList,
            string username, int timeSpan)
        {
            TurnBuild(mySuitArr, myValueArr, seatList, username, timeSpan);

        }

        private void TurnBuild(int[] mySuitArr, int[] myValueArr, List<Seat> seatList, string username, int timeSpan)
        {
            int totalMoveTime = TurnManager.instance.TotalMoveTime;
            List<CardData> myHand = ListCardDataBuildMyHand(mySuitArr, myValueArr);
            seatList = ISeatList.ReArrange(seatList, username);
            DealerTokenSpawn(seatList);
            ListCardDataSpawnMyHand(myHand, seatList);
            CardBacksSpawn(seatList, username);
            DoSmallAndBigBlind(seatList, username);
            GameControllerActivate(seatList, username);
            
            BetAmountSetForSmallAndBigBlind(seatList);
            GameControllerValuesSetForSmallAndBigBlind(seatList, username);
            TimerAnimationActivate(seatList, timeSpan, totalMoveTime);
            
        }

        private void BetAmountSetForSmallAndBigBlind(List<Seat> seatList)
        {
            OmahaEngine.instance.SetBetAmountForSmallAndBigBlind(seatList);
        }

        private void GameControllerValuesSetForSmallAndBigBlind(List<Seat> seatList, string username)
        {
            GameUISpawnManager.instance.SetGameControllerValuesForSmallAndBigBlind(seatList, username, activeScene, smallBlind);
        }

        private void TimerAnimationActivate(List<Seat> seatList, int timeSpan, int totalMoveTime)
        {
            if (activeScene == gameScene)
            {
                OmahaEngine.instance.ActivateTimerAnimation(seatList, activeScene, true, timeSpan, totalMoveTime);
            }

            if (activeScene == tableScene)
            {
                OmahaEngine.instance.ActivateTimerAnimation(seatList, activeScene, false, timeSpan, totalMoveTime);
            }
        }

        private void GameControllerActivate(List<Seat> seatList, string username)
        {
            GameUISpawnManager.instance.ActivateGameController(seatList, username, activeScene);
        }

        private List<CardData> ListCardDataBuildMyHand(int[] mySuitArr, int[] myValueArr)
        {
            List<CardData> myHand = Dealer.instance.BuildMyHand(mySuitArr, myValueArr);
            myHand = Dealer.instance.SortMyHandHighToLow(myHand);
            return myHand;
        }

        public void ListCardDataSpawnMyHand(List<CardData> myHand, List<Seat> seatList)
        {
            GameUISpawnManager.instance.SpawnMyHand(myHand, seatList.Count, gameScene);
        }

        public void CardBacksSpawn(List<Seat> seatList, string username)
        {
            if (activeScene == gameScene)
            {
                GameUISpawnManager.instance.SpawnCardBacksForOtherSeatedPlayers(seatList, username, activeScene);
            }

            if (activeScene == tableScene)
            {
                GameUISpawnManager.instance.SpawnCardBacksForObservers(seatList, activeScene);
            }
        }

        ///////////////////////////////////////////////////LeaveTable Section/////////////////////////////////////////////////////
        public void LeaveTableAreaOpenCloseLeaveTableArea(bool area, string msg, bool openConfirmationButton, bool okButton,
            bool cancelButton)
        {
            if (seatCount == 6)
            {
                if (activeScene == tableScene)
                {
                    if (area)
                    {
                        activeScene.GetComponent<SixPlayerTableScene>().DepositArea.SetActive(false);
                    }

                    activeScene.GetComponent<SixPlayerTableScene>()
                        .OpenCloseLeaveTableArea(area, msg, openConfirmationButton, okButton, cancelButton);
                }

                if (activeScene == gameScene)
                {
                    if (area)
                    {
                        activeScene.GetComponent<SixPlayerGameScene>().DepositArea.SetActive(false);
                    }

                    activeScene.GetComponent<SixPlayerGameScene>()
                        .OpenCloseLeaveTableArea(area, msg, openConfirmationButton, okButton, cancelButton);
                }
            }

            if (seatCount == 8)
            {
                if (activeScene == tableScene)
                {
                    if (area)
                    {
                        activeScene.GetComponent<EightPlayerTableScene>().DepositArea.SetActive(false);
                    }

                    activeScene.GetComponent<EightPlayerTableScene>()
                        .OpenCloseLeaveTableArea(area, msg, openConfirmationButton, okButton, cancelButton);
                }

                if (activeScene == gameScene)
                {
                    if (area)
                    {
                        activeScene.GetComponent<EightPlayerGameScene>().DepositArea.SetActive(false);
                    }

                    activeScene.GetComponent<EightPlayerGameScene>()
                        .OpenCloseLeaveTableArea(area, msg, openConfirmationButton, okButton, cancelButton);
                }
            }
        }

        ///////////////////////////////////////////////////JoinGame Section/////////////////////////////////////////////////////
        public void PlayerAndRakePercentMapAdd(string dummyUserUsername, float rakePercent)
        {
            myOmahaEngineScript.AddPlayerToPlayerAndRakePercentMap(dummyUserUsername, rakePercent);
        }

        public void PlayerAndParentRakePercentMapAdd(string username, float parentPercent)
        {
            myOmahaEngineScript.AddPlayerToPlayerAndParentRakePercentMap(username, parentPercent);
        }

        public void PlayerAndParentMapAdd(string username, string parent)
        {
            myOmahaEngineScript.AddPlayerToPlayerAndParentMap(username, parent);
        }

        public void joinGame(int deposit)
        {
            Player.localPlayer.JoinGame(requestedSeat, deposit);
        }

        public void JoinGameSuccesForLocalPlayer(string localPlayerName, List<Seat> seatList,
            PlayerGameState playerGameState, int timeSpan)
        {
            OpenCloseDepositOptionsWithMessage(false, "giriş başarılı", true, true);
            SitButtonSetInterectable(true);
            
            GameBuild(seatList, localPlayerName, timeSpan, playerGameState);

        }
        private void DealerTokenSpawn(List<Seat> seatList)
        {
            if (activeScene == gameScene)
            {
                GameUISpawnManager.instance.SpawnDealerTokenForSeatedPlayers(seatList, activeScene);
            }

            if (activeScene == tableScene)
            {
                GameUISpawnManager.instance.SpawnDealerTokenForObservers(seatList, activeScene);
            }
        }
        public void JoinGameSuccesForNonLocalPlayers(string newPlayerName, List<Seat> seatList)
        {
            PlayerSpawnNewPlayer(newPlayerName, seatList);
            
        }
        
        private void PlayerSpawnNewPlayer(string newPlayerName, List<Seat> seatList)
        {
            if (activeScene == gameScene)
            {
                seatList = ISeatList.ReArrange(seatList, PlayerGetLocalPlayerName(seatList));
                GamePlayerSpawnManager.instance.SpawnNewPlayerForGameScene(newPlayerName, seatList, gameScene);
            }

            if (activeScene == tableScene)
            {

                GamePlayerSpawnManager.instance.SpawnNewPlayerForTableScene(newPlayerName, seatList, activeScene);
            }
        }

        private string PlayerGetLocalPlayerName(List<Seat> seatList)
        {
            if (seatList.Count == 6)
            {
                return SixPlayerGameScene.instance.LocalPlayer.GetComponent<LocalPlayer>().PlayerNameText.text;
            }

            if (seatList.Count == 8)
            {
                //return EightPlayerGameScene.instance.LocalPlayer.GetComponent<LocalPlayer>().PlayerNameText.text;
            }

            return string.Empty;
        }

        public void JoinGameFail(string msg)
        {
            OpenCloseDepositOptionsWithMessage(true, msg, false, true);
        }

        private void PlayerSpawnOtherPlayers(List<Seat> seatList, string localPlayerName, GameObject gameScene)
        {
            GamePlayerSpawnManager.instance.SpawnOtherPlayers(seatList, localPlayerName, gameScene);
        }

        private void PlayerSpawnLocalPlayer(string newPlayerName, List<Seat> seatList)
        {
            GamePlayerSpawnManager.instance.SpawnLocalPlayer(seatList, newPlayerName, gameScene);
        }

        public void SitButtonSetActivity(bool value)
        {
            if (seatCount == 6)
            {
                if (activeScene == tableScene)
                {
                    activeScene.GetComponent<SixPlayerTableScene>().SetSitButtonsActivity(value);
                }

                if (activeScene == gameScene)
                {
                    activeScene.GetComponent<SixPlayerGameScene>().SetSitButtonsActivity(value);
                }
            }

            if (seatCount == 8)
            {
                if (activeScene == tableScene)
                {
                    //activeScene.GetComponent<EightPlayerTableScene>().SitButtonSetActivity(value);
                }

                if (activeScene == gameScene)
                {
                    //activeScene.GetComponent<EightPlayerGameScene>().SitButtonSetActivity(value);
                }
            }
        }

        public void SitButtonSetInterectable(bool value)
        {
            if (seatCount == 6)
            {
                if (activeScene == tableScene)
                {
                    activeScene.GetComponent<SixPlayerTableScene>().SetSitButtonsInteractable(value);
                }

                if (activeScene == gameScene)
                {
                    activeScene.GetComponent<SixPlayerGameScene>().SetSitButtonsInteractable(value);
                }
            }

            if (seatCount == 8)
            {
                if (activeScene == tableScene)
                {
                    //activeScene.GetComponent<EightPlayerTableScene>().SitButtonSetInterectable(value);
                }

                if (activeScene == gameScene)
                {
                    //activeScene.GetComponent<EightPlayerGameScene>().SitButtonSetInterectable(value);
                }
            }
        }



        ///////////////////////////////////////////////////OpenDepositOptions Section/////////////////////////////////////////////////////
        private void OpenCloseDepositOptionsWithMessage(bool depositArea, string msg, bool sitButton, bool cancelButton)
        {
            if (seatCount == 6)
            {
                if (activeScene == tableScene)
                {
                    if (depositArea)
                    {
                        activeScene.GetComponent<SixPlayerTableScene>().LeaveTableArea.SetActive(false);
                    }
                    activeScene.GetComponent<SixPlayerTableScene>().DepositArea.SetActive(depositArea);
                    activeScene.GetComponent<SixPlayerTableScene>().DepositInformationText.text = msg;
                    activeScene.GetComponent<SixPlayerTableScene>().DepositSitButton.interactable = sitButton;
                    activeScene.GetComponent<SixPlayerTableScene>().DepositCancelButton.interactable = cancelButton;
                    activeScene.GetComponent<SixPlayerTableScene>().DepositInputField.text = String.Empty;
                }

                if (activeScene == gameScene)
                {
                    if (depositArea)
                    {
                        activeScene.GetComponent<SixPlayerGameScene>().LeaveTableArea.SetActive(false);
                    }
                    activeScene.GetComponent<SixPlayerGameScene>().DepositArea.SetActive(depositArea);
                    activeScene.GetComponent<SixPlayerGameScene>().DepositInformationText.text = msg;
                    activeScene.GetComponent<SixPlayerGameScene>().DepositSitButton.interactable = sitButton;
                    activeScene.GetComponent<SixPlayerGameScene>().DepositCancelButton.interactable = cancelButton;
                    activeScene.GetComponent<SixPlayerGameScene>().DepositInputField.text = String.Empty;
                }

            }

            if (seatCount == 8)
            {
                if (activeScene == tableScene)
                {
                    if (depositArea)
                    {
                        activeScene.GetComponent<EightPlayerTableScene>().LeaveTableArea.SetActive(false);
                    }

                    activeScene.GetComponent<EightPlayerTableScene>().DepositArea.SetActive(depositArea);
                    activeScene.GetComponent<EightPlayerTableScene>().DepositInformationText.text = msg;
                    activeScene.GetComponent<EightPlayerTableScene>().DepositSitButton.interactable = sitButton;
                    activeScene.GetComponent<EightPlayerTableScene>().DepositCancelButton.interactable = cancelButton;
                    activeScene.GetComponent<EightPlayerTableScene>().DepositInputField.text = String.Empty;
                }

                if (activeScene == gameScene)
                {
                    if (depositArea)
                    {
                        activeScene.GetComponent<EightPlayerGameScene>().LeaveTableArea.SetActive(false);
                    }

                    activeScene.GetComponent<EightPlayerGameScene>().DepositArea.SetActive(depositArea);
                    activeScene.GetComponent<EightPlayerGameScene>().DepositInformationText.text = msg;
                    activeScene.GetComponent<EightPlayerGameScene>().DepositSitButton.interactable = sitButton;
                    activeScene.GetComponent<EightPlayerGameScene>().DepositCancelButton.interactable = cancelButton;
                    activeScene.GetComponent<EightPlayerGameScene>().DepositInputField.text = String.Empty;
                }
            }
        }

        public void DepositOptionsOpenSucces(float minDeposit, SeatLocations requestedSeat)
        {
            string msg = "Minimum giriş : ";
            msg = string.Concat(msg, minDeposit.ToString());
            this.requestedSeat = requestedSeat;
            OpenCloseDepositOptionsWithMessage(true, msg, true, true);
        }

        public void DepositOptionsOpenFailErrorSeatTaken()
        {
            OpenCloseDepositOptionsWithMessage(true, errorSeatTakenString, false, true);
        }

        ///////////////////////////////////////////////////JoinTable Section/////////////////////////////////////////////////////
        public void TableJoinSucces(List<Seat> seatList, string localPlayerName, int timeSpan,
            PlayerGameState playerGameState, GameState gameState)
        {

            //Spawn TableScene 
            TableSceneAndGameSceneSpawn(seatList);
            IEnumerator coroutine = waitForSpawnTableScene(seatList, localPlayerName, timeSpan, playerGameState, gameState);
            StartCoroutine(coroutine);

        }
        
        private IEnumerator waitForSpawnTableScene(List<Seat> seatList, string localPlayerName, int timeSpan,
            PlayerGameState playerGameState, GameState gameState)
        {
            yield return new WaitForSeconds(TimeManager.instance.SpawnTableAndGameSceneDelayTime);
            if (gameState == GameState.Playing)
            {
                GameBuild(seatList, localPlayerName, timeSpan, playerGameState);
            }
            else
            {
                ReadyForGameBuild(seatList, localPlayerName, timeSpan, playerGameState);
            }
            yield return null;
        }

        private void ReadyForGameBuild(List<Seat> seatList, string localPlayerName, int timeSpan, PlayerGameState playerGameState)
        {
            if (playerGameState == PlayerGameState.Observer)
            {
                TableSceneActivate();
                SitButtonSetActivity(true);
                seatList = ClearForNewTurn(seatList, localPlayerName, false);
            }
            if (playerGameState == PlayerGameState.InGame)
            {
                GameSceneActivate();
                SitButtonSetActivity(true);
                seatList = ClearForNewTurn(seatList, localPlayerName, true);
                
            }
        }

        private void GameBuild(List<Seat> seatList, string localPlayerName, int timeSpan,
            PlayerGameState playerGameState)
        {
            int totalMoveTime = TurnManager.instance.TotalMoveTime;
            if (playerGameState == PlayerGameState.Observer)
            {
                TableSceneActivate();
                seatList = ClearForNewTurn(seatList, localPlayerName, false);
                SitButtonSetActivity(true);
                //PlayerSpawnNonLocalPlayers(seatList);
                
                CardBacksSpawn(seatList, localPlayerName);
                DealerTokenSpawn(seatList);
                DoSmallAndBigBlind(seatList, localPlayerName);
                DoBets(seatList, localPlayerName);
                TimerAnimationActivate(seatList, timeSpan, totalMoveTime);
            }

            if (playerGameState == PlayerGameState.InGame)
            {
                GameSceneActivate();
                seatList = ClearForNewTurn(seatList, localPlayerName, true);
                
                SitButtonSetActivity(true);
                //PlayerSpawnLocalPlayer(localPlayerName, seatList);
                PlayerSpawnOtherPlayers(seatList, localPlayerName, gameScene);
                
                DealerTokenSpawn(seatList);
                CardBacksSpawn(seatList, localPlayerName);
                //DoSmallAndBigBlind(seatList, localPlayerName);
                DoBets(seatList, localPlayerName);
                TimerAnimationActivate(seatList, timeSpan, totalMoveTime);
            }
        }

        

        private void DoBets(List<Seat> seatList, string playerName)
        {
            if (activeScene == gameScene)
            {
                GameUISpawnManager.instance.SpawnBetsForSeatedPlayers(seatList, activeScene);
                GamePlayerSpawnManager.instance.UpdateOtherPlayers(seatList, playerName, activeScene);
                GamePlayerSpawnManager.instance.UpdateLocalPlayer(seatList, playerName, activeScene);
            }

            if (activeScene == tableScene)
            {
                GameUISpawnManager.instance.SpawnBetsForObserverPlayers(seatList, activeScene);
                GamePlayerSpawnManager.instance.UpdateNonLocalPlayersOnTableScene(seatList, activeScene);
            }
        }

        private void PlayerSpawnNonLocalPlayers(List<Seat> seatList)
        {
            if (activeScene == tableScene)
            {
                GamePlayerSpawnManager.instance.SpawnNonLocalPlayersOnTableScene(seatList, activeScene);
            }

        }

        public void TableSceneAndGameSceneSpawn(List<Seat> seatList)
        {
            if (tableScene == null && gameScene == null)
            {
                if (seatList.Count == 6)
                {
                    tableScene = GameUISpawnManager.instance.SpawnSixPlayerScene();
                    gameScene = GameUISpawnManager.instance.SpawnSixPlayerGameScene();
                }

                if (seatList.Count == 8)
                {
                    tableScene = GameUISpawnManager.instance.SpawnEightPlayerScene();
                    gameScene = GameUISpawnManager.instance.SpawnEightPlayerGameScene();
                }
            }
        }

        public void TableSceneActivate()
        {
            inActiveScene = gameScene;
            activeScene = tableScene;
            gameScene.SetActive(false);
            tableScene.SetActive(true);
            activeUI = activeScene.name;
        }

        public void GameSceneActivate()
        {
            inActiveScene = tableScene;
            activeScene = gameScene;
            gameScene.SetActive(true);
            tableScene.SetActive(false);
            activeUI = activeScene.name;
        }


        ///////////////////////////////////////////////////PlayerList Section/////////////////////////////////////////////////////

        public bool PlayerListAdd(string playerName)
        {
            if (myPlayerList.Contains(playerName))
            {
                Debug.LogWarning("TableGameManager.cs -->>> SyncID : " + SyncID + "-->>>PlayerListAdd" +
                                 "Player is already exist in PlayerList!!! : PlayerName -> " + playerName);
                return false;
            }
            else
            {
                myPlayerList.Add(playerName);
                return true;
            }

        }

        public bool PlayerListRemove(string playerName)
        {
            if (myPlayerList.Contains(playerName))
            {
                myPlayerList.Remove(playerName);
                return true;
            }
            else
            {
                Debug.LogWarning("TableGameManager.cs -->>> SyncID : " + SyncID + "-->>>PlayerListRemove" +
                                 "Player does NOT exist in PlayerList!!! : PlayerName -> " + playerName);
                return false;
            }
        }

        public bool PlayerListContain(string playerName)
        {
            if (myPlayerList.Contains(playerName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        ///////////////////////////////////////////////////GameState Section/////////////////////////////////////////////////////
        public void GameStateSet(GameState gameState)
        {
            myGameState = gameState;
        }

        ///////////////////////////////////////////////////Seat Section/////////////////////////////////////////////////////
        public Seat SeatCreateNew(string playerName, SeatLocations seatLocations, float depositAmount)
        {
            Seat newSeat = new Seat();
            newSeat.username = playerName;
            newSeat.location = seatLocations;
            newSeat.balance = depositAmount;
            newSeat.isActive = true;
            newSeat.isMyTurn = false;
            newSeat.isDealer = false;
            newSeat.moveTime = 0;
            newSeat.lastMove = Move.None;
            newSeat.lastMoveAmount = 0;
            newSeat.totalBetInSubTurn = 0;
            newSeat.isPlayerMovedInSubTurn = false;
            return newSeat;
        }
        ///////////////////////////////////////////////////ISeatList Section/////////////////////////////////////////////////////
        private void SeatListSetMoveDetails(Move myNextMove, float moveAmount, string playerName)
        {
            mySeatList = myOmahaEngineScript.SeatListSetMoveDetails(mySeatList, myNextMove, moveAmount, playerName);
        }
        public void UpdateSeatByLocation(Seat newSeat)
        {
            mySeatList = ISeatList.UpdateSeatByLocation(newSeat, mySeatList);
           
        }
        public bool ContainSeatByUsername(string playerName)
        {
            return ISeatList.ContainSeatByUsername(playerName, mySeatList);
        }
        
        public bool isSeatActiveByLocation(SeatLocations seatLocation)
        {
            return ISeatList.isSeatActiveByLocation(seatLocation, mySeatList);
        }
        
        ///////////////////////////////////////////////////InitialSettings Section/////////////////////////////////////////////////////
        public void SeatSummonByCount(int seatCount)
        {
            mySeatList = ISeat.SummonSeatByCount(seatCount, mySeatList);
        }

        public void TableToolsSet(GameObject tablePlayerManager, GameObject gamePlayerManager,
            GameObject gamePlayerSpawnManager, GameObject gameUISpawnManager, GameObject omahaEngine)
        {
            myTablePlayerManagerScript = tablePlayerManager.GetComponent<TablePlayerManager>();
            myGamePlayerManagerScript = gamePlayerManager.GetComponent<GamePlayerManager>();
            myGamePlayerSpawnManagerScript = gamePlayerSpawnManager.GetComponent<GamePlayerSpawnManager>();
            myGameUISpawnManagerScript = gameUISpawnManager.GetComponent<GameUISpawnManager>();
            myOmahaEngineScript = omahaEngine.GetComponent<OmahaEngine>();
            OmahaEngineSet(smallBlind, minDeposit, tableRakePercent);

        }

        private void OmahaEngineSet(float smallBlind, float minDeposit, float tableRakePercent)
        {
            myOmahaEngineScript.SmallBlind = smallBlind;
            myOmahaEngineScript.MinDeposit = minDeposit;
            myOmahaEngineScript.TableRakePercent = tableRakePercent;
        }

        ///////////////////////////////////////////////////Properties Section/////////////////////////////////////////////////////


        public int CurrentTurnID
        {
            get => currentTurnID;
            set => currentTurnID = value;
        }

        public TablePlayerManager MyTablePlayerManagerScript => myTablePlayerManagerScript;

        public GamePlayerManager MyGamePlayerManagerScript => myGamePlayerManagerScript;

        public GamePlayerSpawnManager MyGamePlayerSpawnManagerScript => myGamePlayerSpawnManagerScript;

        public GameUISpawnManager MyGameUISpawnManagerScript => myGameUISpawnManagerScript;


        public OmahaEngine MyOmahaEngineScript => myOmahaEngineScript;

        public GameState MyGameState
        {
            get => myGameState;
            set => myGameState = value;
        }

        public GameObject ActiveScene
        {
            get => activeScene;
            set => activeScene = value;
        }

        public GameObject TableScene
        {
            get => tableScene;
            set => tableScene = value;
        }

        public GameObject GameScene
        {
            get => gameScene;
            set => gameScene = value;
        }

        public List<Seat> MySeatList
        {
            get => mySeatList;
            set => mySeatList = value;
        }

        public List<string> MyPlayerList
        {
            get => myPlayerList;
            set => myPlayerList = value;
        }

        public string SyncID
        {
            get => syncID;
            set => syncID = value;
        }

        public int SeatCount
        {
            get => seatCount;
            set => seatCount = value;
        }

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


        public List<GameMove> GameMoveListGet()
        {
            return myOmahaEngineScript.GameMoveListGet();
        }

        


        

        public float GetMainBet()
        {
            return myOmahaEngineScript.GetMainBet();
        }


        public List<CardData> GetPlayerCards(string playerName)
        {
            return myOmahaEngineScript.GetPlayerCards(playerName);
        }

        public List<Tuple<string, List<CardData>>> GetWinnerAndCombinations()
        {
            return myOmahaEngineScript.GetWinnerAndCombinations();
        }

        public float GetWinAmount()
        {
            return myOmahaEngineScript.GetWinAmount(tableRakePercent);
        }

        public List<string> GetWinnerListOrderByInGamePlayerList(List<string> inGamePlayerList, List<Tuple<string, List<CardData>>> winnerAndCombinations)
        {
            return myOmahaEngineScript.GetWinnerListOrderByInGamePlayerList(inGamePlayerList, winnerAndCombinations);
        }

        public List<List<CardData>> GetWinnerCombinationsOrderByInGamePlayerList(List<string> inGamePlayerList, List<Tuple<string, List<CardData>>> winnerAndCombinations)
        {
            return myOmahaEngineScript.GetWinnerCombinationsOrderByInGamePlayerList(inGamePlayerList, winnerAndCombinations);
        }

        public void AnimateClash(List<Seat> seatList, string myName, List<string> inGamePlayerList,
            List<List<CardData>> inGamePlayersCards, List<string> winnerList, List<List<CardData>> winnerCombinations,
            List<float> winAmountList)
        {
            if (ActiveScene == gameScene)
            {
                seatList = ISeatList.ReArrange(seatList, myName);
                OpenAllInGamePlayerHands(seatList, inGamePlayerList, inGamePlayersCards,true);
                GameControllerDeactivate(seatList);
                ShakeWinningCombinations(seatList, winnerList, winnerCombinations,inGamePlayerList,inGamePlayersCards,true);
                MoveChipsToWinner(seatList,myName, winnerList, winAmountList,true);
                
                
            }
            if (activeScene == tableScene)
            {
                OpenAllInGamePlayerHands(seatList, inGamePlayerList, inGamePlayersCards,false);
                ShakeWinningCombinations(seatList, winnerList, winnerCombinations,inGamePlayerList,inGamePlayersCards,false);
                MoveChipsToWinner(seatList,myName, winnerList, winAmountList,false);
            }
        }

        private void MoveChipsToWinner(List<Seat> seatList, string myName, List<string> winnerList, List<float> winAmountList,
            bool isGameScene)
        {
            if (isGameScene)
            {
                GameUISpawnManager.instance.MoveChipsToWinner(seatList, winnerList, winAmountList,activeScene,isGameScene);
                GamePlayerSpawnManager.instance.UpdateOtherPlayers(seatList, myName, activeScene);
                GamePlayerSpawnManager.instance.UpdateLocalPlayer(seatList, myName, activeScene);
            }
            else
            {
                GameUISpawnManager.instance.MoveChipsToWinner(seatList, winnerList, winAmountList,activeScene,isGameScene);
                GamePlayerSpawnManager.instance.UpdateNonLocalPlayersOnTableScene(seatList,activeScene);
            }
            
        }
        private void MoveChipsToWinner(List<Seat> seatList, string myName, string winner, float winAmount,
            bool isGameScene)
        {
            if (isGameScene)
            {
                GameUISpawnManager.instance.MoveChipsToWinner(seatList, winner, winAmount,activeScene,isGameScene);
                GamePlayerSpawnManager.instance.UpdateOtherPlayers(seatList, myName, activeScene);
                GamePlayerSpawnManager.instance.UpdateLocalPlayer(seatList, myName, activeScene);
            }
            else
            {
                GameUISpawnManager.instance.MoveChipsToWinner(seatList, winner, winAmount,activeScene,isGameScene);
                GamePlayerSpawnManager.instance.UpdateNonLocalPlayersOnTableScene(seatList,activeScene);
            }
            
        }

        private void ShakeWinningCombinations(List<Seat> seatList, List<string> winnerList,
            List<List<CardData>> winnerCombinations, List<string> inGamePlayerList,
            List<List<CardData>> inGamePlayersCards, bool isGameScene)
        {
            GameUISpawnManager.instance.ShakeWinningCombinations(seatList, winnerList, winnerCombinations,inGamePlayerList,inGamePlayersCards,activeScene, isGameScene);
        }

        private void OpenAllInGamePlayerHands(List<Seat> seatList, List<string> inGamePlayerList,
            List<List<CardData>> inGamePlayersCards, bool isGameScene)
        {
            GameUISpawnManager.instance.OpenAllInGamePlayerHands(seatList, inGamePlayerList, inGamePlayersCards,activeScene,isGameScene);
        }

        private void GameControllerDeactivate(List<Seat> seatList)
        {
            GameUISpawnManager.instance.DeactivateGameController(seatList, activeScene);
        }

        public bool isReadyForTurnEnd()
        {
            return myOmahaEngineScript.isReadyForTurnEnd();
        }

        public void DealCards()
        {
            myOmahaEngineScript.DealCards(mySeatList, currentTurnID);
        }

        public void AddWinAmountsToPlayers(List<string> winnerList, List<float> winnerPotList)
        {
            mySeatList = myOmahaEngineScript.AddWinAmountsToPlayers(mySeatList,winnerList, winnerPotList);
        }

        public List<Seat> ClearForNewTurn(List<Seat> seatList, string myName, bool isPlayerInGame)
        {
            if (isPlayerInGame)
            {
                seatList = ISeatList.ReArrange(seatList, myName);
                PlayerSpawnLocalPlayer(myName, seatList);
                GameControllerDeactivate(seatList);
                ClearSharedCards(seatList, isPlayerInGame);
                ClearAllCards(seatList, isPlayerInGame);
                ClearAllBets(seatList, isPlayerInGame);
                HideDealerToken(seatList, isPlayerInGame);
                ClearAllSideBets(seatList, isPlayerInGame);
                
            }
            if (!isPlayerInGame)
            {
                PlayerSpawnNonLocalPlayers(seatList);
                ClearSharedCards(seatList, isPlayerInGame);
                ClearAllCards(seatList, isPlayerInGame);
                ClearAllBets(seatList, isPlayerInGame);
                HideDealerToken(seatList, isPlayerInGame);
                ClearAllSideBets(seatList, isPlayerInGame);
            }

            return seatList;
        }

        private void ClearAllSideBets(List<Seat> seatList, bool isPlayerInGame)
        {
            GameUISpawnManager.instance.ClearAllSideBets(seatList, activeScene, isPlayerInGame);
        }

        private void ClearSharedCards(List<Seat> seatList, bool isPlayerInGame)
        {
            GameUISpawnManager.instance.ClearSharedCards(seatList, activeScene, isPlayerInGame);
        }

        private void HideDealerToken(List<Seat> seatList, bool isPlayerInGame)
        {
            GameUISpawnManager.instance.HideDealerToken(seatList, activeScene, isPlayerInGame);
        }

        private void ClearAllBets(List<Seat> seatList, bool isPlayerInGame)
        {
            GameUISpawnManager.instance.ClearAllBets(seatList, activeScene, isPlayerInGame);
        }

        private void ClearAllCards(List<Seat> seatList, bool isPlayerInGame)
        {
            GameUISpawnManager.instance.ClearAllCards(seatList, activeScene, isPlayerInGame);
            
        }


        public void ResetLastMoves()
        {
            mySeatList = ISeatList.ResetLastMoves(mySeatList);
        }

        public string GetWinnerName()
        {
            return myOmahaEngineScript.GetWinnerName();
        }

        public List<CardData> GetWinnerHand(string winnerName)
        {
            return myOmahaEngineScript.GetWinnerHand(winnerName);
        }

        public void ShowCards(List<Seat> seatList, string myName, bool isPlayerInGame, string winnerName, float winAmount, List<CardData> winnerCards, string SyncID)
        {
            if (ActiveScene == gameScene)
            {
                seatList = ISeatList.ReArrange(seatList, myName);
                OpenWinnerHand(seatList, winnerName, winnerCards,myName,isPlayerInGame);
                MoveChipsToWinner(seatList,myName, winnerName, winAmount,isPlayerInGame);
            }
            if (activeScene == tableScene)
            {
                OpenWinnerHand(seatList, winnerName, winnerCards,myName,isPlayerInGame);
                MoveChipsToWinner(seatList,myName, winnerName, winAmount,isPlayerInGame);
            }
        }

        private void OpenWinnerHand(List<Seat> seatList, string winnerName, List<CardData> winnerCards, string myName,
            bool isGameScene)
        {
            if (isGameScene)
            {
                if (!string.Equals(myName, winnerName))
                {
                    GameUISpawnManager.instance.OpenWinnerHand(seatList, winnerName, winnerCards,activeScene,isGameScene);
                }
            }
            else
            {
                GameUISpawnManager.instance.OpenWinnerHand(seatList, winnerName, winnerCards,activeScene,isGameScene);
            }
        }

        public void CorrectionMove(List<Seat> seatList, string correctionPlayerName,
            float correctionBetAmount, string myName, bool isGameScene)
        {
            if (ActiveScene == gameScene)
            {
                seatList = ISeatList.ReArrange(seatList, myName);
                MakeCorrectionBet(seatList, correctionPlayerName,correctionBetAmount, isGameScene);
                UpdatePlayer(seatList, correctionPlayerName, myName, isGameScene);
            }
            if (activeScene == tableScene)
            {
                MakeCorrectionBet(seatList, correctionPlayerName,correctionBetAmount, isGameScene);
                UpdatePlayer(seatList, correctionPlayerName, myName, isGameScene);
            }
            
        }

        private void MakeCorrectionBet(List<Seat> seatList, string correctionPlayerName,
            float correctionBetAmount, bool isGameScene)
        {
            GameUISpawnManager.instance.MakeCorrectionBet(seatList, correctionPlayerName,correctionBetAmount, activeScene, isGameScene);
        }

        private void UpdatePlayer(List<Seat> seatList, string correctionPlayerName, string myName, bool isGameScene)
        {
            if (isGameScene)
            {
                if (string.Equals(myName, correctionPlayerName))
                {
                    GamePlayerSpawnManager.instance.UpdateLocalPlayer(seatList,myName,activeScene);
                }
                else
                {
                    GamePlayerSpawnManager.instance.UpdateOtherPlayers(seatList,myName,activeScene);
                }
            }
            else
            {
                GamePlayerSpawnManager.instance.UpdateNonLocalPlayersOnTableScene(seatList, activeScene);
            }
            {
                
            }
        }

        public float GetCorrectionBetAmountForClientSide()
        {
            return myOmahaEngineScript.GetCorrectionBetAmountForClientSide();
        }

        public bool AnimationTillEnd()
        {
            return myOmahaEngineScript.GetAnimationTillEnd();
        }

        

        public TurnStage GetAnimationStartStage()
        {
            return myOmahaEngineScript.GetAnimationStartStage();
        }


        public CardData GetTurnCard()
        {
            return myOmahaEngineScript.GetTurnCard();
        }

        public void SetTurnCard(CardData turnCard, int seatCount)
        {
            if (ActiveScene == gameScene)
            {
                AnimateTurnCard(turnCard,seatCount,true);
                
            }
            if (activeScene == tableScene)
            {
                AnimateTurnCard(turnCard,seatCount,false);
            }
        }

        private void AnimateTurnCard(CardData turnCard, int seatCount, bool isGameScene)
        {
            GameUISpawnManager.instance.AnimateTurnCard(turnCard,seatCount, activeScene, isGameScene);
        }

        public CardData GetRiverCard()
        {
            return myOmahaEngineScript.GetRiverCard();
        }

        public void SetRiverCard(CardData riverCard, int seatCount)
        {
            if (ActiveScene == gameScene)
            {
                AnimateRiverCard(riverCard,seatCount,true);
                
            }
            if (activeScene == tableScene)
            {
                AnimateRiverCard(riverCard,seatCount,false);
            }
        }

        private void AnimateRiverCard(CardData riverCard, int seatCount, bool isGameScene)
        {
            GameUISpawnManager.instance.AnimateRiverCard(riverCard,seatCount, activeScene, isGameScene);
        }

        public bool AddGameParticipationIdToMap(int gameParticipationID, string username)
        {
            bool isOkey =  myGamePlayerManagerScript.AddPlayerToPlayerAndGameParticipationIDMap(username, gameParticipationID);
            return isOkey;
        }

        public int GetGameParticipationIdByPlayerName(string playerName)
        {
            return myGamePlayerManagerScript.GetGameParticipationId(playerName);
        }

        public float GetPlayerBalance(string playerName)
        {
            return ISeatList.GetBalanceByPlayerName(mySeatList,playerName);
        }

        public float GetPlayerRakeBackAmount(string userUsername)
        {
            return myOmahaEngineScript.GetPlayerRakeBackAmount(userUsername);
        }

        public string GetPlayerParentName(string playerName)
        {
            return myOmahaEngineScript.GetPlayerParentName(playerName);
        }

        public float GetPlayerParentRakeBackAmount(string playerName)
        {
            return myOmahaEngineScript.GetPlayerParentRakeBackAmount(playerName);
        }

        public int GetPlayerSeatNumber(string playerName)
        {
            return (int) ISeatList.GetSeatLocationByPlayerName(mySeatList,playerName);
        }

        public void RemovePlayerFromSeatAndPlayerMap(int seatNumber)
        {
            myGamePlayerManagerScript.RemovePlayerFromSeatAndPlayerMap(seatNumber);
        }

        public void RemovePlayerFromGamePlayerList(string playerName)
        {
            myGamePlayerManagerScript.RemovePlayerFromGamePlayerList(playerName);
        }

        public void AddPlayerToObserverList(string playerName)
        {
            myTablePlayerManagerScript.AddPlayerToObserverList(playerName);
        }

        public void RemovePlayerFromPlayerAndParentMap(string playerName)
        {
            myOmahaEngineScript.RemovePlayerFromPlayerAndParentMap(playerName);
        }

        public void RemovePlayerFromPlayerAndParentRakePercentMap(string playerName)
        {
            myOmahaEngineScript.RemovePlayerFromPlayerAndParentRakePercentMap(playerName);
        }

        public void RemoveParentFromParentAndParentRakeAmountMap(string playerName)
        {
            myOmahaEngineScript.RemoveParentFromParentAndParentRakeAmountMap(playerName);
        }

        public void RemovePlayerFromPlayerAndPlayerRakePercentMap(string playerName)
        {
            myOmahaEngineScript.RemovePlayerFromPlayerAndPlayerRakePercentMap(playerName);
        }

        public void RemovePlayerFromPlayerAndPlayerRakeAmountMap(string playerName)
        {
            myOmahaEngineScript.RemovePlayerFromPlayerAndPlayerRakeAmountMap(playerName);
        }

        public List<Seat> GetSeatList()
        {
            return mySeatList;
        }

        public void SwapTableSceneFromGameSceneSucces(string playerName, List<Seat> seatList, int timeSpan)
        {
            SwapTableSceneFromGameScene(seatList, playerName, timeSpan);
        }

        private void SwapTableSceneFromGameScene(List<Seat> seatList, string playerName, int timeSpan)
        {
            //seatList = ISeatList.ReArrange(seatList, playerName);
            TableSceneActivate();
            GameBuild(seatList, playerName, timeSpan, PlayerGameState.Observer);
        }
        
        private void RemoveDealerTokenFromGameScene(List<Seat> seatList)
        {
            GamePlayerSpawnManager.instance.RemoveDealerTokenFromGameScene(seatList, activeScene);
        }

        private void RemoveCardBacksFromGameScene(List<Seat> seatList, string playerName)
        {
            GamePlayerSpawnManager.instance.RemoveCardBacksFromGameScene(seatList, playerName, activeScene);
        }

        private void RemoveNonLocalPlayersFromGameScene(List<Seat> seatList)
        {
            GamePlayerSpawnManager.instance.RemoveNonLocalPlayersFromGameScene(seatList, activeScene);
        }

        private void RemoveLocalPlayerFromGameScene(string playerName, List<Seat> seatList)
        {
            GamePlayerSpawnManager.instance.RemoveLocalPlayerFromGameScene(playerName, seatList, activeScene);
        }

        

        public List<string> GetObserverList()
        {
            return myTablePlayerManagerScript.GetObserverList();
        }

        public void PlayerLeaveGameSucces(string leavedPlayerName, List<Seat> newSeatList, string myName, List<Seat> oldSeatList, bool inGameScene)
        {
            oldSeatList = ISeatList.ReArrange(oldSeatList, myName);
            RemovePlayerSeatFromScene(leavedPlayerName, oldSeatList, inGameScene);
            //newSeatList = ISeatList.ReArrange(newSeatList, myName);
            
        }

        private void RemovePlayerSeatFromScene(string leavedPlayerName, List<Seat> oldSeatList, bool inGameScene)
        {
            GamePlayerSpawnManager.instance.RemovePlayerSeatFromScene(leavedPlayerName, oldSeatList, inGameScene, activeScene);
        }

        public void AddPlayerToLeaverPlayerQueue(string playerName)
        {
            myGamePlayerManagerScript.AddPlayerToLeaverPlayerQueue(playerName);
        }

        public bool isToggleMoveLegit(ToggleMove toggleMove, bool toggleValue, float moveAmount, string playerName)
        {
            bool isLegit = false;
            switch (toggleMove)
            {
                case ToggleMove.FoldCheck:
                     isLegit = isFoldCheckLegit(toggleValue, playerName);break;
                case ToggleMove.Check:
                    isLegit = isCheckLegit(toggleValue, playerName);break;
                case ToggleMove.Call:
                    isLegit = isCallLegit(toggleValue, moveAmount, playerName);break;
            }
            return isLegit;
        }

        private bool isCallLegit(bool toggleValue, float moveAmount, string playerName)
        {
            bool isLegit = false;
            bool isExist = false;
            if (toggleValue)
            {
                isExist = myOmahaEngineScript.isToggleMoveExistInPlayerToggleMoveTupleList(playerName, ToggleMove.Call);
                bool isMaxBet = isMaxBetInSubTurn(playerName);
                bool isCallAmountCorrect = myOmahaEngineScript.isCallAmountCorrect(playerName, moveAmount,mySeatList);
                isLegit = !isExist && !isMaxBet && isCallAmountCorrect;
            }
            else
            {
                isExist = myOmahaEngineScript.isToggleMoveExistInPlayerToggleMoveTupleList(playerName, ToggleMove.Call);
                isLegit = isExist;
            }
            return isLegit;
        }

        private bool isMaxBetInSubTurn(string playerName)
        {
            return ISeatList.GetPlayerNameWhoHasMaxTotalBetInSubTurn(mySeatList) == playerName;
            
        }

        private bool isCheckLegit(bool toggleValue, string playerName)
        {
            bool isLegit = false;
            bool isExist = false;
            bool canPlayerSayCheck = myOmahaEngineScript.IsPlayerAllowedToSayCheck(playerName, mySeatList);
            if (toggleValue)
            {
                isExist = myOmahaEngineScript.isToggleMoveExistInPlayerToggleMoveTupleList(playerName, ToggleMove.Check);
                isLegit = !isExist && canPlayerSayCheck;
            }
            else
            {
                isExist = myOmahaEngineScript.isToggleMoveExistInPlayerToggleMoveTupleList(playerName, ToggleMove.Check);
                isLegit = isExist;
            }
            return isLegit;
        }

        private bool isFoldCheckLegit(bool toggleValue, string playerName)
        {
            bool isLegit = false;
            bool isExist = false;
            if (toggleValue)
            {
                isExist = myOmahaEngineScript.isToggleMoveExistInPlayerToggleMoveTupleList(playerName, ToggleMove.FoldCheck);
                isLegit = !isExist;
            }
            else
            {
                isExist = myOmahaEngineScript.isToggleMoveExistInPlayerToggleMoveTupleList(playerName, ToggleMove.FoldCheck);
                isLegit = isExist;
            }
            return isLegit;
        }

        public void AddToggleMoveToTheList(ToggleMove toggleMove, float moveAmount, string playerName)
        {
            myOmahaEngineScript.AddToggleMoveToTheList(toggleMove, moveAmount, playerName);
        }

        public void RemoveToggleMoveFromTheList(string playerName)
        {
            myOmahaEngineScript.RemoveToggleMoveFromTheList(playerName);
        }

        public string GetCurrentHasTurnPlayerName()
        {
            return myOmahaEngineScript.GetCurrentHasTurnPlayerName();
        }

        public ToggleMove GetToggleMoveOfPlayer(string preMovePlayerName)
        {
            return myOmahaEngineScript.GetToggleMoveOfPlayer(preMovePlayerName);
        }

        public float GetToggleMoveAmountOfPlayer(string preMovePlayerName)
        {
            return myOmahaEngineScript.GetToggleMoveAmountOfPlayer(preMovePlayerName);
        }

        public Move GeneratePreMoveFromToggleMove(ToggleMove toggleMove, string preMovePlayerName)
        {
            return myOmahaEngineScript.GeneratePreMoveFromToggleMove(toggleMove, preMovePlayerName, mySeatList);
        }

        public void ToggleMoveIsDone(string myName, ToggleMove toggleMove, List<Seat> seatList)
        {
            GameUISpawnManager.instance.ToggleMoveIsDone(myName, toggleMove, seatList,activeScene);
        }

        public void RemovePreMovePlayerName()
        {
            myOmahaEngineScript.RemovePreMovePlayerName();
        }

        public bool HasPreMove()
        {
            return myOmahaEngineScript.HasPreMove();
        }

        public int GetTurnTime()
        {
            return myOmahaEngineScript.GetTurnTime();
        }

        public int GetTotalMoveTimeForEndGame()
        {
            return myOmahaEngineScript.GetTotalMoveTimeForEndGame();
        }

        public void SetEndGame(bool isEndGame)
        {
            this.isEndGame = isEndGame;
            myOmahaEngineScript.SetIsEndGame(isEndGame);
        }

        public string GetCurrentHasTurnPlayer()
        {
            return myOmahaEngineScript.GetCurrentHasTurnPlayer();
        }

        public void HideEndGameButtons(int seatCount)
        {
            GameUISpawnManager.instance.HideEndGameButtons(seatCount,activeScene);
        }

        public bool CheckPlayerIsInGame(string playerName)
        {
            return ISeatList.CheckIsPlayerInGame(mySeatList, playerName);
        }

        public void ClearSeatFromSeatList(string playerName)
        {
            mySeatList = ISeatList.ClearSeatFromSeatListByPlayerName(mySeatList, playerName);
        }

        public void SetActiveSeatLocations()
        {
            myOmahaEngineScript.SetActiveSeatLocations(mySeatList);
        }

        public void RemovePlayerFromGameParticipationIDMap(string playerName)
        {
            MyGamePlayerManagerScript.RemovePlayerFromGameParticipationIDMap(playerName);
        }

        public TurnEndType GetTurnEndType()
        {
            return myOmahaEngineScript.GetTurnEndType();
        }


        public bool GetReadyForTurnEnd()
        {
            return myOmahaEngineScript.GetReadyForTurnEnd();
        }

        public bool GetAnimationTillEndFlag()
        {
            return myOmahaEngineScript.GetAnimationTillEndFlag();
        }

        public bool IsPlayerOnLeaverQueue(string playerName)
        {
            return myGamePlayerManagerScript.IsPlayerOnLeaverQueue(playerName);
        }

        public List<string> GetLeaverQueuePlayerList()
        {
            return myGamePlayerManagerScript.GetLeaverQueuePlayerList();
        }

        public void SetWaitForPlayerMoveCoroutine(IEnumerator waitForPlayerMove)
        {
            waitForPlayerMoveCoroutine = waitForPlayerMove;
        }

        public IEnumerator GetWaitForPlayerMoveCoroutine()
        {
            return waitForPlayerMoveCoroutine;
        }
        public bool isWaitForPlayerCoroutineNULL()
        {
            return waitForPlayerMoveCoroutine == null;
        }

        public bool GetIsStopCoroutine()
        {
            return stopCurrentCoroutine;
        }

        public bool StopCurrentCoroutine
        {
            get => stopCurrentCoroutine;
            set => stopCurrentCoroutine = value;
        }

        public void SetPlayerTimeOut(bool value)
        {
            myOmahaEngineScript.SetPlayerTimeOut(value);
        }

        public bool GetIsPlayerTimeOut()
        {
            return myOmahaEngineScript.GetIsPlayerTimeOut();
        }

        public void SetMoveTimeCalculate(bool value)
        {
            myOmahaEngineScript.SetMoveTimeCalculate(value);
        }

        public void ParentAndParentRakeAmountMapAdd(string dummyUserParent, float amount)
        {
            myOmahaEngineScript.ParentAndParentRakeAmountMapAdd(dummyUserParent, amount);
        }

        public void PlayerAndRakeAmountMapAdd(string username, float amount)
        {
            myOmahaEngineScript.PlayerAndRakeAmountMapAdd(username, amount);
        }

        public string CoroutineMaster
        {
            get => coroutineMaster;
            set => coroutineMaster = value;
        }

        public string LeaverPlayer
        {
            get => leaverPlayer;
            set => leaverPlayer = value;
        }

        public bool IsCoroutineNeedTransfer
        {
            get => isCoroutineNeedTransfer;
            set => isCoroutineNeedTransfer = value;
        }

        public bool IsCoroutineTransferCompleted
        {
            get => isCoroutineTransferCompleted;
            set => isCoroutineTransferCompleted = value;
        }

        public bool IsCoroutineTransferNeeded(string playerName)
        {
            if(string.Equals(playerName, coroutineMaster))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool GetIsCoroutineTransferCompleted()
        {
            return isCoroutineTransferCompleted;
        }

        public string GetAnotherPlayerName(string playerName)
        {
            return myGamePlayerManagerScript.GetAnotherPlayerName(playerName);
        }

        public bool IsPlayerCorotuineMaster(string playerName)
        {
            if (string.Equals(playerName, coroutineMaster))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public string GetCoroutineMaster()
        {
            return coroutineMaster;
        }

        public bool CheckPlayerIsOnLeaverQueue(string currentHasTurnPlayer)
        {
            return myGamePlayerManagerScript.CheckPlayerIsOnLeaverQueue(currentHasTurnPlayer);
        }

        public string GetAnotherPlayer(string player)
        {
            return myGamePlayerManagerScript.GetAnotherPlayerName(player);
        }

        public float GetSubMaxBet()
        {
            return myOmahaEngineScript.GetSubMaxBet();
        }

        public float GetPlayerSubMaxBet(string playerName)
        {
            return myOmahaEngineScript.GetPlayerSubMaxBet(playerName);
        }

        public void RemovePlayerFromLeaverPlayerQueue(string playerName)
        {
            myGamePlayerManagerScript.RemovePlayerFromLeaverPlayerQueue(playerName);
        }


        public int GetSideBetCount()
        {
            return myOmahaEngineScript.GetSideBetCount();
        }

        public float GetSideBet(int index)
        {
            return myOmahaEngineScript.GetSideBet(index);
        }
        

        public List<float> GetWinnerPotListOrderByInGamePlayerList(List<string> winnerList)
        {
            return myOmahaEngineScript.GetWinnerPotListOrderByInGamePlayerList(winnerList);
        }

        public void MoveChipsToPotSucces(List<Seat> seatList, string myName, float middlePot, int sideBetCount,
            List<float> sideBetList)
        {
            if (ActiveScene == gameScene)
            {
                seatList = ISeatList.ReArrange(seatList, myName);
                SetMiddlePot(middlePot,sideBetCount,sideBetList);
                ClearPreviousSubTurnBets(seatList);
                
            }
            if (activeScene == tableScene)
            {
                SetMiddlePot(middlePot,sideBetCount,sideBetList);
                ClearPreviousSubTurnBets(seatList);
            }
        }

        public void PrepareSmallAndBigBlindGODataByDealer()
        {
            myOmahaEngineScript.PrepareSmallAndBigBlindGODataByDealer(mySeatList);
        }
    }
    

    public enum GameState
    {
        Pending,
        Ready,
        Playing
    }
}