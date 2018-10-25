using System;

namespace blockchain.net
{
    public class Application
    {
        private static Blockchain blockchain = new Blockchain();
        private static PeerToPeer p2p = new PeerToPeer(blockchain);
        public static void Start()
        {
            Console.WriteLine("Welcome to Blockchain CLI!");
            string choice = string.Empty;

            Menu();
            do
            {
                choice = Input();

                switch (choice)
                {
                    case "1":
                        {
                            Connect();
                            Pause();
                            break;
                        }
                    case "3":
                        {
                            Console.WriteLine(blockchain.ToString());
                            Pause();
                            break;
                        }
                    case "4":
                        {
                            Peers();
                            Pause();
                            break;
                        }
                    case "5":
                        {
                            Mine();
                            Pause();
                            break;
                        }
                    case "6":
                        {
                            Open();
                            Pause();
                            break;
                        }
                    default:
                        break;
                }

            } while (!choice.Equals("q", StringComparison.InvariantCultureIgnoreCase));
        }

        private static void Menu()
        {
            Console.WriteLine("1. connect <host> <port>, Connect to a new peer. Eg: connect localhost 2727");
            Console.WriteLine("2. discover, Discover new peers from your connected peers.");
            Console.WriteLine("3. blockchain, See the current state of the blockchain.");
            Console.WriteLine("4. peers, Get the list of connected peers.");
            Console.WriteLine("5. mine <data>, Mine a new block. Eg: mine hello!");
            Console.WriteLine("6. open <port>, Open port to accept incoming connections. Eg: open 2727");
            Console.WriteLine("7. 'q' to exit application");
        }

        private static string Input()
        {
            Console.WriteLine();
            Console.Write("Enter your command: ");
            return Console.ReadLine();
        }

        private static void Connect()
        {
            Console.WriteLine("Enter host to connect (127.0.0.1): ");
            string host = Console.ReadLine();
            if(string.IsNullOrWhiteSpace(host)){
                host = "127.0.0.1";
            }
            Console.WriteLine("Enter port to connect: ");
            string input = Console.ReadLine();
            int port = 0;
            if (int.TryParse(input, out port))
            {
                p2p.ConnectToPeer(host, port).Wait();
                Console.WriteLine($"Connected to peer ${host}:{input}");
            }
            else
            {
                Console.WriteLine("Invalid port!");
            }
        }

        private static void Mine()
        {
            Console.WriteLine("Enter data to mine: ");
            string data = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(data))
            {
                blockchain.Mine(data);
                p2p.BroadcastLatest();
            }
        }

        private static void Peers()
        {
            p2p.Peers.ForEach((peer) => Console.WriteLine(peer.Endpoint.Address));
        }

        private static void Open()
        {
            Console.WriteLine("Enter port to accept incoming connections: ");
            string input = Console.ReadLine();
            int port = 0;
            if (int.TryParse(input, out port))
            {
                p2p.StartServer(port).Wait();
                Console.WriteLine($"Listening to peers on {input}");
            }
            else
            {
                Console.WriteLine("Invalid port!");
            }
        }

        private static void Pause()
        {
            // Console.Write("Your operation is complete, press any key to continue......");
            // Console.ReadKey();
        }
    };
}
