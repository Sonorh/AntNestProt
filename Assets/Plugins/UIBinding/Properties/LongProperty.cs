using UIBinding.Base;

namespace UIBinding
{
	public class LongProperty : Property<long>
	{
		public LongProperty() : base() { }
		public LongProperty(int startValue = 0) : base(startValue) { }

		protected override bool IsValueDifferent(long value)
		{
			return GetValue() != value;
		}
	}
}
