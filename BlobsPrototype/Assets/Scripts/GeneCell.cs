using UnityEngine;
using System.Collections;

public class GeneCell : MonoBehaviour {

	public UISprite bgSprite;
	public UILabel nameLabel;
	public UISprite socketSprite;

	public void Pressed() {
		BlobInfoContextMenu blobInfoContextMenu = gameObject.GetComponentInParent<BlobInfoContextMenu>();
		blobInfoContextMenu.GeneCellPressed(this);
	}


	public GenePointer GetGenePointer() { return gameObject.GetComponentInChildren<GenePointer>(); }


	public Gene GetGene() {
		GenePointer gp = GetGenePointer();
		if(gp != null)
			return gp.gene;
		return null;
	}

	public void Activate() {
		bgSprite.color = ColorDefines.activeCellColor;
	}

	public void Deactivate() {
		bgSprite.color = ColorDefines.inactiveCellColor;
	}

}
