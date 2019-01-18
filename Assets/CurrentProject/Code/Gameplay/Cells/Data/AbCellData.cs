using PM.UsefulThings.Extensions;
using PM.UsefulThings.Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIBinding.Base;

namespace PM.Antnest.Gameplay
{
	public abstract class AbCellData : BaseBindingTarget, IPathNode
	{
		public static event Action<AbCellData> OnClickEvent;
		public static event Action<AbCellData> OnExplore;

		public Vector3Int FieldPosition;
		public bool isExplored;

		Vector3Int IPathNode.FieldPosition => FieldPosition;

		public bool IsWalkable
		{
			get
			{
				return (this is NestCellData) || !(this as EnviromentCellData).IsOccupied;
			}
		}

		public int Priority { get; set; }
		public int QueueIndex { get; set; }
		public long InsertionIndex { get; set; }

		public virtual void OnClick()
		{
			OnClickEvent?.Invoke(this);
		}

		public List<IPathNode> GetNeighbours()
		{
			return MapManager.Instance.FieldData.GetExistingCellsInRadius(FieldPosition, 1, false).ConvertAll<IPathNode>(x => (x as IPathNode));
		}

		public int GetHeuristic(IPathNode target)
		{
			return Mathf.Max(Mathf.Abs(target.FieldPosition.x - this.FieldPosition.x), Mathf.Abs(target.FieldPosition.y - this.FieldPosition.y), Mathf.Abs(target.FieldPosition.z - this.FieldPosition.z));
		}

		public void Explore()
		{
			isExplored = true;
			MapManager.Instance.Field.GetCellsInRadius(FieldPosition, 1);
			OnExplore?.Invoke(this);
		}
	}
}