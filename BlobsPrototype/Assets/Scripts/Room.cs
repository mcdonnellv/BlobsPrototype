using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Room : MonoBehaviour {
	public enum RoomType {
		Field = 0,
		Workshop,
	}

	public int size;
	public RoomManager rm;
	public int tilewidth;
	public int tileHeight;
	public List<Blob> blobs;
	public List<Tile> tiles;
	public RoomType type;
	int showFloatingSpritesRefCt = 0;

	
	public void Setup() {
		if(blobs == null)
			blobs = new List<Blob>();

		if(tiles == null)
			tiles = new List<Tile>();

		if(rm == null)
			rm = GameObject.Find("RoomManager").GetComponent<RoomManager>();

		for(int x = 0; x < size; x++) {
			for(int y = 0; y < size; y++) {
				GameObject toInstantiate = rm.floorTiles[(int)type];
				GameObject instance = Instantiate (toInstantiate) as GameObject;
				Tile tile = instance.GetComponent<Tile>();
				tile.xPos = x;
				tile.yPos = y;
				tiles.Add(tile);
				instance.transform.SetParent (transform);
				tilewidth = instance.GetComponent<UISprite>().width-20;
				tileHeight = instance.GetComponent<UISprite>().height-20;
				instance.transform.localPosition = new Vector3((x - ((size - 1) * .5f)) * tilewidth, 
				                                               (y - ((size - 1) * .5f)) * -tileHeight, 0f);
				instance.transform.localScale = new Vector3(1f,1f,1f);

				UISprite sprite = instance.GetComponent<UISprite>();
				sprite.color = ((x+y) % 2 == 0) ? new Color(.8f,.8f,.8f,1f) : new Color(.65f,.65f,.65f,1f);
			}
		}

		showFloatingSpritesRefCt = 0;
	}


	public void AddBlob(Blob blob) {
		Vector2 v = GetNextFreeTile();
		AddBlobToTile(blob, (int)v.x, (int)v.y);
	}


	public void AddBlobToTile(Blob blob, int xPos, int yPos) {
		if (IsRoomFull())
			return;
		
		Transform blobTransform = blob.gameObject.transform;
		blobTransform.SetParent(transform);
		blobTransform.localScale = new Vector3(1f, 1f, 1f);
		blob.room = this;
		MoveBlob(blob, xPos, yPos);
		if(blobs.Contains(blob) == false)
			blobs.Add(blob);

		rm.UpdateRoomOptionsContextMenuValues(rm.currentRoom);
	}


	public void DeleteBlob(Blob blob) {
		blob.CleanUp();
		blobs.Remove(blob);
		DestroyImmediate(blob.gameObject.gameObject);
	}


	bool MoveBlob(Blob blob, int xPos, int yPos) {
		Blob otherBlob = GetBlobOnTile(xPos, yPos);
		if(otherBlob != null && blob != otherBlob)
			return false;

		xPos = Mathf.Clamp (xPos, 0, size);
		yPos = Mathf.Clamp (yPos, 0, size);
		blob.tilePosX = xPos;
		blob.tilePosY = yPos;
		Tile tile = GetTile(xPos, yPos);
		blob.gameObject.transform.parent = tile.transform;
		tile.GetComponent<UIGrid>().Reposition();
		return true;
	}
	

	public bool IsTileOccupied(int xPos, int yPos) { return (GetBlobOnTile(xPos, yPos) != null); }


	public Blob GetBlobOnTile(int xPos, int yPos) {
		foreach(Blob b in blobs)
			if(b.tilePosX == xPos && b.tilePosY == yPos)
				return b;
		return null;
	}


	public bool IsRoomFull(){
		if(blobs.Count >= size * size)
			return true;
		return false;
	}


	Vector2 GetNextFreeTile(){
		for(int y = 0; y < size; y++)
			for(int x = 0; x < size; x++)
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


	public void ShowFloatingSprites(Blob blobDragged) {
		showFloatingSpritesRefCt++;
		if(showFloatingSpritesRefCt > 1)
			return;

		foreach(Blob b in blobs)
			if(blobDragged == null)
				b.gameObject.GetComponentInChildren<BlobFloatingDisplay>().ShowBlobInfo();
			else
				b.gameObject.GetComponentInChildren<BlobFloatingDisplay>().ShowBlobInfo(blobDragged);
	}


	public void HideFloatingSprites() {
		showFloatingSpritesRefCt--;
		if(showFloatingSpritesRefCt > 0)
			return;

		foreach(Blob b in blobs)
			b.gameObject.GetComponentInChildren<BlobFloatingDisplay>().HideBlobInfo();
	}


	void Update () {
	}
}
