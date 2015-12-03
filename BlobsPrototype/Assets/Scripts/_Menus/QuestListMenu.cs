using UnityEngine;
using System.Collections;

public class QuestListMenu : GenericGameMenu {
	public UIGrid grid;
	public QuestDetailsMenu questDetailsMenu;
	QuestManager questManager;
	Quest selectedQuest;

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
			GameObject gameObject = (GameObject)GameObject.Instantiate(Resources.Load("Quest Cell"));
			QuestCell questCell = gameObject.GetComponent<QuestCell>();
			questCell.transform.parent = grid.transform;
			questCell.transform.localScale = Vector3.one;
			questCell.transform.localPosition = Vector3.zero;
			questCell.SetupBlobCells(quest.blobsAllowed);
			questCell.titleLabel.text = quest.itemName;
			string timeString = "";
			if(quest.days > 0)
				timeString += quest.days.ToString() + " day";
			if(quest.hrs > 0)
				timeString += quest.hrs.ToString() + " hr";
			if(quest.mins > 0)
				timeString += quest.mins.ToString() + " min";
			questCell.durationLabel.text = ColorDefines.ColorToHexString(ColorDefines.goldenTextColor) + timeString + "[-]";
			questCell.rarityLabel.gameObject.SetActive(quest.quality > Quality.Common);
			questCell.icon.spriteName = quest.iconName;
			questCell.icon.atlas = quest.iconAtlas;
		}
		grid.Reposition();
	}


	public void QuestCellPressed(QuestCell questCell) {
		//Invoke("ShowDetails", .2f);
		selectedQuest = questManager.availableQuests[questCell.transform.GetSiblingIndex()];
		questDetailsMenu.Show(this, selectedQuest);
	}


	public void HideDetails() {
		questDetailsMenu.Hide();
	}

	public void Hide() {
		//questDetailsMenu.Hide();
		base.Hide();
	}
}

