using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class QuestListMenu : GenericGameMenu {
	public UIGrid grid;
	public QuestDetailsMenu questDetailsMenu;
	public UIButton completeButton;
	public UIButton abandonButton;
	public UIButton selectButton;
	public UILabel inProgressLabel;
	QuestManager questManager { get { return QuestManager.questManager; } }
	RoomManager roomManager  { get { return RoomManager.roomManager; } }
	Quest selectedQuest;


	public void Pressed() {	Show(); }


	public void Show() { 
		base.Show(); 
		//window.transform.localScale = new Vector3(1,1,1); 
		SetupQuestCells(); 
		if(questDetailsMenu.IsDisplayed())
			questDetailsMenu.UnSelectQuest();
	}


	void SetupQuestCells() {
		grid.transform.DestroyChildren();
		foreach(Quest quest in questManager.availableQuests) {
			GameObject cellGameObject = (GameObject)GameObject.Instantiate(Resources.Load("Quest Cell"));
			QuestCell questCell = cellGameObject.GetComponent<QuestCell>();
			questCell.transform.parent = grid.transform;
			questCell.transform.localScale = Vector3.one;
			questCell.transform.localPosition = Vector3.zero;
			questCell.SetupBlobCells(quest.blobsRequired);
			questCell.titleLabel.text = quest.itemName;
			questCell.rarityLabel.gameObject.SetActive(quest.quality > Quality.Common);
			questCell.icon.spriteName = quest.iconName;
			questCell.icon.atlas = quest.iconAtlas;
			questCell.newLabel.gameObject.SetActive(!quest.alreadySeen);

			foreach(int blobId in quest.blobIds) 
				if(blobId != -1)
					questCell.DisplayBlobImage(roomManager.GetBlobByID(blobId), quest.blobIds.IndexOf(blobId));

			string timeString = "0 sec";
			switch(quest.state) {
			case QuestState.Embarked : 
				break;
			case QuestState.Available :
				timeString = GlobalDefines.TimeToString(quest.GetActionReadyDuration());
				break;
			}

			questCell.durationLabel.text = ColorDefines.ColorToHexString(ColorDefines.goldenTextColor) + timeString + "[-]";
		}
		grid.Reposition();
	}


	public void SetDisplayed() {
		base.SetDisplayed();
		SelectFirstQuest();
	}


	void SelectFirstQuest() {
		if(grid.transform.childCount > 0){
			QuestCell questCell = grid.transform.GetChild(0).GetComponent<QuestCell>();
			questCell.Pressed();
		}
	}


	public void QuestCellPressed(QuestCell questCell) {
		selectedQuest = questManager.availableQuests[questCell.transform.GetSiblingIndex()];
		selectedQuest.alreadySeen = true;
		questCell.newLabel.gameObject.SetActive(false);
		questDetailsMenu.Show(this, selectedQuest, false);
		questDetailsMenu.PopulateWithBlobs();
		UpdateObjectStates(selectedQuest);
	}


	void UpdateObjectStates(Quest quest) {
		switch(quest.state) {
		case QuestState.Available:
			selectButton.gameObject.SetActive(true);
			abandonButton.gameObject.SetActive(true);
			completeButton.gameObject.SetActive(false);
			inProgressLabel.gameObject.SetActive(false);
			break;
		case QuestState.Embarked:
			selectButton.gameObject.SetActive(false);
			abandonButton.gameObject.SetActive(false);
			completeButton.gameObject.SetActive(false);
			inProgressLabel.gameObject.SetActive(true);
			break;
		case QuestState.Completed: 
			selectButton.gameObject.SetActive(false);
			abandonButton.gameObject.SetActive(false);
			completeButton.gameObject.SetActive(true);
			inProgressLabel.gameObject.SetActive(false);
			break;
		}
	}


	public void HideDetails() {
		questDetailsMenu.Hide();
	}
	

	public void Dismiss() {
		questDetailsMenu.Hide();
		Invoke("Hide", questDetailsMenu.GetAnimationDelay() * .4f);
	}


	public void SelectQuestPressed() {
		questDetailsMenu.SelectQuest();
		Invoke("Hide", questDetailsMenu.GetAnimationDelay() * .4f);
	}


	public void AbandonQuestPressed() {
		HudManager.hudManager.popup.Show("Abandon Quest", "Are you sure you want to abandon this quest?", this, "AbandonQuestConfirmed");
	}


	public void AbandonQuestConfirmed() {
		questManager.availableQuests.Remove(selectedQuest);
		SetupQuestCells();
		questDetailsMenu.Hide();
		SelectFirstQuest();
	}

	public void CompleteQuestPressed() {
		questManager.CollectRewardsForQuest(selectedQuest);
	}


	public void RewardsCollected() {
		questDetailsMenu.ClearBlobs(); 
		SetupQuestCells();
		SelectFirstQuest();
	}


	public void QuestComplete(Quest quest) {
		QuestCell questCell = GetQuestCellFromQuest(quest);
		questCell.durationLabel.text = ColorDefines.ColorToHexString(ColorDefines.goldenTextColor) + "COMPLETE" + "[-]";
		if(quest == selectedQuest)
			UpdateObjectStates(quest);
	}

	public void QuestsAdded(List<Quest> quests) {
		questDetailsMenu.Hide();
		questDetailsMenu.ClearBlobs(); 
		Invoke("SetupQuestCells", .5f);
	}


	QuestCell GetQuestCellFromQuest(Quest quest) {
		int index = questManager.availableQuests.IndexOf(quest);
		Transform cellTransform = grid.transform.GetChild(index);
		QuestCell questCell = cellTransform.GetComponent<QuestCell>();
		return questCell;
	}


	void Update() {
		foreach(Transform cellTransform in grid.transform) {
			int index = cellTransform.GetSiblingIndex();
			QuestCell questCell = cellTransform.GetComponent<QuestCell>();
			Quest quest = questManager.availableQuests[index];
			if(quest.state == QuestState.Embarked) {
				string timeString = "";
				if(quest.state == QuestState.Embarked) 
					timeString = GlobalDefines.TimeToString(quest.actionReadyTime - System.DateTime.Now);
				questCell.durationLabel.text = ColorDefines.ColorToHexString(ColorDefines.goldenTextColor) + timeString + "[-]";
			}
		}
	}
}

