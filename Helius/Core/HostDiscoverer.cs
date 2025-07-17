using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Helius.Core
{
    public class HostDiscoverer
    {
        public async Task<List<string>> DiscoverHostsAsync(string subnet)
        {
            Console.WriteLine($"[+] Iniciando descoberta de hosts na sub-rede {subnet}.0/24...");
            var activeHosts = new List<string>();
            var tasks = new List<Task>();

            for (int i = 1; i < 255; i++)
            {
                string ip = $"{subnet}.{i}";
                var pinger = new Ping();
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        var reply = await pinger.SendPingAsync(ip, 1000);
                        if (reply.Status == IPStatus.Success)
                        {
                            Console.WriteLine($"  [ATIVO] Host encontrado: {reply.Address}");
                            lock (activeHosts)
                            {
                                activeHosts.Add(reply.Address.ToString());
                            }
                        }
                    }
                    catch (PingException)
                    {
                        // Ignorar exceções de ping (ex: host não responde)
                    }
                    finally
                    {
                        pinger.Dispose();
                    }
                }));
            }

            await Task.WhenAll(tasks);
            Console.WriteLine($"[+] Descoberta concluída. {activeHosts.Count} hosts ativos encontrados.");
            return activeHosts;
        }
    }
}


