using UnityEngine;
using System.Collections;

public class BuildRoomMenu : MonoBehaviour {

	GameManager2 gameManager;
	HudManager hudManager;
	RoomManager roomManager;

	public void Show() { gameObject.SetActive(true);}
	public void Hide() { gameObject.SetActive(false);}

	// Use this for initialization
	void Start () {
		gameManager = GameObject.Find ("GameManager2").GetComponent<GameManager2> ();
		hudManager = GameObject.Find ("HudManager").GetComponent<HudManager> ();
		roomManager = GameObject.Find ("RoomManager").GetComponent<RoomManager> ();
	}
}
