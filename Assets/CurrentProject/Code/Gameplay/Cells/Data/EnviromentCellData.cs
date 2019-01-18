using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	public class EnviromentCellData : AbCellData
	{
		public CellSource Source;
		public BugTypes Bug;
		public int BugCount = 1;
		public AntSwarm Occupant;

		public bool IsOccupied
		{
			get
			{
				return Occupant != null;
			}
		}

		public bool IsAllyArmy
		{
			get
			{
				return IsOccupied && Occupant.Owner == GameplayManager.Instance.Player;
			}
		}

		public bool IsEnemyArmy
		{
			get
			{
				return IsOccupied && Occupant.Owner != GameplayManager.Instance.Player;
			}
		}

		public bool IsGatherable
		{
			get
			{
				return Bug == BugTypes.None && Source != null && Source.Type != SourceTypes.None && !Source.IsDepleted;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return Bug == BugTypes.None && Source != null && (Source.Type == SourceTypes.None || Source.IsDepleted);
			}
		}

		public bool IsBugged
		{
			get
			{
				return Bug != BugTypes.None;
			}
		}
	}
}