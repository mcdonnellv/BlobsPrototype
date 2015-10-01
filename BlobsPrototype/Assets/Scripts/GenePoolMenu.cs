using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenePoolMenu : MonoBehaviour {

	public UITweener animationWindow;
	public GameObject geneGrid;

	public void Show(){
		GeneManager geneManager = GameObject.Find ("GeneManager").GetComponent<GeneManager>();
		gameObject.SetActive(true);
		transform.localScale = new Vector3(0,0,0);
		animationWindow.onFinished.Clear();
		animationWindow.PlayForward();

		foreach(Transform c in geneGrid.transform) {
			c.DestroyChildren();
		}

		foreach(Gene g in geneManager.storedGenes) {
			if(g == null)
				continue;
			Transform parentSocket = geneGrid.transform.GetChild(geneManager.storedGenes.IndexOf(g));
			parentSocket.DestroyChildren();
			GameObject go = g.CreateGeneGameObject();
			go.transform.parent = parentSocket;
			go.transform.localScale = new Vector3(1f,1f,1f);
			go.transform.localPosition = new Vector3(0f,0f,0f);
		}

	}

	public void Hide() {
		animationWindow.onFinished.Add(new EventDelegate(this, "DisableWindow"));
		animationWindow.PlayReverse();
	}

	void DisableWindow() {
		gameObject.SetActive(false);
	}
}
