using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;

public class EnemyController : MonoBehaviour {
	public Actor actor;
	public BattleAnchor anchor;

	private float allowActionDelay = 0f;


	// Use this for initialization
	void Start () {
		CombatManager.combatManager.onBeat += Beat;
		GameObject obj = new GameObject(actor.name + "Anchor");
		anchor = obj.AddComponent<BattleAnchor>();
		anchor.transform.parent = GameObject.Find("Anchors").transform;
		anchor.transform.position = actor.transform.position;
		actor.SetBehaviorSharedVariable("anchor", obj);

		obj = (GameObject)GameObject.Instantiate(Resources.Load("Enemy Life Bar"));
		obj.name = actor.name + "LifeBar";
		BattleLifeBar lifeBar = obj.GetComponent<BattleLifeBar>();
		lifeBar.health = actor.health;
		obj.transform.parent = HudManager.hudManager.battleHud.transform;
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localScale = Vector3.one;

	}

	void Beat() {
		actor.SetBehaviorSharedVariable("allowNewAction", true);
	}


	// Update is called once per frame
	void Update () {
	}
}
