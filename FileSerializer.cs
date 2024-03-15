using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EmettteurReseau
{
    class FileSerializer
    {
        private string nomFichier;
        private long tailleFichier;
        private byte[] contenu;

        // Constructeur pour initialiser les métadonnées et le contenu du fichier
        public FileSerializer(string cheminDocument)
        {
            // Lecture du contenu du fichier
            contenu = File.ReadAllBytes(cheminDocument);
            FileInfo fileInfo = new FileInfo(cheminDocument);
            nomFichier = fileInfo.Name;
            tailleFichier = contenu.Length;
        }

        // Méthode pour préparer les données à envoyer
        public byte[] DonneeAEnvoyer()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
                {
                    // Écriture de la longueur du nom du fichier
                    binaryWriter.Write(nomFichier.Length);
                    // Écriture du nom du fichier
                    binaryWriter.Write(Encoding.UTF8.GetBytes(nomFichier));
                    // Écriture du contenu du fichier
                    binaryWriter.Write(contenu);
                }
                return memoryStream.ToArray();
            }
        }

        // Getters
        public string GetNomFichier() => nomFichier;
        public long GetTailleFichier() => tailleFichier;
        public byte[] GetFileContent() => contenu;
    }
}
