using System;

namespace blockchain.net
{
    public class Block
    {
        public Block(int index, string previousHash, long timestamp, dynamic data, string hash, int nonce)
        {
            this.Index = index;
            this.PreviousHash = previousHash;
            this.Timestamp = timestamp;
            this.Data = data;
            this.Hash = hash;
            this.Nonce = nonce;
        }

        public int Index
        {
            get; private set;
        }

        public string PreviousHash
        {
            get; private set;
        }

        public long Timestamp
        {
            get; private set;
        }

        public dynamic Data
        {
            get; private set;
        }

        public string Hash
        {
            get; private set;
        }

        public int Nonce
        {
            get; private set;
        }

        public static Block Genesis
        {
            get
            {
                return new Block(
                  0,
                  "0",
                  1508270000000,
                  "Welcome to Blockchain Demo 2.0!",
                  "000dc75a315c77a1f9c98fb6247d03dd18ac52632d7dc6a9920261d8109b37cf",
                  604
                );
            }
        }
    }
}