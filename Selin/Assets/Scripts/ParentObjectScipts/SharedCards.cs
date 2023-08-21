using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class SharedCards : ParentObject
    {
        private int turnId;
        private CardData.Suit flop1Suit;
        private CardData.Value flop1Value;
        private CardData.Suit flop2Suit;
        private CardData.Value flop2Value;
        private CardData.Suit flop3Suit;
        private CardData.Value flop3Value;
        private CardData.Suit turnSuit;
        private CardData.Value turnValue;
        private CardData.Suit riverSuit;
        private CardData.Value riverValue;
        
        private Card[] flop = new Card[3];
        private Card turn;
        private Card river;
        
        
        public SharedCards(int turnId,List<CardData> sharedCards)
        {
            this.turnId = turnId;
            if (sharedCards.Count == 0)
            {
                flop1Suit = CardData.Suit.None;
                flop1Value = CardData.Value.None;
                flop2Suit = CardData.Suit.None;
                flop2Value = CardData.Value.None;
                flop3Suit = CardData.Suit.None;
                flop3Value = CardData.Value.None;
                turnSuit = CardData.Suit.None;
                turnValue = CardData.Value.None;
                riverSuit = CardData.Suit.None;
                riverValue = CardData.Value.None;
            }

            if (sharedCards.Count == 3)
            {
                flop1Suit = sharedCards[0].MySuit;
                flop1Value = sharedCards[0].MyValue;
                flop2Suit = sharedCards[1].MySuit;
                flop2Value = sharedCards[1].MyValue;
                flop3Suit = sharedCards[2].MySuit;
                flop3Value = sharedCards[2].MyValue;
                turnSuit = CardData.Suit.None;
                turnValue = CardData.Value.None;
                riverSuit = CardData.Suit.None;
                riverValue = CardData.Value.None;
            }

            if (sharedCards.Count == 4)
            {
                flop1Suit = sharedCards[0].MySuit;
                flop1Value = sharedCards[0].MyValue;
                flop2Suit = sharedCards[1].MySuit;
                flop2Value = sharedCards[1].MyValue;
                flop3Suit = sharedCards[2].MySuit;
                flop3Value = sharedCards[2].MyValue;
                turnSuit = sharedCards[3].MySuit;
                turnValue = sharedCards[3].MyValue;
                riverSuit = CardData.Suit.None;
                riverValue = CardData.Value.None;
            }
            if (sharedCards.Count == 5)
            {
                flop1Suit = sharedCards[0].MySuit;
                flop1Value = sharedCards[0].MyValue;
                flop2Suit = sharedCards[1].MySuit;
                flop2Value = sharedCards[1].MyValue;
                flop3Suit = sharedCards[2].MySuit;
                flop3Value = sharedCards[2].MyValue;
                turnSuit = sharedCards[3].MySuit;
                turnValue = sharedCards[3].MyValue;
                riverSuit = sharedCards[4].MySuit;
                riverValue = sharedCards[4].MyValue;
            }
        }
        
        //set the flop's suits and values from CardArray
        public void SetFlop(Card[] flop)
        {
            flop1Suit = flop[0].MyCardData.MySuit;
            flop1Value = flop[0].MyCardData.MyValue;
            flop2Suit = flop[1].MyCardData.MySuit;
            flop2Value = flop[1].MyCardData.MyValue;
            flop3Suit = flop[2].MyCardData.MySuit;
            flop3Value = flop[2].MyCardData.MyValue;
        }
        
        //set the turn's suit and value from CardArray
        public void SetTurn(Card turn)
        {
            turnSuit = turn.MyCardData.MySuit;
            turnValue = turn.MyCardData.MyValue;
        }
        
        //set the river's suit and value from CardArray
        public void SetRiver(Card river)
        {
            riverSuit = river.MyCardData.MySuit;
            riverValue = river.MyCardData.MyValue;
        }

        public int TurnId
        {
            get => turnId;
            set => turnId = value;
        }

        public CardData.Suit Flop1Suit
        {
            get => flop1Suit;
            set => flop1Suit = value;
        }

        public CardData.Value Flop1Value
        {
            get => flop1Value;
            set => flop1Value = value;
        }

        public CardData.Suit Flop2Suit
        {
            get => flop2Suit;
            set => flop2Suit = value;
        }

        public CardData.Value Flop2Value
        {
            get => flop2Value;
            set => flop2Value = value;
        }

        public CardData.Suit Flop3Suit
        {
            get => flop3Suit;
            set => flop3Suit = value;
        }

        public CardData.Value Flop3Value
        {
            get => flop3Value;
            set => flop3Value = value;
        }

        public CardData.Suit TurnSuit
        {
            get => turnSuit;
            set => turnSuit = value;
        }

        public CardData.Value TurnValue
        {
            get => turnValue;
            set => turnValue = value;
        }

        public CardData.Suit RiverSuit
        {
            get => riverSuit;
            set => riverSuit = value;
        }

        public CardData.Value RiverValue
        {
            get => riverValue;
            set => riverValue = value;
        }

        public Card[] Flop
        {
            get => flop;
            set => flop = value;
        }

        public Card Turn
        {
            get => turn;
            set => turn = value;
        }

        public Card River
        {
            get => river;
            set => river = value;
        }
    }
}