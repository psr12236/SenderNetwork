using System;
using System.IO;
using System.Threading.Tasks;

namespace EmetteurReseau
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Console.WriteLine("Hello and welcome!");
            //Console.WriteLine("Give me the path of the file you want to send:");
            //string path = Console.ReadLine();

            //if (!File.Exists(path))
            //{
            //    Console.WriteLine("The file does not exist. Please check the path and try again.");
            //    return;
            //}

            var pathsToTest = new List<string>
            {
                "C:\\Users\\marcl\\Desktop\\ManipGit.txt",
                @"C:\Users\marcl\Desktop\ManipGit.txt",
                Path.Combine("C:", "Users", "marcl", "Desktop", "ManipGit.txt"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ManipGit.txt")
            };

            string validPath = pathsToTest.FirstOrDefault(File.Exists);

            if (validPath == null)
            {
                Console.WriteLine("The file does not exist. Please check the paths and try again.");
                return;
            }

            try
            {
                EmetteurUDP emetteur = new EmetteurUDP("localhost", 9876, validPath);
                Console.WriteLine("Packet created. Sending...");
                await emetteur.DemarrerAsync();
                Console.WriteLine("Packet sent.");
                await emetteur.AttendreFINACKetEnvoyerACKAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
