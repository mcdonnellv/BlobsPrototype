using UnityEngine;
using System.Collections;

public class BlobRosterCell : MonoBehaviour {

	[HideInInspector] public Blob blob;
	public UISprite colorBG;
	public UISprite sigilSprite;
	public Transform starSprites;
	public GameObject container;

	public void DisplayBlobImage() {
		if(blob == null)
			return;
		container.SetActive(true);
		Transform parentSlot = transform.FindChild("AttachPt");
		GameObject blobGameObject = blob.gameObject.CreateDuplicateForUi(parentSlot, false);

		//color the cell bg
		colorBG.color = ColorDefines.ColorForElement(blob.element);
	}

	public void SetStarCount(int count) {
		foreach(Transform starTransform in starSprites) {
			int index = starTransform.GetSiblingIndex();
			UISprite starSprite = starTransform.GetComponent<UISprite>();
			if(index < count) 
				starSprite.color = Color.white;
			else
				starSprite.color = new Color(0f, 0f, 0f, 0.6f);
		}
	}

	public void SetSigil() {
		if(blob == null)
			return;
		sigilSprite.atlas = HudManager.hudManager.sigilAtlas;
		sigilSprite.spriteName = GlobalDefines.SpriteNameForSigil(blob.sigil);
		sigilSprite.color = ColorDefines.ColorForElement(blob.element);
	}

	public void Pressed() {
		BlobRosterMenu blobRosterMenu = gameObject.GetComponentInParent<BlobRosterMenu>();
		if(!blobRosterMenu)
			return;
		blobRosterMenu.BlobCellPressed(this);
	}
}
