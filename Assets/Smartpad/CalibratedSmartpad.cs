using UnityEngine;

namespace Smartpad
{
	public class CalibratedSmartpad : EventEmitter, EventListener
	{
		public Transform Offset;
		public EventEmitter Smartpad;

		private Quaternion Buffer;
		private Quaternion Zero;

		public CalibratedSmartpad ()
		{
			Reset ();
		}

		void Start ()
		{
			Smartpad.Add (this);
		}

		public void Reset ()
		{
			Zero = Quaternion.identity;
			Buffer = Quaternion.identity;
		}

		public void Calibrate ()
		{
			if (Offset == null) {
				Zero = Quaternion.Inverse (Smartpad.MostRecent ());
			} else {
				Zero = Quaternion.Inverse (Smartpad.MostRecent () * Quaternion.Inverse (Offset.rotation));
			}
		}

		public override Quaternion MostRecent ()
		{
			return Buffer;
		}

		public void On (Quaternion q)
		{
			Buffer = Zero * q;
			Emit (Buffer);
		}
	}
}
