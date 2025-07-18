using Helius.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helius
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("--- Helius Scanner - Iniciando ---");

            // --- Exemplo 1: Descoberta de Hosts ---
            // var hostDiscoverer = new HostDiscoverer();
            // await hostDiscoverer.DiscoverHostsAsync("192.168.0"); // Altere para sua sub-rede

            // --- Exemplo 2: Varredura Completa em um Único Alvo ---
            string targetIp = "127.0.0.1"; // Mude para o seu alvo. Use um IP que você tenha permissão para escanear!
            List<int> portsToScan = new List<int> { 21, 22, 80, 443, 3306, 8080 };

            var portScanner = new PortScanner();
            var bannerGrabber = new BannerGrabber();
            var cveChecker = new CveChecker();

            Console.WriteLine($"\n[+] Iniciando varredura em: {targetIp}");

            List<int> openPorts = await portScanner.ScanPortsAsync(targetIp, portsToScan);

            if (openPorts.Count == 0)
            {
                Console.WriteLine("[-] Nenhuma porta aberta encontrada.");
                return;
            }

            Console.WriteLine($"\n[+] Portas abertas encontradas: {string.Join(", ", openPorts)}");

            foreach (var port in openPorts)
            {
                Console.WriteLine($"\n--- Analisando Porta {port} ---");
                string banner = await bannerGrabber.GrabBannerAsync(targetIp, port);

                if (!string.IsNullOrEmpty(banner))
                {
                    Console.WriteLine($"  [INFO] Banner: {banner}");
                    // Lógica simples para extrair produto/versão (pode ser melhorada com Regex)
                    string[] parts = banner.Split(' ');
                    if (parts.Length > 0)
                    {
                        string[] productInfo = parts[0].Split('/');
                        if (productInfo.Length == 2)
                        {
                            await cveChecker.CheckVulnerabilitiesAsync(productInfo[0], productInfo[1]);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("  [INFO] Não foi possível obter o banner.");
                }
            }

            Console.WriteLine("\n--- Varredura Concluída ---");
        }
    }
}


