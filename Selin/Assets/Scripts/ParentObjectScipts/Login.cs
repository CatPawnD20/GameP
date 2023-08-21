
namespace Assets.Scripts
{
    /*
 * Login nesnesi için temel sınıftır. Parent object'i extend eder. Bu sayede ID değişkeni olması sağlanır.
 * 2 constructor'u bulunur
 * Tuttuğu değişkenler :
 * - id         --> from parentObject
 * - loginUsername
 */
    public class Login : ParentObject
    {
        private string loginUsername;
        
        
        public Login()
        {
            
        }
        public Login(string loginUsername)
        {
            this.loginUsername = loginUsername;
        }

        
        public string LoginUsername
        {
            get => loginUsername;
            set => loginUsername = value;
        }

    
    }
}