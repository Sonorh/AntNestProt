using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	//todo: make it abstract and executable. too much logic goes to planManager.
	public abstract class AbPlayerPlan
	{
		public event System.Action<AbPlayerPlan> OnDone;
		public event System.Action<AbPlayerPlan> OnSuccess;
		public event System.Action<AbPlayerPlan> OnCancel;
		public event System.Action<AbPlayerPlan> OnFail;

		public PlayerManager Player;
		public PlanTypes PlanType;
		public AbCellData TargetCell;
		public AntSwarm Ants;
		public int Delay;

		protected int delayTillExecute = -1;

		public void Tick()
		{
			if (delayTillExecute == -1)
			{
				delayTillExecute = Delay;
			}

			delayTillExecute--;

			OnTick();

			if (delayTillExecute <= 0)
			{
				Execute();
			}
		}

		public virtual void Update()
		{

		}

		public abstract bool IsExecutable { get; }

		public abstract void Prepare();
		public abstract void Execute();

		public virtual void Success()
		{
			OnSuccessCaller();
		}
		public virtual void Cancel()
		{
			OnCancelCaller();
		}
		public virtual void Fail()
		{
			OnFailCaller();
		}

		protected virtual void OnTick() { }

		private void OnSuccessCaller()
		{
			OnSuccess?.Invoke(this);
			OnDoneCaller();
		}
		private void OnCancelCaller()
		{
			OnCancel?.Invoke(this);
			OnDoneCaller();
		}
		private void OnFailCaller()
		{
			OnFail?.Invoke(this);
			OnDoneCaller();
		}

		protected virtual void OnDoneCaller()
		{
			OnDone?.Invoke(this);
		}
	}
}