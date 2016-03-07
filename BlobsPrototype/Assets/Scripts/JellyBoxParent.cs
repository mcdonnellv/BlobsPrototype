using UnityEngine;
using System.Collections;

public class JellyBoxParent : MonoBehaviour
{
	// Drag your Jelly Sprite into this field in the inspector
	public JellySprite jellySprite;

	void Update ()
	{
		// Attach the Jelly Sprite central body to this gameobject. (I'm doing
		// this in Update() to ensure that the Jelly Sprite is given time to
		// initialize - it'd be neater to do it in Awake(), but you'd also need
		// to fiddle with the script execution order settings to ensure that this script
		// always runs after the Jelly Sprite has been configured)
		if(jellySprite.CentralPoint.transform.parent != this.transform)
		{
			jellySprite.CentralPoint.transform.parent = this.transform;
			jellySprite.SetPosition(this.transform.position, true);
		}
	}
}