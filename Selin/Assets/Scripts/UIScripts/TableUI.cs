using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class TableUI : MonoBehaviour
    {
        ///////////////////////////////////////////////////Variables Section/////////////////////////////////////////////////////
        public static TableUI instance;
        private string inputPassword;
        [Header("Variables")]
        [SerializeField] private string matchID;
        [SerializeField] private int seatCount;
        [SerializeField] private float smallBlind;
        [SerializeField] private float minDeposit;
        [SerializeField] private bool isPublic;
        [Header("TableUI UIElements")]
        //[SerializeField] private TextMeshProUGUI currentPlayerCountText = null;
        [SerializeField] private TextMeshProUGUI seatCountText = null;
        [SerializeField] private TextMeshProUGUI smallBlindText = null;
        [SerializeField] private TextMeshProUGUI minDepositText = null;
        [SerializeField] private TMP_InputField passwordInputField;
        [Header("TableUI Layers")]
        [SerializeField] private GameObject tableDetailsCanvas = null;
        [SerializeField] private GameObject passwordCanvas = null;
        [SerializeField] private GameObject errorCanvas = null;
        [Header("TableUI Buttons")]
        [SerializeField] private Button openPasswordOrJoinTableButton = null;
        [SerializeField] private Button cancelButton = null;
        [SerializeField] private Button joinTableButton = null;
        [SerializeField] private Button backToPasswordButton = null;
        ///////////////////////////////////////////////////Events Section/////////////////////////////////////////////////////
        void Start()
        {
            IEnumerator coroutine = waitForUIDataSet(0.5f);
            StartCoroutine(coroutine);
            instance = this;
        }
        IEnumerator waitForUIDataSet( float second)
        {
            yield return new WaitForSeconds(second);
            seatCountText.text = SeatCount.ToString();
            smallBlindText.text = smallBlind.ToString();
            minDepositText.text = minDeposit.ToString();
            
            yield return null;
        }
        ///////////////////////////////////////////////////Functions Section/////////////////////////////////////////////////////
        /*
         * TableUIClone'a tıklandığında masaya giriş işlemini başlatacak button
         * public olup olmama durumunu kontrol ederek ilgili işlemi yapar
         */
        public void OpenPasswordOrJoinTable()
        {
            openPasswordOrJoinTableButton.interactable = false;
            SpawnManager.instance.ActivateDeactivateAllTableUIClones(false);
            if (isPublic)
            {
                JoinTable();
            }
            else
            {
                OpenPasswordCanvas();
            }
        }

        public void Cancel()
        {
            openPasswordOrJoinTableButton.interactable = true;
            cancelButton.interactable = false;
            joinTableButton.interactable = true;
            SpawnManager.instance.ActivateDeactivateAllTableUIClones(true);
            
            tableDetailsCanvas.SetActive(true);
            passwordCanvas.SetActive(false);
        }

        /*
         * Password canvasındaki Join buttonuna bağlıdır.
         * Ayrıca masa public ise TableUIclone'a tıklanıldıgında da bu fonksiyon calısır.
         * password null veya empty ise password'u null olarak set eder.
         * Password var ise encrpyt eder
         * ardından bilgileri servera iletilmek üzere playerScripte yollar.
         */
        public void JoinTable()
        {
            joinTableButton.interactable = false;
            SpawnManager.instance.ActivateDeactivateAllTableUIClones(false);
            if (isPublic)
            {
                inputPassword = null;
                
                Player.localPlayer.JoinTable(inputPassword,matchID);
            }
            else
            {
                if (string.IsNullOrEmpty(passwordInputField.text) || string.IsNullOrWhiteSpace(passwordInputField.text))
                {
                    inputPassword = null;
                    Player.localPlayer.JoinTable(inputPassword,matchID);
                }
                else
                {
                    inputPassword = passwordInputField.text;
                    string encryptedInputPassword = Decryptor.encrypt(inputPassword);
                    Player.localPlayer.JoinTable(encryptedInputPassword,matchID);
                }
                
            }
        }

        private void OpenPasswordCanvas()
        {
            cancelButton.interactable = true;
            joinTableButton.interactable = true;
            passwordInputField.text = string.Empty;
            tableDetailsCanvas.SetActive(false);
            passwordCanvas.SetActive(true);
            
        }
        private void closePasswordCanvas()
        {
            cancelButton.interactable = true;
            joinTableButton.interactable = true;
            passwordInputField.text = string.Empty;
            tableDetailsCanvas.SetActive(true);
            passwordCanvas.SetActive(false);
        }

        public void OpenErrorCanvas()
        {
            passwordCanvas.SetActive(false);
            errorCanvas.SetActive(true);
            backToPasswordButton.interactable = true;
        }
        
        /*
         * ErrorCanvas'ın backButton'una bağlı
         */
        public void BackToPasswordCanvas()
        {
            backToPasswordButton.interactable = false;
            errorCanvas.SetActive(false);
            OpenPasswordCanvas();
        }

        public void SuccesJoinTable(string matchID)
        {
            closePasswordCanvas();
            openPasswordOrJoinTableButton.interactable = true;
            SpawnManager.instance.ActivateDeactivateAllTableUIClones(true);
            Player.localPlayer.Sync(matchID);
        }

        ///////////////////////////////////////////////////Properties Section/////////////////////////////////////////////////////
        public GameObject TableDetailsCanvas => tableDetailsCanvas;

        public GameObject PasswordCanvas => passwordCanvas;

        public GameObject ErrorCanvas => errorCanvas;

        public Button OpenPasswordOrJoinTableButton => openPasswordOrJoinTableButton;

        public int SeatCount
        {
            get => seatCount;
            set => seatCount = value;
        }

        public float SmallBlind
        {
            get => smallBlind;
            set => smallBlind = value;
        }

        public float MinDeposit
        {
            get => minDeposit;
            set => minDeposit = value;
        }

        public bool IsPublic
        {
            get => isPublic;
            set => isPublic = value;
        }

        public string MatchID
        {
            get => matchID;
            set => matchID = value;
        }

        
    }
    
}