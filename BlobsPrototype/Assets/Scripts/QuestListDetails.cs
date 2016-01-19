using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class QuestListDetails : MonoBehaviour {
	Quest quest;

	public UILabel monsterLabel;
	public UILabel titleLabel;
	public UILabel durationLabel;
	public UILabel descriptionLabel;
	public UILabel rarityLabel;
	public UILabel numBlobsNeededLabel;
	public UILabel bonusLabel;
	public UISprite icon;
	public UIGrid blobGrid;
	public PotentialLootMenu potentialLootMenu;

	MonsterManager monsterManager  { get { return MonsterManager.monsterManager; } }

	public void SetQuest(Quest q) { 
		quest = q;
		Populate();
		UpdateBonuses();
	}


	void Populate() {
		titleLabel.text = quest.itemName.ToUpper();
		descriptionLabel.text = quest.description;
		durationLabel.text = GetTimeString();
	//	numBlobsNeededLabel.text = "NEED " + quest.blobsRequired.ToString() + " BLOBS";
		if(quest.monsters != null & quest.monsters.Count > 0) 
			monsterLabel.text = monsterManager.GetBaseMonsterByID(quest.monsters[0].id).itemName.ToUpper();

		icon.spriteName = quest.iconName;
		icon.atlas = quest.iconAtlas;
		PopulateBlobSlots();
		potentialLootMenu.RebuildSlots(quest);
	}


	void PopulateBlobSlots() {
		foreach(Transform child in blobGrid.transform) {
			bool active = (child.GetSiblingIndex() < quest.blobsRequired);
			child.gameObject.SetActive(active);
			if(!active)
				continue;

			BlobQuestSlot blobSlot = child.GetComponent<BlobQuestSlot>();
			blobSlot.fulfilledSprite.gameObject.SetActive(false);
			blobSlot.sigilSprite.alpha = .5f;
			blobSlot.socketSprite.alpha = .5f;
			Element element = quest.elementRequirements[child.GetSiblingIndex()];
			Color colorRequired = ColorDefines.ColorForElement(element);
			blobSlot.colorBgSprite.color = (element == Element.None) ? ColorDefines.defaultBlobSocketColor: colorRequired;
			Sigil sigil = quest.sigilRequirements[child.GetSiblingIndex()];
			blobSlot.sigilSprite.gameObject.SetActive(sigil != Sigil.None);
			if(sigil != Sigil.None)
				blobSlot.sigilSprite.spriteName = GlobalDefines.SpriteNameForSigil(sigil);
		}
		
		ClearBlobs();
		blobGrid.Reposition();
	}
	

	public void ClearBlobs() {
		foreach(Transform child in blobGrid.transform) {
			BlobDragDropItem blobDragDropItem = child.GetComponentInChildren<BlobDragDropItem>();
			if(blobDragDropItem != null)
				GameObject.Destroy(blobDragDropItem.gameObject);
		}
	}


	string GetTimeString() {
		string timeString = "";
		if(quest.days > 0)
			timeString += quest.days.ToString() + " day";
		if(quest.hrs > 0)
			timeString += quest.hrs.ToString() + " hr";
		if(quest.mins > 0)
			timeString += quest.mins.ToString() + " min";
		return ColorDefines.ColorToHexString(ColorDefines.goldenTextColor) + timeString + "[-]";
	}


	void UpdateBonuses() {
		List<QuestBonus> bonusList = quest.GetAppropriateBonusList();
		if(bonusList == null) return;
		int bonusTotalCt = bonusList.Count;
		bonusLabel.alignment = bonusTotalCt > 1 ? NGUIText.Alignment.Left : NGUIText.Alignment.Center;
		if(bonusTotalCt == 0) {
			bonusLabel.text = "No Bonus";
			return;
		}
		bonusLabel.text = "";
		for(int i=0; i < bonusTotalCt; i++) {
			int reqMatches = quest.GetNumMatchesRequiredForBonus(bonusList[i]);
			bonusLabel.text += "[FFFFFF]" + reqMatches.ToString() + " Match: " + bonusList[i].description + "[-]" + ((i < (bonusTotalCt - 1)) ? "\n" : "");
		}
	}
}
