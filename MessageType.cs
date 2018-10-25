using System;

namespace blockchain.net
{
    public enum MessageType
    {
        REQUEST_LATEST_BLOCK = 0,
        RECEIVE_LATEST_BLOCK = 1,
        REQUEST_BLOCKCHAIN = 2,
        RECEIVE_BLOCKCHAIN = 3,
    };
}
