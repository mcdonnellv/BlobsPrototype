using UnityEngine;
using System;
using System.Collections;

 public interface IBehavior {

	Func<Status> Update { set; }
	Action Initialize { set; }
	Action<Status> Terminate { set; }
	Status Status { get; set; }

	
	Status Tick();
	void Reset();
	Composite End();
	IBehavior SetUpdate(Func<Status> update);
	IBehavior SetInitialize(Action initialize);
	IBehavior SetTerminate(Action<Status> terminate);
}

