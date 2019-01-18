using UIBinding.Base;
using UnityEngine;

namespace UIBinding
{
	public class ColorStrStr : BaseBindingTarget
	{
		public Color color { get; set; }
		public string str1 { get; set; }
		public string str2 { get; set; }
	}

	public class ColorStrStrProperty : Property<ColorStrStr>
	{
		public ColorStrStrProperty() : base() { }
		public ColorStrStrProperty(ColorStrStr startValue) : base(startValue) { }

		protected override bool IsValueDifferent(ColorStrStr other)
		{
			var value = GetValue();

			bool result = false;
			result |= (value.color != other.color);
			result |= (value.str1 != other.str1);
			result |= (value.str2 != other.str2);

			return result;
		}
	}
}
