using UnityEngine;
using System.Collections.Generic;

namespace UIBinding.Base
{
	/*public abstract class BaseMultiBinding : MonoBehaviour
	{
		protected BaseProperty[] properties { get { return m_propertiesArray; } }
		protected IBindingTarget target { get { return m_target; } }

		[SerializeField]
		private string[] m_paths;

		private List<BaseProperty> m_properties;
		private BaseProperty[] m_propertiesArray;
		private IBindingTarget m_target;
		private string[] m_propertyNames;

		protected virtual void Start()
		{
			Init();
			Bind();
		}

		private void OnDestroy()
		{
			Unbind();
		}

		protected void Init()
		{

			FindTarget();
		}

		protected abstract void OnValueChanged(BaseProperty property);

		protected virtual void OnForceUpdate()
		{
			Unbind();
			UpdateProperty();
			Bind();
		}

		protected void Bind()
		{
			if (m_target != null)
			{
				m_properties.ForEach((property) => property.OnValueChanged += OnValueChanged);
				m_target.OnForceUpdateProperties += OnForceUpdate;
			}
		}

		protected void Unbind()
		{
			if (m_target != null)
			{
				m_properties.ForEach((property) => property.OnValueChanged -= OnValueChanged);
				m_target.OnForceUpdateProperties -= OnForceUpdate;
			}
		}

		protected void UpdateProperty()
		{
			if (m_target != null)
			{
				var properties = m_target.GetProperties();
				m_property = FindProperties(properties);
			}
		}

		private void FillNames()
		{
			m_propertyNames = new string[m_paths.Length];
			for (int i = 0; i < m_propertyNames.Length; i++)
			{
				m_propertyNames[i] = m_paths[i];
			}
		}

		private void FindTarget()
		{
			var parent = transform;
			while (parent.parent != null)
			{
				var component = parent.GetComponent<IBindingTarget>();
				if (component != null)
				{
					var properties = component.GetProperties();
					properties = FindProperties(properties);
					if (IsPropertyValid(property))
					{
						m_properties.Add(property);
						m_target = component;
						return;
					}
				}
				parent = parent.parent;
			}
#if UNITY_EDITOR
			Debug.LogErrorFormat("[Binding] BindingTarget wasn't found for \"{0}\" by path \"{1}\"!", GetType().Name, m_path);
#endif
		}

		private bool ContainsPropertyName(string propertyName)
		{
			for (int i = 0; i < m_propertyNames.Length; i++)
			{
				if (m_propertyNames[i] == propertyName)
				{
					return true;
				}
			}
			return false;
		}

		private List<BaseProperty> FindProperties(BaseProperty[] properties)
		{
			var findedProperties = new List<BaseProperty>();
			foreach (var property in properties)
			{
				if (ContainsPropertyName(property.instanceName))
				{
					findedProperties.Add(property);
				}
			}
			return findedProperties;
		}

		protected virtual bool IsPropertyValid(BaseProperty property)
		{
			return property != null;
		}
	}*/
}