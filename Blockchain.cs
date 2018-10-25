using System;
using System.Collections.Immutable;
using System.Security.Cryptography;
using System.Text;

namespace blockchain.net
{
    public class Blockchain
    {
        private ImmutableList<Block> blockchain = ImmutableList<Block>.Empty;
        private int difficulty;

        public Blockchain()
        {
            this.blockchain = blockchain.Add(Block.Genesis);
            this.difficulty = 3;
        }

        public ImmutableList<Block> Chain
        {
            get
            {
                return this.blockchain;
            }
        }

        public Block LatestBlock
        {
            get
            {
                return this.blockchain[this.blockchain.Count - 1];
            }
        }

        public bool IsValidHashDifficulty(string hash)
        {
            var i = 0;
            for (i = 0; i < hash.Length; i++)
            {
                if (hash[i] != '0')
                {
                    break;
                }
            }
            return i >= this.difficulty;
        }

        public dynamic CalculateHashForBlock(Block block)
        {
            return this.CalculateHash(
              block.Index,
              block.PreviousHash,
              block.Timestamp,
              block.Data,
              block.Nonce
            );
        }

        public string CalculateHash(int index, string previousHash, long timestamp, dynamic data, int nonce)
        {
            SHA256 sha256 = SHA256Managed.Create();
            string hashData = index + previousHash + timestamp + data + nonce;

            byte[] hashBytes = sha256.ComputeHash(Encoding.ASCII.GetBytes(hashData));
            return Helpers.HexStringFromBytes(hashBytes);
        }

        public void Mine(dynamic data)
        {
            Block newBlock = this.GenerateNextBlock(data);
            try
            {
                this.AddBlock(newBlock);
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public Block GenerateNextBlock(dynamic data)
        {
            int nextIndex = this.LatestBlock.Index + 1;
            string previousHash = this.LatestBlock.Hash;
            long timestamp = new DateTime().Ticks;
            int nonce = 0;
            string nextHash = this.CalculateHash(
              nextIndex,
              previousHash,
              timestamp,
              data,
              nonce
            );

            while (!this.IsValidHashDifficulty(nextHash))
            {
                nonce = nonce + 1;
                timestamp = new DateTime().Ticks;
                nextHash = this.CalculateHash(
                  nextIndex,
                  previousHash,
                  timestamp,
                  data,
                  nonce
                );
            }

            Block nextBlock = new Block(
              nextIndex,
              previousHash,
              timestamp,
              data,
              nextHash,
              nonce
            );

            return nextBlock;
        }

        public void AddBlock(Block newBlock)
        {
            if (this.IsValidNextBlock(newBlock, this.LatestBlock))
            {
                this.blockchain = this.blockchain.Add(newBlock);
            }
            else
            {
                throw new Exception("Error: Invalid block");
            }
        }

        public bool IsValidNextBlock(Block nextBlock, Block previousBlock)
        {
            string nextBlockHash = this.CalculateHashForBlock(nextBlock);

            if (previousBlock.Index + 1 != nextBlock.Index)
            {
                return false;
            }
            else if (previousBlock.Hash != nextBlock.PreviousHash)
            {
                return false;
            }
            else if (nextBlockHash != nextBlock.Hash)
            {
                return false;
            }
            else if (!this.IsValidHashDifficulty(nextBlockHash))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool IsValidChain(Blockchain chain)
        {
            // JSON.stringify(chain[0]) !== JSON.stringify(Block.genesis)
            if (chain.Chain[0] != Block.Genesis)
            {
                return false;
            }

            var tempChain = ImmutableList<Block>.Empty;
            tempChain.Add(chain.Chain[0]);

            for (var i = 1; i < chain.Chain.Count; i = i + 1)
            {
                if (this.IsValidNextBlock(chain.Chain[i], tempChain[i - 1]))
                {
                    tempChain = tempChain.Add(chain.Chain[i]);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsChainLonger(Blockchain chain)
        {
            return chain.Chain.Count > this.blockchain.Count;
        }

        public void ReplaceChain(Blockchain newChain)
        {
            if (this.IsValidChain(newChain) && this.IsChainLonger(newChain))
            {
                this.blockchain = newChain.Chain;
            }
            else
            {
                throw new Exception("Error: invalid chain");
            }
        }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(Chain);
        }
    }
}
