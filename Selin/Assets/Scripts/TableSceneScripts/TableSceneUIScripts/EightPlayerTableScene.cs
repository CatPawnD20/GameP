using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class EightPlayerTableScene : MonoBehaviour
    {
        ///////////////////////////////////////////////////Variables Section/////////////////////////////////////////////////////
        public static EightPlayerTableScene instance;
        private int deposit = 0;
        [SerializeField] private Transform bottomRightParent;
        [SerializeField] private Transform bottomLeftParent;
        [SerializeField] private Transform leftTopParent;
        [SerializeField] private Transform leftBottomParent;
        [SerializeField] private Transform topLeftParent;
        [SerializeField] private Transform topRightParent;
        [SerializeField] private Transform rightTopParent;
        [SerializeField] private Transform rightBottomParent;
        [Header("DepositArea")]
        [SerializeField] private GameObject depositArea;
        [SerializeField] private TMP_InputField depositInputField;
        [SerializeField] private Button depositSitButton;
        [SerializeField] private TMP_Text depositInformationText;
        [SerializeField] private Button depositCancelButton;
        [Header("LeaveTable")]
        [SerializeField] private GameObject leaveTableArea;
        [SerializeField] private TMP_Text leaveTableInformationText;
        [SerializeField] private Button leaveTableOpenConfirmationButton;
        [SerializeField] private Button leaveTableOkButton;
        [SerializeField] private Button leaveTableCancelButton;
        ///////////////////////////////////////////////////Events Section/////////////////////////////////////////////////////
        void Start()
        {
            instance = this;
        }
        ///////////////////////////////////////////////////LeaveTable Section/////////////////////////////////////////////////////
        public void OpenCloseLeaveTableArea(bool area,string msg,bool openConfirmationButton,bool okButton,bool cancelButton)
        {
            leaveTableArea.SetActive(area);
            leaveTableInformationText.text = msg;
            leaveTableOpenConfirmationButton.interactable = openConfirmationButton;
            leaveTableOkButton.interactable = okButton;
            leaveTableCancelButton.interactable = cancelButton;
        }
        
        /*
         * LeaveTableOpenConfirmationButton onClick eventine bağlıdır.
         */
        public void openLeaveTableConfirmation()
        {
            leaveTableOpenConfirmationButton.interactable = false;
            OpenCloseLeaveTableArea(true,"Masadan Ayrıl",false,true,true);
            depositArea.SetActive(false);
            //GameUISpawnManager.instance.ActivateDeactivateSitButtons(false);
        }
        
        /*
         * leaveTableOkButton onClick eventine bağlıdır.
         */
        public void leaveTable()
        {
            leaveTableOkButton.interactable = false;
            OpenCloseLeaveTableArea(false,"Masadan Ayrıl",true,false,false);
            Player.localPlayer.LeaveTable();
        }
        
        /*
         * leaveTableCancelButton onClick eventine bağlıdır
         */
        public void cancelLeaveTable()
        {
            leaveTableCancelButton.interactable = false;
            OpenCloseLeaveTableArea(false,"Masadan Ayrıl",true,true,false);
            //GameUISpawnManager.instance.ActivateDeactivateSitButtons(true);
        }
        ///////////////////////////////////////////////////DepositArea Section/////////////////////////////////////////////////////

        /*
         * DepositArea'nın depositSitButton onClick eventine bağlıdır.
         */
        public void depositSit()
        {
            depositSitButton.interactable = false;
            depositCancelButton.interactable = false;
            deposit = Convert.ToInt32(DepositInputField.text);
            TableGameManager.instance.joinGame(deposit);
            depositInputField.text = String.Empty;
        }
        
        /*
         * DepositArea'nın depositCancelButton onClick eventine bağlıdır.
         */
        public void depositCancel()
        {
            depositCancelButton.interactable = false;
            depositArea.SetActive(false);
            
        }
        ///////////////////////////////////////////////////Properties Section/////////////////////////////////////////////////////
        public GameObject LeaveTableArea => leaveTableArea;

        public TMP_Text LeaveTableInformationText => leaveTableInformationText;

        public Button LeaveTableOpenConfirmationButton => leaveTableOpenConfirmationButton;

        public Button LeaveTableOkButton => leaveTableOkButton;

        public Button LeaveTableCancelButton => leaveTableCancelButton;

        public GameObject DepositArea
        {
            get => depositArea;
            set => depositArea = value;
        }

        public TMP_InputField DepositInputField
        {
            get => depositInputField;
            set => depositInputField = value;
        }

        public Button DepositSitButton
        {
            get => depositSitButton;
            set => depositSitButton = value;
        }

        public TMP_Text DepositInformationText
        {
            get => depositInformationText;
            set => depositInformationText = value;
        }

        public Button DepositCancelButton
        {
            get => depositCancelButton;
            set => depositCancelButton = value;
        }

        public Transform BottomRightParent => bottomRightParent;

        public Transform BottomLeftParent => bottomLeftParent;

        public Transform LeftTopParent => leftTopParent;

        public Transform LeftBottomParent => leftBottomParent;

        public Transform TopLeftParent => topLeftParent;

        public Transform TopRightParent => topRightParent;

        public Transform RightTopParent => rightTopParent;

        public Transform RightBottomParent => rightBottomParent;
    }
}