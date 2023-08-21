using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class LocalPlayer : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerNameText;
        [SerializeField] private TMP_Text balanceText;
        [SerializeField] private SeatLocations myLocation;
        [SerializeField] private Image turnTimerImage;
        private Coroutine countDownCoroutine;

        public void DisableTimer()
        {
            if(countDownCoroutine != null)
            {
                StopCoroutine(countDownCoroutine);
            }
            turnTimerImage.fillAmount = 0;
        }

        public void AnimateTurnTimer(int remainingTime, SeatLocations animationSeatLocation, int totalMoveTime,
            GameObject activeScene, List<Seat> seatList)
        {
            Debug.LogWarning("AnimateTurnTimer called From LocalPlayer. SeatLocation: "+animationSeatLocation+" remainingTime: "+remainingTime);
            int startTime = totalMoveTime-remainingTime;
            if(countDownCoroutine != null)
            {
                Debug.LogWarning("CountDownCoroutine is Stopped");
                StopCoroutine(countDownCoroutine);
            }
            countDownCoroutine = StartCoroutine(CountDown(startTime, totalMoveTime, remainingTime,activeScene, seatList));
        }


        private IEnumerator CountDown(int startTime, int totalMoveTime, int remainingTime, GameObject activeScene,
            List<Seat> seatList)
        {
            
            Debug.LogWarning("CountDown Coroutine Started. Remain Time: " + remainingTime);

            while (remainingTime > 0)
            {
                turnTimerImage.fillAmount = 1 - ((float)remainingTime / totalMoveTime);
                remainingTime--;

                yield return new WaitForSeconds(1f);
            }

            // Dolma işlemi için manuel ayarlama
            turnTimerImage.fillAmount = 1;

            Debug.LogWarning("DeactivateGameController. Remain Time: " + remainingTime);
            GameUISpawnManager.instance.DeactivateGameController(seatList, activeScene);

            // Coroutine'i sonlandırma
            Debug.LogWarning("CountDown Coroutine Ended. Remain Time: " + remainingTime);
            yield return null;
        }
        private IEnumerator CountDown(int totalMoveTime, int remainingTime, GameObject activeScene, List<Seat> seatList)
        {
            Debug.LogWarning("CountDown Coroutine Started. Remain Time : "+remainingTime);
            while (remainingTime >= 0)
            {
                turnTimerImage.fillAmount = 1 - Mathf.InverseLerp(0, totalMoveTime, remainingTime);
                remainingTime--;
                if (remainingTime == 0)
                {
                    Debug.LogWarning("DeacTivateGameController. Remain Time : "+remainingTime);
                    GameUISpawnManager.instance.DeactivateGameController(seatList, activeScene);
                }
                yield return new WaitForSeconds(1f);
            }
            turnTimerImage.fillAmount = 0;
            Debug.LogWarning("CountDown Coroutine Ended. Remain Time : "+remainingTime);
            yield return null;
        }
        
        
        //set the player name
        public void SetPlayerName(string name)
        {
            playerNameText.text = name;
        }
        public void SetPlayerInfo(Seat seat)
        {
            playerNameText.text = seat.username;
            balanceText.text = seat.balance.ToString("N1");
        }
        public TMP_Text PlayerNameText
        {
            get => playerNameText;
            set => playerNameText = value;
        }

        public TMP_Text BalanceText
        {
            get => balanceText;
            set => balanceText = value;
        }

        public SeatLocations MyLocation
        {
            get => myLocation;
            set => myLocation = value;
        }


        
    }
}