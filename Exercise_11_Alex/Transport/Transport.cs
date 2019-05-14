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
			// we using dataReceived instead
			//bool receivedACK = false;
			//byte[] buff = new byte[(size + 4)];

			/*// Send one packet with data of size 1000 untill whole message is sent
			for (int i = 0; i < buffer.Length; i += buffer.Length)
			{
				buffer[(int)TransCHKSUM.SEQNO] = (byte)seqNo;
                buffer[(int)TransCHKSUM.TYPE] = (byte)TransType.DATA;
                
				Array.Copy(buf, i, buffer, 4, buffer.Length - 4);

				checksum.calcChecksum(ref buffer, buffer.Length);

				while (!receivedACK)
                {
                    Console.WriteLine("debug: transport.send in receivedAck while loop begining");
                    link.send(buff, buff.Length);
                    Console.WriteLine("debug: transport.send in receivedAck while loop middle");
                    receivedACK = receiveAck();
                    Console.WriteLine("debug: transport.send in receivedAck while loop end");
                }

				receivedACK = false;

				//seqNo = (byte)((buffer[(int)TransCHKSUM.SEQNO] + 1) % 2);
                              
			}*/

			if (size > buffer.Length)
				Console.WriteLine("size of message give to buffer is too big");

			Array.Copy(buf, 0, buffer, 4, size); // size because application doesn't give too big packets

			buffer[(int)TransCHKSUM.SEQNO] = (byte)seqNo;
			buffer[(int)TransCHKSUM.TYPE] = (byte)TransType.DATA;

			checksum.calcChecksum(ref buffer, buffer.Length);

			while (!dataReceived)
			{
				Console.WriteLine("debug: transport.send in receivedAck while loop begining");
                link.send(buffer, buffer.Length);
                Console.WriteLine("debug: transport.send in receivedAck while loop middle");
				dataReceived = receiveAck();
                Console.WriteLine("debug: transport.send in receivedAck while loop end");
            }
			dataReceived = false; // remember to reset
            // Only sendAck and receiveAck should change the seqNo
			//seqNo = (byte)((buffer[(int)TransCHKSUM.SEQNO] + 1) % 2); 
        }

		/// <summary>
		/// Receive the specified buffer.
		/// </summary>
		/// <param name='buffer'>
		/// Buffer.
		/// </param>
		public int receive (ref byte[] buf)
		{
//			var buff = new byte[buf.Length];
       
            Console.WriteLine("debug: transport.receive before link.receive");
            recvSize = link.receive(ref buffer);
            Console.WriteLine("debug: transport.receive after link.receive");

			while(!checksum.checkChecksum(buffer, recvSize)||buffer[(int)TransCHKSUM.SEQNO] != seqNo)
			{
				Console.WriteLine("debug: receive in checksum while loop begining");
				sendAck(false);
				Console.WriteLine("debug: receive in checksum while loop between");
                recvSize = link.receive(ref buffer);    
				Console.WriteLine("debug: receive in checksum while loop end");
			}

			Console.WriteLine("debug: receive checksum checked out");
				Array.Copy(buffer, 4, buf, 0, (recvSize - 4));
                sendAck(true);
				  

            /*
             * 
            var buff = new byte[buf.Length];
            Console.WriteLine($"")
            Console.WriteLine("debug: transport.receive before link.receive");
            recvSize = link.receive(ref buff);
            Console.WriteLine("debug: transport.receive after link.receive");

            while(!checksum.checkChecksum(buff, recvSize)||buff[(int)TransCHKSUM.SEQNO] != seqNo)
            {
                Console.WriteLine("debug: receive in checksum while loop begining");
                sendAck(false);
                Console.WriteLine("debug: receive in checksum while loop between");
                recvSize = link.receive(ref buff);    
                Console.WriteLine("debug: receive in checksum while loop end");
            }

            Console.WriteLine("debug: receive checksum checked out");
                Array.Copy(buff, 4, buf, 0, (recvSize - 4));
                sendAck(true);*/

            
			return recvSize;
		}
	}
}