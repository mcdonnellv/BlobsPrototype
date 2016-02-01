using UnityEngine;
using System;
using System.Collections;

public class Sequence : Composite {

	private int _sequence;

	public Sequence() {
		Update = () => {
			for(;;) {
				Status s = GetChild(_sequence).Tick();
				if(s != Status.BhSuccess) {
					if(s == Status.BhFailure) {
						_sequence = 0;
					}
					return s;
				}

				if(++_sequence == ChildCount) {
					_sequence = 0;
					return Status.BhSuccess;
				}
			}
		};

		Initialize = () => { _sequence = 0; };
	}
}

