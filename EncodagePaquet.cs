using System;
using System.Net;
using System.Net.Sockets;
using System.Linq;

namespace EmettteurReseau
{
    public class EncodagePaquet
    {
        public static readonly int MAX_PACKET_SIZE = 1024; // Taille maximale du paquet
        private static readonly int HEADER_SIZE = 8; // Taille de l'en-tête
        public static readonly int PACKET_SIZE = 1000; // Taille ajustée pour les données
        private short numSequence;
        private bool synFlag, ackFlag, finFlag, rstFlag;
        private byte[] data;
        private IPAddress destinationAddress;
        private int destinationPort;

        public EncodagePaquet(short numSequence, bool synFlag, bool ackFlag, bool finFlag, bool rstFlag, byte[] data, IPAddress address, int port)
        {
            this.numSequence = numSequence;
            this.synFlag = synFlag;
            this.ackFlag = ackFlag;
            this.finFlag = finFlag;
            this.rstFlag = rstFlag;
            this.data = data;
            this.destinationAddress = address;
            this.destinationPort = port;
        }

        public byte[] CreationPaquet()
        {
            byte[] header = new byte[HEADER_SIZE];
            // Construction de l'en-tête
            header[0] = (byte)(numSequence >> 8);
            header[1] = (byte)(numSequence);
            header[2] = (byte)((synFlag ? 0b1000 : 0) | (ackFlag ? 0b0100 : 0) | (finFlag ? 0b0010 : 0) | (rstFlag ? 0b0001 : 0));
            // Placeholder pour le checksum (indices 3 et 4)
            // Espace réservé (indices 5 à 7)

            byte[] packetData = new byte[header.Length + data.Length];
            Buffer.BlockCopy(header, 0, packetData, 0, header.Length);
            Buffer.BlockCopy(data, 0, packetData, HEADER_SIZE, data.Length);

            // Calcul du checksum (exemple simple, à améliorer pour une utilisation réelle)
            short checksum = CalculateChecksum(packetData);
            packetData[3] = (byte)(checksum >> 8);
            packetData[4] = (byte)(checksum);

            return packetData;
        }

        private short CalculateChecksum(byte[] data)
        {
            // Implémentation simplifiée. Pour une vraie application, utilisez une fonction de checksum robuste.
            int checksum = 0;
            foreach (var b in data)
            {
                checksum += b;
            }
            return (short)(checksum % short.MaxValue);
        }

        // Getters
        public short NumSequence { get { return numSequence; } }
        public bool SynFlag { get { return synFlag; } }
        public bool AckFlag { get { return ackFlag; } }
        public bool FinFlag { get { return finFlag; } }
        public bool RstFlag { get { return rstFlag; } }
        public byte[] Data { get { return data; } }
        public IPAddress DestinationAddress { get { return destinationAddress; } }
        public int DestinationPort { get { return destinationPort; } }
    }
}
