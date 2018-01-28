using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Waher.Networking.PeerToPeer;

/// Components evaluated
// https://github.com/BlueSquid1/P2PNET/wiki/Getting-Started
// Waher.Networking.PeerToPeer
namespace blockchain.net
{
    public class PeerToPeer
    {
        private readonly Blockchain blockchain = new Blockchain();
        private PeerToPeerNetwork server = null;
        private List<PeerConnection> peers = new List<PeerConnection>();

        public PeerToPeer(Blockchain blockchain)
        {
            this.blockchain = blockchain;
        }

        public List<PeerConnection> Peers
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
            server = new PeerToPeerNetwork($"block-chain-{port}", port, port, new Sniffer());

            server.OnPeerConnected += (e, p) =>
            {
                peers.Add(p);
            };

            server.OnStateChange += (object Sender, PeerToPeerNetworkState NewState) =>
            {
                if (NewState == PeerToPeerNetworkState.Ready)
                {
                }
            };

            server.Wait();

            IPAddress ipAddress = null;
            IPAddress.TryParse(host, out ipAddress);
            var peerConnection = await server.ConnectToPeer(new IPEndPoint(ipAddress, port));
            InitMessageHandler(peerConnection);

            await Write(peerConnection, Messages.GetLatestBlock());
        }

        public void InitMessageHandler(PeerConnection peer)
        {
            peer.OnReceived += async (object Sender, byte[] Packet) =>
            {
                string receivedMsg = Helpers.StringFromBytes(Packet);
                Message message = JsonConvert.DeserializeObject<Message>(receivedMsg);
                await HandleMessage(peer, message);
            };
        }

        public async Task ConnectToPeer(string host, int port)
        {
            var peerAddress = $"tcp://{host}:{port}";
            IPAddress ipAddress = null;
            IPAddress.TryParse(host, out ipAddress);
            var peerConnection = await server.ConnectToPeer(new IPEndPoint(ipAddress, port));
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

        public async Task Write(PeerConnection peer, Message message)
        {
            await Task.Factory.StartNew(() =>
            {
                var msg = Helpers.BytesFromMessage(message);
                peer.SendTcp(msg);
            });
        }

        public async Task HandleReceivedLatestBlock(Message message, PeerConnection peer)
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

        public async Task HandleMessage(PeerConnection peer, Message message)
        {
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
