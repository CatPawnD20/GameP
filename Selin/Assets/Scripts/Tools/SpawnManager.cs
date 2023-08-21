using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.Linq;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    /*
     * Spawn işlemlerini kontrol eder.
     * Spawn edilecek GO'yu tutar
     * public readonly SyncList<string> tableCloneMatchIDSyncList = new SyncList<string>(); spawn edilmiş nesnelerin matchID'lerini tutar
     * public Dictionary<string, TableUIDataStruct> tableUIDataStructMap = new Dictionary<string, TableUIDataStruct>(); -->> spawn edebilmek için gerekli bilgileri tutar
     */
    public class SpawnManager : NetworkBehaviour
    {
        ///////////////////////////////////////////////////Variable Section/////////////////////////////////////////////////////
        public static SpawnManager instance;
        private NetworkMatch networkMatch;
        private TableUIDataStruct dummyStruct = default;
        
        
        public Dictionary<string, TableUIDataStruct> tableUIDataStructMap = new Dictionary<string, TableUIDataStruct>();
        public readonly SyncList<string> tableCloneMatchIDSyncList = new SyncList<string>();
        public Dictionary<string, GameObject> tableUICloneMap = new Dictionary<string, GameObject>();
        
        public Dictionary<string, GameObject> tablePlayerManagerMap = new Dictionary<string, GameObject>();
        public Dictionary<string, GameObject> gamePlayerManagerMap = new Dictionary<string, GameObject>();
        public Dictionary<string, GameObject> tableGameManagerMap = new Dictionary<string, GameObject>();
        public Dictionary<string, GameObject> gamePlayerSpawnManagerMap = new Dictionary<string, GameObject>();
        public Dictionary<string, GameObject> gameUISpawnManagerMap = new Dictionary<string, GameObject>();
        public Dictionary<string, GameObject> dealerMap = new Dictionary<string, GameObject>();
        public Dictionary<string, GameObject> chipManagerMap = new Dictionary<string, GameObject>();
        public Dictionary<string, GameObject> turnManagerMap = new Dictionary<string, GameObject>();
        public Dictionary<string, GameObject> omahaEngineMap = new Dictionary<string, GameObject>();
        
        
        [Header("SyncData")]
        [SerializeField] private string syncID;
        [SerializeField] private string networkMatchID;
        [Header("Spawnable")]
        [SerializeField] private GameObject tableUIPrefab;
        [SerializeField] private GameObject tablePlayerManager;
        [SerializeField] private GameObject gamePlayerManager;
        [SerializeField] private GameObject tableGameManager;
        [SerializeField] private GameObject gamePlayerSpawnManager;
        [SerializeField] private GameObject gameUISpawnManager;
        [SerializeField] private GameObject dealer;
        [SerializeField] private GameObject chipManager;
        [SerializeField] private GameObject turnManager;
        [SerializeField] private GameObject omahaEngine;
        [SerializeField] private GameObject timeManager;
        ///////////////////////////////////////////////////Callback Section/////////////////////////////////////////////////////
        private void Awake()
        {
            Debug.Log("SpawnManager -->>> awake");
            networkMatch = GetComponent<NetworkMatch>();
            syncID = "lobby";
            networkMatch.matchId = GuidGenerator.syncIDToGuid(syncID);
            networkMatchID = networkMatch.matchId.ToString();
            
        }
        void Start()
        {
            Debug.Log("SpawnManager.cs -->>> start");
            instance = this;
        }
        ///////////////////////////////////////////////////SpawnTableTools Section/////////////////////////////////////////////////////
        /*
         * Bir oyun masasının sahip olması gerek toolları
         * 1- Spawn eder
         * 2- Mapler <GameObject, string> şeklinde burada key herzaman matchID'dir
         * 3- Client tarafında çalıştırılmış ise bu toolları TableScene'e yollar
         */
        public void SpawnTableTools(TableUIDataStruct newDataStruct)
        {
            GameObject newTableGameManager = SpawnTableGameManager(newDataStruct);
            GameObject newTablePlayerManager = SpawnTablePlayerManager(newDataStruct.matchId);
            GameObject newGamePlayerManager = SpawnGamePlayerManager(newDataStruct.matchId);
            GameObject newGamePlayerSpawnManager = SpawnGamePlayerSpawnManager(newDataStruct.matchId);
            GameObject newGameUISpawnManager = SpawnGameUISpawnManager(newDataStruct.matchId);
            GameObject newDealer = SpawnDealer(newDataStruct.matchId);
            GameObject newTurnManager = SpawnTurnManager(newDataStruct.matchId);
            GameObject newChipManager = SpawnChipManager(newDataStruct.matchId);
            GameObject newOmahaEngine = SpawnOmahaEngine(newDataStruct.matchId);
            

            if (isServer)
            {
                AddToTableGameManagerMap(newDataStruct.matchId, newTableGameManager);
                AddToTablePlayerManagerMap(newDataStruct.matchId, newTablePlayerManager);
                AddToGamePlayerManagerMap(newDataStruct.matchId, newGamePlayerManager);
                AddToGamePlayerSpawnManagerMap(newDataStruct.matchId, newGamePlayerSpawnManager);
                AddToGameUISpawnManagerMap(newDataStruct.matchId, newGameUISpawnManager);
                AddToDealerMap(newDataStruct.matchId, newDealer);
                AddToChipManagerMap(newDataStruct.matchId, newChipManager);
                AddToTurnManagerMap(newDataStruct.matchId, newTurnManager);
                AddToOmahaEngineMap(newDataStruct.matchId, newOmahaEngine);
                
                newTableGameManager.GetComponent<TableGameManager>().TableToolsSet(
                      newTablePlayerManager
                    , newGamePlayerManager
                    , newGamePlayerSpawnManager
                    , newGameUISpawnManager
                      , newOmahaEngine);
                newOmahaEngine.GetComponent<OmahaEngine>().SetOmahaEngineTools(
                      newDealer
                    , newChipManager
                     , newTurnManager
                      );
                
            }
            if (isClient)
            {
                Debug.Log("SpawnManager.cs -->>> SpawnTableTools -->>> isClient :" + isClient);
                SceneManager.MoveGameObjectToScene(newTablePlayerManager,SceneManager.GetSceneByBuildIndex(3));
                SceneManager.MoveGameObjectToScene(newGamePlayerManager,SceneManager.GetSceneByBuildIndex(3));
                SceneManager.MoveGameObjectToScene(newTableGameManager,SceneManager.GetSceneByBuildIndex(3));
                SceneManager.MoveGameObjectToScene(newGamePlayerSpawnManager,SceneManager.GetSceneByBuildIndex(3));
                SceneManager.MoveGameObjectToScene(newGameUISpawnManager,SceneManager.GetSceneByBuildIndex(3));
                SceneManager.MoveGameObjectToScene(newOmahaEngine,SceneManager.GetSceneByBuildIndex(3));
                SceneManager.MoveGameObjectToScene(newDealer,SceneManager.GetSceneByBuildIndex(3));
                SceneManager.MoveGameObjectToScene(newChipManager,SceneManager.GetSceneByBuildIndex(3));
                SceneManager.MoveGameObjectToScene(newTurnManager,SceneManager.GetSceneByBuildIndex(3));
                
            }
            
        }

        

        public void RemoveTableToolsFromTheirMaps(string matchId)
        {
            RemoveFromGamePlayerManagerMap(matchId);
            RemoveFromGamePlayerSpawnManagerMap(matchId);
            RemoveFromGameUISpawnManagerMap(matchId);
            RemoveFromTableGameManagerMap(matchId);
            RemoveFromTablePlayerManagerMap(matchId);
            RemoveFromDealerMap(matchId);
            RemoveFromOmahaEngineMap(matchId);
            RemoveFromTurnManagerMap(matchId);
            RemoveFromChipnManagerMap(matchId);
            
        }

        private GameObject SpawnTableGameManager(TableUIDataStruct tableUIDataStruct)
        {
            GameObject dummyTableGameManager = Instantiate(tableGameManager);
            dummyTableGameManager.GetComponent<TableGameManager>().SyncID = tableUIDataStruct.matchId;
            dummyTableGameManager.GetComponent<TableGameManager>().SeatCount = tableUIDataStruct.seatCount;
            dummyTableGameManager.GetComponent<TableGameManager>().SmallBlind = tableUIDataStruct.smallBlind;
            dummyTableGameManager.GetComponent<TableGameManager>().MinDeposit = tableUIDataStruct.minDeposit;
            dummyTableGameManager.GetComponent<TableGameManager>().TableRakePercent = tableUIDataStruct.tableRakePercent;
            dummyTableGameManager.name = "TableGameManager : " + tableUIDataStruct.matchId;
            dummyTableGameManager.GetComponent<TableGameManager>().SeatSummonByCount(tableUIDataStruct.seatCount);
            return dummyTableGameManager;
            
        }
        private GameObject SpawnGameUISpawnManager(string matchId)
        {
            GameObject dummyGameUISpawnManager = Instantiate(gameUISpawnManager);
            dummyGameUISpawnManager.GetComponent<GameUISpawnManager>().SyncID = matchId;
            dummyGameUISpawnManager.name = "GameUISpawnManager : " + matchId;
            return dummyGameUISpawnManager;
        }
        private GameObject SpawnTablePlayerManager(string matchId)
        {
            GameObject dummyTablePlayerManager = Instantiate(tablePlayerManager);
            dummyTablePlayerManager.GetComponent<TablePlayerManager>().SyncID = matchId;
            dummyTablePlayerManager.name = "TablePlayerManager : " + matchId;
            return dummyTablePlayerManager;
        }
        private GameObject SpawnGamePlayerManager(string matchId)
        {
            GameObject dummyGamePlayerManager = Instantiate(gamePlayerManager);
            dummyGamePlayerManager.GetComponent<GamePlayerManager>().SyncID = matchId;
            dummyGamePlayerManager.name = "GamePlayerManager : " + matchId;
            return dummyGamePlayerManager;
        }
        private GameObject SpawnGamePlayerSpawnManager(string matchId)
        {
            GameObject dummyGamePlayerSpawnManager = Instantiate(gamePlayerSpawnManager);
            dummyGamePlayerSpawnManager.GetComponent<GamePlayerSpawnManager>().SyncID = matchId;
            dummyGamePlayerSpawnManager.name = "GamePlayerSpawnManager : " + matchId;
            return dummyGamePlayerSpawnManager;
        }
        private GameObject SpawnDealer(string matchId)
        {
            GameObject dummyDealer = Instantiate(dealer);
            dummyDealer.GetComponent<Dealer>().SyncID = matchId;
            dummyDealer.name = "Dealer : " + matchId;
            return dummyDealer;
        }
        private GameObject SpawnChipManager(string matchId)
        {
            GameObject dummyChipManager = Instantiate(chipManager);
            dummyChipManager.GetComponent<ChipManager>().SyncID = matchId;
            dummyChipManager.name = "ChipManager : " + matchId;
            return dummyChipManager;
        }
        private GameObject SpawnTurnManager(string matchId)
        {
            GameObject dummyTurnManager = Instantiate(turnManager);
            dummyTurnManager.GetComponent<TurnManager>().SyncID = matchId;
            dummyTurnManager.name = "TurnManager : " + matchId;
            return dummyTurnManager;
        }
        private GameObject SpawnOmahaEngine(string matchId)
        {
            GameObject dummyOmahaEngine = Instantiate(omahaEngine);
            dummyOmahaEngine.GetComponent<OmahaEngine>().SyncID = matchId;
            dummyOmahaEngine.name = "OmahaEngine : " + matchId;
            return dummyOmahaEngine;
        }
        
        
        
        ///////////////////////////////////////////////////SpawnTableUI Section/////////////////////////////////////////////////////
        public void ActivateDeactivateAllTableUIClones(bool interactable)
        {
            foreach (var tableUIClone in tableUICloneMap.Values)
            {
                tableUIClone.GetComponent<TableUI>().OpenPasswordOrJoinTableButton.interactable = interactable;
            }
        }
        public GameObject SpawnTableUI(TableUIDataStruct tableUIDataStruct)
        {
            tableUIPrefab = Instantiate(tableUIPrefab);
            tableUIPrefab.GetComponent<TableUI>().MatchID = tableUIDataStruct.matchId;
            tableUIPrefab.GetComponent<TableUI>().TableDetailsCanvas.SetActive(true);
            tableUIPrefab.GetComponent<TableUI>().PasswordCanvas.SetActive(false);
            tableUIPrefab.GetComponent<TableUI>().ErrorCanvas.SetActive(false);
            tableUIPrefab.GetComponent<TableUI>().SeatCount = tableUIDataStruct.seatCount;
            tableUIPrefab.GetComponent<TableUI>().SmallBlind = tableUIDataStruct.smallBlind;
            tableUIPrefab.GetComponent<TableUI>().MinDeposit = tableUIDataStruct.minDeposit;
            tableUIPrefab.GetComponent<TableUI>().IsPublic = !tableUIDataStruct.hasPassword;
            tableUIPrefab.name = "TableUIClone : " + tableUIDataStruct.matchId;
            return tableUIPrefab;
        }
        public GameObject SpawnTableUI(string matchId)
        {
            Debug.LogWarning("SpawnManager.cs -->>> SpawnTableUI -->>> Bunun server tarafında olması anlamsız");
            TableUIDataStruct newTableUIDataStruct = tableUIDataStructMap[matchId];
            tableUIPrefab = Instantiate(tableUIPrefab);
            tableUIPrefab.GetComponent<TableUI>().TableDetailsCanvas.SetActive(true);
            tableUIPrefab.GetComponent<TableUI>().PasswordCanvas.SetActive(false);
            tableUIPrefab.GetComponent<TableUI>().ErrorCanvas.SetActive(false);
            tableUIPrefab.GetComponent<TableUI>().MatchID = newTableUIDataStruct.matchId;
            tableUIPrefab.GetComponent<TableUI>().SeatCount = newTableUIDataStruct.seatCount;
            tableUIPrefab.GetComponent<TableUI>().SmallBlind = newTableUIDataStruct.smallBlind;
            tableUIPrefab.GetComponent<TableUI>().MinDeposit = newTableUIDataStruct.minDeposit;
            tableUIPrefab.GetComponent<TableUI>().IsPublic = !newTableUIDataStruct.hasPassword;
            tableUIPrefab.name = "TableUIClone : " + newTableUIDataStruct.matchId;
            return tableUIPrefab;
            
        }
        public void SetParentToTableUIClone(Transform tableUIParent, GameObject tableUIClone)
        {
            tableUIClone.transform.SetParent(tableUIParent);
        }

        ///////////////////////////////////////////////////CheckAllTableTools Section/////////////////////////////////////////////////////
        
        //check all table tools in not null
        public bool CheckAllTableTools(string matchID)
        {
            GameObject dummyTableGameManager = GetFromTableGameManagerMap(matchID);
            GameObject dummyTablePlayerManager = GetFromTablePlayerManagerMap(matchID);
            GameObject dummyGamePlayerSpawnManager = GetFromGamePlayerSpawnManagerMap(matchID);
            GameObject dummyGamePlayerManager = GetFromGamePlayerManagerMap(matchID);
            GameObject dummyGameUISpawnManager = GetFromGameUISpawnManagerMap(matchID);
            GameObject dummyDealer = GetFromDealerMap(matchID);
            GameObject dummyChipManager = GetFromChipManagerMap(matchID);
            GameObject dummyTurnManager = GetFromTurnManagerMap(matchID);
            GameObject dummyOmahaEngine = GetFromOmahaEngineMap(matchID);
            
            if (dummyTableGameManager == null || dummyTablePlayerManager == null || dummyGamePlayerSpawnManager == null || dummyGamePlayerManager == null || dummyGameUISpawnManager == null || dummyDealer == null || dummyChipManager == null || dummyTurnManager == null || dummyOmahaEngine == null)
            {
                Debug.LogWarning("SpawnManager.cs -->>> CheckAllTableTools -->>> Table tools null");
                return false;
            }
            else
            {
                return true;
            }
            
        }
        ///////////////////////////////////////////////////tableGameManagerMap Section/////////////////////////////////////////////////////
        
        public TableGameManager GetTableGameManager(string matchId)
        {
            if (tableGameManagerMap.ContainsKey(matchId))
            {
                return tableGameManagerMap[matchId].GetComponent<TableGameManager>();
            }
            else
            {
                Debug.LogWarning("SpawnManager.cs -->>> GetTableGameManager -->>>  key :  " +matchId + 
                                 "bulunamadı. Null geri dönderildi!!!");
                return null;
            }
        }
        
        public GameObject GetFromTableGameManagerMap(string matchId)
        {
            if (tableGameManagerMap.ContainsKey(matchId))
            {
                return tableGameManagerMap[matchId];
            }
            else
            {
                Debug.LogWarning("SpawnManager.cs -->>> GetFromTableGameManagerMap -->>>  key :  " +matchId + 
                                 "bulunamadı. Null geri dönderildi!!!");
                return null;
            }
        }
        private bool AddToTableGameManagerMap(string matchId, GameObject newTableGameManager)
        {
            if (tableGameManagerMap.ContainsKey(matchId))
            {
                Debug.LogWarning("SpawnManager.cs -->>> AddToTableGameManagerMap -->>>  key :  " +matchId + 
                                 "Bu eleman zaten var. False geri dönderildi!!!");
                return false;
            }
            else
            {
                tableGameManagerMap.Add(matchId,newTableGameManager);
                return true;
            }
        }
        
        private bool RemoveFromTableGameManagerMap(string matchId)
        {
            if (tableGameManagerMap.ContainsKey(matchId))
            {
                tableGameManagerMap.Remove(matchId);
                return true;
            }
            else
            {
                Debug.LogWarning("SpawnManager.cs -->>> RemoveFromTableGameManagerMap -->>>  key :  " +matchId + 
                                 "bulunamadı. Null geri dönderildi!!!");
                return false;
            }
        }
        
        ///////////////////////////////////////////////////GameUISpawnManagerMap Section/////////////////////////////////////////////////////
        public GameObject GetFromGameUISpawnManagerMap(string matchId)
        {
            if (gameUISpawnManagerMap.ContainsKey(matchId))
            {
                return gameUISpawnManagerMap[matchId];
            }
            else
            {
                Debug.LogWarning("SpawnManager.cs -->>> GetFromGameUISpawnManagerMap -->>>  key :  " +matchId + 
                                 "bulunamadı. Null geri dönderildi!!!");
                return null;
            }
        }

        private bool AddToGameUISpawnManagerMap(string matchId, GameObject newGameUISpawnManager)
        {
            if (gameUISpawnManagerMap.ContainsKey(matchId))
            {
                Debug.LogWarning("SpawnManager.cs -->>> AddToGameUISpawnManagerMap -->>>  key :  " +matchId + 
                                 "Bu eleman zaten var. False geri dönderildi!!!");
                return false;
            }
            else
            {
                gameUISpawnManagerMap.Add(matchId,newGameUISpawnManager);
                return true;
            }
        }
        private bool RemoveFromGameUISpawnManagerMap(string matchId)
        {
            if (gameUISpawnManagerMap.ContainsKey(matchId))
            {
                gameUISpawnManagerMap.Remove(matchId);
                return true;
            }
            else
            {
                Debug.LogWarning("SpawnManager.cs -->>> RemoveFromGameUISpawnManagerMap -->>>  key :  " +matchId + 
                                 "bulunamadı. Null geri dönderildi!!!");
                return false;
            }
        }
        ///////////////////////////////////////////////////GamePlayerSpawnManagerMap Section/////////////////////////////////////////////////////
        
        public GameObject GetFromGamePlayerSpawnManagerMap(string matchId)
        {
            if (gamePlayerSpawnManagerMap.ContainsKey(matchId))
            {
                return gamePlayerSpawnManagerMap[matchId];
            }
            else
            {
                Debug.LogWarning("SpawnManager.cs -->>> GetFromGamePlayerSpawnManagerMap -->>>  key :  " +matchId + 
                                 "bulunamadı. Null geri dönderildi!!!");
                return null;
            }
        }

        private bool AddToGamePlayerSpawnManagerMap(string matchId, GameObject newGamePlayerSpawnManager)
        {
            if (gamePlayerSpawnManagerMap.ContainsKey(matchId))
            {
                Debug.LogWarning("SpawnManager.cs -->>> AddToGamePlayerSpawnManagerMap -->>>  key :  " +matchId + 
                                 "Bu eleman zaten var. False geri dönderildi!!!");
                return false;
            }
            else
            {
                gamePlayerSpawnManagerMap.Add(matchId,newGamePlayerSpawnManager);
                return true;
            }
        }
        private bool RemoveFromGamePlayerSpawnManagerMap(string matchId)
        {
            if (gamePlayerSpawnManagerMap.ContainsKey(matchId))
            {
                gamePlayerSpawnManagerMap.Remove(matchId);
                return true;
            }
            else
            {
                Debug.LogWarning("SpawnManager.cs -->>> RemoveFromGamePlayerSpawnManagerMap -->>>  key :  " +matchId + 
                                 "bulunamadı. Null geri dönderildi!!!");
                return false;
            }
        }
        ///////////////////////////////////////////////////GamePlayerManagerMap Section/////////////////////////////////////////////////////
        public GamePlayerManager GetGamePlayerManager(string matchId)
        {
            if (gamePlayerManagerMap.ContainsKey(matchId))
            {
                return gamePlayerManagerMap[matchId].GetComponent<GamePlayerManager>();
            }
            else
            {
                Debug.LogWarning("SpawnManager.cs -->>> GetGamePlayerManager -->>>  key :  " +matchId + 
                                 "bulunamadı. Null geri dönderildi!!!");
                return null;
            }
        }
        public GameObject GetFromGamePlayerManagerMap(string matchId)
        {
            if (gamePlayerManagerMap.ContainsKey(matchId))
            {
                return gamePlayerManagerMap[matchId];
            }
            else
            {
                Debug.LogWarning("SpawnManager.cs -->>> GetFromGamePlayerManagerMap -->>>  key :  " +matchId + 
                                 "bulunamadı. Null geri dönderildi!!!");
                return null;
            }
        }

        private bool AddToGamePlayerManagerMap(string matchId, GameObject newGamePlayerManager)
        {
            if (gamePlayerManagerMap.ContainsKey(matchId))
            {
                Debug.LogWarning("SpawnManager.cs -->>> AddToGamePlayerManagerMap -->>>  key :  " +matchId + 
                                 "Bu eleman zaten var. False geri dönderildi!!!");
                return false;
            }
            else
            {
                gamePlayerManagerMap.Add(matchId,newGamePlayerManager);
                return true;
            }
        }
        private bool RemoveFromGamePlayerManagerMap(string matchId)
        {
            if (gamePlayerManagerMap.ContainsKey(matchId))
            {
                gamePlayerManagerMap.Remove(matchId);
                return true;
            }
            else
            {
                Debug.LogWarning("SpawnManager.cs -->>> RemoveFromGamePlayerManagerMap -->>>  key :  " +matchId + 
                                 "bulunamadı. Null geri dönderildi!!!");
                return false;
            }
        }
        ///////////////////////////////////////////////////TablePlayerManagerMap Section/////////////////////////////////////////////////////
        public TablePlayerManager GetTablePlayerManager(string matchId)
        {
            if (tablePlayerManagerMap.ContainsKey(matchId))
            {
                return tablePlayerManagerMap[matchId].GetComponent<TablePlayerManager>();
            }
            else
            {
                Debug.LogWarning("SpawnManager.cs -->>> GetTablePlayerManager -->>>  key :  " +matchId + 
                                 "bulunamadı. Null geri dönderildi!!!");
                return null;
            }
        }
            
        
        public GameObject GetFromTablePlayerManagerMap(string matchId)
        {
            if (tablePlayerManagerMap.ContainsKey(matchId))
            {
                return tablePlayerManagerMap[matchId];
            }
            else
            {
                Debug.LogWarning("SpawnManager.cs -->>> GetFromTablePlayerManagerMap -->>>  key :  " +matchId + 
                                 "bulunamadı. Null geri dönderildi!!!");
                return null;
            }
        }

        private bool AddToTablePlayerManagerMap(string matchId, GameObject newTablePlayerManager)
        {
            if (tablePlayerManagerMap.ContainsKey(matchId))
            {
                Debug.LogWarning("SpawnManager.cs -->>> AddToTablePlayerManagerMap -->>>  key :  " +matchId + 
                                 "Bu eleman zaten var. False geri dönderildi!!!");
                return false;
            }
            else
            {
                tablePlayerManagerMap.Add(matchId,newTablePlayerManager);
                return true;
            }
        }
        private bool RemoveFromTablePlayerManagerMap(string matchId)
        {
            if (tablePlayerManagerMap.ContainsKey(matchId))
            {
                tablePlayerManagerMap.Remove(matchId);
                return true;
            }
            else
            {
                Debug.LogWarning("SpawnManager.cs -->>> RemoveFromTablePlayerManagerMap -->>>  key :  " +matchId + 
                                 "bulunamadı. Null geri dönderildi!!!");
                return false;
            }
        }
        ///////////////////////////////////////////////////OmahaEngineMap Section/////////////////////////////////////////////////////
        
        public GameObject GetFromOmahaEngineMap(string matchId)
        {
            if (omahaEngineMap.ContainsKey(matchId))
            {
                return omahaEngineMap[matchId];
            }
            else
            {
                Debug.LogWarning("SpawnManager.cs -->>> GetFromOmahaEngineMap -->>>  key :  " +matchId + 
                                 "bulunamadı. Null geri dönderildi!!!");
                return null;
            }
        }
        

        private bool AddToOmahaEngineMap(string matchId, GameObject newOmahaEngine)
        {
            if (omahaEngineMap.ContainsKey(matchId))
            {
                Debug.LogWarning("SpawnManager.cs -->>> AddToOmahaEngineMap -->>>  key :  " +matchId + 
                                 "Bu eleman zaten var. False geri dönderildi!!!");
                return false;
            }
            else
            {
                omahaEngineMap.Add(matchId,newOmahaEngine);
                return true;
            }
        }
        private bool RemoveFromOmahaEngineMap(string matchId)
        {
            if (omahaEngineMap.ContainsKey(matchId))
            {
                omahaEngineMap.Remove(matchId);
                return true;
            }
            else
            {
                Debug.LogWarning("SpawnManager.cs -->>> RemoveFromOmahaEngineMap -->>>  key :  " +matchId + 
                                 "bulunamadı. Null geri dönderildi!!!");
                return false;
            }
        }
        ///////////////////////////////////////////////////TurnManagerMap Section/////////////////////////////////////////////////////
        public TurnManager GetTurnManager(string matchId)
        {
            if (turnManagerMap.ContainsKey(matchId))
            {
                return turnManagerMap[matchId].GetComponent<TurnManager>();
            }
            else
            {
                Debug.LogWarning("SpawnManager.cs -->>> GetTurnManager -->>>  key :  " +matchId + 
                                 "bulunamadı. Null geri dönderildi!!!");
                return null;
            }
        }
        public GameObject GetFromTurnManagerMap(string matchId)
        {
            if (turnManagerMap.ContainsKey(matchId))
            {
                return turnManagerMap[matchId];
            }
            else
            {
                Debug.LogWarning("SpawnManager.cs -->>> GetFromTurnManagerMap -->>>  key :  " +matchId + 
                                 "bulunamadı. Null geri dönderildi!!!");
                return null;
            }
        }
        private bool AddToTurnManagerMap(string matchId, GameObject newTurnManager)
        {
            if (turnManagerMap.ContainsKey(matchId))
            {
                Debug.LogWarning("SpawnManager.cs -->>> AddToTurnManagerMap -->>>  key :  " +matchId + 
                                 "Bu eleman zaten var. False geri dönderildi!!!");
                return false;
            }
            else
            {
                turnManagerMap.Add(matchId,newTurnManager);
                return true;
            }
        }
        private bool RemoveFromTurnManagerMap(string matchId)
        {
            if (turnManagerMap.ContainsKey(matchId))
            {
                turnManagerMap.Remove(matchId);
                return true;
            }
            else
            {
                Debug.LogWarning("SpawnManager.cs -->>> RemoveFromTurnManagerMap -->>>  key :  " +matchId + 
                                 "bulunamadı. Null geri dönderildi!!!");
                return false;
            }
        }
        ///////////////////////////////////////////////////ChipManagerMap Section/////////////////////////////////////////////////////
        public GameObject GetFromChipManagerMap(string matchId)
        {
            if (chipManagerMap.ContainsKey(matchId))
            {
                return chipManagerMap[matchId];
            }
            else
            {
                Debug.LogWarning("SpawnManager.cs -->>> GetFromChipManagerMap -->>>  key :  " +matchId + 
                                 "bulunamadı. Null geri dönderildi!!!");
                return null;
            }
        }

        private bool AddToChipManagerMap(string matchId, GameObject newChipManager)
        {
            if (chipManagerMap.ContainsKey(matchId))
            {
                Debug.LogWarning("SpawnManager.cs -->>> AddToChipManagerMap -->>>  key :  " +matchId + 
                                 "Bu eleman zaten var. False geri dönderildi!!!");
                return false;
            }
            else
            {
                chipManagerMap.Add(matchId,newChipManager);
                return true;
            }
        }
        
        private bool RemoveFromChipnManagerMap(string matchId)
        {
            if (chipManagerMap.ContainsKey(matchId))
            {
                chipManagerMap.Remove(matchId);
                return true;
            }
            else
            {
                Debug.LogWarning("SpawnManager.cs -->>> RemoveFromChipManagerMap -->>>  key :  " +matchId + 
                                 "bulunamadı. Null geri dönderildi!!!");
                return false;
            }
        }
        ///////////////////////////////////////////////////DealerMap Section/////////////////////////////////////////////////////
        public Dealer GetDealer(string matchId)
        {
            if (dealerMap.ContainsKey(matchId))
            {
                return dealerMap[matchId].GetComponent<Dealer>();
            }
            else
            {
                Debug.LogWarning("SpawnManager.cs -->>> GetDealer -->>>  key :  " +matchId + 
                                 "bulunamadı. Null geri dönderildi!!!");
                return null;
            }
        }
        public GameObject GetFromDealerMap(string matchId)
        {
            if (dealerMap.ContainsKey(matchId))
            {
                return dealerMap[matchId];
            }
            else
            {
                Debug.LogWarning("SpawnManager.cs -->>> GetFromDealerMap -->>>  key :  " +matchId + 
                                 "bulunamadı. Null geri dönderildi!!!");
                return null;
            }
        }

        private bool AddToDealerMap(string matchId, GameObject newDealer)
        {
            if (dealerMap.ContainsKey(matchId))
            {
                Debug.LogWarning("SpawnManager.cs -->>> AddToDealerMap -->>>  key :  " +matchId + 
                                 "Bu eleman zaten var. False geri dönderildi!!!");
                return false;
            }
            else
            {
                dealerMap.Add(matchId,newDealer);
                return true;
            }
        }
        private bool RemoveFromDealerMap(string matchId)
        {
            if (dealerMap.ContainsKey(matchId))
            {
                dealerMap.Remove(matchId);
                return true;
            }
            else
            {
                Debug.LogWarning("SpawnManager.cs -->>> RemoveFromDealerMap -->>>  key :  " +matchId + 
                                 "bulunamadı. Null geri dönderildi!!!");
                return false;
            }
        }
        ///////////////////////////////////////////////////CloneMatchIdSyncList Section/////////////////////////////////////////////////////
        public void UpdateCloneMatchIDSyncList(string newTableMatchId)
        {
            if (tableCloneMatchIDSyncList.Contains(newTableMatchId))
            {
                Debug.LogWarning("SpawnManager.cs -->>> UpdateCloneMatchIDSyncList -->>> Bu eleman zaten var");
            }
            else
            {
                tableCloneMatchIDSyncList.Add(newTableMatchId);
            }
        }
        
        ///////////////////////////////////////////////////TableUIDataStructMap Section/////////////////////////////////////////////////////
        public void UpdateTableUIDataStructMap(TableUIDataStruct newDataStruct)
        {
            if (tableUIDataStructMap.Keys.Contains(newDataStruct.matchId))
            {
                Debug.LogWarning("SpawnManager.cs -->>> UpdateTableUIDataStructMap -->>> Bu eleman zaten var");
            }
            else
            {
                tableUIDataStructMap.Add(newDataStruct.matchId,newDataStruct);
            }
        }
        public TableUIDataStruct GetFromTableUIDataStructMap(string matchId)
        {
            if (tableUIDataStructMap.ContainsKey(matchId))
            {
                return tableUIDataStructMap[matchId];
            }
            else
            {
                Debug.LogWarning("SpawnManager.cs -->>> GetFromTableUIDataStructMap -->>>  key :  " +matchId + 
                                 "bulunamadı. Default TableUI geri dönderildi!!!");
                return dummyStruct;
            }
        }
        public TableUIDataStruct setDataStruct(string matchId, int seatCount, float smallBlind, float minDeposit,
            bool hasPassword, float tableRakePercent)
        {
            TableUIDataStruct newDataStruct = default;
            newDataStruct.matchId = matchId;
            newDataStruct.seatCount = seatCount;
            newDataStruct.smallBlind = smallBlind;
            newDataStruct.minDeposit = minDeposit;
            newDataStruct.hasPassword = hasPassword;
            newDataStruct.tableRakePercent = tableRakePercent;
            return newDataStruct;
        }

        ///////////////////////////////////////////////////tableUICloneMap Section/////////////////////////////////////////////////////
        public void UpdateTableUICloneMap(GameObject newTableUIClone)
        {
            if (tableUICloneMap.Keys.Contains(newTableUIClone.GetComponent<TableUI>().MatchID))
            {
                Debug.LogWarning("SpawnManager.cs -->>> UpdateTableUICloneMap -->>> Bu eleman zaten var");
            }
            else
            {
                tableUICloneMap.Add(newTableUIClone.GetComponent<TableUI>().MatchID,newTableUIClone);
            }
        }
        public TableUI GetTableUI(string matchId)
        {
            if (tableUICloneMap.ContainsKey(matchId))
            {
                return tableUICloneMap[matchId].GetComponent<TableUI>();
            }
            else
            {
                Debug.LogWarning("SpawnManager.cs -->>> GetTableUI -->>>  key :  " +matchId + 
                                 "bulunamadı. Null geri dönderildi!!!");
                return null;
            }
        }
        public GameObject GetFromTableUICloneMap(string matchId)
        {
            if (tableUICloneMap.ContainsKey(matchId))
            {
                return tableUICloneMap[matchId];
            }
            else
            {
                Debug.LogWarning("SpawnManager.cs -->>> GetFromTableUICloneMap -->>>  key :  " +matchId + 
                                 "bulunamadı. Null geri dönderildi!!!");
                return null;
            }
        }
        public bool AddCloneToTableUICloneMap(string matchID,GameObject clone )
        {
            if (tableUICloneMap.ContainsKey(matchID))
            {
                Debug.LogWarning("SpawnManager.cs -->>> Key : " + matchID + "-->>>AddCloneToTableUICloneMap" +
                                 "Clone is already exist in CloneMap!!! :  ");
                return false;
            }
            else
            {
                tableUICloneMap.Add(matchID,clone);
                return true;
            }
        }
        public bool RemoveCloneFromTableUICloneMap(string matchID )
        {
            if (tableUICloneMap.ContainsKey(matchID))
            {
                tableUICloneMap.Remove(matchID);
                return true;
            }
            else
            {
                Debug.LogWarning("SpawnManager.cs -->>> Key : " + matchID + "-->>>RemoveCloneFromTableUICloneMap" +
                                 "Clone does NOT exist in CloneMap!!! :  ");
                return false;
            }
        }
        public GameObject GetCloneFromTableUICloneMap(string matchID )
        {
            if (tableUICloneMap.ContainsKey(matchID))
            {
               
                return  tableUICloneMap[matchID];
            }
            else
            {
                Debug.LogWarning("SpawnManager.cs -->>> Key : " + matchID + "-->>>GetCloneFromTableUICloneMap" +
                                 "Clone does NOT exist in CloneMap!!! :  ");
                return null;
            }
        }
        public bool isCloneAlive(string matchID)
        {
            return tableCloneMatchIDSyncList.Contains(matchID);
        }
        
        public void SyncTables()
        {
            foreach (var matchId in tableCloneMatchIDSyncList)
            {
                if (tableUICloneMap.ContainsKey(matchId))
                {
                    //bu masa zaten var do nothing
                }
                else
                {
                    
                    Player.localPlayer.GetTableStruct(matchId);
                }
            }

            foreach (var matchId in tableUICloneMap.Keys)
            {
                if (tableCloneMatchIDSyncList.Contains(matchId))
                {
                    //bu clone yaşıyor do nothing
                }
                else
                {
                    removeTableUIClone(matchId);
                }
            }
        }

        private void removeTableUIClone(string matchId)
        {
            Debug.LogWarning("SpawnManager.cs -->>> RemoveTableUI -->>> Boş");
        }

        public void GetTableStructSucces(TableUIDataStruct tableUIDataStruct)
        {
            GameObject newTableUI = SpawnTableUI(tableUIDataStruct);
            SetParentToTableUIClone(SaloonAreaUI.instance.TableUIParent,newTableUI);
            UpdateTableUICloneMap(newTableUI);
        }
        ///////////////////////////////////////////////////Properties Section/////////////////////////////////////////////////////


        
    }
    
    /*
     * TableUI'da tutulan bilgileri kullanabilmek ve  aktarabilmek için oluşturulmuş data yapısıdır.
     */
    public struct TableUIDataStruct
    {
        public string matchId;
        public int seatCount;
        public float smallBlind;
        public float minDeposit;
        public bool hasPassword;
        public float tableRakePercent;
    }

    
}