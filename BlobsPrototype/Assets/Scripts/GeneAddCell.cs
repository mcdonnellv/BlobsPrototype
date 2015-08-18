using UnityEngine;
using System.Collections;

public class GeneAddCell : MonoBehaviour 
{
	public UILabel nameLabel;
	public UISprite rarityIndicator;
	public UIButton button;
	public GeneAddPopup geneAddPopup;
	public Gene gene;


	public void Pressed()
	{
		int index = transform.GetSiblingIndex();
		geneAddPopup.Pressed(index);
	}

	public void PressedCell()
	{
		int index = transform.GetSiblingIndex();
		geneAddPopup.PressedCell(index);
	}
}
