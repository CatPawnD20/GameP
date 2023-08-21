using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/*
 * LoginScene için temel sınıftır.
 * Tuttuğu değişkenler
 * - username
 * - password
 * Değişkenleri sayesinde kullanıcnın girdiği bilgileri tutar
 * Fonksiyonları
 * - Login
 * - LoginSucces
 * - LoginFailErrorInvalidUsernameOrPassword
 * - LoginFailErrorUserAlreadyOnline
 */
namespace Assets.Scripts
{
    public class LoginUI : MonoBehaviour
    {
        public static LoginUI instance;
        private string username;
        private string password;

        [SerializeField] private InputField usernameInputField = null;
        [SerializeField] private InputField passwordInputField = null;
        [SerializeField] private Button loginButton = null;
        [SerializeField] private Canvas loginUICanvas = null;
        [SerializeField] private Canvas errorCanvas = null;
        [SerializeField] private TextMeshProUGUI errorMessageText;

        private void Start()
        {
            instance = this;
        }

        //Login Section//
        /*
         * Bu fonksiyon Login buttonu'na tıklanması ile çalışacaktır.
         * 1- buttonu inaktif duruma getirecektir.
         * 2- username ve password bilgilerini encrypt edecek.
         * 3- Sonrasında Player(LocalPlayer)'in Login fonksiyonunu çağırarak Login işlemini başlatır.
         */
        public void Login()
        {
            errorCanvas.enabled = false;
            loginButton.interactable = false;
            
            username = usernameInputField.text;
            password = passwordInputField.text;

            string usernameCypher = Decryptor.encrypt(username);
            Debug.Log("LoginUI.cs -->>> Login -->>> usernameCypher : "+ usernameCypher);
            string passwordCypher = Decryptor.encrypt(password);

            Player.localPlayer.Login(usernameCypher, passwordCypher);
            Debug.Log("LoginUI.cs -->>> Login -->>> Player.localPlayer.isClient : "+ Player.localPlayer.isClient.ToString());
            
        }

        /*
         * Login işlemi succes olduğunda Client'in login ekranında meydana gelen değişikliği sağlar
         * Login işleminin succes olması durumunda Lobby screen Additive şekilde load olur.
         * Bu durum login canvasının yeni scene'de hala var olmasına sebep olur
         * Login canvası enable ederek bu sorunu çözdüm
         * 
         */
        public void loginSucces(string username)
        {
            Player.localPlayer.LocalPlayerName = username;
            loginUICanvas.enabled = false;

        }   
        
        /*
         * Login işlemi fail olduğunda Client'in login ekranında meydana gelen değişikliği sağlar
         */
        public void loginFailErrorInvalidPasswordOrUsername()
        {
            errorCanvas.enabled = true;
            errorMessageText.text = "Invalid Username or Password";
            loginButton.interactable = true;
        }
        
        /*
         * Login işlemi fail olduğunda Client'in login ekranında meydana gelen değişikliği sağlar
         */
        public void loginFailErrorUserAlreadyOnline()
        {
            errorCanvas.enabled = true;
            errorMessageText.text = "User is already Online";
            loginButton.interactable = true;
            
        }
       
        


        
    }
}