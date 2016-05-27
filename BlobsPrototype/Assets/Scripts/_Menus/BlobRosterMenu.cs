using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenericBlobMenu : GenericGameMenu {
	
}


public class BlobRosterMenu : GenericBlobMenu {

	public UIGrid grid;

	public void Pressed() {	Show(); }

	public override void Show() {
		if(IsDisplayed()) {
			return;
		}

		base.Show();
		RebuildSlots(32);

	}


	void RebuildSlots(int slotCount) {
		grid.transform.DestroyChildren();
		for (int i = 0; i < slotCount; i++) {
			GameObject slot = (GameObject)GameObject.Instantiate(Resources.Load("Blob Roster Cell"));

			BlobRosterCell blobRosterCell = slot.GetComponent<BlobRosterCell>();
			blobRosterCell.container.SetActive(false);
			slot.transform.parent = grid.transform;
			slot.transform.localScale = Vector3.one;

			// add the blob
			if(i < BlobManager.blobManager.blobs.Count) {
				blobRosterCell.blob = BlobManager.blobManager.blobs[i];
				blobRosterCell.DisplayBlobImage();
				blobRosterCell.SetStarCount((int)blobRosterCell.blob.quality);
				blobRosterCell.SetSigil();
			}
		}
		grid.Reposition();
	}


	public void BlobCellPressed(BlobRosterCell blobRosterCell) {
		//blobRosterCell.blob.gameObject.DisplayBlobInfo();
		BlobDetailsMenu blobDetailsMenu = HudManager.hudManager.blobDetailsMenu;
		blobDetailsMenu.blob = blobRosterCell.blob;
		blobDetailsMenu.Show(this);
		Hide();
	}
}
