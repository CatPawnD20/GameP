using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts
{
    /*
     * Database işlemleri için arayüz görevini üstlenir.
     * Kendi içinde ****Object Mapper**** tutar. Ve IProcessable tipinde bir işaretçi tutar.
     * Genel olarak çalışması şu şekildedir.
     * Kendine gelen parametreler yardımı ile IProcessable işaretçisinin tipini belirler ve set eder.
     * Ardından IProcessable objesinde tanımlı olan işlemi başlatır.
     */
    public class DBFacade : MonoBehaviour
    {
        private TableMapper tableMapper = new TableMapper();
        private IProcessable iProcessable;

        void Start()
        {
            this.tableMapper = new TableMapper();
        }

        /*
         * Parametre olarak gelen type ile önce kendi tuttuğu iProcessable işaretçisini ayarlar.
         * Böylece ilgili processable nesne tipini bulur. Ardından parametre olarak gelen username bilgisini,
         * nesnenin get fonksiyonuna parametre olarak vererek nesneye erişir.
         */
        public ParentObject get(string username, Type parameterType){
            iProcessable = tableMapper.getMapper(parameterType);
            return iProcessable.getItem(username);
        }
    
        /*
         * Parametre olarak gelen type ile önce kendi tuttuğu iProcessable işaretçisini ayarlar.
         * Böylece ilgili processable nesne tipini bulur. Ardından parametre olarak gelen id bilgisini,
         * nesnenin get fonksiyonuna parametre olarak vererek nesneye erişir.
         */
        public ParentObject get(int id, Type parameterType){
            iProcessable = tableMapper.getMapper(parameterType);
            return iProcessable.getItem(id);
        }

        /*
         * Parametre olarak gelen type ile önce kendi tuttuğu iProcessable işaretçisini ayarlar.
         * Böylece ilgili processable nesne tipini bulur. Ardından parametre olarak gelen id bilgisini,
         * nesnenin delete fonksiyonuna parametre olarak vererek nesneyi siler
         */
        public void delete(int id, Type parameterType){
            iProcessable = tableMapper.getMapper(parameterType);
            iProcessable.deleteItem(id);
        }
        
        /*
         * Parametre olarak gelen monoBehaviourObject ile önce type bilgisini bulur.
         * Bu type bilgisi ile kendi tuttuğu iProcessable işaretçisini ayarlar.
         * Böylece ilgili processable nesne tipini bulur. Ardından parametre olarak gelen monoBehaviourObject'i, 
         * nesnenin update fonksiyonuna parametre olarak vererek nesneyi günceller.
         */
        public void update(ParentObject parentObject){
            IProcessable DBMapper = tableMapper.getMapper(parentObject.GetType());
            DBMapper.updateItem(parentObject);
        }
        
        /*
         * Parametre olarak gelen monoBehaviourObject ile önce type bilgisini bulur.
         * Bu type bilgisi ile kendi tuttuğu iProcessable işaretçisini ayarlar.
         * Böylece ilgili processable nesne tipini bulur. Ardından parametre olarak gelen monoBehaviourObject'i, 
         * nesnenin put fonksiyonuna parametre olarak vererek doğru şekilde kayıt eder.
         */
        public void put(ParentObject parentObject ){
            IProcessable DBMapper = tableMapper.getMapper(parentObject.GetType());
            DBMapper.putItem(parentObject);
        }
        
        public int putAndReturnID(ParentObject parentObject){
            IProcessable DBMapper = tableMapper.getMapper(parentObject.GetType());
            return DBMapper.putItemAndReturnId(parentObject);
        }
        
        
        //Kendi tuttuğu tableMapper işaretçisini parametre olarak gelen tableMapper ile eşler.
        public void setTableMapper(TableMapper tableMapper) {
            this.tableMapper = tableMapper;
        }
        //Kendi tuttuğu iProcessable işaretçisini parametre olarak gelen iProcessable ile eşler.
        public void setiProcessable(IProcessable iProcessable) {
            this.iProcessable = iProcessable;
        }
        
        //Kendi tuttuğu tableMapper'ı getirir.
        public TableMapper getTableMapper() {
            return tableMapper;
        }
        //Kendi tuttuğu iProcessable'ı getirir.
        public IProcessable getiProcessable() {
            return iProcessable;
        }
    }
}