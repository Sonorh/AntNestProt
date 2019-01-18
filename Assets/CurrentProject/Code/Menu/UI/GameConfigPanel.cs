using PM.Antnest.General;
using PM.UsefulThings;
using System;
using System.Collections.Generic;
using UIBinding;
using UnityEngine;

namespace PM.Antnest.Menu
{
	public class GameConfigPanel : AbPanel
	{
		private readonly IntProperty botNumber = new IntProperty();
		private readonly FloatProperty botNumberSlider = new FloatProperty();
		private readonly IntProperty spawnDistance = new IntProperty();
		private readonly FloatProperty spawnDistanceSlider = new FloatProperty();
		private readonly IntProperty resourceChance = new IntProperty();
		private readonly FloatProperty resourceChanceSlider = new FloatProperty();
		private readonly IntProperty resourceQuality = new IntProperty();
		private readonly FloatProperty resourceQualitySlider = new FloatProperty();
		private readonly IntProperty bugChance = new IntProperty();
		private readonly FloatProperty bugChanceSlider = new FloatProperty();
		private readonly IntProperty bugStrength = new IntProperty();
		private readonly FloatProperty bugStrengthSlider = new FloatProperty();
		private readonly IntProperty startFruits = new IntProperty();
		private readonly FloatProperty startFruitsSlider = new FloatProperty();
		private readonly IntProperty startMeat = new IntProperty();
		private readonly FloatProperty startMeatSlider = new FloatProperty();
		private readonly IntProperty startSwarmWorkers = new IntProperty();
		private readonly FloatProperty startSwarmWorkersSlider = new FloatProperty();
		private readonly IntProperty startSwarmWarriors = new IntProperty();
		private readonly FloatProperty startSwarmWarriorsSlider = new FloatProperty();

		public override bool IsSolid => true;

		private GameConfigRangesConfig ranges;
		private GameConfig config;

		private void Start()
		{
			config = MainMenuHolderManager.Instance.Configs.CurrentGameConfig;
			ranges = MainMenuHolderManager.Instance.Configs.GCRangesConfig;

			config.Mimicry(MainMenuHolderManager.Instance.Configs.DefaultGameConfig);

			config.StartSwarm.Add(Gameplay.AntTypes.Worker, 10);

			SetDefault();

			botNumberSlider.OnValueChanged += BotNumberSlider_OnValueChanged;
			spawnDistanceSlider.OnValueChanged += SpawnDistanceSlider_OnValueChanged;
			resourceChanceSlider.OnValueChanged += ResourceChanceSlider_OnValueChanged;
			resourceQualitySlider.OnValueChanged += ResourceQualitySlider_OnValueChanged;
			bugChanceSlider.OnValueChanged += BugChanceSlider_OnValueChanged;
			bugStrengthSlider.OnValueChanged += BugStrengthSlider_OnValueChanged;
			startFruitsSlider.OnValueChanged += StartFruitsSlider_OnValueChanged;
			startMeatSlider.OnValueChanged += StartMeatSlider_OnValueChanged;
			startSwarmWorkersSlider.OnValueChanged += StartSwarmWorkersSlider_OnValueChanged;
			startSwarmWarriorsSlider.OnValueChanged += StartSwarmWarriorsSlider_OnValueChanged;
		}

		private void OnDestroy()
		{
			botNumberSlider.OnValueChanged -= BotNumberSlider_OnValueChanged;
			spawnDistanceSlider.OnValueChanged -= SpawnDistanceSlider_OnValueChanged;
			resourceChanceSlider.OnValueChanged -= ResourceChanceSlider_OnValueChanged;
			resourceQualitySlider.OnValueChanged -= ResourceQualitySlider_OnValueChanged;
			bugChanceSlider.OnValueChanged -= BugChanceSlider_OnValueChanged;
			bugStrengthSlider.OnValueChanged -= BugStrengthSlider_OnValueChanged;
			startFruitsSlider.OnValueChanged -= StartFruitsSlider_OnValueChanged;
			startMeatSlider.OnValueChanged -= StartMeatSlider_OnValueChanged;
			startSwarmWorkersSlider.OnValueChanged -= StartSwarmWorkersSlider_OnValueChanged;
			startSwarmWarriorsSlider.OnValueChanged -= StartSwarmWarriorsSlider_OnValueChanged;
		}

