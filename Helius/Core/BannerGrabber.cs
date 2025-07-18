using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Helius.Core
{
    public class BannerGrabber
    {
        public async Task<string?> GrabBannerAsync(string ipAddress, int port, CancellationToken cancellationToken = default)
        {
            using (var client = new TcpClient())
            {
                try
                {
                    var connectTask = client.ConnectAsync(ipAddress, port);
                    var completedTask = await Task.WhenAny(connectTask, Task.Delay(2000, cancellationToken));
                    
                    if (completedTask != connectTask || cancellationToken.IsCancellationRequested)
                    {
                        return null; // Timeout na conexão ou cancelamento
                    }

                    using (var stream = client.GetStream())
                    {
                        stream.ReadTimeout = 2000;
                        var buffer = new byte[1024];
                        var readTask = stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                        
                        var readCompletedTask = await Task.WhenAny(readTask, Task.Delay(2000, cancellationToken));
                        
                        if (readCompletedTask != readTask || cancellationToken.IsCancellationRequested)
                        {
                            return null; // Timeout na leitura ou cancelamento
                        }

                        int bytesRead = await readTask;
                        if (bytesRead > 0)
                        {
                            return Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    return null;
                }
                catch
                {
                    // Ignora erros (serviço pode não enviar banner, fechar conexão, etc.)
                }
            }
            return null;
        }
    }
}