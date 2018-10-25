using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using blockchain.net.Sockets;
using blockchain.net.Sockets.EventArgs;
using Newtonsoft.Json;

/// Components evaluated
// https://github.com/BlueSquid1/P2PNET/wiki/Getting-Started
// Waher.Networking.PeerToPeer
namespace blockchain.net
{
    public class PeerToPeer
    {
        private readonly Blockchain blockchain = new Blockchain();
        private TcpListener server = null;
        private List<ClientSocket> peers = new List<ClientSocket>();

        public PeerToPeer(Blockchain blockchain)
        {
            this.blockchain = blockchain;
        }

        public List<ClientSocket> Peers
        {
            get
            {
                return peers;
            }
        }

        public async Task StartServer(int port)
        {
            var host = "127.0.0.1";
            var peerAddress = $"tcp://{host}:{port}";
            server = new TcpListener(IPAddress.Parse(host), port);
            server.Start();
            await Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    var connectedClient = await server.AcceptTcpClientAsync();
                    var peer = new ClientSocket(connectedClient);
                    await InitMessageHandler(peer);
                }
            });
            // TcpClient client = new TcpClient();
            // await client.ConnectAsync(host, port);
        }

        public async Task InitMessageHandler(ClientSocket peer)
        {
            this.peers.Add(peer);
            peer.Connected += (c) =>
            {
                Console.WriteLine($"Connected: {peer} -> {c.BaseSocket.Endpoint}");
            };

            peer.DataReceived += async (DataReceivedArgs args) =>
            {
                string receivedMsg = Helpers.StringFromBytes(args.Data);
                Message message = JsonConvert.DeserializeObject<Message>(receivedMsg);
                await HandleMessage(peer, message);
            };
            await this.Write(peer, Messages.GetLatestBlock());
        }

        public async Task ConnectToPeer(string host, int port)
        {
            var peerAddress = $"tcp://{host}:{port}";
            IPAddress ipAddress = null;
            IPAddress.TryParse(host, out ipAddress);
            TcpClient client = new TcpClient();
            await client.ConnectAsync(host, port);
            var peer = new ClientSocket(client);
            await InitMessageHandler(peer);
        }

        public void DiscoverPeers()
        {
        }

        public void BroadcastLatest()
        {
            this.Broadcast(Messages.SendLatestBlock(this.blockchain.LatestBlock));
        }

        public void Broadcast(Message message)
        {
            this.peers.ForEach(peer => this.Write(peer, message).Wait());
        }

        public async Task Write(ClientSocket peer, Message message)
        {
            await Task.Factory.StartNew(() =>
            {
                var msg = Helpers.BytesFromMessage(message);
                peer.Send(msg);
            });
        }

        public async Task HandleReceivedLatestBlock(Message message, ClientSocket peer)
        {
            var receivedBlock = message.Block;
            var latestBlock = this.blockchain.LatestBlock;

            if (latestBlock.Hash == receivedBlock.PreviousHash)
            {
                try
                {
                    this.blockchain.AddBlock(receivedBlock);
                }
                catch (Exception err)
                {
                    throw err;
                }
            }
            else if (receivedBlock.Index > latestBlock.Index)
            {
                await this.Write(peer, Messages.GetBlockchain());
            }
            else
            {
                // Do nothing.
            }
        }

        public void HandleReceivedBlockchain(Message message)
        {
            var receivedChain = message.Blockchain;

            try
            {
                this.blockchain.ReplaceChain(receivedChain);
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public async Task HandleMessage(ClientSocket peer, Message message)
        {
            Console.WriteLine($"message.Type => {message.Type} => {peer.Endpoint}");
            
            switch (message.Type)
            {
                case MessageType.REQUEST_LATEST_BLOCK:
                    await this.Write(peer, Messages.SendLatestBlock(this.blockchain.LatestBlock));
                    break;
                case MessageType.REQUEST_BLOCKCHAIN:
                    await this.Write(peer, Messages.SendBlockchain(this.blockchain));
                    break;
                case MessageType.RECEIVE_LATEST_BLOCK:
                    await this.HandleReceivedLatestBlock(message, peer);
                    break;
                case MessageType.RECEIVE_BLOCKCHAIN:
                    this.HandleReceivedBlockchain(message);
                    break;
                default:
                    throw new Exception("Received invalid message.");
            }
        }

        public void CloseConnection()
        {
            server = null;
        }
    }
}
