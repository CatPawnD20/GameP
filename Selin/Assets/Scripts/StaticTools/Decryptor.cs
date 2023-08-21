using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts
{
    /*
     * Player script'in karmaşıklığını önlemek amacı ile dcrpyt işlemi için bu class oluşturulmuştur.
     * Bu class sadece decrypt işlemi yapmaktadır.
     */
    public static class Decryptor 
    {
        /*
         * Parametre olarak gelen cypher string'i decrypt edip geri döndürecek
         */
        public static string decrypt(string s)
        {
            return s;
        }
        
        /*
         * Parametre olarak gelen string'i encrypt edip geri döndürecek
         */
        public static  string encrypt(string s)
        {
            return s;
        }
    }
    
    
}