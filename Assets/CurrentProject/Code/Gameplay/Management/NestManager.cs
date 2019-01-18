using PM.UsefulThings.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	public class NestManager
	{
		public event Action OnNestRebuilt;

		public QuadTree<AbNestRoomData> Rooms = new QuadTree<AbNestRoomData>(100, new Rect(-5, 0, 11, 10));

		public NestManager()
		{
			Rooms.Add(new EntranceData { Position = Vector2Int.zero });
			Rooms.Add(new WarehouseData { Position = new Vector2Int(0, 1) });
			Rooms.Add(new BuildingSpotData { Position = new Vector2Int(-1, 1) });
			Rooms.Add(new BuildingSpotData { Position = new Vector2Int(0, 2) });
			Rooms.Add(new BuildingSpotData { Position = new Vector2Int(1, 1) });
		}

		public void Build(RoomTypes type, Vector2Int position)
		{
			if (type == RoomTypes.Construction)
			{
				return;
			}

			var room = Rooms[position];
			if (room == null || room.Type != RoomTypes.Construction || (room as ConstructionData).TargetType != type)
			{
				return;
			}

			Rooms.RemoveAt(position);
			switch (type)
			{
				case RoomTypes.Entrance:
					Rooms.Add(new EntranceData { Position = position });
					break;
				case RoomTypes.BuildingSpot:
					Rooms.Add(new BuildingSpotData { Position = position });
					break;
				case RoomTypes.Warehause:
					Rooms.Add(new WarehouseData { Position = position });
					break;
				case RoomTypes.Nursery:
					Rooms.Add(new NurseryData { Position = position });
					break;
				case RoomTypes.Armory:
					Rooms.Add(new ArmoryData { Position = position });
					break;
				default:
					break;
			}

			for (int y = 0; y <= 1; y++)
			{
				for (int x = -1; x <= 1; x++)
				{
					if (Mathf.Abs(x) == Mathf.Abs(y))
					{
						continue;
					}

					var pos = position + new Vector2Int(x, y);

					if (Rooms[pos] == null && Rooms.ContainsLocation(pos))
					{
						Rooms.Add(new BuildingSpotData { Position = pos });
					}
				}
			}

			OnNestRebuilt?.Invoke();
		}

		public bool CanBuild(RoomTypes type, Vector2Int position)
		{
			var room = Rooms[position];
			if (room != null && room.Type == RoomTypes.BuildingSpot)
			{
				return true;
			}

			return false;
		}

		public bool AddConstruction(RoomTypes type, Vector2Int position)
		{
			if (!CanBuild(type, position))
			{
				return false;
			}

			Rooms.RemoveAt(position);
			Rooms.Add(new ConstructionData { Position = position, TargetType = type });

			OnNestRebuilt?.Invoke();

			return true;
		}

		public void RemoveConstruction(Vector2Int position)
		{
			var room = Rooms[position];
			if (room != null && room.Type == RoomTypes.Construction)
			{
				Rooms.RemoveAt(position);
				Rooms.Add(new BuildingSpotData { Position = position });
			}

			OnNestRebuilt?.Invoke();
		}

		public void SetUpgradeRoom(AbNestRoomData room, bool value)
		{
			if (room.CanUpgrade && value)
			{
				room.IsUpgrading = true;

				OnNestRebuilt?.Invoke();
			}
			else if (!value)
			{
				room.IsUpgrading = false;

				OnNestRebuilt?.Invoke();
			}
		}
	}
}