using UnityEngine;
using System.Collections;

public class BlobInteractPopup : GenericGameMenu {
	
	public UIButton leftButton;
	public UIButton rightButton;
	Blob blob1;
	Blob blob2;
	BreedManager breedmanager;
	
	public void Show(Blob a, Blob b) {
		blob1 = a;
		blob2 = b;
		base.Show();
		breedmanager = GameObject.Find ("BreedManager").GetComponent<BreedManager> ();
		leftButton.isEnabled = (blob1.canBreed && blob2.canBreed);
		rightButton.isEnabled = (blob1.canMerge && blob2.canMerge);
	}
	
	public void LeftButtonPressed() {
		breedmanager.AskBlobsInteract(blob1, blob2, BlobInteractAction.Breed);
		Hide();
	}

	public void RightButtonPressed() {
		breedmanager.AskBlobsInteract(blob1, blob2, BlobInteractAction.Merge);
		Hide();
	}
}
