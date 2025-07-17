
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Helius.Core
{
    public class CveChecker
    {
        private readonly HttpClient _httpClient;

        public CveChecker()
        {
            _httpClient = new HttpClient();
            // Adicionar um User-Agent é uma boa prática
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Helius/1.0");
        }

        public async Task CheckVulnerabilitiesAsync(string product, string version)
        {
            // API pública do cve.circl.lu para pesquisa
            string apiUrl = $"https://cve.circl.lu/api/search/{product}/{version}";
            
            Console.WriteLine($"  [CVE] Consultando vulnerabilidades para {product} versão {version}...");

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    // Em um projeto real, você usaria uma biblioteca como Newtonsoft.Json para deserializar
                    // e exibir os resultados de forma estruturada.
                    if (jsonResponse.Length > 2) // Resposta vazia é "[]"
                    {
                        Console.WriteLine($"    [ALERTA] Vulnerabilidades encontradas! Verifique os detalhes em: {apiUrl}");
                    }
                    else
                    {
                        Console.WriteLine("    [OK] Nenhuma CVE encontrada na base de dados para esta versão.");
                    }
                }
                else
                {
                    Console.WriteLine($"    [AVISO] A consulta à API de CVEs falhou com o status: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"    [ERRO] Falha ao conectar à API de CVEs: {ex.Message}");
            }
        }
    }
}


