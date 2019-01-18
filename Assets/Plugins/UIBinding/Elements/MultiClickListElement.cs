using UIBinding.Data;


namespace UIBinding.Elements
{
	public class MultiClickListElement : BaseListElement
	{
		private MultiClickListElementData m_data = null;


		protected override void OnInit()
		{
			m_data = data as MultiClickListElementData;
		}

		public virtual void Button1ClickHandler()
		{
			if (m_data != null)
			{
				m_data.Click1();
			}
		}

		public virtual void Button2ClickHandler()
		{
			if (m_data != null)
			{
				m_data.Click2();
			}
		}

		public virtual void Button3ClickHandler()
		{
			if (m_data != null)
			{
				m_data.Click3();
			}
		}

		public virtual void Button4ClickHandler()
		{
			if (m_data != null)
			{
				m_data.Click4();
			}
		}

		public virtual void Button5ClickHandler()
		{
			if (m_data != null)
			{
				m_data.Click5();
			}
		}
	}
}