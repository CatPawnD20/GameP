using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public static class ISeatList 
    {
        public static List<Seat> ReArrange(List<Seat> seatList, string localPlayerName)
        {
            int localPlayerRealLocation = 0;
            int reLocationIndex = 0;
            List<Seat> dummySeatList = new List<Seat>();
            if (seatList.Count == 6)
            {
                localPlayerRealLocation = GetPlayerLocationByName(seatList, localPlayerName);
                reLocationIndex = 13 - localPlayerRealLocation;
                foreach (var seat in seatList)
                {
                    int newLocation = ((int) seat.location + reLocationIndex) % 12;
                    Seat dummySeat = new Seat();
                    dummySeat = ISeat.TransferSeatData(dummySeat, seat);
                    dummySeat.location = (SeatLocations) newLocation;
                    dummySeatList.Add(dummySeat);
                }
            }

            if (seatList.Count == 8)
            {
                localPlayerRealLocation = GetPlayerLocationByName(seatList, localPlayerName);
                Dictionary<int, int> dummyMap = GamePlayerSpawnManager.instance.RelocationMap[localPlayerRealLocation];
                foreach (var seat in seatList)
                {
                    int newLocation = dummyMap[(int) seat.location];
                    Seat dummySeat = new Seat();
                    dummySeat = ISeat.TransferSeatData(dummySeat, seat);
                    dummySeat.location = (SeatLocations) newLocation;
                    dummySeatList.Add(dummySeat);
                }
            }

            return dummySeatList;
        }
        public static float GetTotalBetInSubTurnByPlayerName(List<Seat> seatList, string playerName)
        {
            float totalBet = 0;
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    if (seat.username == playerName)
                    {
                        totalBet = seat.totalBetInSubTurn;
                    }
                }
            }

            return totalBet;
        }
        public static float GetLastMoveAmountByPlayerName(List<Seat> seatList, string playerName)
        {
            float lastMoveAmount = 0;
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    if (seat.username == playerName)
                    {
                        lastMoveAmount = seat.lastMoveAmount;
                    }
                }
            }

            return lastMoveAmount;
        }
        public static int GetPlayerLocationByName(List<Seat> seatList, string localPlayerName)
        {
            int localPlayerRealLocation = 0;
            foreach (var seat in seatList)
            {
                if (string.Equals(seat.username, localPlayerName))
                {
                    localPlayerRealLocation = (int) seat.location;
                }
            }

            return localPlayerRealLocation;
        }
        public static string GetPlayerNameByLocation(List<Seat> seatList, SeatLocations seatLocations)
        {
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    if (seat.location == seatLocations)
                    {
                        return seat.username;
                    }
                }
            }
            return null;
        }
        public static int GetLocationByIsMyTurn(List<Seat> seatList)
        {
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    if (seat.isMyTurn)
                    {
                        return (int) seat.location;
                    }
                }
            }

            return 0;
        }
        public static string GetPlayerNameByIsMyTurn(List<Seat> seatList)
        {
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    if (seat.isMyTurn)
                    {
                        return seat.username;
                    }
                }
            }
            return string.Empty;
        }
        public static float GetMoveTimeByPlayerName(List<Seat> seatList, string playerName)
        {
            foreach (var seat in seatList)
            {
                if (string.Equals(playerName, seat.username))
                {
                    return seat.moveTime;
                }
            }
            return 0;
        }
        public static float GetBalanceByPlayerName(List<Seat> seatList, string playerName)
        {
            foreach (var seat in seatList)
            {
                if (string.Equals(playerName, seat.username))
                {
                    return seat.balance;
                }
            }
            return 0;
        }
        public static SeatLocations GetSeatLocationByIsMyTurn(List<Seat> seatList)
        {
            SeatLocations seatLocation = SeatLocations.None;
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    if (seat.isMyTurn)
                    {
                        seatLocation = seat.location;
                    }
                }
            }
            return seatLocation;
        }
        public static List<Seat> UpdateSeatByLocation(Seat newSeat, List<Seat> seatList)
        {
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    if (string.Equals(seat.username, newSeat.username))
                    {
                        Debug.LogWarning("TableGameManager.cs -->>> SyncID : " + "-->>>SeatListUpdateSeatByLocation" +
                                         "Player exist in ISeatList!!! : PlayerName -> " + newSeat.username);

                        return seatList;
                    }

                }
                else
                {
                    if (newSeat.location == seat.location)
                    {
                        seatList.Remove(seat);
                        seatList.Add(newSeat);
                        return seatList;
                    }
                }
            }

            return seatList;
        }
        public static bool ContainSeatByUsername(string playerName, List<Seat> seatList)
        {
            foreach (var seat in seatList)
            {
                if (string.Equals(seat.username, playerName))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool isSeatActiveByLocation(SeatLocations location, List<Seat> seatList)
        {
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    if (seat.location == location)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static int GetActivePlayerCount(List<Seat> seatList)
        {
            int i = 0;
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    i = i + 1;
                }
            }
            return i;
        }
        public static Move GetLastMoveByPlayerName(List<Seat> seatList, string playerName)
        {
            foreach (var seat in seatList)
            {
                if (seat.username == playerName)
                {
                    return seat.lastMove;
                }
            }
            return Move.None;
        }
        public static int FindCurrentDealerLocation(List<Seat> seatList)
        {
            int dealerLocation = 0;
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    if (seat.isDealer)
                    {
                        dealerLocation = (int) seat.location;
                        return dealerLocation;
                    }
                }
            }
            return dealerLocation;
        }
        public static bool CheckIsMyTurnByPlayerName(List<Seat> mySeatList, string playerName, Move nextMove)
        {
            bool hisTurn = false;
            foreach (var seat in mySeatList)
            {
                if (seat.isActive)
                {
                    if (string.Equals(seat.username, playerName))
                    {
                        hisTurn = seat.isMyTurn;
                    }
                    
                }
            }
            if (!hisTurn)
            {
                Debug.LogWarning("Player hasTurn is false!!! PlayerName : " + playerName+" NextMove : "+nextMove);
            }
            return hisTurn;
            
        }
        public static bool CheckLastMoveIsNotFoldByPlayerName(List<Seat> seatList, string playerName)
        {
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    if (string.Equals(seat.username, playerName))
                    {
                        if (seat.lastMove == Move.Fold)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
            Debug.LogWarning("No Active Seat Found For Player " + playerName + " SeatListCheckLastMoveIsNotFoldByPlayerName Return Value : false");
            return false;
        }
        public static bool CheckPlayerHasEnoughBalance(List<Seat> seatList, string playerName, float moveAmount)
        {
            if (GetBalanceByPlayerName(seatList,playerName) >= moveAmount)
            {
                return true;
            }
            return false;
        }
        public static List<Seat> SetSeatVarIsPlayerMovedInSubTurnByPlayerName(List<Seat> seatList, string playerName,bool condition)
        {
            Seat dummySeat = GetSeatByPlayerName(seatList, playerName);
            return SetSeatVarIsPlayerMovedInSubTurn(seatList,dummySeat, condition);
        }
        public static List<Seat> SetSeatVarIsDealerByLocation(List<Seat> seatList, int seatLocation,bool isDealer,TurnManager turnManager)
        {
            Seat dummySeat = GetSeatByLocation(seatList, (SeatLocations) seatLocation);
            seatList = SetSeatVarIsDealer(seatList,dummySeat, isDealer,turnManager);
            return seatList;
        }
        public static List<Seat> SetSeatVarIsMyTurnByLocation(List<Seat> seatList, int hasTurnLocation, bool isMyTurn, TurnManager turnManager)
        {
            Seat dummySeat = GetSeatByLocation(seatList, (SeatLocations) hasTurnLocation);
            seatList = SetSeatVarISMyTurn(seatList,dummySeat,isMyTurn,turnManager);
            return seatList;
        }
        public static List<Seat> SetSeatVarLastMoveByMove(List<Seat> seatList, GameMove gameMove)
        {
            Seat dummySeat = GetSeatByPlayerName(seatList, gameMove.PlayerName);
            seatList = SetSeatVarLastMove(seatList,dummySeat,gameMove.Move);
            return seatList;
        }
        public static List<Seat> SetSeatVarMoveTimeByMove(List<Seat> seatList, GameMove gameMove)
        {
            Seat dummySeat = GetSeatByPlayerName(seatList, gameMove.PlayerName);
            seatList = SetSeatVarMoveTime(seatList,dummySeat,gameMove.MoveTime);
            return seatList;
        }
        public static List<Seat> SetSeatVarMoveTimeByPlayerName(List<Seat> seatList, string playerName,int turnTime)
        {
            Seat dummySeat = GetSeatByPlayerName(seatList, playerName);
            return SetSeatVarMoveTime(seatList,dummySeat,turnTime);
        }
        public static List<Seat> SetLastMoveByPlayerName(List<Seat> seatList, Move myNextMove, string playerName)
        {
            Seat dummySeat = GetSeatByPlayerName(seatList, playerName);
            return SetSeatVarLastMove(seatList,dummySeat, myNextMove);
        }
        public static List<Seat> SetSeatVarLastMoveAmountByPlayerName(List<Seat> seatList, float moveAmount, string playerName)
        {
            Seat dummySeat = GetSeatByPlayerName(seatList, playerName);
            return SetSeatVarLastMoveAmount(seatList,dummySeat,moveAmount);
        }
        public static List<Seat> SetSeatVarIsPlayerInGameByPlayerName(List<Seat> seatList, string playerName, bool condition)
        {
            Seat dummySeat = GetSeatByPlayerName(seatList, playerName);
            return SetSeatVarIsPlayerInGame(seatList,dummySeat,condition);
        }
        public static List<Seat> SetSeatVarLastMoveAmountByGameMove(List<Seat> seatList, GameMove gameMove)
        {
            Seat dummySeat = GetSeatByPlayerName(seatList, gameMove.PlayerName);
            return SetSeatVarLastMoveAmount(seatList,dummySeat,gameMove.Amount);
        }
        public static List<Seat> UpdateSeatVarBalanceByGameMove(List<Seat> seatList, GameMove gameMove)
        {
            Seat dummySeat = GetSeatByPlayerName(seatList, gameMove.PlayerName);
            return UpdateSeatVarMinusBalance(seatList,dummySeat,gameMove.Amount);
        }
        public static List<Seat> UpdateSeatVarTotalBetInSubTurnByMove(List<Seat> seatList, GameMove gameMove)
        {
            Seat dummySeat = GetSeatByPlayerName(seatList, gameMove.PlayerName);
            return UpdateSeatVarTotalBetInSubTurn(seatList,dummySeat,gameMove.Amount);
        }
        public static List<Seat> UpdateSeatVarTotalBetInSubTurnByPlayerName(List<Seat> seatList, string playerName, float moveAmount)
        {
            Seat dummySeat = GetSeatByPlayerName(seatList, playerName);
            return UpdateSeatVarTotalBetInSubTurn(seatList,dummySeat,moveAmount);
        }
        public static List<Seat> UpdateSeatVarIsPlayerInGameByMove(List<Seat> seatList, string playerName, Move move)
        {
            Seat dummySeat = GetSeatByPlayerName(seatList, playerName);
            return UpdateSeatVarIsPlayerInGame(seatList,dummySeat,move);
        }
        
        public static Seat GetSeatByLocation(List<Seat> seatList, SeatLocations seatLocation)
        {
            Seat dummySeat = new Seat();
            foreach (var seat in seatList)
            {
                if (seat.location == seatLocation)
                {
                    return seat;
                }
            }
            Debug.LogError("No Seat Found For Location " + seatLocation + " GetSeatByLocation Return Value : dummySeat");
            return dummySeat;
        }
        public static Seat GetSeatByPlayerName(List<Seat> seatList, string playerName)
        {
            Seat dummySeat = new Seat();
            foreach (var seat in seatList)
            {
                if (seat.username == playerName)
                {
                    return seat;
                }
            }
            Debug.LogError("No Seat Found For PlayerName " + playerName + " GetSeatByPlayerName Return Value : dummySeat");
            return dummySeat;
        }
        public static List<Seat> UpdateSeatVarTotalBetInSubTurn(List<Seat> seatList, Seat seat, float moveAmount)
        {
            Seat dummySeat = new Seat();
            dummySeat = ISeat.TransferSeatData(dummySeat, seat);
            dummySeat.totalBetInSubTurn = seat.totalBetInSubTurn + moveAmount;
            seatList = UpdateSeatByPlayerName(dummySeat,seatList);
            return seatList;
        }
        public static List<Seat> UpdateSeatVarMinusBalance(List<Seat> seatList, Seat seat, float moveAmount)
        {
            Seat dummySeat = new Seat();
            dummySeat = ISeat.TransferSeatData(dummySeat, seat);
            dummySeat.balance = seat.balance - moveAmount;
            seatList = UpdateSeatByPlayerName(dummySeat,seatList);
            return seatList;
        }
        public static List<Seat> UpdateSeatVarAddBalance(List<Seat> seatList, Seat seat, float moveAmount)
        {
            Seat dummySeat = new Seat();
            dummySeat = ISeat.TransferSeatData(dummySeat, seat);
            dummySeat.balance = seat.balance + moveAmount;
            seatList = UpdateSeatByPlayerName(dummySeat,seatList);
            return seatList;
        }
        private static List<Seat> UpdateSeatVarIsPlayerInGame(List<Seat> seatList, Seat seat, Move move)
        {
            Seat dummySeat = new Seat();
            dummySeat = ISeat.TransferSeatData(dummySeat, seat);
            dummySeat.isPlayerInGame = move != Move.Fold;
            seatList = UpdateSeatByPlayerName(dummySeat,seatList);
            return seatList;
        }
        public static List<Seat> SetSeatVarLastMoveAmount(List<Seat> seatList, Seat seat, float moveAmount)
        {
            Seat dummySeat = new Seat();
            dummySeat = ISeat.TransferSeatData(dummySeat, seat);
            dummySeat.lastMoveAmount = moveAmount;
            seatList = UpdateSeatByPlayerName(dummySeat,seatList);
            return seatList;
        }
        public static List<Seat> SetSeatVarIsPlayerInGame(List<Seat> seatList, Seat seat, bool condition)
        {
            Seat dummySeat = new Seat();
            dummySeat = ISeat.TransferSeatData(dummySeat, seat);
            dummySeat.isPlayerInGame = condition;
            seatList = UpdateSeatByPlayerName(dummySeat,seatList);
            return seatList;
        }
        public static List<Seat> SetSeatVarIsPlayerMovedInSubTurn(List<Seat> seatList, Seat seat, bool isPlayerMovedInSubTurn)
        {
            Seat dummySeat = new Seat();
            dummySeat = ISeat.TransferSeatData(dummySeat, seat);
            dummySeat.isPlayerMovedInSubTurn = isPlayerMovedInSubTurn;
            seatList = UpdateSeatByPlayerName(dummySeat,seatList);
            return seatList;
        }
        public static List<Seat> SetSeatVarISMyTurn(List<Seat> seatList, Seat seat, bool isMyTurn, TurnManager turnManager)
        {
            Seat dummySeat = new Seat();
            dummySeat = ISeat.TransferSeatData(dummySeat, seat);
            dummySeat.isMyTurn = isMyTurn;
            if (isMyTurn)
            {
                turnManager.SetCurrentHasTurnGOData(dummySeat);
            }
            seatList = ISeatList.UpdateSeatByPlayerName(dummySeat,seatList);
            return seatList;
        }
        public static List<Seat> SetSeatVarIsDealer(List<Seat> seatList, Seat seat, bool isDealer,TurnManager turnManager)
        {
            Seat dummySeat = new Seat();
            dummySeat = ISeat.TransferSeatData(dummySeat, seat);
            dummySeat.isDealer = isDealer;
            if (isDealer)
            {
                turnManager.SetCurrentDealerGOData(dummySeat);
            }
            seatList = UpdateSeatBySeatLocation(dummySeat,seatList);
            return seatList;
        }
        public static List<Seat> SetSeatVarLastMove(List<Seat> seatList, Seat seat, Move move)
        {
            Seat dummySeat = new Seat();
            dummySeat = ISeat.TransferSeatData(dummySeat, seat);
            dummySeat.lastMove = move;
            seatList = UpdateSeatByPlayerName(dummySeat,seatList);
            return seatList;
        }
        public static List<Seat> SetSeatVarMoveTime(List<Seat> seatList, Seat seat, float moveTime)
        {
            Seat dummySeat = new Seat();
            dummySeat = ISeat.TransferSeatData(dummySeat, seat);
            dummySeat.moveTime = moveTime;
            seatList = UpdateSeatByPlayerName(dummySeat,seatList);
            return seatList;
        }
        public static List<Seat> UpdateSeatByPlayerName(Seat newSeat, List<Seat> seatList)
        {
            List<Seat> dummyList = new List<Seat>();
            foreach (var seat in seatList)
            {
                if (!string.Equals(seat.username, newSeat.username))
                {
                    dummyList.Add(seat);
                }
            }
            dummyList.Add(newSeat);
            if(dummyList.Count != seatList.Count)
                Debug.LogError("UpdateSeatByPlayerName Error : dummyList.Count : "+dummyList.Count+" !="+seatList.Count+": seatList.Count");
            return dummyList;
        }
        public static List<Seat> UpdateSeatBySeatLocation(Seat newSeat, List<Seat> seatList)
        {
            List<Seat> dummyList = new List<Seat>();
            foreach (var seat in seatList)
            {
                if (seat.location != newSeat.location)
                {
                    dummyList.Add(seat);
                }
            }
            dummyList.Add(newSeat);
            if(dummyList.Count != seatList.Count)
                Debug.LogError("UpdateSeatBySeatLocation Error : dummyList.Count : "+dummyList.Count+" !="+seatList.Count+": seatList.Count");
            return dummyList;
        }
        public static int ActiveSeatLocationsFindNextActiveLocation(int currentLocation, int[] activeSeatLocations)
        {
            int nextActiveLocation = 0;
            if (isCurrentLocationInActiveSeatLocations(activeSeatLocations, currentLocation))
            {
                for (int i = 0; i < activeSeatLocations.Length; i++)
                {
                    if (activeSeatLocations[i] == currentLocation)
                    {
                        if (i == activeSeatLocations.Length - 1)
                        {
                            nextActiveLocation = activeSeatLocations[0];
                        }
                        else
                        {
                            nextActiveLocation = activeSeatLocations[i + 1];
                        }
                    }
                }
            }else
            {
                //ActiveSeatLocation listesinde yoksa
                if (activeSeatLocations.Max() < currentLocation)
                {
                    return activeSeatLocations[0];
                }
                for (int i = 0; i < activeSeatLocations.Length; i++)
                {
                    if (activeSeatLocations[i] > currentLocation)
                    {
                        nextActiveLocation = activeSeatLocations[i];
                        return nextActiveLocation;
                    }
                }
            }
            
            return nextActiveLocation;
        }

        private static bool isCurrentLocationInActiveSeatLocations(int[] activeSeatLocations, int currentLocation)
        {
            bool isCurrentLocationInActiveSeatLocations = false;
            for (int i = 0; i < activeSeatLocations.Length; i++)
            {
                if (activeSeatLocations[i] == currentLocation)
                {
                    isCurrentLocationInActiveSeatLocations = true;
                }
            }
            return isCurrentLocationInActiveSeatLocations;
        }

        public static int ActiveSeatLocationsFindMin(int[] activeSeatLocations)
        {
            if (activeSeatLocations != null)
            {
                return activeSeatLocations[0];
            }
            Debug.LogWarning("ActiveSeatLocations is null");
            return 0;
        }
        public static int[] ActiveSeatLocationsreOrderByDealerSeat(int dealerSeat, int[] activeSeatLocationArray)
        {
            int[] dummyArray = new int[activeSeatLocationArray.Length];
            int k = 0;
            for (int i = 0; i < dummyArray.Length; i++)
            {
                if (activeSeatLocationArray[i] > dealerSeat)
                {
                    dummyArray[k] = activeSeatLocationArray[i];
                    k++;
                }
            }

            int eleman = activeSeatLocationArray.Length - k - 1;
            for (int i = 0; i <= eleman; i++)
            {
                if (activeSeatLocationArray[i] <= dealerSeat)
                {
                    dummyArray[k] = activeSeatLocationArray[i];
                    k++;
                }
            }
            return dummyArray;
        }


        public static List<string> GenerateInGamePlayerList(List<Seat> seatList)
        {
            List<string> inGamePlayerList = new List<string>();
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    if (seat.isPlayerInGame)
                    {
                        inGamePlayerList.Add(seat.username);
                    }
                }
            }
            return inGamePlayerList;
        }


        public static List<Seat> ResetForNewTurnStage(List<Seat> seatList)
        {
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    if (seat.isPlayerInGame)
                    {
                        seatList =  SetSeatVarIsPlayerMovedInSubTurn(seatList, seat, false);
                    }
                }
            }

            return seatList;

        }

        public static List<Seat> SetSeatVarTotalBetInSubTurn(List<Seat> seatList, Seat seat, int amount)
        {
            Seat dummySeat = new Seat();
            dummySeat = ISeat.TransferSeatData(dummySeat, seat);
            dummySeat.totalBetInSubTurn = amount;
            seatList = UpdateSeatByPlayerName(dummySeat,seatList);
            return seatList;
        }


        public static List<Seat> ClearAllPlayersSubTurnTotalBets(List<Seat> mySeatList)
        {
            foreach (var seat in mySeatList)
            {
                if (seat.isActive)
                {
                    if (seat.isPlayerInGame)
                    {
                        mySeatList = SetSeatVarTotalBetInSubTurn(mySeatList, seat, 0);
                    }
                }
            }
            return mySeatList;
        }

        public static float GetOpponenetsMaxTotalBetInSubTurn(List<Seat> seatList, string myName)
        {
            float maxTotalBet = 0;
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    if (seat.isPlayerInGame)
                    {
                        if (!string.Equals(seat.username, myName))
                        {
                            if (seat.totalBetInSubTurn > maxTotalBet)
                            {
                                maxTotalBet = seat.totalBetInSubTurn;
                            }
                        }
                    }
                }
            }
            return maxTotalBet;
        }

        public static bool CheckAllInPlayerHasTurnByIsMyTurn(List<Seat> seatList)
        {
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    if (seat.isPlayerInGame)
                    {
                        if (seat.isMyTurn)
                        {
                            if (seat.lastMove == Move.AllIn)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        // inGame playerlar arasında all'in dememiş birden fazla player var ise true döner
        public static bool HasEnoughPlayersForTurn(List<Seat> mySeatList)
        {
            int playerCount = 0;
            foreach (var seat in mySeatList)
            {
                if (seat.isActive)
                {
                    if (seat.isPlayerInGame)
                    {
                        if (seat.lastMove != Move.AllIn)
                        {
                            playerCount++;
                        }
                    }
                }
            }
            if (playerCount > 1)
            {
                return true;
            }
            return false;
        }

        //Fold dememiş playerların listesini verir
        public static List<string> GetInGamePlayerList(List<Seat> seatList)
        {
            List<string> inGamePlayerList = new List<string>();
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    if (seat.isPlayerInGame)
                    {
                        if (seat.lastMove != Move.Fold)
                        {
                            inGamePlayerList.Add(seat.username);
                        }
                    }
                }
            }
            return inGamePlayerList;
        }

        public static string GetLastPlayer(List<Seat> mySeatList)
        {
            int notFoldPlayerCount = 0;
            string lastPlayer = "";
            foreach (var seat in mySeatList)
            {
                if (seat.isActive)
                {
                    if (seat.isPlayerInGame)
                    {
                        if (seat.lastMove != Move.Fold)
                        {
                            notFoldPlayerCount++;
                            lastPlayer = seat.username;
                        }
                    }
                }
            }
            if (notFoldPlayerCount == 1)
            {
                return lastPlayer;
            }
            Debug.LogWarning("GetLastPlayer() metodu hatalı çalıştı");
            return "";

        }
        public static string GetLastPlayerIncludeAllin(List<Seat> mySeatList)
        {
            int notFoldNotAllinPlayerCount = 0;
            string lastPlayer = "";
            foreach (var seat in mySeatList)
            {
                if (seat.isActive)
                {
                    if (seat.isPlayerInGame)
                    {
                        if (seat.lastMove != Move.Fold && seat.lastMove != Move.AllIn)
                        {
                            notFoldNotAllinPlayerCount++;
                            lastPlayer = seat.username;
                        }
                    }
                }
            }
            if (notFoldNotAllinPlayerCount == 1)
            {
                return lastPlayer;
            }
            Debug.LogWarning("GetLastPlayerIncludeAllin() metodu hatalı çalıştı");
            return "";

        }

        public static List<Seat> SetSeatVarIsMyTurnJustForWinner(List<Seat> mySeatList, string winnerName)
        {
            foreach (var seat in mySeatList)
            {
                if (seat.isActive)
                {
                    if (string.Equals(seat.username, winnerName))
                    {
                        Seat dummySeat = new Seat();
                        dummySeat = ISeat.TransferSeatData(dummySeat, seat);
                        dummySeat.isMyTurn = true;
                        mySeatList = UpdateSeatByPlayerName(dummySeat, mySeatList);
                    }
                    else
                    {
                        Seat dummySeat = new Seat();
                        dummySeat = ISeat.TransferSeatData(dummySeat, seat);
                        dummySeat.isMyTurn = false;
                        mySeatList = UpdateSeatByPlayerName(dummySeat, mySeatList);
                    }
                }
            }
            return mySeatList;
        }

        public static List<Seat> GiveTurnToNoBody(List<Seat> mySeatList)
        {
            foreach (var seat in mySeatList)
            {
                if (seat.isActive)
                {
                    if (seat.isPlayerInGame)
                    {
                        Seat dummySeat = new Seat();
                        dummySeat = ISeat.TransferSeatData(dummySeat, seat);
                        dummySeat.isMyTurn = false;
                        mySeatList = UpdateSeatByPlayerName(dummySeat, mySeatList);
                    }
                }
            }
            return mySeatList;
        }

        public static bool CheckPlayerIsWinnerByPlayerName(List<Seat> seatList, string playerName)
        {
            int inGamePlayerCount = GetInGamePlayerList(seatList).Count;
            string lastPlayer = GetLastPlayer(seatList);
            if (inGamePlayerCount == 1)
            {
                if (string.Equals(lastPlayer, playerName))
                {
                    return true;
                }
            }
            return false;
        }

        public static List<Seat> ResetLastMoves(List<Seat> mySeatList)
        {
            foreach (var seat in mySeatList)
            {
                if (seat.isActive)
                {
                    if (seat.isPlayerInGame)
                    {
                        Seat dummySeat = new Seat();
                        dummySeat = ISeat.TransferSeatData(dummySeat, seat);
                        dummySeat.lastMove = Move.None;
                        mySeatList = UpdateSeatByPlayerName(dummySeat, mySeatList);
                    }
                }
            }
            return mySeatList;
        }

        public static List<Seat> UpdateSeatVarAddBalanceByPlayerName(List<Seat> seatList, string playerName, float amount)
        {
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    if (string.Equals(seat.username, playerName))
                    {
                        Seat dummySeat = new Seat();
                        dummySeat = ISeat.TransferSeatData(dummySeat, seat);
                        dummySeat.balance += amount;
                        seatList = UpdateSeatByPlayerName(dummySeat, seatList);
                    }
                }
            }
            return seatList;
        }
        public static List<Seat> UpdateSeatVarMinusBalanceByPlayerName(List<Seat> seatList, string playerName, float amount)
        {
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    if (string.Equals(seat.username, playerName))
                    {
                        Seat dummySeat = new Seat();
                        dummySeat = ISeat.TransferSeatData(dummySeat, seat);
                        dummySeat.balance -= amount;
                        seatList = UpdateSeatByPlayerName(dummySeat, seatList);
                    }
                }
            }
            return seatList;
        }

        public static List<Seat> SetSeatVarIsPlayerInGameByBalance(List<Seat> mySeatList)
        {
            foreach (var seat in mySeatList)
            {
                if (seat.isActive)
                {
                    if (seat.balance <= 0)
                    {
                        Seat dummySeat = new Seat();
                        dummySeat = ISeat.TransferSeatData(dummySeat, seat);
                        dummySeat.isPlayerInGame = false;
                        mySeatList = UpdateSeatByPlayerName(dummySeat, mySeatList);
                    }
                }
            }
            return mySeatList;
        }

        public static SeatLocations GetSeatLocationByPlayerName(List<Seat> mySeatList, string playerName)
        {
            foreach (var seat in mySeatList)
            {
                if (seat.isActive)
                {
                    if (string.Equals(seat.username, playerName))
                    {
                        return seat.location;
                    }
                }
            }
            return SeatLocations.None;
        }

        public static List<Seat> ClearSeatFromSeatListByPlayerName(List<Seat> oldSeatList, string playerName)
        {
            foreach (var seat in oldSeatList)
            {
                if (string.Equals(seat.username, playerName))
                {
                    Seat dummySeat = new Seat();
                    dummySeat =  ISeat.CreateNewSeatByLocation(seat.location);
                    oldSeatList = UpdateSeatBySeatLocation(dummySeat, oldSeatList);
                }
            }
            return oldSeatList;
            
            /*
            Seat removedSeat = new Seat();
            foreach (var seat in oldSeatList)
            {
                if (seat.isActive)
                {
                    if (string.Equals(seat.username, playerName))
                    {
                        removedSeat = seat;
                    }
                }
            }
            Seat newSeat = new Seat();
            newSeat = ISeat.TransferSeatLocation(newSeat,removedSeat);
            List<Seat> newSeatList = new List<Seat>();
            
            foreach (var seat in oldSeatList)
            {
                if (!string.Equals(seat.username, playerName))
                {
                    newSeatList.Add(seat);
                }
            }
            newSeatList.Add(newSeat);
            return newSeatList;
            */
        }

        public static List<Seat> ClearMoveTime(List<Seat> seatList)
        {
            foreach (var seat in seatList)
            {
                if (seat.moveTime != 0)
                {
                    Seat dummySeat = new Seat();
                    dummySeat = ISeat.TransferSeatData(dummySeat, seat);
                    dummySeat.moveTime = 0;
                    seatList = UpdateSeatByPlayerName(dummySeat, seatList);
                }
            }
            return seatList;
        }

        public static List<Seat> ClearLastMove(List<Seat> seatList)
        {
            foreach (var seat in seatList)
            {
                if (seat.lastMove != Move.None)
                {
                    Seat dummySeat = new Seat();
                    dummySeat = ISeat.TransferSeatData(dummySeat, seat);
                    dummySeat.lastMove = Move.None;
                    seatList = UpdateSeatByPlayerName(dummySeat, seatList);
                }
            }
            return seatList;
        }

        public static List<Seat> ClearLastMoveAmount(List<Seat> seatList)
        {
            foreach (var seat in seatList)
            {
                if (seat.lastMoveAmount != 0)
                {
                    Seat dummySeat = new Seat();
                    dummySeat = ISeat.TransferSeatData(dummySeat, seat);
                    dummySeat.lastMoveAmount = 0;
                    seatList = UpdateSeatByPlayerName(dummySeat, seatList);
                }
            }
            return seatList;
        }

        public static List<Seat> ClearTotalBetInSubTurn(List<Seat> seatList)
        {
            foreach (var seat in seatList)
            {
                if (seat.totalBetInSubTurn != 0)
                {
                    Seat dummySeat = new Seat();
                    dummySeat = ISeat.TransferSeatData(dummySeat, seat);
                    dummySeat.totalBetInSubTurn = 0;
                    seatList = UpdateSeatByPlayerName(dummySeat, seatList);
                }
            }
            return seatList;
        }

        public static List<Seat> ClearIsPlayerMovedInSubTurn(List<Seat> seatList)
        {
            foreach (var seat in seatList)
            {
                if (seat.isPlayerMovedInSubTurn)
                {
                    Seat dummySeat = new Seat();
                    dummySeat = ISeat.TransferSeatData(dummySeat, seat);
                    dummySeat.isPlayerMovedInSubTurn = false;
                    seatList = UpdateSeatByPlayerName(dummySeat, seatList);
                }
            }
            return seatList;
        }

        public static List<Seat> ClearIsMyTurn(List<Seat> seatList)
        {
            foreach (var seat in seatList)
            {
                if (seat.isMyTurn)
                {
                    Seat dummySeat = new Seat();
                    dummySeat = ISeat.TransferSeatData(dummySeat, seat);
                    dummySeat.isMyTurn = false;
                    seatList = UpdateSeatByPlayerName(dummySeat, seatList);
                }
            }
            return seatList;
        }

        public static List<Seat> SetSeatVarIsPlayerInGameByGamePlayerList(List<Seat> seatList, List<string> inGamePlayerList)
        {
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    if (inGamePlayerList.Contains(seat.username))
                    {
                        Seat dummySeat = new Seat();
                        dummySeat = ISeat.TransferSeatData(dummySeat, seat);
                        dummySeat.isPlayerInGame = true;
                        seatList = UpdateSeatByPlayerName(dummySeat, seatList);
                    }
                }
            }
            return seatList;
        }

        public static List<Seat> ClearIsPlayerInGame(List<Seat> seatList)
        {
            foreach (var seat in seatList)
            {
                if (seat.isPlayerInGame)
                {
                    Seat dummySeat = new Seat();
                    dummySeat = ISeat.TransferSeatData(dummySeat, seat);
                    dummySeat.isPlayerInGame = false;
                    seatList = UpdateSeatByPlayerName(dummySeat, seatList);
                }
            }
            return seatList;
        }

        public static bool CheckFoldedPlayerHasTurnByIsMyTurn(List<Seat> seatList)
        {
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    if (seat.isMyTurn)
                    {
                        if (seat.lastMove == Move.Fold)
                        {
                            return true;
                        }
                    }
                    
                }
            }
            return false;
        }

        public static string GetPlayerNameWhoHasMaxTotalBetInSubTurn(List<Seat> mySeatList)
        {
            float maxTotalBetInSubTurn = 0;
            string playerName = "";
            foreach (var seat in mySeatList)
            {
                if (seat.isActive)
                {
                    if (seat.totalBetInSubTurn > maxTotalBetInSubTurn)
                    {
                        maxTotalBetInSubTurn = seat.totalBetInSubTurn;
                        playerName = seat.username;
                    }
                }
            }
            return playerName;
        }

        public static List<string> GetPlayerNameListWithZeroBalance(List<Seat> mySeatList)
        {
            List<string> playerNameList = new List<string>();
            foreach (var seat in mySeatList)
            {
                if (seat.isActive)
                {
                    if (seat.balance == 0)
                    {
                        playerNameList.Add(seat.username);
                    }
                }
            }
            return playerNameList;
        }

        public static bool CheckIsPlayerInGame(List<Seat> seatList, string playerName)
        {
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    if (string.Equals(seat.username, playerName))
                    {
                        if (seat.isPlayerInGame)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static SeatLocations ActiveSeatLocationsFindPreviousActiveLocation(int currentDealerlocation, int[] activeSeatLocations)
        {
            int previousActiveLocation = 0;
            if (isCurrentLocationInActiveSeatLocations( activeSeatLocations,currentDealerlocation))
            {
                for (int i = 0; i < activeSeatLocations.Length; i++)
                {
                    if (activeSeatLocations[i] == currentDealerlocation)
                    {
                        if (i == 0)
                        {
                            previousActiveLocation = activeSeatLocations[activeSeatLocations.Length - 1];
                        }
                        else
                        {
                            previousActiveLocation = activeSeatLocations[i - 1];
                        }
                    }
                }
            }
            else
            {
                if (activeSeatLocations.Max() < currentDealerlocation)
                {
                    previousActiveLocation = activeSeatLocations.Max();
                    return (SeatLocations)previousActiveLocation;
                }
                if (activeSeatLocations.Min() > currentDealerlocation)
                {
                    previousActiveLocation = activeSeatLocations.Max();
                    return (SeatLocations)previousActiveLocation;
                }
                for (int i = 0; i < activeSeatLocations.Length; i++)
                {
                    if (activeSeatLocations[i] > currentDealerlocation)
                    {
                        previousActiveLocation = activeSeatLocations[i -1];
                        return (SeatLocations)previousActiveLocation;
                    }
                }
            }
            
            return (SeatLocations)previousActiveLocation;
        }

        public static bool GetSeatVarIsMyTurnByPlayerName(string playerName, List<Seat> mySeatList)
        {
            foreach (var seat in mySeatList)
            {
                if (seat.isActive)
                {
                    if (string.Equals(seat.username, playerName))
                    {
                        return seat.isMyTurn;
                    }
                }
            }
            return false;
        }

        public static List<string> GeneratePlayerListWhoHasNotFoldedOrAllin(List<Seat> seatList)
        {
            List<string> playerList = new List<string>();
            foreach (var seat in seatList)
            {
                if (seat.isActive)
                {
                    if (seat.isPlayerInGame)
                    {
                        if (seat.lastMove != Move.Fold && seat.lastMove != Move.AllIn)
                        {
                            playerList.Add(seat.username);
                        }
                    }
                }
            }
            return playerList;
        }

        public static List<Seat> GenerateDeepCopy(List<Seat> seatList)
        {
            List<Seat> dummyList = new List<Seat>();
            foreach (var seat in seatList)
            {
                Seat dummySeat = new Seat();
                dummySeat = ISeat.TransferSeatData(dummySeat, seat);
                dummyList.Add(dummySeat);
            }
            return dummyList;
        }
    }
}