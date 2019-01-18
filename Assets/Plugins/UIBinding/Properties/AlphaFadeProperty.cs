using PM.UsefulThings.Extensions;
using System;

namespace UIBinding
{
	public class AlphaFadeProperty : FloatProperty
	{
		public event Action OnFadeFinish;

		public float fadeTime { get; set; }
		public bool force { get; private set; }

		public AlphaFadeProperty() : base()
		{
			fadeTime = 1f;
		}
		public AlphaFadeProperty(float startFadeTime = 1f) : base()
		{
			fadeTime = startFadeTime;
		}
		public AlphaFadeProperty(float startValue = 0f, float startFadeTime = 1f) : base(startValue)
		{
			fadeTime = startFadeTime;
		}

		public void SetForce(float alpha)
		{
			force = true;
			value = alpha;
			force = false;
		}

		public void Finish()
		{
			OnFadeFinish.Call();
		}
	}
}