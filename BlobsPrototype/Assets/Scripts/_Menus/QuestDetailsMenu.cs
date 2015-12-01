using UnityEngine;
using System.Collections;

public class QuestDetailsMenu : GenericGameMenu {
	public QuestListMenu questListMenu;
	public UILabel titleLabel;
	public UILabel descriptionLabel;
	public UILabel objectiveLabel;
	public UISprite icon;
	public UILabel durationLabel;
	public UILabel rarityLabel;
	public Quest quest;

	public void Pressed() {	Show(); }

	public void Show(Quest questParam) { 
		base.Show(); 
		window.transform.localScale = new Vector3(1,1,1);
		quest = questParam;
		Setup();
	}

	void Setup() {
		titleLabel.text = quest.itemName;
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
	}

	public void Hide() {
		questListMenu.Invoke("Show", .2f);
		base.Hide();
	}
}
