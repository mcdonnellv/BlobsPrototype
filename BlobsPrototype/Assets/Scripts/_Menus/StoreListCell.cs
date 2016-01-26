using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoreListCell : MonoBehaviour {
	[HideInInspector] public int baseGeneId = -1;
	public UILabel titleLabel;
	public UILabel costLabel;
	public UISprite icon;
	public GameObject handle;
	public UILabel newLabel;
	public UISprite bg;
	public UISprite bgBorder;


	public void Pressed() {
		HudManager.hudManager.storeMenu.SelectCellByIndex(transform.GetSiblingIndex());
		Select();
		foreach(Transform child in transform.parent) {
			if(child == transform)
				continue;
			child.SendMessage("Unselect");
		}
	}

	public void Unselect() {
		bg.color = ColorDefines.HexStringToColor("2E2822");
		bgBorder.color = ColorDefines.HexStringToColor("6F6E6A");
		handle.transform.localPosition = new Vector3(0, handle.transform.localPosition.y, handle.transform.localPosition.z);
	}
	
	public void Select() {
		bg.color = ColorDefines.HexStringToColor("4F3D2B");
		bgBorder.color = ColorDefines.HexStringToColor("E4C372");
		handle.transform.localPosition = new Vector3(20, handle.transform.localPosition.y, handle.transform.localPosition.z);
	}

}
