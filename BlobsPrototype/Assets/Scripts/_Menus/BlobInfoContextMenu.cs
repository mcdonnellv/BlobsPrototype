using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BlobInfoContextMenu : GenericGameMenu {

	public Blob blob;
	public UILabel actionLabel;
	public UILabel genderLabel;
	public UILabel qualityLabel;
	public UILabel missionsLabel;
	public UILabel elementLabel;
	public UILabel sigilLabel;
	public List<UILabel> statLabels;
	public UIGrid geneGrid;
	public UIGrid statGrid;
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
	HudManager hudManager { get { return HudManager.hudManager; } }
	GameManager2 gameManager { get { return GameManager2.gameManager; } }
	BreedManager breedManager { get { return BreedManager.breedManager; } }
	RoomManager roomManager  { get { return RoomManager.roomManager; } }

	

	public Blob DisplayedBlob() { return blob; }

	public void Show(int blobId) {
		Blob newBlob = roomManager.GetBlobByID(blobId);
		gameObject.SetActive(true);
		if(IsDisplayed() && newBlob != blob) // We are just changing the displayed blobs
			FlashChangeAnim();
		else {
			base.Show();
			window.transform.localScale = new Vector3(1,1,1);
		}

		blob = newBlob;
		actionButton1.gameObject.SetActive(true);
		actionButton2.gameObject.SetActive(true);
		DisplayBlobInfo();
		DisplayBlobImage();
		roomManager.MoveScrollViewToBlob(blob.transform, blob.room);
		blob.room.ShowFloatingSprites(null);
	}


	void DisplayBlobInfo() {
		genderLabel.text = blob.male ? "Male" : "Female";
		qualityLabel.text = ColorDefines.ColorToHexString(ColorDefines.ColorForQuality(blob.quality)) + blob.quality.ToString() + "[-]";
		qualityLabel.gameObject.SetActive(blob.quality != Quality.Common);
		actionLabel.text = blob.GetActionString();
		missionsLabel.text =  "Missions: " + blob.missionCount.ToString();
		genderSprite.spriteName = (blob.male) ? "maleIcon" : "femaleIcon";
		genderSprite.color = (blob.male) ? ColorDefines.maleColor : ColorDefines.femaleColor;
		actionButton1.isEnabled = (blob.state != BlobState.Idle) ? false : true;
		actionButton2.isEnabled = (blob.state != BlobState.Idle) ? false : true;
		actionButton2Label.text = "Sell +1[gold]";
		progressBar.value = 0f;
		UpdateStats();
		geneGrid.transform.DestroyChildren();
		BuildEmptyGeneCells();
		FillGeneCells();
	}


	void UpdateStats() {
		blob.CalculateStats();
		statLabels[0].text = blob.combatStats.health.ToString();
		statLabels[1].text = blob.combatStats.stamina.ToString();
		statLabels[2].text = blob.combatStats.attack.ToString();
		statLabels[3].text = blob.combatStats.armor.ToString();
		elementLabel.text = blob.element.ToString();
		sigilLabel.text = GlobalDefines.StringForSigil(blob.sigil);
		UpdateStatColors();
	}


	void UpdateStatColors() {
		UpdateColorForStatLabel(statLabels[0], blob.combatStats.health, CombatStats.defaultHealth);
		UpdateColorForStatLabel(statLabels[1], blob.combatStats.stamina, CombatStats.defaultStamina);
		UpdateColorForStatLabel(statLabels[2], blob.combatStats.attack, CombatStats.defaultAttack);
		UpdateColorForStatLabel(statLabels[3], blob.combatStats.armor, CombatStats.defaultArmor);
		elementLabel.color = ColorDefines.ColorForElement(blob.combatStats.element);
	}


	void UpdateColorForStatLabel(UILabel statLabel, int value, int defaultValue) {
		statLabel.color = Color.white;
		if(value > defaultValue)
			statLabel.color = ColorDefines.positiveTextColor;
		else if(value < defaultValue)
			statLabel.color = ColorDefines.negativeTextColor;
		else
			statLabel.color = Color.white;
	}
	

	void BuildEmptyGeneCells() {
		for(int i = 0; i < blob.allowedGeneCount; i++) {
			GameObject go = (GameObject)GameObject.Instantiate(Resources.Load("Gene Cell"));
			GeneCell geneCell = go.GetComponent<GeneCell>();
			geneCell.nameLabel.text = "Empty";
			go.transform.parent = geneGrid.transform;
			go.transform.localScale = new Vector3(1f,1f,1f);
			go.transform.localPosition = new Vector3(0f,0f,0f);
			geneCell.Deactivate();
		}
		geneGrid.Reposition();
	}

	void FillGeneCells() {
		foreach(Gene g in blob.genes) {
			int index = blob.genes.IndexOf(g);
			if (index >= geneGrid.transform.childCount)
				break;
			GeneCell geneCell = geneGrid.transform.GetChild(index).GetComponent<GeneCell>();
			GameObject go = g.CreateGeneGameObject(this);
			geneCell.socketSprite.transform.DestroyChildren();
			go.transform.parent = geneCell.socketSprite.transform;
			go.transform.localScale = new Vector3(1f,1f,1f);
			go.transform.localPosition = new Vector3(0f,0f,0f);
			geneCell.nameLabel.text = g.itemName;
			geneCell.nameLabel.color = ColorDefines.ColorForQuality(g.quality);
			if(g.active)
				geneCell.Activate();
			else
				geneCell.Deactivate();
		}
	}


	void DisplayBlobImage() {
		blobSpritesContainer.transform.DestroyChildren();
		blob.CreateDuplicateForUi(blobSpritesContainer.transform, false);
	}


	public void Hide() {
		blob.room.HideFloatingSprites();
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


	public void GeneCellPressed(GeneCell geneCell) {
		GenePointer gp = geneCell.GetGenePointer();
		if(gp == null) {
			//pressed a gene cell that is empty, intention is to add a gene
			if(hudManager.inventoryMenu.mode != InventoryMenu.Mode.AddGene) {
				hudManager.itemInfoPopup.Hide();
				hudManager.inventoryMenu.Show(InventoryMenu.Mode.AddGene);
			}
		}
		else
			GenePressed(gp);
	}


	public void AddGeneToBlob(Gene gene) {
		blob.genes.Add(gene);
		//if(gene.quality == Quality.Bad)
			gene.state = GeneState.Available;
		gene.CheckActivationStatus();
		geneGrid.transform.DestroyChildren();
		BuildEmptyGeneCells();
		FillGeneCells();
	}


	public void GenePressed(GenePointer genePointer) {
		ItemInfoPopup itemInfoPopup = hudManager.itemInfoPopup;
		if(genePointer == null) 
			return;
		itemInfoPopup.defaultStartPosition = PopupPosition.Left1;
		itemInfoPopup.Show(this, genePointer.gene);
	}


	// Update is called once per frame
	void Update () {
		if(blob == null)
			return;

		if(blob.actionDuration.TotalSeconds > 0) {
			System.TimeSpan ts = (blob.actionReadyTime - System.DateTime.Now);
			float fraction = (float)(ts.TotalSeconds / blob.actionDuration.TotalSeconds);
			progressBar.value = 1f - fraction;
			actionLabel.text = blob.GetActionString() + "   " + GlobalDefines.TimeToString(ts);
		}
	}
}
