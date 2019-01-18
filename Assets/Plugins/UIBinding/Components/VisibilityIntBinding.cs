using UnityEngine;
using System.Collections.Generic;

using UIBinding.Base;

namespace UIBinding.Components
{
	public class VisibilityIntBinding : BaseBinding<IntProperty>
	{
		public enum CheckType
		{
			Equal,
			Greater,
			Less,
			InRange
		}

		[SerializeField]
		private CheckType m_checkType = CheckType.Equal;
		[SerializeField]
		private string m_value = "0";
		[SerializeField]
		private bool m_invert = false;

		private List<int> m_values = new List<int>();

		private void Awake()
		{
			ParseValues();
		}

		protected override void OnUpdateValue()
		{
			var visible = false;
			switch (m_checkType)
			{
				case CheckType.Equal:
					{
						var propertyValue = property.value;
						foreach (var value in m_values)
						{
							if (propertyValue == value)
							{
								visible = true;
								break;
							}
						}
					}
					break;
				case CheckType.Greater:
					{
						var value = m_values[0];
						visible = property.value > value;
					}
					break;
				case CheckType.Less:
					{
						var value = m_values[0];
						visible = property.value < value;
					}
					break;
				case CheckType.InRange:
					{
						var minValue = m_values[0];
						var maxValue = m_values[1];
						visible = (property.value >= minValue) && (property.value <= maxValue);
					}
					break;
			}
			gameObject.SetActive(m_invert ? !visible : visible);
		}

		private void ParseValues()
		{
			m_values.Clear();

			switch (m_checkType)
			{
				case CheckType.Equal:
					{
						var trimmedValue = m_value.Trim();
						var stringValues = trimmedValue.Split(',');
						foreach (var stringValue in stringValues)
						{
							AddValue(stringValue);
						}
					}
					break;
				case CheckType.Greater:
				case CheckType.Less:
					{
						AddValue(m_value);
					}
					break;
				case CheckType.InRange:
					{
						var trimmedValue = m_value.Trim();
						var stringValues = trimmedValue.Split(',');
						if (stringValues.Length < 2)
						{
#if UNITY_EDITOR
							Debug.LogErrorFormat("Couldn't parse \"{0}\" to \"{1}\" check type", m_value, m_checkType.ToString());
#endif
							return;
						}
						for (int i = 0; i < 2; i++)
						{
							var stringValue = stringValues[i];
							AddValue(stringValue);
						}
					}
					break;
			}
		}

		private void AddValue(string stringValue)
		{
			var value = 0;
			if (int.TryParse(stringValue, out value))
			{
				m_values.Add(value);
			}
#if UNITY_EDITOR
			else
			{
				Debug.LogErrorFormat("Couldn't parse \"{0}\" to {1} type", m_value, m_checkType.ToString());
			}
#endif
		}
	}
}