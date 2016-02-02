using UnityEngine;
using System;
using System.Collections;

public class PrioritySequence : Sequence {
	private int _lastSequence;
	
	public PrioritySequence() {
		Update = () => {
			_sequence = 0;
			for(;;) {
				Status s = GetChild(_sequence).Tick();
				if(s != Status.BhSuccess) {
					// if dont get success, reset all previous children so we can restart them again
					for(int i = _sequence + 1; i <= _lastSequence; i++) {
						GetChild(i).Reset();
					}
					_lastSequence = _sequence;
					return s;
				}
				
				if(++_sequence == ChildCount) 
					return Status.BhSuccess;
			}
		};
	}
}