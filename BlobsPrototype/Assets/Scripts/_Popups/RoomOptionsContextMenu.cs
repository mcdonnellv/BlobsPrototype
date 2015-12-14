using UnityEngine;
using System.Collections;

public class RoomOptionsContextMenu : MonoBehaviour {

	public UILabel roomInfo;
	public UIButton upgradeRoomButton;
	public UIButton createRoomButton;
	Room room;
	public TweenPosition tp;
	public bool displayRoomQueued = false;


	public void UpdateUpgradeRoomCost(int cost, bool maxReached, string requirement) {
		UILabel[] labels = upgradeRoomButton.GetComponentsInChildren<UILabel>();
		labels[0].text = "UPGRADE";
		labels[1].text = "";
		labels[2].text = "";
		upgradeRoomButton.isEnabled = false;

		if (maxReached) {
		}
		else if(requirement != null && requirement != "") {
			labels[2].text = requirement;
		}
		else {
			labels[1].text = string.Format("{0}[gold]",cost);
			upgradeRoomButton.isEnabled = true;
		}
	}

	public void UpdateCreateRoomCost(int cost,  string requirement) {
		UILabel[] labels = createRoomButton.GetComponentsInChildren<UILabel>();
		labels[0].text = "NEW ROOM";
		labels[1].text = "";
		labels[2].text = "";
		createRoomButton.isEnabled = false;

		if(requirement != null && requirement != "") {
			labels[2].text = requirement;
		}
		else {
			labels[1].text = string.Format("{0}[gold]",cost);
			createRoomButton.isEnabled = true;
		}

	}


	public void Show() { DisplayWithRoom(RoomManager.roomManager.currentRoom); }


	public void DisplayWithRoom(Room roomParam) {
		displayRoomQueued = false;
		if(IsDisplayed() == true) {
			Dismiss();
			displayRoomQueued = true;
		}
		else {
			tp.PlayForward();
			tp.enabled = true;
		}

		room = roomParam;
	}


	public bool IsDisplayed() {
		return (transform.localPosition == tp.to && tp.enabled == false);
	}


	public void Dismiss() {
		tp.PlayReverse();
	}

	public void AnimationDone() {
		if(displayRoomQueued)
			DisplayWithRoom(room);

	}

	// Use this for initialization
	void Start () {
		tp = gameObject.GetComponent<TweenPosition>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
