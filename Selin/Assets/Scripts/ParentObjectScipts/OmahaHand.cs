using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class OmahaHand : ParentObject
    {
        private List<CardData> cards;
        private string playerName;
        private int turnId;
        private CardData.Suit card1Suit;
        private CardData.Value card1Value;
        private CardData.Suit card2Suit;
        private CardData.Value card2Value;
        private CardData.Suit card3Suit;
        private CardData.Value card3Value;
        private CardData.Suit card4Suit;
        private CardData.Value card4Value;

        
       
        
        public OmahaHand()
        {
            
        }

        public OmahaHand(string playerName, int turnId, List<CardData> cards)
        {
            this.playerName = playerName;
            this.turnId = turnId;
            this.cards = cards;
            SetCardData(cards);
        }

        
        public void SetCardData(List<CardData> cards)
        {
            card1Suit = cards[0].MySuit;
            card1Value = cards[0].MyValue;
            card2Suit = cards[1].MySuit;
            card2Value = cards[1].MyValue;
            card3Suit = cards[2].MySuit;
            card3Value = cards[2].MyValue;
            card4Suit = cards[3].MySuit;
            card4Value = cards[3].MyValue;
        }
        
        public CardData.Suit Card1Suit
        {
            get => card1Suit;
            set => card1Suit = value;
        }

        public CardData.Value Card1Value
        {
            get => card1Value;
            set => card1Value = value;
        }

        public CardData.Suit Card2Suit
        {
            get => card2Suit;
            set => card2Suit = value;
        }

        public CardData.Value Card2Value
        {
            get => card2Value;
            set => card2Value = value;
        }

        public CardData.Suit Card3Suit
        {
            get => card3Suit;
            set => card3Suit = value;
        }

        public CardData.Value Card3Value
        {
            get => card3Value;
            set => card3Value = value;
        }

        public CardData.Suit Card4Suit
        {
            get => card4Suit;
            set => card4Suit = value;
        }

        public CardData.Value Card4Value
        {
            get => card4Value;
            set => card4Value = value;
        }

        public List<CardData> Cards
        {
            get => cards;
            set => cards = value;
        }

        public string PlayerName
        {
            get => playerName;
            set => playerName = value;
        }

        public int TurnId
        {
            get => turnId;
            set => turnId = value;
        }
    }
}