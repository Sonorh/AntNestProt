using UnityEngine;

using UIBinding.Base;

namespace UIBinding.Components
{
	public class RotationAxisBinding : BaseBinding<FloatProperty>
	{
		public enum AxisType
		{
			X,
			Y,
			Z
		}

		[SerializeField]
		private AxisType m_axis = AxisType.X;
		[SerializeField]
		private bool m_local = false;

		protected override void OnUpdateValue()
		{
			var eulerAngles = transform.eulerAngles;
			switch (m_axis)
			{
				case AxisType.X:
					SetAngle(property.value, eulerAngles.y, eulerAngles.z);
					break;
				case AxisType.Y:
					SetAngle(eulerAngles.x, property.value, eulerAngles.z);
					break;
				case AxisType.Z:
					SetAngle(eulerAngles.x, eulerAngles.y, property.value);
					break;
			}
		}

		private void SetAngle(float x, float y, float z)
		{
			var angle = new Vector3(x, y, z);
			if (m_local)
			{
				transform.localEulerAngles = angle;
			}
			else
			{
				transform.eulerAngles = angle;
			}
		}
	}
}