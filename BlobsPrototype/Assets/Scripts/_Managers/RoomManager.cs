using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class RoomManager : MonoBehaviour {

	private static RoomManager _roomManager;
	public static RoomManager roomManager { get {if(_roomManager == null) _roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>(); return _roomManager; } }

	public List<Room> rooms = new List<Room>();
	public GameObject[] floorTiles;
	public UIScrollView scrollView;
	public Room currentRoom;
	public Vector2 scrollVector = new Vector2(0f,0f);
	public float scrollSpeed = 400f;

	Transform worldTransform;
	public int maxSize;
	public int minSize;
	int tileWidth = 100;
	GameManager2 gameManager { get { return GameManager2.gameManager; } }
	HudManager hudManager { get { return HudManager.hudManager; } }


	public Room CreateRoom(int size, Room.RoomType type) {
		if(worldTransform == null)
			worldTransform = scrollView.transform;

		size = Mathf.Clamp (size, 0, maxSize);
		GameObject roomObject = new GameObject ("RoomObject");
		roomObject.transform.SetParent(worldTransform);
		roomObject.transform.localScale = new Vector3(1f,1f,1f);
		Vector3 v = new Vector3(GetNewRoomPostionX(), 0, 0);
		roomObject.transform.localPosition = v;
		Room room = roomObject.AddComponent<Room>();
		room.rm = this;
		room.size = size;
		room.type = type;
		room.Setup();
		rooms.Add(room);
		SetCurrentRoom(room);
		return room;
	}


	public void SetCurrentRoom(Room room) {
		//hudManager.roomOptionsContextMenu.DisplayWithRoom(room);
		MoveScrollViewToRoom(room);
		currentRoom = room;
		UpdateRoomOptionsContextMenuValues(currentRoom);
	}


	public void UpdateRoomOptionsContextMenuValues(Room room) {
		RoomOptionsContextMenu rocm = hudManager.roomOptionsContextMenu;
		rocm.roomInfo.text = room.type.ToString() + " (Level " + (room.size - minSize + 1).ToString() + ")";
		rocm.UpdateUpgradeRoomCost(RoomCostForSize(room.size + 1), (room.size >= maxSize), null);
		string reqStr = null;
		rocm.UpdateCreateRoomCost(NewRoomCost(), reqStr);
	}


	public void MoveScrollViewToBlob(Transform blobTransform, Room room) {
		//scrollView.GetComponent<UICenterOnChild>().CenterOn(room.transform);

		Vector3 offset = new Vector3(tileWidth, 0 ,0f);
		Vector3 newPos = -blobTransform.parent.localPosition; // tile position
		newPos += -room.transform.localPosition; // room position
		newPos += offset;
		SpringPanel.Begin(scrollView.GetComponent<UIPanel>().gameObject, newPos, 13f).strength = 8f;
	}


	public void MoveScrollViewToRoom(Room room) {
		GameObject go = scrollView.GetComponent<UICenterOnChild>().centeredObject;
		Room centeredRoom = null;
		if(go != null)
			centeredRoom = go.GetComponent<Room>();
		if(centeredRoom == null || centeredRoom != room)
			scrollView.GetComponent<UICenterOnChild>().CenterOn(room.transform);
	}


	float GetNewRoomPostionX() {
		if(rooms == null || rooms.Count == 0)
			return 0f;

		return tileWidth * maxSize * rooms.Count + (rooms.Count * tileWidth);
	}


	public void ResizeRoom(Room room, int size){
		if(size < room.size || size == room.size)
			return;
		
		foreach(Transform t in room.transform)
			if(t.gameObject.GetComponent<Tile>() != null)
				GameObject.Destroy(t.gameObject);
		room.tiles.Clear();
		room.size = size;
		room.Setup();

		foreach(Blob b in room.blobs)
			room.AddBlobToTile(b, b.tilePosX, b.tilePosY);

		foreach(Blob b in room.blobs)
			foreach (Behaviour behaviour in b.gameObject.GetComponentsInChildren<Behaviour>())
				behaviour.enabled = true;



		Vector3 v = new Vector3(room.transform.localPosition.x, 0, 0);
		room.transform.localPosition = v;
		SetCurrentRoom(room);
	}


	public Blob GetBlobByID(int id) {
		foreach(Room r in rooms)
			foreach(Blob b in r.blobs)
				if(id == b.id)
					return b;
		return null;
	}


	public int RoomCostForSize(int size) {
		float pow = 2.5f;
		return (int)Mathf.Pow(size - minSize, pow) * 100;
	}


	public int NewRoomCost() {
		int numRooms = rooms.Count;
		float pow = numRooms + .5f;
		return (int)Mathf.Pow(2, pow) * 1000;
	}


	public void TryUpgradeRoom() {
		MoveScrollViewToRoom(currentRoom);
		int cost = RoomCostForSize(currentRoom.size + 1);
		if(gameManager.gameVars.gold < cost) {
			hudManager.popup.Show("Upgrade Room", "Not enough Gold.");
			return;
		}
	
		hudManager.popup.Show ("Upgrade Room", 
		                       string.Format ("Upgrade room to size {0}?", (currentRoom.size + 1).ToString ()),
		                       this,
		                       "UpgradeRoomConfirmed"); 
	}

	public void TryDeleteRoom() {
		MoveScrollViewToRoom(currentRoom);
		if(rooms.Count == 1) {
			hudManager.popup.Show("Delete Room", "Cannot delete Your last room"); 
			return;
		}

		if(currentRoom.blobs.Count > 0) {
			hudManager.popup.Show("Delete Room", "Remove blobs before deleting."); 
			return;
		}

		hudManager.popup.Show ("Delete Room", 
		                       "Are you sure?",
		                       this,
		                       "DeleteRoomConfirmed"); 
	}


	public void TryCreateRoom() {
		MoveScrollViewToRoom(currentRoom);
		int cost = NewRoomCost();
		if(gameManager.gameVars.gold < cost) {
			hudManager.popup.Show("Create New Room", "Not enough Gold.");
			return;
		}
		
		hudManager.buildRoomMenu.Show();
	}
	

	public void UpgradeRoomConfirmed() {
		int cost = RoomCostForSize(currentRoom.size + 1);
		if(currentRoom.size >= maxSize) {
			hudManager.popup.Show("Upgrade Room", "Room is at maximum size.");
			return;
		}

		gameManager.AddGold(-cost);
		ResizeRoom(currentRoom, currentRoom.size + 1);
		SetCurrentRoom(currentRoom);
	}

	public void DeleteRoomConfirmed() {
		Room roomToDelete = currentRoom;
		rooms.Remove(roomToDelete);
		Destroy(roomToDelete.gameObject);

		foreach(Room r in rooms) {
			float x = tileWidth * maxSize * rooms.IndexOf(r) + (rooms.Count * tileWidth);
			r.transform.localPosition = new Vector3(x, 0, 0);
		}
		SetCurrentRoom(rooms[0]);
	}


	public void CreateBreedingRoomConfirmed() {
		CreateRoomWithType(Room.RoomType.Field);
	}


	public void CreateWorkingRoomConfirmed() {
		CreateRoomWithType(Room.RoomType.Workshop);
	}


	public void CreateRoomWithType(Room.RoomType type) {
		gameManager.AddGold(-NewRoomCost());
		CreateRoom(minSize, type);
		hudManager.buildRoomMenu.Hide();
	}


	public void ToggleAllFloatingSprites(bool show) {
		foreach(Room room in rooms) {
			if(show)
				room.ShowFloatingSprites(null);
			else
				room.HideFloatingSprites();
		}
	}


	// Use this for initialization
	void Start () {
		worldTransform = scrollView.transform;
	}
	
	// Update is called once per frame
	void Update () {
		if(scrollVector.x != 0 || scrollVector.y != 0)
			scrollView.MoveRelative(scrollVector * scrollSpeed * Time.deltaTime);

		GameObject go = scrollView.GetComponent<UICenterOnChild>().centeredObject;
		Room centeredRoom = null;
		if(go != null)
			centeredRoom = go.GetComponent<Room>();
		if(centeredRoom != currentRoom){
			SetCurrentRoom(centeredRoom);
		}
	}
}
