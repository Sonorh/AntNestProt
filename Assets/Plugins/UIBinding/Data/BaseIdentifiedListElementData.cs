namespace UIBinding
{
	public abstract class BaseIdentifiedListElementData : BaseListElementData
	{
		public virtual int id { get { return m_id; } set { m_id = value; } }

		protected int m_id;
	}
}