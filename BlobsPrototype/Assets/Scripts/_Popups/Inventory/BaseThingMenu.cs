﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseThingMenu : MonoBehaviour {
	
	public GameObject grid;
	public ItemInfoPopup itemInfoPopup;
	protected int selectedIndex = -1;
	protected GameObject slotHighlight;
	HudManager hudManager { get { return HudManager.hudManager; } }


	public virtual GameObject CreateGameObject(BaseThing g) {return null;}
	public virtual void SetSelectedThing(int index) {}
	public virtual BaseThing GetSelectedThing() {return null;}
	public virtual void ShowInfo() {}

	public void SelectedThingSetup(int itemCount) {
		UpdateItemCounts();

		if(selectedIndex == -1 && itemCount > 0)
			selectedIndex = 0;
		
		if(itemCount == 0) {
			selectedIndex = -1;
			itemInfoPopup.ClearFields();
			itemInfoPopup.Hide();
		}
		selectedIndex = (selectedIndex >= itemCount) ? itemCount - 1 : selectedIndex;
		if(selectedIndex >= 0 && selectedIndex < itemCount) //check within bounds
			SetSelectedThing(selectedIndex);
	}

	public void GameMenuDisplayed(GenericGameMenu gameMenu) {
		if (gameMenu is InventoryMenu)
			ShowInfo();
	}


	public void SetupThingInSocket(BaseThing thing, int index){
		Transform parentSocket = grid.transform.GetChild(index);
		GameObject go = CreateGameObject(thing);
		go.transform.parent = parentSocket;
		go.transform.localScale = Vector3.one;
		go.transform.localPosition = Vector3.zero;
		go.GetComponent<UISprite>().depth = parentSocket.GetComponent<UISprite>().depth + 2;
	}

	public virtual void UpdateItemCounts() {
		foreach(Transform inventorySocket in grid.transform)
			inventorySocket.gameObject.GetComponentInChildren<UILabel>().text = "";
	}


	public void CreateSlotHighlight(Transform socket) {
		if(slotHighlight != null)
			GameObject.Destroy(slotHighlight);
		slotHighlight = (GameObject)GameObject.Instantiate(Resources.Load("Gene Slot Highlight"));
		slotHighlight.transform.parent = socket;
		slotHighlight.transform.localScale = Vector3.one;
		slotHighlight.transform.localPosition = Vector3.zero;
		slotHighlight.GetComponent<UISprite>().depth = socket.GetComponent<UISprite>().depth;
		UISprite slotSpriteBg = socket.GetComponent<UISprite>();
		UISprite highlightSprite = slotHighlight.GetComponent<UISprite>();
		highlightSprite.depth = slotSpriteBg.depth;
		selectedIndex = socket.GetSiblingIndex();
	}

	public virtual void DeleteSelectedThing() {}

	public void CleanUpAfterDelete(int itemsLeft, GameObject pointerObject) {
		if(itemsLeft <= 0) {
			GameObject.Destroy(pointerObject);
			GameObject.Destroy(slotHighlight);
			selectedIndex = -1;
			itemInfoPopup.Hide();
		}
	}

	void Update() {
		if(slotHighlight != null)
			slotHighlight.transform.localPosition = Vector3.zero;
	}
}