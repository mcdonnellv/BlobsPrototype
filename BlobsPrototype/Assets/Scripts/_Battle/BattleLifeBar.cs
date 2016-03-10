using UnityEngine;
using System.Collections;

public class BattleLifeBar : MonoBehaviour {
	public ActorHealth health;
	public UIProgressBar lifeBar;
	protected bool moveWithActor = true;

	// Use this for initialization
	void Start () {

	}

	public void OnValueChanged() {
		if(lifeBar.value >= .66)
			lifeBar.foregroundWidget.color = Color.green;
		else if (lifeBar.value >= .33)
			lifeBar.foregroundWidget.color = Color.yellow;
		else
			lifeBar.foregroundWidget.color = Color.red;
	}

	// Update is called once per frame
	void Update () {
		if(health == null) {
			HostDead();
			return;
		}
		
		if(lifeBar != null && health.startHealth != 0)
			lifeBar.value = health.health / health.startHealth;

		if(moveWithActor) {
			Camera uiCam = HudManager.hudManager.battleHud.camera;
			Vector3 pos = health.transform.position;
			pos.y = Mathf.Clamp(pos.y, pos.y , -.5f);
			Vector2 screenPos = Camera.main.WorldToScreenPoint(pos);
			transform.position = uiCam.ScreenToWorldPoint(screenPos);
		}
	}

	protected virtual void HostDead() {
		GameObject.Destroy(gameObject);
	}
}
