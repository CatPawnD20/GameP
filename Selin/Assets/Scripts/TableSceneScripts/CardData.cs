using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class CardData
    {
        public Suit mySuit;
        public Value myValue;

        public CardData()
        {
            
        }
        public CardData(Suit suit, Value value)
        {
            mySuit = suit;
            myValue = value;
        }
        
        
        


        ///////////////////////////////////////////////////Enums Section/////////////////////////////////////////////////////
        public enum Suit
        {
            Hearts = 1,
            Diamonds = 2,
            Clubs = 3,
            Spades = 4,
            None = 0
        }
        public enum Value
        {
            Two = 2,
            Three = 3,
            Four = 4,
            Five = 5,
            Six = 6,
            Seven = 7,
            Eight = 8,
            Nine = 9,
            Ten = 10,
            Jack = 11,
            Queen = 12,
            King = 13,
            Ace = 14,
            None = 0
        }
        ///////////////////////////////////////////////////Properties Section/////////////////////////////////////////////////////
        public Suit MySuit
        {
            get => mySuit;
            set => mySuit = value;
        }

        public Value MyValue
        {
            get => myValue;
            set => myValue = value;
        }

        public bool isEqual(CardData card1, CardData card2)
        {
            if (card1.MySuit == card2.MySuit && card1.MyValue == card2.MyValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}