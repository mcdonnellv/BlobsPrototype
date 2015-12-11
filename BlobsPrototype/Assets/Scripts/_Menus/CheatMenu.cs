using UnityEngine;
using System.Collections;

public class CheatMenu : GenericGameMenu {

	GameManager2 gameManager;
	HudManager hudManager { get { return HudManager.hudManager; } }
	RoomManager roomManager  { get { return RoomManager.roomManager; } }

	public void Pressed() {	base.Show(); }

	public void AddGold() { hudManager.UpdateGold(gameManager.gameVars.AddGold(1000)); }

	public void IncrementMissionCount() { 
		foreach(Blob blob in roomManager.currentRoom.blobs)
			if(blob.hasHatched)
				blob.ReturnFromQuest();
	}

	// Use this for initialization
	void Start () {
		gameManager = GameObject.Find ("GameManager2").GetComponent<GameManager2> ();
	}

	// Update is called once per frame
	void Update () {
	
	}
}
