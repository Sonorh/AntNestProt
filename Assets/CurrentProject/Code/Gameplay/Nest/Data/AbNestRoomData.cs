using PM.UsefulThings.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	public abstract class AbNestRoomData : IQuadTreeObject
	{
		public static event Action<AbNestRoomData> OnClickEvent;

		public Vector2Int Position { get; set; }

		public RoomTypes Type { get; protected set; }
		public int LevelId { get; protected set; } = 0;

		public bool CanUpgrade
		{
			get
			{
				return !IsUpgrading;
			}
		}

		public bool IsUpgrading { get; set; }

		public void OnMouseClickHandler()
		{
			OnClickEvent?.Invoke(this);
		}
	}
}