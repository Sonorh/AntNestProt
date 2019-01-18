using UIBinding.Base;
using System.Collections;
using UnityEngine;

namespace UIBinding.Components
{
	public class TransformBinding : BaseBinding<TransformProperty>
	{
		protected override void Start()
		{
			base.Start();
			property.value = transform;
		}
	}
}