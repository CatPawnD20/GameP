using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Assets.Scripts
{
    public class Bet : MonoBehaviour
    {
        [SerializeField] private Button betButton = null;
        [SerializeField] private TMP_Text betText = null;

        private void Awake()
        {
            betText.text = string.Empty;
            betButton.interactable = false;
        }
        
        public void SetBetAmount(float amount)
        {
            betText.text = amount.ToString("N1");
        }

        public Button BetButton
        {
            get => betButton;
            set => betButton = value;
        }

        public TMP_Text BetText
        {
            get => betText;
            set => betText = value;
        }


        public void Clear()
        {
            betText.text = string.Empty;
        }
    }
}