using UIBinding.Base;

namespace UIBinding
{
	public class IntProperty : Property<int>
	{
		public IntProperty() : base() { }
		public IntProperty(int startValue = 0) : base(startValue) { }

		protected override bool IsValueDifferent(int value)
		{
			return GetValue() != value;
		}
	}
}