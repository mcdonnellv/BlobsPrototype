using UnityEngine;
using System.Collections;

public class GenericGameMenu : MonoBehaviour {

	public UILabel headerLabel;
	public UITweener animationBG;
	public UITweener animationWindow;
	public GameObject window;
	public GameObject BG;
	public bool displayed = false;


	public void Show() {
		gameObject.SetActive(true);
		window.SetActive(true);
		window.transform.localScale = new Vector3(0,0,0);
		animationWindow.onFinished.Clear();
		animationWindow.PlayForward();
		animationWindow.onFinished.Add(new EventDelegate(this, "SetDisplayed"));

		if(animationBG != null) {
			animationBG.onFinished.Clear();
			animationBG.PlayForward();
		}
	}


	public void SetDisplayed() {
		displayed = true;
		animationWindow.onFinished.Clear();
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
