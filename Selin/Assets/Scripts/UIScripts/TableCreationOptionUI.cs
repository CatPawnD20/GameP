using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    /*
     * Masa create edebilmek için açılan özellikler sayfasıdır.
     * aldığı parametrelerle createTable işlemini başlatır.
     */
    public class TableCreationOptionUI : MonoBehaviour
    {
        public static TableCreationOptionUI instance;
        [SerializeField] private Button fiveSmallBlindButton = null;
        [SerializeField] private Button tenSmallBlindButton = null;
        [SerializeField] private Button twentySmallBlindButton = null;
        [SerializeField] private Button fivetySmallBlindButton = null;
        [SerializeField] private Button hundredSmallBlindButton = null;
        [SerializeField] private Button sixSeatButton = null;
        [SerializeField] private Button eightSeatButton = null;
        [SerializeField] private Button createNewTableButton = null;
        [SerializeField] private Button backToMiddleFrameButton = null;
        [SerializeField] private TMP_InputField passwordInputField = null;
        [SerializeField] private Canvas errorCanvas = null;
        private int seatCount;
        private int smallBlind;
        private string password;

        private void Start()
        {
            instance = this;
        }
        

        /*
         * Yeni masa yaratma işlemini başlatacak fonksiyon "CreateButton"'a bağlı
         */
        public void createNewTable()
        {
            createNewTableButton.interactable = false;
            if (string.IsNullOrEmpty(passwordInputField.text) || string.IsNullOrWhiteSpace(passwordInputField.text))
            {
                password = null;
            }
            else
            {
                password = passwordInputField.text;
            }
            Player.localPlayer.createNewTable(Player.localPlayer.LocalPlayerName,seatCount,smallBlind,password);
            
        }

        public void CreateNewTableSucces(TableUIDataStruct tableUIDataStruct)
        {
            GameObject tableUIClone = SpawnManager.instance.SpawnTableUI(tableUIDataStruct);
            
            SpawnManager.instance.SetParentToTableUIClone(SaloonAreaUI.instance.TableUIParent,tableUIClone);
            //Yeni eklendi documents de guncelle
            SpawnManager.instance.UpdateTableUICloneMap(tableUIClone);
            Debug.Log("Masa oluşturuldu");
            backToMiddleFrame();
        }
        
        public void CreateNewTableFailErrorTableListFull()
        {
            errorCanvas.enabled = true;
        }

        public void errorBackToLobby()
        {
            errorCanvas.enabled = false;
            backToMiddleFrame();
        }
        
        public void backToMiddleFrame()
        {
            ControlAreaUI.instance.OpenTableCreationOptionButton.interactable = true;
            backToMiddleFrameButton.interactable = false;
            ControlAreaUI.instance.MiddleFrame.SetActive(true);
            ControlAreaUI.instance.TableCreationOptionFrame.SetActive(false);
        }

        public void hundredSmallBlind()
        {
            smallBlind = 100;
            Debug.Log("TableCreationOptionUI.cs -->>> hundredSmallBlind -->>> smallBlind  : "+ smallBlind);
            hundredSmallBlindButton.interactable = false;
            fivetySmallBlindButton.interactable = true;
            twentySmallBlindButton.interactable = true;
            tenSmallBlindButton.interactable = true;
            fiveSmallBlindButton.interactable = true;
        }
        public void fivetySmallBlind()
        {
            smallBlind = 50;
            hundredSmallBlindButton.interactable = true;
            fivetySmallBlindButton.interactable = false;
            twentySmallBlindButton.interactable = true;
            tenSmallBlindButton.interactable = true;
            fiveSmallBlindButton.interactable = true;
            Debug.Log("TableCreationOptionUI.cs -->>> fivetySmallBlind -->>> smallBlind  : "+ smallBlind);
        }
        public void twentySmallBlind()
        {
            smallBlind = 20;
            hundredSmallBlindButton.interactable = true;
            fivetySmallBlindButton.interactable = true;
            twentySmallBlindButton.interactable = false;
            tenSmallBlindButton.interactable = true;
            fiveSmallBlindButton.interactable = true;
            Debug.Log("TableCreationOptionUI.cs -->>> twentySmallBlind -->>> smallBlind  : "+ smallBlind);
        }
        public void tenSmallBlind()
        {
            smallBlind = 10;
            hundredSmallBlindButton.interactable = true;
            fivetySmallBlindButton.interactable = true;
            twentySmallBlindButton.interactable = true;
            tenSmallBlindButton.interactable = false;
            fiveSmallBlindButton.interactable = true;
            Debug.Log("TableCreationOptionUI.cs -->>> tenSmallBlind -->>> smallBlind  : "+ smallBlind);
        }
        public void fiveSmallBlind()
        {
            smallBlind = 5;
            hundredSmallBlindButton.interactable = true;
            fivetySmallBlindButton.interactable = true;
            twentySmallBlindButton.interactable = true;
            tenSmallBlindButton.interactable = true;
            fiveSmallBlindButton.interactable = false;
            Debug.Log("TableCreationOptionUI.cs -->>> fiveSmallBlind -->>> smallBlind  : "+ smallBlind);
        }
        
        public void sixSeat()
        {
            seatCount = 6;
            sixSeatButton.interactable = false;
            eightSeatButton.interactable = true;
            Debug.Log("TableCreationOptionUI.cs -->>> sixSeat -->>> seatCount  : "+ seatCount);
        }
        public void eightSeat()
        {
            seatCount = 8;
            sixSeatButton.interactable = true;
            eightSeatButton.interactable = false;
            Debug.Log("TableCreationOptionUI.cs -->>> eightSeat -->>> seatCount  : "+ seatCount);
        }

        
        public string Password
        {
            get => password;
            set => password = value;
        }

        public int SeatCount
        {
            get => seatCount;
            set => seatCount = value;
        }

        public int SmallBind
        {
            get => smallBlind;
            set => smallBlind = value;
        }

        public Button FiveSmallBlindButton
        {
            get => fiveSmallBlindButton;
            set => fiveSmallBlindButton = value;
        }

        public Button TenSmallBlindButton
        {
            get => tenSmallBlindButton;
            set => tenSmallBlindButton = value;
        }

        public Button TwentySmallBlindButton
        {
            get => twentySmallBlindButton;
            set => twentySmallBlindButton = value;
        }

        public Button FivetySmallBlindButton
        {
            get => fivetySmallBlindButton;
            set => fivetySmallBlindButton = value;
        }

        public Button HundredSmallBlindButton
        {
            get => hundredSmallBlindButton;
            set => hundredSmallBlindButton = value;
        }

        public Button SixSeatButton
        {
            get => sixSeatButton;
            set => sixSeatButton = value;
        }

        public Button EightSeatButton
        {
            get => eightSeatButton;
            set => eightSeatButton = value;
        }

        public Button CreateNewTableButton
        {
            get => createNewTableButton;
            set => createNewTableButton = value;
        }

        public Button BackToMiddleFrameButton
        {
            get => backToMiddleFrameButton;
            set => backToMiddleFrameButton = value;
        }
        
    }
}