using PM.UsefulThings.Extensions;
using UnityEngine;

namespace UIBinding.Base
{
	public abstract class BaseBinding : MonoBehaviour
	{
		protected BaseProperty property { get { return m_property; } }
		protected IBindingTarget target { get { return m_target; } }

		[SerializeField]
		private string m_path = "";

		private BaseProperty m_property;
		private IBindingTarget m_target;
		private string m_propertyName;

		protected virtual void Start()
		{
			Init();
			Bind(true);
			UpdateValue();
		}

		private void OnDestroy()
		{
			Unbind(true);
		}

		protected void Init()
		{
			m_propertyName = m_path;
			FindTarget();
		}

		protected virtual void OnValueChanged()
		{
			UpdateValue();
		}

		protected virtual void OnForceUpdate()
		{
			Unbind();
			UpdateProperty();
			Bind();
			UpdateValue();
		}

		protected void Bind(bool total = false)
		{
			if (m_property != null)
			{
				m_property.OnValueChanged += OnValueChanged;

				if (total)
				{
					m_target.OnForceUpdateProperties += OnForceUpdate;
				}
			}
		}

		protected void Unbind(bool total = false)
		{
			if (m_property != null)
			{
				m_property.OnValueChanged -= OnValueChanged;

				if (total)
				{
					m_target.OnForceUpdateProperties -= OnForceUpdate;
				}
			}
		}

		protected void UpdateProperty()
		{
			if (m_target != null)
			{
				var properties = m_target.GetProperties();
				m_property = FindProperty(properties);
			}
		}

		protected void UpdateValue()
		{
			if (m_property != null)
			{
				OnUpdateValue();
			}
		}

		protected virtual void OnUpdateValue() { }

		private void FindTarget()
		{
			var parent = transform;
			while (parent != null)
			{
				var component = parent.GetComponent<IBindingTarget>();
				if (component != null)
				{
					var properties = component.GetProperties();
					var property = FindProperty(properties);
					if (IsPropertyValid(property))
					{
						m_property = property;
						m_target = component;
						return;
					}
				}
				parent = parent.parent;
			}
#if UNITY_EDITOR
			Debug.LogErrorFormat("[Binding] BindingTarget wasn't found for \"{0}\" by path \"{1}\" \nTransform: \"{2}\"", GetType().Name, m_path, transform.GetFullPath());
#endif
		}

		private BaseProperty FindProperty(BaseProperty[] properties)
		{
			foreach (var property in properties)
			{
				if (property.instanceName == m_propertyName)
				{
					return property;
				}
			}
			return null;
		}

		protected virtual bool IsPropertyValid(BaseProperty property)
		{
			return property != null;
		}
	}

	public abstract class BaseBinding<T> : BaseBinding where T : BaseProperty
	{
		protected new T property { get { return base.property as T; } }

		protected override bool IsPropertyValid(BaseProperty property)
		{
			return (property != null) && (property is T);
		}
	}
}