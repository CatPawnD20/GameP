using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Card : MonoBehaviour
    {
        ///////////////////////////////////////////////////Variables Section/////////////////////////////////////////////////////
        private CardData myCardData;
        [SerializeField] private RawImage myRawImage = null;
        [SerializeField] private Texture2D cardBack = null;
        [Header("Hearts")]
        [SerializeField] private Texture2D heart2 = null;
        [SerializeField] private Texture2D heart3 = null;
        [SerializeField] private Texture2D heart4 = null;
        [SerializeField] private Texture2D heart5 = null;
        [SerializeField] private Texture2D heart6 = null;
        [SerializeField] private Texture2D heart7 = null;
        [SerializeField] private Texture2D heart8 = null;
        [SerializeField] private Texture2D heart9 = null;
        [SerializeField] private Texture2D heart10 = null;
        [SerializeField] private Texture2D heartJ = null;
        [SerializeField] private Texture2D heartQ = null;
        [SerializeField] private Texture2D heartK = null;
        [SerializeField] private Texture2D heartA = null;
        [Header("Diamonds")]
        [SerializeField] private Texture2D diamond2 = null;
        [SerializeField] private Texture2D diamond3 = null;
        [SerializeField] private Texture2D diamond4 = null;
        [SerializeField] private Texture2D diamond5 = null;
        [SerializeField] private Texture2D diamond6 = null;
        [SerializeField] private Texture2D diamond7 = null;
        [SerializeField] private Texture2D diamond8 = null;
        [SerializeField] private Texture2D diamond9 = null;
        [SerializeField] private Texture2D diamond10 = null;
        [SerializeField] private Texture2D diamondJ = null;
        [SerializeField] private Texture2D diamondQ = null;
        [SerializeField] private Texture2D diamondK = null;
        [SerializeField] private Texture2D diamondA = null;
        [Header("Clubs")]
        [SerializeField] private Texture2D club2 = null;
        [SerializeField] private Texture2D club3 = null;
        [SerializeField] private Texture2D club4 = null;
        [SerializeField] private Texture2D club5 = null;
        [SerializeField] private Texture2D club6 = null;
        [SerializeField] private Texture2D club7 = null;
        [SerializeField] private Texture2D club8 = null;
        [SerializeField] private Texture2D club9 = null;
        [SerializeField] private Texture2D club10 = null;
        [SerializeField] private Texture2D clubJ = null;
        [SerializeField] private Texture2D clubQ = null;
        [SerializeField] private Texture2D clubK = null;
        [SerializeField] private Texture2D clubA = null;
        [Header("Spades")]
        [SerializeField] private Texture2D spade2 = null;
        [SerializeField] private Texture2D spade3 = null;
        [SerializeField] private Texture2D spade4 = null;
        [SerializeField] private Texture2D spade5 = null;
        [SerializeField] private Texture2D spade6 = null;
        [SerializeField] private Texture2D spade7 = null;
        [SerializeField] private Texture2D spade8 = null;
        [SerializeField] private Texture2D spade9 = null;
        [SerializeField] private Texture2D spade10 = null;
        [SerializeField] private Texture2D spadeJ = null;
        [SerializeField] private Texture2D spadeQ = null;
        [SerializeField] private Texture2D spadeK = null;
        [SerializeField] private Texture2D spadeA = null;
        
        ///////////////////////////////////////////////////Events Section/////////////////////////////////////////////////////
        private void Awake()
        {
            myRawImage.texture = cardBack;
        }
        ///////////////////////////////////////////////////Methods Section/////////////////////////////////////////////////////
        
        public void SetCardData(CardData cardData)
        {
            myCardData = cardData;
            SetCardTexture(myCardData.MySuit, myCardData.MyValue);
        }

        private void SetCardTexture(CardData.Suit mySuit, CardData.Value myValue)
        {
            switch (mySuit)
            {
                case CardData.Suit.Hearts:
                    switch (myValue)
                    {
                        case CardData.Value.Two:
                            myRawImage.texture = heart2;
                            break;
                        case CardData.Value.Three:
                            myRawImage.texture = heart3;
                            break;
                        case CardData.Value.Four:
                            myRawImage.texture = heart4;
                            break;
                        case CardData.Value.Five:
                            myRawImage.texture = heart5;
                            break;
                        case CardData.Value.Six:
                            myRawImage.texture = heart6;
                            break;
                        case CardData.Value.Seven:
                            myRawImage.texture = heart7;
                            break;
                        case CardData.Value.Eight:
                            myRawImage.texture = heart8;
                            break;
                        case CardData.Value.Nine:
                            myRawImage.texture = heart9;
                            break;
                        case CardData.Value.Ten:
                            myRawImage.texture = heart10;
                            break;
                        case CardData.Value.Jack:
                            myRawImage.texture = heartJ;
                            break;
                        case CardData.Value.Queen:
                            myRawImage.texture = heartQ;
                            break;
                        case CardData.Value.King:
                            myRawImage.texture = heartK;
                            break;
                        case CardData.Value.Ace:
                            myRawImage.texture = heartA;
                            break;
                    }
                    break;
                case CardData.Suit.Diamonds:
                    switch (myValue)
                    {
                        case CardData.Value.Two:
                            myRawImage.texture = diamond2;
                            break;
                        case CardData.Value.Three:
                            myRawImage.texture = diamond3;
                            break;
                        case CardData.Value.Four:
                            myRawImage.texture = diamond4;
                            break;
                        case CardData.Value.Five:
                            myRawImage.texture = diamond5;
                            break;
                        case CardData.Value.Six:
                            myRawImage.texture = diamond6;
                            break;
                        case CardData.Value.Seven:
                            myRawImage.texture = diamond7;
                            break;
                        case CardData.Value.Eight:
                            myRawImage.texture = diamond8;
                            break;
                        case CardData.Value.Nine:
                            myRawImage.texture = diamond9;
                            break;
                        case CardData.Value.Ten:
                            myRawImage.texture = diamond10;
                            break;
                        case CardData.Value.Jack:
                            myRawImage.texture = diamondJ;
                            break;
                        case CardData.Value.Queen:
                            myRawImage.texture = diamondQ;
                            break;
                        case CardData.Value.King:
                            myRawImage.texture = diamondK;
                            break;
                        case CardData.Value.Ace:
                            myRawImage.texture = diamondA;
                            break;
                    }
                    break;
                case CardData.Suit.Clubs:
                    switch (myValue)
                    {
                        case CardData.Value.Two:
                            myRawImage.texture = club2;
                            break;
                        case CardData.Value.Three:
                            myRawImage.texture = club3;
                            break;
                        case CardData.Value.Four:
                            myRawImage.texture = club4;
                            break;
                        case CardData.Value.Five:
                            myRawImage.texture = club5;
                            break;
                        case CardData.Value.Six:
                            myRawImage.texture = club6;
                            break;
                        case CardData.Value.Seven:
                            myRawImage.texture = club7;
                            break;
                        case CardData.Value.Eight:
                            myRawImage.texture = club8;
                            break;
                        case CardData.Value.Nine:
                            myRawImage.texture = club9;
                            break;
                        case CardData.Value.Ten:
                            myRawImage.texture = club10;
                            break;
                        case CardData.Value.Jack:
                            myRawImage.texture = clubJ;
                            break;
                        case CardData.Value.Queen:
                            myRawImage.texture = clubQ;
                            break;
                        case CardData.Value.King:
                            myRawImage.texture = clubK;
                            break;
                        case CardData.Value.Ace:
                            myRawImage.texture = clubA;
                            break;
                    }
                    break;
                case CardData.Suit.Spades:
                    switch (myValue)
                    {
                        case CardData.Value.Two:
                            myRawImage.texture = spade2;
                            break;
                        case CardData.Value.Three:
                            myRawImage.texture = spade3;
                            break;
                        case CardData.Value.Four:
                            myRawImage.texture = spade4;
                            break;
                        case CardData.Value.Five:
                            myRawImage.texture = spade5;
                            break;
                        case CardData.Value.Six:
                            myRawImage.texture = spade6;
                            break;
                        case CardData.Value.Seven:
                            myRawImage.texture = spade7;
                            break;
                        case CardData.Value.Eight:
                            myRawImage.texture = spade8;
                            break;
                        case CardData.Value.Nine:
                            myRawImage.texture = spade9;
                            break;
                        case CardData.Value.Ten:
                            myRawImage.texture = spade10;
                            break;
                        case CardData.Value.Jack:
                            myRawImage.texture = spadeJ;
                            break;
                        case CardData.Value.Queen:
                            myRawImage.texture = spadeQ;
                            break;
                        case CardData.Value.King:
                            myRawImage.texture = spadeK;
                            break;
                        case CardData.Value.Ace:
                            myRawImage.texture = spadeA;
                            break;
                    }
                    break;
            }
            
        }


        ///////////////////////////////////////////////////Properties Section/////////////////////////////////////////////////////
       
        public CardData MyCardData
        {
            get => myCardData;
            set => myCardData = value;
        }


        public bool isEqual(CardData parameterCardData)
        {
            if (myCardData.MySuit == parameterCardData.MySuit && myCardData.MyValue == parameterCardData.myValue)
            {
                return true;
            }
            return false;
        }

        public void MovePlayerCardByLocation(SeatLocations seatLocations)
        { 
            switch (seatLocations) 
            { 
                case SeatLocations.Bottom: 
                    MovePlayerCardToUp();
                    break;
                case SeatLocations.BottomLeft:
                    MovePlayerCardToUp();
                    break;
                case SeatLocations.BottomRight: 
                    MovePlayerCardToUp();
                    break;
                case SeatLocations.Top: 
                    MovePlayerCardToDown(); 
                    break;
                case SeatLocations.TopLeft: 
                    MovePlayerCardToDown(); 
                    break;
                case SeatLocations.TopRight:
                    MovePlayerCardToDown(); 
                    break;
                case SeatLocations.Right: 
                    MovePlayerCardToLeft(); 
                    break;
                case SeatLocations.RightTop: 
                    MovePlayerCardToLeft(); 
                    break;
                case SeatLocations.RightBottom: 
                    MovePlayerCardToLeft(); 
                    break;
                case SeatLocations.Left: 
                    MovePlayerCardToRight(); 
                    break;
                case SeatLocations.LeftTop: 
                    MovePlayerCardToRight(); 
                    break;
                case SeatLocations.LeftBottom: 
                    MovePlayerCardToRight(); 
                    break;
                         
            }
        }
        public void MovePlayerCardToUp()
        {
            // Kartı yukarı taşı
            transform.position = new Vector3(transform.position.x, transform.position.y + 10.0f, transform.position.z);

            // 5 saniye sonra kartı eski pozisyonuna geri taşı
            StartCoroutine(MovePlayerCardBackToDown(5.0f));
        }

        IEnumerator MovePlayerCardBackToDown(float delay)
        {
            yield return new WaitForSeconds(delay);

            // Kartı eski pozisyonuna geri taşı
            transform.position = new Vector3(transform.position.x, transform.position.y - 10.0f, transform.position.z);
        }
        public void MovePlayerCardToRight()
        {
            // Kartı sağa taşı
            transform.position = new Vector3(transform.position.x + 10.0f, transform.position.y, transform.position.z);
            
            // 5 saniye sonra kartı eski pozisyonuna geri taşı
            StartCoroutine(MovePlayerCardBackToLeft(5.0f));
        }
        IEnumerator MovePlayerCardBackToLeft(float delay)
        {
            yield return new WaitForSeconds(delay);

            // Kartı eski pozisyonuna geri taşı
            transform.position = new Vector3(transform.position.x - 10.0f, transform.position.y, transform.position.z);
        }
        public void MovePlayerCardToLeft()
        {
            // Kartı sola taşı
            transform.position = new Vector3(transform.position.x - 10.0f, transform.position.y, transform.position.z);
            
            // 5 saniye sonra kartı eski pozisyonuna geri taşı
            StartCoroutine(MovePlayerCardBackToRight(5.0f));
        }
        IEnumerator MovePlayerCardBackToRight(float delay)
        {
            yield return new WaitForSeconds(delay);

            // Kartı eski pozisyonuna geri taşı
            transform.position = new Vector3(transform.position.x + 10.0f, transform.position.y, transform.position.z);
        }
        public void MovePlayerCardToDown()
        {
            // Kartı aşağı taşı
            transform.position = new Vector3(transform.position.x, transform.position.y - 10.0f, transform.position.z);
            
            // 5 saniye sonra kartı eski pozisyonuna geri taşı
            StartCoroutine(MovePlayerCardBackToUp(5.0f));
        }
        IEnumerator MovePlayerCardBackToUp(float delay)
        {
            yield return new WaitForSeconds(delay);

            // Kartı eski pozisyonuna geri taşı
            transform.position = new Vector3(transform.position.x, transform.position.y + 10.0f, transform.position.z);
        }
        
        public void MoveSharedCardToUp()
        {
            // Kartı yukarı taşı
            transform.position = new Vector3(transform.position.x, transform.position.y + 10.0f, transform.position.z);

            // 5 saniye sonra kartı eski pozisyonuna geri taşı
            StartCoroutine(MoveSharedCardBackToDown(5.0f));
        }

        IEnumerator MoveSharedCardBackToDown(float delay)
        {
            yield return new WaitForSeconds(delay);

            // Kartı eski pozisyonuna geri taşı
            transform.position = new Vector3(transform.position.x, transform.position.y - 10.0f, transform.position.z);
        }


        public void Clear()
        {
            myRawImage.texture = cardBack;
        }
    }
}