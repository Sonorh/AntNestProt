using PM.UsefulThings.Extensions;
using System;


namespace UIBinding.Data
{
	public class MultiClickListElementData : BaseListElementData
	{
		public event Action<MultiClickListElementData> OnClick1;
		public event Action<MultiClickListElementData> OnClick2;
		public event Action<MultiClickListElementData> OnClick3;
		public event Action<MultiClickListElementData> OnClick4;
		public event Action<MultiClickListElementData> OnClick5;


		public void Click1()
		{
			OnClick1.Call(this);
		}

		public void Click2()
		{
			OnClick2.Call(this);
		}

		public void Click3()
		{
			OnClick3.Call(this);
		}

		public void Click4()
		{
			OnClick4.Call(this);
		}

		public void Click5()
		{
			OnClick5.Call(this);
		}
	}
}