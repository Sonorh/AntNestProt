using System;

using UIBinding.Base;

namespace UIBinding.Elements
{
	public abstract class BaseListElement : BaseBindingBehaviourTarget, IComparable<BaseListElement>
	{
		public BaseListElementData data { get { return m_data; } }

		private BaseListElementData m_data;

		public void Init(BaseListElementData data)
		{
			OnDataChange(data);

			m_data = data;

			OnInit();
			ForceUpdateProperties();
		}

		protected virtual void OnInit() { }
		protected virtual void OnDataChange(BaseListElementData newData) { }

		public int CompareTo(BaseListElement other)
		{
			return m_data.Sort.CompareTo(other.m_data.Sort);
		}

		public virtual void ButtonClickHandler()
		{
			data.ClickFromUI();
		}

		protected virtual void OnDestroy()
		{
			data.OnDestroy();
		}
	}
}