using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BlobInfoContextMenu : MonoBehaviour {

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





	public void DisplayWithBlob(Blob blobParam) {

		if(IsDisplayed()) 
			QuickAnimOutIn();

		hudManager.itemInfoPopup.Hide();
		blob = blobParam;
		actionButton1.gameObject.SetActive(true);
		actionButton2.gameObject.SetActive(true);
		qualityLabel.fontSize = 24;
		rankLabel.fontSize = 24;

		if(blob.hasHatched == false) {
			DisplayEggInfo();
		}
		else {
			partnerLabel.text = (blob.spouseId == -1) ? "No Partner" : "Partner Info";
			genderLabel.text = blob.male ? "Male" : "Female";
			qualityLabel.text = "[" + ColorToHex(ColorDefines.ColorForQuality(blob.quality)) + "]" + blob.quality.ToString() + "[-]";
			genderSprite.gameObject.SetActive(true);
			if(blob.male) {
				genderSprite.spriteName = "maleIcon";
				genderSprite.color = ColorDefines.maleColor;
			}
			
			if(blob.female) {
				genderSprite.spriteName = "femaleIcon";
				genderSprite.color = ColorDefines.femaleColor;
			}

			bool isIdle = (blob.state == BlobState.Idle);
			actionButton1.isEnabled = !isIdle ? false : true;
			actionButton2.isEnabled = !isIdle ? false : true;
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

		DisplayBlobImage();


		if(IsDisplayed() == false) {
			TweenPosition tp = gameObject.GetComponent<TweenPosition>();
			tp.PlayForward();
			tp.enabled = true;
		}

		RoomManager roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
		roomManager.MoveScrollViewToBlob(blob.transform, blob.room);
	}


	void DisplayEggInfo() {
		rankLabel.text = "Unknown";
		genderLabel.text = "Unknown";
		qualityLabel.text = "[" + ColorToHex(ColorDefines.ColorForQuality(blob.quality)) + "]" + blob.quality.ToString() + "[-]";
		genderSprite.gameObject.SetActive(false);
		actionButton2.gameObject.SetActive(false);
		actionButton1.isEnabled = true;
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


	public void Dismiss() {
		hudManager.itemInfoPopup.Hide();
		TweenPosition tp = gameObject.GetComponent<TweenPosition>();
		tp.PlayReverse();
		//roomManager.scrollView.GetComponent<UICenterOnChild>().CenterOn(blob.room.transform);
	}

	public void QuickAnimOutIn() {
		changeFlashObject.gameObject.SetActive(true);
		changeFlashAnim.Play();
		changeFlashAnim.enabled = true;
	}


	public Blob DisplayedBlob() { return blob; }


	public bool IsDisplayed() {
		TweenPosition tp = gameObject.GetComponent<TweenPosition>();
		return (transform.localPosition == tp.to && tp.enabled == false);
	}


	string ColorToHex(Color32 color) {
		string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
		return hex;
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
