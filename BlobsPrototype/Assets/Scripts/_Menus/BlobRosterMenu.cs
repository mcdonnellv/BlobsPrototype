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
			GameObject slot = (GameObject)GameObject.Instantiate(Resources.Load("Blob Roster Slot"));
			slot.transform.parent = grid.transform;
			slot.transform.localScale = Vector3.one;

			// add the blob
			if(i < BlobManager.blobManager.blobs.Count) {
				Blob blob = BlobManager.blobManager.blobs[i];
				Transform blobTransform = blob.gameObject.transform;
				blobTransform.SetParent(slot.transform.FindChild("AttachPt"));
				blobTransform.localScale = Vector3.one;
				blobTransform.localPosition = Vector3.zero;
				Transform trans = slot.transform.FindChild("BG");
				UISprite colorBG = trans.gameObject.GetComponent<UISprite>();
				colorBG.color = ColorDefines.ColorForElement(blob.element);
			}
		}
		grid.Reposition();
	}
}
