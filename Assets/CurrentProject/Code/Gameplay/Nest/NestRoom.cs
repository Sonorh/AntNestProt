using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PM.Antnest.Gameplay
{
	public class NestRoom : MonoBehaviour
	{
		private AbNestRoomData _data;
		public AbNestRoomData Data
		{
			get
			{
				return _data;
			}
			set
			{
				_data = value;
				OnDataChanged();
			}
		}

		private void OnDataChanged()
		{
			(this.transform as RectTransform).anchoredPosition = new Vector2(400 * _data.Position.x, -200 * _data.Position.y);

			var image = this.GetComponent<Image>();
			switch (Data.Type)
			{
				case RoomTypes.Entrance:
					image.color = new Color32(51,23,23,255);
					break;
				case RoomTypes.BuildingSpot:
					image.color = new Color32(32, 22, 22, 255);
					break;
				case RoomTypes.Construction:
					image.color = new Color32(11, 11, 11, 255);
					break;
				case RoomTypes.Warehause:
					if (Data.IsUpgrading)
					{
						image.color = new Color32(38, 40, 27, 255);
					}
					else
					{
						image.color = new Color32(38, 60, 27, 255);
					}
					break;
				case RoomTypes.Nursery:
					if (Data.IsUpgrading)
					{
						image.color = new Color32(36, 37, 40, 255);
					}
					else
					{
						image.color = new Color32(36, 37, 70, 255);
					}
					break;
				case RoomTypes.Armory:
					image.color = new Color32(70, 36, 36, 255);
					break;
				default:
					break;
			}
		}

		public void OnMouseClickHandler()
		{
			Data.OnMouseClickHandler();
		}
	}
}