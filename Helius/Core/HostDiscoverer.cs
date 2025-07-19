using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace Helius.Core
{
    public class HostDiscoverer
    {
        public async Task<List<string>> DiscoverHostsAsync(string subnet, CancellationToken cancellationToken = default, Action<string>? onHostFound = null)
        {
            var activeHosts = new List<string>();
            var tasks = new List<Task>();

            for (int i = 1; i < 255; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                string ip = $"{subnet}.{i}";
                var pinger = new Ping();
                
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        var reply = await pinger.SendPingAsync(ip, 1000);
                        if (reply.Status == IPStatus.Success)
                        {
                            onHostFound?.Invoke(reply.Address.ToString());
                            lock (activeHosts)
                            {
                                activeHosts.Add(reply.Address.ToString());
                            }
                        }
                    }
                    catch (PingException)
                    {
                        // Ignorar exceções de ping
                    }
                    finally
                    {
                        pinger.Dispose();
                    }
                }, cancellationToken));
            }

            await Task.WhenAll(tasks);
            return activeHosts;
        }
    }
}