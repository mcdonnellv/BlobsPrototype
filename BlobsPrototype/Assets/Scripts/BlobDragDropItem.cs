using UnityEngine;
using System.Collections;

public class BlobDragDropItem : UIDragDropItem
{
	public Blob blob;

	protected override void OnDragDropRelease (GameObject surface)
	{
		if (surface != null && blob != null)
		{
			Tile tile = (Tile)surface.GetComponent<Tile>();
			if(tile == null)
			{
				tile = surface.transform.parent.GetComponent<Tile>();
				if(tile == null)
					return;
				surface = surface.transform.parent.gameObject;
			}

			Room room = surface.transform.parent.GetComponent<Room>();
			if(room.IsTileOccupied(tile.xPos, tile.yPos))
			{
				// Re-enable the collider
				if (mButton != null) mButton.isEnabled = true;
				else if (mCollider != null) mCollider.enabled = true;
				else if (mCollider2D != null) mCollider2D.enabled = true;
				transform.localPosition = new Vector3(0f,0f,0f);
				base.OnDragDropEnd();
				return;
			}

			blob.tilePosX = tile.xPos;
			blob.tilePosY = tile.yPos;
		}
		base.OnDragDropRelease(surface);
	}
}