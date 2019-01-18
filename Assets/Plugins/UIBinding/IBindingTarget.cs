using System;

namespace UIBinding.Base
{
	public interface IBindingTarget
	{
		event Action OnForceUpdateProperties;

		BaseProperty[] GetProperties();
	}
}