using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings
{
	public class StateMachine<T> where T : struct, IConvertible
	{
		public event Action<T, T> OnStateFinish;
		public event Action<T, T> OnStateStart;

		public T CurrentState { get; private set; }

		public StateMachine()
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException("T must be an enumerated type");
			}
		}

		public bool SwitchState(T newState)
		{
			OnStateFinish?.Invoke(CurrentState, newState);
			CurrentState = newState;
			OnStateStart?.Invoke(CurrentState, newState);
			return true;
		}
	}
}