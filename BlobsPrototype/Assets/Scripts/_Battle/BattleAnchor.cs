using UnityEngine;
using System.Collections;

public class BattleAnchor : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		DrawDebug();
	}

	private void DrawDebug() {
		Debug.DrawRay(transform.position, new Vector3(-.2f,-.5f,0f));
		Debug.DrawRay(transform.position, new Vector3(.2f,-.5f,0f));
	}
}
