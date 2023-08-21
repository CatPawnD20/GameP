using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Windows;

namespace Assets.Scripts
{
    public class Dealer : MonoBehaviour
    {
        ///////////////////////////////////////////////////Variables Section/////////////////////////////////////////////////////
        public static Dealer instance;
        [SerializeField] private string syncID;

        private ArrayList deck = new ArrayList();
        private List<CardData> flopCards = new List<CardData>();
        private CardData turnCard;
        private CardData riverCard;

        public IDictionary<string, List<CardData>> playerHandMap = new Dictionary<string, List<CardData>>();
        private IDictionary<int, List<CardData>> playerHandIndexMap = new Dictionary<int, List<CardData>>();
        private List<OmahaHand> omahaHandList = new List<OmahaHand>();
        private List<Tuple<string,ToggleMove,float>> playerToggleMoveTupleList = new List<Tuple<string, ToggleMove, float>>();
        private string preMovePlayer;

        private List<CardData> seat1Hand = new List<CardData>();
        private List<CardData> seat2Hand = new List<CardData>();
        private List<CardData> seat3Hand = new List<CardData>();
        private List<CardData> seat4Hand = new List<CardData>();
        private List<CardData> seat5Hand = new List<CardData>();
        private List<CardData> seat6Hand = new List<CardData>();
        private List<CardData> seat7Hand = new List<CardData>();
        private List<CardData> seat8Hand = new List<CardData>();
        private List<CardData> seat9Hand = new List<CardData>();
        private List<CardData> seat10Hand = new List<CardData>();
        private List<CardData> seat11Hand = new List<CardData>();
        private List<CardData> seat12Hand = new List<CardData>();
        ///////////////////////////////////////////////////Events Section/////////////////////////////////////////////////////
        private void Awake()
        {
            playerHandIndexMap.Add(1,seat1Hand);
            playerHandIndexMap.Add(2,seat2Hand);
            playerHandIndexMap.Add(3,seat3Hand);
            playerHandIndexMap.Add(4,seat4Hand);
            playerHandIndexMap.Add(5,seat5Hand);
            playerHandIndexMap.Add(6,seat6Hand);
            playerHandIndexMap.Add(7,seat7Hand);
            playerHandIndexMap.Add(8,seat8Hand);
            playerHandIndexMap.Add(9,seat9Hand);
            playerHandIndexMap.Add(10,seat10Hand);
            playerHandIndexMap.Add(11,seat11Hand);
            playerHandIndexMap.Add(12,seat12Hand);
            
        }

        void Start()
        {
            instance = this;
        }
        ///////////////////////////////////////////////////TurnStage Section/////////////////////////////////////////////////////
        public List<CardData> GetMiddleCards(TurnStage myTurnStage)
        {
            List<CardData> cards = new List<CardData>();
            switch (myTurnStage)
            {
                case TurnStage.PreFlop:
                    cards =  GetFlopCards();
                    break;
                case TurnStage.PreTurn:
                    cards =  GetTurnCard();
                    break;
                case TurnStage.PreRiver:
                    cards =  GetRiverCard();
                    break;
            }

            return cards;
        }

        private List<CardData> GetRiverCard()
        {
            flopCards.Add(riverCard);
            return flopCards;
        }

        public List<CardData> GetTurnCard()
        {
            flopCards.Add(turnCard);
            return flopCards;
        }


        private List<CardData> GetFlopCards()
        {
            return flopCards; 
        }

