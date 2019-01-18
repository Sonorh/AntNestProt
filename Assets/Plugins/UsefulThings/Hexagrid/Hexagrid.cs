using PM.UsefulThings.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PM.UsefulThings
{
	public class Hexagrid<T> : IEnumerable<KeyValuePair<Vector3Int, T>> where T : class
	{
		private Dictionary<Vector3Int, T> cells = new Dictionary<Vector3Int, T>();

		public System.Func<Vector3Int, T> HexcellConstructor { get; set; }

		[System.Runtime.CompilerServices.IndexerName("Position")]
		public T this[Vector3Int index]
		{
			get
			{
				//valid cell should have sum of exis == 0
				if (index.x + index.y + index.z != 0)
				{
					throw new System.Exception("Invalid cell position requested");
				}

				if (cells.ContainsKey(index))
					return cells[index];

				var cell = HexcellConstructor(index);
				cells[index] = cell;

				return cell;
			}
			set
			{
				//valid cell should have sum of exis == 0
				if (index.x + index.y + index.z != 0)
				{
					throw new System.Exception("Invalid cell position set");
				}

				if (cells.ContainsKey(index))
				{
					//todo destroy old cell
				}

				cells[index] = value;
			}
		}

		public List<T> GetNeighbours(Vector3Int center)
		{
			return GetCellsInRadius(center, 1, false);
		}

		public T GetCell(Vector3Int pos)
		{
			return this[pos];
		}

		public bool IsCellCreated(Vector3Int pos)
		{
			return cells.ContainsKey(pos);
		}

		public List<T> GetCellsInRadius(Vector3Int center, int offset, bool isCenterIncluded = true)
		{
			//valid cell should have sum of exis == 0
			if (center.x + center.y + center.z != 0)
			{
				throw new System.Exception("Invalid center position set");
			}

			offset = Mathf.Abs(offset);

			var result = new List<T>();

			for (int x = -offset; x <= offset; x++)
			{
				for (int y = -offset; y <= offset; y++)
				{
					for (int z = -offset; z <= offset; z++)
					{
						//valid cell should have sum of exis == 0
						if (x + y + z != 0)
							continue;

						result.Add(this[center + new Vector3Int(x, y, z)]);
					}
				}
			}

			if (!isCenterIncluded)
				result.Remove(this[center]);

			return result;
		}

		public List<T> GetExistingCellsInRadius(Vector3Int center, int offset, bool isCenterIncluded = true)
		{
			//valid cell should have sum of exis == 0
			if (center.x + center.y + center.z != 0)
			{
				throw new System.Exception("Invalid center position set");
			}

			offset = Mathf.Abs(offset);

			var result = new List<T>();

			for (int x = -offset; x <= offset; x++)
			{
				for (int y = -offset; y <= offset; y++)
				{
					for (int z = -offset; z <= offset; z++)
					{
						//valid cell should have sum of exis == 0
						if (x + y + z != 0)
							continue;

						if (IsCellCreated(center + new Vector3Int(x, y, z)))
						{
							result.Add(this[center + new Vector3Int(x, y, z)]);
						}
					}
				}
			}

			if (!isCenterIncluded)
				result.Remove(this[center]);

			return result;
		}

		public ICollection<T> Cells
		{
			get
			{
				return cells.Values;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return cells.GetEnumerator();
		}

		public IEnumerator<KeyValuePair< Vector3Int, T>> GetEnumerator()
		{
			return cells.GetEnumerator();
		}

		public void RemoveAt(Vector3Int pos)
		{
			cells.Remove(pos);
		}

		public void Remove(T cell)
		{
			KeyValuePair<Vector3Int, T> result;

			if (cells.Find(x => x == cell, out result))
			{
				cells.Remove(result.Key);
			}
		}

		public void Clear()
		{
			cells.Clear();
		}
	}
}