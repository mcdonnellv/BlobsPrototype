using UnityEngine;
using System.Collections;

public class BuildRoomMenu : MonoBehaviour {

	GameManager2 gameManager;
	HudManager hudManager { get { return HudManager.hudManager; } }
	RoomManager roomManager  { get { return RoomManager.roomManager; } }

	public void Show() { gameObject.SetActive(true);}
	public void Hide() { gameObject.SetActive(false);}

	// Use this for initialization
	void Start () {
		gameManager = GameObject.Find ("GameManager2").GetComponent<GameManager2> ();
	}
}