		private void SetDefault()
		{
			botNumber.value = Mathf.Clamp(config.BotNumber, ranges.BotNumberMinMax.x, ranges.BotNumberMinMax.y);
			spawnDistance.value = Mathf.Clamp(config.SpawnDistance, ranges.SpawnDistanceMinMax.x, ranges.SpawnDistanceMinMax.y);
			resourceChance.value = config.ResourceChance;
			resourceQuality.value = config.ResourceQuality;
			bugChance.value = config.BugChance;
			bugStrength.value = config.BugStrength;
			startFruits.value = Mathf.Clamp(config.StartFruits, ranges.StartFruitsMinMax.x, ranges.StartFruitsMinMax.y); 
			startMeat.value = Mathf.Clamp(config.StartMeat, ranges.StartMeatMinMax.x, ranges.StartMeatMinMax.y); 
			if (config.StartSwarm.Ants.ContainsKey(Gameplay.AntTypes.Worker))
			{
				startSwarmWorkers.value = config.StartSwarm.Ants[Gameplay.AntTypes.Worker];
			}
			if (config.StartSwarm.Ants.ContainsKey(Gameplay.AntTypes.Warrior))
			{
				startSwarmWarriors.value = config.StartSwarm.Ants[Gameplay.AntTypes.Warrior];
			}

			botNumberSlider.value = (botNumber.value - ranges.BotNumberMinMax.x) / (float)(ranges.BotNumberMinMax.y - ranges.BotNumberMinMax.x);
			spawnDistanceSlider.value = (spawnDistance.value - ranges.SpawnDistanceMinMax.x) / (float)(ranges.SpawnDistanceMinMax.y - ranges.SpawnDistanceMinMax.x);
			resourceChanceSlider.value = GetLMHDefaultSlider(resourceChance.value, ranges.ResourceChanceLMH);
			resourceQualitySlider.value = GetLMHDefaultSlider(resourceQuality.value, ranges.ResourceQualityLMH);
			bugChanceSlider.value = GetLMHDefaultSlider(bugChance.value, ranges.BugChanceLMH);
			bugStrengthSlider.value = GetLMHDefaultSlider(bugStrength.value, ranges.BugStrengthLMH);
			startFruitsSlider.value = (startFruits.value - ranges.StartFruitsMinMax.x) / (float)(ranges.StartFruitsMinMax.y - ranges.StartFruitsMinMax.x);
			startMeatSlider.value = (startMeat.value - ranges.StartMeatMinMax.x) / (float)(ranges.StartMeatMinMax.y - ranges.StartMeatMinMax.x);
			startSwarmWorkersSlider.value = (startSwarmWorkers.value - ranges.StartSwarmMinMax.x) / (float)(ranges.StartSwarmMinMax.y - ranges.StartSwarmMinMax.x);
			startSwarmWarriorsSlider.value = (startSwarmWarriors.value - ranges.StartSwarmMinMax.x) / (float)(ranges.StartSwarmMinMax.y - ranges.StartSwarmMinMax.x);
		}

		private float GetLMHDefaultSlider(int value, Vector3Int LMH)
		{
			if (value == LMH.x)
			{
				return 0f;
			}
			else if (value == LMH.x)
			{
				return 0.5f;
			}
			else if (value == LMH.x)
			{
				return 1f;
			}
			else
			{
				return 0.5f;
			}
		}

		private void BotNumberSlider_OnValueChanged()
		{
			botNumber.value = ranges.BotNumberMinMax.x + Mathf.CeilToInt((ranges.BotNumberMinMax.y - ranges.BotNumberMinMax.x) * botNumberSlider.value);

			config.BotNumber = botNumber.value;
		}

