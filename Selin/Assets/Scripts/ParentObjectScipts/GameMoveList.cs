using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameMoveList : ParentObject
    {
        private IDictionary<int,GameMove> gameMoveMap = new Dictionary<int, GameMove>();

        public GameMoveList(int id)
        {
            this.id = id;
        }


        public IDictionary<int, GameMove> GameMoveMap => gameMoveMap;
    }
}