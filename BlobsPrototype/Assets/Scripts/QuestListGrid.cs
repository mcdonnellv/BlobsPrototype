using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class QuestListGrid : UIGrid {

	QuestManager questManager { get { return QuestManager.questManager; } }
	RoomManager roomManager  { get { return RoomManager.roomManager; } }

	void DestroySampleCells() {
		for (int i = 0; i < transform.childCount; i++) {
			QuestCell cell = transform.GetChild(i).GetComponent<QuestCell>();
			if(cell != null && cell.questId == -1) {
				i--;
				DestroyImmediate(cell.gameObject);
			}
		}
	}

	public void SetupQuestCells() {
		bool reposition = false;
		DestroySampleCells();
		
		foreach(Quest quest in questManager.availableQuests) {
			QuestCell questCell = GetQuestCellFromQuest(quest);
			
			if(questCell != null) {
				if(questCell.questId == quest.id) {
					SetBlobImages(quest, questCell);
					continue;
				}
				else
					DestroyImmediate(questCell.gameObject);
			}
			
			reposition = true;
			GameObject cellGameObject = (GameObject)GameObject.Instantiate(Resources.Load("Quest Cell"));
			questCell = cellGameObject.GetComponent<QuestCell>();
			questCell.transform.parent = transform;
			questCell.transform.localScale = Vector3.one;
			questCell.transform.localPosition = Vector3.zero;
			questCell.SetupBlobCells(quest.blobsRequired);
			questCell.titleLabel.text = quest.itemName;
			questCell.rarityLabel.gameObject.SetActive(quest.quality > Quality.Common);
			questCell.icon.spriteName = quest.iconName;
			questCell.icon.atlas = quest.iconAtlas;
			questCell.newLabel.gameObject.SetActive(!quest.alreadySeen);
			questCell.questId = quest.id;
			SetBlobImages(quest, questCell);
			
			string timeString = "0 sec";
			switch(quest.state) {
			case QuestState.Embarked : break;
			case QuestState.Available : timeString = GlobalDefines.TimeToString(quest.GetActionReadyDuration()); break;
			}
			
			questCell.durationLabel.text = ColorDefines.ColorToHexString(ColorDefines.goldenTextColor) + timeString + "[-]";
		}
		
		if(reposition)
			base.Reposition();
	}


	void SetBlobImages(Quest quest, QuestCell questCell) {
		foreach(int blobId in quest.blobIds) 
			if(blobId != -1)
				questCell.DisplayBlobImage(roomManager.GetBlobByID(blobId), quest.blobIds.IndexOf(blobId));
	}


	public QuestCell GetQuestCellFromQuest(Quest quest) {
		int index = questManager.availableQuests.IndexOf(quest);
		return GetQuestCellFromIndex(index);
	}


	public QuestCell GetQuestCellFromIndex(int index) {
		if(index >= 0 && index < transform.childCount) {
			Transform cellTransform = transform.GetChild(index);
			QuestCell questCell = cellTransform.GetComponent<QuestCell>();
			return questCell;
		}
		return null;
	}


	public void RemoveQuestCellWithIndex(int index) {
		QuestCell questCell = GetQuestCellFromIndex(index);
		DestroyImmediate(questCell.gameObject);
		base.Reposition();
	}


	public void MarkQuestComplete(Quest quest) {
		QuestCell questCell = GetQuestCellFromQuest(quest);
		questCell.durationLabel.text = ColorDefines.ColorToHexString(ColorDefines.goldenTextColor) + "COMPLETE" + "[-]";
	}


	public void UpdatePerFrame() {
		foreach(Transform cellTransform in transform) {
			int index = cellTransform.GetSiblingIndex();
			QuestCell questCell = cellTransform.GetComponent<QuestCell>();
			Quest quest = questManager.availableQuests[index];
			if(index >= questManager.availableQuests.Count) {
				DestroyImmediate(questCell.gameObject);
				break;
			}
			
			if(quest.state == QuestState.Embarked) {
				string timeString = "";
				if(quest.state == QuestState.Embarked) 
					timeString = GlobalDefines.TimeToString(quest.actionReadyTime - System.DateTime.Now);
				questCell.durationLabel.text = ColorDefines.ColorToHexString(ColorDefines.goldenTextColor) + timeString + "[-]";
			}
		}
	}

}