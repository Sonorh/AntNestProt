using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings.Observer
{
	public class Observable<T>
	{
		public event System.Action<T> OnValueChanged;

		private T _value;
		public T Value
		{
			get
			{
				return _value; 
			}
			set
			{
				_value = value;
				OnValueChanged?.Invoke(_value);
			}
		}
		 
		public static explicit operator T(Observable<T> obj)
		{
			return obj.Value;
		}
	}

}