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
			string fileName;
			long fileSize;
			var buff = new byte[BUFSIZE];
			var transport = new Transport(BUFSIZE, APP);
			var counter = 0;
           

			while (true)
			{
				Console.WriteLine("debug: server before receive for filename"); //test
				counter = transport.receive(ref buff); // result of transport might be buggy :(
			
                // test changing to get this name and file correctly first time
				fileName = Encoding.UTF8.GetString(buff, 0, buff.Length);
				//fileName = fileName + '\0'; // test

                String file = LIB.extractFileName(fileName);
				Console.WriteLine($"debug: server extracted filename: {file}");
				fileSize = LIB.check_File_Exists(file); //error handling


				// TESTING
				string teststring = "FileToSend.txt";

				if (teststring.IndexOfAny(Path.GetInvalidFileNameChars()) > 0)
                {
                    Console.WriteLine("debug: teststring har illegal filename");
                }
				else
					Console.WriteLine("debug: teststring er helt ok, selvom den ligner den anden");

				int indexOfIllegaChar = file.IndexOfAny(Path.GetInvalidFileNameChars());
				if(indexOfIllegaChar > 0)
				{
					Console.WriteLine($"debug: har illegal filename on index {indexOfIllegaChar}");
					Console.WriteLine($"debug: charen er {file[indexOfIllegaChar]}");
				}

			

				//File.OpenRead(file);


                // TESTING DONE
				Console.WriteLine($"debug: filesize before if statement is {fileSize}");

				if (fileSize != 0)
				{
					Console.WriteLine("debug: server in filesize!=0 before transport.send");
					var fileSizeToSend = Encoding.UTF8.GetBytes(fileSize.ToString());

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