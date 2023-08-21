using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    /*
     * TableScene'in doğru player sayısına göre şekil alması için gerekli olan SPAWN işlemlerinden sorumludur.
     * Spawn Ettikleri
     *                  -->>> 6 veya 8 kişilik TableScene(Masaya giriş yapmamış user'in gördüğü ekran)
     *                  -->>> 6 veya 8 kişilik GameScene(Masaya giriş yapmış user'in gördüğü ekran)
     *                  -->>> SitButton
     * Spawn Fonksiyonları, kaç kişilik olduğuna  ve Scene tipine göre(TableScene,GameScene) 4 ayrı şekilde çağırılır.
     * 1- Scene spawn edilir
     * 2- Her seatLocationuna bir SitButton spawn edilir.
     * 3- Ve her SitButton'a hangi konuma spawn edildiği bildirilir.
     * Böylece tıklanması durumunda tetiklenecek fonksiyona bu bilgiyi aktarabilir
     */
    public class GameUISpawnManager : MonoBehaviour
    {
        public static GameUISpawnManager instance;
        [SerializeField] private string syncID;
        [Header("TableScene")]
        [SerializeField] private GameObject sixPlayerScene;
        [SerializeField] private GameObject eightPlayerScene;
        [Header("GameScene")]
        [SerializeField] private GameObject sixPlayerGameScene = null;
        [SerializeField] private GameObject eightPlayerGameScene = null;
        ///////////////////////////////////////////////////Events Section/////////////////////////////////////////////////////
        void Start()
        {
            instance = this;
        }
        ///////////////////////////////////////////////////SpawnNextMoveBet Section///////////////////////////////////////////////////
        public void SpawnCallBetNoStageChangeForSeatedPlayers(List<Seat> seatList, string lastMovePlayerName, GameObject activeScene)
        {
            if (seatList.Count == 6)
            {
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        if (string.Equals(seat.username, lastMovePlayerName))
                        {
                            activeScene.GetComponent<SixPlayerGameScene>().SpawnBet(seat.location,seat.totalBetInSubTurn);
                        }
                    }
                }
            }
        }

        public void SpawnCallBetNoStageChangeForObservers(List<Seat> seatList, string lastMovePlayerName, GameObject activeScene)
        {
            if (seatList.Count == 6)
            {
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        if (seat.username == lastMovePlayerName)
                        {
                            activeScene.GetComponent<SixPlayerTableScene>().SpawnBet(seat.location, seat.totalBetInSubTurn);
                        }
                    }
                }
            }
        }
        
        ///////////////////////////////////////////////////ActivateGameController Section/////////////////////////////////////////////////////
        public void ActivateGameController(List<Seat> seatList, string username, GameObject activeScene)
        {
            if (seatList.Count == 6)
            {
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        if (seat.isPlayerInGame)
                        {
                            if (seat.lastMove != Move.AllIn)
                            {
                                if (string.Equals(seat.username,username))
                                {
                                
                                    activeScene.GetComponent<SixPlayerGameScene>().ActivateGameController(seat.isMyTurn);
                                
                                }
                            }
                        }
                    }
                }
            }
        }
        
        public void SetGameControllerValuesForSmallAndBigBlind(List<Seat> seatList, string username,
            GameObject activeScene, float smallBlind)
        {
            if (seatList.Count == 6)
            {
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        if (string.Equals(seat.username,username))
                        {
                            activeScene.GetComponent<SixPlayerGameScene>().SetGameControllerValuesForSmallAndBigBlind(seat.username,seatList,seat.isMyTurn,smallBlind);
                        }
                    }
                }
            }
        }
        public void SetGameControllerValuesForMoveCall(List<Seat> seatList, string myName, string lastMovePlayerName,
            GameObject activeScene, bool isStageChange, float middlePot, float smallBlind, int sideBetCount,
            List<float> sideBetList)
        {
            if (seatList.Count == 6)
            {
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        if (string.Equals(seat.username,myName))
                        {
                            activeScene.GetComponent<SixPlayerGameScene>().SetGameControllerValuesForMoveCall(seatList,myName,lastMovePlayerName,seat.isMyTurn,isStageChange,middlePot,smallBlind,sideBetCount,sideBetList);
                        }
                    }
                }
            }
        }
        
        ///////////////////////////////////////////////////SpawnBigBlindBet Section//////////////////////////////////////////////////
        public void SpawnBigBlindBetForSeatedPlayers(List<Seat> seatList, GameObject activeScene)
        {
            if (seatList.Count == 6)
            {
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        if (seat.lastMove == Move.BigBlind)
                        {
                            activeScene.GetComponent<SixPlayerGameScene>().SpawnBet(seat.location, seat.lastMoveAmount);
                        }
                    }
                }
            }
        }
        
        public void SpawnBigBlindBetForObservers(List<Seat> seatList, GameObject activeScene)
        {
            if (seatList.Count == 6)
            {
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        if (seat.lastMove == Move.BigBlind)
                        {
                            activeScene.GetComponent<SixPlayerTableScene>().SpawnBet(seat.location, seat.lastMoveAmount);
                        }
                    }
                }
            }
        }
        ///////////////////////////////////////////////////SpawnSmallBlindBet Section//////////////////////////////////////////////////
        public void SpawnSmallBlindBetForSeatedPlayers(List<Seat> seatList, GameObject activeScene)
        {
            if (seatList.Count == 6)
            {
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        if (seat.lastMove == Move.SmallBlind)
                        {
                            activeScene.GetComponent<SixPlayerGameScene>().SpawnBet(seat.location, seat.lastMoveAmount);
                        }
                    }
                }
            }
        }
        
        public void SpawnSmallBlindBetForObservers(List<Seat> seatList, GameObject activeScene)
        {
            if (seatList.Count == 6)
            {
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        if (seat.lastMove == Move.SmallBlind)
                        {
                            activeScene.GetComponent<SixPlayerTableScene>().SpawnBet(seat.location, seat.lastMoveAmount);
                        }
                    }
                }
            }
        }
        ///////////////////////////////////////////////////SpawnDealerToken Section/////////////////////////////////////////////////////
        public void SpawnDealerTokenForObservers(List<Seat> seatList, GameObject activeScene)
        {
            if (seatList.Count == 6)
            {
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        if (seat.isDealer)
                        {
                            activeScene.GetComponent<SixPlayerTableScene>().SpawnDealerToken((int) seat.location);
                        }
                    }
                }
            }
        }
        public void SpawnDealerTokenForSeatedPlayers(List<Seat> seatList, GameObject activeScene)
        {
            if (seatList.Count == 6)
            {
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        if (seat.isDealer)
                        {
                            activeScene.GetComponent<SixPlayerGameScene>().SpawnDealerToken((int) seat.location);
                        }
                    }
                }
            }
        }
        ///////////////////////////////////////////////////CardBacksSpawn Section/////////////////////////////////////////////////////
        public void SpawnCardBacksForOtherSeatedPlayers(List<Seat> seatList, string username, GameObject activeScene)
        {
            if (seatList.Count == 6)
            {
                foreach (var seat in seatList)
                {
                    if (!string.Equals(seat.username, username))
                    {
                        if (seat.isActive)
                        {
                            activeScene.GetComponent<SixPlayerGameScene>().CardBackMap[(int) seat.location].SetActive(true);
                        }
                    }
                }
            }
            else if (seatList.Count == 8)
            {
                foreach (var seat in seatList)
                {
                    if (!string.Equals(seat.username, username))
                    {
                        if (seat.isActive)
                        {
                            //EightPlayerGameScene.instance.CardBackMap[(int) seat.location].SetActive(true);
                        }
                    }
                }
            }
        }
        
        public void SpawnCardBacksForObservers(List<Seat> seatList, GameObject activeScene)
        {
            if (seatList.Count == 6)
            {
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        activeScene.GetComponent<SixPlayerTableScene>().CardBackMap[(int) seat.location].SetActive(true);
                    }
                }
            }
            else if (seatList.Count == 8)
            {
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        //EightPlayerGameScene.instance.CardBackMap[(int) seat.location].SetActive(true);
                    }
                }
            }
        }
        ///////////////////////////////////////////////////SpawnHand Section/////////////////////////////////////////////////////
        public void SpawnMyHand(List<CardData> myHand, int seatCount, GameObject gameScene)
        {
            if (seatCount == 6)
            {
                gameScene.GetComponent<SixPlayerGameScene>().HandMap[1].SetActive(true);
                gameScene.GetComponent<SixPlayerGameScene>().HandMap[1].GetComponent<Hand>().SetHandActivity(true);
                gameScene.GetComponent<SixPlayerGameScene>().HandMap[1].GetComponent<Hand>().SetHand(myHand);
            }
            else if (seatCount == 8)
            {
                //EightPlayerGameScene.instance.LocalPlayerHand.GetComponent<Hand>().SetHandActivity(true);
                //EightPlayerGameScene.instance.LocalPlayerHand.GetComponent<Hand>().SetHand(myHand);
            }
        }
        
        ///////////////////////////////////////////////////SpawnTableScene Section/////////////////////////////////////////////////////
        public GameObject SpawnEightPlayerScene()
        {
            GameObject dummyEightPlayerScene  = Instantiate(eightPlayerScene);
            SceneManager.MoveGameObjectToScene(dummyEightPlayerScene,SceneManager.GetSceneByBuildIndex(3));
            return dummyEightPlayerScene;
        }

        public GameObject SpawnSixPlayerScene()
        {
            GameObject dummySixPlayerScene  = Instantiate(sixPlayerScene);
            SceneManager.MoveGameObjectToScene(dummySixPlayerScene,SceneManager.GetSceneByBuildIndex(3));
            return dummySixPlayerScene;
        }
        ///////////////////////////////////////////////////SpawnGameScene Section/////////////////////////////////////////////////////
        public GameObject SpawnSixPlayerGameScene()
        {
            GameObject dummySixPlayerGameScene  = Instantiate(sixPlayerGameScene);
            SceneManager.MoveGameObjectToScene(dummySixPlayerGameScene,SceneManager.GetSceneByBuildIndex(3));
            return dummySixPlayerGameScene;
        }
        public GameObject SpawnEightPlayerGameScene()
        {
            GameObject dummyEightPlayerGameScene  = Instantiate(eightPlayerGameScene);
            SceneManager.MoveGameObjectToScene(dummyEightPlayerGameScene,SceneManager.GetSceneByBuildIndex(3));
            return dummyEightPlayerGameScene;
        }
        
        ///////////////////////////////////////////////////Properties Section/////////////////////////////////////////////////////
        
        public string SyncID
        {
            get => syncID;
            set => syncID = value;
        }


        public void SetGameControllerValuesForMoveCheck(List<Seat> seatList, string myName, string lastMovePlayerName,
            GameObject activeScene, bool isStageChange, float middlePot, float smallBlind, int sideBetCount,
            List<float> sideBetList)
        {
            if (seatList.Count == 6)
            {
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        if (string.Equals(seat.username,myName))
                        {
                            activeScene.GetComponent<SixPlayerGameScene>().SetGameControllerValuesForMoveCheck(seatList,myName,lastMovePlayerName,seat.isMyTurn,isStageChange,middlePot,sideBetCount,sideBetList,smallBlind);
                        }
                    }
                }
            }
        }

        public void SetMiddleCards(List<CardData> middleCards, GameObject activeScene, bool isGameScene)
        {
            if (isGameScene)
            {
                activeScene.GetComponent<SixPlayerGameScene>().SetMiddleCards(middleCards);
                
            }
            else
            {
                activeScene.GetComponent<SixPlayerTableScene>().SetMiddleCards(middleCards);
            }
        }

        public void SetMiddlePot(float middlePot, int sideBetCount, List<float> sideBetList, GameObject activeScene,
            bool isGameScene)
        {
            if (isGameScene)
            {
                activeScene.GetComponent<SixPlayerGameScene>().SetMiddlePot(middlePot,sideBetCount,sideBetList);
            }
            else
            {
                activeScene.GetComponent<SixPlayerTableScene>().SetMiddlePot(middlePot,sideBetCount,sideBetList);
            }
        }

        public void ClearPreviousSubTurnBets(List<Seat> seatList, GameObject activeScene, bool isGameScene)
        {
            if (isGameScene)
            {
                activeScene.GetComponent<SixPlayerGameScene>().ClearPreviousSubTurnBets(seatList);
            }
            else
            {
                activeScene.GetComponent<SixPlayerTableScene>().ClearPreviousSubTurnBets(seatList);
            }
        }

        public void SpawnRaiseBetNoStageChangeForSeatedPlayers(List<Seat> seatList, string lastMovePlayerName, GameObject activeScene)
        {
            if (seatList.Count == 6)
            {
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        if (string.Equals(seat.username, lastMovePlayerName))
                        {
                            activeScene.GetComponent<SixPlayerGameScene>().SpawnBet(seat.location,seat.totalBetInSubTurn);
                        }
                    }
                }
            }
        }

        public void SpawnRaiseBetNoStageChangeForObservers(List<Seat> seatList, string lastMovePlayerName, GameObject activeScene)
        {
            if (seatList.Count == 6)
            {
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        if (seat.username == lastMovePlayerName)
                        {
                            activeScene.GetComponent<SixPlayerTableScene>().SpawnBet(seat.location, seat.totalBetInSubTurn);
                        }
                    }
                }
            }
        }

        public void SetGameControllerValuesForMoveRaise(List<Seat> seatList, string myName, string lastMovePlayerName,
            GameObject activeScene, float middlePot, int sideBetCount, List<float> sideBetList, float smallBlind)
        {
            if (seatList.Count == 6)
            {
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        if (string.Equals(seat.username,myName))
                        {
                            activeScene.GetComponent<SixPlayerGameScene>().SetGameControllerValuesForMoveRaise(seatList,myName,lastMovePlayerName,seat.isMyTurn,middlePot,sideBetCount,sideBetList,smallBlind);
                        }
                    }
                }
            }
        }

        public void SpawnAllinBetNoStageChangeForSeatedPlayers(List<Seat> seatList, string lastMovePlayerName, GameObject activeScene)
        {
            if (seatList.Count == 6)
            {
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        if (string.Equals(seat.username, lastMovePlayerName))
                        {
                            activeScene.GetComponent<SixPlayerGameScene>().SpawnBet(seat.location,seat.totalBetInSubTurn);
                        }
                    }
                }
            }
        }

        public void SetGameControllerValuesForMoveAllin(List<Seat> seatList, string myName, string lastMovePlayerName,
            GameObject activeScene, bool isStageChange, float middlePot, int sideBetCount, List<float> sideBetList,
            float smallBlind)
        {
            if (seatList.Count == 6)
            {
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        if (string.Equals(seat.username,myName))
                        {
                            activeScene.GetComponent<SixPlayerGameScene>().SetGameControllerValuesForMoveAllin(seatList,myName,lastMovePlayerName,seat.isMyTurn,middlePot,sideBetCount,sideBetList,isStageChange,smallBlind);
                        }
                    }
                }
            }
        }

        public void DeactivateGameController(List<Seat> seatList, GameObject activeScene)
        {
            if (seatList.Count == 6)
            {
                activeScene.GetComponent<SixPlayerGameScene>().DeactivateGameController();
            }
        }

        public void OpenAllInGamePlayerHands(List<Seat> seatList, List<string> inGamePlayerList,
            List<List<CardData>> inGamePlayersCards, GameObject activeScene, bool isGameScene)
        {
            if (seatList.Count == 6)
            {
                if (isGameScene)
                {
                    activeScene.GetComponent<SixPlayerGameScene>().OpenAllInGamePlayerHands(seatList,inGamePlayerList,inGamePlayersCards);
                }
                else
                {
                    activeScene.GetComponent<SixPlayerTableScene>().OpenAllInGamePlayerHands(seatList,inGamePlayerList,inGamePlayersCards);
                }
            }
        }

        public void ShakeWinningCombinations(List<Seat> seatList, List<string> winnerList,
            List<List<CardData>> winnerCombinations, List<string> inGamePlayerList,
            List<List<CardData>> inGamePlayersCards, GameObject activeScene,
            bool isGameScene)
        {
            if (seatList.Count == 6)
            {
                if (isGameScene)
                {
                    activeScene.GetComponent<SixPlayerGameScene>().ShakeWinningCombinations(seatList,winnerList,winnerCombinations,inGamePlayerList,inGamePlayersCards);
                }
                else
                {
                    activeScene.GetComponent<SixPlayerTableScene>().ShakeWinningCombinations(seatList,winnerList,winnerCombinations,inGamePlayerList,inGamePlayersCards);
                }
            }
        }

        public void CloseHand(List<Seat> seatList, string lastMovePlayerName, string myName, GameObject activeScene,
            bool isGameScene)
        {
            if (seatList.Count == 6)
            {
                if (isGameScene)
                {
                    foreach (var seat in seatList)
                    {
                        if (seat.isActive)
                        {
                            if (string.Equals(seat.username,lastMovePlayerName))
                            {
                                if (myName == lastMovePlayerName)
                                {
                                    activeScene.GetComponent<SixPlayerGameScene>().CloseHand(seat.location);
                                }
                                else
                                {
                                    activeScene.GetComponent<SixPlayerGameScene>().CloseCardBack(seat.location);
                                }
                            }
                        }
                    }
                    
                }
                else
                {
                    foreach (var seat in seatList)
                    {
                        if (seat.isActive)
                        {
                            if (string.Equals(seat.username,lastMovePlayerName))
                            {
                                activeScene.GetComponent<SixPlayerTableScene>().CloseCardBack(seat.location);
                            }
                        }
                    }
                }
            }
            {
                if (seatList.Count == 6)
                {
                    
                }
            }
        }
        
        public void ActivatePreShowDown(List<Seat> seatList, string lastMovePlayerName, string myName, GameObject activeScene)
        {
            string winnerName = ISeatList.GetLastPlayer(seatList);
            if (seatList.Count == 6)
            {
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        if (string.Equals(seat.username,myName))
                        {
                            if (myName == winnerName)
                            {
                                activeScene.GetComponent<SixPlayerGameScene>().ActivatePreShowDown();
                            }
                        }
                    }
                }
            }
        }

        public void MoveChipsToWinner(List<Seat> seatList, List<string> winnerList, List<float> winAmountList, GameObject activeScene, bool isGameScene)
        {
            if (seatList.Count == 6)
            {
                if (isGameScene)
                {
                    activeScene.GetComponent<SixPlayerGameScene>().MoveChipsToWinner(seatList,winnerList,winAmountList);
                }
                else
                {
                    activeScene.GetComponent<SixPlayerTableScene>().MoveChipsToWinner(seatList,winnerList,winAmountList);
                }
            }
        }
        public void MoveChipsToWinner(List<Seat> seatList, string winner, float winAmount, GameObject activeScene, bool isGameScene)
        {
            if (seatList.Count == 6)
            {
                if (isGameScene)
                {
                    activeScene.GetComponent<SixPlayerGameScene>().MoveChipsToWinner(seatList,winner,winAmount);
                }
                else
                {
                    activeScene.GetComponent<SixPlayerTableScene>().MoveChipsToWinner(seatList,winner,winAmount);
                }
            }
        }

        public void ClearAllCards(List<Seat> seatList, GameObject activeScene, bool isPlayerInGame)
        {
            if (activeScene != null)
            {
                if (seatList.Count == 6)
                {
                    if (isPlayerInGame)
                    {
                        activeScene.GetComponent<SixPlayerGameScene>().ClearAllCards(seatList);
                        activeScene.GetComponent<SixPlayerGameScene>().HideAllHands(seatList);
                    }
                    else
                    {
                        activeScene.GetComponent<SixPlayerTableScene>().ClearAllCards(seatList);
                        activeScene.GetComponent<SixPlayerTableScene>().HideAllHands(seatList);
                    }
                }
            }
        }

        public void ClearAllBets(List<Seat> seatList, GameObject activeScene, bool isPlayerInGame)
        {
            if (activeScene == null)
            {
                return;
            }
            if (seatList.Count == 6)
            {
                if (isPlayerInGame)
                {
                    activeScene.GetComponent<SixPlayerGameScene>().ClearAllBets(seatList);
                    activeScene.GetComponent<SixPlayerGameScene>().HideAllBets(seatList);
                }
                else
                {
                    activeScene.GetComponent<SixPlayerTableScene>().ClearAllBets(seatList);
                    activeScene.GetComponent<SixPlayerTableScene>().HideAllBets(seatList);
                }
            }
        }

        public void HideDealerToken(List<Seat> seatList, GameObject activeScene, bool isPlayerInGame)
        {
            if (activeScene == null)
            {
                return;
            }
            if (seatList.Count == 6)
            {
                if (isPlayerInGame)
                {
                    activeScene.GetComponent<SixPlayerGameScene>().HideDealerToken(seatList);
                }
                else
                {
                    activeScene.GetComponent<SixPlayerTableScene>().HideDealerToken(seatList);
                }
            }
        }

        public void ClearSharedCards(List<Seat> seatList, GameObject activeScene, bool isPlayerInGame)
        {
            if (activeScene == null)
            {
                return; 
            }
            if (seatList.Count == 6)
            {
                if (isPlayerInGame)
                {
                    activeScene.GetComponent<SixPlayerGameScene>().ClearSharedCards(seatList);
                }
                else
                {
                    activeScene.GetComponent<SixPlayerTableScene>().ClearSharedCards(seatList);
                }
            }
        }

        public void OpenWinnerHand(List<Seat> seatList, string winnerName, List<CardData> winnerCards, GameObject activeScene, bool isGameScene)
        {
            if (seatList.Count == 6)
            {
                if (isGameScene)
                {
                    activeScene.GetComponent<SixPlayerGameScene>().OpenWinnerHand(seatList,winnerName,winnerCards);
                }
                else
                {
                    activeScene.GetComponent<SixPlayerTableScene>().OpenWinnerHand(seatList,winnerName,winnerCards);
                }
            }
        }

        

        public void MakeCorrectionBet(List<Seat> seatList, string correctionPlayerName, float correctionBetAmount, GameObject activeScene, bool isGameScene)
        {
            if (seatList.Count == 6)
            {
                if (isGameScene)
                {
                    activeScene.GetComponent<SixPlayerGameScene>().MakeCorrectionBet(seatList,correctionPlayerName,correctionBetAmount);
                }
                else
                {
                    activeScene.GetComponent<SixPlayerTableScene>().MakeCorrectionBet(seatList,correctionPlayerName,correctionBetAmount);
                }
            }
        }

        public void AnimateTurnCard(CardData turnCard, int seatCount, GameObject activeScene, bool isGameScene)
        {
            if (seatCount == 6)
            {
                if (isGameScene)
                {
                    activeScene.GetComponent<SixPlayerGameScene>().AnimateTurnCard(turnCard);
                }
                else
                {
                    activeScene.GetComponent<SixPlayerTableScene>().AnimateTurnCard(turnCard);
                }
            }
        }

        public void AnimateRiverCard(CardData riverCard, int seatCount, GameObject activeScene, bool isGameScene)
        {
            if (seatCount == 6)
            {
                if (isGameScene)
                {
                    activeScene.GetComponent<SixPlayerGameScene>().AnimateRiverCard(riverCard);
                }
                else
                {
                    activeScene.GetComponent<SixPlayerTableScene>().AnimateRiverCard(riverCard);
                }
            }
        }


        public void SetGameControllerValuesForMoveFold(List<Seat> seatList, string myName, string lastMovePlayerName,
            GameObject activeScene, float middlePot, int sideBetCount, List<float> sideBetList, float smallBlind,
            bool isStageChange)
        {
            if (seatList.Count == 6)
            {
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        if (string.Equals(seat.username,myName))
                        {
                            activeScene.GetComponent<SixPlayerGameScene>().SetGameControllerValuesForMoveFold(seatList,myName,lastMovePlayerName,seat.isMyTurn,middlePot,sideBetCount,sideBetList,smallBlind,isStageChange);
                        }
                    }
                }
            }
        }


        public void ToggleMoveIsDone(string myName, ToggleMove toggleMove, List<Seat> seatList, GameObject activeScene)
        {
            if (seatList.Count == 6)
            {
                activeScene.GetComponent<SixPlayerGameScene>().ToggleMoveIsDone(myName,toggleMove,seatList);
            }
        }

        public void HideEndGameButtons(int seatCount, GameObject activeScene)
        {
            if (seatCount == 6)
            {
                activeScene.GetComponent<SixPlayerGameScene>().HideEndGameButtons();
            }
        }

        

        

        public void SpawnBetsForSeatedPlayers(List<Seat> seatList, GameObject activeScene)
        {
            if (seatList.Count == 6)
            {
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        activeScene.GetComponent<SixPlayerGameScene>().SpawnBet(seat.location, seat.totalBetInSubTurn);
                    }
                }
            }
        }

        public void SpawnBetsForObserverPlayers(List<Seat> seatList, GameObject activeScene)
        {
            if (seatList.Count == 6)
            {
                foreach (var seat in seatList)
                {
                    if (seat.isActive)
                    {
                        activeScene.GetComponent<SixPlayerTableScene>().SpawnBet(seat.location, seat.totalBetInSubTurn);
                    }
                }
            }
        }

        public void ClearAllSideBets(List<Seat> seatList, GameObject activeScene, bool isPlayerInGame)
        {
            if (activeScene == null)
            {
                return;
            }
            if (seatList.Count == 6)
            {
                if (isPlayerInGame)
                {
                    activeScene.GetComponent<SixPlayerGameScene>().ClearAllSideBets();
                }
                else
                {
                    activeScene.GetComponent<SixPlayerTableScene>().ClearAllSideBets();
                }
            }
        }
    }
    
     
}