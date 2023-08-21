using UnityEngine;

namespace Assets.Scripts
{
    /*
     * Database'de işlem görebilecek nesneler için oluşturulmuş interface'tir.
     * Processable --> database'de işlenebilir anlamında kullanılmıştır.
     * Database'le ilişik kuracak nesneler ****nesneAdıDB**** şeklinde sınıflara sahiptir
     * 
     */
    public interface IProcessable
    {
        ParentObject getItem(int id);
        ParentObject getItem(string username);
        void deleteItem(int id);
        void updateItem(ParentObject parentObject);
        void putItem(ParentObject parentObject);
        
        int putItemAndReturnId(ParentObject parentObject);
    }
}