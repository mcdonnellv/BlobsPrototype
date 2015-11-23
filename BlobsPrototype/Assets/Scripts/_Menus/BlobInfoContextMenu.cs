using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BlobInfoContextMenu : GenericGameMenu {

	public Blob blob;
	public UILabel partnerLabel;
	public UILabel rankLabel;
	public UILabel genderLabel;
	public UILabel qualityLabel;
	public List<UILabel> statLabels;
	public GameObject geneGrid;
	public GameObject statGrid;

	public UILabel actionButton1Label;
	public UILabel actionButton2Label;
	public UIButton actionButton1;
	public UIButton actionButton2;
	public UISprite genderSprite;
	public UIButton dismissButton;
	public UIProgressBar progressBar;
	public UIWidget blobSpritesContainer;
	public UISprite changeFlashObject;
	public TweenAlpha changeFlashAnim;
	HudManager hudManager;
	GameManager2 gameManager;
	BreedManager breedManager;
	RoomManager roomManager;


	public Blob DisplayedBlob() { return blob; }
	public bool IsDisplayed() { return (transform.localPosition == ((TweenPosition)animationWindow).to && animationWindow.enabled == false); }

	public void Show(Blob blobParam) {
		gameObject.SetActive(true);
		if(IsDisplayed() && blobParam != blob) // We are just changing the displayed blobs
			FlashChangeAnim();
		else {
			base.Show();
			window.transform.localScale = new Vector3(1,1,1);
		}

		hudManager.itemInfoPopup.Hide();
		blob = blobParam;
		actionButton1.gameObject.SetActive(true);
		actionButton2.gameObject.SetActive(true);
		qualityLabel.fontSize = 24;
		rankLabel.fontSize = 24;

		if(blob.hasHatched == false)
			DisplayEggInfo();
		else
			DisplayBlobInfo();

		DisplayBlobImage();
		RoomManager roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
		roomManager.MoveScrollViewToBlob(blob.transform, blob.room);
	}


	void DisplayEggInfo() {
		rankLabel.text = "Unknown";
		genderLabel.text = "Unknown";
		qualityLabel.text = ColorDefines.ColorToHexString(ColorDefines.ColorForQuality(blob.quality)) + blob.quality.ToString() + "[-]";
		genderSprite.gameObject.SetActive(false);
		actionButton2.gameObject.SetActive(false);
		actionButton1.isEnabled = true;
	}


	void DisplayBlobInfo() {
		partnerLabel.text = (blob.spouseId == -1) ? "No Partner" : "Partner Info";
		genderLabel.text = blob.male ? "Male" : "Female";
		qualityLabel.text = ColorDefines.ColorToHexString(ColorDefines.ColorForQuality(blob.quality)) + blob.quality.ToString() + "[-]";
		genderSprite.gameObject.SetActive(true);
		genderSprite.spriteName = (blob.male) ? "maleIcon" : "femaleIcon";
		genderSprite.color = (blob.male) ? ColorDefines.maleColor : ColorDefines.femaleColor;
		actionButton1.isEnabled = (blob.state != BlobState.Idle) ? false : true;
		actionButton2.isEnabled = (blob.state != BlobState.Idle) ? false : true;
		actionButton2Label.text = "Sell +1[gold]";
		Blob spouse = blob.GetSpouse();
		blob.CalculateStats();
		statLabels[0].text = blob.combatStats.health.ToString();
		statLabels[1].text = blob.combatStats.stamina.ToString();
		statLabels[2].text = blob.combatStats.attack.ToString();
		statLabels[3].text = blob.combatStats.armor.ToString();
		
		foreach(Transform c in geneGrid.transform) {
			c.DestroyChildren();
			c.gameObject.SetActive((c.GetSiblingIndex() < blob.allowedGeneCount));
		}
		
		foreach(Gene g in blob.genes) {
			if (blob.genes.IndexOf(g) >= blob.allowedGeneCount)
				break;
			Transform parentSocket = geneGrid.transform.GetChild(blob.genes.IndexOf(g));
			parentSocket.DestroyChildren();
			GameObject go = g.CreateGeneGameObject();
			go.transform.parent = parentSocket;
			go.transform.localScale = new Vector3(1f,1f,1f);
			go.transform.localPosition = new Vector3(0f,0f,0f);
		}
	}


	void DisplayBlobImage() {
		blobSpritesContainer.transform.DestroyChildren();
		GameObject blobGameObject = (GameObject)GameObject.Instantiate(blob.gameObject);
		blobGameObject.transform.SetParent(blobSpritesContainer.transform);
		blobGameObject.transform.localPosition = new Vector3(0f, -18f, 0f);
		blobGameObject.transform.localScale = blob.transform.localScale;
		Destroy(blobGameObject.transform.Find("FloatingDisplay").gameObject);
		Destroy(blobGameObject.GetComponent("Blob"));
		Destroy(blobGameObject.GetComponent("BoxCollider"));
		Destroy(blobGameObject.GetComponent("BlobDragDropItem"));
		Destroy(blobGameObject.GetComponent("UIButton"));
		if(!blob.hasHatched)
			blobGameObject.transform.localPosition = new Vector3(0f, -10f, 0f);
	}


	public void Hide() {
		hudManager.itemInfoPopup.Hide();
		base.Hide();
	}

	public void FlashChangeAnim() {
		changeFlashObject.gameObject.SetActive(true);
		changeFlashAnim.Play();
		changeFlashAnim.enabled = true;
	} 


	public void ActionButton1Pressed() {
		hudManager.inventoryMenu.Show(InventoryMenu.Mode.Feed);
		actionButton1.isEnabled = false;
		actionButton2.isEnabled = false;
	}


	public void ActionButton2Pressed() {
		hudManager.popup.Show("Sell Blob", 
		                      "Are you sure you want to sell this blob for 1[gold]?", 
		                      this, "SellBlobConfirmed");
	}


	void SellBlobConfirmed() {
		gameManager.AddGold(1);
		blob.room.DeleteBlob(blob);
		UIButton.current = null;
		dismissButton.SendMessage("OnClick");
	}
	

	// Use this for initialization
	void Start () {
		hudManager = GameObject.Find("HudManager").GetComponent<HudManager>();
		gameManager = GameObject.Find("GameManager2").GetComponent<GameManager2>();
		breedManager = GameObject.Find("BreedManager").GetComponent<BreedManager>();
		roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
	}
	
	// Update is called once per frame
	void Update () {
		if(blob == null)
			return;

		if(blob.actionDuration.TotalSeconds > 0) {
			if(progressBar.gameObject.activeSelf == false)
				progressBar.gameObject.SetActive(true);
			System.TimeSpan ts = (blob.actionReadyTime - System.DateTime.Now);
			float fraction = (float)(ts.TotalSeconds / blob.actionDuration.TotalSeconds);
			progressBar.value = 1f - fraction;
		}
		else if(progressBar.gameObject.activeSelf == true)
			progressBar.gameObject.SetActive(false);
	}
}
