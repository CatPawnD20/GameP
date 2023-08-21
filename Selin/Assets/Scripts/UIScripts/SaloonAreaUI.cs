using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Assets.Scripts
{
    public class SaloonAreaUI : MonoBehaviour
    {
        public static SaloonAreaUI instance;
        
        [SerializeField] private Transform tableUIParent;
        private void Start()
        {
            instance = this;
        }
        

        public Transform TableUIParent
        {
            get => tableUIParent;
            set => tableUIParent = value;
        }
    }
}