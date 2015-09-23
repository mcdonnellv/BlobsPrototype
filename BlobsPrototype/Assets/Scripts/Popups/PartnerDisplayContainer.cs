using UnityEngine;
using System.Collections;

public class PartnerDisplayContainer : MonoBehaviour {
	public UILabel statusLabel;
	public UIWidget blobSpritesContainer;
	public UIButton actionButton1;
	public UIButton actionButton2;
	Blob blob;


	public void DisplayWithBlob(Blob blobParam) {
		blob = blobParam;
		bool partnering = (blob != null && blob.GetSpouse().state == Blob.State.Dating);
		statusLabel.text = (blob == null) ? "No Partner" : (partnering ? "Pairing" : "Partner");
		actionButton1.isEnabled = (blob != null && !partnering);
		actionButton2.isEnabled = (blob != null && !partnering);
		DisplayBlobImage();
	}

	void DisplayBlobImage() {
		blobSpritesContainer.transform.DestroyChildren();
		if(blob != null) {
			GameObject blobGameObject = (GameObject)GameObject.Instantiate(blob.gameObject);
			blobGameObject.transform.SetParent(blobSpritesContainer.transform);
			blobGameObject.transform.localPosition = new Vector3(0f, -10f, 0f);
			blobGameObject.transform.localScale = new Vector3(.5f, .5f, 0f);
			Destroy(blobGameObject.transform.Find("FloatingDisplay").gameObject);
			Destroy(blobGameObject.GetComponent("Blob"));
			Destroy(blobGameObject.GetComponent("BoxCollider"));
			Destroy(blobGameObject.GetComponent("BlobDragDropItem"));
			Destroy(blobGameObject.GetComponent("UIButton"));
		}
	}


	public void ViewButtonPressed() {

		HudManager hudManager = GameObject.Find("HudManager").GetComponent<HudManager>();
		hudManager.blobInfoContextMenu.DisplayWithBlob(blob);
	}


	public void BreakButtonPressed() {
		HudManager hudManager = GameObject.Find("HudManager").GetComponent<HudManager>();
		hudManager.popup.Show("Unpair", "Are you sure you want to unpair these blobs?", this, "BreakConfirmed");
	}


	void BreakConfirmed() {
		BreedManager breedManager = GameObject.Find("BreedManager").GetComponent<BreedManager>();
		breedManager.UnPairBlob(blob);
	}
}
