using System;
using System.Collections;
using System.Collections.Generic;
using UIBinding.Base;
using UnityEngine;
using UnityEngine.UI;

namespace PM.UsefulThings
{
	[DisallowMultipleComponent]
	public class AbPanel : BaseBindingBehaviourTarget, IWindowUT
	{
		public virtual bool IsSolid
		{
			get
			{
				return false;
			}
		}

		public virtual bool CanHaveChildren
		{
			get
			{
				return true;
			}
		}

		public virtual bool IsInteractable { get; set; }
		public virtual bool IsFocused { get; set; }

		public List<IWindowUT> Children { get; } = new List<IWindowUT>();

#pragma warning disable 67 // Add readonly modifier
		public event Action<IWindowUT> OnClosed;
		public event Action<IWindowUT, bool> OnSetInteractable;
		public event Action<IWindowUT, bool> OnSetFocus;
		public event Action<IWindowUT, IWindowUT> OnChildAdded;
#pragma warning restore 67

		public virtual IWindowUT AddChild(IWindowUT newChild)
		{
			Children.Add(newChild);
			OnChildAdded?.Invoke(this, newChild);
			return this;
		}

		public virtual void Close()
		{
			OnClosed?.Invoke(this);
			Destroy(this.gameObject);
		}

		public IWindowUT Init(bool isInteractable = true, bool isFocused = true)
		{
			this.IsInteractable = isInteractable;
			this.IsFocused = isFocused;
			return this;
		}
	}
}