using PM.UsefulThings;
using System.Collections;
using System.Collections.Generic;
using UIBinding.Base;
using UnityEngine;
using UnityEngine.UI;
using UIBinding;

namespace PM.Antnest.Gameplay
{
	public abstract class AbCell : BaseBindingBehaviourTarget
	{
		protected ColorProperty avatarColor = new ColorProperty();
		protected StringProperty state = new StringProperty();
		protected StringProperty value = new StringProperty();

		public ColliderTransfer ClickTransfer;

		public AbCellData Data { get; protected set; }

		public virtual void Reset(AbCellData data)
		{
			Data = data;
		}

		public Vector3 ScreenPosition
		{
			get
			{
				return new Vector3((Data.FieldPosition.x - Data.FieldPosition.z) / 2f, Data.FieldPosition.y * Mathf.Sqrt(3f) / 2);
			}
		}

		public void OnMouseClick()
		{
			if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
			{
				Data.OnClick();
			}
		}

		private void Start()
		{
			ClickTransfer.OnMouseClick += OnMouseClick;
			AbCellData.OnExplore += UpdateInfo;
		}

		private void OnDestroy()
		{
			ClickTransfer.OnMouseClick -= OnMouseClick;
			AbCellData.OnExplore -= UpdateInfo;
		}

		public abstract void UpdateInfo(AbCellData data = null);

		public void SetColor(Color color)
		{
			avatarColor.value = color;
		}
	}
}