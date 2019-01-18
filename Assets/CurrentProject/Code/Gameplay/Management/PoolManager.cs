using PM.UsefulThings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	public class PoolManager : SoftMonoSingleton<PoolManager>
	{
		public SimplePool<NestCell> NestCellPool;
		public SimplePool<EnviromentCell> EnviromentCellPool;
		public SimplePool<AntAvatar> AntPool;

		protected override void Awake()
		{
			base.Awake();

			NestCellPool = new SimplePool<NestCell>(GameplayHolderManager.Instance.Prefabs.NestCellPrefab);
			EnviromentCellPool = new SimplePool<EnviromentCell>(GameplayHolderManager.Instance.Prefabs.EnviromentCellPrefab);
			AntPool = new SimplePool<AntAvatar>(GameplayHolderManager.Instance.Prefabs.AntPrefab);
		}
	}
}