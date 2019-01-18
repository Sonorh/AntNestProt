using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PM.UsefulThings;
using PM.Antnest.Gameplay;

namespace PM.Antnest.General
{
	[CreateAssetMenu(fileName = "GameConfig", menuName = "Config/GameConfig", order = 10)]
	public class GameConfig : ScriptableObject
	{
		public int BotNumber;
		public int SpawnDistance;
		public int ResourceChance;
		public int ResourceQuality;
		public int BugChance;
		public int BugStrength;
		public int StartFruits;
		public int StartMeat;
		public AntSwarm StartSwarm = new AntSwarm();

		public void Mimicry(GameConfig config)
		{
			this.BotNumber = config.BotNumber;
			this.SpawnDistance = config.SpawnDistance;
			this.ResourceChance = config.ResourceChance;
			this.ResourceQuality = config.ResourceQuality;
			this.BugChance = config.BugChance;
			this.BugStrength = config.BugStrength;
			this.StartFruits = config.StartFruits;
			this.StartMeat = config.StartMeat;

			StartSwarm = new AntSwarm(null, AntTypes.Larva);
			foreach (var item in config.StartSwarm.Ants)
			{
				StartSwarm.Add(item.Key, item.Value);
			}

			if (!StartSwarm.Ants.ContainsKey(AntTypes.Worker))
			{
				StartSwarm.Add(AntTypes.Worker, 0);
			}
			if (!StartSwarm.Ants.ContainsKey(AntTypes.Warrior))
			{
				StartSwarm.Add(AntTypes.Warrior, 0);
			}
		}
	}
}