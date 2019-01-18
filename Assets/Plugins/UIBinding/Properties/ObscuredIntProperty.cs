using UIBinding.Base;
using CodeStage.AntiCheat.ObscuredTypes;

namespace UIBinding
{
	public class ObscuredIntProperty : Property<ObscuredInt>
	{
		public ObscuredIntProperty() : base() { }
		public ObscuredIntProperty(int startValue = 0) : base(startValue) { }

		protected override bool IsValueDifferent(ObscuredInt value)
		{
			return !GetValue().Equals(value);
		}
	}
}