using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class nonLocalPlayer : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerNameText;
        [SerializeField] private TMP_Text balanceText;
        [SerializeField] private SeatLocations myLocation;
        [SerializeField] private TMP_Text lastMoveText;
        [SerializeField] private Image turnTimerImage;
        public TMP_Text LastMoveText => lastMoveText;
        
        private Coroutine countDownCoroutine;

        public void DisableTimer()
        {
            if(countDownCoroutine != null)
            {
                StopCoroutine(countDownCoroutine);
            }
            turnTimerImage.fillAmount = 0;
        }

        public void AnimateTurnTimer(int remainingTime, SeatLocations animationSeatLocation, int totalMoveTime)
        {
            int startTime = totalMoveTime-remainingTime;
            if(countDownCoroutine != null)
            {
                StopCoroutine(countDownCoroutine);
            }
            countDownCoroutine = StartCoroutine(CountDown(startTime, totalMoveTime, remainingTime));
        }


        private IEnumerator CountDown(int startTime, int totalMoveTime, int remainingTime)
        {
            while (remainingTime >= 0)
            {
                turnTimerImage.fillAmount = 1 - Mathf.InverseLerp(0, totalMoveTime, remainingTime);
                remainingTime--;
                yield return new WaitForSeconds(1f);
            }
            turnTimerImage.fillAmount = 0;
        }


        public void SetPlayerInfo(Seat seat)
        {
            playerNameText.text = seat.username;
            balanceText.text = seat.balance.ToString("N1");
            lastMoveText.text = seat.lastMove.ToString();
            
        }
        public SeatLocations MyLocation
        {
            get => myLocation;
            set => myLocation = value;
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


       
    }
}