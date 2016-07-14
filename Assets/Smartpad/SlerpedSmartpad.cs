using UnityEngine;

namespace Smartpad
{
	public class SlerpedSmartpad : EventEmitter, EventListener
	{
		public EventEmitter Smartpad;
		public float T = 0.9f;

		private Quaternion Buffer;

		void Start ()
		{
			Smartpad.Add (this);
		}

		public override Quaternion MostRecent ()
		{
			return Buffer;
		}

		public void On (Quaternion q)
		{
			Buffer = Quaternion.Slerp (Buffer, q, Time.deltaTime / Time.fixedDeltaTime * T);
			Emit (Buffer);
		}
	}
}
