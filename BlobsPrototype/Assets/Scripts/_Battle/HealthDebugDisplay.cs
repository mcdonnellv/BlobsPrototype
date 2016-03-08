using UnityEngine;
using System.Collections;

public class HealthDebugDisplay : MonoBehaviour {
	public ActorHealth health;
	public UnityEngine.UI.Text healthLabel;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		healthLabel.text = System.Math.Truncate(health.health).ToString();
	}
}
