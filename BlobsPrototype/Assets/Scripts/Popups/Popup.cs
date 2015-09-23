using UnityEngine;
using System.Collections;

public class Popup : MonoBehaviour 
{
	public UILabel headerLabel;
	public UILabel bodyLabel;

	public UIWidget singleChoiceContainer;
	public UIWidget doubleChoiceContainer;

	public UITweener animationBG;
	public UITweener animationWindow;

	public GameObject window;
	public GameObject BG;

	UIButton okButton;
	UIButton cancelButton;

	EventDelegate assignedOkEventDelegate;

	void Start() {
		//gameObject.SetActive(false);
	}

	public void Show(string header, string body)
	{
		gameObject.SetActive(true);
		headerLabel.text = header;
		bodyLabel.text = body;
		SetupStartAnimation();
		EnableSingleContainer();
	}


	public void Show(string header, string body, MonoBehaviour target, string methodName) {
		gameObject.SetActive(true);
		headerLabel.text = header;
		bodyLabel.text = body;
		SetupStartAnimation();
		EnableDoubleContainer();
		if (assignedOkEventDelegate != null)
			okButton.onClick.Remove(assignedOkEventDelegate);
		assignedOkEventDelegate = new EventDelegate(target, methodName);
		okButton.onClick.Add(assignedOkEventDelegate);
	}

	void SetupStartAnimation() {
		window.SetActive(true);
		window.transform.localScale = new Vector3(0,0,0);
		animationBG.onFinished.Clear();
		animationBG.PlayForward();
		animationWindow.onFinished.Clear();
		animationWindow.PlayForward();
	}

	void EnableSingleContainer() {
		singleChoiceContainer.gameObject.SetActive(true);
		doubleChoiceContainer.gameObject.SetActive(false);
		okButton = singleChoiceContainer.GetComponentInChildren<UIButton>();
		UILabel label = okButton.GetComponentInChildren<UILabel>();
		label.text = "Ok";
	}


	void EnableDoubleContainer() {
		singleChoiceContainer.gameObject.SetActive(false);
		doubleChoiceContainer.gameObject.SetActive(true);
		UIButton[] buttons = doubleChoiceContainer.GetComponentsInChildren<UIButton>(true);
		UILabel[] labels = doubleChoiceContainer.GetComponentsInChildren<UILabel>(true);
		okButton = buttons[0];
		cancelButton = buttons[1];
		UILabel okLabel = labels[0];
		UILabel cancelLabel = labels[1];
		okLabel.text = "Confirm";
		cancelLabel.text = "Cancel";
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
		okButton.onClick.Remove(assignedOkEventDelegate);
		assignedOkEventDelegate = null;
		animationWindow.enabled = false;
		animationBG.enabled = false;
		gameObject.SetActive(false);
	}


}

