using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/*
 * LobbyScene için temel sınıftır.
 * 
 */
namespace Assets.Scripts
{
    public class LobbyUI : MonoBehaviour
    {
        public static LobbyUI instance;
        [SerializeField]private string username;
        [SerializeField]private float balance;
        [SerializeField] private TextMeshProUGUI localPlayerNameText = null;
        [SerializeField] private TextMeshProUGUI localPlayerBalanceText = null;
        [SerializeField] private GameObject controlPanel = null;
        [SerializeField] private GameObject operatorPanel = null;
        [SerializeField] private GameObject playerPanel = null;
        
        
        void Start()
        {
            instance = this;
            localPlayerNameText.text = username;
            localPlayerBalanceText.text = balance.ToString();
        }

        /*
         * LobbyScene'e gönderilen değerler ile değişkenlerini günceller.
         * Yapılmadığı durumda değerler değişse bile start fonksiyonunda set edildiği şekilde görünür.
         */
        private void Update()
        {
            localPlayerNameText.text = username;
            localPlayerBalanceText.text = balance.ToString();
        }

        /*
         * Login işleminin ardından LocalPlayer ekranında masa spawn eden fonksiyondur.
         * Masa sayısı kadar çağırılır.
         */
        public void SpawnTableUISucces(TableUIDataStruct tableUIDataStruct)
        {
            GameObject newTableUIClone = SpawnManager.instance.SpawnTableUI(tableUIDataStruct);
            SpawnManager.instance.AddCloneToTableUICloneMap(tableUIDataStruct.matchId, newTableUIClone);
            SpawnManager.instance.SetParentToTableUIClone(SaloonAreaUI.instance.TableUIParent,newTableUIClone);
        }
        
        /*
         * Login işlemi başarılı oldugunda çağılır.
         * Lobbyde set edilecek bilgiler için alternatif bir çözüm
         * UserType'a göre ek özellikleri barındıran panelleri aktif eder
         */
        public void loginSucces(string username, float balance, UserTypes userType)
        {
            this.username = username;
            this.balance = balance;
            if (userType == UserTypes.Admin)
            {
                controlPanel.SetActive(true);
                operatorPanel.SetActive(false);
                playerPanel.SetActive(false);
            }
            if (userType == UserTypes.Operator)
            {
                controlPanel.SetActive(false);
                operatorPanel.SetActive(true);
                playerPanel.SetActive(false);
            }
            if (userType == UserTypes.Player)
            {
                controlPanel.SetActive(false);
                operatorPanel.SetActive(false);
                playerPanel.SetActive(true);
            }
        }
        
        public string Username
        {
            get => username;
            set => username = value;
        }

        public float Balance
        {
            get => balance;
            set => balance = value;
        }

        
    }
}