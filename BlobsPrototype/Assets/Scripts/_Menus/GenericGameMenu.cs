using UnityEngine;
using System.Collections;

public class GenericGameMenu : MonoBehaviour {

	public UILabel headerLabel;
	public UITweener animationBG;
	public UITweener animationWindow;
	public GameObject window;
	public GameObject BG;


	public void Show() {
		gameObject.SetActive(true);
		window.SetActive(true);
		window.transform.localScale = new Vector3(0,0,0);
		animationWindow.onFinished.Clear();
		animationWindow.PlayForward();

		if(animationBG != null) {
			animationBG.onFinished.Clear();
			animationBG.PlayForward();
		}
	}


	public void Show(string header) {
		Show();
		headerLabel.text = header;
	}


	public void Hide() {
		animationWindow.onFinished.Add(new EventDelegate(this, "DisableWindow"));
		animationWindow.PlayReverse();

		if(animationBG != null) {
			animationWindow.onFinished.Add(new EventDelegate(animationBG, "PlayReverse"));
			animationBG.onFinished.Add(new EventDelegate(this, "Cleanup"));
		}
		else {
			animationWindow.onFinished.Add(new EventDelegate(this, "Cleanup"));
		}
	}

	
	public void DisableWindow() {
		animationWindow.onFinished.Clear();
		window.SetActive(false);
	}


	public void Cleanup() {
		animationWindow.enabled = false;
		if(animationBG != null)
			animationBG.enabled = false;
		gameObject.SetActive(false);
	}
}
