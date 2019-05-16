using System;
using System.IO;
using System.Text;
using Transportlaget;
using Library;

namespace Application
{
	class file_client
	{
		/// <summary>
		/// The BUFSIZE.
		/// </summary>
		private const int BUFSIZE = 1000;
		private const string APP = "FILE_CLIENT";

		/// <summary>
		/// Initializes a new instance of the <see cref="file_client"/> class.
		/// 
		/// file_client metoden opretter en peer-to-peer forbindelse
		/// Sender en forspÃ¸rgsel for en bestemt fil om denne findes pÃ¥ serveren
		/// Modtager filen hvis denne findes eller en besked om at den ikke findes (jvf. protokol beskrivelse)
		/// Lukker alle streams og den modtagede fil
		/// Udskriver en fejl-meddelelse hvis ikke antal argumenter er rigtige
		/// </summary>
		/// <param name='args'>
		/// Filnavn med evtuelle sti.
		/// </param>
	    private file_client(String[] args)
	    {
	    	// TO DO Your own code
            string fileToRequest = args[0];

            // LinkLayer connection is already established, as it is a serial connection, and the APP string
            // determines which end (client or server) of the serial connection this application is.
            var transportConnection = new Transport(BUFSIZE, APP);
			//New transportConnection
            var transportConnection2 = new Transport(BUFSIZE, APP);

            byte[] fileToRequestBytes = Encoding.ASCII.GetBytes(fileToRequest);
            transportConnection2.send(fileToRequestBytes, fileToRequestBytes.Length); // Request specific files size           

                      

            // Get the filesize
            byte[] fileSizeBufferBytes = new byte[1000];
            int fileSizeRecvSize = transportConnection.receive(ref fileSizeBufferBytes);

			Console.WriteLine($"Filesize: {fileSizeRecvSize}");

            if (fileSizeRecvSize > 0)// && != errorcode
            {
				Console.WriteLine($"Filesize of requested file is: {Encoding.ASCII.GetString(fileSizeBufferBytes)}");
				receiveFile(fileToRequest, long.Parse(Encoding.ASCII.GetString(fileSizeBufferBytes)), transportConnection);
            }


        }

		/// <summary>
		/// Receives the file.
		/// </summary>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		/// <param name='transport'>
		/// Transportlaget
		/// </param>
		private void receiveFile (String fileName, long fileSize, Transport transport)
		{
            // TO DO Your own code
            FileStream fs = File.Create(fileName);
            long bytesReceived = 0;
            int bytesReceivingNow;
            byte [] buf = new byte[BUFSIZE];
            
            byte[] fileNameBytes = Encoding.ASCII.GetBytes(fileName);

            while (bytesReceived < fileSize)
            {
                bytesReceivingNow = transport.receive(ref buf);
                fs.Write(buf, 0, bytesReceivingNow);
                bytesReceived += bytesReceivingNow;
            }

            fs.Close();
        }



		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// First argument: Filname
		/// </param>
		public static void Main (string[] args)
		{
            Console.WriteLine("Client starting up");
			new file_client(args);
		}
	}
}