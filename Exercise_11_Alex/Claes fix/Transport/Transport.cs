using System;
using System.Text;
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
   
			buff = new byte[(size + 4)];
			Array.Copy(buf, 0, buff, 4, size);

			buff[(int)TransCHKSUM.SEQNO] = (byte)seqNo;
			buff[(int)TransCHKSUM.TYPE] = (byte)TransType.DATA;

			checksum.calcChecksum(ref buff, buff.Length);

			int i = 0; // testing

		//	while (!receivedACK)
		//	{
		//		i++; // testing

		//		Console.WriteLine($"debug: in the send while loop: {i}");

		//		Console.WriteLine("In Transport.send inden link send"); //test
				link.send(buff, size + 4);
				receivedACK = receiveAck();
			Console.WriteLine($"receivedACK{receivedACK}");
			Console.WriteLine($"SeqNo: {seqNo}");
			seqNo = (byte)((buff[(int)TransCHKSUM.SEQNO] + 1) % 2);
			//Console.WriteLine($"SeqNo: {seqNo=(1+seqNo)%2}");
		//	}

			//	i = 0;


		}
        
		/// <summary>
		/// Receive the specified buffer.
		/// </summary>
		/// <param name='buffer'>
		/// Buffer.
		/// </param>
		public int receive (ref byte[] buf)
		{
			var buff = new byte[(buf.Length + 4)];
			Console.WriteLine("In Transport.receive"); //test
			//Console.WriteLine($"The transport.receive buffer has length {buff.Length}");
			recvSize = link.receive(ref buff);

			while((!checksum.checkChecksum(buff, recvSize)))//||(buff[(int)TransCHKSUM.SEQNO] != seqNo))
			{

				bool checksumResult = checksum.checkChecksum(buff, recvSize);
				Console.WriteLine($"{checksumResult}");
				Console.WriteLine("In Transport.receive while loop before ack"); //test
				sendAck(false);
                recvSize = link.receive(ref buff); 
			}
    			Console.WriteLine("In Transport.receive after while loop");
				Array.Copy(buff, 4, buf, 0, (recvSize - 4));
                sendAck(true);
			//seqNo = (byte)((buff[(int)TransCHKSUM.SEQNO] + 1) % 2);

			Console.WriteLine($"SeqNo of recevier(this terminal): {seqNo}\n SeqNo of sender (other terminal) {buff[(int)TransCHKSUM.SEQNO]}");
				  

            
			return (recvSize-4);
		}
               

	}
}