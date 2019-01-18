using UnityEngine;
using System.Collections;
using PM.UsefulThings.Extensions;

namespace UIBinding.Elements
{
	[RequireComponent(typeof(Animator))]
	public abstract class BaseAnimatedListElement : BaseListElement
	{
		private readonly int ShowTrigger = Animator.StringToHash("Show");
		private readonly int HideTrigger = Animator.StringToHash("Hide");
		private readonly int AwaitShowState = Animator.StringToHash("AwaitShow");

		public bool showInProgress { get; private set; }
		public bool hideInProgress { get; private set; }

		private Animator m_animator;
		private float m_showDelay;
		private float m_hideDelay;

		private void Awake()
		{
			m_animator = GetComponent<Animator>();
			m_showDelay = 0f;
			m_hideDelay = 0f;
		}

		protected override void OnDestroy()
		{
			showInProgress = false;
			hideInProgress = false;

			StopAllCoroutines();

			base.OnDestroy();
		}

		public void Show()
		{
			if (m_animator.HasParameter(ShowTrigger) && !showInProgress)
			{
				StartCoroutine(ShowCoroutine());
			}
		}

		public void Hide()
		{
			if (m_animator.HasParameter(HideTrigger) && !hideInProgress)
			{
				StartCoroutine(HideCoroutine());
			}
		}

		public void HideInstant()
		{
			showInProgress = false;
			hideInProgress = false;

			StopAllCoroutines();

			m_animator.enabled = true;
			m_animator.Play(AwaitShowState, 0, 1f);
			m_animator.Update(0f);
			m_animator.enabled = false;
		}

		public void SetDelays(float showDelay, float hideDelay)
		{
			m_showDelay = showDelay;
			m_hideDelay = hideDelay;
		}

		private IEnumerator ShowCoroutine()
		{
			showInProgress = true;

			yield return new WaitForSecondsRealtime(m_showDelay);

			m_animator.enabled = true;

			var oldStateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
			m_animator.SetTrigger(ShowTrigger);

			var stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
			while (stateInfo.fullPathHash == oldStateInfo.fullPathHash) // Ожидаем переключения стейта по триггеру.
			{
				stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
				yield return null;
			}
			while ((stateInfo.length > 0f) && (stateInfo.normalizedTime < 1f))
			{
				stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
				yield return null;
			}

			m_animator.enabled = false;

			showInProgress = false;
		}

		private IEnumerator HideCoroutine()
		{
			hideInProgress = true;

			while (showInProgress)
			{
				yield return null;
			}

			yield return new WaitForSecondsRealtime(m_hideDelay);

			m_animator.enabled = true;

			var oldStateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
			m_animator.SetTrigger(HideTrigger);

			var stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
			while (stateInfo.fullPathHash == oldStateInfo.fullPathHash) // Ожидаем переключения стейта по триггеру.
			{
				stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
				yield return null;
			}
			while ((stateInfo.length > 0f) && (stateInfo.normalizedTime < 1f))
			{
				stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
				yield return null;
			}

			m_animator.enabled = false;

			hideInProgress = false;
		}
	}
}