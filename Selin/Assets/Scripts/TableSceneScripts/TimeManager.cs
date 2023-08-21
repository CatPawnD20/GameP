using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class TimeManager : MonoBehaviour
    {
        ///////////////////////////////////////////////////Variables Section/////////////////////////////////////////////////////
        public static TimeManager instance;

        private void Awake()
        {
            instance = this;
        }
        [Header("JoinTable")]
        /*
         * loadTableSceneDelayTime : TableScene'in yüklenmesi için bekletilme süresi. JoinTable işlemi yapılırken kullanılır.
         */
        [SerializeField] private float loadTableSceneDelayTime = 2.0f;
     
        /*
         * spawnTableToolsDelayTime : TableScene'in yüklenmesi tamamlandıktan sonra, TableTools'un spawn edilmesi için bekletilme süresi.
         */
        [SerializeField] private float spawnTableToolsDelayTime = 0.5f;
        
        [SerializeField] private float spawnTableAndGameSceneDelayTime = 0.1f;
        
        [Header("Game")]
        
        /*
         * showCardsDelayTime : Oyuncunun kartlarını görmesi için bekletilme süresi.
         */
        [SerializeField] private float showCardsDelayTime = 5.0f;
        
        /*
         * nextMoveDelayTime : Oyuncudan Server'a gelen hamle isteğinin, İşleme koyulmadan önceki bekletilme süresi.
         */
        [SerializeField] private float nextMoveDelayTime = 0.1f;
        
        /*
         * correctionMoveDelayTime : Serverdan oyuncuya yollanan hamle işlemininden önce gerekliyse düzeltme hamlesi yollanmaktadır.
         * Bu düzeltme hamlesini ile hamle işleme koyulmadan önceki bekletilme süresi.
         */
        [SerializeField] private float correctionMoveDelayTime = 4.0f;
        
        /*
         * correctionMoveDelayTime : Oyunun son evresinde masadaki pulların pota atılması için gerekli olan animasyonun başlaması için bekletilme süresi.
         */
        [SerializeField] private float moveChipsToPotDelayTime = 2.5f;
        
        /*
         * lastMoveDelayTime : Son oyuncu hamlesinin yollanması ile animasyonun başlaması arasındaki bekletilme süresi.
         */
        [SerializeField] private float lastMoveDelayTime = 0.5f;
        
        [Header("Animation")]
        
        /*
         * clashAnimationDelayTime : Clash animasyonundan sonra begin new turn arasındaki bekletilme süresi.
         */
        [SerializeField] private float clashAnimationDelayTime = 10.0f;
        
        /*
         * animationStartDelayTime : Son oyuncu hamlesinin yollanması ile animasyonun başlaması arasındaki bekletilme süresi.
         */
        [SerializeField] private float animationStartDelayTime = 2.0f;
        
        /*
         * leaveGameDelayTime : Oyuncunun leaveGame isteği işlenirken. LeaveGame isteğinin işlenmesi için bekletilme süresi.
         */
        [SerializeField] private float leaveGameDelayTime = 0.5f;
        
        /*
         * tableSceneDestroyCompleteTime : TableScene ve GameScene'in yok edilmesi için bekletilme süresi.
         */
        [SerializeField] private float tableSceneDestroyCompleteTime = 0.2f;
        
        [SerializeField] private float beginNewTurnDelayTime = 2.0f;
        
        ///////////////////////////////////////////////////Property Section/////////////////////////////////////////////////////
        
        public float AnimationStartDelayTime => animationStartDelayTime;
        public float NextMoveDelayTime => nextMoveDelayTime;
        public float CorrectionMoveDelayTime => correctionMoveDelayTime;
        public float LeaveGameDelayTime => leaveGameDelayTime;
        public float TableSceneDestroyCompleteTime => tableSceneDestroyCompleteTime;
        public float BeginNewTurnDelayTime => beginNewTurnDelayTime;
        public float LoadTableSceneDelayTime => loadTableSceneDelayTime;
        public float SpawnTableToolsDelayTime => spawnTableToolsDelayTime;
        public float SpawnTableAndGameSceneDelayTime => spawnTableAndGameSceneDelayTime;
        public float MoveChipsToPotDelayTime => moveChipsToPotDelayTime;
        public float LastMoveDelayTime => lastMoveDelayTime;

        public float ClashAnimationDelayTime => clashAnimationDelayTime;
        
        public float ShowCardsDelayTime => showCardsDelayTime;
    }
}