using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Helius.Core
{
    public class CveChecker
    {
        private readonly HttpClient _httpClient;

        public CveChecker()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Helius/1.0");
        }

        public async Task<List<string>> CheckVulnerabilitiesAsync(string product, string version, CancellationToken cancellationToken = default)
        {
            var cveList = new List<string>();
            string apiUrl = $"https://cve.circl.lu/api/search/{product}/{version}";

            try
            {
                var response = await _httpClient.GetAsync(apiUrl, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    
                    if (jsonResponse.Length > 2) // Resposta vazia é "[]"
                    {
                        try
                        {
                            var cveData = JsonConvert.DeserializeObject<List<CveItem>>(jsonResponse);
                            if (cveData != null)
                            {
                                foreach (var cve in cveData)
                                {
                                    if (!string.IsNullOrEmpty(cve.Id))
                                    {
                                        cveList.Add(cve.Id);
                                    }
                                }
                            }
                        }
                        catch (JsonException)
                        {
                            // Em caso de erro no parsing JSON, adiciona indicação genérica
                            cveList.Add("Vulnerabilidades detectadas - consulte API");
                        }
                    }
                }
            }
            catch (HttpRequestException)
            {
                // Falha na conexão com a API
            }
            catch (OperationCanceledException)
            {
                // Operação cancelada
            }

            return cveList;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }

    public class CveItem
    {
        [JsonProperty("id")]
        public string Id { get; set; } = "";

        [JsonProperty("summary")]
        public string Summary { get; set; } = "";

        [JsonProperty("Published")]
        public string Published { get; set; } = "";

        [JsonProperty("cvss")]
        public double Cvss { get; set; }
    }
}