using UnityEngine;
using UnityEngine.UI;

namespace PM.UsefulThings.Extensions
{
	public static class RectTransformExtensions
	{
		public static void SetDefaultScale(this RectTransform trans)
		{
			trans.localScale = Vector3.one;
		}

		public static void SetPivotAndAnchors(this RectTransform trans, Vector2 aVec)
		{
			trans.pivot = aVec;
			trans.anchorMin = aVec;
			trans.anchorMax = aVec;
		}

		public static Vector2 GetSize(this RectTransform trans)
		{
			return trans.rect.size;
		}

		public static float GetWidth(this RectTransform trans)
		{
			return trans.rect.width;
		}

		public static float GetHeight(this RectTransform trans)
		{
			return trans.rect.height;
		}

		public static void SetPositionOfPivot(this RectTransform trans, Vector2 newPos)
		{
			trans.localPosition = new Vector3(newPos.x, newPos.y, trans.localPosition.z);
		}

		public static void SetLeftBottomPosition(this RectTransform trans, Vector2 newPos)
		{
			trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
		}

		public static void SetLeftTopPosition(this RectTransform trans, Vector2 newPos)
		{
			trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
		}

		public static void SetRightBottomPosition(this RectTransform trans, Vector2 newPos)
		{
			trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
		}

		public static void SetRightTopPosition(this RectTransform trans, Vector2 newPos)
		{
			trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
		}

		public static void SetSize(this RectTransform trans, Vector2 newSize)
		{
			Vector2 oldSize = trans.rect.size;
			Vector2 deltaSize = newSize - oldSize;
			trans.offsetMin = trans.offsetMin - new Vector2(deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
			trans.offsetMax = trans.offsetMax + new Vector2(deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y));
		}

		public static void SetWidth(this RectTransform trans, float newSize)
		{
			SetSize(trans, new Vector2(newSize, trans.rect.size.y));
		}

		public static void SetHeight(this RectTransform trans, float newSize)
		{
			SetSize(trans, new Vector2(trans.rect.size.x, newSize));
		}

		public static Rect GetScreenRect(this RectTransform rectTransform)
		{
			//DONT CALL FROM AWAKE!!!

			Vector3[] corners = new Vector3[4];

			rectTransform.GetWorldCorners(corners);

			float xMin = float.PositiveInfinity;
			float xMax = float.NegativeInfinity;
			float yMin = float.PositiveInfinity;
			float yMax = float.NegativeInfinity;

			for (int i = 0; i < 4; i++)
			{
				// For Canvas mode Screen Space - Overlay there is no Camera; best solution I've found
				// is to use RectTransformUtility.WorldToScreenPoint) with a null camera.

				Vector3 screenCoord = RectTransformUtility.WorldToScreenPoint(null, corners[i]);

				if (screenCoord.x < xMin)
				{
					xMin = screenCoord.x;
				}
				if (screenCoord.x > xMax)
				{
					xMax = screenCoord.x;
				}
				if (screenCoord.y < yMin)
				{
					yMin = screenCoord.y;
				}
				if (screenCoord.y > yMax)
				{
					yMax = screenCoord.y;
				}
			}

			Rect result = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);

			return result;
		}
	}
}