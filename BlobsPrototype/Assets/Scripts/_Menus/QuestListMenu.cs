using UnityEngine;
using System;
using System.Collections;

public class QuestListMenu : GenericGameMenu {
	public UIGrid grid;
	public QuestDetailsMenu questDetailsMenu;
	public UIButton completeButton;
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

			foreach(int blobId in quest.blobIds) 
				if(blobId != -1)
					questCell.DisplayBlobImage(roomManager.GetBlobByID(blobId), quest.blobIds.IndexOf(blobId));

			string timeString = "0 sec";
			switch(quest.state) {
			case QuestState.Embarked : 
				break;
			case QuestState.Available :
				if(quest.days > 0)
					timeString += quest.days.ToString() + " day";
				if(quest.hrs > 0)
					timeString += quest.hrs.ToString() + " hr";
				if(quest.mins > 0)
					timeString += quest.mins.ToString() + " min";
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
		questDetailsMenu.Show(this, selectedQuest, false);
		questDetailsMenu.PopulateWithBlobs();
		switch(selectedQuest.state) {
		case QuestState.Available:
			selectButton.gameObject.SetActive(true);
			completeButton.gameObject.SetActive(false);
			inProgressLabel.gameObject.SetActive(false);
			break;
		case QuestState.Embarked:
			selectButton.gameObject.SetActive(false);
			completeButton.gameObject.SetActive(false);
			inProgressLabel.gameObject.SetActive(true);
			break;
		case QuestState.Completed: 
			selectButton.gameObject.SetActive(false);
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

	public void CompleteQuestPressed() {
		questDetailsMenu.QuestCompleted(); 
		questManager.QuestCompleted(selectedQuest);
		SetupQuestCells();
		SelectFirstQuest();
	}


	void Update() {
		foreach(Transform cellTransform in grid.transform) {
			int index = cellTransform.GetSiblingIndex();
			QuestCell questCell = cellTransform.GetComponent<QuestCell>();
			Quest quest = questManager.availableQuests[index];
			if(quest.state == QuestState.Embarked) {
				string timeString = "";
				if(quest.state == QuestState.Embarked) {
					DateTime now = System.DateTime.Now;
					TimeSpan ts = quest.actionReadyTime - now;
					if(ts.Days > 0)
						timeString += ts.Days.ToString() + " day";
					if(ts.Days == 0 && ts.Hours > 0)
						timeString += (timeString == "" ? "" : "  " ) + ts.Hours.ToString() + " hr";
					if(ts.Days == 0 && ts.Hours == 0 && ts.Minutes > 0)
						timeString += (timeString == "" ? "" : "  " ) + ts.Minutes.ToString() + " min";
					if(ts.Days == 0 && ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds > 0)
						timeString += (timeString == "" ? "" : "  " ) + ts.Seconds.ToString() + " sec";
					if(timeString == "")
						timeString = "0 sec";
				}
				questCell.durationLabel.text = ColorDefines.ColorToHexString(ColorDefines.goldenTextColor) + timeString + "[-]";
			}
			else if(quest.state == QuestState.Completed)
				questCell.durationLabel.text = ColorDefines.ColorToHexString(ColorDefines.goldenTextColor) + "COMPLETE" + "[-]";
		}
	}
}

