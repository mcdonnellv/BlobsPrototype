using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;

public class EnemyController : MonoBehaviour {
	public Actor actor;
	public BattleAnchor anchor;

	private float allowActionDelay = 0f;

	// Use this for initialization
	void Start () {
		GameObject obj = new GameObject(actor.name + "Anchor");
		anchor = obj.AddComponent<BattleAnchor>();
		anchor.transform.parent = GameObject.Find("Anchors").transform;
		anchor.transform.position = actor.transform.position;
		actor.SetBehaviorSharedVariable("anchor", obj);

		obj = (GameObject)GameObject.Instantiate(Resources.Load("Life Bar"));
		obj.name = actor.name + "LifeBar";
		BattleBlobLifeBar lifeBar = obj.GetComponent<BattleBlobLifeBar>();
		lifeBar.health = actor.health;
		obj.transform.parent = HudManager.hudManager.battleHud.transform;
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localScale = Vector3.one;

	}

	// Update is called once per frame
	void Update () {
		if((bool)actor.GetBehaviorSharedVariable("allowNewAction") == false && allowActionDelay <= 0f)
			allowActionDelay = Time.time + 4f;
		
		if(Time.time > allowActionDelay && allowActionDelay > 0f) {
			actor.SetBehaviorSharedVariable("allowNewAction", true);
			allowActionDelay = 0f;
		}
	}
}
