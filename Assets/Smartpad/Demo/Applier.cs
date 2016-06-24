using UnityEngine;
using Smartpad;

public class Applier : MonoBehaviour, EventListener
{
	public CalibratedSmartpad CalibratedSmartpad;

	void Start ()
	{
		CalibratedSmartpad.Add (this);
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Q)) {
			if (Input.GetKey (KeyCode.LeftShift)) {
				CalibratedSmartpad.Reset ();
			} else if (Input.GetKey (KeyCode.LeftControl)) {
				CalibratedSmartpad.Calibrate ();
			}
		}
	}

	public void On (Quaternion q)
	{
		transform.rotation = q;
	}
}
