using UnityEngine;
using System.Collections;

public class GeneCell : MonoBehaviour 
{
	public UILabel nameLabel;
	public UISprite rarityIdicator;
	public UIButton button;
	public InfoPanel infoPanel;
	public Gene gene;

	public void InfoButtonPressed()
	{
		int index = transform.GetSiblingIndex();
		InfoPanel ip = (InfoPanel)gameObject.GetComponentInParent<InfoPanel>();
		ip.GeneInfoPressed(index);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
