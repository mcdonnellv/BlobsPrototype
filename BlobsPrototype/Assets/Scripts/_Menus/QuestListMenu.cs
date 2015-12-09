using UnityEngine;
using System;
using System.Collections;

public class QuestListMenu : GenericGameMenu {
	public UIGrid grid;
	public QuestDetailsMenu questDetailsMenu;
	QuestManager questManager;
	Quest selectedQuest;
	RoomManager _roomManager;
	RoomManager roomManager { get {if(_roomManager == null) _roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>(); return _roomManager; } }

	public void Pressed() {	Show(); }

	public void Show() { 
		base.Show(); 
		window.transform.localScale = new Vector3(1,1,1); 
		SetupQuestCells(); 
	}

	void SetupQuestCells() {
		questManager = GameObject.Find("QuestManager").GetComponent<QuestManager>();
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


	public void QuestCellPressed(QuestCell questCell) {
		selectedQuest = questManager.availableQuests[questCell.transform.GetSiblingIndex()];
		switch(selectedQuest.state) {
		case QuestState.Available:
		case QuestState.Embarked: 
			questDetailsMenu.Show(this, selectedQuest);
			break;
		case QuestState.Completed: 
			questDetailsMenu.QuestCompleted(); 
			questManager.QuestCompleted(selectedQuest);
			SetupQuestCells();
			break;
		}
	}


	public void HideDetails() {
		questDetailsMenu.Hide();
	}

	public void Hide() {
		//questDetailsMenu.Hide();
		base.Hide();
	}

	void Update() {
		foreach(Quest quest in questManager.availableQuests) {
			int index = questManager.availableQuests.IndexOf(quest);
			Transform cellTransform = grid.transform.GetChild(index);
			QuestCell questCell = cellTransform.GetComponent<QuestCell>();

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

