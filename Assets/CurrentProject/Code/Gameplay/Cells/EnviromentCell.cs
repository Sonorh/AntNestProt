using PM.UsefulThings.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	public class EnviromentCell : AbCell
	{
		public new EnviromentCellData Data { get; protected set; }
		
		public override void Reset(AbCellData data)
		{
			base.Reset(data);

			this.Data = data as EnviromentCellData;

			UpdateInfo();
		}

		private void OnEnable()
		{
			CellSource.OnGathered += OnCellSourceGatheredHandler;
		}

		private void OnDisable()
		{
			CellSource.OnGathered -= OnCellSourceGatheredHandler;
		}

		private void OnCellSourceGatheredHandler(CellSource source)
		{
			if (Data.Source == source)
			{
				UpdateInfo();
			}
		}

		public override void UpdateInfo(AbCellData data = null)
		{
			if (data != null && data != this.Data)
			{
				return;
			}

			if (this.Data.isExplored)
			{
				if (Data.IsBugged)
				{
					avatarColor.value = new Color32(200, 39, 39, 255);
					state.value = Data.Bug.ToString().Localized();
					value.value = "Count".Localized() + ": " + Data.BugCount;
				}
				else if (Data.IsGatherable)
				{
					switch (Data.Source.Type)
					{
						case SourceTypes.FlimsyMeat:
							state.value = "Meat".Localized();
							value.value = Data.Source.Residual.ToString();
							avatarColor.value = new Color32(210, 210, 5, 255);
							break;
						case SourceTypes.FlimsyFruit:
							state.value = "Fruits".Localized();
							value.value = Data.Source.Residual.ToString();
							avatarColor.value = new Color32(17, 97, 17, 255);
							break;
						case SourceTypes.SolidMeat:
							state.value = "Meat".Localized();
							value.value = "[" + (Data.Source.Size - Data.Source.Gathered) + "/" + Data.Source.Size + "]";
							avatarColor.value = new Color32(236, 150, 10, 255);
							break;
						case SourceTypes.SolidFruit:
							state.value = "Fruits".Localized();
							value.value = "[" + (Data.Source.Size - Data.Source.Gathered) + "/" + Data.Source.Size + "]";
							avatarColor.value = new Color32(30, 200, 30, 255);
							break;
						default:
							break;
					}
				}
				else
				{
					avatarColor.value = Color.white;
					state.value = "Empty".Localized();
					value.value = string.Empty;
				}
			}
			else
			{
				avatarColor.value = new Color32(200, 200, 200, 255);
				state.value = "Explore".Localized();
				value.value = string.Empty;
			}
		}
	}
}