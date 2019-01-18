using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings
{
	public interface IWindowManagerUT
	{
		IWindowUT ActiveFrame { get; }
		IWindowUT FrameWithFocus { get; }
		IWindowUT OpenNewFrame(IWindowUT newWindow, WindowCloseModes mode = WindowCloseModes.CloseNonSolid, bool isSolid = false);
		IWindowUT AddChildToActiveFrame(IWindowUT newChild);
	}
}