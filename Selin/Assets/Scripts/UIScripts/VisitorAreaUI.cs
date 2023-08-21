using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TMPro;
using UnityEngine;

/*
 * Lobby ekranında sol tarafta bulunan playerList arayüzüne bağlıdır.
 * Update içinde kendi içinde tuttuğu text'i VisitorArea'nın syncPlayerListi ile eşler
 * Bu durum ilerde düzeltilebilir.
 */

namespace Assets.Scripts
{
    public class VisitorAreaUI : MonoBehaviour
    {
        public static VisitorAreaUI instance;
        
        [SerializeField] public TextMeshProUGUI onlinePlayerNameText = null;
        
        
        /*
         * Lobby ekranına geçişte 
         */
        public void Start()
        {
            instance = this;
            if (VisitorArea.instance != null)
            {
                
                Debug.Log("VisitorAreaUI.cs -->>> onStart -->>> ");
                onlinePlayerNameText.text = SyncListListToString(VisitorArea.instance.onlinePlayerList);
            }
            
        }

        private void Update()
        {
            if (VisitorArea.instance != null)
            {
                if (VisitorArea.instance.onlinePlayerList.Count != 0)
                {
                    onlinePlayerNameText.text = SyncListListToString(VisitorArea.instance.onlinePlayerList);
                }
            }
            
            
        }

        private string SyncListListToString(SyncList<string> onlinePlayerList)
        {
            var dummyString = new System.Text.StringBuilder();
            foreach (string playerName in onlinePlayerList)
            {
                dummyString.AppendLine(playerName);
            }
            return dummyString.ToString();
        }
        
        

       
    }
}