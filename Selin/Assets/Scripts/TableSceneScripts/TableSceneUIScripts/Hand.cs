using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts
{
    public class Hand : MonoBehaviour
    {
        [SerializeField] private GameObject card1;
        [SerializeField] private GameObject card2;
        [SerializeField] private GameObject card3;
        [SerializeField] private GameObject card4;
        
        
        private List<GameObject> cardList = new List<GameObject>();
        private List<int> shakenCards = new List<int>();

        private void Awake()
        {
            cardList.Add(card1);
            cardList.Add(card2);
            cardList.Add(card3);
            cardList.Add(card4);
            Debug.Log("Hand.cs -->>> awake");
        }
        

        public void SetHandActivity(bool active)
        {
            foreach (var card in cardList)
            {
                card.SetActive(active);
            }
        }

        public void SetHand(List<CardData> myHand)
        {
            for (int i = 0; i < myHand.Count; i++)
            {
                cardList[i].GetComponent<Card>().SetCardData(myHand[i]);
            }
        }
        

        public GameObject Card1 => card1;

        public GameObject Card2 => card2;

        public GameObject Card3 => card3;

        public GameObject Card4 => card4;

        public List<GameObject> CardList => cardList;

        public void ShakeWinningCardsFromPlayerHand(List<CardData> winnerCards, SeatLocations seatLocation)
        {
            foreach (var winnerCard in winnerCards)
            {
                foreach (var card in cardList)
                {
                    if (card.GetComponent<Card>().isEqual(winnerCard))
                    {
                        shakenCards.Add(cardList.IndexOf(card));
                        card.GetComponent<Card>().MovePlayerCardByLocation(seatLocation);
                    }
                }
            }
        }

        public void Clear()
        {
            foreach (var card in cardList)
            {
                card.GetComponent<Card>().Clear();
            }
        }

        public void OpenHand(List<CardData> winnerCards)
        {
            
        }
    }
}