using UnityEngine;
using System.Collections;

public enum Quality {
	None = -1,
	Standard,
	Common,
	Rare,
	Epic,
	Legendary,
};

public enum Gender {
	Male,
	Female,
}

public enum BlobState {
	Idle,
	Breeding,
	Hatching,
	HatchReady,
	Working,
	WorkingReady,
};


public class GlobalDefines : MonoBehaviour {
}
