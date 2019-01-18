using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	public abstract class AbAvatarPlan : AbPlayerPlan
	{
		protected AntAvatar antAvatar;

		private void ShowAvatar()
		{
			if (antAvatar == null)
			{
				SpawnAvatar();
				antAvatar.transform.position = MapManager.Instance.Field[Ants.Position.FieldPosition].ScreenPosition;
			}
		}

		private void HideAvatar()
		{
			if (antAvatar != null)
			{
				PoolManager.Instance.AntPool.Push(antAvatar);
				antAvatar = null;
			}
		}

		public override void Update()
		{
			base.Update();

			if (Ants.AntCount == 0)
			{
				OnDoneCaller();
			}
		}

		public override void Prepare()
		{
			MoveToTarget();

			MapManager.Instance.OnHide += HideAvatar;
			MapManager.Instance.OnShow += ShowAvatar;
		}

		protected void SpawnAvatar()
		{
			antAvatar = PoolManager.Instance.AntPool.Pull();
			antAvatar.Swarm = Ants;
		}

		protected void MoveToTarget()
		{
			var nextCell = GetNextPathCellData();
			if (nextCell != null)
			{
				Ants.Pack();
				Ants.Position = nextCell;
				if (antAvatar != null)
				{
					antAvatar.transform.position = MapManager.Instance.Field[nextCell.FieldPosition].ScreenPosition;
				}
				Ants.Camp();
			}
		}

		protected AbCellData GetNextPathCellData()
		{
			var path = MapManager.Instance.GetPath(Ants.Position, TargetCell);
			if (path.Count == 0)
			{
				return null;
			}

			for (int i = Mathf.Min(Ants.Speed, path.Count) - 1; i >= 0; i--)
			{
				if (MapManager.Instance.FieldData[path[i]].IsWalkable)
				{
					return MapManager.Instance.FieldData[path[i]];
				}
			}

			return null;
		}

		protected override void OnDoneCaller()
		{
			Ants.Pack();
			if (antAvatar != null)
			{
				PoolManager.Instance.AntPool.Push(antAvatar);
			}
			MapManager.Instance.OnHide -= HideAvatar;
			MapManager.Instance.OnShow -= ShowAvatar;
			base.OnDoneCaller();
		}

		public override bool IsExecutable
		{
			get
			{
				return IsSpawnPossible();
			}
		}

		protected bool IsSpawnPossible()
		{
			return GetNextPathCellData() != null;
		}
	}
}