using UnityEngine;
using System.Collections;

public class GenePointer : MonoBehaviour {
	public Gene gene;
	public MonoBehaviour owningMenu = null;

	public void GenePressed() {
		if(owningMenu != null)
			owningMenu.SendMessage("GenePressed", this);
	}
}
