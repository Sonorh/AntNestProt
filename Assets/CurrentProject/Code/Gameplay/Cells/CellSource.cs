using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	public class CellSource
	{
		public static event System.Action<CellSource> OnGathered;
		public static event System.Action<CellSource, int> OnResidualChanged;
		public static event System.Action<CellSource> OnDepleted;


		public SourceTypes Type;
		public int Size;
		public int Residual;
		public int Gathered;

		public bool IsDepleted { get { return Residual == 0; } }

		public ResourceTypes ResourceType
		{
			get
			{
				switch (Type)
				{
					case SourceTypes.FlimsyMeat:
					case SourceTypes.SolidMeat:
						return ResourceTypes.Meat;

					case SourceTypes.FlimsyFruit:
					case SourceTypes.SolidFruit:
						return ResourceTypes.Fruit;

					case SourceTypes.None:
					default:
						return ResourceTypes.Meat;
				}
			}
		}

		public CellSource(SourceTypes type, int size)
		{
			this.Type = type;
			this.Size = size;
			this.Residual = size;
			this.Gathered = 0;
		}

		public int Gather(int amount)
		{
			if (Type == SourceTypes.None || IsDepleted || amount == 0)
				return 0;

			if (Type == SourceTypes.FlimsyFruit || Type == SourceTypes.FlimsyMeat)
			{
				int gath = Residual - amount > 0 ? amount : Residual;

				Residual -= gath;
				Gathered += gath;

				if (gath > 0)
				{
					OnResidualChanged?.Invoke(this, Residual);
					OnGathered?.Invoke(this);
				}

				if (Residual == 0)
				{
					OnDepleted?.Invoke(this);
				}

				return gath;
			}
			else
			{
				int gath = Size - Gathered - amount > 0 ? amount : Size - Gathered;

				Gathered += gath;

				if (gath > 0)
				{
					OnGathered?.Invoke(this);
				}

				if (Size - Gathered <= 0)
				{
					Residual = 0;

					//todo: think if change event is needed here
					OnResidualChanged?.Invoke(this, Residual);
					OnDepleted?.Invoke(this);

					return Size;
				}
				else
				{
					return 0;
				}
			}
		}
	}
}