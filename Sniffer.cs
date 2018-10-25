
using System;

namespace blockchain.net
{
    public class Sniffer //: ISniffer
    {
        public void Error(string error)
        {
            Console.WriteLine("Error: " + error);
        }

        public void Exception(string error)
        {
            Console.WriteLine("Exception: " + error);
        }

        public void Information(string error)
        {
            Console.WriteLine("Information: " + error);
        }

        public void ReceiveBinary(byte[] error)
        {
            Console.WriteLine("ReceiveBinary: " + error);
        }

        public void ReceiveText(string error)
        {
            Console.WriteLine("ReceiveText: " + error);
        }

        public void TransmitBinary(byte[] error)
        {
            Console.WriteLine("TransmitBinary: " + error);
        }

        public void TransmitText(string error)
        {
            Console.WriteLine("TransmitText: " + error);
        }

        public void Warning(string error)
        {
            Console.WriteLine("Warning: " + error);
        }
    }
}