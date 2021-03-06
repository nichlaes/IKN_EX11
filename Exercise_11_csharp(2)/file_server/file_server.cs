using System;
using System.IO;
using System.Text;
using Transportlaget;
using Library;

namespace Application
{
	class file_server
	{
		/// <summary>
		/// The BUFSIZE
		/// </summary>
		private const int BUFSIZE = 1000;
		private const string APP = "FILE_SERVER";

		/// <summary>
		/// Initializes a new instance of the <see cref="file_server"/> class.
		/// </summary>
		private file_server ()
		{
			// While loop
			String fileName;
			long fileSize;
			var buff = new byte[BUFSIZE];
			var transport = new Transport(1000, APP);
			var transport2 = new Transport(1000, APP);
			var counter = 0;

			counter = transport2.receive(ref buff);

            fileName = Encoding.ASCII.GetString(buff, 0, counter);

            String file = LIB.extractFileName(fileName);
            fileSize = LIB.check_File_Exists(file); //error handling

            Console.WriteLine($"Filename: {file}"); 
            Console.WriteLine($"Filename: {file.Length}"); 


				if (fileSize != 0)
				{
					var fileSizeToSend = Encoding.ASCII.GetBytes(fileSize.ToString());

					transport.send(fileSizeToSend, fileSizeToSend.Length);
				    sendFile(file, fileSize, new Transport(BUFSIZE,APP));
				}
                
		}

		/// <summary>
		/// Sends the file.
		/// </summary>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		/// <param name='fileSize'>
		/// File size.
		/// </param>
		/// <param name='tl'>
		/// Tl.
		/// </param>
		private void sendFile(String fileName, long fileSize, Transport transport)
		{
			var buff = new byte[BUFSIZE];



			FileStream fs = new FileStream(fileName,
                FileMode.Open,
                FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = fileSize;
            int sendBytes = 0;

            if (fileSize>=1000)
			{
				for (int i = 0; i < (fileSize / 1000); i++)
                {
                                   
					for (int j = 0; j < BUFSIZE; j++)
                    {
                        buff[j] = br.ReadByte();
                        sendBytes++;
                    }
                    

					transport.send(buff,BUFSIZE);
			    }

			}

			if ((fileSize % 1000) != 0)
            {
				for (int j = 0; j < (fileSize%1000); j++)
                {
					buff[j] = br.ReadByte();   
					sendBytes++;
                }

				transport.send(buff, (int)(fileSize%1000));
             }


            

          
            
		}



		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main (string[] args)
		{
			new file_server();

           
		}
	}
}