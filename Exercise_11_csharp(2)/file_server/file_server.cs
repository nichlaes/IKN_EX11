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
			// TO DO Your own code
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
			// TO DO Your own code
		}

		public void send(ref byte[] buf, int size)
        {
			var buffer = new byte[20];

			buffer[0] = (byte)'A';
            var j = 1;

            for (int i = 0; i < size; i++)
            {
                switch (buf[i])
                {
                    case (byte)'A':
                        buffer[j++] = (byte)'B';
                        buffer[j++] = (byte)'c';
                        break;

                    case (byte)'B':
                        buffer[j++] = (byte)'B';
                        buffer[j++] = (byte)'D';
                        break;

                    default:
                        buffer[j++] = buf[i];
                        break;
                }
            }

			buffer[j] = (byte)'A';
			buf = buffer;
        }

        

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main (string[] args)
		{
			//new file_server();

           
		}
	}
}