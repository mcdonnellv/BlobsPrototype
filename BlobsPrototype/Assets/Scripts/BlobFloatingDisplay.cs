using UnityEngine;
using System.Collections;

public class BlobFloatingDisplay : MonoBehaviour {
	public UIProgressBar progressBar;
	public UILabel stateLabel;
	public Blob blob;
	public UISprite genderSprite;
	public UISprite sigilSprite;
	public UISprite harvestSprite;
	public UISprite spriteBG;
	public UIWidget widget;
	Blob blobDragged;
	bool spritesShowing = false;


	// Use this for initialization
	void Start () {
		stateLabel.gameObject.SetActive(true);
		harvestSprite.gameObject.SetActive(false);
		stateLabel.text = "";
		progressBar.gameObject.SetActive(false);
		HideSprites();
	}


	public void ShowGenderSprite() {
		genderSprite.gameObject.SetActive(true);
		if(blob.male) {
			genderSprite.spriteName = "maleIcon";
			genderSprite.color = ColorDefines.maleColor;
		}
		else {
			genderSprite.spriteName = "femaleIcon";
			genderSprite.color = ColorDefines.femaleColor;
		}
	}

	public void ShowSigilSprite() {
		sigilSprite.gameObject.SetActive(true);
		sigilSprite.atlas = HudManager.hudManager.sigilAtlas;
		sigilSprite.spriteName = GlobalDefines.SpriteNameForSigil(blob.sigil);
		sigilSprite.color = ColorDefines.ColorForElement(blob.element);
	}


	public void ShowHarvestSprite() { harvestSprite.gameObject.SetActive(true); }
	public void HideHarvestSprite() { harvestSprite.gameObject.SetActive(false); }

	public void ShowBlobInfo(Blob blobDraggedParam) {
		blobDragged = blobDraggedParam;
		if(!blob.hasHatched || !blobDragged.hasHatched) {
			if(blob != blobDragged)
				widget.alpha = .5f;
			return;
		}

		ShowSprites();
		bool greyOut = false;

		if(blob == blobDragged)
			greyOut = false;

		if(greyOut) {
			genderSprite.color = Color.white;
			widget.alpha = .5f;
		}
		else {
			widget.alpha = 1f;
		}
	}


	public void ShowBlobInfo() {
		if(!blob.hasHatched) 
			return;
		ShowSprites();
	}


	public void ShowSprites() {
		spritesShowing = true;
		spriteBG.gameObject.SetActive(true);
		ShowGenderSprite();
		ShowSigilSprite();
	}


	public void HideBlobInfo() {
		HideSprites();
		widget.alpha = 1f;
	}


	public void HideSprites() {
		spritesShowing = false;
		spriteBG.gameObject.SetActive(false);
		genderSprite.gameObject.SetActive(false);
		sigilSprite.gameObject.SetActive(false);
	}


	bool ShouldDisplayBarForState(BlobState blobState) {
		switch(blobState) {
		case BlobState.Breeding:
		case BlobState.Hatching: 
		case BlobState.Working: 	
		case BlobState.Questing:
		case BlobState.QuestComplete:
		case BlobState.Recovering:
			return true;
		}
		return false;
	}

	bool ShouldDisplayHarvestSpriteForState(BlobState blobState) {
		switch(blobState) {
		case BlobState.WorkingReady:
		case BlobState.HatchReady: return true;
		}
		return false;
	}


	// Update is called once per frame
	void Update () {
		if(progressBar == null || blob == null)
			return;

		if(spritesShowing) {
			progressBar.gameObject.SetActive(false);
			stateLabel.text = "";
			return;
		}

		if(blob.actionDuration.TotalSeconds > 0 && ShouldDisplayBarForState(blob.state)) {
			if(progressBar.gameObject.activeSelf == false ) {
				progressBar.gameObject.SetActive(true);
				genderSprite.gameObject.SetActive(false);
				stateLabel.gameObject.SetActive(true);
			}
			System.TimeSpan ts = (blob.actionReadyTime - System.DateTime.Now);
			float fraction = (float)(ts.TotalSeconds / blob.actionDuration.TotalSeconds);
			progressBar.value = (1f - fraction);
			stateLabel.text = blob.GetActionString() + "\n" +  GlobalDefines.TimeToString(ts);
		}
		else {
			if(blob.state == BlobState.QuestComplete) {
				stateLabel.text = blob.GetActionString();
			}
			else if(progressBar.gameObject.activeSelf == true) {
				progressBar.gameObject.SetActive(false);
				stateLabel.text = "";
				if(ShouldDisplayHarvestSpriteForState(blob.state))
					ShowHarvestSprite();
			}
		}
	}
}
