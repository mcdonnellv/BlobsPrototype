using UnityEngine;
using System.Collections;

public class BlobDragDropItem : UIDragDropItem {
	Animator animator;
	UIScrollView scrollView;
	RoomManager roomManager;
	HudManager hudManager;

	void Start () {
		roomManager = GameObject.Find ("RoomManager").GetComponent<RoomManager> ();
		hudManager = GameObject.Find("HudManager").GetComponent<HudManager>();
		scrollView = roomManager.scrollView;
		animator = GetComponent<Animator>();
	}

	protected override void OnDrag (Vector2 delta) {
		hudManager = GameObject.Find("HudManager").GetComponent<HudManager>();
		if(hudManager.dragToUi == false) {
		UICenterOnChild coc = roomManager.scrollView.GetComponent<UICenterOnChild>();
			SpringPanel sp = roomManager.scrollView.GetComponent<SpringPanel>();
			coc.enabled = false;
			sp.enabled = false;
			Blob blob = gameObject.GetComponent<Blob>();
			blob.room.ShowFloatingSprites(blob);
			animator.SetBool("dragging", true);
			if(hudManager.blobInfoContextMenu.IsDisplayed())
				hudManager.blobInfoContextMenu.Dismiss();
		}

		base.OnDrag(delta);
	}

	protected override void OnDragDropEnd () {
		UICenterOnChild coc = roomManager.scrollView.GetComponent<UICenterOnChild>();
		SpringPanel sp = roomManager.scrollView.GetComponent<SpringPanel>();
		coc.enabled = true;
		sp.enabled = true;
	}



	protected override void OnDragDropRelease (GameObject surface) {
		roomManager.scrollVector = new Vector2(0f,0f);
		animator.SetBool("dragging", false);
		Blob blob = gameObject.GetComponent<Blob>();
		blob.room.HideFloatingSprites();
		if (surface != null && blob != null)
		{
			BlobDragDropContainer blobContainer =  (BlobDragDropContainer)surface.GetComponent<BlobDragDropContainer>();
			if (blobContainer == null) {
				blobContainer = (surface.transform.parent == null) ? null : surface.transform.parent.GetComponent<BlobDragDropContainer>();
				if(blobContainer == null) {
					ReEnableCollider();
					return;
				}
				surface = surface.transform.parent.gameObject;
			}

			Tile tile = (Tile)surface.GetComponent<Tile>();
			if(tile != null) {
				DropBlobOnTile(blob, tile);
				return;
			}
		}
		base.OnDragDropRelease(surface);
	}


	void DropBlobOnTile(Blob blob, Tile tile) {
		Room room = tile.transform.parent.GetComponent<Room>();
		Blob curentOccupant = room.GetBlobOnTile(tile.xPos, tile.yPos);
		if(curentOccupant != null && blob != curentOccupant) {
			if ((blob.canBreed && curentOccupant.canBreed) || (blob.canMerge && curentOccupant.canMerge))
				hudManager.blobInteractPopup.Show(blob, curentOccupant);
			ReEnableCollider();
			return;
		}
		
		if(blob.room != room) {
			if(blob.spouseId == -1) {
				Room oldRoom = blob.room;
				room.AddBlobToTile(blob, tile.xPos, tile.yPos);
				oldRoom.blobs.Remove(blob);
			}
			else {
				hudManager.popup.Show("Cannot Move", "This Blob is paried with a blob from the previous room. Unpair them first.");
				ReEnableCollider();
				return;
			}
		}
		else {
			blob.tilePosX = tile.xPos;
			blob.tilePosY = tile.yPos;
		}

		base.OnDragDropRelease(tile.gameObject);
	}


	void ReEnableCollider() {
		// Re-enable the collider
		if (mButton != null) mButton.isEnabled = true;
		else if (mCollider != null) mCollider.enabled = true;
		else if (mCollider2D != null) mCollider2D.enabled = true;
		transform.localPosition = new Vector3(0f,0f,0f);
		base.OnDragDropEnd();
	}


	protected override void OnDragDropMove (Vector2 delta) {
		if(hudManager.dragToUi == false) {
			Vector2 dir = new Vector2(0f, 0f); 
			if(mTrans.position.x > 1.5f)
				dir.x += -1;
			else if(mTrans.position.x < -1.5f)
				dir.x += 1;
			
			if(mTrans.position.y > .7f)
				dir.y += -1;
			else if(mTrans.position.y < -.7f)
				dir.y += 1;
			
			dir.Normalize();
			roomManager.scrollVector = dir;
		}
		mTrans.localPosition += (Vector3)delta;
	}





	void Update() {
		if(mDragging)
			mTrans.localPosition += (Vector3)(-roomManager.scrollVector * roomManager.scrollSpeed * Time.deltaTime);
	}
	
}