        ///////////////////////////////////////////////////DealCards Section/////////////////////////////////////////////////////
        public void PrintHandSuitArray(int[] suitArray)
        {
            for (int i = 0; i < suitArray.Length; i++)
            {
                Debug.Log((CardData.Suit) suitArray[i]);
            }
        }
        public void PrintHandValueArray(int[] valueArray)
        {
            for (int i = 0; i < valueArray.Length; i++)
            {
                Debug.Log((CardData.Value) valueArray[i]);
            }
        }
        public void PrintHand(List<CardData> hand)
        {
            foreach (CardData card in hand)
            {
                Debug.Log(card.MySuit + " " + card.MyValue);
            }
        }
        public int[] DealCards(List<Seat> seatList, SeatLocations currentDealerlocation, int[] activeSeatLocations,
            int currentTurnID)
        {
            deck = createNewDeck();
            deck = shuffleDeckXTimes(deck, 8);
            activeSeatLocations = ISeatList.ActiveSeatLocationsreOrderByDealerSeat((int) currentDealerlocation, activeSeatLocations);
            dealAllCards(activeSeatLocations, seatList,deck);
            BuildOmahaHandList(currentTurnID);
            return activeSeatLocations;
        }

        private ArrayList shuffleDeckXTimes(ArrayList arrayList, int x)
        {
            for (int i = 0; i < x; i++)
            {
                deck = shuffleDeck(deck);
            }
            return deck;
        }

        private void dealAllCards(int[] activeSeatLocationArray, List<Seat> seatList, ArrayList deck)
        {
            PlayerHandsClear();
            FlopTurnRiverClear();
            int k = DealPlayerCards(activeSeatLocationArray, seatList, deck);
            SetFlopTurnRiverCards(deck, k*4);
        }

        private int DealPlayerCards(int[] activeSeatLocationArray, List<Seat> seatList, ArrayList arrayList)
        {
            int k = 0;
            CardData dummyCardData;
            int jump = activeSeatLocationArray.Length;
            for (int j = 0; j < activeSeatLocationArray.Length; j++)
            {
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        if ((int)seat.location == activeSeatLocationArray[j])
                        {
                            dummyCardData = deck[k] as CardData;
                            playerHandIndexMap[(int) seat.location].Add(dummyCardData);
                            dummyCardData = deck[k + jump] as CardData;
                            playerHandIndexMap[(int) seat.location].Add(dummyCardData);
                            dummyCardData = deck[k + jump + jump] as CardData;
                            playerHandIndexMap[(int) seat.location].Add(dummyCardData);
                            dummyCardData = deck[k + jump + jump + jump] as CardData;
                            playerHandIndexMap[(int) seat.location].Add(dummyCardData);
                            playerHandMap.Add(seat.username,playerHandIndexMap[(int) seat.location]);
                            k++;
                        }
                    }
                }
            }

