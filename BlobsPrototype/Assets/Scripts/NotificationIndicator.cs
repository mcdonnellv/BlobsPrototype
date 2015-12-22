using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class NotificationIndicator : MonoBehaviour {
	public UILabel errorLabel;
	public UILabel noticeLabel;
	public UILabel perisitentlabel;

	List<string> noticeQueue = new List<string>();
	List<string> errorQueue = new List<string>();


	void Start () {
		errorLabel.gameObject.SetActive(false);
		noticeLabel.gameObject.SetActive(false);
		perisitentlabel.gameObject.SetActive(false);
	}

	public void AddNoticeToQueue(string text) { noticeQueue.Add(text); }
	public void AddErrorToQueue(string text) { errorQueue.Add(text); }

	void DisplayNotice(string text) {
		noticeLabel.gameObject.SetActive(true);
		noticeLabel.text = text.ToUpper();
		UITweener animation = noticeLabel.GetComponent<UITweener>();
		animation.PlayForward();
		animation.Invoke("PlayReverse", animation.duration + 1f);
		Invoke("HideNotice", animation.duration * 3f + 1f);
	}

	void HideNotice() { noticeLabel.gameObject.SetActive(false); }
	bool IsNoticeDisplayed() { return noticeLabel.gameObject.activeSelf; }


	void DisplayError(string text) {
		if(IsInvoking("HideError"))
			CancelInvoke("HideError");

		if(IsInvoking("PlayErrorEndAnim"))
			CancelInvoke("PlayErrorEndAnim");

		errorLabel.gameObject.SetActive(true);
		errorLabel.text = text.ToUpper();
		UITweener animation = errorLabel.GetComponent<UITweener>();
		animation.PlayForward();
		animation.ResetToBeginning();
		animation.PlayForward();
		Invoke("PlayErrorEndAnim", animation.duration + 1f);
	}


	void HideError() { errorLabel.gameObject.SetActive(false); }


	void PlayErrorEndAnim() { 
		if (IsErrorDisplayed() == false)
			return;
		UITweener animation = errorLabel.GetComponent<UITweener>();
		Invoke("HideError", animation.duration);
		animation.PlayReverse();
	}


	bool IsErrorDisplayed() { return errorLabel.gameObject.activeSelf; }


	public void DisplayPersistentNotice(string text) {
		perisitentlabel.gameObject.SetActive(true);
		perisitentlabel.text = text.ToUpper();
		UITweener animation = perisitentlabel.GetComponent<UITweener>();
		animation.PlayForward();
	}

	public void HidePersistentNotice() {
		UITweener animation = perisitentlabel.GetComponent<UITweener>();
		animation.PlayReverse();
		Invoke("DeactivatePersistentNotice", animation.duration);
	}

	void DeactivatePersistentNotice() { perisitentlabel.gameObject.SetActive(false); }

	void Update() {
		if(noticeQueue.Count > 0 && !IsNoticeDisplayed()) {
			DisplayNotice(noticeQueue[0]);
			noticeQueue.RemoveAt(0);
		}

		if(errorQueue.Count > 0) {
			DisplayError(errorQueue[0]);
			errorQueue.RemoveAt(0);
		}
	}
}
