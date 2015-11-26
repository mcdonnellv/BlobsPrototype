using UnityEngine;
using System.Collections;

public class GenericGameMenu : MonoBehaviour {

	static Vector3[] popupPositions = null;
	public PopupPosition defaultStartPosition;
	public UILabel headerLabel;
	public GameObject window;
	public GameObject BG;
	private bool displayed = false;
	private Vector3 oldPosition = Vector3.one;
	protected UITweener animationBG;
	protected UITweener animationWindow;
	

	public bool IsDisplayed() { return displayed; }


	public GenericGameMenu() {
		if (popupPositions == null) {
			popupPositions = new Vector3[(int)PopupPosition.Max];
			popupPositions[(int)PopupPosition.Center] = Vector3.zero;
			popupPositions[(int)PopupPosition.Right1] = new Vector3(435f, 0f, 0f);
			popupPositions[(int)PopupPosition.Right2] = new Vector3(300f, 0f, 0f);
		}
	}


	public void Show() {
		gameObject.SetActive(true);
		HudManager.IncrementPopupRefCount();
		if(oldPosition == Vector3.one)
			oldPosition = transform.localPosition;
		if(defaultStartPosition != PopupPosition.DontSet)
			transform.localPosition = popupPositions[(int)defaultStartPosition];

		window.SetActive(true);
		window.transform.localScale = Vector3.zero;
		animationWindow = window.GetComponent<UITweener>();
		animationWindow.onFinished.Clear();
		animationWindow.PlayForward();
		animationWindow.onFinished.Add(new EventDelegate(this, "SetDisplayed"));

		if(BG != null) {
			animationBG = BG.GetComponent<UITweener>();
			animationBG.onFinished.Clear();
			animationBG.PlayForward();
		}
	}


	public void SetDisplayed() {
		displayed = true;
		animationWindow.onFinished.Clear();
		BroadcastMessage("GameMenuDisplayed", this);
	}


	public void Show(string header) {
		Show();
		headerLabel.text = header;
	}
	

	public void Hide() {
		if(!displayed)
			return;
		animationWindow.onFinished.Clear();
		animationWindow.onFinished.Add(new EventDelegate(this, "DisableWindow"));
		animationWindow.PlayReverse();

		if(animationBG != null) {
			animationWindow.onFinished.Add(new EventDelegate(animationBG, "PlayReverse"));
			animationBG.onFinished.Add(new EventDelegate(this, "Cleanup"));
		}
	}


	public void HideInstant() {
		animationWindow.ResetToBeginning();
		DisableWindow();
	}

	
	public void DisableWindow() {
		animationWindow.onFinished.Clear();
		window.SetActive(false);
		HudManager.DecrementPopupRefCount();
		if(defaultStartPosition != PopupPosition.DontSet && oldPosition != Vector3.one)
			transform.localPosition = oldPosition;

		if(animationBG == null)
			Cleanup();
	}

	public virtual void Cleanup() {
		animationWindow.enabled = false;
		if(animationBG != null)
			animationBG.enabled = false;
		displayed = false;
		gameObject.SetActive(false);
	}
}