            return k;
        }

        private void BuildOmahaHandList(int currentTurnID)
        {
            foreach (var playerName in playerHandMap.Keys)
            {
                OmahaHand newOmahaHand = new OmahaHand(playerName,currentTurnID, playerHandMap[playerName]);
                omahaHandList.Add(newOmahaHand);
            }
        }

        private void SetFlopTurnRiverCards(ArrayList arrayList, int lastCardIndex)
        {
            int i = lastCardIndex + 1; 
            flopCards.Add(arrayList[i] as CardData);
            flopCards.Add(arrayList[i + 1] as CardData);
            flopCards.Add(arrayList[i + 2] as CardData);
            turnCard = arrayList[i + 4] as CardData;
            riverCard = arrayList[i + 6] as CardData;
        }

        private void FlopTurnRiverClear()
        {
            flopCards.Clear();
            turnCard = null;
            riverCard = null;
        }

        private void PlayerHandsClear()
        {
            playerHandMap.Clear();
            foreach (var hand in PlayerHandIndexMap.Values)
            {
                hand.Clear();
            }
        }
        
        ///////////////////////////////////////////////////Deck Section/////////////////////////////////////////////////////
        private ArrayList createNewDeck()
        {
            ArrayList dummyDeck = new ArrayList();
            for (int i = 1; i < 5; i++)
            {
                for (int j = 2; j < 15; j++)
                {
                    CardData newCardData = new CardData((CardData.Suit) i, (CardData.Value) j);
                    dummyDeck.Add(newCardData);
                }
            }
            return dummyDeck;
        }

        private ArrayList shuffleDeck(ArrayList paramDeck)
        {
            ArrayList dummyDeck = new ArrayList();
            CardData dummyCardData;
            int deckSize = paramDeck.Count;
            while (deckSize > 0)
            {
                int randomCardNumber = GuidGenerator.GetRandomNumber() % deckSize;
                dummyCardData = paramDeck[randomCardNumber] as CardData;
                paramDeck.RemoveAt(randomCardNumber);
                dummyDeck.Add(dummyCardData);
                deckSize = paramDeck.Count;
            }
            return dummyDeck;
        }

        public List<CardData> BuildMyHand(int[] mySuitArr, int[] myValueArr)
        {
            List<CardData> myHand = new List<CardData>();
            for (int i = 0; i < mySuitArr.Length; i++)
            {
                CardData newCardData = new CardData((CardData.Suit) mySuitArr[i], (CardData.Value) myValueArr[i]);
                myHand.Add(newCardData);
            }
            return myHand;
        }
        
        public List<CardData> SortMyHandHighToLow(List<CardData> hand)
        {
            List<CardData> dummyHand = new List<CardData>();
            dummyHand = hand;
            dummyHand = dummyHand.OrderBy(x => x.MySuit).ThenByDescending(x => x.MyValue).ToList();
            return dummyHand;
        }
        public int[] handToSuitArray(List<CardData> hand)
        {
            int[] dummyArray = new int[4];
            for (int i = 0; i < 4; i++)
            {
                dummyArray[i] = (int) hand[i].MySuit;
            }

            return dummyArray;
        }
        public int[] handToValueArray(List<CardData> hand)
        {
            int[] dummyArray = new int[4];
            for (int i = 0; i < 4; i++)
            {
                dummyArray[i] = (int) hand[i].MyValue;
            }
            return dummyArray;
        }
        ///////////////////////////////////////////////////HandMap Section/////////////////////////////////////////////////////
        public List<CardData> GetHandFromPlayerHandMap(string username)
        {
            if (playerHandMap.ContainsKey(username))
            {
                return playerHandMap[username];
            }
            Debug.LogWarning("Dealer.cs: getHandFromPlayerHandMap() - playerHandMap does not contain username: " + username);
            return null;
        }
        ///////////////////////////////////////////////////Properties Section/////////////////////////////////////////////////////
        public List<OmahaHand> OmahaHandList => omahaHandList;

        public string SyncID
        {
            get => syncID;
            set => syncID = value;
        }

        public IDictionary<string, List<CardData>> PlayerHandMap
        {
            get => playerHandMap;
            set => playerHandMap = value;
        }

        public IDictionary<int, List<CardData>> PlayerHandIndexMap
        {
            get => playerHandIndexMap;
            set => playerHandIndexMap = value;
        }


        public List<CardData> GetSharedCards(TurnStage currentTurnStage)
        {
            List<CardData> sharedCards = new List<CardData>();
            switch (currentTurnStage)
            {
                case TurnStage.PreFlop:
                    return SetSharedCards(sharedCards,TurnStage.PreFlop);
                case TurnStage.PreTurn:
                    return SetSharedCards(sharedCards,TurnStage.PreTurn);
                case TurnStage.PreRiver:
                    return SetSharedCards(sharedCards,TurnStage.PreRiver);
                case TurnStage.PreShowDown:
                    return SetSharedCards(sharedCards,TurnStage.PreShowDown);
                default:
                    return null;
            }
        }

        private List<CardData> SetSharedCards(List<CardData> sharedCards, TurnStage currentTurnStage)
        {
            switch (currentTurnStage)
            {
                case TurnStage.PreFlop:
                    return sharedCards;
                case TurnStage.PreTurn:
                    sharedCards.Add(flopCards[0]);
                    sharedCards.Add(flopCards[1]);
                    sharedCards.Add(flopCards[2]);
                    return sharedCards;
                case TurnStage.PreRiver:
                    sharedCards.Add(flopCards[0]);
                    sharedCards.Add(flopCards[1]);
                    sharedCards.Add(flopCards[2]);
                    sharedCards.Add(turnCard);
                    return sharedCards;
                case TurnStage.PreShowDown:
                    sharedCards.Add(flopCards[0]);
                    sharedCards.Add(flopCards[1]);
                    sharedCards.Add(flopCards[2]);
                    sharedCards.Add(turnCard);
                    sharedCards.Add(riverCard);
                    return sharedCards;
                default:
                    return null;
            }
        }

        public List<CardData> GetPlayerCards(string playerName)
        {
            List<CardData> playerCards = new List<CardData>();
            if (playerHandMap.ContainsKey(playerName))
            {
                playerCards = playerHandMap[playerName];
            }
            return playerCards;
        }

        public Tuple<int, List<CardData>> CalculateHandRank(List<CardData> playerCards, List<CardData> sharedCards)
        {
            List<Tuple<int,List<CardData>>> maxValuesAndCombinations = new List<Tuple<int, List<CardData>>>();
            maxValuesAndCombinations.Clear();
            maxValuesAndCombinations.Add(getValueOfBestFiveCard(playerCards[0],playerCards[1],sharedCards));
            maxValuesAndCombinations.Add(getValueOfBestFiveCard(playerCards[0],playerCards[2],sharedCards));
            maxValuesAndCombinations.Add(getValueOfBestFiveCard(playerCards[0],playerCards[3],sharedCards));
            maxValuesAndCombinations.Add(getValueOfBestFiveCard(playerCards[1],playerCards[2],sharedCards));
            maxValuesAndCombinations.Add(getValueOfBestFiveCard(playerCards[1],playerCards[3],sharedCards));
            maxValuesAndCombinations.Add(getValueOfBestFiveCard(playerCards[2],playerCards[3],sharedCards));
            return FindMinValueAndCombination(maxValuesAndCombinations);
        }

        private Tuple<int, List<CardData>> getValueOfBestFiveCard(CardData playerCard1, CardData playerCard2, List<CardData> sharedCards)
        {
            List<Tuple<int,List<CardData>>> maxValuesAndCombinations = new List<Tuple<int, List<CardData>>>();
            maxValuesAndCombinations.Clear();
            maxValuesAndCombinations.Add(FindBestFiveCard(playerCard1,playerCard2, sharedCards[0], sharedCards[1], sharedCards[2]));
            maxValuesAndCombinations.Add(FindBestFiveCard(playerCard1,playerCard2, sharedCards[0], sharedCards[1], sharedCards[3]));
            maxValuesAndCombinations.Add(FindBestFiveCard(playerCard1,playerCard2, sharedCards[0], sharedCards[1], sharedCards[4]));
            maxValuesAndCombinations.Add(FindBestFiveCard(playerCard1,playerCard2, sharedCards[0], sharedCards[2], sharedCards[3]));
            maxValuesAndCombinations.Add(FindBestFiveCard(playerCard1,playerCard2, sharedCards[0], sharedCards[2], sharedCards[4]));
            maxValuesAndCombinations.Add(FindBestFiveCard(playerCard1,playerCard2, sharedCards[0], sharedCards[3], sharedCards[4]));
            maxValuesAndCombinations.Add(FindBestFiveCard(playerCard1,playerCard2, sharedCards[1], sharedCards[2], sharedCards[3]));
            maxValuesAndCombinations.Add(FindBestFiveCard(playerCard1,playerCard2, sharedCards[1], sharedCards[2], sharedCards[4]));
            maxValuesAndCombinations.Add(FindBestFiveCard(playerCard1,playerCard2, sharedCards[1], sharedCards[3], sharedCards[4]));
            maxValuesAndCombinations.Add(FindBestFiveCard(playerCard1,playerCard2, sharedCards[2], sharedCards[3], sharedCards[4]));
            return FindMinValueAndCombination(maxValuesAndCombinations);
            
        }

        private Tuple<int, List<CardData>> FindMinValueAndCombination(List<Tuple<int, List<CardData>>> maxValuesAndCombinations)
        {
            int minValue = maxValuesAndCombinations[0].Item1;
            List<CardData> minCombination = maxValuesAndCombinations[0].Item2;
            foreach (var maxValueAndCombination in maxValuesAndCombinations)
            {
                if (maxValueAndCombination.Item1 < minValue)
                {
                    minValue = maxValueAndCombination.Item1;
                    minCombination = maxValueAndCombination.Item2;
                }
            }
            return new Tuple<int, List<CardData>>(minValue,minCombination);
        }


        private Tuple<int, List<CardData>> FindBestFiveCard(CardData playerCard1, CardData playerCard2, CardData sharedCards1, CardData sharedCards2, CardData sharedCards3)
        {
            Tuple<int, List<CardData>> fiveCardValueAndCardDatas = new Tuple<int, List<CardData>>(0, new List<CardData>());
            Kard[] handArray = TransferCardDatasToKard(playerCard1, playerCard2, sharedCards1, sharedCards2, sharedCards3);
            int handValue = Eval.Eval5Cards(handArray);
            List<CardData> bestFiveCard = new List<CardData>();
            bestFiveCard.Add(playerCard1);
            bestFiveCard.Add(playerCard2);
            bestFiveCard.Add(sharedCards1);
            bestFiveCard.Add(sharedCards2);
            bestFiveCard.Add(sharedCards3);
            fiveCardValueAndCardDatas = new Tuple<int, List<CardData>>(handValue, bestFiveCard);
            return fiveCardValueAndCardDatas;
        }

        private Kard[] TransferCardDatasToKard(CardData playerCard1, CardData playerCard2, CardData sharedCards1, CardData sharedCards2, CardData sharedCards3)
        { 
            Kard[] kardArray = new Kard[5];
            kardArray[0] = CardDataToKard(playerCard1);
            kardArray[1] = CardDataToKard(playerCard2);
            kardArray[2] = CardDataToKard(sharedCards1);
            kardArray[3] = CardDataToKard(sharedCards2);
            kardArray[4] = CardDataToKard(sharedCards3);
            return kardArray;

        }

        private Kard CardDataToKard(CardData cardData)
        {
            char suitChar = 'a';
            switch (cardData.MySuit)
            {
                case CardData.Suit.Clubs:
                    suitChar = 'c';
                    break;
                case CardData.Suit.Diamonds:
                    suitChar = 'd';
                    break;
                case CardData.Suit.Hearts:
                    suitChar = 'h';
                    break;
                case CardData.Suit.Spades:
                    suitChar = 's';
                    break;
            }
            char valueChar = '0';
            switch (cardData.MyValue)
            {
                case CardData.Value.Ace:
                    valueChar = 'A';
                    break;
                case CardData.Value.Two:
                    valueChar = '2';
                    break;
                case CardData.Value.Three:
                    valueChar = '3';
                    break;
                case CardData.Value.Four:
                    valueChar = '4';
                    break;
                case CardData.Value.Five:
                    valueChar = '5';
                    break;
                case CardData.Value.Six:
                    valueChar = '6';
                    break;
                case CardData.Value.Seven:
                    valueChar = '7';
                    break;
                case CardData.Value.Eight:
                    valueChar = '8';
                    break;
                case CardData.Value.Nine:
                    valueChar = '9';
                    break;
                case CardData.Value.Ten:
                    valueChar = 'T';
                    break;
                case CardData.Value.Jack:
                    valueChar = 'J';
                    break;
                case CardData.Value.Queen:
                    valueChar = 'Q';
                    break;
                case CardData.Value.King:
                    valueChar = 'K';
                    break;
            }
            Kard kard = new Kard(String.Concat(valueChar, suitChar));
            return kard;
        }

        public List<Seat> AddWinAmountsToPlayers(List<Seat> seatList, List<string> winnerList, List<float> winnerPotList)
        {
            if (winnerPotList.Count != winnerList.Count)
            {
                Debug.LogError("winnerPotList.Count != winnerList.Count");
                return seatList;
            }

            for (int i = 0; i < winnerList.Count; i++)
            {
                foreach (var seat in seatList)
                {
                    if (string.Equals(seat.username, winnerList[i]))
                    {
                        seatList = ISeatList.UpdateSeatVarAddBalance(seatList,seat, winnerPotList[i]);
                    }
                }
            }
            
            return seatList;
        }

        public CardData GetTurnCardOnly()
        {
            return turnCard;
        }

        public CardData GetRiverCardOnly()
        {
            return riverCard;
        }

        

        

        

        


        public void ClearDealerGOData()
        {
            playerToggleMoveTupleList.Clear();
            deck.Clear();
            flopCards.Clear();
            turnCard = null;
            riverCard = null;
            playerHandMap.Clear();
            omahaHandList.Clear();
            seat1Hand.Clear();
            seat2Hand.Clear();
            seat3Hand.Clear();
            seat4Hand.Clear();
            seat5Hand.Clear();
            seat6Hand.Clear();
            seat7Hand.Clear();
            seat8Hand.Clear();
            seat9Hand.Clear();
            seat10Hand.Clear();
            seat11Hand.Clear();
            seat12Hand.Clear();
            
        
        }

        public bool isToggleMoveExistInPlayerToggleMoveTupleList(string playerName, ToggleMove toggleMove)
        {
            if (playerToggleMoveTupleList.Count == 0)
            {
                return false;
            }
            foreach (var playerToggleMoveTuple in playerToggleMoveTupleList)
            {
                if (string.Equals(playerToggleMoveTuple.Item1, playerName))
                {
                    if (playerToggleMoveTuple.Item2 == toggleMove)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void AddToggleMoveToTheList(ToggleMove toggleMove, float moveAmount, string playerName)
        {
            playerToggleMoveTupleList.Add(new Tuple<string, ToggleMove, float>(playerName, toggleMove, moveAmount));
        }

        public void RemoveToggleMoveFromTheList(string playerName)
        {
            for (int i = playerToggleMoveTupleList.Count - 1; i >= 0; i--)
            {
                var playerToggleMoveTuple = playerToggleMoveTupleList[i];
                if (string.Equals(playerToggleMoveTuple.Item1, playerName))
                {
                    playerToggleMoveTupleList.RemoveAt(i);
                }
            }
        }

        public bool PlayerHasPreMove(string playerName)
        {
            if (string.IsNullOrEmpty(playerName))
            {
                return false;
            }
            foreach (var playerToggleMoveTuple in playerToggleMoveTupleList)
            {
                if (string.Equals(playerToggleMoveTuple.Item1, playerName))
                {
                    return true;
                }
            }
            return false;
        }

        public ToggleMove GetToggleMoveOfPlayer(string preMovePlayerName)
        {
            foreach (var playerToggleMoveTuple in playerToggleMoveTupleList)
            {
                if (string.Equals(playerToggleMoveTuple.Item1, preMovePlayerName))
                {
                    return playerToggleMoveTuple.Item2;
                }
            }
            return ToggleMove.None;
        }

        public float GetToggleMoveAmountOfPlayer(string preMovePlayerName)
        {
            foreach (var playerToggleMoveTuple in playerToggleMoveTupleList)
            {
                if (string.Equals(playerToggleMoveTuple.Item1, preMovePlayerName))
                {
                    return playerToggleMoveTuple.Item3;
                }
            }
            return -1;
        }

        public void SetPreMovePlayerName(string preMovePlayerName)
        {
            preMovePlayer = preMovePlayerName;
        }

        public string GetPreMovePlayerName()
        {
            return preMovePlayer;
        }

        public void RemovePreMovePlayerName()
        {
            preMovePlayer = string.Empty;
        }

        public bool HasPreMove()
        {
            return PlayerHasPreMove(preMovePlayer);
        }
    }
}