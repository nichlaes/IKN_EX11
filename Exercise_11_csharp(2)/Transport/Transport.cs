using System;
using Linklaget;

/// <summary>
/// Transport.
/// </summary>
namespace Transportlaget
{
	/// <summary>
	/// Transport.
	/// </summary>
	public class Transport
	{
		/// <summary>
		/// The link.
		/// </summary>
		private Link link;
		/// <summary>
		/// The 1' complements checksum.
		/// </summary>
		private Checksum checksum;
		/// <summary>
		/// The buffer.
		/// </summary>
		private byte[] buffer;
		/// <summary>
		/// The seq no.
		/// </summary>
		private byte seqNo;
		/// <summary>
		/// The old_seq no.
		/// </summary>
		private byte old_seqNo;
		/// <summary>
		/// The error count.
		/// </summary>
		private int errorCount;
		/// <summary>
		/// The DEFAULT_SEQNO.
		/// </summary>
		private const int DEFAULT_SEQNO = 2;
		/// <summary>
		/// The data received. True = received data in receiveAck, False = not received data in receiveAck
		/// </summary>
		private bool dataReceived;
		/// <summary>
		/// The number of data the recveived.
		/// </summary>
		private int recvSize = 0;

		/// <summary>
		/// Initializes a new instance of the <see cref="Transport"/> class.
		/// </summary>
		public Transport (int BUFSIZE, string APP)
		{
			link = new Link(BUFSIZE+(int)TransSize.ACKSIZE, APP);
			checksum = new Checksum();
			buffer = new byte[BUFSIZE+(int)TransSize.ACKSIZE];
			seqNo = 0;
			old_seqNo = DEFAULT_SEQNO;
			errorCount = 0;
			dataReceived = false;
		}

		/// <summary>
		/// Receives the ack.
		/// </summary>
		/// <returns>
		/// The ack.
		/// </returns>
		private bool receiveAck()
		{
			recvSize = link.receive(ref buffer);
			dataReceived = true;

			if (recvSize == (int)TransSize.ACKSIZE) {
				dataReceived = false;
				if (!checksum.checkChecksum (buffer, (int)TransSize.ACKSIZE) ||
				  buffer [(int)TransCHKSUM.SEQNO] != seqNo ||
				  buffer [(int)TransCHKSUM.TYPE] != (int)TransType.ACK)
				{
					return false;
				}
				seqNo = (byte)((buffer[(int)TransCHKSUM.SEQNO] + 1) % 2);
			}
 
			return true;
		}

		/// <summary>
		/// Sends the ack.
		/// </summary>
		/// <param name='ackType'>
		/// Ack type.
		/// </param>
		private void sendAck (bool ackType)
		{
			byte[] ackBuf = new byte[(int)TransSize.ACKSIZE];
			ackBuf [(int)TransCHKSUM.SEQNO] = (byte)
				(ackType ? (byte)buffer [(int)TransCHKSUM.SEQNO] : (byte)(buffer [(int)TransCHKSUM.SEQNO] + 1) % 2);
			ackBuf [(int)TransCHKSUM.TYPE] = (byte)(int)TransType.ACK;
			checksum.calcChecksum (ref ackBuf, (int)TransSize.ACKSIZE);
			link.send(ackBuf, (int)TransSize.ACKSIZE);
		}

		/// <summary>
		/// Send the specified buffer and size.
		/// </summary>
		/// <param name='buffer'>
		/// Buffer.
		/// </param>
		/// <param name='size'>
		/// Size.
		/// </param>
		public void send(byte[] buf, int size)
		{
			bool receivedACK = false;
			byte[] buff;

			if (size <= 1000)
			{
				buff = new byte[(size + 4)];
				Array.Copy(buf, 0, buff, 4, size);

				buff[(int)TransCHKSUM.SEQNO] = (byte)seqNo;
				buff[(int)TransCHKSUM.TYPE] = (byte)0;

				checksum.calcChecksum(ref buff, buff.Length);

				while (!receivedACK)
				{
					link.send(buff, buff.Length);
					receivedACK = receiveAck();
				}
				receivedACK = false;
			}
			else
			{
				buff = new byte[1004];

				for (int i = 0; i < (size / 1000); i++)
				{
					Array.Copy(buf, (i * 1000), buff, 4, 1000);
					buff[(int)TransCHKSUM.SEQNO] = (byte)seqNo;
					buff[(int)TransCHKSUM.TYPE] = (byte)TransType.DATA;

					checksum.calcChecksum(ref buff, buff.Length);

					while (!receivedACK)
					{
						link.send(buff, buff.Length);
						receivedACK = receiveAck();
					}
					receivedACK = false;
				}

				if (size % 1000 != 0)
				{
					Array.Copy(buf, (size/1000*1000), buff, 4, size%1000);
					buff[(int)TransCHKSUM.SEQNO] = (byte)seqNo;
					buff[(int)TransCHKSUM.TYPE] = (byte)TransType.DATA;

					checksum.calcChecksum(ref buff, size%1000);

					while (!receivedACK)
					{
						link.send(buff, buff.Length);
						receivedACK = receiveAck();
					}
					receivedACK = false;
                    
				}

			}
		}

		/// <summary>
		/// Receive the specified buffer.
		/// </summary>
		/// <param name='buffer'>
		/// Buffer.
		/// </param>
		public int receive (ref byte[] buf)
		{
			var sumReceived = 0;
			var buff = new byte[buf.Length];
			recvSize = link.receive(ref buff);

			if (checksum.checkChecksum(buf, recvSize))
			{
				Array.Copy(buff, 4, buf, sumReceived, (recvSize - 4));
				sendAck(true);
			}
			else sendAck(false);
				
   

            
			return sumReceived;
		}
	}
}