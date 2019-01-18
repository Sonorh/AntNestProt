using UnityEngine;

using UIBinding.Base;

namespace UIBinding
{
	public class TransformProperty : Property<Transform>
	{
		public TransformProperty() : base() { }
		public TransformProperty(Transform startValue = null) : base(startValue) { }
	}
}