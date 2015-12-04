using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestDetailsMenu : GenericGameMenu {
	public QuestListMenu questListMenu;
	public UILabel descriptionLabel;
	public UILabel objectiveLabel;
	public UISprite icon;
	public UILabel durationLabel;
	public UILabel rarityLabel;
	public UIButton selectButton;
	public GameObject blobContainer;
	public UIButton rewardsButton;
	public UIGrid blobGrid;
	public Quest quest;
	bool questSelected = false;
	int defaultWindowHeight = 578;
	HudManager _hudManager;
	HudManager hudManager { get {if(_hudManager == null) _hudManager = GameObject.Find("HudManager").GetComponent<HudManager>(); return _hudManager; } }
	
	[HideInInspector] public GenericGameMenu owner = null;

	public void GameMenuClosing(GenericGameMenu menu) { if(menu == owner && !HasBeenSelected()) Hide();}
	public void Pressed() {	Show(); }
	public bool HasBeenSelected() {return questSelected; }

	public void Show(GenericGameMenu caller, Quest questParam) {
		gameObject.SetActive(true);
		if(IsDisplayed() && owner == caller && quest != questParam) // We are just changing the displayed info
			FlashChangeAnim();
		else 
			base.Show(); 
		owner = caller;
		quest = questParam;
		Setup();
	}

	void Setup() {
		headerLabel.text = quest.itemName;
		icon.spriteName = quest.iconName;
		icon.atlas = quest.iconAtlas;
		descriptionLabel.text = quest.description;
		rarityLabel.gameObject.SetActive(quest.quality > Quality.Common);
		string timeString = "";
		if(quest.days > 0)
			timeString += quest.days.ToString() + " day";
		if(quest.hrs > 0)
			timeString += quest.hrs.ToString() + " hr";
		if(quest.mins > 0)
			timeString += quest.mins.ToString() + " min";
		durationLabel.text = ColorDefines.ColorToHexString(ColorDefines.goldenTextColor) + timeString + "[-]";
		switch(quest.type) {
		case QuestType.Combat: objectiveLabel.text = ColorDefines.ColorToHexString(ColorDefines.goldenTextColor) + "Objective: [-] Eliminate the monster"; break;
		case QuestType.Scouting:
		case QuestType.Gathering: objectiveLabel.text = ColorDefines.ColorToHexString(ColorDefines.goldenTextColor) + "Objective: [-] Deploy blobs and await their return"; break;
		}

		blobContainer.SetActive(true);
		int blobsRequired = quest.blobsAllowed;
		foreach(Transform child in blobGrid.transform) {
			child.gameObject.SetActive(blobsRequired > 0);
			blobsRequired--;
		}

		blobGrid.Reposition();
		blobContainer.SetActive(false);
		selectButton.gameObject.SetActive(true);
		rewardsButton.gameObject.SetActive(true);
		ResizeWindow();
	}


	public void SelectQuest() {
		questSelected = true;
		owner.Hide();
		animationWindow.PlayReverse();
		Invoke("ChangePositionToRight", GetAnimationDelay());
		hudManager.dragToUi = true;
	}


	public void ViewRewards() {
		hudManager.potentialLootMenu.Show(quest);
	}


	public void ChangePositionToRight() {
		blobContainer.SetActive(true);
		selectButton.gameObject.SetActive(false);
		rewardsButton.gameObject.SetActive(false);
		ChangePosition(PopupPosition.Right1);
		ResizeWindow();
		animationWindow.PlayForward();
	}


	public void ChangePositionToLeft() {
		ChangePosition(defaultStartPosition);
		ResizeWindow();
		animationWindow.PlayForward();
	}


	public void UnSelectQuest() {
		questSelected = false;
		blobContainer.SetActive(false);
		selectButton.gameObject.SetActive(true);
		rewardsButton.gameObject.SetActive(true);
		animationWindow.PlayReverse();
		owner.Invoke("Show", GetAnimationDelay()/2);
		hudManager.dragToUi = false;
		Hide();
	}


	public void ResizeWindow() {
		int height = defaultWindowHeight;
		if(!HasBeenSelected())
			height -= 50;
		window.GetComponent<UISprite>().height = height;
	}


	public void BlobAddedToContainer(BlobDragDropContainer blobDragDropContainer) {
		Blob blobAdded = blobDragDropContainer.GetComponentInChildren<Blob>();

		foreach(Transform child in blobGrid.transform) {
			if(child == blobDragDropContainer.transform)
				continue;
			Blob blob = child.GetComponentInChildren<Blob>();
			if(blob != null) {
				if(blob.id == blobAdded.id)
					GameObject.Destroy(blob.gameObject);
			}
		}
	}

	List<Blob> GetChosenBlobs() {
		List<Blob> blobs = new List<Blob>();
		foreach(Transform child in blobGrid.transform) {
			Blob blob = child.GetComponentInChildren<Blob>();
			if(blob != null)
				blobs.Add(blob);
		}

		return blobs;
	}

	void ClearBlobs() {
		foreach(Transform child in blobGrid.transform) {
			Blob blob = child.GetComponentInChildren<Blob>();
			if(blob != null) 
					GameObject.Destroy(blob.gameObject);
		}
	}


	public void Depart() {
		//simulate mission success
		List<Blob> blobs = GetChosenBlobs();
		if(blobs.Count != quest.blobsAllowed) {
			hudManager.popup.Show("Quest", "Add more blobs to start this quest");
			return;
		}

		foreach(Blob blob in blobs)
			blob.missionCount++;
		Hide();
		hudManager.popup.Show("Quest", "Quest completed!", this, "GiveReward");
		ClearBlobs();
	}

	public void GiveReward() {
		hudManager.lootMenu.Show(quest);
	}


	public override void Cleanup() {
		questSelected = false;
		base.Cleanup();
	}

}