		private void SpawnDistanceSlider_OnValueChanged()
		{
			spawnDistance.value = ranges.SpawnDistanceMinMax.x + Mathf.CeilToInt((ranges.SpawnDistanceMinMax.y - ranges.SpawnDistanceMinMax.x) * spawnDistanceSlider.value);

			config.SpawnDistance = spawnDistance.value;
		}

		private void ResourceChanceSlider_OnValueChanged()
		{
			if (resourceChanceSlider.value <= 0.33f)
			{
				resourceChance.value = ranges.ResourceChanceLMH.x;
			}
			else if (resourceChanceSlider.value <= 0.66f)
			{
				resourceChance.value = ranges.ResourceChanceLMH.y;
			}
			else
			{
				resourceChance.value = ranges.ResourceChanceLMH.z;
			}

			config.ResourceChance = resourceChance.value;
		}

		private void ResourceQualitySlider_OnValueChanged()
		{
			if (resourceQualitySlider.value <= 0.33f)
			{
				resourceQuality.value = ranges.ResourceQualityLMH.x;
			}
			else if (resourceQualitySlider.value <= 0.66f)
			{
				resourceQuality.value = ranges.ResourceQualityLMH.y;
			}
			else
			{
				resourceQuality.value = ranges.ResourceQualityLMH.z;
			}

			config.ResourceQuality = resourceQuality.value;
		}

		private void BugChanceSlider_OnValueChanged()
		{
			if (bugChanceSlider.value <= 0.33f)
			{
				bugChance.value = ranges.BugChanceLMH.x;
			}
			else if (bugChanceSlider.value <= 0.66f)
			{
				bugChance.value = ranges.BugChanceLMH.y;
			}
			else
			{
				bugChance.value = ranges.BugChanceLMH.z;
			}

			config.BugChance = bugChance.value;
		}

		private void BugStrengthSlider_OnValueChanged()
		{
			if (bugStrengthSlider.value <= 0.33f)
			{
				bugStrength.value = ranges.BugStrengthLMH.x;
			}
			else if (bugStrengthSlider.value <= 0.66f)
			{
				bugStrength.value = ranges.BugStrengthLMH.y;
			}
			else
			{
				bugStrength.value = ranges.BugStrengthLMH.z;
			}

			config.BugStrength = bugStrength.value;
		}

		private void StartFruitsSlider_OnValueChanged()
		{
			startFruits.value = ranges.StartFruitsMinMax.x + Mathf.CeilToInt((ranges.StartFruitsMinMax.y - ranges.StartFruitsMinMax.x) * startFruitsSlider.value);

			config.StartFruits = startFruits.value;
		}

		private void StartMeatSlider_OnValueChanged()
		{
			startMeat.value = ranges.StartMeatMinMax.x + Mathf.CeilToInt((ranges.StartMeatMinMax.y - ranges.StartMeatMinMax.x) * startMeatSlider.value);

			config.StartMeat = startMeat.value;
		}

		private void StartSwarmWorkersSlider_OnValueChanged()
		{
			startSwarmWorkers.value = ranges.StartSwarmMinMax.x + Mathf.CeilToInt((ranges.StartSwarmMinMax.y - ranges.StartSwarmMinMax.x) * startSwarmWorkersSlider.value);

			config.StartSwarm.Remove(Gameplay.AntTypes.Worker, int.MaxValue);
			config.StartSwarm.Add(Gameplay.AntTypes.Worker, startSwarmWorkers.value);
		}

		private void StartSwarmWarriorsSlider_OnValueChanged()
		{
			startSwarmWarriors.value = ranges.StartSwarmMinMax.x + Mathf.CeilToInt((ranges.StartSwarmMinMax.y - ranges.StartSwarmMinMax.x) * startSwarmWarriorsSlider.value);

			config.StartSwarm.Remove(Gameplay.AntTypes.Warrior, int.MaxValue);
			config.StartSwarm.Add(Gameplay.AntTypes.Warrior, startSwarmWarriors.value);
		}

		public void StartGame()
		{
			MainMenuManager.Instance.StartTheGame();
		}
	}
}