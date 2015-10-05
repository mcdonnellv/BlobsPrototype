using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BlobInfoContextMenu : MonoBehaviour {

	Blob blob;
	public UILabel partnerLabel;
	public UILabel rankLabel;
	public UILabel genderLabel;
	public UILabel qualityLabel;
	public List<UILabel> statLabels;
	public List<UILabel> statNameLabels;
	public GameObject geneGrid;

	public UILabel actionButton1Label;
	public UILabel actionButton2Label;
	public UIButton actionButton1;
	public UIButton actionButton2;
	public UISprite genderSprite;
	public UIButton dismissButton;
	public UIProgressBar progressBar;
	public UIWidget blobSpritesContainer;
	public PartnerDisplayContainer partnerDisplayContainer;
	HudManager hudManager;
	GameManager2 gameManager;
	BreedManager breedManager;
	RoomManager roomManager;




	public void DisplayWithBlob(Blob blobParam) {
		blob = blobParam;

		if(blob.state == Blob.State.Nugget) {
			DisplayNuggetInfo();
			return;
		}


		actionButton1.gameObject.SetActive(true);
		actionButton2.gameObject.SetActive(true);
		partnerDisplayContainer.gameObject.SetActive(true);
		qualityLabel.fontSize = 24;
		rankLabel.fontSize = 24;

		if(blob.hasHatched == false) {
			DisplayEggInfo();
		}
		else {
			partnerLabel.text = (blob.spouseId == -1) ? "No Partner" : "Partner Info";
			genderLabel.text = blob.male ? "Male" : "Female";
			rankLabel.text = "Rank " + blob.level.ToString();
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

			bool isIdle = (blob.state == Blob.State.Idle);
			actionButton1.isEnabled = !isIdle ? false : true;
			actionButton1Label.text = blob.GetActionString();
			actionButton2.isEnabled = !isIdle ? false : true;
			actionButton2Label.text = "Sell +" + blob.sellValue.ToString() + "[gold]";
			Blob spouse = blob.GetSpouse();
			partnerDisplayContainer.DisplayWithBlob(spouse);

			foreach (UILabel l in statNameLabels) 
				l.text = Stat.GetStatIdByIndex(statNameLabels.IndexOf(l)).ToString();

			blob.CalculateStats();
			foreach (UILabel l in statLabels) 
				l.text = blob.stats.values[statLabels.IndexOf(l)].ToString();

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

			//foreach(Stat s in blob.stats) {
				//GameObject statGameObject = (GameObject)GameObject.Instantiate(Resources.Load("Stat Container"));
				//TODO: statGameObject.
			//}
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
		actionButton1Label.text = blob.GetActionString();
		partnerDisplayContainer.gameObject.SetActive(false);
		actionButton2.gameObject.SetActive(false);
		actionButton1.isEnabled = true;
	}

	void DisplayNuggetInfo() {
		rankLabel.text = "[FEC520]Gold Nugget[-]";
		rankLabel.fontSize = 28;
		genderLabel.text = "-";
		qualityLabel.text = "SELL NUGGETS FOR GOLD";
		qualityLabel.fontSize = 18;
		genderSprite.gameObject.SetActive(false);
		actionButton1.gameObject.SetActive(false);
		actionButton2.gameObject.SetActive(true);
		actionButton1.isEnabled = false;
		actionButton2.isEnabled = true;
		actionButton2Label.text = "Sell +" + blob.sellValue.ToString() + "[gold]";
		DisplayBlobImage();
		partnerDisplayContainer.gameObject.SetActive(false);
		partnerDisplayContainer.gameObject.SetActive(false);

		if(IsDisplayed() == false) {
			TweenPosition tp = gameObject.GetComponent<TweenPosition>();
			tp.PlayForward();
			tp.enabled = true;
		}
		
		RoomManager roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
		roomManager.MoveScrollViewToBlob(blob.transform, blob.room);
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
		if(blob.state == Blob.State.Nugget || !blob.hasHatched)
			blobGameObject.transform.localPosition = new Vector3(0f, -10f, 0f);
	}


	public void Dismiss() {
		hudManager.itemInfoPopup.Hide();
		TweenPosition tp = gameObject.GetComponent<TweenPosition>();
		tp.PlayReverse();
		//roomManager.scrollView.GetComponent<UICenterOnChild>().CenterOn(blob.room.transform);
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
		BreedManager breedManager = GameObject.Find("BreedManager").GetComponent<BreedManager>();
		dismissButton.SendMessage("OnClick");
		if(blob.hasHatched)
			breedManager.AttemptBreed(blob, blob.GetSpouse());
		else
			blob.Hatch(true);
	}


	public void ActionButton2Pressed() {
		if(blob.isNugget) {
			SellBlobConfirmed();
			return;
		}
		hudManager.popup.Show("Sell Blob", 
		                      "Are you sure you want to sell this blob for " + blob.sellValue.ToString() + "[gold]?", 
		                      this, "SellBlobConfirmed");
	}


	void SellBlobConfirmed() {
		gameManager.AddGold(blob.sellValue);
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
			bool reverse = (blob.state == Blob.State.Depressed);
			progressBar.value = reverse ? (fraction) : (1f - fraction);
		}
		else if(progressBar.gameObject.activeSelf == true)
			progressBar.gameObject.SetActive(false);

	
	}
}
