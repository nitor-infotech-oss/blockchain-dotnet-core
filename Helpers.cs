using System;
using System.Text;

namespace blockchain.net
{
    public static class Helpers
    {
        /// <summary>
        /// Convert an array of bytes to a string of hex digits
        /// </summary>
        /// <param name="bytes">array of bytes</param>
        /// <returns>String of hex digits</returns>
        public static string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Convert an array of bytes to a string of hex digits
        /// </summary>
        /// <param name="bytes">array of bytes</param>
        /// <returns>String of hex digits</returns>
        public static byte[] BytesFromMessage(Message message)
        {
            var messageJson = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            return Encoding.ASCII.GetBytes(messageJson);
        }

        /// <summary>
        /// Convert an array of bytes to a string of hex digits
        /// </summary>
        /// <param name="bytes">array of bytes</param>
        /// <returns>String of hex digits</returns>
        public static string StringFromBytes(byte[] bytes)
        {
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
    }
}