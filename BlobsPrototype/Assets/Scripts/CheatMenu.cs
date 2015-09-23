using UnityEngine;
using System.Collections;

public class CheatMenu : MonoBehaviour {

	GameManager2 gameManager;
	HudManager hudManager;
	RoomManager roomManager;

	public void AddGold() { hudManager.UpdateGold(gameManager.gameVars.AddGold(1000)); }
	public void Show() { gameObject.SetActive(true);}
	public void Hide() { gameObject.SetActive(false);}
	public void MaxRank() { 
		foreach(Room r in roomManager.rooms)
			foreach(Blob b in r.blobs)
				b.level = 100;
		hudManager.roomOptionsContextMenu.createRoomButton.isEnabled = true;
	}
	public void MinRank() { 
		foreach(Room r in roomManager.rooms)
			foreach(Blob b in r.blobs)
				b.level = 1;
		hudManager.roomOptionsContextMenu.createRoomButton.isEnabled = true;
	}

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
