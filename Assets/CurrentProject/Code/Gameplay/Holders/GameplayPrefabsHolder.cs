using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
    //[CreateAssetMenu(fileName = "GameplayPrefabsHolder", menuName = "GameplayPrefabsHolder", order = 1)]
    public class GameplayPrefabsHolder : ScriptableObject
    {
        public NestCell NestCellPrefab;
		public EnviromentCell EnviromentCellPrefab;
		public AntAvatar AntPrefab;
		public NestRoom NestRoomPrefab;
	}
}