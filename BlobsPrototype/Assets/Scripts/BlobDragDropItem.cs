using UnityEngine;
using System.Collections;

public class BlobDragDropItem : UIDragDropItem {
	Animator animator { get { return GetComponent<Animator>(); } }
	UIScrollView scrollView { get { return roomManager.scrollView; } }
	RoomManager roomManager  { get { return RoomManager.roomManager; } }
	HudManager hudManager { get { return HudManager.hudManager; } }
	public bool uiClone = false;



	protected override void OnClone (GameObject original) {
		uiClone = true;
		cloneOnDrag = false;
		Vector3 worldPos = transform.position;
		transform.parent = hudManager.dragObjectHelper.transform;
		transform.localPosition = Vector3.zero;
		transform.position += worldPos;
	}


	protected override void OnDragStart () {
		cloneOnDrag = hudManager.dragToUi && !uiClone;
		base.OnDragStart();
	}


	protected override void OnDrag (Vector2 delta) {
		if (!interactable) return;
		if (!mDragging || !enabled || mTouch != UICamera.currentTouch) return;
		UICenterOnChild coc = roomManager.scrollView.GetComponent<UICenterOnChild>();
		SpringPanel sp = roomManager.scrollView.GetComponent<SpringPanel>();
		coc.enabled = false;
		sp.enabled = false;
		Blob blob = gameObject.GetComponent<Blob>();
		blob.room.ShowFloatingSprites(blob);
		animator.SetBool("dragging", true);
		if(hudManager.blobInfoContextMenu.IsDisplayed())
			hudManager.blobInfoContextMenu.Hide();
		OnDragDropMove(delta * mRoot.pixelSizeAdjustment);
	}


	protected override void OnDragDropEnd () {
		UICenterOnChild coc = roomManager.scrollView.GetComponent<UICenterOnChild>();
		SpringPanel sp = roomManager.scrollView.GetComponent<SpringPanel>();
		coc.enabled = true;
		sp.enabled = true;
	}
	

	protected override void OnDragDropRelease (GameObject surface) {
		Blob blob = gameObject.GetComponent<Blob>();
		blob.room.HideFloatingSprites();
		roomManager.scrollVector = new Vector2(0f,0f);
		animator.SetBool("dragging", false);
		if(uiClone) {
			OnDragDropReleaseForClone(surface);
			return;
		}
		if (surface != null && blob != null) {
			BlobDragDropContainer blobContainer = (BlobDragDropContainer)surface.GetComponent<BlobDragDropContainer>();
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


	void OnDragDropReleaseForClone (GameObject surface) {
		Blob blob = gameObject.GetComponent<Blob>();
		transform.SendMessageUpwards("BlobRemovedFromContainer", blob.id);
		if(surface == null) {
			GameObject.Destroy(gameObject);
			return;
		}
		BlobDragDropContainer blobContainer = (BlobDragDropContainer)surface.GetComponentInParent<BlobDragDropContainer>();

		if(blobContainer == null || blobContainer.uiContainer == false) {
			GameObject.Destroy(gameObject);
			return;
		}

		if(blobContainer.hasBlob) {
			BlobDragDropItem currentResident = blobContainer.gameObject.GetComponentInChildren<BlobDragDropItem>();
			GameObject.Destroy(currentResident.gameObject);
		}

		base.OnDragDropRelease(surface);
		blobContainer.BlobAdded(blob);
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
		if(uiClone) {
			mTrans.localPosition += (Vector3)delta;
			return;
		}
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
		mTrans.localPosition += (Vector3)delta;
	}


	void Update() {
		if(mDragging)
			mTrans.localPosition += (Vector3)(-roomManager.scrollVector * roomManager.scrollSpeed * Time.deltaTime);
	}
	
}


