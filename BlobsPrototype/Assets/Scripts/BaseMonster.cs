using UnityEngine;
using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;

[Serializable]
public class BaseMonster : BaseThing {
	public string actorPrefabName = "Enemy";
	public BehaviorTree behaviorTree;
	public ActorData data;
}
