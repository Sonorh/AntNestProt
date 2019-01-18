using UnityEngine;

using UIBinding.Base;

namespace UIBinding
{
	public class FloatProperty : Property<float>
	{
		public FloatProperty() : base() { }
		public FloatProperty(float startValue = 0f) : base(startValue) { }

		protected override bool IsValueDifferent(float value)
		{
			return Mathf.Abs(GetValue() - value) > float.Epsilon;
		}
	}
}