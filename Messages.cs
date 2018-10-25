using System;
using System.Collections.Immutable;

namespace blockchain.net
{
    public class Messages
    {
        public static Message GetLatestBlock()
        {
            return new Message
            {
                Type = MessageType.REQUEST_LATEST_BLOCK
            };
        }

        public static Message SendLatestBlock(Block block)
        {
            return new Message
            {
                Type = MessageType.RECEIVE_LATEST_BLOCK,
                Block = block
            };
        }

        public static Message GetBlockchain()
        {
            return new Message
            {
                Type = MessageType.REQUEST_BLOCKCHAIN
            };
        }

        public static Message SendBlockchain(Blockchain blockchain)
        {
            return new Message
            {
                Type = MessageType.RECEIVE_BLOCKCHAIN,
                Blockchain = blockchain
            };
        }
    }
}
