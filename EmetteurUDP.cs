using EmettteurReseau;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace EmetteurReseau
{
        public class EmetteurUDP
        {
            private IPAddress hostAddress;
            private int port;
            private string filePath;
            private UdpClient socket;

        //public EmetteurUDP(string host, int port, string filePath)
        //{
        //    this.hostAddress = IPAddress.Parse(host);
        //    this.port = port;
        //    this.filePath = filePath;
        //    this.socket = new UdpClient();
        //}

        public EmetteurUDP(string host, int port, string filePath)
        {
            // Si l'hôte est "localhost", résolvez-le en une adresse IP locale en fonction du type de socket
            IPAddress address;
            if (host.Equals("localhost", StringComparison.OrdinalIgnoreCase))
            {
                // Utilisez la méthode appropriée pour déterminer si votre socket est IPv4 ou IPv6
                address = Dns.GetHostAddresses(host)
                    .FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork); // Pour IPv4
                                                                                         // Changez InterNetwork en InterNetworkV6 si vous utilisez IPv6
            }
            else
            {
                address = IPAddress.Parse(host);
            }

            this.hostAddress = address;
            this.port = port;
            this.filePath = filePath;
            this.socket = new UdpClient();
        }

        public async Task DemarrerAsync()
            {
                await EnvoyerSYNAsync();
                await AttendreSYNACKAsync();
                await EnvoyerFichierAsync();
            }

            private async Task EnvoyerSYNAsync()
            {
                EncodagePaquet paquetSYN = new EncodagePaquet(1, true, false, false, false, new byte[0], hostAddress, port);
                var packet = paquetSYN.CreationPaquet();
                await socket.SendAsync(packet, packet.Length, new IPEndPoint(hostAddress, port));
                Console.WriteLine("Paquet SYN envoyé.");
                // ImprimerFlags(new DecodagePaquet(packet)); // À implémenter
            }

            private async Task AttendreSYNACKAsync()
            {
                UdpReceiveResult receivedResults = await socket.ReceiveAsync();
                // DecodagePaquet paquetRecu = new DecodagePaquet(receivedResults.Buffer); // À implémenter
                // Logic to handle SYN-ACK
                Console.WriteLine("Paquet SYN-ACK reçu. Connexion établie.");
                // ImprimerFlags(paquetRecu); // À implémenter
            }

            private async Task EnvoyerFichierAsync()
            {
                byte[] fileData = File.ReadAllBytes(filePath);
                int numPackets = (int)Math.Ceiling((double)fileData.Length / EncodagePaquet.PACKET_SIZE);

                for (int i = 0; i < numPackets; i++)
                {
                    int offset = i * EncodagePaquet.PACKET_SIZE;
                    int length = Math.Min(EncodagePaquet.PACKET_SIZE, fileData.Length - offset);
                    byte[] packetData = new byte[length];
                    Array.Copy(fileData, offset, packetData, 0, length);

                    EncodagePaquet paquetData = new EncodagePaquet((short)(i + 2), false, false, false, false, packetData, hostAddress, port);
                    var packet = paquetData.CreationPaquet();
                    await socket.SendAsync(packet, packet.Length, new IPEndPoint(hostAddress, port));
                    Console.WriteLine($"Paquet de données {i + 1} envoyé.");
                }

                // Envoi d'un paquet FIN pour indiquer la fin de la transmission
                EncodagePaquet paquetFin = new EncodagePaquet((short)(numPackets + 2), false, false, true, false, new byte[0], hostAddress, port);
                var finPacket = paquetFin.CreationPaquet();
                await socket.SendAsync(finPacket, finPacket.Length, new IPEndPoint(hostAddress, port));
                Console.WriteLine("Paquet FIN envoyé.");
                // ImprimerFlags(new DecodagePaquet(finPacket)); // À implémenter
            }

            public async Task AttendreFINACKetEnvoyerACKAsync()
            {
                UdpReceiveResult receivedResults = await socket.ReceiveAsync();
                // DecodagePaquet paquetRecu = new DecodagePaquet(receivedResults.Buffer); // À implémenter

                // Check if FIN-ACK received
                Console.WriteLine("Paquet FIN-ACK reçu.");
                EncodagePaquet paquetACK = new EncodagePaquet(4, false, true, false, false, new byte[0], hostAddress, port);
                var ackPacket = paquetACK.CreationPaquet();
                await socket.SendAsync(ackPacket, ackPacket.Length, new IPEndPoint(hostAddress, port));
                socket.Close();
                Console.WriteLine("Paquet ACK final envoyé. Connexion fermée.");
            }
        }
    }

