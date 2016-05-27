using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlobDetailsSubMenus : MonoBehaviour {
	public BlobDetailsSubMenu statsSubMenu;
	public BlobDetailsSubMenu traitsSubMenu;
	public BlobDetailsSubMenu genesSubMenu;
	public UIScrollView scrollView;

	public void Start() {
		StatsTabPressed();
	}

	public void ActivateSubMenu(BlobDetailsSubMenu subMenu) {
		subMenu.gameObject.SetActive(true);
		scrollView.ResetPosition();
	}

	public void DectivateSubMenu(BlobDetailsSubMenu subMenu) {
		subMenu.gameObject.SetActive(false);
	}

	public void StatsTabPressed() {
		ActivateSubMenu(statsSubMenu);
		DectivateSubMenu(traitsSubMenu);
		DectivateSubMenu(genesSubMenu);
	}

	public void TraitsTabPressed() {
		DectivateSubMenu(statsSubMenu);
		ActivateSubMenu(traitsSubMenu);
		DectivateSubMenu(genesSubMenu);
	}

	public void GenesTabPressed() {
		DectivateSubMenu(statsSubMenu);
		DectivateSubMenu(traitsSubMenu);
		ActivateSubMenu(genesSubMenu);
	}
}
