using PM.UsefulThings;
using PM.UsefulThings.Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
    public class MapManager : SoftMonoSingleton<MapManager>
	{
		public event System.Action OnHide;
		public event System.Action OnShow;

		public Hexagrid<AbCell> Field { get; set; } = new Hexagrid<AbCell>();
		public Hexagrid<AbCellData> FieldData { get; set; } = new Hexagrid<AbCellData>();

		protected override void Awake()
		{
			base.Awake();

			Field.HexcellConstructor = CreateNewCell;
		}

		private AbCell CreateNewCell(Vector3Int pos)
		{
			AbCell res;
			var data = FieldData[pos];
			if (data is NestCellData)
			{
				res = PoolManager.Instance.NestCellPool.Pull();
			}
			else
			{
				res = PoolManager.Instance.EnviromentCellPool.Pull();
			}
			res.Reset(data);
			res.transform.localPosition = res.ScreenPosition;
			return res;
		}

		public List<Vector3Int> GetPath(AbCellData start, AbCellData finish)
		{
			var path = AStar.FindPath(start, finish).ConvertAll<Vector3Int>(x => x.FieldPosition);
			return path;
		}

		public void Hide()
		{
			foreach (var cell in Field.Cells)
			{
				if (cell.Data is NestCellData)
				{
					PoolManager.Instance.NestCellPool.Push(cell as NestCell);
				}
				else
				{
					PoolManager.Instance.EnviromentCellPool.Push(cell as EnviromentCell);
				}
			}
			Field.Clear();
			OnHide?.Invoke();
		}

		public void Show()
		{
			foreach (var data in FieldData.Cells)
			{
				Field.GetCell(data.FieldPosition);
			}
			OnShow?.Invoke();
		}

		public void Clear()
		{
			foreach (var cell in Field.Cells)
			{
				if (cell.Data is NestCellData)
				{
					 PoolManager.Instance.NestCellPool.Push(cell as NestCell);
				}
				else
				{
					 PoolManager.Instance.EnviromentCellPool.Push(cell as EnviromentCell);
				}
			}
			FieldData.Clear();
			Field.Clear();
		}
	}
}