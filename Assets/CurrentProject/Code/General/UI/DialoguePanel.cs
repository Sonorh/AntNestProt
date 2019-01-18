using PM.UsefulThings;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PM.Antnest.General
{
	public class DialoguePanel : AbPanel
	{
		[SerializeField]
		private Button firstButton;
		[SerializeField]
		private Button secondButton;
		[SerializeField]
		private Button thirdButton;

		private Action firstAction;
		private Action secondAction;
		private Action thirdAction;

		public void Init(string firstButton, Action firstAction, string secondButton = null, Action secondAction = null, string thirdButton = null, Action thirdAction = null)
		{
			if (!string.IsNullOrEmpty(firstButton))
			{
				this.firstButton.transform.GetChild(0).GetComponent<Text>().text = firstButton;
				this.firstAction = firstAction;
			}
			else
			{
				this.firstButton.gameObject.SetActive(false);
			}
			if (!string.IsNullOrEmpty(secondButton))
			{
				this.secondButton.transform.GetChild(0).GetComponent<Text>().text = secondButton;
				this.secondAction = secondAction;
			}
			else
			{
				this.secondButton.gameObject.SetActive(false);
			}
			if (!string.IsNullOrEmpty(thirdButton))
			{
				this.thirdButton.transform.GetChild(0).GetComponent<Text>().text = thirdButton;
				this.thirdAction = thirdAction;
			}
			else
			{
				this.thirdButton.gameObject.SetActive(false);
			}
		}

		public void OnFirstButtonClick()
		{
			firstAction?.Invoke();
		}

		public void OnSecondButtonClick()
		{
			secondAction?.Invoke();
		}

		public void OnThirdButtonClick()
		{
			thirdAction?.Invoke();
		}
	}
}