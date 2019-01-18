using UnityEngine;
using UnityEngine.UI;

namespace PM.UsefulThings.Extensions
{
	public static class ColorExtensions
	{
		public static Color ChangeAlpha(this Color color, float alpha)
		{
			color.a = alpha;
			return color;
		}

		public static bool EqualsOpaque(this Color color, Color other)
		{
			return (color.r == other.r) && (color.g == other.g) && (color.b == other.b);
		}

		public static string ToHexString(this Color color, bool withGrid = true)
		{
			var colorString = ColorUtility.ToHtmlStringRGBA(color);
			return withGrid ? "#" + colorString : colorString;
		}
	}

	public static class GraphicColorSetters
	{
		public static void SetAlpha(this Graphic graphic, float alpha)
		{
			var color = graphic.color;
			color.a = alpha;
			graphic.color = color;
		}

		public static void SetColorOnly(this Graphic graphic, Color color)
		{
			color.a = graphic.color.a;
			graphic.color = color;
		}
	}

	public static class SpriteRendererColorSetters
	{
		public static void SetAlpha(this SpriteRenderer renderer, float alpha)
		{
			var color = renderer.color;
			color.a = alpha;
			renderer.color = color;
		}

		public static void SetColorOnly(this SpriteRenderer renderer, Color color)
		{
			color.a = renderer.color.a;
			renderer.color = color;
		}
	}

	public static class StringColorFormatter
	{
		private const string COLORED_TEXT_FORMAT_WITH_GRID = "<color=#{1}>{0}</color>";
		private const string COLORED_TEXT_FORMAT = "<color={1}>{0}</color>";

		public static void SetColor(this string text, string color)
		{
			text = GetColored(text, color);
		}

		public static void SetColor(this string text, Color color)
		{
			text = GetColored(text, color);
		}

		public static string GetColored(object obj, string color)
		{
			var formatString = color.Contains("#") ? COLORED_TEXT_FORMAT : COLORED_TEXT_FORMAT_WITH_GRID;
			return string.Format(formatString, obj, color);
		}

		public static string GetColored(object obj, Color color)
		{
			var colorString = color.ToHexString();
			return string.Format(COLORED_TEXT_FORMAT, obj, colorString);
		}
	}
}