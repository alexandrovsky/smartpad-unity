using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Smartpad
{
	public class Smartpad : EventEmitter
	{
		public int Port = 55555;

		private readonly IPEndPoint LocalEP;
		private readonly Socket LocalSocket;
		private EndPoint RemoteEP;
		private readonly AsyncCallback Callback;
		private bool NewDataReceived;
		private bool NewDataToProcess;

		private int i;
		// socket thread buffer
		private byte[] BufferASocket;
		// between threads buffer
		private byte[] BufferBBetween;
		// unity thread buffer
		private byte[] BufferCUnity;
		// single float buffer
		private byte[] BufferDSingle;
		// emitted buffer
		private Quaternion Buffer;

		public Smartpad ()
		{
			LocalEP = new IPEndPoint (IPAddress.Any, Port);
			LocalSocket = new Socket (LocalEP.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
			RemoteEP = new IPEndPoint (0, 0);
			Callback = new AsyncCallback (EndReceive);
			NewDataReceived = false;
			NewDataToProcess = false;

			BufferASocket = new byte[16];
			BufferBBetween = new byte[16];
			BufferCUnity = new byte[16];
			BufferDSingle = new byte[4];
			Buffer = Quaternion.identity;
		}

		void Start ()
		{
			LocalSocket.Bind (LocalEP);
			BeginReceive ();
		}

		void FixedUpdate ()
		{
			// lock
			try {
				Monitor.Enter (NewDataReceived);

				// data to unity thread
				if (NewDataReceived) {
					Array.Copy (BufferBBetween, 0, BufferCUnity, 0, 16);
					NewDataToProcess = true;
					NewDataReceived = false;
				}

				// unlock
			} finally {
				Monitor.Exit (NewDataReceived);
			}

			if (NewDataToProcess) {
				for (i = 0; i < 4; ++i) {
					// grab value
					Array.Copy (BufferCUnity, i * 4, BufferDSingle, 0, 4);

					// network byte order
					if (BitConverter.IsLittleEndian) {
						Array.Reverse (BufferDSingle);
					}

					// byte to float into quaternion
					Buffer [i] = BitConverter.ToSingle (BufferDSingle, 0);
				}
				Emit (Buffer);
				NewDataToProcess = false;
			}
		}

		public override Quaternion MostRecent ()
		{
			return Buffer;
		}

		private void BeginReceive ()
		{
			// wait for new data
			LocalSocket.BeginReceiveFrom (BufferASocket, 0, 16, SocketFlags.None, ref RemoteEP, Callback, (object)this);
		}

		private void EndReceive (IAsyncResult result)
		{
			// pause receiving
			LocalSocket.EndReceiveFrom (result, ref RemoteEP);

			// lock
			try {
				Monitor.Enter (NewDataReceived);

				// data from socket thread
				Array.Copy (BufferASocket, 0, BufferBBetween, 0, 16);
				NewDataReceived = true;

				// unlock
			} finally {
				Monitor.Exit (NewDataReceived);
			}

			// resume receiving
			BeginReceive ();
		}
	}
}
