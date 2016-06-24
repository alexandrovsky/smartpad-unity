using UnityEngine;
using UnityEngine.UI;
using Smartpad;

public class Info : MonoBehaviour, EventListener
{
	public EventEmitter Smartpad;
	public Text Text;

	void Start ()
	{
		Smartpad.Add (this);
	}

	public void On (Quaternion q)
	{
		Vector3 e = q.eulerAngles;
		Text.text = "x: " + q.x + "\ny: " + q.y + "\nz: " + q.z + "\nw: " + q.w + "\n\nx: " + e.x + "\ny: " + e.y + "\nz: " + e.z;
	}
}
