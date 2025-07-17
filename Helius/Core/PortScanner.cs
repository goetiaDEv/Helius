
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Helius.Core
{
    public class PortScanner
    {
        public async Task<List<int>> ScanPortsAsync(string ipAddress, List<int> ports)
        {
            Console.WriteLine($"[+] Iniciando varredura de portas em {ipAddress}...");
            var openPorts = new List<int>();
            var tasks = new List<Task>();

            foreach (var port in ports)
            {
                tasks.Add(Task.Run(async () =>
                {
                    using (var tcpClient = new TcpClient())
                    {
                        try
                        {
                            var task = tcpClient.ConnectAsync(ipAddress, port);
                            if (await Task.WhenAny(task, Task.Delay(1000)) == task)
                            {
                                Console.WriteLine($"  [ABERTA] Porta TCP {port}");
                                lock (openPorts)
                                {
                                    openPorts.Add(port);
                                }
                            }
                        }
                        catch (SocketException)
                        {
                            // Porta fechada ou filtrada
                        }
                    }
                }));
            }

            await Task.WhenAll(tasks);
            openPorts.Sort();
            return openPorts;
        }
    }
}


