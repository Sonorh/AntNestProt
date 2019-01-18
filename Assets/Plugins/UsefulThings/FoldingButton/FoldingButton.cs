using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PM.UsefulThings
{
	public class FoldingButton : Button
	{
		public GameObject Content;

		private GraphicRaycaster m_Raycaster;
		private PointerEventData m_PointerEventData;
		private EventSystem m_EventSystem;

		protected override void Start()
		{
			base.Start();

			var trg = this.gameObject;
			do
			{
				m_Raycaster = trg.GetComponent<GraphicRaycaster>();

				if (m_Raycaster != null)
					break;

				if (trg.transform.parent == null)
				{
					Debug.Log("FoldingButton can't find raycaster");
					break;
				}

				trg = trg.transform.parent.gameObject;
			} while (trg != null);

			m_EventSystem = trg.GetComponent<EventSystem>();
		}

		public override void OnPointerClick(PointerEventData eventData)
		{
			if (CheckIfButtonClicked())
			{
				base.OnPointerClick(eventData);

				Content.SetActive(!Content.activeSelf);
			}
		}

		public override void OnPointerDown(PointerEventData eventData)
		{
			if (CheckIfButtonClicked())
			{
				base.OnPointerDown(eventData);
			}
		}

		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
		}

		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);


			Content.SetActive(false);
		}

		private bool CheckIfButtonClicked()
		{
			m_PointerEventData = new PointerEventData(m_EventSystem)
			{
				position = Input.mousePosition
			};

			List<RaycastResult> results = new List<RaycastResult>();

			m_Raycaster.Raycast(m_PointerEventData, results);

			//For every result returned, check if it's not a content click
			foreach (RaycastResult result in results)
			{
				if (result.gameObject.transform.IsChildOf(Content.transform))
					continue;

				if (result.gameObject == this.gameObject || result.gameObject.transform.IsChildOf(this.transform))
				{
					return true;
				}
			}

			return false;
		}
	}
}