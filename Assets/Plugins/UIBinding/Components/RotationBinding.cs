using UIBinding.Base;

namespace UIBinding.Components
{
	public class RotationBinding : BaseBinding<QuaternionProperty>
	{
		protected override void OnUpdateValue()
		{
			transform.rotation = property.value;
		}
	}
}