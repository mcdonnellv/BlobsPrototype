using UnityEngine;
using System;
using System.Collections;

public class Behavior : IBehavior {
	public Status Status { get; set; }
	public Action Initialize { protected get; set; }
	public IBehavior Parent { protected get; set; }
	public Func<Status> Update { protected get; set; }
	public Action<Status> Terminate { protected get; set; }

	public Status Tick() {
		if(Status == Status.BhInvalid && Initialize != null) {
			Initialize();
		}

		Status = Update();

		if(Status != Status.BhRunning && Initialize != null)
			Terminate(Status);

		return Status;
	}
}
