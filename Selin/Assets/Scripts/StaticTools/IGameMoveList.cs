using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public static class IGameMoveList 
    {
        public static List<GameMove> RemoveSmallAndBigBlind(List<GameMove> gameMoveList)
        {
            List<GameMove> gameMoveList1 = new List<GameMove>();
            foreach (GameMove gameMove in gameMoveList)
            {
                if (gameMove.Move != Move.SmallBlind && gameMove.Move != Move.BigBlind)
                {
                    gameMoveList1.Add(gameMove);
                }
            }

            if (gameMoveList1.Count == 0)
            {
                return gameMoveList;
            }
            return gameMoveList1;
        }
        
        public static List<GameMove> RemoveGameMovesOfPlayersByMove(List<GameMove> gameMoveList, Move move)
        {
            
            List<string> playerNames = new List<string>();
            
            playerNames.AddRange(GeneratePlayerListByMove(gameMoveList, move));
            if (playerNames.Count == 0)
            {
                return gameMoveList;
            }
            foreach (var playerName in playerNames)
            {
                gameMoveList = RemoveGameMovesOfPlayersByPlayerNames(gameMoveList,playerName);
            }
            return gameMoveList;
        }
        
        public static List<string> GeneratePlayerListByMove(List<GameMove> gameMoveList, Move move)
        {
            List<string> playerNames =
                gameMoveList.Where(x => x.move == move).Select(x => x.PlayerName).Distinct().ToList();
            return playerNames;
        }
        
        public static float CalculateMaxTotalBetAmongPlayersByMove(List<GameMove> gameMoveList, Move move)
        {
            float maxTotalBet = 0;
            List<string> playerNames = GeneratePlayerListByMove(gameMoveList, move);
            foreach (var playerName in playerNames)
            {
                float totalBet = 0;
                foreach (var gameMove in gameMoveList)
                {
                    if (gameMove.PlayerName == playerName)
                    {
                        totalBet += gameMove.Amount;
                    }
                }
                if (totalBet > maxTotalBet)
                {
                    maxTotalBet = totalBet;
                }
            }
            return maxTotalBet;
        }
        
        public static bool HasEveryPlayerEnoughBet(List<GameMove> gameMoveList, float minBet)
        {
            if (gameMoveList.Count == 0)
            {
                return true;
            }
            List<string> playerNames = GeneratePlayerList(gameMoveList);
            foreach (var playerName in playerNames)
            {
                float totalBet = 0;
                foreach (var gameMove in gameMoveList)
                {
                    if (gameMove.PlayerName == playerName)
                    {
                        totalBet += gameMove.Amount;
                    }
                }
                if (totalBet >= minBet)
                {
                    return true;
                }
            }
            return false;
        }

        public static List<string> GeneratePlayerList(List<GameMove> gameMoveList)
        {
            List<string> playerNames = gameMoveList.Select(x => x.PlayerName).Distinct().ToList();
            return playerNames;
        }
        
        public static bool HasEveryPlayerMoved(List<GameMove> subMoveList,
            List<string> playerListWhoHasNotFoldedOrAllin)
        {
            List<GameMove> withOutSmallAndBigBlind = RemoveSmallAndBigBlind(subMoveList);
            foreach (var player in playerListWhoHasNotFoldedOrAllin)
            {
                if(!HasPlayerMoved(withOutSmallAndBigBlind, player))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool HasPlayerMoved(List<GameMove> withOutSmallAndBigBlind, string player)
        {         
            foreach (var gameMove in withOutSmallAndBigBlind)
            {
                if (gameMove.PlayerName == player)
                {
                    return true;
                }
            }
            return false;
            
        }

        public static bool HasEveryPlayerMoved2(List<GameMove> moveList, List<GameMove> subMoveList,
            List<string> InGamePlayerList)
        {
            List<GameMove> withOutSmallAndBigBlind = RemoveSmallAndBigBlind(subMoveList);
            List<GameMove> withOutSmallAndBigBlindMoveList = RemoveSmallAndBigBlind(moveList);
            if (subMoveList.Count != withOutSmallAndBigBlind.Count)
            {
                //PreFlop asamasındadır.
                if (HasAllin(withOutSmallAndBigBlind))
                {
                    if (HasFold(withOutSmallAndBigBlind))
                    {
                        //Allin var Fold var.
                        List<GameMove> withOutFold = RemoveGameMovesOfPlayersByMove(withOutSmallAndBigBlind, Move.Fold);
                        int withOutFoldPlayerCount = GeneratePlayerList(withOutFold).Count;
                        if (withOutFoldPlayerCount == InGamePlayerList.Count)
                        {
                            //Herkesin en az birer hamlesi var.
                            List<GameMove> withOutAllin = RemoveGameMovesOfPlayersByMove(withOutSmallAndBigBlind, Move.AllIn);
                            List<GameMove> withOutFoldAndAllin = RemoveGameMovesOfPlayersByMove(withOutAllin, Move.Fold);
                            List<string> withOutFoldAndAllinPlayerList = GeneratePlayerList(withOutFoldAndAllin);
                            List<int> MoveCountPerPlayer = new List<int>();
                            foreach (var inGamePlayer in withOutFoldAndAllinPlayerList)
                            {
                                int moveCount = withOutFoldAndAllin.Count(x => x.PlayerName == inGamePlayer);
                                MoveCountPerPlayer.Add(moveCount);
                            }

                            return true;
                        }
                        else
                        {
                            //Herkesin en az birer hamlesi yok.
                            return false;
                        }

                    }else
                    {
                        //Allin var Fold yok.
                        List<string> subMoveListPlayerList = GeneratePlayerList(withOutSmallAndBigBlind);
                        if (subMoveListPlayerList.Count == InGamePlayerList.Count)
                        {
                            //Herkesin en az birer hamlesi var.
                            List<GameMove> withOutAllin = RemoveGameMovesOfPlayersByMove(withOutSmallAndBigBlind, Move.AllIn);
                            List<string> withOutAllinPlayerList = GeneratePlayerList(withOutAllin);
                            List<int> MoveCountPerPlayer = new List<int>();
            
                            foreach (var inGamePlayer in withOutAllinPlayerList)
                            {
                                int moveCount = withOutAllin.Count(x => x.PlayerName == inGamePlayer);
                                MoveCountPerPlayer.Add(moveCount);
                            }
                            return true;
                        }
                        else
                        {
                            //Herkesin en az birer hamlesi yok.
                            return false;
                        }
                        
                    }
                        
                }else
                {
                    if (HasFold(withOutSmallAndBigBlind))
                    {
                        //Allin yok Fold var.
                        List<GameMove> withOutFold = RemoveGameMovesOfPlayersByMove(withOutSmallAndBigBlind, Move.Fold);
                        List<string> withOutFoldPlayerList = GeneratePlayerList(withOutFold);
                        if (withOutFoldPlayerList.Count == InGamePlayerList.Count)
                        {
                            //Herkesin en az birer hamlesi var.
                            List<int> MoveCountPerPlayer = new List<int>();
            
                            foreach (var inGamePlayer in InGamePlayerList)
                            {
                                int moveCount = withOutFold.Count(x => x.PlayerName == inGamePlayer);
                                MoveCountPerPlayer.Add(moveCount);
                            }

                            return true;
                        }
                        else
                        {
                            //Herkesin en az birer hamlesi yok.
                            return false;
                        }
                        
                    }else
                    {
                        //Allin yok Fold yok.
                        List<string> subMoveListPlayerList = GeneratePlayerList(withOutSmallAndBigBlind);
                        if (subMoveListPlayerList.Count == InGamePlayerList.Count)
                        {
                            //Herkesin en az birer hamlesi var.
                            List<int> MoveCountPerPlayer = new List<int>();
            
                            foreach (var inGamePlayer in InGamePlayerList)
                            {
                                int moveCount = withOutSmallAndBigBlind.Count(x => x.PlayerName == inGamePlayer);
                                MoveCountPerPlayer.Add(moveCount);
                            }

                            return true;
                        }
                        else
                        {
                            //Herkesin en az birer hamlesi yok.
                            return false;
                        }
                        
                    }
                }
            }
            else
            {
                //Flop, Turn, River asamasındadır.
                if (HasAllin(subMoveList))
                {
                    if (HasFold(subMoveList))
                    {
                        //Allin var Fold var.
                        List<GameMove> withOutFold = RemoveGameMovesOfPlayersByMove(subMoveList, Move.Fold);
                        int withOutFoldPlayerCount = GeneratePlayerList(withOutFold).Count;
                        if (withOutFoldPlayerCount == InGamePlayerList.Count)
                        {
                            //Herkesin en az birer hamlesi var.
                            List<GameMove> withOutAllin = RemoveGameMovesOfPlayersByMove(subMoveList, Move.AllIn);
                            List<GameMove> withOutFoldAndAllin = RemoveGameMovesOfPlayersByMove(withOutAllin, Move.Fold);
                            List<string> withOutFoldAndAllinPlayerList = GeneratePlayerList(withOutFoldAndAllin);
                            List<int> MoveCountPerPlayer = new List<int>();
                            foreach (var inGamePlayer in withOutFoldAndAllinPlayerList)
                            {
                                int moveCount = withOutFoldAndAllin.Count(x => x.PlayerName == inGamePlayer);
                                MoveCountPerPlayer.Add(moveCount);
                            }
                            return true;
                        }
                        else
                        {
                            //Herkesin en az birer hamlesi yok.
                            return false;
                        }
                        
                        
                            
                    }else
                    {
                        //Allin var Fold yok.
                        List<string> subMoveListPlayerList = GeneratePlayerList(subMoveList);
                        if (subMoveListPlayerList.Count == InGamePlayerList.Count)
                        {
                            //Herkesin en az birer hamlesi var.
                            List<GameMove> withOutAllin = RemoveGameMovesOfPlayersByMove(subMoveList, Move.AllIn);
                            List<string> withOutAllinPlayerList = GeneratePlayerList(withOutAllin);
                            List<int> MoveCountPerPlayer = new List<int>();
            
                            foreach (var inGamePlayer in withOutAllinPlayerList)
                            {
                                int moveCount = withOutAllin.Count(x => x.PlayerName == inGamePlayer);
                                MoveCountPerPlayer.Add(moveCount);
                            }
                            return true;
                        }
                        else
                        {
                            //Herkesin en az birer hamlesi yok.
                            return false;
                        }
                        
                    }
                        
                }else
                {
                    if (HasFold(subMoveList))
                    {
                        //Allin yok Fold var.
                        List<GameMove> withOutFold = RemoveGameMovesOfPlayersByMove(subMoveList, Move.Fold);
                        List<string> withOutFoldPlayerList = GeneratePlayerList(withOutFold);
                        if (withOutFoldPlayerList.Count == InGamePlayerList.Count)
                        {
                            //Herkesin en az birer hamlesi var.
                            List<int> MoveCountPerPlayer = new List<int>();
            
                            foreach (var inGamePlayer in InGamePlayerList)
                            {
                                int moveCount = withOutFold.Count(x => x.PlayerName == inGamePlayer);
                                MoveCountPerPlayer.Add(moveCount);
                            }
                            return true;
                        }
                        else
                        {
                            //Herkesin en az birer hamlesi yok.
                            return false;
                        }
                        
                            
                    }else
                    {
                        //Allin yok Fold yok.
                        List<string> subMoveListPlayerList = GeneratePlayerList(subMoveList);
                        if (subMoveListPlayerList.Count == InGamePlayerList.Count)
                        {
                            //Herkesin en az birer hamlesi var.
                            List<int> MoveCountPerPlayer = new List<int>();
            
                            foreach (var inGamePlayer in InGamePlayerList)
                            {
                                int moveCount = subMoveList.Count(x => x.PlayerName == inGamePlayer);
                                MoveCountPerPlayer.Add(moveCount);
                            }

                            return true;
                        }
                        else
                        {
                            //Herkesin en az birer hamlesi yok.
                            return false;
                        }
                        
                    }
                }
            }
        }

        public static bool HasFold(List<GameMove> withOutSmallAndBigBlind)
        {
            foreach (var gameMove in withOutSmallAndBigBlind)
            {
                if (gameMove.move == Move.Fold)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool HasAllin(List<GameMove> withOutSmallAndBigBlind)
        {
            foreach (var gameMove in withOutSmallAndBigBlind)
            {
                if (gameMove.move == Move.AllIn)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool isAllEqual(List<int> moveCountPerPlayer)
        {
            int first = moveCountPerPlayer[0];
            foreach (var moveCount in moveCountPerPlayer)
            {
                if (moveCount != first)
                {
                    return false;
                }
            }
            if (first == 0)
            {
                return false;
            }
            return true;
        }

        public static int GetMoveCountByMove(List<GameMove> gameMoveList, Move move)
        {
            return gameMoveList.Count(x => x.move == move);
        }

        public static int GetPlayerCount(List<GameMove> gameMoveList)
        {
            return gameMoveList.Select(x => x.PlayerName).Distinct().Count();
        }

        public static string WhichPlayerHasBiggerTotalBetByMove(List<GameMove> gameMoveList, Move move)
        {
            List<string> playerNames = GeneratePlayerListByMove(gameMoveList, move);
            float maxTotalBet = 0;
            string playerName = "";
            foreach (var name in playerNames)
            {
                float totalBet = 0;
                foreach (var gameMove in gameMoveList)
                {
                    if (gameMove.PlayerName == name)
                    {
                        totalBet += gameMove.Amount;
                    }
                }
                if (totalBet > maxTotalBet)
                {
                    maxTotalBet = totalBet;
                    playerName = name;
                }
            }
            return playerName;
        }

        public static float GetTotalBetByPlayerName(List<GameMove> gameMoveList, string playerName)
        {
            return gameMoveList.Where(x => x.PlayerName == playerName).Sum(x => x.Amount);
        }

        public static List<GameMove> RemoveGameMovesOfPlayersByPlayerNames(List<GameMove> gameMoveList,string playerNames)
        {
            List<GameMove> removedGameMoves = GenerateGameMoveListByPlayerNames(gameMoveList, playerNames);
            List<GameMove> remainGameMoves = GenerateGameMoveListByRemoveGameMoveList(gameMoveList, removedGameMoves);
            return remainGameMoves;
        }

        public static List<GameMove> GenerateGameMoveListByRemoveGameMoveList(List<GameMove> gameMoveList, List<GameMove> removedGameMoves)
        {
            List<GameMove> remainGameMoves = new List<GameMove>();
            foreach (var gameMove in gameMoveList)
            {
                if (!removedGameMoves.Exists(x => x.MoveSequenceNo == gameMove.MoveSequenceNo))
                {
                    remainGameMoves.Add(gameMove);
                }
            }
            if (removedGameMoves.Count == 0)
            {
                return gameMoveList;
            }
            return remainGameMoves;
        }

        public static List<GameMove> GenerateGameMoveListByPlayerNames(List<GameMove> gameMoveList, string playerNames)
        {
            List<GameMove> gameMoves = new List<GameMove>();
            foreach (var gameMove in gameMoveList)
            {
                if (gameMove.PlayerName == playerNames)
                {
                    gameMoves.Add(gameMove);
                }
            }
            if (gameMoveList.Count == 0)
            {
                return gameMoveList;
            }
            return gameMoves;
        }

        public static float CalculateMaxTotalBet(List<GameMove> gameMoveList)
        {
            float maxTotalBet = 0;
            List<string> playerNames = GeneratePlayerList(gameMoveList);
            foreach (var playerName in playerNames)
            {
                float totalBet = 0;
                foreach (var gameMove in gameMoveList)
                {
                    if (gameMove.PlayerName == playerName)
                    {
                        totalBet += gameMove.Amount;
                    }
                }
                if (totalBet > maxTotalBet)
                {
                    maxTotalBet = totalBet;
                }
            }
            return maxTotalBet;
        }
        
        public static bool isTotalBetsEqual(List<GameMove> subMoveList)
        {
            if (subMoveList.Count == 0)
            {
                return true;
            }
            List<float> totalBets = new List<float>();
            List<string> playerNames = GeneratePlayerList(subMoveList);
            if (playerNames.Count == 1)
            {
                return true;
            }
            foreach (var player in playerNames)
            {
                float totalBet = subMoveList.Where(x => x.PlayerName == player).Sum(x => x.Amount);
                totalBets.Add(totalBet);
            }
            if (totalBets.Distinct().Count() == 1)
            {
                return true;
            }
            return false;
        }


        public static float GetUserRakeBackAmountTotalByPlayerName(List<GameMove> gameMoveList, string playerName)
        {
            float totalRakeAmount = 0;
            foreach (var gameMove in gameMoveList)
            {
                if (gameMove.PlayerName == playerName)
                {
                    totalRakeAmount += gameMove.UserRakeBack;
                }
            }
            return totalRakeAmount;
        }

        public static float GetParentRakeBackAmountTotalByPlayerName(List<GameMove> gameMoveList, string playerName)
        {
            float totalRakeAmount = 0;
            foreach (var gameMove in gameMoveList)
            {
                if (gameMove.PlayerName == playerName)
                {
                    totalRakeAmount += gameMove.ParentRakeBack;
                }
            }
            return totalRakeAmount;
        }

        
        public static List<Tuple<List<string>, float>> GenerateSideBetPossibleOwnersAndAmountsTupleList(
            List<GameMove> subMoveList, List<GameMove> gameMoves)
        {
            List<Tuple<List<string>,float>> sideBetPossibleOwnersAndAmountsTupleList = new List<Tuple<List<string>,float>>();
            
            List<Tuple<List<string>,float>> allinPlayerNameAndTotalPotAmountTupleListGroupByTotalBet = new List<Tuple<List<string>, float>>();
            allinPlayerNameAndTotalPotAmountTupleListGroupByTotalBet = GenerateAllinPlayerNameAndTotalPotAmountTupleListGroupByTotalBet(subMoveList);
            
            float maxAllinTotalBet = 0;
            foreach (var tuple in allinPlayerNameAndTotalPotAmountTupleListGroupByTotalBet)
            {
                if (tuple.Item2 > maxAllinTotalBet)
                {
                    maxAllinTotalBet = tuple.Item2;
                }
            }
            List<Tuple<List<string>,float>> continuePlayerNameAndTotalPotAmountTupleListGroupByTotalBet = new List<Tuple<List<string>, float>>();
            continuePlayerNameAndTotalPotAmountTupleListGroupByTotalBet = GenerateContinuePlayerNameAndTotalPotAmountTupleListGroupByTotalBet(subMoveList,maxAllinTotalBet);
            
            sideBetPossibleOwnersAndAmountsTupleList = MergeAllinAndContinuePlayerNameAndTotalPotAmountTupleListGroupByTotalBet(allinPlayerNameAndTotalPotAmountTupleListGroupByTotalBet,continuePlayerNameAndTotalPotAmountTupleListGroupByTotalBet);
            
            sideBetPossibleOwnersAndAmountsTupleList =  AddFoldedBetsToSideBetPossibleOwnersAndAmountsTupleList(sideBetPossibleOwnersAndAmountsTupleList,subMoveList);
            
            return sideBetPossibleOwnersAndAmountsTupleList;
        }

        private static List<Tuple<List<string>, float>> AddFoldedBetsToSideBetPossibleOwnersAndAmountsTupleList(List<Tuple<List<string>, float>> sideBetPossibleOwnersAndAmountsTupleList, List<GameMove> subMoveList)
        {
            List<Tuple<List<string>, float>> sideBetPossibleOwnersAndAmountsTupleListWithFoldedBets = new List<Tuple<List<string>, float>>();
            List<string> foldedPlayerNames = GeneratePlayerListByMove(subMoveList, Move.Fold);
            
            float foldedTotalBets = 0;
            foreach (var foldedPlayerName in foldedPlayerNames)
            {
                foldedTotalBets = foldedTotalBets + GetTotalBetByPlayerName(subMoveList, foldedPlayerName);
            }
            //add foldedTotalBEts to each tuple
            foreach (var tuple in sideBetPossibleOwnersAndAmountsTupleList)
            {
                float totalPot = tuple.Item2 + foldedTotalBets;
                sideBetPossibleOwnersAndAmountsTupleListWithFoldedBets.Add(new Tuple<List<string>, float>(tuple.Item1,totalPot));
            }
            return sideBetPossibleOwnersAndAmountsTupleListWithFoldedBets;
        }

        private static List<Tuple<List<string>, float>> MergeAllinAndContinuePlayerNameAndTotalPotAmountTupleListGroupByTotalBet(List<Tuple<List<string>, float>> allinPlayerNameAndTotalPotAmountTupleListGroupByTotalBet, List<Tuple<List<string>, float>> continuePlayerNameAndTotalPotAmountTupleListGroupByTotalBet)
        {
            if (continuePlayerNameAndTotalPotAmountTupleListGroupByTotalBet.Count == 0)
            {
                return allinPlayerNameAndTotalPotAmountTupleListGroupByTotalBet;
            }

            if (allinPlayerNameAndTotalPotAmountTupleListGroupByTotalBet.Count == 0)
            {
                return continuePlayerNameAndTotalPotAmountTupleListGroupByTotalBet;
            }
            List<Tuple<List<string>, float>> sideBetPossibleOwnersAndAmountsTupleList = new List<Tuple<List<string>, float>>();
            foreach (var tuple in allinPlayerNameAndTotalPotAmountTupleListGroupByTotalBet)
            {
                sideBetPossibleOwnersAndAmountsTupleList.Add(tuple);
            }
            foreach (var tuple in continuePlayerNameAndTotalPotAmountTupleListGroupByTotalBet)
            {
                sideBetPossibleOwnersAndAmountsTupleList.Add(tuple);
            }
            return sideBetPossibleOwnersAndAmountsTupleList;
        }

        private static List<Tuple<List<string>, float>>GenerateContinuePlayerNameAndTotalPotAmountTupleListGroupByTotalBet(List<GameMove> subMoveList,
                float maxAllinTotalBet)
        {
            List<Tuple<List<string>,float>> sideBetPossibleOwnersAndAmountsTupleList = new List<Tuple<List<string>,float>>();
            List<Tuple<List<string>,float>> continuePlayerNameAndTotalPotAmountTupleListGroupByTotalBet = new List<Tuple<List<string>, float>>();
            
            int sideBetCount = 0;
            List<string> AllinPlayerNames = GeneratePlayerListByMove(subMoveList, Move.AllIn);
            List<string> inGamePlayerNames = GenerateInGamePlayerList(subMoveList);
            
            //carpan olarak kullanilacak sayi
            List<string> continuePlayerNames = GenerateContinuePlayerList(subMoveList);
            
            if (continuePlayerNames.Count == 0)
            {
                Debug.LogWarning("continuePlayerNames.Count == 0");
                return sideBetPossibleOwnersAndAmountsTupleList;
            }
            
            if (continuePlayerNames.Count == 1)
            {
                Debug.LogWarning("continuePlayerNames.Count == 1");
                return sideBetPossibleOwnersAndAmountsTupleList;
            }
            
            List<Tuple<string,float>> continuePlayerNameAndTotalBetTupleList = new List<Tuple<string, float>>();
            float continueBetAmount = 0;
            foreach (var playerName in continuePlayerNames)
            {
                continueBetAmount = GetTotalBetByPlayerName(subMoveList, playerName);
                //continueBetAmount = continueBetAmount -(maxAllinTotalBet / (AllinPlayerNames.Count + continuePlayerNames.Count));
                continuePlayerNameAndTotalBetTupleList.Add(new Tuple<string, float>(playerName,continueBetAmount));
            }
            
            List<Tuple<string,float>> allinPlayerNameAndTotalBetTupleList = new List<Tuple<string, float>>();
            float allinBetAmount = 0;
            foreach (var playerName in AllinPlayerNames)
            {
                allinBetAmount = GetTotalBetByPlayerName(subMoveList, playerName);
                allinPlayerNameAndTotalBetTupleList.Add(new Tuple<string, float>(playerName,allinBetAmount));
            }

            float maxAllinAmount = 0;
            if (allinPlayerNameAndTotalBetTupleList.Count !=  0)
            {
                maxAllinAmount = allinPlayerNameAndTotalBetTupleList.Max(x => x.Item2);
            }
            float maxContinueAmount = 0;
            if (continuePlayerNameAndTotalBetTupleList.Count !=  0)
            {
                maxContinueAmount = continuePlayerNameAndTotalBetTupleList.Max(x => x.Item2);
            }
            
            //neden ????
            if (maxAllinAmount == maxContinueAmount)
            {
                Debug.LogWarning("maxAllinAmount == maxContinueAmount");
                //return sideBetPossibleOwnersAndAmountsTupleList;
            }
            
            //reOrder the totalbets  from small to big **bu turnde allin diyenlerin toplamdaki total betleri
            List<Tuple<string,float>> continuePlayerNameAndTotalBetTupleListOrdered = continuePlayerNameAndTotalBetTupleList.OrderBy(x => x.Item2).ToList();
            List<Tuple<List<string>,float>> continuePlayerNameAndTotalBetTupleListGroupByTotalBet = new List<Tuple<List<string>, float>>();
            List<string> sameTotalBetPlayerNames = new List<string>();
            float totalBetForGroup = 0;
            
            for (int i = 0; i < continuePlayerNameAndTotalBetTupleListOrdered.Count; i++)
            {
                if (i == 0)
                {
                    sameTotalBetPlayerNames.Add(continuePlayerNameAndTotalBetTupleListOrdered[i].Item1);
                    totalBetForGroup = continuePlayerNameAndTotalBetTupleListOrdered[i].Item2;
                }
                else
                {
                    if (continuePlayerNameAndTotalBetTupleListOrdered[i].Item2 == totalBetForGroup)
                    {
                        sameTotalBetPlayerNames.Add(continuePlayerNameAndTotalBetTupleListOrdered[i].Item1);
                    }
                    else
                    {
                        continuePlayerNameAndTotalBetTupleListGroupByTotalBet.Add(new Tuple<List<string>, float>(sameTotalBetPlayerNames,totalBetForGroup));
                        sameTotalBetPlayerNames = new List<string>();
                        sameTotalBetPlayerNames.Add(continuePlayerNameAndTotalBetTupleListOrdered[i].Item1);
                        
                        totalBetForGroup = continuePlayerNameAndTotalBetTupleListOrdered[i].Item2 ;
                    }
                }
            }
            
            continuePlayerNameAndTotalBetTupleListGroupByTotalBet.Add(new Tuple<List<string>, float>(sameTotalBetPlayerNames,totalBetForGroup));
            
            foreach (var tuple in continuePlayerNameAndTotalBetTupleListGroupByTotalBet)
            {
                float totalPotForGroup = 0;
                int groupCount = 0;
                int countBig = 0;
                float totalSmall = 0;
                foreach (var variableTuple in continuePlayerNameAndTotalBetTupleListGroupByTotalBet)
                {
                    if (tuple.Item2 == variableTuple.Item2)
                    {
                        groupCount = variableTuple.Item1.Count;
                    }
                    if (tuple.Item2 < variableTuple.Item2)
                    {
                        countBig = countBig + variableTuple.Item1.Count;
                    }
                    if (tuple.Item2 > variableTuple.Item2)
                    {
                        totalSmall = totalSmall + (variableTuple.Item1.Count * variableTuple.Item2);
                    }
                }

                totalPotForGroup = (tuple.Item2 * (countBig + groupCount) + totalSmall +(maxAllinTotalBet / (AllinPlayerNames.Count + continuePlayerNames.Count)));
                continuePlayerNameAndTotalPotAmountTupleListGroupByTotalBet.Add(new Tuple<List<string>, float>(tuple.Item1,totalPotForGroup));
            }

            return continuePlayerNameAndTotalPotAmountTupleListGroupByTotalBet;
        }

        private static List<Tuple<List<string>, float>> GenerateAllinPlayerNameAndTotalPotAmountTupleListGroupByTotalBet(List<GameMove> subMoveList)
        {
            List<Tuple<List<string>,float>> sideBetPossibleOwnersAndAmountsTupleList = new List<Tuple<List<string>,float>>();
            List<Tuple<List<string>,float>> allinPlayerNameAndTotalPotAmountTupleListGroupByTotalBet = new List<Tuple<List<string>, float>>();
            
            int sideBetCount = 0;
            List<string> AllinPlayerNames = GeneratePlayerListByMove(subMoveList, Move.AllIn);
            List<string> inGamePlayerNames = GenerateInGamePlayerList(subMoveList);
            
            //carpan olarak kullanilacak sayi
            List<string> continuePlayerNames = GenerateContinuePlayerList(subMoveList);
            
            if (AllinPlayerNames.Count == 0)
            {
                Debug.LogWarning("AllinPlayerNames.Count == 0");
                return sideBetPossibleOwnersAndAmountsTupleList;
            }
            List<Tuple<string,float>> allinPlayerNameAndTotalBetTupleList = new List<Tuple<string, float>>();
            
            float betAmount = 0;
            foreach (var playerName in AllinPlayerNames)
            {
                betAmount = GetTotalBetByPlayerName(subMoveList, playerName);
                allinPlayerNameAndTotalBetTupleList.Add(new Tuple<string, float>(playerName,betAmount));
            }

            //Sona 1 player  kalmış ise  Player'i son tier'e ekler #Sebebi oyunun devam edemeyecek durumda olması
            if (continuePlayerNames.Count == 1)
            {
                betAmount = GetTotalBetByPlayerName(subMoveList, continuePlayerNames[0]);
                allinPlayerNameAndTotalBetTupleList.Add(new Tuple<string, float>(continuePlayerNames[0],betAmount));
                continuePlayerNames.Clear();
            }
            
            //reOrder the totalbets  from small to big **bu turnde allin diyenlerin toplamdaki total betleri
            List<Tuple<string,float>> allinPlayerNameAndTotalBetTupleListOrdered = allinPlayerNameAndTotalBetTupleList.OrderBy(x => x.Item2).ToList();
            List<Tuple<List<string>,float>> allinPlayerNameAndTotalBetTupleListGroupByTotalBet = new List<Tuple<List<string>, float>>();
            List<string> sameTotalBetPlayerNames = new List<string>();
            float totalBetForGroup = 0;
            
            for (int i = 0; i < allinPlayerNameAndTotalBetTupleListOrdered.Count; i++)
            {
                if (i == 0)
                {
                    sameTotalBetPlayerNames.Add(allinPlayerNameAndTotalBetTupleListOrdered[i].Item1);
                    totalBetForGroup = allinPlayerNameAndTotalBetTupleListOrdered[i].Item2;
                }
                else
                {
                    if (allinPlayerNameAndTotalBetTupleListOrdered[i].Item2 == totalBetForGroup)
                    {
                        sameTotalBetPlayerNames.Add(allinPlayerNameAndTotalBetTupleListOrdered[i].Item1);
                    }
                    else
                    {
                        allinPlayerNameAndTotalBetTupleListGroupByTotalBet.Add(new Tuple<List<string>, float>(sameTotalBetPlayerNames,totalBetForGroup));
                        sameTotalBetPlayerNames = new List<string>();
                        sameTotalBetPlayerNames.Add(allinPlayerNameAndTotalBetTupleListOrdered[i].Item1);
                        totalBetForGroup = allinPlayerNameAndTotalBetTupleListOrdered[i].Item2;
                    }
                }
            }
            allinPlayerNameAndTotalBetTupleListGroupByTotalBet.Add(new Tuple<List<string>, float>(sameTotalBetPlayerNames,totalBetForGroup));
            
            foreach (var tuple in allinPlayerNameAndTotalBetTupleListGroupByTotalBet)
            {
                float totalPotForGroup = 0;
                int groupCount = 0;
                int countBig = 0;
                float totalSmall = 0;
                foreach (var varıableTuple in allinPlayerNameAndTotalBetTupleListGroupByTotalBet)
                {
                    if (tuple.Item2 == varıableTuple.Item2)
                    {
                        groupCount = varıableTuple.Item1.Count;
                    }
                    if (tuple.Item2 < varıableTuple.Item2)
                    {
                        countBig = countBig + varıableTuple.Item1.Count;
                    }

                    if (tuple.Item2 > varıableTuple.Item2)
                    {
                        totalSmall = totalSmall + (varıableTuple.Item1.Count * varıableTuple.Item2);
                    }
                }
                totalPotForGroup =  (tuple.Item2 * (countBig + groupCount) + totalSmall) + (continuePlayerNames.Count * tuple.Item2);
                allinPlayerNameAndTotalPotAmountTupleListGroupByTotalBet.Add(new Tuple<List<string>, float>(tuple.Item1,totalPotForGroup));
            }

            return allinPlayerNameAndTotalPotAmountTupleListGroupByTotalBet;
        }

        public static List<string> GenerateContinuePlayerList(List<GameMove> moveList)
        {
            List<string> playerNames = GeneratePlayerList(moveList);
            List<string> foldPlayerNames = GeneratePlayerListByMove(moveList, Move.Fold);
            List<string> allInPlayerNames = GeneratePlayerListByMove(moveList, Move.AllIn);
            List<string> continuePlayerNames = playerNames.Except(foldPlayerNames).Except(allInPlayerNames).ToList();
            return continuePlayerNames;
        }

        public static List<string> GenerateInGamePlayerList(List<GameMove> moveList)
        {
            List<string> foldPlayerNames = GeneratePlayerListByMove(moveList, Move.Fold);
            List<string> playerNames = GeneratePlayerList(moveList);
            List<string> inGamePlayerNames = playerNames.Except(foldPlayerNames).ToList();
            return inGamePlayerNames;
        }

        public static List<GameMove> OrderMoveListByMoveSeqNo(List<GameMove> turnMoves)
        {
            List<GameMove> orderedMoveList = new List<GameMove>();
            orderedMoveList = turnMoves.OrderBy(x => x.MoveSequenceNo).ToList();
            return orderedMoveList;
        }
    }
}