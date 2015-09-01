using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Room : MonoBehaviour {

	public int columns = 6;
	public int rows = 6;
	public RoomManager rm;
	public int tilewidth;
	public int tileHeight;
	public List<Blob> blobs;
	public List<Tile> tiles;

	public void Setup(){
		if(blobs == null)
			blobs = new List<Blob>();

		if(tiles == null)
			tiles = new List<Tile>();

		for(int x = 0; x < columns; x++){
			for(int y = 0; y < rows; y++){
				GameObject toInstantiate = rm.floorTiles[Random.Range (0, rm.floorTiles.Length)];
				GameObject instance = Instantiate (toInstantiate) as GameObject;
				Tile tile = instance.GetComponent<Tile>();
				tile.xPos = x;
				tile.yPos = y;
				tiles.Add(tile);
				instance.transform.SetParent (transform);
				tilewidth = instance.GetComponent<UISprite>().width-4;
				tileHeight = instance.GetComponent<UISprite>().height-4;
				instance.transform.localPosition = new Vector3(x * tilewidth, y * -tileHeight, 0f);
				instance.transform.localScale = new Vector3(1f,1f,1f);
			}
		}
	}

	public void AddBlob(Blob blob){
		if (IsRoomFull())
			return;

		Transform blobTransform = blob.blobGameObject.transform;
		blobTransform.SetParent(transform);
		float scale = .4f;
		blobTransform.localScale = new Vector3(scale,scale,1f);
		Vector2 v = GetNextFreeTile();
		MoveBlob(blob, (int)v.x, (int)v.y);
		blobs.Add(blob);
	}

	bool MoveBlob(Blob blob, int xPos, int yPos){
		if(IsTileOccupied(xPos, yPos))
			return false;
		xPos = Mathf.Clamp (xPos, 0, rows);
		yPos = Mathf.Clamp (yPos, 0, columns);
		blob.tilePosX = xPos;
		blob.tilePosY = yPos;
		Tile tile = GetTile(xPos, yPos);
		blob.blobGameObject.transform.parent = tile.transform;
		tile.GetComponent<UIGrid>().Reposition();
		return true;
	}

	public bool IsTileOccupied(int xPos, int yPos){
		foreach(Blob b in blobs)
			if(b.tilePosX == xPos && b.tilePosY == yPos)
				return true;
		return false;
	}

	public bool IsRoomFull(){
		if(blobs.Count >= columns * rows)
			return true;
		return false;
	}

	Vector2 GetNextFreeTile(){
		for(int y = 0; y < columns; y++)
			for(int x = 0; x < rows; x++)
				if(IsTileOccupied(x,y) == false)
					return new Vector2(x,y);
		return new Vector2(0,-1);
	}

	Tile GetTile(int x, int y)
	{
		foreach(Tile tile in tiles)
			if(tile.xPos == x && tile.yPos == y)
				return tile;
		return null;
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	}
}
