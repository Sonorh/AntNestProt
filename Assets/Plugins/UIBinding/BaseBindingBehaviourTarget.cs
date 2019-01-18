using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using PM.UsefulThings.Extensions;

namespace UIBinding.Base
{
	public abstract class BaseBindingBehaviourTarget : MonoBehaviour, IBindingTarget
	{
		private const string INSTANCE_FIELD_NAME = "m_instanceName";

		protected List<FieldInfo> m_bindingFieldInfos = new List<FieldInfo>();
		protected List<BaseProperty> m_properties = new List<BaseProperty>();

		public event Action OnForceUpdateProperties;

		public BaseBindingBehaviourTarget()
		{
			CollectProperties();
		}

		public BaseProperty[] GetProperties()
		{
			if (m_bindingFieldInfos.Count > 0)
			{
				var includedProperties = GetPropertiesFromBindingFields();
				return m_properties.Union(includedProperties).ToArray();
			}
			else
			{
				return m_properties.ToArray();
			}
		}

		protected void ForceUpdateProperties()
		{
			OnForceUpdateProperties.SafeCall();
		}

		private void CollectProperties()
		{
			var interfaceName = typeof(IBindingTarget).Name;
			var propertyType = typeof(BaseProperty);
			GetType().UntilType<BaseBindingBehaviourTarget>((type) =>
			{
				var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
				foreach (var fieldInfo in fieldInfos)
				{
					var fieldType = fieldInfo.FieldType;
					if ((fieldType.GetInterface(interfaceName) != null) && !m_bindingFieldInfos.Contains(fieldInfo))
					{
						m_bindingFieldInfos.Add(fieldInfo);
					}
					else
					{
						fieldType = fieldType.GetInitialType();
						if (fieldType == propertyType)
						{
							var property = fieldInfo.GetValue(this) as BaseProperty;
							if (property != null && !m_properties.Contains(property))
							{
								SetPropertyInstanceName(fieldType, fieldInfo, property);
								m_properties.Add(property);
							}
						}
					}
				}
			});
		}

		private void SetPropertyInstanceName(Type propertyType, FieldInfo propertyField, BaseProperty property)
		{
			var fieldInfos = propertyType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
			foreach (var fieldInfo in fieldInfos)
			{
				if (fieldInfo.Name == INSTANCE_FIELD_NAME)
				{
					fieldInfo.SetValue(property, propertyField.Name);
				}
			}
		}

		protected virtual List<BaseProperty> GetPropertiesFromBindingFields()
		{
			var result = new List<BaseProperty>();
			foreach (var fieldInfo in m_bindingFieldInfos)
			{
				var value = fieldInfo.GetValue(this) as IBindingTarget;
				if (value != null)
				{
					result.AddRange(value.GetProperties());
				}
			}
			return result;
		}
	}
}