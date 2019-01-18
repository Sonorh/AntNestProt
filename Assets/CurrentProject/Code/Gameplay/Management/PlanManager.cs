using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	public class PlanManager
	{
		public List<AbPlayerPlan> Plans = new List<AbPlayerPlan>();

		public PlanManager()
		{
			TimeManager.Instance.OnStartTurn += OnTickHandler;
			GameplayManager.Instance.OnGameplayOver += OnGameplayOver;
		}

		public void OnGameplayOver()
		{
			GameplayManager.Instance.OnGameplayOver -= OnGameplayOver;

			TimeManager.Instance.OnStartTurn -= OnTickHandler;
			foreach (var plan in Plans.ToArray())
			{
				RemovePlan(plan);
			}
		}

		public void OnTickHandler()
		{
			foreach (var plan in Plans.ToArray())
			{
				plan.Tick();
			}
		}

		public void ForceExecuteAllPlans()
		{
			foreach (var plan in Plans.ToArray())
			{
				ForceExecutePlan(plan);
			}
		}

		public void ForceExecutePlan(AbPlayerPlan plan)
		{
			plan.Execute();
		}

		public void TryToAddPlan(AbPlayerPlan plan)
		{
			if (plan.IsExecutable && (plan.TargetCell == null || !Plans.Exists(x => x.TargetCell == plan.TargetCell && x.TargetCell != x.Player.Nest)))
			{
				Plans.Add(plan);
				plan.OnDone += RemovePlan;
				plan.Prepare();
			} 
		}

		private void RemovePlan(AbPlayerPlan plan)
		{
			plan.OnDone -= RemovePlan;
			Plans.Remove(plan);
		}

		public bool CheckPlan(AbPlayerPlan plan)
		{
			return plan.IsExecutable && CheckCell(plan.TargetCell);
		}

		public bool CheckCell(AbCellData cell)
		{
			return cell == null || !Plans.Exists(x => x.TargetCell == cell && x.TargetCell != x.Player.Nest);
		}

		public void UpdatePlans()
		{
			foreach (var plan in Plans.ToArray())
			{
				plan.Update();
			}
		}
	}
}