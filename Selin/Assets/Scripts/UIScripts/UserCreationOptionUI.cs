using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class UserCreationOptionUI : MonoBehaviour
    {
        public static UserCreationOptionUI instance;
        private string username;
        private string password;
        private float deposit;
        private float rakePercent;
        private string friend;
        private float friendPercent;
        
        private UserTypes userTypes;

        [SerializeField] private TMP_InputField usernameInputField;
        [SerializeField] private TMP_InputField passwordInputField;
        [SerializeField] private TMP_InputField depositInputField;
        [SerializeField] private TMP_InputField rakePercentInputField;
        [SerializeField] private TMP_InputField friendInputField;
        [SerializeField] private TMP_InputField friendPercentInputField;
        [SerializeField] private Button userTypePlayerButton;
        [SerializeField] private Button userTypeOperatorButton;
        [SerializeField] private Button userTypeAdminButton;
        [SerializeField] private Button createNewUserButton;
        [SerializeField] private Button backToMiddleFrameButton;
        [Header("Information Panel")]
        [SerializeField] private GameObject informationArea = null;
        [SerializeField] private TMP_Text informationText = null;
        [SerializeField] private Button informationSuccesBackButton;
        [SerializeField] private Button informationFailBackButton;
        
        private void Start()
        {
            instance = this;
        }
        public void informationFailBack()
        {
            informationFailBackButton.interactable = false;
            informationFailBackButton.GameObject().SetActive(false);
            informationArea.SetActive(false);
            createNewUserButton.interactable = true;
        }
        public void createNewUserFail(string errorMessage)
        {
            informationArea.SetActive(true);
            informationFailBackButton.GameObject().SetActive(true);
            informationFailBackButton.interactable = true;
            informationText.text = errorMessage;
        }

        /*
         * InformationArea'da başarı durumundaki "BACK" buttonuna bağlıdır. 
         * informationAreayı kapatır ve middleFrame'e dönüş yapar 
         */
        public void informationSuccesBack()
        {
            informationSuccesBackButton.interactable = false;
            informationSuccesBackButton.GameObject().SetActive(false);
            informationArea.SetActive(false);
            backToMiddleFrame();
        }
        
        /*
         * İnformation areayı açar ve mesaj gösterir.
         */
        public void createNewUserSucces()
        {
            informationArea.SetActive(true);
            informationSuccesBackButton.GameObject().SetActive(true);
            informationSuccesBackButton.interactable = true;
            informationText.text = "Kullanıcı başarıyla oluşturuldu";
        }
        
        /*
         * UserCreationOptionUI daki "BACK" buttonuna bağlıdır
         */
        public void backToMiddleFrame()
        {
            ControlAreaUI.instance.OpenUserCreationOptionButton.interactable = true;
            backToMiddleFrameButton.interactable = false;
            ControlAreaUI.instance.MiddleFrame.SetActive(true);
            ControlAreaUI.instance.UserCreationOptionFrame.SetActive(false);
        }
        
        /*
         * UserCreationOptionUI'da bulunan "Create" buttonuna bağlıdır.
         * Girilen tüm değerlere null check atar ve değişkenleri buna göre  alınan yada olmassı gereken değerlere set eder
         * Ardından creation işlemini başlatır.
         */
        public void createNewUser()
        {
            createNewUserButton.interactable = false;
            if (string.IsNullOrEmpty(usernameInputField.text) || string.IsNullOrWhiteSpace(usernameInputField.text))
            {
                username = null;
            }
            else
            {
                username = usernameInputField.text;
            }
            if (string.IsNullOrEmpty(passwordInputField.text) || string.IsNullOrWhiteSpace(passwordInputField.text))
            {
                password = null;
            }
            else
            {
                password = passwordInputField.text;
            }
            
            if (string.IsNullOrEmpty(depositInputField.text) || string.IsNullOrWhiteSpace(depositInputField.text))
            {
                deposit = 0;
            }
            else
            {
                deposit = (float) Convert.ToSingle(depositInputField.text);
            }
            if (string.IsNullOrEmpty(rakePercentInputField.text) || string.IsNullOrWhiteSpace(rakePercentInputField.text))
            {
                rakePercent = 20;
            }
            else
            {
                rakePercent = (float) Convert.ToSingle(rakePercentInputField.text);
            }
            
            if (string.IsNullOrEmpty(friendInputField.text) || string.IsNullOrWhiteSpace(friendInputField.text))
            {
                friend = null;
                friendPercent = 0;
            }
            else
            {
                friend = friendInputField.text;
                if (string.IsNullOrEmpty(friendPercentInputField.text) || string.IsNullOrWhiteSpace(friendPercentInputField.text))
                {
                    friendPercent = 10;
                }
                else
                {
                    friendPercent = (float) Convert.ToSingle(friendPercentInputField.text);
                }
            }
            Player.localPlayer.createNewUser(username,password,deposit,rakePercent,friend,friendPercent,UserType);
        }
        
        public void userTypeAdmin()
        {
            userTypeAdminButton.interactable = false;
            userTypeOperatorButton.interactable = true;
            userTypePlayerButton.interactable = true;
            UserType = UserTypes.Admin;
        }
        public void userTypeOperator()
        {
            userTypeAdminButton.interactable = true;
            userTypeOperatorButton.interactable = false;
            userTypePlayerButton.interactable = true;
            UserType = UserTypes.Operator;
        }
        public void userTypePlayer()
        {
            userTypeAdminButton.interactable = true;
            userTypeOperatorButton.interactable = true;
            userTypePlayerButton.interactable = false;
            UserType = UserTypes.Player;
        }

        public GameObject InformationArea
        {
            get => informationArea;
            set => informationArea = value;
        }

        public float RakePercent
        {
            get => rakePercent;
            set => rakePercent = value;
        }

        public float FriendPercent
        {
            get => friendPercent;
            set => friendPercent = value;
        }

        public UserTypes UserType
        {
            get => userTypes;
            set => userTypes = value;
        }

        public string Password
        {
            get => password;
            set => password = value;
        }

        public string Username
        {
            get => username;
            set => username = value;
        }

        public float Deposit
        {
            get => deposit;
            set => deposit = value;
        }

        public string Friend
        {
            get => friend;
            set => friend = value;
        }

        public TMP_InputField UsernameInputField
        {
            get => usernameInputField;
            set => usernameInputField = value;
        }

        public TMP_InputField PasswordInputField
        {
            get => passwordInputField;
            set => passwordInputField = value;
        }

        public TMP_InputField RakePercentInputField
        {
            get => rakePercentInputField;
            set => rakePercentInputField = value;
        }

        public TMP_InputField FriendPercentInputField
        {
            get => friendPercentInputField;
            set => friendPercentInputField = value;
        }

        public TMP_InputField FriendInputField
        {
            get => friendInputField;
            set => friendInputField = value;
        }

        public TMP_InputField DepositInputField
        {
            get => depositInputField;
            set => depositInputField = value;
        }

        public Button UserTypePlayerButton
        {
            get => userTypePlayerButton;
            set => userTypePlayerButton = value;
        }

        public Button UserTypeOperatorButton
        {
            get => userTypeOperatorButton;
            set => userTypeOperatorButton = value;
        }

        public Button UserTypeAdminButton
        {
            get => userTypeAdminButton;
            set => userTypeAdminButton = value;
        }

        public Button CreateNewUserButton
        {
            get => createNewUserButton;
            set => createNewUserButton = value;
        }

        public Button BackToMiddleFrameButton
        {
            get => backToMiddleFrameButton;
            set => backToMiddleFrameButton = value;
        }
    }
}