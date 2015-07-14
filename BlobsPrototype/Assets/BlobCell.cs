using UnityEngine;
using System.Collections;

public class BlobCell : MonoBehaviour 
{

	public GameManager gm;
	public UISlider progressBar;
	public UIButton button;
	public float fillSpeed = 1f;

	// Use this for initialization
	public void Pressed () 
	{
		int index = transform.GetSiblingIndex();
		gm.PressGridItem(index);
	}

	void Update () 
	{
		if(progressBar.value > 0f)
		{
			progressBar.value -= Time.deltaTime * (1f / fillSpeed);
			
			if(progressBar.value <= 0f)
			{
				progressBar.value = 0f;
				gm.BlobCellProgressDone(this);
			}
		}
	}

}
