using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts
{
    public class TurnWinners : ParentObject
    {
        private int turnId;
        private string winner1;
        private float win1Amount;
        private string winner2;
        private float win2Amount;
        private string winner3;
        private float win3Amount;
        private string winner4;
        private float win4Amount;
        private string winner5;
        private float win5Amount;
        private string winner6;
        private float win6Amount;
        private string winner7;
        private float win7Amount;
        private string winner8;
        private float win8Amount;

        public TurnWinners()
        {
            
        }

        public TurnWinners(int turnId,string winnerName, float winAmount)
        {
            this.turnId = turnId;
            winner1 = winnerName;
            win1Amount = winAmount;
            winner2 = string.Empty;
            win2Amount = 0;
            winner3 = string.Empty;
            win3Amount = 0;
            winner4 = string.Empty;
            win4Amount = 0;
            winner5 = string.Empty;
            win5Amount = 0;
            winner6 = string.Empty;
            win6Amount = 0;
            winner7 = string.Empty;
            win7Amount = 0;
            winner8 = string.Empty;
            win8Amount = 0;
        }

        public TurnWinners(int turnId,List<string> winners,float winAmount)
        {
            this.turnId = turnId;
            switch (winners.Count)
            {
                case 2:
                    winner1 = winners[0];
                    win1Amount = winAmount;
                    winner2 = winners[1];
                    win2Amount = winAmount;
                    winner3 = string.Empty;
                    win3Amount = 0;
                    winner4 = string.Empty;
                    win4Amount = 0;
                    winner5 = string.Empty;
                    win5Amount = 0;
                    winner6 = string.Empty;
                    win6Amount = 0;
                    winner7 = string.Empty;
                    win7Amount = 0;
                    winner8 = string.Empty;
                    win8Amount = 0;
                    break;
                case 3:
                    winner1 = winners[0];
                    win1Amount = winAmount;
                    winner2 = winners[1];
                    win2Amount = winAmount;
                    winner3 = winners[2];
                    win3Amount = winAmount;
                    winner4 = string.Empty;
                    win4Amount = 0;
                    winner5 = string.Empty;
                    win5Amount = 0;
                    winner6 = string.Empty;
                    win6Amount = 0;
                    winner7 = string.Empty;
                    win7Amount = 0;
                    winner8 = string.Empty;
                    win8Amount = 0;
                    break;
                case 4:
                    winner1 = winners[0];
                    win1Amount = winAmount;
                    winner2 = winners[1];
                    win2Amount = winAmount;
                    winner3 = winners[2];
                    win3Amount = winAmount;
                    winner4 = winners[3];
                    win4Amount = winAmount;
                    winner5 = string.Empty;
                    win5Amount = 0;
                    winner6 = string.Empty;
                    win6Amount = 0;
                    winner7 = string.Empty;
                    win7Amount = 0;
                    winner8 = string.Empty;
                    win8Amount = 0;
                    break;
                case 5:
                    winner1 = winners[0];
                    win1Amount = winAmount;
                    winner2 = winners[1];
                    win2Amount = winAmount;
                    winner3 = winners[2];
                    win3Amount = winAmount;
                    winner4 = winners[3];
                    win4Amount = winAmount;
                    winner5 = winners[4];
                    win5Amount = winAmount;
                    winner6 = string.Empty;
                    win6Amount = 0;
                    winner7 = string.Empty;
                    win7Amount = 0;
                    winner8 = string.Empty;
                    win8Amount = 0;
                    break;
                case 6:
                    winner1 = winners[0];
                    win1Amount = winAmount;
                    winner2 = winners[1];
                    win2Amount = winAmount;
                    winner3 = winners[2];
                    win3Amount = winAmount;
                    winner4 = winners[3];
                    win4Amount = winAmount;
                    winner5 = winners[4];
                    win5Amount = winAmount;
                    winner6 = winners[5];
                    win6Amount = winAmount;
                    winner7 = string.Empty;
                    win7Amount = 0;
                    winner8 = string.Empty;
                    win8Amount = 0;
                    break;
                case 7:
                    winner1 = winners[0];
                    win1Amount = winAmount;
                    winner2 = winners[1];
                    win2Amount = winAmount;
                    winner3 = winners[2];
                    win3Amount = winAmount;
                    winner4 = winners[3];
                    win4Amount = winAmount;
                    winner5 = winners[4];
                    win5Amount = winAmount;
                    winner6 = winners[5];
                    win6Amount = winAmount;
                    winner7 = winners[6];
                    win7Amount = winAmount;
                    winner8 = string.Empty;
                    win8Amount = 0;
                    break;
                case 8:
                    winner1 = winners[0];
                    win1Amount = winAmount;
                    winner2 = winners[1];
                    win2Amount = winAmount;
                    winner3 = winners[2];
                    win3Amount = winAmount;
                    winner4 = winners[3];
                    win4Amount = winAmount;
                    winner5 = winners[4];
                    win5Amount = winAmount;
                    winner6 = winners[5];
                    win6Amount = winAmount;
                    winner7 = winners[6];
                    win7Amount = winAmount;
                    winner8 = winners[7];
                    win8Amount = winAmount;
                    break;
            }
            
        }

        public TurnWinners(int currentTurnID, List<Tuple<string, float>> winnersTierTupleList)
        {
            this.turnId = currentTurnID;
            switch (winnersTierTupleList.Count)
            {
                case 1:
                    winner1 = winnersTierTupleList[0].Item1;
                    win1Amount = winnersTierTupleList[0].Item2;
                    winner2 = string.Empty;
                    win2Amount = 0;
                    winner3 = string.Empty;
                    win3Amount = 0;
                    winner4 = string.Empty;
                    win4Amount = 0;
                    winner5 = string.Empty;
                    win5Amount = 0;
                    winner6 = string.Empty;
                    win6Amount = 0;
                    winner7 = string.Empty;
                    win7Amount = 0;
                    winner8 = string.Empty;
                    win8Amount = 0;
                    break;
                case 2:
                    winner1 = winnersTierTupleList[0].Item1;
                    win1Amount = winnersTierTupleList[0].Item2;
                    winner2 = winnersTierTupleList[1].Item1;
                    win2Amount = winnersTierTupleList[1].Item2;
                    winner3 = string.Empty;
                    win3Amount = 0;
                    winner4 = string.Empty;
                    win4Amount = 0;
                    winner5 = string.Empty;
                    win5Amount = 0;
                    winner6 = string.Empty;
                    win6Amount = 0;
                    winner7 = string.Empty;
                    win7Amount = 0;
                    winner8 = string.Empty;
                    win8Amount = 0;
                    break;
                case 3:
                    winner1 = winnersTierTupleList[0].Item1;
                    win1Amount = winnersTierTupleList[0].Item2;
                    winner2 = winnersTierTupleList[1].Item1;
                    win2Amount = winnersTierTupleList[1].Item2;
                    winner3 = winnersTierTupleList[2].Item1;
                    win3Amount = winnersTierTupleList[2].Item2;
                    winner4 = string.Empty;
                    win4Amount = 0;
                    winner5 = string.Empty;
                    win5Amount = 0;
                    winner6 = string.Empty;
                    win6Amount = 0;
                    winner7 = string.Empty;
                    win7Amount = 0;
                    winner8 = string.Empty;
                    win8Amount = 0;
                    break;
                case 4:
                    winner1 = winnersTierTupleList[0].Item1;
                    win1Amount = winnersTierTupleList[0].Item2;
                    winner2 = winnersTierTupleList[1].Item1;
                    win2Amount = winnersTierTupleList[1].Item2;
                    winner3 = winnersTierTupleList[2].Item1;
                    win3Amount = winnersTierTupleList[2].Item2;
                    winner4 = winnersTierTupleList[3].Item1;
                    win4Amount = winnersTierTupleList[3].Item2;
                    winner5 = string.Empty;
                    win5Amount = 0;
                    winner6 = string.Empty;
                    win6Amount = 0;
                    winner7 = string.Empty;
                    win7Amount = 0;
                    winner8 = string.Empty;
                    win8Amount = 0;
                    break;
                case 5:
                    winner1 = winnersTierTupleList[0].Item1;
                    win1Amount = winnersTierTupleList[0].Item2;
                    winner2 = winnersTierTupleList[1].Item1;
                    win2Amount = winnersTierTupleList[1].Item2;
                    winner3 = winnersTierTupleList[2].Item1;
                    win3Amount = winnersTierTupleList[2].Item2;
                    winner4 = winnersTierTupleList[3].Item1;
                    win4Amount = winnersTierTupleList[3].Item2;
                    winner5 = winnersTierTupleList[4].Item1;
                    win5Amount = winnersTierTupleList[4].Item2;
                    winner6 = string.Empty;
                    win6Amount = 0;
                    winner7 = string.Empty;
                    win7Amount = 0;
                    winner8 = string.Empty;
                    win8Amount = 0;
                    break;
                case 6:
                    winner1 = winnersTierTupleList[0].Item1;
                    win1Amount = winnersTierTupleList[0].Item2;
                    winner2 = winnersTierTupleList[1].Item1;
                    win2Amount = winnersTierTupleList[1].Item2;
                    winner3 = winnersTierTupleList[2].Item1;
                    win3Amount = winnersTierTupleList[2].Item2;
                    winner4 = winnersTierTupleList[3].Item1;
                    win4Amount = winnersTierTupleList[3].Item2;
                    winner5 = winnersTierTupleList[4].Item1;
                    win5Amount = winnersTierTupleList[4].Item2;
                    winner6 = winnersTierTupleList[5].Item1;
                    win6Amount = winnersTierTupleList[5].Item2;
                    winner7 = string.Empty;
                    win7Amount = 0;
                    winner8 = string.Empty;
                    win8Amount = 0;
                    break;
                case 7:
                    winner1 = winnersTierTupleList[0].Item1;
                    win1Amount = winnersTierTupleList[0].Item2;
                    winner2 = winnersTierTupleList[1].Item1;
                    win2Amount = winnersTierTupleList[1].Item2;
                    winner3 = winnersTierTupleList[2].Item1;
                    win3Amount = winnersTierTupleList[2].Item2;
                    winner4 = winnersTierTupleList[3].Item1;
                    win4Amount = winnersTierTupleList[3].Item2;
                    winner5 = winnersTierTupleList[4].Item1;
                    win5Amount = winnersTierTupleList[4].Item2;
                    winner6 = winnersTierTupleList[5].Item1;
                    win6Amount = winnersTierTupleList[5].Item2;
                    winner7 = winnersTierTupleList[6].Item1;
                    win7Amount = winnersTierTupleList[6].Item2;
                    winner8 = string.Empty;
                    win8Amount = 0;
                    break;
                case 8:
                    winner1 = winnersTierTupleList[0].Item1;
                    win1Amount = winnersTierTupleList[0].Item2;
                    winner2 = winnersTierTupleList[1].Item1;
                    win2Amount = winnersTierTupleList[1].Item2;
                    winner3 = winnersTierTupleList[2].Item1;
                    win3Amount = winnersTierTupleList[2].Item2;
                    winner4 = winnersTierTupleList[3].Item1;
                    win4Amount = winnersTierTupleList[3].Item2;
                    winner5 = winnersTierTupleList[4].Item1;
                    win5Amount = winnersTierTupleList[4].Item2;
                    winner6 = winnersTierTupleList[5].Item1;
                    win6Amount = winnersTierTupleList[5].Item2;
                    winner7 = winnersTierTupleList[6].Item1;
                    win7Amount = winnersTierTupleList[6].Item2;
                    winner8 = winnersTierTupleList[7].Item1;
                    win8Amount = winnersTierTupleList[7].Item2;
                    break;
                default:
                    Debug.LogError("Invalid number of winners In TurnWinners Constructor");
                    break;
            }

        }

        public int TurnId
        {
            get => turnId;
            set => turnId = value;
        }

        public string Winner1
        {
            get => winner1;
            set => winner1 = value;
        }

        public float Win1Amount
        {
            get => win1Amount;
            set => win1Amount = value;
        }

        public string Winner2
        {
            get => winner2;
            set => winner2 = value;
        }

        public float Win2Amount
        {
            get => win2Amount;
            set => win2Amount = value;
        }

        public string Winner3
        {
            get => winner3;
            set => winner3 = value;
        }

        public float Win3Amount
        {
            get => win3Amount;
            set => win3Amount = value;
        }

        public string Winner4
        {
            get => winner4;
            set => winner4 = value;
        }

        public float Win4Amount
        {
            get => win4Amount;
            set => win4Amount = value;
        }

        public string Winner5
        {
            get => winner5;
            set => winner5 = value;
        }

        public float Win5Amount
        {
            get => win5Amount;
            set => win5Amount = value;
        }

        public string Winner6
        {
            get => winner6;
            set => winner6 = value;
        }

        public float Win6Amount
        {
            get => win6Amount;
            set => win6Amount = value;
        }

        public string Winner7
        {
            get => winner7;
            set => winner7 = value;
        }

        public float Win7Amount
        {
            get => win7Amount;
            set => win7Amount = value;
        }

        public string Winner8
        {
            get => winner8;
            set => winner8 = value;
        }

        public float Win8Amount
        {
            get => win8Amount;
            set => win8Amount = value;
        }

        public float GetWinAmountByPlayerName(string playerName)
        {
            if (playerName == winner1)
            {
                return win1Amount;
            }
            else if (playerName == winner2)
            {
                return win2Amount;
            }
            else if (playerName == winner3)
            {
                return win3Amount;
            }
            else if (playerName == winner4)
            {
                return win4Amount;
            }
            else if (playerName == winner5)
            {
                return win5Amount;
            }
            else if (playerName == winner6)
            {
                return win6Amount;
            }
            else if (playerName == winner7)
            {
                return win7Amount;
            }
            else if (playerName == winner8)
            {
                return win8Amount;
            }
            else
            {
                Debug.LogError("Player name not found in TurnWinners");
                return 0;
            }
        }
    }
}