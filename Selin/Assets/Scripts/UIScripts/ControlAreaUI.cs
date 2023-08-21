using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ControlAreaUI : MonoBehaviour
    {
        ///////////////////////////////////////////////////Variables Section/////////////////////////////////////////////////////
        public static ControlAreaUI instance;
        [SerializeField] private GameObject middleFrame = null;
        
        [SerializeField] private Button openTableCreationOptionButton = null;
        [SerializeField] private GameObject tableCreationOptionFrame = null;
        
        [SerializeField] private Button openUserCreationOptionButton = null;
        [SerializeField] private GameObject userCreationOptionFrame = null;
        ///////////////////////////////////////////////////Events Section/////////////////////////////////////////////////////
        /*
         * Kullanıcı admin değilse bazı buttonlar görünmeyecek bu durumu Start fonkisyonu içinde ayarlamalı!!!
         */
        void Start()
        {
            instance = this;
        }
        ///////////////////////////////////////////////////OpenUserCreationOption Section/////////////////////////////////////////////////////
        /*
         * OpenTableCreationOptionButton OnClick eventine bağlı
         * bu fonksiyon sadece admin ve operator ekranında, masa kurma ayarları arayüzünü açacak
         */
        public void OpenUserCreationOptionUI()
        {
            openUserCreationOptionButton.interactable = false;
            Player.localPlayer.OpenUserCreationOptionUI();
            
        }
        /*
         * açılacak UserCreationOptionUI'ın default değerlerini set eder.
         */
        public void OpenUserCreationOptionUISucces()
        {
            
            userCreationOptionFrame.SetActive(true);
            UserCreationOptionUI.instance.UsernameInputField.text = String.Empty;
            UserCreationOptionUI.instance.PasswordInputField.text = String.Empty;
            UserCreationOptionUI.instance.DepositInputField.text = String.Empty;
            UserCreationOptionUI.instance.RakePercentInputField.text = String.Empty;
            UserCreationOptionUI.instance.FriendInputField.text = String.Empty;
            UserCreationOptionUI.instance.FriendPercentInputField.text = String.Empty;
            UserCreationOptionUI.instance.Username = String.Empty;
            UserCreationOptionUI.instance.Password = String.Empty;
            UserCreationOptionUI.instance.Deposit = 0;
            UserCreationOptionUI.instance.RakePercent = 20;
            UserCreationOptionUI.instance.Friend = String.Empty;
            UserCreationOptionUI.instance.FriendPercent = 10;
            UserCreationOptionUI.instance.UserType = UserTypes.Player;
            UserCreationOptionUI.instance.UserTypeAdminButton.interactable = true;
            UserCreationOptionUI.instance.UserTypeOperatorButton.interactable = true;
            UserCreationOptionUI.instance.UserTypePlayerButton.interactable = true;
            UserCreationOptionUI.instance.BackToMiddleFrameButton.interactable = true;
            UserCreationOptionUI.instance.CreateNewUserButton.interactable = true;
            UserCreationOptionUI.instance.InformationArea.SetActive(false);
            
        }
        ///////////////////////////////////////////////////OpenTableCreationOption Section/////////////////////////////////////////////////////
        /*
         * OpenTableCreationOptionButton OnClick eventine bağlı
         * bu fonksiyon sadece admin ekranında, masa kurma ayarları arayüzünü açacak
         */
        public void OpenTableCreationOptionUI()
        {
            openTableCreationOptionButton.interactable = false;
            Player.localPlayer.OpenTableCreationOptionUI();
            
        }
        
        /*
         * Acılan TableCreationOptionUI default değerlere sahiptir bu defaultların set edildiği yerdir.
         */
        public void OpenTableCreationOptionUISucces()
        {
            
            tableCreationOptionFrame.SetActive(true);
            TableCreationOptionUI.instance.SeatCount = 6;
            TableCreationOptionUI.instance.SmallBind = 5;
            TableCreationOptionUI.instance.Password = null;
            TableCreationOptionUI.instance.HundredSmallBlindButton.interactable = true;
            TableCreationOptionUI.instance.FivetySmallBlindButton.interactable = true;
            TableCreationOptionUI.instance.TwentySmallBlindButton.interactable = true;
            TableCreationOptionUI.instance.TenSmallBlindButton.interactable = true;
            TableCreationOptionUI.instance.FiveSmallBlindButton.interactable = true;
            TableCreationOptionUI.instance.SixSeatButton.interactable = true;
            TableCreationOptionUI.instance.EightSeatButton.interactable = true;
            TableCreationOptionUI.instance.BackToMiddleFrameButton.interactable = true;
            TableCreationOptionUI.instance.CreateNewTableButton.interactable = true;
            
        }
        ///////////////////////////////////////////////////Properties Section/////////////////////////////////////////////////////
        public GameObject MiddleFrame
        {
            get => middleFrame;
            set => middleFrame = value;
        }
        public GameObject TableCreationOptionFrame
        {
            get => tableCreationOptionFrame;
            set => tableCreationOptionFrame = value;
        }
        public Button OpenTableCreationOptionButton
        {
            get => openTableCreationOptionButton;
            set => openTableCreationOptionButton = value;
        }

        public Button OpenUserCreationOptionButton
        {
            get => openUserCreationOptionButton;
            set => openUserCreationOptionButton = value;
        }

        public GameObject UserCreationOptionFrame
        {
            get => userCreationOptionFrame;
            set => userCreationOptionFrame = value;
        }
    }
}