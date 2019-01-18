using PM.UsefulThings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
    public class GameplayHolderManager : SoftMonoSingleton<GameplayHolderManager>
	{
        public GameplayPrefabsHolder Prefabs;
		public ConfigHolder Configs;
		public SpriteHolder Sprites;
    }
}