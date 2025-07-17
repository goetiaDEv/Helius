
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Helius.Core
{
    public class BannerGrabber
    {
        public async Task<string> GrabBannerAsync(string ipAddress, int port)
        {
            using (var client = new TcpClient())
            {
                try
                {
                    var connectTask = client.ConnectAsync(ipAddress, port);
                    if (await Task.WhenAny(connectTask, Task.Delay(2000)) != connectTask)
                    {
                        return null; // Timeout na conexão
                    }

                    using (var stream = client.GetStream())
                    {
                        stream.ReadTimeout = 2000;
                        var buffer = new byte[1024];
                        var readTask = stream.ReadAsync(buffer, 0, buffer.Length);
                        
                        if (await Task.WhenAny(readTask, Task.Delay(2000)) != readTask)
                        {
                            return null; // Timeout na leitura
                        }

                        int bytesRead = await readTask;
                        if (bytesRead > 0)
                        {
                            return Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();
                        }
                    }
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


