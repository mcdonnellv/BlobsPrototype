using UnityEngine;
using System;
using System.Collections;

public interface IBehavior {
	Status Tick();
	Status Status { get; set; }
	IBehavior Parent { get; set; }
	Action Initialize { set; }
	Action<Status> Terminate { set; }
	Func<Status> Update { set; }

}

