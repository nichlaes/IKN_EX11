using System;
using System.IO;
using System.Text;
using Transportlaget;
using Library;
using Linklaget;

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
			var transport = new Transport(BUFSIZE, APP);
			var counter = 0;
           

			while (true)
			{
				Console.WriteLine("debug: server before receive"); //test
				counter = transport.receive(ref buff);
				Console.WriteLine("debug: server after receive"); //test

                // test changing to get this name and file correctly first time
				fileName = Encoding.ASCII.GetString(buff,0, counter);

                string file = LIB.extractFileName(fileName);
				Console.WriteLine($"debug: server extracted filename: {file}");
				fileSize = LIB.check_File_Exists(file); //error handling

				Console.WriteLine($"debug: filesize before if statement is {fileSize}");

				if (fileSize != 0)
				{
					Console.WriteLine("debug: server in filesize!=0 before transport.send");
					var fileSizeToSend = Encoding.ASCII.GetBytes(fileSize.ToString());

					transport.send(fileSizeToSend, fileSizeToSend.Length);
					Console.WriteLine("debug: server in after transport.send before sendFile()");
					sendFile(file, fileSize, transport);
					Console.WriteLine("debug: server in after sendFile()");
				}

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
				for (int i = 0; i < fileSize / 1000; i++)
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        buff[j] = br.ReadByte();
                        sendBytes++;
                    }

					transport.send(buff,BUFSIZE);
			    }

			}

			if (fileSize % 1000 != 0)
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