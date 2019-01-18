using PM.UsefulThings.Extensions;
using System;

namespace UIBinding.Base
{
	public abstract class BaseProperty
	{
		public event Action OnValueChanged;

		public string instanceName { get { return m_instanceName; } }

		private string m_instanceName = "";

		protected void ValueChanged()
		{
			OnValueChanged.Call();
		}
	}

	public abstract class Property : BaseProperty
	{
		public abstract string ValueToString();
	}

	public abstract class Property<T> : Property
	{
		public T value { get { return GetValue(); } set { SetValue(value); } }

		private T m_value;
		private Type m_type;
		private bool m_changing;
		private readonly bool m_isValue;

		public Property(T startValue = default(T))
		{
			m_value = startValue;
			m_type = typeof(T);
			m_isValue = m_type.IsValueType;
		}

		protected virtual void SetValue(T value)
		{
			if (m_changing)
			{
				return;
			}
			m_changing = true;

			var changed = false;
			if (m_isValue)
			{
				changed = IsValueDifferent(value);
			}
			else
			{
				changed = (value == null && m_value != null)
						|| (value != null && m_value == null)
						|| ((m_value != null) && IsClassDifferent(value));
			}

			if (changed)
			{
				m_value = value;
				ValueChanged();
			}
			m_changing = false;
		}

		protected virtual T GetValue()
		{
			return m_value;
		}

		protected virtual bool IsValueDifferent(T value)
		{
			return !m_value.Equals(value);
		}

		private bool IsClassDifferent(T value)
		{
			return !m_value.Equals(value);
		}

		public override string ValueToString()
		{
			return value == null ? null : value.ToString();
		}
	}
}