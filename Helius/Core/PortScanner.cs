using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Helius.Core
{
    public class PortScanner
    {
        public async Task<List<int>> ScanPortsAsync(string ipAddress, List<int> ports, CancellationToken cancellationToken = default, Action<int>? onPortFound = null)
        {
            var openPorts = new List<int>();
            var tasks = new List<Task>();

            foreach (var port in ports)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                tasks.Add(Task.Run(async () =>
                {
                    using (var tcpClient = new TcpClient())
                    {
                        try
                        {
                            var connectTask = tcpClient.ConnectAsync(ipAddress, port);
                            var completedTask = await Task.WhenAny(connectTask, Task.Delay(1000, cancellationToken));
                            
                            if (completedTask == connectTask && !cancellationToken.IsCancellationRequested)
                            {
                                onPortFound?.Invoke(port);
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
                        catch (OperationCanceledException)
                        {
                            // Operação cancelada
                        }
                    }
                }, cancellationToken));
            }

            await Task.WhenAll(tasks);
            openPorts.Sort();
            return openPorts;
        }
    }
}