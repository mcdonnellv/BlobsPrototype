using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class QuestDetailsMenu : GenericGameMenu {
	public QuestListMenu questListMenu;
	public UILabel descriptionLabel;
	public UILabel objectiveLabel;
	public UISprite icon;
	public UILabel durationLabel;
	public UILabel rarityLabel;
	public UILabel infoLabel;
	public UIButton selectButton;
	public GameObject blobContainer;
	public UIWidget objectiveContainer;
	public UIButton rewardsButton;
	public UIGrid blobGrid;
	public Quest quest;
	bool questSelected = false;
	int defaultWindowHeight = 500;
	HudManager hudManager { get { return HudManager.hudManager; } }

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
		int blobsRequired = quest.blobsRequired;
		int index = 0;
		foreach(Transform child in blobGrid.transform) {
			if(index < quest.blobsRequired) {
				List<UISprite> sprites = child.gameObject.GetComponentsInChildren<UISprite>(true).ToList();
				sprites[0].color = ColorDefines.ColorForElement(quest.elementRequirements[index]);
			}
			child.gameObject.SetActive(blobsRequired > 0);
			blobsRequired--;
			index++;
		}

		ClearBlobs();
		blobGrid.Reposition();
		blobContainer.SetActive(false);
		objectiveContainer.gameObject.SetActive(true);

		switch(quest.state) {
		case QuestState.Embarked: 
			selectButton.isEnabled = false; 
			selectButton.GetComponentInChildren<UILabel>().text = "IN PROGRESS";
			break;
		default: 
			selectButton.isEnabled = true; 
			selectButton.GetComponentInChildren<UILabel>().text = "SELECT";
			break;
		}

		UpdateInfoText();
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
		objectiveContainer.gameObject.SetActive(false);
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
		objectiveContainer.gameObject.SetActive(true);
		animationWindow.PlayReverse();
		owner.Invoke("Show", GetAnimationDelay()/2);
		hudManager.dragToUi = false;
		Hide();
	}


	public void ResizeWindow() {
		//int height = defaultWindowHeight;
		//if(!HasBeenSelected())
			//height = 534;
		//window.GetComponent<UISprite>().height = height;
	}


	public void BlobAddedToContainer(BlobDragDropContainer blobDragDropContainer) {
		Blob blobAdded = blobDragDropContainer.GetComponentInChildren<Blob>();
		foreach(Transform child in blobGrid.transform) {
			if(child == blobDragDropContainer.transform)
				continue;
			Blob blob = child.GetComponentInChildren<Blob>();
			if(blob != null) 
				if(blob.id == blobAdded.id) 

					GameObject.Destroy(blob.gameObject);
				

		}
		quest.AddBlob(blobAdded.id, blobDragDropContainer.transform.GetSiblingIndex());
		UpdateInfoText();
	}


	public void BlobRemovedFromContainer(int blobId) {
		quest.RemoveBlob(blobId);
		UpdateInfoText();

	}


	void UpdateInfoText() {
		if(quest.blobIds.Count < quest.blobsRequired)
			infoLabel.text = "DRAG BLOBS TO FORM A PARTY";
		else if(quest.IsHighYield())
			infoLabel.text = "REWARD YIELD: HIGH";
		else
			infoLabel.text = "REWARD YIELD: LOW";
	}


	List<int> GetChosenBlobs() {
		List<int> blobs = new List<int>();
		foreach(Transform child in blobGrid.transform) {
			Blob blob = child.GetComponentInChildren<Blob>();
			if(blob != null)
				blobs.Add(blob.id);
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
		List<int> blobs = GetChosenBlobs();
		if(blobs.Count != quest.blobsRequired) {
			hudManager.popup.Show("Quest", "Add more blobs to start this quest");
			return;
		}
		quest.Start(GetChosenBlobs());
		Hide();
	}


	public void QuestCompleted(){
		ClearBlobs();
	}


	public override void Cleanup() {
		questSelected = false;
		hudManager.dragToUi = false;
		base.Cleanup();
	}

}
