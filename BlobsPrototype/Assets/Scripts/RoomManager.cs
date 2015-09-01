using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class RoomManager : MonoBehaviour {

	public List<Room> rooms = new List<Room>();
	public Transform worldTransform;
	public GameObject[] floorTiles;
	int maxColumns = 6;
	int maxRows = 6;
	int tileWidth = 100;

	public Room CreateRoom(int columns, int rows)
	{
		columns = Mathf.Clamp (columns, 0, maxColumns);
		rows = Mathf.Clamp (rows, 0, maxRows);

		GameObject roomObject = new GameObject ("RoomObject");
		roomObject.transform.SetParent(worldTransform);
		roomObject.transform.localScale = new Vector3(1f,1f,1f);
		Vector3 v = new Vector3(GetNewRoomPostionX(), GetRoomPostionY(rows), 0);
		roomObject.transform.localPosition = v;
		Room room = roomObject.AddComponent<Room>();
		room.rm = this;
		room.columns = columns;
		room.rows = rows;
		room.Setup();
		rooms.Add(room);
		return room;
	}

	float GetNewRoomPostionX() {
		if(rooms == null || rooms.Count == 0)
			return 0f;

		return tileWidth * maxColumns * rooms.Count + (rooms.Count * tileWidth);
	}

	float GetRoomPostionY(int rows) {
		return ((maxRows - rows) / 2f * -tileWidth);
	}

	public void ResizeRoom(Room room, int size){
		if(size < room.rows || size < room.columns)
			return;
		
		if(size == room.rows && size == room.columns)
			return;
		
		foreach(Transform t in room.transform)
			if(t.gameObject.GetComponent<Tile>() != null)
				GameObject.Destroy(t.gameObject);
		
		room.rows = size;
		room.columns = size;
		room.Setup();

		Vector3 v = new Vector3(room.transform.localPosition.x, GetRoomPostionY(size), 0);
		room.transform.localPosition = v;
	}


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
