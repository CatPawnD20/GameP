using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Mirror;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts
{
    public static class GuidGenerator 
    {
    
        public static void update(NetworkMatch networkMatch, string syncID)
        {
            networkMatch.matchId = syncIDToGuid(syncID);
        }
        public static Guid syncIDToGuid(string syncID)
        {
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider ();
            byte[] inputBytes = Encoding.Default.GetBytes (syncID);
            byte[] hashBytes = provider.ComputeHash (inputBytes);
            return new Guid (hashBytes);
        }
        
        public static Guid GenerateNewNetworkMatchId(string matchID)
        {
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider ();
            byte[] inputBytes = Encoding.Default.GetBytes (matchID);
            byte[] hashBytes = provider.ComputeHash (inputBytes);
            return new Guid (hashBytes);

        }
        public static int GetRandomNumber()
        {
            int randomNumber;
            using (RNGCryptoServiceProvider rngCrypt = new RNGCryptoServiceProvider())
            {
                byte[] tokenBuffer = new byte[4];        // `int32` takes 4 bytes in C#
                rngCrypt.GetBytes(tokenBuffer);
                randomNumber =  BitConverter.ToInt32(tokenBuffer, 0);
            }

            if (randomNumber < 0)
            {
                randomNumber = math.abs(randomNumber);
            }

            return randomNumber;
        }
        
        public static int[] SortArray(int[] arr ) 
        {
            int n = arr.Length;

            for (int i = 0; i < n - 1; i++)
            for (int j = 0; j < n - i - 1; j++)
                if (arr[j] > arr[j + 1])
                {
                    (arr[j], arr[j + 1]) = (arr[j + 1], arr[j]);
                }
            return arr;
        }

        
    }
}