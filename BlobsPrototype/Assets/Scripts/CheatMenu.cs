using UnityEngine;
using System.Collections;

public class CheatMenu : GenericGameMenu {

	GameManager2 gameManager;
	HudManager hudManager;
	RoomManager roomManager;

	public void Pressed() {	base.Show(); }

	public void AddGold() { hudManager.UpdateGold(gameManager.gameVars.AddGold(1000)); }

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
