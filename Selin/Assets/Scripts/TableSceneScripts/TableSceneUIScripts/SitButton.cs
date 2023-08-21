using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class SitButton : MonoBehaviour
    {
        ///////////////////////////////////////////////////Variables Section/////////////////////////////////////////////////////
        public static SitButton instance;
        [SerializeField] private SeatLocations mySeatLocation;
        [SerializeField] private Button openDepositAreaButton = null;
        ///////////////////////////////////////////////////Events Section/////////////////////////////////////////////////////
        // Start is called before the first frame update
        void Start()
        {
            instance = this;
        }
        ///////////////////////////////////////////////////OpenDepositArea Section/////////////////////////////////////////////////////
        /*
         * SitButtonPrefab'ın openDepositOptionsButton OnClick eventine bağlı
         */
        public void OpenDepositArea()
        {
            //Önce kendini ardından tüm buttonları false yapıyor
            openDepositAreaButton.interactable = false;
            TableGameManager.instance.LeaveTableAreaOpenCloseLeaveTableArea(false,string.Empty,true,true,true);
            TableGameManager.instance.SitButtonSetInterectable(false);
            
            Player.localPlayer.OpenDepositOptions(mySeatLocation);
        }
        
        ///////////////////////////////////////////////////Properties Section/////////////////////////////////////////////////////
        public Button OpenDepositAreaButton => openDepositAreaButton;

        public SeatLocations MySeatLocation
        {
            get => mySeatLocation;
            set => mySeatLocation = value;
        }
    }
}