using System;
using System.IO;
using System.Threading.Tasks;

namespace EmetteurReseau
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Utilisation: EmetteurReseau.exe <adresse IP> <port> <chemin du fichier>");
                return;
            }

            string ip = args[0];
            int port = int.Parse(args[1]);
            string filePath = args[2];

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Le fichier n'existe pas. Veuillez vérifier le chemin et réessayer.");
                return;
            }

            try
            {
                EmetteurUDP emetteur = new EmetteurUDP(ip, port, filePath);
                Console.WriteLine("Paquet créé. Envoi en cours...");
                await emetteur.DemarrerAsync();
                Console.WriteLine("Paquet envoyé.");
                await emetteur.AttendreFINACKetEnvoyerACKAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
