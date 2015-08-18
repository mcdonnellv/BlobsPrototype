using UnityEngine;
using System.Collections;

public class GeneCell : MonoBehaviour 
{
	public UILabel nameLabel;
	public UILabel infoLabel;
	public UISprite rarityIndicator;
	public UISprite cellSprite;
	public UIButton button;
	public UIButton addGeneButton;
	public InfoPanel infoPanel;
	public Gene gene;


	public void AddGenePressed() 
	{
		int index = transform.GetSiblingIndex();
		InfoPanel ip = (InfoPanel)gameObject.GetComponentInParent<InfoPanel>();
		ip.AddGenePressed(index);
	}

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
