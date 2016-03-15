using UnityEngine;
using System.Collections;

public class BattleGameEndDisplayObject : MonoBehaviour {

	public UITweener scaleTween;

	void OnEnable() {
		scaleTween.enabled = true;
		scaleTween.PlayForward();
		Invoke("Hide", 3f);
	}

	void Hide() {
		scaleTween.PlayReverse();
		Invoke("Disable", scaleTween.duration);
	}

	void Disable() {
		gameObject.SetActive(false);
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
