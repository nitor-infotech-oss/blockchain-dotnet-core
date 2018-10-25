using System;
using System.Collections.Immutable;

namespace blockchain.net
{
    public class Message
    {
        public MessageType Type { get; set; }
        public Block Block { get; set; }
        public Blockchain Blockchain { get; set; }
    }
}