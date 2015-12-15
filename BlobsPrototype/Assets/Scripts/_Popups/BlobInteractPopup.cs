using UnityEngine;
using System.Collections;

public class BlobInteractPopup : GenericGameMenu {
	
	public UIButton leftButton;
	public UIButton rightButton;
	Blob blob1;
	Blob blob2;
	BreedManager breedManager { get { return BreedManager.breedManager; } }
	
	public void Show(Blob a, Blob b) {
		blob1 = a;
		blob2 = b;
		base.Show();
		leftButton.isEnabled = (blob1.canBreed && blob2.canBreed);
		rightButton.isEnabled = (blob1.canMerge && blob2.canMerge);
	}
	
	public void LeftButtonPressed() {
		breedManager.AskBlobsInteract(blob1, blob2, BlobInteractAction.Breed);
		Hide();
	}

	public void RightButtonPressed() {
		breedManager.AskBlobsInteract(blob1, blob2, BlobInteractAction.Merge);
		Hide();
	}
}
