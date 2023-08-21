using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/*
 * Bu class otomatik olarak gerçekleştirilecek bazı işlemler için tasarlandı
 * 1- Serverin başlatılması
 * 2- Clientin, server'a bağlanması
 */
namespace Assets.Scripts
{
    public class AutoStart : MonoBehaviour
    {
        [SerializeField] private NetworkManagerCustom networkManager = null;
        
        
        void Start()
        {
           Debug.Log("AutoStart.cs -->>> onStart");
        }

        public void StartServer()
        {
            networkManager.StartServer();
            Debug.Log("AutoStart.cs -->>> onStartServer");

        }

        public void ConnectServer()
        {
            networkManager.networkAddress = "localhost";
            networkManager.StartClient();
            Debug.Log("AutoStart.cs -->>> ConnectServer");
        }

        
    }
}