using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class SixPlayerGameScene : MonoBehaviour
    {
        ///////////////////////////////////////////////////Variables Section/////////////////////////////////////////////////////
        public static SixPlayerGameScene instance;
        private int deposit = 0;
        [Header("DepositArea")]
        [SerializeField] private GameObject depositArea;
        [SerializeField] private TMP_InputField depositInputField;
        [SerializeField] private TMP_Text depositInformationText;
        [SerializeField] private Button depositSitButton;
        [SerializeField] private Button depositCancelButton;
        [Header("LeaveTable")]
        [SerializeField] private GameObject leaveTableArea;
        [SerializeField] private TMP_Text leaveTableInformationText;
        [SerializeField] private Button leaveTableOpenConfirmationButton;
        [SerializeField] private Button leaveTableOkButton;
        [SerializeField] private Button leaveTableCancelButton;
        [Header("LocalPlayer")]
        [SerializeField] private GameObject localPlayer;
        [SerializeField] private GameObject localPlayerHand;
        [SerializeField] private GameObject bottomRightDealerToken;
        [SerializeField] private GameObject bottomRightBet;
        [Header("CardBacks")]
        [SerializeField] private GameObject leftCardBack;
        [SerializeField] private GameObject rightCardBack;
        [SerializeField] private GameObject topLeftCardBack;
        [SerializeField] private GameObject topRightCardBack;
        [SerializeField] private GameObject bottomLeftCardBack;
        [Header("SitButtons")]
        [SerializeField] private GameObject bottomLeftSitButton;
        [SerializeField] private GameObject leftSitButton;
        [SerializeField] private GameObject topLeftSitButton;
        [SerializeField] private GameObject topRightSitButton;
        [SerializeField] private GameObject rightSitButton;
        [Header("NlPlayers")]
        [SerializeField] private GameObject bottomLeftNLPlayer;
        [SerializeField] private GameObject leftNLPlayer;
        [SerializeField] private GameObject topLeftNLPlayer;
        [SerializeField] private GameObject topRightNLPlayer;
        [SerializeField] private GameObject rightNLPlayer;
        [Header("DealerTokens")]
        [SerializeField] private GameObject bottomLeftDealerToken;
        [SerializeField] private GameObject leftDealerToken;
        [SerializeField] private GameObject topLeftDealerToken;
        [SerializeField] private GameObject topRightDealerToken;
        [SerializeField] private GameObject rightDealerToken; 
        [Header("Bets")]
        [SerializeField] private GameObject bottomLeftBet;
        [SerializeField] private GameObject leftBet;
        [SerializeField] private GameObject topLeftBet;
        [SerializeField] private GameObject topRightBet;
        [SerializeField] private GameObject rightBet;
        [Header("Hands")]
        [SerializeField] private GameObject bottomLeftHand;
        [SerializeField] private GameObject leftHand;
        [SerializeField] private GameObject topLeftHand;
        [SerializeField] private GameObject topRightHand;
        [SerializeField] private GameObject rightHand;
        [Header("FlopCards")]
        [SerializeField] private GameObject flopCard1;
        [SerializeField] private GameObject flopCard2;
        [SerializeField] private GameObject flopCard3;
        [SerializeField] private GameObject turnCard;
        [SerializeField] private GameObject riverCard;
        [Header("Pot")]
        [SerializeField] private GameObject potButton;
        [SerializeField] private GameObject sideBet1Button;
        [SerializeField] private GameObject sideBet2Button;
        [SerializeField] private GameObject sideBet3Button;
        [SerializeField] private GameObject sideBet4Button;
        [Header("GameController")]
        [SerializeField] private GameObject gameController;

        private SeatLocations animationLocation;
        
        private IDictionary<int, GameObject> cardBackMap = new Dictionary<int, GameObject>();
        private IDictionary<int, GameObject> sitButtonMap = new Dictionary<int, GameObject>();
        private IDictionary<int, GameObject> nLPlayerMap = new Dictionary<int, GameObject>();
        private IDictionary<int, GameObject> dealerTokenMap = new Dictionary<int, GameObject>();
        private IDictionary<int, GameObject> betMap = new Dictionary<int, GameObject>();
        private IDictionary<int, GameObject> flopCardMap = new Dictionary<int, GameObject>();
        private IDictionary<int, GameObject> handMap = new Dictionary<int, GameObject>();
        private IDictionary<int, GameObject> sideBetMap = new Dictionary<int, GameObject>();
        
        ///////////////////////////////////////////////////Events Section/////////////////////////////////////////////////////
        private void Awake()
        {
            setPotToZero();
            initiateAllObjectMaps();
            setDefaultVisibiltyValues();
        }
        private void Start()
        {
            instance = this;
            
        }
        ///////////////////////////////////////////////////AnimateTurnTimer Section/////////////////////////////////////////////////////
       
        

        ///////////////////////////////////////////////////AnimateTurnTimer Section/////////////////////////////////////////////////////
        public void AnimateTurnTimer(SeatLocations animationSeatLocation, int remainingTime, int totalMoveTime,
            GameObject activeScene, List<Seat> seatList)
        {
            if (animationSeatLocation == SeatLocations.BottomRight)
            {
                localPlayer.GetComponent<LocalPlayer>().AnimateTurnTimer(remainingTime,animationSeatLocation,totalMoveTime,activeScene,seatList);
                
            }
            else
            {
                nLPlayerMap[(int)animationSeatLocation].GetComponent<nonLocalPlayer>().AnimateTurnTimer(remainingTime,animationSeatLocation,totalMoveTime);
            }
            
        }

        private bool isShowCardsEnabled()
        {
            return gameController.GetComponent<GameController>().isShowCardsEnabled();
        }

        ///////////////////////////////////////////////////GameController Section/////////////////////////////////////////////////////
        public void SetGameControllerValuesForMoveCall(List<Seat> seatList, string myName, string lastMovePlayerName,
            bool isMyTurn, bool isStageChange, float middlePot, float smallBlind, int sideBetCount,
            List<float> sideBetList)
        {
            if (isMyTurn)
            {
                gameController.GetComponent<GameController>().SetMovesCanvasValuesForMoveCall(seatList, myName, lastMovePlayerName,isStageChange,middlePot,smallBlind,sideBetCount,sideBetList);
                
            }
            else
            {
                gameController.GetComponent<GameController>().SetPreMovesCanvasValuesForMoveCall(seatList, myName, lastMovePlayerName);
                
            }
        }
        public void ActivateGameController(bool isMyTurn)
        {
            GameController.SetActive(true);
            if (isMyTurn)
            {
                 
                gameController.GetComponent<GameController>().ActivateDeactivateMovesCanvas(isMyTurn);
                
            }
            else
            {
                
                gameController.GetComponent<GameController>().ActivateDeactivatePreMovesCanvas(!isMyTurn);
            }   
            
        }
        
        public void SetGameControllerValuesForSmallAndBigBlind(string playerName, List<Seat> seatList,
            bool isMyTurn, float smallBlind)
        {
            if (isMyTurn)
            {
                gameController.GetComponent<GameController>().SetMovesCanvasValuesForSmall(playerName, seatList,smallBlind);
                
            }
            else
            {
                gameController.GetComponent<GameController>().SetPreMovesCanvasValuesForBig(playerName, seatList);
                
            }
        }
        ///////////////////////////////////////////////////SpawnBet Section/////////////////////////////////////////////////////
        public void SpawnBet(SeatLocations seatLocation, float amount)
        {
            betMap[(int) seatLocation].SetActive(true);
            betMap[(int) seatLocation].GetComponent<Bet>().SetBetAmount(amount);
        }
        ///////////////////////////////////////////////////SpawnDealerToken Section/////////////////////////////////////////////////////
        public void SpawnDealerToken(int seatLocation)
        {
            foreach (var dealerToken in dealerTokenMap.Values)
            {
                dealerToken.SetActive(false);
            }
            dealerTokenMap[seatLocation].SetActive(true);
        }
         ///////////////////////////////////////////////////LeaveTable Section/////////////////////////////////////////////////////
        public void OpenCloseLeaveTableArea(bool area,string msg,bool openConfirmationButton,bool okButton,bool cancelButton)
        {
            leaveTableArea.SetActive(area);
            leaveTableInformationText.text = msg;
            leaveTableOpenConfirmationButton.interactable = openConfirmationButton;
            leaveTableOkButton.interactable = okButton;
            leaveTableCancelButton.interactable = cancelButton;
        }
        
        /*
         * LeaveTableOpenConfirmationButton onClick eventine bağlıdır.
         */
        public void openLeaveTableConfirmation()
        {
            leaveTableOpenConfirmationButton.interactable = false;
            OpenCloseLeaveTableArea(true,"Masadan Ayrıl",false,true,true);
            depositArea.SetActive(false);
            SetSitButtonsInteractable(false);
        }
        
        /*
         * leaveTableOkButton onClick eventine bağlıdır.
         */
        public void leaveTable()
        {
            leaveTableOkButton.interactable = false;
            OpenCloseLeaveTableArea(false,"Masadan Ayrıl",true,false,false);
            Player.localPlayer.LeaveTable();
        }
        
        /*
         * leaveTableCancelButton onClick eventine bağlıdır
         */
        public void cancelLeaveTable()
        {
            leaveTableCancelButton.interactable = false;
            OpenCloseLeaveTableArea(false,"Masadan Ayrıl",true,true,false);
            SetSitButtonsInteractable(true);
        }
        ///////////////////////////////////////////////////DepositArea Section/////////////////////////////////////////////////////

        /*
         * DepositArea'nın depositSitButton onClick eventine bağlıdır.
         */
        public void depositSit()
        {
            depositSitButton.interactable = false;
            depositCancelButton.interactable = false;
            deposit = Convert.ToInt32(DepositInputField.text);
            TableGameManager.instance.joinGame(deposit);
            depositInputField.text = String.Empty;
        }
        
        /*
         * DepositArea'nın depositCancelButton onClick eventine bağlıdır.
         */
        public void depositCancel()
        {
            depositCancelButton.interactable = false;
            depositArea.SetActive(false);
            
        }
        
        ///////////////////////////////////////////////////İnitialSettings Section/////////////////////////////////////////////////////
        private void initiateAllObjectMaps()
        {
            InitiateCardBackMap();
            InitiateSitButtonMap();
            InitiateNLPlayerMap();
            InitiateDealerTokenMap();
            InitiateBetMap();
            InitiateHandMap();
            InitiateFlopCardMap();
            InitiateSideBetMap();
        }

        private void setDefaultVisibiltyValues()
        {
            depositArea.SetActive(false);
            leaveTableArea.SetActive(false);
            SetSitButtonsActivity(true);
            SetSitButtonsInteractable(true);
            setCardBacksActivity(false);
            setNLPlayersActivity(false);
            setDealerTokensActivity(false);
            setBetsActivity(false);
            setFlopCardsActivity(false);
            setHandsActivity(false);
            setSideBetsActivity(false);
        }

        private void setSideBetsActivity(bool value)
        {
            foreach (var sideBet in sideBetMap.Values)
            {
                sideBet.SetActive(value);
            }
        }

        private void setHandsActivity(bool value)
        {
            foreach (var hand in handMap.Values)
            {
                hand.SetActive(value);
            }
        }

        private void setFlopCardsActivity(bool value)
        {
            foreach (var flopCard in flopCardMap.Values)
            {
                flopCard.SetActive(value);
            }
        }

        private void setBetsActivity(bool value)
        {
            foreach (var bet in betMap.Values)
            {
                bet.SetActive(value);
            }
        }

        private void setDealerTokensActivity(bool value)
        {
            foreach (var dealerToken in dealerTokenMap.Values)
            {
                dealerToken.SetActive(value);
            }
        }

        private void setNLPlayersActivity(bool value)
        {
            foreach (var nLPlayer in nLPlayerMap.Values)
            {
                nLPlayer.SetActive(value);
            }
        }

        private void setPotToZero()
        {
            potButton.GetComponentInChildren<TMP_Text>().text = "0";
            sideBet1Button.GetComponentInChildren<TMP_Text>().text = "0";
            sideBet2Button.GetComponentInChildren<TMP_Text>().text = "0";
            sideBet3Button.GetComponentInChildren<TMP_Text>().text = "0";
            sideBet4Button.GetComponentInChildren<TMP_Text>().text = "0";
        }

        private void setCardBacksActivity(bool value)
        {
            foreach (var cardBack in cardBackMap.Values)
            {
                cardBack.SetActive(value);
            }
        }
        
        public void SetSitButtonsInteractable(bool value)
        {
            foreach (var sitButton in sitButtonMap.Values)
            {
                sitButton.GetComponent<SitButton>().OpenDepositAreaButton.interactable = value;
            }
        }
        
        public void SetSitButtonsActivity(bool value)
        {
            foreach (var sitButton in sitButtonMap.Values)
            {
                sitButton.SetActive(value);
            }
        }
        
        ///////////////////////////////////////////////////HandMap Section/////////////////////////////////////////////////////
        private void InitiateHandMap()
        {
            handMap.Add(1,localPlayerHand);
            handMap.Add(3,bottomLeftHand);
            handMap.Add(5,leftHand);
            handMap.Add(7,topLeftHand);
            handMap.Add(9,topRightHand);
            handMap.Add(11,rightHand);
        }
        ///////////////////////////////////////////////////FlopCardMap Section/////////////////////////////////////////////////////
        private void InitiateFlopCardMap()
        {
            flopCardMap.Add(1,flopCard1);
            flopCardMap.Add(2,flopCard2);
            flopCardMap.Add(3,flopCard3);
            flopCardMap.Add(4,turnCard);
            flopCardMap.Add(5,riverCard);
        }
        ///////////////////////////////////////////////////SideBetMap Section/////////////////////////////////////////////////////
        private void InitiateSideBetMap()
        {
            sideBetMap.Add(1,sideBet1Button);
            sideBetMap.Add(2,sideBet2Button);
            sideBetMap.Add(3,sideBet3Button);
            sideBetMap.Add(4,sideBet4Button);
        }
        ///////////////////////////////////////////////////BetMap Section/////////////////////////////////////////////////////
        private void InitiateBetMap()
        {
            betMap.Add(1, bottomRightBet);
            betMap.Add(3, bottomLeftBet);
            betMap.Add(5, leftBet);
            betMap.Add(7, topLeftBet);
            betMap.Add(9, topRightBet);
            betMap.Add(11, rightBet);
        }
        ///////////////////////////////////////////////////DealerTokenMap Section/////////////////////////////////////////////////////
        private void InitiateDealerTokenMap()
        {
            dealerTokenMap.Add(1, bottomRightDealerToken);
            dealerTokenMap.Add(3, bottomLeftDealerToken);
            dealerTokenMap.Add(5, leftDealerToken);
            dealerTokenMap.Add(7, topLeftDealerToken);
            dealerTokenMap.Add(9, topRightDealerToken);
            dealerTokenMap.Add(11, rightDealerToken);
        }
        ///////////////////////////////////////////////////CardBackMap Section/////////////////////////////////////////////////////
        private void InitiateCardBackMap()
        {
            cardBackMap.Add(3,bottomLeftCardBack);
            cardBackMap.Add(5,leftCardBack);
            cardBackMap.Add(7,topLeftCardBack);
            cardBackMap.Add(9,topRightCardBack);
            cardBackMap.Add(11,rightCardBack);
        }
        ///////////////////////////////////////////////////SitButton Section/////////////////////////////////////////////////////
        private void InitiateSitButtonMap()
        {
            sitButtonMap.Add(3,bottomLeftSitButton);
            sitButtonMap.Add(5,leftSitButton);
            sitButtonMap.Add(7,topLeftSitButton);
            sitButtonMap.Add(9,topRightSitButton);
            sitButtonMap.Add(11,rightSitButton);
        }
        ///////////////////////////////////////////////////NLPlayerMap Section/////////////////////////////////////////////////////
        private void InitiateNLPlayerMap()
        {
            nLPlayerMap.Add(3,bottomLeftNLPlayer);
            nLPlayerMap.Add(5,leftNLPlayer);
            nLPlayerMap.Add(7,topLeftNLPlayer);
            nLPlayerMap.Add(9,topRightNLPlayer);
            nLPlayerMap.Add(11,rightNLPlayer);
        }
        ///////////////////////////////////////////////////Properties Section/////////////////////////////////////////////////////


        public GameObject DepositArea
        {
            get => depositArea;
            set => depositArea = value;
        }

        public TMP_InputField DepositInputField
        {
            get => depositInputField;
            set => depositInputField = value;
        }

        public TMP_Text DepositInformationText
        {
            get => depositInformationText;
            set => depositInformationText = value;
        }

        public Button DepositSitButton
        {
            get => depositSitButton;
            set => depositSitButton = value;
        }

        public Button DepositCancelButton
        {
            get => depositCancelButton;
            set => depositCancelButton = value;
        }

        public GameObject LeaveTableArea
        {
            get => leaveTableArea;
            set => leaveTableArea = value;
        }

        public TMP_Text LeaveTableInformationText
        {
            get => leaveTableInformationText;
            set => leaveTableInformationText = value;
        }

        public Button LeaveTableOpenConfirmationButton
        {
            get => leaveTableOpenConfirmationButton;
            set => leaveTableOpenConfirmationButton = value;
        }

        public Button LeaveTableOkButton
        {
            get => leaveTableOkButton;
            set => leaveTableOkButton = value;
        }

        public Button LeaveTableCancelButton
        {
            get => leaveTableCancelButton;
            set => leaveTableCancelButton = value;
        }

        public GameObject LocalPlayer
        {
            get => localPlayer;
            set => localPlayer = value;
        }

        public GameObject LocalPlayerHand
        {
            get => localPlayerHand;
            set => localPlayerHand = value;
        }

        public GameObject LeftCardBack
        {
            get => leftCardBack;
            set => leftCardBack = value;
        }

        public GameObject RightCardBack
        {
            get => rightCardBack;
            set => rightCardBack = value;
        }

        public GameObject TopLeftCardBack
        {
            get => topLeftCardBack;
            set => topLeftCardBack = value;
        }

        public GameObject TopRightCardBack
        {
            get => topRightCardBack;
            set => topRightCardBack = value;
        }

        public GameObject BottomLeftCardBack
        {
            get => bottomLeftCardBack;
            set => bottomLeftCardBack = value;
        }

        public GameObject BottomLeftSitButton
        {
            get => bottomLeftSitButton;
            set => bottomLeftSitButton = value;
        }

        public GameObject LeftSitButton
        {
            get => leftSitButton;
            set => leftSitButton = value;
        }

        public GameObject TopLeftSitButton
        {
            get => topLeftSitButton;
            set => topLeftSitButton = value;
        }

        public GameObject TopRightSitButton
        {
            get => topRightSitButton;
            set => topRightSitButton = value;
        }

        public GameObject RightSitButton
        {
            get => rightSitButton;
            set => rightSitButton = value;
        }

        public GameObject BottomLeftNlPlayer
        {
            get => bottomLeftNLPlayer;
            set => bottomLeftNLPlayer = value;
        }

        public GameObject LeftNlPlayer
        {
            get => leftNLPlayer;
            set => leftNLPlayer = value;
        }

        public GameObject TopLeftNlPlayer
        {
            get => topLeftNLPlayer;
            set => topLeftNLPlayer = value;
        }

        public GameObject TopRightNlPlayer
        {
            get => topRightNLPlayer;
            set => topRightNLPlayer = value;
        }

        public GameObject RightNlPlayer
        {
            get => rightNLPlayer;
            set => rightNLPlayer = value;
        }

        public GameObject BottomRightDealerToken
        {
            get => bottomRightDealerToken;
            set => bottomRightDealerToken = value;
        }

        public GameObject BottomLeftDealerToken
        {
            get => bottomLeftDealerToken;
            set => bottomLeftDealerToken = value;
        }

        public GameObject LeftDealerToken
        {
            get => leftDealerToken;
            set => leftDealerToken = value;
        }

        public GameObject TopLeftDealerToken
        {
            get => topLeftDealerToken;
            set => topLeftDealerToken = value;
        }

        public GameObject TopRightDealerToken
        {
            get => topRightDealerToken;
            set => topRightDealerToken = value;
        }

        public GameObject RightDealerToken
        {
            get => rightDealerToken;
            set => rightDealerToken = value;
        }

        public GameObject BottomRightBet
        {
            get => bottomRightBet;
            set => bottomRightBet = value;
        }

        public GameObject BottomLeftBet
        {
            get => bottomLeftBet;
            set => bottomLeftBet = value;
        }

        public GameObject LeftBet
        {
            get => leftBet;
            set => leftBet = value;
        }

        public GameObject TopLeftBet
        {
            get => topLeftBet;
            set => topLeftBet = value;
        }

        public GameObject TopRightBet
        {
            get => topRightBet;
            set => topRightBet = value;
        }

        public GameObject RightBet
        {
            get => rightBet;
            set => rightBet = value;
        }

        public GameObject BottomLeftHand
        {
            get => bottomLeftHand;
            set => bottomLeftHand = value;
        }

        public GameObject LeftHand
        {
            get => leftHand;
            set => leftHand = value;
        }

        public GameObject TopLeftHand
        {
            get => topLeftHand;
            set => topLeftHand = value;
        }

        public GameObject TopRightHand
        {
            get => topRightHand;
            set => topRightHand = value;
        }

        public GameObject RightHand
        {
            get => rightHand;
            set => rightHand = value;
        }

        public GameObject FlopCard1
        {
            get => flopCard1;
            set => flopCard1 = value;
        }

        public GameObject FlopCard2
        {
            get => flopCard2;
            set => flopCard2 = value;
        }

        public GameObject FlopCard3
        {
            get => flopCard3;
            set => flopCard3 = value;
        }

        public GameObject TurnCard
        {
            get => turnCard;
            set => turnCard = value;
        }

        public GameObject RiverCard
        {
            get => riverCard;
            set => riverCard = value;
        }

        public GameObject PotButton
        {
            get => potButton;
            set => potButton = value;
        }

        public GameObject GameController
        {
            get => gameController;
            set => gameController = value;
        }

        public IDictionary<int, GameObject> CardBackMap
        {
            get => cardBackMap;
            set => cardBackMap = value;
        }

        public IDictionary<int, GameObject> SitButtonMap
        {
            get => sitButtonMap;
            set => sitButtonMap = value;
        }

        public IDictionary<int, GameObject> NLPlayerMap
        {
            get => nLPlayerMap;
            set => nLPlayerMap = value;
        }

        public IDictionary<int, GameObject> DealerTokenMap
        {
            get => dealerTokenMap;
            set => dealerTokenMap = value;
        }

        public IDictionary<int, GameObject> BetMap
        {
            get => betMap;
            set => betMap = value;
        }

        public IDictionary<int, GameObject> FlopCardMap
        {
            get => flopCardMap;
            set => flopCardMap = value;
        }

        public IDictionary<int, GameObject> HandMap
        {
            get => handMap;
            set => handMap = value;
        }

        public int Deposit
        {
            get => deposit;
            set => deposit = value;
        }


        public void SetGameControllerValuesForMoveCheck(List<Seat> seatList, string myName, string lastMovePlayerName,
            bool isMyTurn, bool isStageChange, float middlePot, int sideBetCount, List<float> sideBetList,
            float smallBlind)
        {
            if (isMyTurn)
            {
                gameController.GetComponent<GameController>().SetMovesCanvasValuesForMoveCheck(seatList, myName, lastMovePlayerName,isStageChange,middlePot,smallBlind,sideBetCount,sideBetList);
                
            }
            else
            {
                gameController.GetComponent<GameController>().SetPreMovesCanvasValuesForMoveCheck(seatList, myName, lastMovePlayerName);
                
            }
        }

        public void SetMiddleCards(List<CardData> middleCards)
        {
            if (middleCards.Count == 3)
            {
                flopCard1.SetActive(true);
                flopCard1.GetComponent<Card>().SetCardData(middleCards[0]);
                flopCard2.SetActive(true);
                flopCard2.GetComponent<Card>().SetCardData(middleCards[1]);
                flopCard3.SetActive(true);
                flopCard3.GetComponent<Card>().SetCardData(middleCards[2]);
            }
            if (middleCards.Count == 4)
            {
                flopCard1.SetActive(true);
                flopCard1.GetComponent<Card>().SetCardData(middleCards[0]);
                flopCard2.SetActive(true);
                flopCard2.GetComponent<Card>().SetCardData(middleCards[1]);
                flopCard3.SetActive(true);
                flopCard3.GetComponent<Card>().SetCardData(middleCards[2]);
                turnCard.SetActive(true);
                turnCard.GetComponent<Card>().SetCardData(middleCards[3]);
            }
            if (middleCards.Count == 5)
            {
                flopCard1.SetActive(true);
                flopCard1.GetComponent<Card>().SetCardData(middleCards[0]);
                flopCard2.SetActive(true);
                flopCard2.GetComponent<Card>().SetCardData(middleCards[1]);
                flopCard3.SetActive(true);
                flopCard3.GetComponent<Card>().SetCardData(middleCards[2]);
                turnCard.SetActive(true);
                turnCard.GetComponent<Card>().SetCardData(middleCards[3]);
                riverCard.SetActive(true);
                riverCard.GetComponent<Card>().SetCardData(middleCards[4]);
            }
        }

        public void SetMiddlePot(float middlePot, int sideBetCount, List<float> sideBetList)
        {
            
            switch (sideBetCount)
            {
                case 0:
                    potButton.GetComponentInChildren<TMP_Text>().text = middlePot.ToString("N1");
                    break;
                case 1:
                    potButton.GetComponentInChildren<TMP_Text>().text = middlePot.ToString("N1");
                    sideBetMap[1].SetActive(true);
                    sideBetMap[1].GetComponentInChildren<TMP_Text>().text = 0.ToString("N1");
                    break;
                case 2:
                    sideBetMap[1].SetActive(true);
                    sideBetMap[1].GetComponentInChildren<TMP_Text>().text = sideBetList[0].ToString("N1");
                    potButton.GetComponentInChildren<TMP_Text>().text = middlePot.ToString("N1");
                    break;
                case 3:
                    sideBetMap[1].SetActive(true);
                    sideBetMap[1].GetComponentInChildren<TMP_Text>().text = sideBetList[0].ToString("N1");
                    sideBetMap[2].SetActive(true);
                    sideBetMap[2].GetComponentInChildren<TMP_Text>().text = sideBetList[1].ToString("N1");
                    potButton.GetComponentInChildren<TMP_Text>().text = middlePot.ToString("N1");
                    break;
                case 4:
                    sideBetMap[1].SetActive(true);
                    sideBetMap[1].GetComponentInChildren<TMP_Text>().text = sideBetList[0].ToString("N1");
                    sideBetMap[2].SetActive(true);
                    sideBetMap[2].GetComponentInChildren<TMP_Text>().text = sideBetList[1].ToString("N1");
                    sideBetMap[3].SetActive(true);
                    sideBetMap[3].GetComponentInChildren<TMP_Text>().text = sideBetList[2].ToString("N1");
                    potButton.GetComponentInChildren<TMP_Text>().text = middlePot.ToString("N1");
                    break;
                case 5:
                    sideBetMap[1].SetActive(true);
                    sideBetMap[1].GetComponentInChildren<TMP_Text>().text = sideBetList[0].ToString("N1");
                    sideBetMap[2].SetActive(true);
                    sideBetMap[2].GetComponentInChildren<TMP_Text>().text = sideBetList[1].ToString("N1");
                    sideBetMap[3].SetActive(true);
                    sideBetMap[3].GetComponentInChildren<TMP_Text>().text = sideBetList[2].ToString("N1");
                    sideBetMap[4].SetActive(true);
                    sideBetMap[4].GetComponentInChildren<TMP_Text>().text = sideBetList[3].ToString("N1");
                    potButton.GetComponentInChildren<TMP_Text>().text = middlePot.ToString("N1");
                    break;
                default:
                    Debug.LogWarning("Side Bet Count is not between 0 and 5");
                    break;
            }
        }
        
        

        public void ClearPreviousSubTurnBets(List<Seat> seatList)
        {
            foreach (Seat seat in seatList)
            {
                if (seat.isActive)
                {
                    betMap[(int) seat.location].GetComponent<Bet>().Clear();
                    betMap[(int) seat.location].SetActive(false);
                }
            }
        }

        public void SetGameControllerValuesForMoveRaise(List<Seat> seatList, string myName, string lastMovePlayerName,
            bool isMyTurn, float middlePot, int sideBetCount, List<float> sideBetList, float smallBlind)
        {
            if (isMyTurn)
            {
                gameController.GetComponent<GameController>().SetMovesCanvasValuesForMoveRaise(seatList, myName, lastMovePlayerName,middlePot,sideBetCount,sideBetList,smallBlind);
                
            }
            else
            {
                gameController.GetComponent<GameController>().SetPreMovesCanvasValuesForMoveRaise(seatList, myName, lastMovePlayerName);
                
            }
        }

        public void SetGameControllerValuesForMoveAllin(List<Seat> seatList, string myName, string lastMovePlayerName,
            bool isMyTurn, float middlePot, int sideBetCount, List<float> sideBetList, bool isStageChange,
            float smallBlind)
        {
            if (isMyTurn)
            {
                gameController.GetComponent<GameController>().SetMovesCanvasValuesForMoveAllin(seatList, myName, lastMovePlayerName,middlePot,sideBetCount,sideBetList,isStageChange,smallBlind);
                
            }
            else
            {
                gameController.GetComponent<GameController>().SetPreMovesCanvasValuesForMoveAllin(seatList, myName, lastMovePlayerName);
                
            }
        }

        public void DeactivateGameController()
        {
            GameController.SetActive(false);
        }

        public void OpenAllInGamePlayerHands(List<Seat> seatList, List<string> inGamePlayerList, List<List<CardData>> inGamePlayersCards)
        {
            foreach (var playerName in inGamePlayerList)
            {
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        if (string.Equals(playerName,seat.username))
                        {
                            handMap[(int) seat.location].SetActive(true);
                            handMap[(int) seat.location].GetComponent<Hand>().SetHand(inGamePlayersCards[inGamePlayerList.IndexOf(playerName)]);
                        }
                    }
                }
            }
        }

        public void ShakeWinningCombinations(List<Seat> seatList, List<string> winnerList,
            List<List<CardData>> winnerCombinations, List<string> inGamePlayerList,
            List<List<CardData>> inGamePlayersCards)
        {
             List<Tuple<string,List<CardData>,List<CardData>>> winnersAndWinningCardsAndWinningSharedCards = MakeTupleList(winnerList,winnerCombinations,inGamePlayerList,inGamePlayersCards);
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        foreach (var winner in winnersAndWinningCardsAndWinningSharedCards)
                        {
                            if (string.Equals(seat.username,winner.Item1))
                            {
                                handMap[(int) seat.location].GetComponent<Hand>().ShakeWinningCardsFromPlayerHand(winner.Item2,seat.location);
                            }
                            ShakeWinningCardsFromSharedCards(winner.Item3);
                        }
                    }
                }
        }

        private void ShakeWinningCardsFromSharedCards(List<CardData> winnerSharedCards)
        {
            List<GameObject> sharedCards = new List<GameObject>();
            sharedCards.Add(flopCard1);
            sharedCards.Add(flopCard2);
            sharedCards.Add(flopCard3);
            sharedCards.Add(turnCard);
            sharedCards.Add(riverCard);
            foreach (var winnerCard in winnerSharedCards)
            {
                foreach (var card in sharedCards)
                {
                    if (card.GetComponent<Card>().isEqual(winnerCard))
                    {
                        card.GetComponent<Card>().MoveSharedCardToUp();
                    }
                }
            }
        }

        private List<Tuple<string, List<CardData>, List<CardData>>> MakeTupleList(List<string> winnerList, List<List<CardData>> winnerCombinations, List<string> inGamePlayerList, List<List<CardData>> inGamePlayersCards)
        {
            List<Tuple<string, List<CardData>, List<CardData>>> winnersAndWinningCardsAndWinningSharedCards = new List<Tuple<string, List<CardData>, List<CardData>>>();
            List<Tuple<string,List<CardData>>> winnersAndHisHands = FindWinnersAndHisHands(winnerList,inGamePlayerList,inGamePlayersCards);
            List<Tuple<string,List<CardData>>> winnersAndWinnerCombinations = FindWinnersAndWinnerCombinations(winnersAndHisHands,winnerList,winnerCombinations);
            List<List<CardData>> winnersAndHisWinningCards = FindWinnersAndHisWinningCards(winnersAndWinnerCombinations,winnersAndHisHands);
            List<List<CardData>> winnersAndWinningSharedCards = FindWinnersAndHisWinningSharedCards(winnersAndWinnerCombinations,winnersAndHisHands);

            foreach (var winner in winnerList)
            {
                winnersAndWinningCardsAndWinningSharedCards.Add(new Tuple<string, List<CardData>, List<CardData>>(winner,winnersAndHisWinningCards[winnerList.IndexOf(winner)],winnersAndWinningSharedCards[winnerList.IndexOf(winner)]));
            }
            return winnersAndWinningCardsAndWinningSharedCards;
        }

        private List<List<CardData>> FindWinnersAndHisWinningSharedCards(List<Tuple<string, List<CardData>>> winnersAndWinnerCombinations, List<Tuple<string, List<CardData>>> winnersAndHisHands)
        {
            List<List<CardData>> winnersAndHisWinningSharedCards = new List<List<CardData>>();
            int winnerIndex = winnersAndHisHands.Count;
            for (int i = 0; i < winnerIndex; i++)
            {
                List<CardData> hisWinningSharedCards = FindHisWinningSharedCards(winnersAndHisHands[i].Item2,winnersAndWinnerCombinations[i].Item2);
                winnersAndHisWinningSharedCards.Add(hisWinningSharedCards);
            }
            return winnersAndHisWinningSharedCards;
        }

        private List<CardData> FindHisWinningSharedCards(List<CardData> hisHand, List<CardData> hisWinningCombination)
        {
            List<CardData> hisWinningSharedCards = new List<CardData>();
            foreach (var winnerCard in hisWinningCombination)
            {
                foreach (var playerCard in hisHand)
                {
                    if (!winnerCard.isEqual(playerCard,winnerCard))
                    {
                        hisWinningSharedCards.Add(winnerCard);
                    }
                }
            }
            return hisWinningSharedCards;
        }

        private List<List<CardData>> FindWinnersAndHisWinningCards(List<Tuple<string, List<CardData>>> winnersAndWinnerCombinations, List<Tuple<string, List<CardData>>> winnersAndHisHands)
        {
            List<List<CardData>> winnersAndHisWinningCards = new List<List<CardData>>();
            int winnerIndex = winnersAndHisHands.Count;

            for (int i = 0; i < winnerIndex; i++)
            {
                List<CardData> hisWinningCards = FindHisWinningCards(winnersAndHisHands[i].Item2,winnersAndWinnerCombinations[i].Item2);
                winnersAndHisWinningCards.Add(hisWinningCards);
            }

            return winnersAndHisWinningCards;
        }

        private List<CardData> FindHisWinningCards(List<CardData> hisHand, List<CardData> hisWinningCombination)
        {
            List<CardData> hisWinningCards = new List<CardData>();
            foreach (var card in hisHand)
            {
                foreach (var winningCard in hisWinningCombination)
                {
                    if (card.isEqual(winningCard,card))
                    {
                        hisWinningCards.Add(card);
                    }
                }
            }
            return hisWinningCards;
        }


        private List<Tuple<string, List<CardData>>> FindWinnersAndWinnerCombinations(List<Tuple<string, List<CardData>>> winnersAndHisHands, List<string> winnerList, List<List<CardData>> winnerCombinations)
        {
            List<Tuple<string, List<CardData>>> winnersAndWinnerCombinations = new List<Tuple<string, List<CardData>>>();
            foreach (var winner in winnerList)
            {
                foreach (var winnerAndHisHand in winnersAndHisHands)
                {
                    if (string.Equals(winner,winnerAndHisHand.Item1))
                    {
                        winnersAndWinnerCombinations.Add(new Tuple<string, List<CardData>>(winner,winnerCombinations[winnersAndHisHands.IndexOf(winnerAndHisHand)]));
                    }
                }
            }
            return winnersAndWinnerCombinations;
        }

        private List<Tuple<string, List<CardData>>> FindWinnersAndHisHands(List<string> winnerList, List<string> inGamePlayerList, List<List<CardData>> inGamePlayersCards)
        {
            List<Tuple<string, List<CardData>>> winnersAndHisHands = new List<Tuple<string, List<CardData>>>();
            foreach (var winner in winnerList)
            {
                foreach (var player in inGamePlayerList)
                {
                    if (string.Equals(winner,player))
                    {
                        winnersAndHisHands.Add(new Tuple<string, List<CardData>>(winner,inGamePlayersCards[inGamePlayerList.IndexOf(player)]));
                    }
                }
            }
            return winnersAndHisHands;
        }

        public void CloseHand(SeatLocations seatLocation)
        {
            handMap[(int) seatLocation].SetActive(false);
        }

        public void CloseCardBack(SeatLocations seatLocation)
        {
            cardBackMap[(int) seatLocation].SetActive(false);
        }
        

        
        public void ActivatePreShowDown()
        {
            gameController.GetComponent<GameController>().MyButtonList.ForEach(button => button.interactable = false);
            gameController.GetComponent<GameController>().AllInButton.gameObject.SetActive(false);
            gameController.GetComponent<GameController>().ShowCardsButton.gameObject.SetActive(true);
            gameController.GetComponent<GameController>().ShowCardsButton.interactable = true;
            gameController.GetComponent<GameController>().HideCardsButton.gameObject.SetActive(true);
            gameController.GetComponent<GameController>().HideCardsButton.interactable = true;
        }
        
        public void DeActivatePreShowDown()
        {
            gameController.GetComponent<GameController>().MyButtonList.ForEach(button => button.interactable = true);
            gameController.GetComponent<GameController>().AllInButton.gameObject.SetActive(true);
            gameController.GetComponent<GameController>().ShowCardsButton.gameObject.SetActive(false);
            gameController.GetComponent<GameController>().ShowCardsButton.interactable = false;
            gameController.GetComponent<GameController>().HideCardsButton.gameObject.SetActive(false);
            gameController.GetComponent<GameController>().HideCardsButton.interactable = false;
        }

        public void MoveChipsToWinner(List<Seat> seatList, List<string> winnerList, List<float> winAmountList)
        {
            setPotToZero();
            //MoveChipToWinner(seatList, winnerList, winAmountList);
        }
        public void MoveChipsToWinner(List<Seat> seatList, string winner, float winAmount)
        {
            setPotToZero();
            //MoveChipToWinner(seatList, winnerList, winAmountList);
        }

        public void ClearAllCards(List<Seat> seatList)
        {
            foreach (var seat in seatList)
            {
                handMap[(int) seat.location].GetComponent<Hand>().Clear();
            }
        }

        public void HideAllHands(List<Seat> seatList)
        {
            foreach (var seat in seatList)
            {
                handMap[(int) seat.location].SetActive(false);
            }
        }

        public void ClearAllBets(List<Seat> seatList)
        {
            setPotToZero();
            foreach (var seat in seatList)
            {
                betMap[(int) seat.location].GetComponent<Bet>().Clear();
            }
        }

        public void HideAllBets(List<Seat> seatList)
        {
            foreach (var seat in seatList)
            {
                betMap[(int) seat.location].SetActive(false);
            }
        }

        public void HideDealerToken(List<Seat> seatList)
        {
            foreach (var seat in seatList)
            {
                if (seat.isDealer)
                {
                    dealerTokenMap[(int) seat.location].SetActive(false);
                }
            }
        }

        public void ClearSharedCards(List<Seat> seatList)
        {
            foreach (var sharedCard in flopCardMap.Values)
            {
                sharedCard.GetComponent<Card>().Clear();
                sharedCard.SetActive(false);
            }
        }

        public void OpenWinnerHand(List<Seat> seatList, string winnerName, List<CardData> winnerCards)
        {
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    if (string.Equals(winnerName, seat.username))
                    {
                        handMap[(int) seat.location].SetActive(true);
                        handMap[(int) seat.location].GetComponent<Hand>().SetHand(winnerCards);
                    }
                }
            }
        }

       

        public void MakeCorrectionBet(List<Seat> seatList, string correctionPlayerName, float correctionBetAmount)
        {
            foreach (var seat in seatList)
            {
                if (string.Equals(seat.username, correctionPlayerName))
                {
                    betMap[(int) seat.location].GetComponent<Bet>().SetBetAmount(correctionBetAmount);
                }
            }
        }

        public void AnimateTurnCard(CardData cardData)
        {
            turnCard.SetActive(true);
            turnCard.GetComponent<Card>().SetCardData(cardData);
        }

        public void AnimateRiverCard(CardData cardData)
        {
            riverCard.SetActive(true);
            riverCard.GetComponent<Card>().SetCardData(cardData);
        }

        public void SetGameControllerValuesForMoveFold(List<Seat> seatList, string myName, string lastMovePlayerName,
            bool isMyTurn, float middlePot, int sideBetCount, List<float> sideBetList, float smallBlind,
            bool isStageChange)
        {
            if (isMyTurn)
            {
                gameController.GetComponent<GameController>().SetMovesCanvasValuesForMoveFold(seatList, myName, lastMovePlayerName,middlePot,sideBetCount,sideBetList,smallBlind,isStageChange);
                
            }
            else
            {
                gameController.GetComponent<GameController>().SetPreMovesCanvasValuesForMoveFold(seatList, myName, lastMovePlayerName);
                
            }
        }

        public void ToggleMoveIsDone(string myName, ToggleMove toggleMove, List<Seat> seatList)
        {
            gameController.GetComponent<GameController>().ToggleMoveIsDone();
        }


        public void DisableAllTimers()
        {
            localPlayer.GetComponent<LocalPlayer>().DisableTimer();
            foreach (var nlPlayer in nLPlayerMap.Values)
            {
                nlPlayer.GetComponent<nonLocalPlayer>().DisableTimer();
            }
        }

        public void HideEndGameButtons()
        {
            gameController.GetComponent<GameController>().HideEndGameButtons();
        }


        public void ClearAllSideBets()
        {
            sideBetMap.Values.ToList().ForEach(sideBet => sideBet.SetActive(false));
        }
    }
}