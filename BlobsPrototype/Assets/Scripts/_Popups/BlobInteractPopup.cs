using UnityEngine;
using System.Collections;

public class BlobInteractPopup : MonoBehaviour {

	public UITweener animationBG;
	public UITweener animationWindow;
	public GameObject window;
	public GameObject BG;
	Blob blob1;
	Blob blob2;

	BreedManager breedmanager;
	HudManager hudManager;
	RoomManager roomManager;
	
	public void Show(Blob a, Blob b) {
		blob1 = a;
		blob2 = b;
		gameObject.SetActive(true);
		window.SetActive(true);
		window.transform.localScale = new Vector3(0,0,0);
		animationBG.onFinished.Clear();
		animationBG.PlayForward();
		animationWindow.onFinished.Clear();
		animationWindow.PlayForward();
	}
	
	// Use this for initialization
	void Start () {
		breedmanager = GameObject.Find ("BreedManager").GetComponent<BreedManager> ();
	}

	public void Hide() {
		animationWindow.onFinished.Add(new EventDelegate(animationBG, "PlayReverse"));
		animationWindow.onFinished.Add(new EventDelegate(this, "DisableWindow"));
		animationWindow.PlayReverse();
		animationBG.onFinished.Add(new EventDelegate(this, "DisablePopup"));
	}
	
	void DisableWindow() {
		window.SetActive(false);
	}

	void DisablePopup() {
		animationWindow.enabled = false;
		animationBG.enabled = false;
		gameObject.SetActive(false);
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
