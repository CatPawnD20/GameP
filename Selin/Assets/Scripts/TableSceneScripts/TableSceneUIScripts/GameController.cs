 using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GameController : MonoBehaviour
    {
        ///////////////////////////////////////////////////Variables Section/////////////////////////////////////////////////////
        private float preMoveAmount = 0.0f;
        [SerializeField] private GameObject movesCanvas;
        [SerializeField] private GameObject preMovesCanvas;
        [Header("PreMoves")] 
        [SerializeField] private TMP_Text lastWinnerNameText;
        [SerializeField] private TMP_Text lastWinnerAmountText;
        [SerializeField] private Toggle foldCheckToggle;
        [SerializeField] private Toggle checkToggle;
        [SerializeField] private Toggle callToggle;
        [SerializeField] private TMP_Text callToggleAmountText;
        private float callToggleAmount = 0.0f;
        [Header("Moves")]
        [SerializeField] private Button foldButton;
        [SerializeField] private Button checkButton;
        [SerializeField] private Button callButton;
        [SerializeField] private TMP_Text callButtonAmountText;
        private float callButtonAmount = 0.0f;
        [SerializeField] private Button raisePot4Button;
        [SerializeField] private TMP_Text raisePot4ButtonAmountText;
        private float raisePot4ButtonAmount = 0.0f;
        [SerializeField] private Button raisePot2Button;
        [SerializeField] private TMP_Text raisePot2ButtonAmountText;
        private float raisePot2ButtonAmount = 0.0f;
        [SerializeField] private Button raisePotButton;
        [SerializeField] private TMP_Text raisePotButtonAmountText;
        private float raisePotButtonAmount = 0.0f;
        [SerializeField] private Button downPot10Button;
        [SerializeField] private TMP_Text downPot10ButtonAmountText;
        private float downPot10ButtonAmount = 0.0f;
        [SerializeField] private Button upPot10Button;
        [SerializeField] private TMP_Text upPot10ButtonAmountText;
        private float upPot10ButtonAmount = 0.0f;
        [SerializeField] private Button raiseButton;
        [SerializeField] private TMP_Text raiseButtonAmountText;
        private float raiseButtonAmount = 0.0f;
        [SerializeField] private Button allInButton;
        [SerializeField] private TMP_Text allInButtonAmountText;
        private float allInButtonAmount = 0.0f;
        [SerializeField] private Button showCardsButton;
        [SerializeField] private Button hideCardsButton;
        
        private List<Button> myButtonList = new List<Button>();
        private float myOpponentsMaxTotalBetInSubTurn = 0;
        private float opponentlastBet = 0;
        private int upButtonClickCount = 0;
        ///////////////////////////////////////////////////Events Section/////////////////////////////////////////////////////
        private void Awake()
        {
            movesCanvas.SetActive(false);
            preMovesCanvas.SetActive(false);
            lastWinnerNameText.text = string.Empty;
            lastWinnerAmountText.text = string.Empty;
            foldCheckToggle.isOn = false;
            checkToggle.isOn = false;
            callToggle.isOn = false;
            callToggleAmountText.text = string.Empty;
            callButtonAmountText.text = string.Empty;
            raisePot4ButtonAmountText.text = string.Empty;
            raisePot2ButtonAmountText.text = string.Empty;
            raisePotButtonAmountText.text = string.Empty;
            downPot10ButtonAmountText.text = string.Empty;
            upPot10ButtonAmountText.text = string.Empty;
            raiseButtonAmountText.text = string.Empty;
            allInButtonAmountText.text = string.Empty;
            
            myButtonList.Add(foldButton);
            myButtonList.Add(checkButton);
            myButtonList.Add(callButton);
            myButtonList.Add(raisePot4Button);
            myButtonList.Add(raisePot2Button);
            myButtonList.Add(raisePotButton);
            myButtonList.Add(downPot10Button);
            myButtonList.Add(upPot10Button);
            myButtonList.Add(raiseButton);
            myButtonList.Add(allInButton);
            myButtonList.Add(showCardsButton);
            myButtonList.Add(hideCardsButton);
        }
        ///////////////////////////////////////////////////ToggleMethods Section/////////////////////////////////////////////////////
        public void FoldCheckToggleMethod()
        {
            if (foldCheckToggle.isOn)
            {
                checkToggle.isOn = false;

                callToggle.isOn = false;

                Player.localPlayer.ToggleMoves(ToggleMove.FoldCheck, true ,0);
            }
            else
            {
                Player.localPlayer.ToggleMoves(ToggleMove.FoldCheck, false ,0);
            }
        }
        public void CheckToggleMethod()
        {
            if (checkToggle.isOn)
            {
                foldCheckToggle.isOn = false;

                callToggle.isOn = false;

                Player.localPlayer.ToggleMoves(ToggleMove.Check, true ,0);
            }else
            {
                Player.localPlayer.ToggleMoves(ToggleMove.Check, false ,0);
            }
        }
        public void CallToggleMethod()
        {
            if (callToggle.isOn)
            {
                foldCheckToggle.isOn = false;

                checkToggle.isOn = false;

                Player.localPlayer.ToggleMoves(ToggleMove.Call, true ,callToggleAmount);
            }
            else
            {
                Player.localPlayer.ToggleMoves(ToggleMove.Call, false ,callToggleAmount);
            }
        }
        ///////////////////////////////////////////////////ButtonMethods Section/////////////////////////////////////////////////////

        public void AllInButtonMethod()
        {
            upButtonClickCount = 0;
            myButtonList.ForEach(button => button.interactable = false);
            Player.localPlayer.NextMove(Move.AllIn, allInButtonAmount);
        }
        public void RaiseButtonMethod()
        {
            upButtonClickCount = 0;
            myButtonList.ForEach(button => button.interactable = false);
            if (raiseButtonAmount == allInButtonAmount)
            {
                Player.localPlayer.NextMove(Move.AllIn, raisePotButtonAmount);
            }
            else
            {
                Player.localPlayer.NextMove(Move.Raise, raiseButtonAmount);
            }
            
        }
        public void UpPot10ButtonMethod()
        {
            upButtonClickCount++;
            raiseButtonAmount = raiseButtonAmount + upPot10ButtonAmount;
            upPot10Button.interactable = true;
            if (raiseButtonAmount > raisePotButtonAmount || raiseButtonAmount > allInButtonAmount)
            {
                upPot10Button.interactable = false;
                raiseButtonAmount = raiseButtonAmount - upPot10ButtonAmount;
                upButtonClickCount--;
            }
            if (raiseButtonAmount == raisePotButtonAmount || raiseButtonAmount == allInButtonAmount)
            {
                upPot10Button.interactable = false;
            }
            downPot10Button.interactable = true;
            raiseButtonAmountText.text = raiseButtonAmount.ToString();
        }
        public void downPot10ButtonMethod()
        {
            upButtonClickCount--;
            raiseButtonAmount = raiseButtonAmount - downPot10ButtonAmount;
            if (upButtonClickCount > 0)
            {
                downPot10Button.interactable = true;
            }
            upPot10Button.interactable = true;
            raiseButtonAmountText.text = raiseButtonAmount.ToString();
        }
        public void RaisePotButtonMethod()
        {
            upButtonClickCount = 0;
            myButtonList.ForEach(button => button.interactable = false);
            if (raisePotButtonAmount == allInButtonAmount)
            {
                Player.localPlayer.NextMove(Move.AllIn, raisePotButtonAmount);
            }else
            {
                Player.localPlayer.NextMove(Move.Raise, raisePotButtonAmount);
            }
            
        }
        public void RaisePot2ButtonMethod()
        {
            upButtonClickCount = 0;
            myButtonList.ForEach(button => button.interactable = false);
            if (raisePot2ButtonAmount == allInButtonAmount)
            {
                Player.localPlayer.NextMove(Move.AllIn, raisePotButtonAmount);
            }else
            {
                Player.localPlayer.NextMove(Move.Raise, raisePot2ButtonAmount);
            }
            
        }
        public void RaisePot4ButtonMethod()
        {
            upButtonClickCount = 0;
            myButtonList.ForEach(button => button.interactable = false);
            if (raisePot4ButtonAmount == allInButtonAmount)
            {
                Player.localPlayer.NextMove(Move.AllIn, raisePotButtonAmount);
            }else
            {
                Player.localPlayer.NextMove(Move.Raise, raisePot4ButtonAmount);
            }
            
        }
        public void CallButtonMethod()
        {
            upButtonClickCount = 0;
            myButtonList.ForEach(button => button.interactable = false);
            if (callButtonAmount == allInButtonAmount)
            {
                Player.localPlayer.NextMove(Move.AllIn, raisePotButtonAmount);
            }else
            {
                Player.localPlayer.NextMove(Move.Call, callButtonAmount);
            }
            
        }
        public void CheckButtonMethod()
        {
            upButtonClickCount = 0;
            myButtonList.ForEach(button => button.interactable = false);
            Player.localPlayer.NextMove(Move.Check,0);
        }
        public void FoldButtonMethod()
        {
            upButtonClickCount = 0;
            myButtonList.ForEach(button => button.interactable = false);
            Player.localPlayer.NextMove(Move.Fold, 0);
        }
        public void ShowCardsButtonMethod()
        {
            upButtonClickCount = 0;
            myButtonList.ForEach(button => button.interactable = false);
            showCardsButton.gameObject.SetActive(false);
            hideCardsButton.gameObject.SetActive(false);
            allInButton.gameObject.SetActive(true);
            Player.localPlayer.NextMove(Move.ShowCards, 0);
        }
        public void HideCardsButtonMethod()
        {
            upButtonClickCount = 0;
            myButtonList.ForEach(button => button.interactable = false);
            showCardsButton.gameObject.SetActive(false);
            hideCardsButton.gameObject.SetActive(false);
            allInButton.gameObject.SetActive(true);
            Player.localPlayer.NextMove(Move.HideCards, 0);
        }
        ///////////////////////////////////////////////////NextMoveCall Section/////////////////////////////////////////////////////
        public void SetMovesCanvasValuesForMoveCall(List<Seat> seatList, string myName, string lastMovePlayerName,
            bool isStageChange, float middlePot, float smallBlind, int sideBetCount, List<float> sideBetList)
        {
            float myPreviousTotalBet = 0;
            myPreviousTotalBet = ISeatList.GetTotalBetInSubTurnByPlayerName(seatList, myName);
            myOpponentsMaxTotalBetInSubTurn = ISeatList.GetOpponenetsMaxTotalBetInSubTurn(seatList, myName);
            opponentlastBet = ISeatList.GetLastMoveAmountByPlayerName(seatList, lastMovePlayerName);
            float totalPot = ChipManager.instance.TotalPot;
            if (isStageChange)
            {
                middlePot = SumAllPots(middlePot, sideBetCount, sideBetList);
                totalPot = middlePot;
            }
            float myCurrentBalance = ISeatList.GetBalanceByPlayerName(seatList, myName);
            SetMoveButtonsAmountsForMoveCall(myPreviousTotalBet,totalPot,myCurrentBalance,isStageChange,smallBlind);
            SetMoveButtonsActivityForMoveCall(myPreviousTotalBet, myCurrentBalance);
        }

        private float SumAllPots(float middlePot, int sideBetCount, List<float> sideBetList)
        {
            for (int i = 0; i < sideBetCount-1; i++)
            {
                middlePot += sideBetList[i];
            }
            return middlePot;
        }

        private void SetMoveButtonsActivityForMoveCall(float myPreviousTotalBet, float myCurrentBalance)
        {
            SetCheckButton(myPreviousTotalBet,myOpponentsMaxTotalBetInSubTurn);
            SetCallButton(myCurrentBalance);
            SetRaisePotButton(myCurrentBalance);
            SetRaisePot2Button(myCurrentBalance);
            SetRaisePot4Button(myCurrentBalance);
            SetRaiseButton(myCurrentBalance);
            SetDownPot10Button(myCurrentBalance);
            SetUpPot10Button(myCurrentBalance);
            SetAllInButton(myCurrentBalance);
            SetFoldButton();
        }

        private void SetFoldButton()
        {
            if (callButtonAmount == 0)
            {
                foldButton.interactable = false;
            }
            else
            {
                foldButton.interactable = true;
            }
        }

        private void SetMoveButtonsAmountsForMoveCall(float myPreviousBet, float totalPot, float myCurrentBalance,
            bool isStageChange, float smallBlind)
        {
            callButtonAmount = myOpponentsMaxTotalBetInSubTurn - myPreviousBet;
            //raisePotButtonAmount = opponentlastBet * 2 + totalPot - myPreviousBet - myPreviousBet;
            raisePotButtonAmount = totalPot + callButtonAmount * 2 - myPreviousBet;
            if (isStageChange)
            {
                raisePotButtonAmount = totalPot;
            }
            raisePot2ButtonAmount = raisePotButtonAmount/2;
            raisePot4ButtonAmount = raisePotButtonAmount/4;
            
            downPot10ButtonAmount = smallBlind*2;
            upPot10ButtonAmount = smallBlind*2;
            raiseButtonAmount = smallBlind*2+callButtonAmount;
            allInButtonAmount = myCurrentBalance;
        }
        
        public void SetPreMovesCanvasValuesForMoveCall(List<Seat> seatList, string myName, string lastMovePlayerName)
        {
            float myPreviousTotalBet = 0;
            myPreviousTotalBet = ISeatList.GetTotalBetInSubTurnByPlayerName(seatList, myName);
            myOpponentsMaxTotalBetInSubTurn = ISeatList.GetOpponenetsMaxTotalBetInSubTurn(seatList, myName);
            opponentlastBet = ISeatList.GetLastMoveAmountByPlayerName(seatList, lastMovePlayerName);
            float totalPot = ChipManager.instance.TotalPot;
            float myCurrentBalance = ISeatList.GetBalanceByPlayerName(seatList, myName);
            SetPreMoveButtonsAmountsForMoveCall(myPreviousTotalBet);
            SetPreMoveButtonsActivityForMoveCall(myPreviousTotalBet, totalPot, myCurrentBalance);
        }

        private void SetPreMoveButtonsActivityForMoveCall(float myPreviousTotalBet, float totalPot, float myCurrentBalance)
        {
            SetCallToggleActivity(myCurrentBalance);
            SetCheckToggleActivity(myPreviousTotalBet, myOpponentsMaxTotalBetInSubTurn);
            SetFoldCheckToggleActivity(myPreviousTotalBet, myOpponentsMaxTotalBetInSubTurn);
        }

        private void SetPreMoveButtonsAmountsForMoveCall(float myPreviousBet)
        {
            callToggleAmount = myOpponentsMaxTotalBetInSubTurn - myPreviousBet;
            if (callToggleAmount < 0)
            {
                callToggleAmount = 0;
            }
        }
        
        ///////////////////////////////////////////////////NextMoveCheck Section/////////////////////////////////////////////////////
        
        public void SetMovesCanvasValuesForMoveCheck(List<Seat> seatList, string myName, string lastMovePlayerName,
            bool isStageChange, float middlePot, float smallBlind, int sideBetCount, List<float> sideBetList)
        {
            float myPreviousTotalBet = 0;
            myPreviousTotalBet = ISeatList.GetTotalBetInSubTurnByPlayerName(seatList, myName);
            myOpponentsMaxTotalBetInSubTurn = ISeatList.GetOpponenetsMaxTotalBetInSubTurn(seatList, myName);
            opponentlastBet = ISeatList.GetLastMoveAmountByPlayerName(seatList, lastMovePlayerName);
            float totalPot = ChipManager.instance.TotalPot;
            if (isStageChange)
            {
                middlePot = SumAllPots(middlePot, sideBetCount, sideBetList);
                totalPot = middlePot;
            }
            float myCurrentBalance = ISeatList.GetBalanceByPlayerName(seatList, myName);
            SetMoveButtonsAmountsForMoveCheck(myPreviousTotalBet,totalPot,myCurrentBalance,isStageChange,smallBlind);
            SetMoveButtonsActivityForMoveCheck(myPreviousTotalBet, myCurrentBalance);
        }

        private void SetMoveButtonsActivityForMoveCheck(float myPreviousTotalBet, float myCurrentBalance)
        {
            SetCheckButton(myPreviousTotalBet,myOpponentsMaxTotalBetInSubTurn);
            SetCallButton(myCurrentBalance);
            SetRaisePotButton(myCurrentBalance);
            SetRaisePot2Button(myCurrentBalance);
            SetRaisePot4Button(myCurrentBalance);
            SetRaiseButton(myCurrentBalance);
            SetDownPot10Button(myCurrentBalance);
            SetUpPot10Button(myCurrentBalance);
            SetAllInButton(myCurrentBalance);
            SetFoldButton();
        }

        private void SetMoveButtonsAmountsForMoveCheck(float myPreviousBet, float totalPot, float myCurrentBalance,
            bool isStageChange, float smallBlind)
        {
            callButtonAmount = myOpponentsMaxTotalBetInSubTurn - myPreviousBet;
            raisePotButtonAmount = totalPot + callButtonAmount * 2 - myPreviousBet;
            if (isStageChange)
            {
                raisePotButtonAmount = totalPot;
            }
            raisePot2ButtonAmount = raisePotButtonAmount/2;
            raisePot4ButtonAmount = raisePotButtonAmount/4;
            downPot10ButtonAmount = smallBlind*2;
            upPot10ButtonAmount = smallBlind*2;
            raiseButtonAmount = smallBlind*2 + callButtonAmount;
            allInButtonAmount = myCurrentBalance;
        }

        public void SetPreMovesCanvasValuesForMoveCheck(List<Seat> seatList, string myName, string lastMovePlayerName)
        {
            float myPreviousTotalBet = 0;
            myPreviousTotalBet = ISeatList.GetTotalBetInSubTurnByPlayerName(seatList, myName);
            myOpponentsMaxTotalBetInSubTurn = ISeatList.GetOpponenetsMaxTotalBetInSubTurn(seatList, myName);
            opponentlastBet = ISeatList.GetLastMoveAmountByPlayerName(seatList, lastMovePlayerName);
            float totalPot = ChipManager.instance.TotalPot;
            float myCurrentBalance = ISeatList.GetBalanceByPlayerName(seatList, myName);
            SetPreMoveButtonsAmountsForMoveCheck(myPreviousTotalBet);
            SetPreMoveButtonsActivityForMoveCheck(myPreviousTotalBet, totalPot, myCurrentBalance);
        }

        private void SetPreMoveButtonsActivityForMoveCheck(float myPreviousTotalBet, float totalPot, float myCurrentBalance)
        {
            SetCallToggleActivity(myCurrentBalance);
            SetCheckToggleActivity(myPreviousTotalBet, myOpponentsMaxTotalBetInSubTurn);
            SetFoldCheckToggleActivity(myPreviousTotalBet, myOpponentsMaxTotalBetInSubTurn);
        }

        private void SetPreMoveButtonsAmountsForMoveCheck(float myPreviousBet)
        {
            callToggleAmount = myOpponentsMaxTotalBetInSubTurn - myPreviousBet;
            if (callToggleAmount < 0)
            {
                callToggleAmount = 0;
            }
        }
        ///////////////////////////////////////////////////NextMoveFold Section/////////////////////////////////////////////////////
        public void SetMovesCanvasValuesForMoveFold(List<Seat> seatList, string myName, string lastMovePlayerName,
            float middlePot, int sideBetCount, List<float> sideBetList, float smallBlind, bool isStageChange)
        {
            float myPreviousTotalBet = 0;
            myPreviousTotalBet = ISeatList.GetTotalBetInSubTurnByPlayerName(seatList, myName);
            myOpponentsMaxTotalBetInSubTurn = ISeatList.GetOpponenetsMaxTotalBetInSubTurn(seatList, myName);
            opponentlastBet = ISeatList.GetLastMoveAmountByPlayerName(seatList, lastMovePlayerName);
            float totalPot = ChipManager.instance.TotalPot;
            if (isStageChange)
            {
                middlePot = SumAllPots(middlePot, sideBetCount, sideBetList);
                totalPot = middlePot;
            }
            float myCurrentBalance = ISeatList.GetBalanceByPlayerName(seatList, myName);
            SetMoveButtonsAmountsForMoveFold(myPreviousTotalBet,totalPot,myCurrentBalance,isStageChange,smallBlind);
            SetMoveButtonsActivityForMoveFold(myPreviousTotalBet, myCurrentBalance);
        }

        private void SetMoveButtonsActivityForMoveFold(float myPreviousTotalBet, float myCurrentBalance)
        {
            SetCheckButton(myPreviousTotalBet,myOpponentsMaxTotalBetInSubTurn);
            SetCallButton(myCurrentBalance);
            SetRaisePotButton(myCurrentBalance);
            SetRaisePot2Button(myCurrentBalance);
            SetRaisePot4Button(myCurrentBalance);
            SetRaiseButton(myCurrentBalance);
            SetDownPot10Button(myCurrentBalance);
            SetUpPot10Button(myCurrentBalance);
            SetAllInButton(myCurrentBalance);
            SetFoldButton();
        }

        private void SetMoveButtonsAmountsForMoveFold(float myPreviousBet, float totalPot, float myCurrentBalance, bool isStageChange, float smallBlind)
        {
            callButtonAmount = myOpponentsMaxTotalBetInSubTurn - myPreviousBet;
            raisePotButtonAmount = totalPot + callButtonAmount * 2 - myPreviousBet;
            if (isStageChange)
            {
                raisePotButtonAmount = totalPot;
            }
            raisePot2ButtonAmount = raisePotButtonAmount/2;
            raisePot4ButtonAmount = raisePotButtonAmount/4;
            downPot10ButtonAmount = smallBlind*2;
            upPot10ButtonAmount = smallBlind*2;
            raiseButtonAmount = smallBlind*2 + callButtonAmount;
            allInButtonAmount = myCurrentBalance;
        }

        public void SetPreMovesCanvasValuesForMoveFold(List<Seat> seatList, string myName, string lastMovePlayerName)
        {
            float myPreviousTotalBet = 0;
            myPreviousTotalBet = ISeatList.GetTotalBetInSubTurnByPlayerName(seatList, myName);
            myOpponentsMaxTotalBetInSubTurn = ISeatList.GetOpponenetsMaxTotalBetInSubTurn(seatList, myName);
            opponentlastBet = ISeatList.GetLastMoveAmountByPlayerName(seatList, lastMovePlayerName);
            float totalPot = ChipManager.instance.TotalPot;
            float myCurrentBalance = ISeatList.GetBalanceByPlayerName(seatList, myName);
            SetPreMoveButtonsAmountsForMoveFold(myPreviousTotalBet);
            SetPreMoveButtonsActivityForMoveFold(myPreviousTotalBet, totalPot, myCurrentBalance);
        }

        private void SetPreMoveButtonsActivityForMoveFold(float myPreviousTotalBet, float totalPot, float myCurrentBalance)
        {
            SetCallToggleActivity(myCurrentBalance);
            SetCheckToggleActivity(myPreviousTotalBet, myOpponentsMaxTotalBetInSubTurn);
            SetFoldCheckToggleActivity(myPreviousTotalBet, myOpponentsMaxTotalBetInSubTurn);
        }

        private void SetPreMoveButtonsAmountsForMoveFold(float myPreviousBet)
        {
            callToggleAmount = myOpponentsMaxTotalBetInSubTurn - myPreviousBet;
            if (callToggleAmount < 0)
            {
                callToggleAmount = 0;
            }
        }

        ///////////////////////////////////////////////////NextMoveRaise Section/////////////////////////////////////////////////////
         public void SetMovesCanvasValuesForMoveRaise(List<Seat> seatList, string myName, string lastMovePlayerName,
             float middlePot, int sideBetCount, List<float> sideBetList, float smallBlind)
        {
            float myPreviousTotalBet = 0;
            myPreviousTotalBet = ISeatList.GetTotalBetInSubTurnByPlayerName(seatList, myName);
            myOpponentsMaxTotalBetInSubTurn = ISeatList.GetOpponenetsMaxTotalBetInSubTurn(seatList, myName);
            opponentlastBet = ISeatList.GetLastMoveAmountByPlayerName(seatList, lastMovePlayerName);
            float totalPot = ChipManager.instance.TotalPot;
            float myCurrentBalance = ISeatList.GetBalanceByPlayerName(seatList, myName);
            SetMoveButtonsAmountsForMoveRaise(myPreviousTotalBet,totalPot,myCurrentBalance,smallBlind);
            SetMoveButtonsActivityForMoveRaise(myPreviousTotalBet, myCurrentBalance);
        }

        private void SetMoveButtonsActivityForMoveRaise(float myPreviousTotalBet, float myCurrentBalance)
        {
            SetCheckButton(myPreviousTotalBet,myOpponentsMaxTotalBetInSubTurn);
            SetCallButton(myCurrentBalance);
            SetRaisePotButton(myCurrentBalance);
            SetRaisePot2Button(myCurrentBalance);
            SetRaisePot4Button(myCurrentBalance);
            SetRaiseButton(myCurrentBalance);
            SetDownPot10Button(myCurrentBalance);
            SetUpPot10Button(myCurrentBalance);
            SetAllInButton(myCurrentBalance);
            SetFoldButton();
        }

        private void SetMoveButtonsAmountsForMoveRaise(float myPreviousBet, float totalPot, float myCurrentBalance,
            float smallBlind)
        {
            callButtonAmount = myOpponentsMaxTotalBetInSubTurn - myPreviousBet;
            raisePotButtonAmount = totalPot + callButtonAmount * 2 - myPreviousBet;
            raisePot2ButtonAmount = raisePotButtonAmount/2;
            raisePot4ButtonAmount = raisePotButtonAmount/4;
            downPot10ButtonAmount = smallBlind*2;
            upPot10ButtonAmount = smallBlind*2;
            raiseButtonAmount = smallBlind*2 + callButtonAmount;
            allInButtonAmount = myCurrentBalance;
        }

        public void SetPreMovesCanvasValuesForMoveRaise(List<Seat> seatList, string myName, string lastMovePlayerName)
        {
            float myPreviousTotalBet = 0;
            myPreviousTotalBet = ISeatList.GetTotalBetInSubTurnByPlayerName(seatList, myName);
            myOpponentsMaxTotalBetInSubTurn = ISeatList.GetOpponenetsMaxTotalBetInSubTurn(seatList, myName);
            opponentlastBet = ISeatList.GetLastMoveAmountByPlayerName(seatList, lastMovePlayerName);
            float totalPot = ChipManager.instance.TotalPot;
            float myCurrentBalance = ISeatList.GetBalanceByPlayerName(seatList, myName);
            SetPreMoveButtonsAmountsForMoveRaise(myPreviousTotalBet);
            SetPreMoveButtonsActivityForMoveRaise(myPreviousTotalBet, totalPot, myCurrentBalance);
        }

        private void SetPreMoveButtonsActivityForMoveRaise(float myPreviousTotalBet, float totalPot, float myCurrentBalance)
        {
            SetCallToggleActivity(myCurrentBalance);
            SetCheckToggleActivity(myPreviousTotalBet, myOpponentsMaxTotalBetInSubTurn);
            SetFoldCheckToggleActivity(myPreviousTotalBet, myOpponentsMaxTotalBetInSubTurn);
        }

        private void SetPreMoveButtonsAmountsForMoveRaise(float myPreviousBet)
        {
            callToggleAmount = myOpponentsMaxTotalBetInSubTurn - myPreviousBet;
            if (callToggleAmount < 0)
            {
                callToggleAmount = 0;
            }
        }

        ///////////////////////////////////////////////////NextMoveAllin Section/////////////////////////////////////////////////////
        public void SetMovesCanvasValuesForMoveAllin(List<Seat> seatList, string myName, string lastMovePlayerName,
            float middlePot, int sideBetCount, List<float> sideBetList, bool isStageChange, float smallBlind)
        {
            
            float myPreviousTotalBet = 0;
            myPreviousTotalBet = ISeatList.GetTotalBetInSubTurnByPlayerName(seatList, myName);
            myOpponentsMaxTotalBetInSubTurn = ISeatList.GetOpponenetsMaxTotalBetInSubTurn(seatList, myName);
            opponentlastBet = ISeatList.GetLastMoveAmountByPlayerName(seatList, lastMovePlayerName);
            float totalPot = ChipManager.instance.TotalPot;
            if (isStageChange)
            {
                middlePot = SumAllPots(middlePot, sideBetCount, sideBetList);
                totalPot = middlePot;
            }
            float myCurrentBalance = ISeatList.GetBalanceByPlayerName(seatList, myName);
            SetMoveButtonsAmountsForMoveAllin(myPreviousTotalBet,totalPot,myCurrentBalance,smallBlind);
            SetMoveButtonsActivityForMoveAllin(myPreviousTotalBet, myCurrentBalance);
        }

        private void SetMoveButtonsActivityForMoveAllin(float myPreviousTotalBet, float myCurrentBalance)
        {
            SetCheckButton(myPreviousTotalBet,myOpponentsMaxTotalBetInSubTurn);
            SetCallButton(myCurrentBalance);
            SetRaisePotButton(myCurrentBalance);
            SetRaisePot2Button(myCurrentBalance);
            SetRaisePot4Button(myCurrentBalance);
            SetRaiseButton(myCurrentBalance);
            SetDownPot10Button(myCurrentBalance);
            SetUpPot10Button(myCurrentBalance);
            SetAllInButton(myCurrentBalance);
            SetFoldButton();
        }

        private void SetMoveButtonsAmountsForMoveAllin(float myPreviousBet, float totalPot, float myCurrentBalance,
            float smallBlind)
        {
            callButtonAmount = myOpponentsMaxTotalBetInSubTurn - myPreviousBet;
            raisePotButtonAmount = totalPot + callButtonAmount * 2 - myPreviousBet;
            raisePot2ButtonAmount = raisePotButtonAmount/2;
            raisePot4ButtonAmount = raisePotButtonAmount/4;
            downPot10ButtonAmount = smallBlind*2;
            upPot10ButtonAmount = smallBlind*2;
            raiseButtonAmount = smallBlind*2 + callButtonAmount;
            allInButtonAmount = myCurrentBalance;
        }

        public void SetPreMovesCanvasValuesForMoveAllin(List<Seat> seatList, string myName, string lastMovePlayerName)
        {
            float myPreviousTotalBet = 0;
            myPreviousTotalBet = ISeatList.GetTotalBetInSubTurnByPlayerName(seatList, myName);
            myOpponentsMaxTotalBetInSubTurn = ISeatList.GetOpponenetsMaxTotalBetInSubTurn(seatList, myName);
            opponentlastBet = ISeatList.GetLastMoveAmountByPlayerName(seatList, lastMovePlayerName);
            float totalPot = ChipManager.instance.TotalPot;
            float myCurrentBalance = ISeatList.GetBalanceByPlayerName(seatList, myName);
            SetPreMoveButtonsAmountsForMoveAllin(myPreviousTotalBet);
            SetPreMoveButtonsActivityForMoveAllin(myPreviousTotalBet, totalPot, myCurrentBalance);
        }

        private void SetPreMoveButtonsActivityForMoveAllin(float myPreviousTotalBet, float totalPot, float myCurrentBalance)
        {
            SetCallToggleActivity(myCurrentBalance);
            SetCheckToggleActivity(myPreviousTotalBet, myOpponentsMaxTotalBetInSubTurn);
            SetFoldCheckToggleActivity(myPreviousTotalBet, myOpponentsMaxTotalBetInSubTurn);
        }

        private void SetPreMoveButtonsAmountsForMoveAllin(float myPreviousBet)
        {
            callToggleAmount = myOpponentsMaxTotalBetInSubTurn - myPreviousBet;
            if (callToggleAmount < 0)
            {
                callToggleAmount = 0;
            }
        }

        ///////////////////////////////////////////////////SmallAndBig Section/////////////////////////////////////////////////////
        public void ActivateDeactivatePreMovesCanvas(bool activate)
        {
            movesCanvas.SetActive(!activate);
            preMovesCanvas.SetActive(activate);
        }
        public void ActivateDeactivateMovesCanvas(bool activate)
        {
            
            myButtonList.ForEach(button => button.interactable = activate);
            movesCanvas.SetActive(activate);
            preMovesCanvas.SetActive(!activate);
        }

        public void SetPreMovesCanvasValuesForBig(string playerName, List<Seat> seatList)
        {
            float myPreviousBet = 0;
            myPreviousBet = FindMyBet(playerName,seatList);
            myOpponentsMaxTotalBetInSubTurn = ChipManager.instance.SubMaxBet;
            float totalPot = ChipManager.instance.TotalPot;
            float myCurrentBalance = ISeatList.GetBalanceByPlayerName(seatList, playerName);
            SetButtonsAmountsBig(myPreviousBet);
            SetButtonsActivityBig(myPreviousBet, totalPot, myCurrentBalance);
        }
        
        public void SetMovesCanvasValuesForSmall(string playerName, List<Seat> seatList, float smallBlind)
        {
            float myPreviousBet = 0;
            myPreviousBet = FindMyBet(playerName,seatList);
            myOpponentsMaxTotalBetInSubTurn = ChipManager.instance.SubMaxBet;
            float totalPot = ChipManager.instance.TotalPot;
            float myCurrentBalance = ISeatList.GetBalanceByPlayerName(seatList, playerName);
            
            SetButtonsAmountsForSmall(myPreviousBet,totalPot,myCurrentBalance,smallBlind);
            SetButtonsActivityForSmall(myPreviousBet, totalPot, myCurrentBalance);
        }

        private void SetButtonsActivityBig(float myPreviousBet, float totalPot, float myCurrentBalance)
        {
            SetAllToggleFalse();
            SetCallToggleActivity(myCurrentBalance);
            SetCheckToggleActivity(myPreviousBet, myOpponentsMaxTotalBetInSubTurn);
            SetFoldCheckToggleActivity(myPreviousBet, myOpponentsMaxTotalBetInSubTurn);
        }

        private void SetAllToggleFalse()
        {
            callToggle.isOn = false;
            checkToggle.isOn = false;
            foldCheckToggle.isOn = false;
        }

        private void SetButtonsActivityForSmall(float myPreviousBet, float totalPot, float myCurrentBalance)
        {
            SetCheckButton(myPreviousBet,myOpponentsMaxTotalBetInSubTurn);
            SetCallButton(myCurrentBalance);
            SetRaisePotButton(myCurrentBalance);
            SetRaisePot2Button(myCurrentBalance);
            SetRaisePot4Button(myCurrentBalance);
            SetDownPot10Button(myCurrentBalance);
            SetUpPot10Button(myCurrentBalance);
            SetRaiseButton(myCurrentBalance);
            SetAllInButtonForSmall(myCurrentBalance);
            SetFoldButton();
        }

        private void SetButtonsAmountsForSmall(float myPreviousBet, float totalPot,
            float myCurrentBalance, float smallBlind)
        {
            callButtonAmount = myOpponentsMaxTotalBetInSubTurn - myPreviousBet;
            raisePotButtonAmount = callButtonAmount * 2 + totalPot - myPreviousBet;
            raisePot2ButtonAmount = raisePotButtonAmount/2;
            raisePot4ButtonAmount = raisePotButtonAmount/4;
            downPot10ButtonAmount = smallBlind*2;
            upPot10ButtonAmount = smallBlind*2;
            raiseButtonAmount = smallBlind*2 + callButtonAmount;
            allInButtonAmount = myCurrentBalance;
        }
        private void SetFoldCheckToggleActivity(float myPreviousTotalBet, float myOppeonentsMaxPreviousBet)
        {
            foldCheckToggle.interactable = true;
            /*
             *
             * if (myPreviousTotalBet >= myOppeonentsMaxPreviousBet)
            {
                foldCheckToggle.interactable = true;
            }
            else
            {
                foldCheckToggle.interactable = false;
                foldCheckToggle.isOn = false;
            }
             */
            
        }

        private void SetCheckToggleActivity(float myPreviousTotalBet, float myOppeonentsMaxPreviousBet)
        {
            if (myPreviousTotalBet >= myOppeonentsMaxPreviousBet)
            {
                checkToggle.interactable = true;
            }
            else
            {
                checkToggle.interactable = false;
                checkToggle.isOn = false;
            }
        }

        private void SetCallToggleActivity(float myCurrentBalance)
        {
            if (callToggleAmount > 0)
            {
                if (myCurrentBalance >= callToggleAmount)
                {
                    callToggle.interactable = true;
                    callToggleAmountText.text = callToggleAmount.ToString("N1");
                }
            }
            else
            {
                callToggleAmount = 0;
                callToggleAmountText.text = callToggleAmount.ToString("N1");
                callToggle.interactable = false;
                callToggle.isOn = false;
            }
            
        }

        private void SetButtonsAmountsBig(float myPreviousBet)
        { 
            callToggleAmount = myOpponentsMaxTotalBetInSubTurn - myPreviousBet;
            if (callToggleAmount < 0)
            {
                callToggleAmount = 0;
            }
            
        }
        
        private float FindMyBet(string playerName, List<Seat> seatList)
        {
            float myBet = 0;
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    if (string.Equals(seat.username, playerName))
                    {
                        myBet = seat.lastMoveAmount;
                    }
                }
            }

            return myBet;
        }
        
        private void SetAllInButton(float myCurrentBalance)
        {
            if (myCurrentBalance <= raisePotButtonAmount)
            {
                allInButton.interactable = true;
                allInButtonAmountText.text = myCurrentBalance.ToString("N1");
            }
            else
            {
                allInButtonAmountText.text = myCurrentBalance.ToString("N1");
                allInButton.interactable = false;
            }
        }
        private void SetAllInButtonForSmall(float myCurrentBalance)
        {
            if (myCurrentBalance <= raisePotButtonAmount)
            {
                allInButton.interactable = true;
                allInButtonAmountText.text = myCurrentBalance.ToString("N1");
            }
            else
            {
                allInButtonAmountText.text = myCurrentBalance.ToString("N1");
                allInButton.interactable = false;
            }
        }

        private void SetRaiseButton( float myCurrentBalance)
        {
            raiseButton.interactable = false;
            if (myCurrentBalance >= raiseButtonAmount)
            {
                raiseButton.interactable = true;
                raiseButtonAmountText.text = raiseButtonAmount.ToString("N1");
            }
        }

        private void SetUpPot10Button(float myCurrentBalance)
        {
            upPot10Button.interactable = false;
            if (raiseButton.IsActive())
            {
                upPot10Button.interactable = true;
            }
            upPot10ButtonAmountText.text = upPot10ButtonAmount.ToString("N1");
        }

        private void SetDownPot10Button(float myCurrentBalance)
        {
            downPot10Button.interactable = false;
            downPot10ButtonAmountText.text = downPot10ButtonAmount.ToString("N1");
        }

        private void SetRaisePot4Button(float myCurrentBalance)
        {
            raisePot4ButtonAmountText.text = raisePot4ButtonAmount.ToString("N1");
            if (callButtonAmount < raisePot4ButtonAmount)
            {
                if (myCurrentBalance >= raisePot4ButtonAmount)
                {
                    raisePot4Button.interactable = true;
                }
                else
                {
                    raisePot4Button.interactable = false;
                }
            }
            else
            {
                raisePot4Button.interactable = false;
            }
            
            
        }
        
        private void SetRaisePot2Button(float myCurrentBalance)
        {
            raisePot2ButtonAmountText.text = raisePot2ButtonAmount.ToString("N1");
            if (callButtonAmount < raisePot2ButtonAmount)
            {
                if (myCurrentBalance >= raisePot2ButtonAmount)
                {
                    raisePot2Button.interactable = true;
                }
                else
                {
                    raisePot2Button.interactable = false;
                }
            }
            else
            {
                raisePot2Button.interactable = false;
            }
        }
        
        

        private void SetRaisePotButton(float myCurrentBalance)
        {
            raisePotButtonAmountText.text = raisePotButtonAmount.ToString("N1");
            
            if (callButtonAmount < raisePotButtonAmount)
            {
                if (myCurrentBalance > raisePotButtonAmount)
                {
                    raisePotButton.interactable = true;
                }
                else
                {
                    raisePotButton.interactable = false;
                }
            }
            else
            {
                raisePotButton.interactable = false;
            }
        }

        private void SetCallButton(float myCurrentBalance)
        {
            if (callButtonAmount < 0)
            {
                callButtonAmount = 0;
            }
            callButtonAmountText.text = callButtonAmount.ToString("N1");
            if (myCurrentBalance > callButtonAmount && callButtonAmount > 0)
            {
                
                callButton.interactable = true;
            }
            else
            {
                callButton.interactable = false;
            }
            
            
        }

        private bool isPlayerBalanceEnough(string playerName, List<Seat> seatList, float callAmount)
        {
            bool isEnough = false;
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    if (string.Equals(seat.username, playerName))
                    {
                        if (seat.balance >= callAmount)
                        {
                            isEnough = true;
                        }
                    }
                }
            }
            return isEnough;
        }

        private void SetCheckButton(float myPreviousTotalBet, float myOpponentsMaxTotalBetInSubTurn)
        {
            if (myPreviousTotalBet == myOpponentsMaxTotalBetInSubTurn)
            {
                checkButton.interactable = true;
            }
            else
            {
                checkButton.interactable = false;
            }
        }
        
        ///////////////////////////////////////////////////Properties Section/////////////////////////////////////////////////////
        
        public List<Button> MyButtonList => myButtonList;

        public float PreMoveAmount
        {
            get => preMoveAmount;
            set => preMoveAmount = value;
        }

        public GameObject MovesCanvas => movesCanvas;

        public GameObject PreMovesCanvas => preMovesCanvas;

        public TMP_Text LastWinnerNameText => lastWinnerNameText;

        public TMP_Text LastWinnerAmountText => lastWinnerAmountText;

        public Toggle FoldCheckToggle => foldCheckToggle;

        public Toggle CheckToggle => checkToggle;

        public Toggle CallToggle => callToggle;

        public TMP_Text CallToggleAmountText => callToggleAmountText;

        public Button FoldButton => foldButton;

        public Button CheckButton => checkButton;

        public Button CallButton => callButton;

        public TMP_Text CallButtonAmountText => callButtonAmountText;

        public Button RaisePot4Button => raisePot4Button;

        public TMP_Text RaisePot4ButtonAmountText => raisePot4ButtonAmountText;

        public Button RaisePot2Button => raisePot2Button;

        public TMP_Text RaisePot2ButtonAmountText => raisePot2ButtonAmountText;

        public Button RaisePotButton => raisePotButton;

        public TMP_Text RaisePotButtonAmountText => raisePotButtonAmountText;

        public Button DownPot10Button => downPot10Button;

        public TMP_Text DownPot10ButtonAmountText => downPot10ButtonAmountText;

        public Button UpPot10Button => upPot10Button;

        public TMP_Text UpPot10ButtonAmountText => upPot10ButtonAmountText;

        public Button RaiseButton => raiseButton;

        public TMP_Text RaiseButtonAmountText => raiseButtonAmountText;

        public Button AllInButton => allInButton;

        public TMP_Text AllInButtonAmountText => allInButtonAmountText;

        public Button ShowCardsButton => showCardsButton;

        public Button HideCardsButton => hideCardsButton;


        public void ToggleMoveIsDone()
        {
            ActivateDeactivatePreMovesCanvas(true);
            foldCheckToggle.isOn = false;
            checkToggle.isOn = false;
            callToggle.isOn = false;
        }

        public bool isShowCardsEnabled()
        {
            if (showCardsButton.IsActive())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void HideEndGameButtons()
        {
            showCardsButton.gameObject.SetActive(false);
            hideCardsButton.gameObject.SetActive(false);
            allInButton.gameObject.SetActive(true);
        }
    }
}