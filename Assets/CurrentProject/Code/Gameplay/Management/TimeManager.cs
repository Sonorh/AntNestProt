using PM.UsefulThings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	public class TimeManager : Singleton<TimeManager>
	{
		public event System.Action OnStartTurn;
		public event System.Action OnEndOfTurn;
		public event System.Action OnTimeChanged;

		public int Year { get; private set; } = System.DateTime.Now.Year;
		public Weeks Now { get; private set; } = Weeks.JanuaryFirst;

		public bool IsWinter
		{
			get
			{
				return (int)Now <= (int)Weeks.FebruaryFourth || (int)Now >= (int)Weeks.DecemberFirst;
			}
		}

		public void Next()
		{
			OnEndOfTurn?.Invoke();
			if (Now == Weeks.DecemberFourth)
			{
				Year++;
				Now = Weeks.JanuaryFirst;
			}
			else
			{
				Now++;
			}
			OnTimeChanged?.Invoke();
			OnStartTurn?.Invoke();
		}

		public void ScrollTo(Weeks week, int year = -1)
		{
			if (year != -1)
			{
				this.Year = year;
			}
			else if ((int)week < (int)Now)
			{
				this.Year++;
			}

			Now = week;

			OnTimeChanged?.Invoke();
		}

		public Weeks GetTimeAfter(int weeks)
		{
			int result = (int)Now;
			result += weeks;
			result %= System.Enum.GetValues(typeof(Weeks)).Length;

			return (Weeks)result;
		}
	}
}