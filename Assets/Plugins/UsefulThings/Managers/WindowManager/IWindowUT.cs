using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings
{
	public interface IWindowUT
	{
		bool IsSolid { get; }
		bool CanHaveChildren { get; }
		bool IsInteractable { get; set; }
		bool IsFocused { get; set; }
		List<IWindowUT> Children { get; }

		//first argument is sender
		event Action<IWindowUT> OnClosed;
		event Action<IWindowUT, bool> OnSetInteractable;
		event Action<IWindowUT, bool> OnSetFocus;
		event Action<IWindowUT, IWindowUT> OnChildAdded;

		IWindowUT Init(bool isInteractable = true, bool isFocused = true);
		void Close();
		IWindowUT AddChild(IWindowUT newChild);
	}
}