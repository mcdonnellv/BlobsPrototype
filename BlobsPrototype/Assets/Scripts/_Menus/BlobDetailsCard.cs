using UnityEngine;
using System.Collections;

public class BlobDetailsCard : MonoBehaviour {
	[HideInInspector] public Blob blob;

	public UILabel nameLabel;
	public UILabel descriptionLabel;
	public UISprite sigilSprite;
	public UISprite colorBG;
	public Transform blobAttachPoint;
	public Transform starSprites;

	public void Setup(Blob blobParam) {
		blob = blobParam;
		blobAttachPoint.transform.DestroyChildren();

		GameObject blobGameObject = blob.gameObject.CreateDuplicateForUi(blobAttachPoint, false);
		nameLabel.text = "NAME";//blob.name;
		descriptionLabel.text = "Description goes here";
		sigilSprite.atlas = HudManager.hudManager.sigilAtlas;
		sigilSprite.spriteName = GlobalDefines.SpriteNameForSigil(blob.sigil);
		sigilSprite.color = ColorDefines.ColorForElement(blob.element);
		colorBG.color = ColorDefines.ColorForElement(blob.element);
		SetStarCount((int)blob.quality);
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
}
