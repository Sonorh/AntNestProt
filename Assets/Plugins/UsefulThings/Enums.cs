using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings
{
    public enum Sides2D
    {
        Right,
        Top,
        Left,
        Bottom
    }

    public enum Directions2D
    {
        Right,
        Up,
        Left,
        Down
    }


    public enum NonableDirections2D
    {
        None = 0,
        Right = 1,
        Up = 2,
        Left = 4,
        Down = 8
    }

	public enum WindowCloseModes
	{
		CloseNothing,
		CloseNonSolid,
		CloseEverything
	}
}