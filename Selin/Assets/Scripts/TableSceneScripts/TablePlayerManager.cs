using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    /*
     * TableScene'e geçiş yapmış userların aktif oyuncu veya observer farketmeksizin yönetimini yapacak olan sınıftır.
     * Tuttuğu Listeler
     * ObserverList -->> TableScene'e geçiş yapmış fakat masaya oturmamış playerları ifade eder.
     * tablePlayerList -->> TableScene'e geçiş yapmış tüm userları ifade eder. (masadakiler ve observerlar)
     * Bu listeler
     *              -->> server tarafında ADD-REMOVE functionları ile idare edilirken
     *              -->> client tarafında UPDATE functionları ile idare edilir. Tüm update işlemleri TableGameManager'in List'lerini temel alır .
     */
    public class TablePlayerManager : MonoBehaviour
    {
        public static TablePlayerManager instance;
        [SerializeField] private string syncID;
        [SerializeField] private List<string> observerList = new List<string>();
        [SerializeField] private List<string> tablePlayerList = new List<string>();
        
        
        ///////////////////////////////////////////////////Events Section/////////////////////////////////////////////////////
        private void Start()
        {
            instance = this;
        }
        
        ///////////////////////////////////////////////////TablePlayerList Section/////////////////////////////////////////////////////
        public void ClearUpdateTablePlayerList(List<string> players)
        {
            tablePlayerList.Clear();
            foreach (var player in players)
            {
                tablePlayerList.Add(player);
            }
        }
        public bool AddPlayerToTablePlayerList(string playerName)
        {
            if (tablePlayerList.Contains(playerName))
            {
                Debug.LogWarning("TablePlayerManager.cs -->>> SyncID : " + SyncID + " -->>>AddPlayerToTablePlayerList" +
                                 "Player is already exist in TablePlayerList!!! : PlayerName -> " + playerName);
                return false;
            }
            else
            {
                tablePlayerList.Add(playerName);
                return true;
            }
            
        }
        public bool RemovePlayerFromTablePlayerList(string playerName)
        {
            if (tablePlayerList.Contains(playerName))
            {
                tablePlayerList.Remove(playerName);
                return true;
            }
            else
            {
                Debug.LogWarning("TablePlayerManager.cs -->>> SyncID : " + SyncID + "-->>>RemovePlayerFromTablePlayerList" +
                                 "Player does NOT exist in TablePlayerList!!! : PlayerName -> " + playerName);
                return false;
            }
        }
        public bool isPlayerOnTablePlayerList(string playerName)
        {
            if (tablePlayerList.Contains(playerName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        ///////////////////////////////////////////////////ObserverList Section/////////////////////////////////////////////////////
        public bool AddPlayerToObserverList(string playerName)
        {
            if (observerList.Contains(playerName))
            {
                Debug.LogWarning("TablePlayerManager.cs -->>> SyncID : " + SyncID + "-->>>AddPlayerToObserverList" +
                                 "Player is already exist in ObserverList!!! : PlayerName -> " + playerName);
                return false;
            }
            else
            {
                observerList.Add(playerName);
                return true;
            }
            
        }
        public bool RemovePlayerFromObserverList(string playerName)
        {
            if (observerList.Contains(playerName))
            {
                observerList.Remove(playerName);
                return true;
            }
            else
            {
                Debug.LogWarning("TablePlayerManager.cs -->>> SyncID : " + SyncID + "-->>>RemovePlayerFromObserverList" +
                                 "Player does NOT exist in ObserverList!!! : PlayerName -> " + playerName);
                return false;
            }
        }
        public bool isPlayerOnObserverList(string playerName)
        {
            if (observerList.Contains(playerName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        ///////////////////////////////////////////////////Properties Section/////////////////////////////////////////////////////
        public List<string> ObserverList
        {
            get => observerList;
            set => observerList = value;
        }
        public List<string> TablePlayerList
        {
            get => tablePlayerList;
            set => tablePlayerList = value;
        }
        public string SyncID
        {
            get => syncID;
            set => syncID = value;
        }


        public List<string> GetObserverList()
        {
            return observerList;
        }
    }
}