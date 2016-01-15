using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class QuestListMenu : GenericGameMenu {
	public UIButton completeButton;
	public UIButton abandonButton;
	public UIButton selectButton;
	public UILabel inProgressLabel;
	public QuestListGrid questGrid;
	public QuestListDetails questDetails;
	QuestManager questManager { get { return QuestManager.questManager; } }
	RoomManager roomManager  { get { return RoomManager.roomManager; } }
	HudManager hudManager  { get { return HudManager.hudManager; } }
	Quest selectedQuest;

	public void QuestRemoved(int index) { questGrid.RemoveQuestCellWithIndex(index); }
	public void QuestCellPressed(QuestCell questCell) { SelectQuestByIndex(questCell.transform.GetSiblingIndex()); }
	void SelectFirstQuest() { PressQuestCellByIndex(0); }
	public void Pressed() {	Show(); }


	public override void Show() { 
		base.Show(); 
		selectButton.gameObject.SetActive(false);
		abandonButton.gameObject.SetActive(false);
		completeButton.gameObject.SetActive(false);
		inProgressLabel.gameObject.SetActive(false);
		questGrid.SetupQuestCells();
		questGrid.repositionNow = true;
	}


	public override void SetDisplayed() {
		base.SetDisplayed();
		SelectFirstQuest();
	}


	void PressQuestCellByIndex(int index) {
		QuestCell questCell = questGrid.GetQuestCellFromIndex(index);
		if(questCell != null) questCell.Pressed();
	}


	public void SelectQuestByIndex(int index) {
		QuestCell questCell = questGrid.GetQuestCellFromIndex(index);
		questCell.newLabel.gameObject.SetActive(false);

		selectedQuest = questManager.availableQuests[index];
		selectedQuest.alreadySeen = true;

		//now display quest details
		questDetails.SetQuest(selectedQuest); 

		//questDetailsMenu.Show(this, selectedQuest, false);
		//questDetailsMenu.transform.localPosition = detailsReferencePoint.transform.localPosition;
		//questDetailsMenu.PopulateWithBlobs();
		UpdateButtonsFromQuestState(selectedQuest.state);
	}


	void UpdateButtonsFromQuestState(QuestState s) {
		selectButton.gameObject.SetActive(false);
		abandonButton.gameObject.SetActive(false);
		completeButton.gameObject.SetActive(false);
		inProgressLabel.gameObject.SetActive(false);
		switch(s) {
		case QuestState.Available:
			selectButton.gameObject.SetActive(true);
			abandonButton.gameObject.SetActive(true);
			break;
		case QuestState.Embarked:
			inProgressLabel.gameObject.SetActive(true);
			break;
		case QuestState.Completed: 
			completeButton.gameObject.SetActive(true);
			break;
		}
	}

	
	public void DismissButtonPressed() { Hide(); }


	public void SelectButtonPressed() {
		hudManager.questDetailsMenu.Show(selectedQuest);
		Hide();
	}


	public void AbandonButtonPressed() {
		if(selectedQuest == null)
			hudManager.ShowError("No quest selected");
		else
			hudManager.popup.Show("Abandon Quest", "Are you sure you want to abandon this quest?", this, "AbandonQuestConfirmed");
	}


	public void AbandonQuestConfirmed() {
		int index = questManager.availableQuests.IndexOf(selectedQuest);
		questManager.availableQuests.Remove(selectedQuest);
		questGrid.RemoveQuestCellWithIndex(index);
		if(questManager.availableQuests.Count == 0)
			return; //show no quests info thing;
		if(index < questManager.availableQuests.Count)
			PressQuestCellByIndex(index);
		else if(index - 1 >= 0)
			PressQuestCellByIndex(index - 1);
	}

	public void CompleteButtonPressed() {
		completeButton.gameObject.SetActive(false);
		questManager.CollectRewardsForQuest(selectedQuest);
	}


	public void RewardsCollected() {
		questDetails.ClearBlobs(); 
		questGrid.SetupQuestCells();  //overkill, just remove 1 cell and adjust
		SelectFirstQuest();
	}


	public void QuestComplete(Quest quest) {
		questGrid.MarkQuestComplete(quest);
		if(quest == selectedQuest)
			UpdateButtonsFromQuestState(quest.state);

		int index = questManager.availableQuests.IndexOf(quest);
		PressQuestCellByIndex(index);
	}


	public void QuestsAdded(List<Quest> quests) {
		questDetails.ClearBlobs(); 
		questGrid.SetupQuestCells();
		int index = questManager.availableQuests.IndexOf(quests[0]);
		SelectQuestByIndex(index);
		if(selectedQuest.type == QuestType.Scouting)
			SelectButtonPressed();
	}


	void Update() {
		questGrid.UpdatePerFrame();
	}

}

