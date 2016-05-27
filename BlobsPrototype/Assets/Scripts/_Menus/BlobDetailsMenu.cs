using UnityEngine;
using System.Collections;

public class BlobDetailsMenu : GenericGameMenu {

	[HideInInspector] public Blob blob;
	[HideInInspector] public GenericGameMenu owner = null;
	public BlobDetailsCard detailsCard;

	public void Pressed() {	Show(null); }

	public void Show(GenericGameMenu caller) {
		gameObject.SetActive(true);
		base.Show();
		owner = caller;
		detailsCard.Setup(blob);
	}

	public override void Hide() {
		base.Hide();
		owner.Show();
	}
}
