using UnityEngine;
using System.Collections;

public class CheatMenu : MonoBehaviour {

	GameManager2 gameManager;
	HudManager hudManager;
	RoomManager roomManager;

	public void AddGold() { hudManager.UpdateGold(gameManager.gameVars.AddGold(1000)); }
	public void Show() { gameObject.SetActive(true);}
	public void Hide() { gameObject.SetActive(false);}

	// Use this for initialization
	void Start () {
		gameManager = GameObject.Find ("GameManager2").GetComponent<GameManager2> ();
		hudManager = GameObject.Find ("HudManager").GetComponent<HudManager> ();
		roomManager = GameObject.Find ("RoomManager").GetComponent<RoomManager> ();
	}
	// Update is called once per frame
	void Update () {
	
	}
}
