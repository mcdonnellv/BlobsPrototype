using UnityEngine;
using System.Collections;

public class BlobFloatingDisplay : MonoBehaviour {
	public UIProgressBar progressBar;
	public UILabel stateLabel;
	public UILabel levelLabel;
	public UIPanel spritePanel;
	public Blob blob;
	public UISprite genderSprite;
	public UISprite heartSprite;
	public UISprite harvestSprite;
	public UISprite levelSprite;
	Blob blobDragged;


	// Use this for initialization
	void Start () {
		stateLabel.gameObject.SetActive(true);
		harvestSprite.gameObject.SetActive(false);
		stateLabel.text = "";
		levelLabel.text = "";
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


	public void ShowHeartSprite() {
		if (blob.spouseId == blobDragged.id || (blobDragged == blob && blob.spouseId != -1))
			heartSprite.gameObject.SetActive(true);
	}

	public void ShowLevelSprite() {
		levelSprite.gameObject.SetActive(true);
	}

	public void ShowHarvestSprite() {
		harvestSprite.gameObject.SetActive(true);
	}


	public void HideHarvestSprite() {
		harvestSprite.gameObject.SetActive(false);
	}

	public void ShowBlobInfo(Blob blobDraggedParam) {
		blobDragged = blobDraggedParam;
		if(blob.isNugget || !blob.hasHatched || 
		   blobDragged.isNugget || !blobDragged.hasHatched || 
		   blobDragged.isInfant || blob.isInfant) {
			if(blob != blobDragged)
				blob.gameObject.GetComponent<UIWidget>().alpha = .5f;
			return;
		}

		ShowSprites();
		bool greyOut = false;

		if(blobDragged.spouseId == -1) {
			if(blob.spouseId != -1) 
				greyOut = true;
		}
		else {
			if(blob.id != blobDragged.id && blob.id != blobDragged.spouseId)
				greyOut = true;
		}

		levelLabel.gameObject.SetActive(true);
		levelLabel.text = blob.level.ToString();

		if(blob == blobDragged)
			greyOut = false;

		if(greyOut) {
			genderSprite.color = Color.white;
			blob.gameObject.GetComponent<UIWidget>().alpha = .5f;
		}
		else {
			blob.gameObject.GetComponent<UIWidget>().alpha = 1f;
		}
	}


	public void ShowSprites() {
		ShowGenderSprite();
		ShowHeartSprite();
		ShowLevelSprite();
		spritePanel.gameObject.GetComponent<UIGrid>().Reposition();
	}


	public void HideBlobInfo() {
		HideSprites();
		levelLabel.gameObject.SetActive(false);
		blob.gameObject.GetComponent<UIWidget>().alpha = 1f;
	}


	public void HideSprites() {
		genderSprite.gameObject.SetActive(false);
		heartSprite.gameObject.SetActive(false);
		levelSprite.gameObject.SetActive(false);
	}


	// Update is called once per frame
	void Update () {

		if(progressBar == null || blob == null)
			return;

		if(blob.actionDuration.TotalSeconds > 0 && Blob.ShouldDisplayBarForState(blob.state)) {
			if(progressBar.gameObject.activeSelf == false ) {
				progressBar.gameObject.SetActive(true);
				genderSprite.gameObject.SetActive(false);
				stateLabel.gameObject.SetActive(true);
				stateLabel.text = blob.GetActionString().ToUpper();
			}
			System.TimeSpan ts = (blob.actionReadyTime - System.DateTime.Now);
			float fraction = (float)(ts.TotalSeconds / blob.actionDuration.TotalSeconds);
			bool reverse = (blob.state == Blob.State.Depressed);
			progressBar.value = reverse ? (fraction) : (1f - fraction);
		}
		else if(progressBar.gameObject.activeSelf == true) {
			progressBar.gameObject.SetActive(false);
			stateLabel.text = "";
			if(Blob.ShouldDisplayHarvestSpriteForState(blob.state))
				ShowHarvestSprite();
		}
	}
}
