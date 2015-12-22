using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PopupData {
	public string header; 
	public string body;
	public MonoBehaviour target; 
	public string methodName;
}


public class Popup : GenericGameMenu {

	public UILabel bodyLabel;
	public UIWidget singleChoiceContainer;
	public UIWidget doubleChoiceContainer;
	UIButton okButton;

	static List<PopupData> popupQueue = new List<PopupData>();


	public void Show(string header, string body) {
		PopupData pData = new PopupData();
		pData.header = header;
		pData.body = body;
		Popup.popupQueue.Add(pData);
		Execute();
	}
	
	public void Show(string header, string body, MonoBehaviour target, string methodName) {
		PopupData pData = new PopupData();
		pData.header = header;
		pData.body = body;
		pData.target = target;
		pData.methodName = methodName;
		Popup.popupQueue.Add(pData);
		Execute();
	}

	public void Show(PopupData pData) {
		base.Show();
		if(pData.target == null) {
			headerLabel.text = pData.header.ToUpper();
			bodyLabel.text = pData.body;
			EnableSingleContainer();
		}
		else {
			headerLabel.text = pData.header.ToUpper();
			bodyLabel.text = pData.body;
			EnableDoubleContainer();
			EventDelegate assignedOkEventDelegate = new EventDelegate(pData.target, pData.methodName);
			okButton.onClick.Clear();
			okButton.onClick.Add(assignedOkEventDelegate);
			okButton.onClick.Add(new EventDelegate(this, "Hide"));
		}
	}

	public override void Cleanup() {
		base.Cleanup();
		Popup.popupQueue.Remove(Popup.popupQueue[0]);
		Execute();

	}


	public void Execute() {
		if(Popup.popupQueue.Count == 1) 
			Show(Popup.popupQueue[0]);
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
		UILabel okLabel = labels[0];
		UILabel cancelLabel = labels[1];
		okLabel.text = "Confirm";
		cancelLabel.text = "Cancel";
	}
}

