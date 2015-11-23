using UnityEngine;
using System.Collections;

public class Popup : GenericGameMenu {

	public UILabel bodyLabel;
	public UIWidget singleChoiceContainer;
	public UIWidget doubleChoiceContainer;
	UIButton okButton;
	UIButton cancelButton;
	EventDelegate assignedOkEventDelegate;


	public void Show(string header, string body) {
		base.Show(header);
		bodyLabel.text = body;
		EnableSingleContainer();
	}


	public void Show(string header, string body, MonoBehaviour target, string methodName) {
		base.Show(header);
		bodyLabel.text = body;
		EnableDoubleContainer();
		if (assignedOkEventDelegate != null)
			okButton.onClick.Remove(assignedOkEventDelegate);
		assignedOkEventDelegate = new EventDelegate(target, methodName);
		okButton.onClick.Add(assignedOkEventDelegate);
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


	public void Cleanup() {
		okButton.onClick.Remove(assignedOkEventDelegate);
		assignedOkEventDelegate = null;
		base.Cleanup();
	}


}

