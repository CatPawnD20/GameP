using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts
{
    /*
     * Kendi içinde IDictionary tipinde bir ****mapper**** adında bir dictionary tutar.
     * Ayrıca her IProcessable sınıfından birer nesneye sahiptir. 
     * Böylece Type bilgisi ---->>> Processable nesne bilgisine dönüştürülür.
     */
    public class TableMapper 
    {
        private IDictionary<string, IProcessable> mapper = new Dictionary<string,IProcessable>();
        private static PostgreSQL postgreSQL = new PostgreSQL();

        private LoginDB loginDB = new LoginDB(postgreSQL);
        private UserDB userDB = new UserDB(postgreSQL); 
        private LogoutDB logoutDB = new LogoutDB(postgreSQL);
        private TableDB tableDB = new TableDB(postgreSQL);
        private TurnDB turnDB = new TurnDB(postgreSQL);
        private TurnPlayersDB turnPlayersDB = new TurnPlayersDB(postgreSQL);
        private GameMoveDB gameMoveDB = new GameMoveDB(postgreSQL);
        private OmahaHandDB omahaHandDB = new OmahaHandDB(postgreSQL);
        private SharedCardsDB sharedCardsDB = new SharedCardsDB(postgreSQL);
        private TurnWinnersDB turnWinnersDB = new TurnWinnersDB(postgreSQL);
        private GameParticipationDB gameParticipationDB = new GameParticipationDB(postgreSQL);

        
        public TableMapper() {
            mapper.Add(typeof(Login).ToString(),loginDB);
            mapper.Add(typeof(User).ToString(),userDB);
            mapper.Add(typeof(Logout).ToString(),logoutDB);
            mapper.Add(typeof(Table).ToString(),tableDB);
            mapper.Add(typeof(Turn).ToString(),turnDB);
            mapper.Add(typeof(TurnPlayers).ToString(),turnPlayersDB);
            mapper.Add(typeof(GameMove).ToString(),gameMoveDB);
            mapper.Add(typeof(OmahaHand).ToString(),omahaHandDB);
            mapper.Add(typeof(SharedCards).ToString(),sharedCardsDB);
            mapper.Add(typeof(TurnWinners).ToString(),turnWinnersDB);
            mapper.Add(typeof(GameParticipation).ToString(),gameParticipationDB);
        }

        public IProcessable getMapper(Type parameterType)
        {
            Debug.Log("TableMapper.cs -->>> getMapper -->>> parameterType : "+parameterType.ToString());
            return mapper[parameterType.ToString()];
            //return mapper.TryGetValue(parameterType,);
        }
    
        public void setMapper(IDictionary<string, IProcessable> mapper) {
            this.mapper = mapper;
        }
    }
}