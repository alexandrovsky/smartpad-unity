using System.Collections.Generic;
using UnityEngine;

namespace Smartpad
{
	public abstract class EventEmitter : MonoBehaviour
	{
		private readonly List<EventListener> Listeners;

		public EventEmitter ()
		{
			Listeners = new List<EventListener> ();
		}

		public void Add (EventListener listener)
		{
			Listeners.Add (listener);
		}

		public void Remove (EventListener listener)
		{
			Listeners.Remove (listener);
		}

		protected void Emit (Quaternion q)
		{
			foreach (EventListener listener in Listeners) {
				listener.On (q);
			}
		}

		public abstract Quaternion MostRecent ();
	}
}
