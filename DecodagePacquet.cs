using System;
using System.Net;

namespace EmettteurReseau
{
    public class DecodagePaquet
    {
        private short numSequence;
        private bool synFlag, ackFlag, finFlag, rstFlag;
        private byte[] data;

        public DecodagePaquet(byte[] packetData)
        {
            if (packetData.Length < 8)
            {
                throw new ArgumentException("Paquet reçu trop court pour contenir un en-tête valide.");
            }

            // Extraction du numéro de séquence
            numSequence = (short)((packetData[0] << 8) | packetData[1]);

            // Extraction des drapeaux
            byte flags = packetData[2];
            synFlag = (flags & 0b1000) != 0;
            ackFlag = (flags & 0b0100) != 0;
            finFlag = (flags & 0b0010) != 0;
            rstFlag = (flags & 0b0001) != 0;

            // Extraction des données
            data = new byte[packetData.Length - 8];
            Array.Copy(packetData, 8, data, 0, data.Length);

            // Le checksum est normalement vérifié ici, mais est omis pour la brièveté
        }

        // Getters
        public short NumSequence => numSequence;
        public bool SynFlag => synFlag;
        public bool AckFlag => ackFlag;
        public bool FinFlag => finFlag;
        public bool RstFlag => rstFlag;
        public byte[] Data => data;
    }
}
