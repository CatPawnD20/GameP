using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    /*
     * Player script'in karmaşıklığını önlemek amacı ile password kontrol işlemi için bu class oluşturulmuştur.
     * Bu class sadece password için kontrol işlemi yapmaktadır.
     */
    public static class PasswordChecker 
    {
        /*
         * parametre olarak gelen iki stringi karşılaştırır. Uyuşuyorsa true döndürür.
         */
        public static bool isPasswordCorrect(string parameterPassword, string password)
        {
            if (string.IsNullOrEmpty(parameterPassword) ||  string.IsNullOrEmpty(password))
            {
                if (string.IsNullOrEmpty(parameterPassword) && string.IsNullOrEmpty(password))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (parameterPassword.Equals(password))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            
            
            
        }
    }
}