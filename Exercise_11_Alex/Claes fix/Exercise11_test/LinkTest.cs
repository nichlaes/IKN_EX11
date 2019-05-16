using Linklaget;
using NUnit.Framework;

namespace Exercise11_test
{
    public class LinkTest
    {
        private Link linker;
        private byte[] buffer;
        private byte[] buf = new byte[]{(byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E'};
        private byte[] buff = new byte[1000]; 
        private byte[] sendBuf= new byte[] { (byte)'A', (byte)'B', (byte)'C', (byte)'B', (byte)'D', (byte)'C', (byte)'D', (byte)'E', (byte)'A' };

        [SetUp]
        public void Setup()
        {
          // linker = new Link(1000,"");
           buffer = new byte[1000];
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        public void LinkSend(int index)
        {
            int size = buf.Length;
            

            buffer[0] = (byte)'A';
            var j = 1;

            for (int i = 0; i < size; i++)
            {
                switch (buf[i])
                {
                    case (byte)'A':
                        buffer[j++] = (byte)'B';
                        buffer[j++] = (byte)'C';
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

            buffer[j++] = (byte)'A';

          
            Assert.AreEqual(buffer[index],sendBuf[index]);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(4)]
        public void LinkReceive(int index)
        {
            
            int byteReceived;
            bool doneReading = false;
            var i = 0;
            var DelCount = 0;
            /*
            while (!doneReading)
            {
                byteReceived = sendBuf[i];
                if (byteReceived == (byte)'A')
                {
                    DelCount++;
                    if (DelCount == 2)
                        doneReading = true;
                }

                buff[i++] = (byte)byteReceived;

            }
            */
            i = 0;
            var j = 0;

            if (sendBuf[i] == (byte)'A')
            {
                i++;

                while (sendBuf[i] != (byte)'A')
                {
                    if (sendBuf[i] == (byte)'B')
                    {
                        i++;

                        if (sendBuf[i] == (byte)'C')
                            buffer[j++] = (byte)'A';
                        else buffer[j++] = (byte)'B';
                    }
                    else { buffer[j++] = sendBuf[i]; }

                    i++;
                }
            }

            Assert.AreEqual(buffer[index],buf[index]);
            Assert.AreEqual(buffer[--j], (byte)'E');


        }





    }
}