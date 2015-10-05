using UnityEngine;
using System.Collections;

public class ItemInfoPopup : MonoBehaviour {

	public UITweener animationWindow;

	public void Show() {
		gameObject.SetActive(true);
		transform.localScale = new Vector3(0,0,0);
		animationWindow.onFinished.Clear();
		animationWindow.PlayForward();
	}
	
	public void Hide() {
		animationWindow.onFinished.Add(new EventDelegate(this, "DisableWindow"));
		animationWindow.PlayReverse();
	}
	
	void DisableWindow() {
		animationWindow.onFinished.Clear();
		gameObject.SetActive(false);
	}
}
