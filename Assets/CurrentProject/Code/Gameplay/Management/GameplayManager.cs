using PM.Antnest.General;
using PM.UsefulThings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	public class GameplayManager : SoftMonoSingleton<GameplayManager>
	{
		public event System.Action OnGameplayOver;

		public PlayerManager Player;
		public List<PlayerManager> Enemies;

		private Dictionary<PlayerManager, Vector3Int> NestPositions = new Dictionary<PlayerManager, Vector3Int>();

		private GameConfig config;

		protected override void Awake()
		{
			base.Awake();

			WindowManagerUT.Instance.CloseFrames();
			WindowManagerUT.Instance.ClosePanels();

			SoundManager.Instance?.PlayMusic(SoundManager.Instance?.GameplayMusic);
		}

		public void Start()
		{
			config = GameplayHolderManager.Instance.Configs.GameSettings;

			// initialize
			MapManager.Instance.FieldData.HexcellConstructor = CreateNewCellData;
			AbCellData.OnClickEvent += InputManager.Instance.OnCellClick;
			AbNestRoomData.OnClickEvent += InputManager.Instance.OnRoomClick;

			TimeManager.Instance.ScrollTo(Weeks.MarchFirst, System.DateTime.Now.Year);

			// create player
			Player = new PlayerManager();
			Player.Resourcer.Store(ResourceTypes.Fruit, config.StartFruits);
			Player.Resourcer.Store(ResourceTypes.Meat, config.StartMeat);
			Player.Anter.AntIncome(config.StartSwarm);
			NestPositions.Add(Player, Vector3Int.zero);
			Player.Nest = MapManager.Instance.FieldData[Vector3Int.zero] as NestCellData;

			// create enemy players
			Enemies = new List<PlayerManager>();

			// open field
			MapManager.Instance.FieldData.GetCellsInRadius(Vector3Int.zero, 1);

			MapManager.Instance.Show();
			// show nest screen
			WindowManagerUT.Instance.OpenNewPanel<ForagePanel>();

			TimeManager.Instance.OnTimeChanged += OnTimeChangedHandler;
		}

		private void OnTimeChangedHandler()
		{
			if (TimeManager.Instance.Now == Weeks.MarchFirst)
			{
				MapManager.Instance.Clear();
				Player.Nest = MapManager.Instance.FieldData[Vector3Int.zero] as NestCellData;
				MapManager.Instance.FieldData.GetCellsInRadius(Vector3Int.zero, 1);
				MapManager.Instance.Show();
			}
		}

		private AbCellData CreateNewCellData(Vector3Int pos)
		{
			AbCellData res;
			if (NestPositions.ContainsValue(pos))
			{
				res = new NestCellData();
			}
			else
			{
				//todo: randomize this cell
				var env = new EnviromentCellData();
				if (Random.Range(0, 100) < config.ResourceChance)
				{
					if (Random.Range(0, 100) < 70)
					{
						if (Random.Range(0, 100) < 30)
						{
							env.Source = new CellSource(SourceTypes.SolidFruit, 2 * config.ResourceQuality + Random.Range(-config.ResourceQuality, config.ResourceQuality));
						}
						else
						{
							env.Source = new CellSource(SourceTypes.FlimsyFruit, config.ResourceQuality + Random.Range(-config.ResourceQuality / 2, config.ResourceQuality / 2));
						}
					}
					else
					{
						if (Random.Range(0, 100) < 30)
						{
							env.Source = new CellSource(SourceTypes.SolidMeat, 2 * config.ResourceQuality + Random.Range(-config.ResourceQuality, config.ResourceQuality));
						}
						else
						{
							env.Source = new CellSource(SourceTypes.FlimsyMeat, config.ResourceQuality + Random.Range(-config.ResourceQuality / 2, config.ResourceQuality / 2));
						}
					}
				}
				if (Random.Range(0, 100) < config.BugChance)
				{
					env.Bug = BugTypes.Caterpillar;
					env.BugCount = pos.magnitude < 4 ? config.BugStrength * 1 : pos.magnitude < 8 ? config.BugStrength * 2 : config.BugStrength * 3;
				}
				res = env;
			}
			res.FieldPosition = pos;
			return res;
		}

		private void Update()
		{
			if (Input.GetKeyDown((KeyCode)PlayerPrefs.GetInt("Esc")))
			{
				Pause();
			}
		}

		public void Pause()
		{
			if (WindowManagerUT.Instance.ActivePanel == null || WindowManagerUT.Instance.ActivePanel.GetType() != typeof(PausePanel))
			{
				MapManager.Instance.Hide();
				WindowManagerUT.Instance.OpenNewPanel<PausePanel>();
			}
		}

		public void Unpause()
		{
			if (WindowManagerUT.Instance.ActivePanel != null && WindowManagerUT.Instance.ActivePanel.GetType() == typeof(PausePanel))
			{
				WindowManagerUT.Instance.ActivePanel.Close();
				MapManager.Instance.Show();
				WindowManagerUT.Instance.OpenNewPanel<ForagePanel>();
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			AbCellData.OnClickEvent -= InputManager.Instance.OnCellClick;
			AbNestRoomData.OnClickEvent -= InputManager.Instance.OnRoomClick;
			TimeManager.Instance.OnTimeChanged -= OnTimeChangedHandler;
		}

		public void GoToMainMenu()
		{
			OnGameplayOver?.Invoke();

			UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
		}
	}
}