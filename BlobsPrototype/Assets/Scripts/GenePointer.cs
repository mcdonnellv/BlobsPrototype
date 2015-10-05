using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenePointer : MonoBehaviour {
	public Gene gene;

	public void GenePressed() {
		HudManager hudManager = GameObject.Find("HudManager").GetComponent<HudManager>();
		hudManager.inventoryMenu.genesMenu.ShowInfoForGeneGameObject(this);
	}
}
