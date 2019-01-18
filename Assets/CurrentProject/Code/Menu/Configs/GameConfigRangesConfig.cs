using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PM.UsefulThings;
using PM.Antnest.Gameplay;

namespace PM.Antnest.General
{
	//[CreateAssetMenu(fileName = "GameConfigRangesConfig", menuName = "Config/GameConfigRangesConfig", order = 10)]
	public class GameConfigRangesConfig : ScriptableObject
	{
		public Vector2Int BotNumberMinMax;
		public Vector2Int SpawnDistanceMinMax;
		// LMH = low/mid/high
		public Vector3Int ResourceChanceLMH;
		public Vector3Int ResourceQualityLMH;
		public Vector3Int BugChanceLMH;
		public Vector3Int BugStrengthLMH;
		public Vector2Int StartFruitsMinMax;
		public Vector2Int StartMeatMinMax;
		public Vector2Int StartSwarmMinMax;
	}
}