using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Logout nesnesi için temel sınıftır. Parent object'i extend eder. Bu sayede ID değişkeni olması sağlanır.
 * 2 constructor'u bulunur
 * Tuttuğu değişkenler :
 * - id         --> from parentObject
 * - loginUsername
 */

namespace Assets.Scripts
{
    public class Logout : ParentObject
    {
        private string logoutUsername;

        public Logout()
        {
            
        }

        public Logout(string logoutUsername)
        {
            this.logoutUsername = logoutUsername;
        }

        public string LogoutUsername
        {
            get => logoutUsername;
            set => logoutUsername = value;
        }
    }
}