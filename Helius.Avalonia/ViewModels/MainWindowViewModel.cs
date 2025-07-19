using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using Helius.Core;
using Helius.Models;

namespace Helius.Avalonia.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    private readonly HostDiscoverer _hostDiscoverer;
    private readonly PortScanner _portScanner;
    private readonly BannerGrabber _bannerGrabber;
    private readonly CveChecker _cveChecker;
    private CancellationTokenSource? _cancellationTokenSource;

    private string _targetIp = "127.0.0.1";
    private string _subnetBase = "192.168.1";
    private string _portsToScan = "21,22,80,443,3306,8080";
    private bool _isScanning = false;
    private bool _isHostDiscoveryEnabled = false;
    private string _consoleOutput = "";
    private int _scanProgress = 0;
    private string _scanStatus = "Pronto para iniciar";

    public ObservableCollection<ScanResultItem> ScanResults { get; } = new();

    public string TargetIp
    {
        get => _targetIp;
        set => this.RaiseAndSetIfChanged(ref _targetIp, value);
    }

    public string SubnetBase
    {
        get => _subnetBase;
        set => this.RaiseAndSetIfChanged(ref _subnetBase, value);
    }

    public string PortsToScan
    {
        get => _portsToScan;
        set => this.RaiseAndSetIfChanged(ref _portsToScan, value);
    }

    public bool IsScanning
    {
        get => _isScanning;
        set => this.RaiseAndSetIfChanged(ref _isScanning, value);
    }

    public bool IsHostDiscoveryEnabled
    {
        get => _isHostDiscoveryEnabled;
        set => this.RaiseAndSetIfChanged(ref _isHostDiscoveryEnabled, value);
    }

    public string ConsoleOutput
    {
        get => _consoleOutput;
        set => this.RaiseAndSetIfChanged(ref _consoleOutput, value);
    }

    public int ScanProgress
    {
        get => _scanProgress;
        set => this.RaiseAndSetIfChanged(ref _scanProgress, value);
    }

    public string ScanStatus
    {
        get => _scanStatus;
        set => this.RaiseAndSetIfChanged(ref _scanStatus, value);
    }

    public ICommand StartScanCommand { get; }
    public ICommand StopScanCommand { get; }
    public ICommand ClearResultsCommand { get; }
    public ICommand ClearConsoleCommand { get; }

    public MainWindowViewModel()
    {
        _hostDiscoverer = new HostDiscoverer();
        _portScanner = new PortScanner();
        _bannerGrabber = new BannerGrabber();
        _cveChecker = new CveChecker();

        StartScanCommand = ReactiveCommand.CreateFromTask(StartScan, this.WhenAnyValue(x => x.IsScanning).Select(x => !x));
        StopScanCommand = ReactiveCommand.Create(StopScan, this.WhenAnyValue(x => x.IsScanning));
        ClearResultsCommand = ReactiveCommand.Create(ClearResults);
        ClearConsoleCommand = ReactiveCommand.Create(ClearConsole);
    }

    private async Task StartScan()
    {
        IsScanning = true;
        ScanProgress = 0;
        ScanStatus = "Iniciando varredura...";
        ScanResults.Clear();
        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            AppendToConsole("[INFO] Iniciando varredura Helius...", ConsoleMessageType.Info);

            List<string> hostsToScan = new();

            if (IsHostDiscoveryEnabled)
            {
                ScanStatus = "Descobrindo hosts...";
                AppendToConsole($"[+] Descobrindo hosts na sub-rede {SubnetBase}.0/24...", ConsoleMessageType.Info);
                
                hostsToScan = await _hostDiscoverer.DiscoverHostsAsync(SubnetBase, _cancellationTokenSource.Token, 
                    (host) => AppendToConsole($"  [ATIVO] Host encontrado: {host}", ConsoleMessageType.Success));
                
                AppendToConsole($"[+] Descoberta concluída. {hostsToScan.Count} hosts ativos encontrados.", ConsoleMessageType.Info);
            }
            else
            {
                hostsToScan.Add(TargetIp);
            }

            if (hostsToScan.Count == 0)
            {
                AppendToConsole("[-] Nenhum host ativo encontrado.", ConsoleMessageType.Warning);
                return;
            }

            var ports = ParsePorts(PortsToScan);
            int totalOperations = hostsToScan.Count * ports.Count;
            int currentOperation = 0;

            foreach (var host in hostsToScan)
            {
                if (_cancellationTokenSource.Token.IsCancellationRequested) break;

                ScanStatus = $"Escaneando {host}...";
                AppendToConsole($"\n[+] Iniciando varredura em: {host}", ConsoleMessageType.Info);

                var openPorts = await _portScanner.ScanPortsAsync(host, ports, _cancellationTokenSource.Token,
                    (port) => 
                    {
                        AppendToConsole($"  [ABERTA] Porta TCP {port}", ConsoleMessageType.Success);
                        currentOperation++;
                        ScanProgress = (int)((double)currentOperation / totalOperations * 100);
                    });

                if (openPorts.Count == 0)
                {
                    AppendToConsole("[-] Nenhuma porta aberta encontrada.", ConsoleMessageType.Warning);
                    continue;
                }

                AppendToConsole($"[+] Portas abertas encontradas: {string.Join(", ", openPorts)}", ConsoleMessageType.Info);

                foreach (var port in openPorts)
                {
                    if (_cancellationTokenSource.Token.IsCancellationRequested) break;

                    AppendToConsole($"\n--- Analisando Porta {port} ---", ConsoleMessageType.Info);
                    
                    var banner = await _bannerGrabber.GrabBannerAsync(host, port, _cancellationTokenSource.Token);
                    
                    var result = new ScanResultItem
                    {
                        Host = host,
                        Port = port,
                        Status = "Aberta",
                        Banner = banner ?? "Não disponível",
                        Service = GetServiceName(port),
                        Vulnerabilities = new List<string>()
                    };

                    if (!string.IsNullOrEmpty(banner))
                    {
                        AppendToConsole($"  [INFO] Banner: {banner}", ConsoleMessageType.Info);
                        
                        var productInfo = ExtractProductInfo(banner);
                        if (productInfo.HasValue)
                        {
                            var cves = await _cveChecker.CheckVulnerabilitiesAsync(productInfo.Value.Product, productInfo.Value.Version, _cancellationTokenSource.Token);
                            result.Vulnerabilities.AddRange(cves);
                            
                            if (cves.Any())
                            {
                                AppendToConsole($"  [ALERTA] {cves.Count} vulnerabilidades encontradas!", ConsoleMessageType.Error);
                            }
                            else
                            {
                                AppendToConsole("  [OK] Nenhuma CVE encontrada na base de dados.", ConsoleMessageType.Success);
                            }
                        }
                    }
                    else
                    {
                        AppendToConsole("  [INFO] Não foi possível obter o banner.", ConsoleMessageType.Warning);
                    }

                    ScanResults.Add(result);
                }
            }

            ScanStatus = "Varredura concluída";
            ScanProgress = 100;
            AppendToConsole("\n[+] Varredura concluída com sucesso!", ConsoleMessageType.Success);
        }
        catch (OperationCanceledException)
        {
            ScanStatus = "Varredura cancelada";
            AppendToConsole("\n[!] Varredura cancelada pelo usuário.", ConsoleMessageType.Warning);
        }
        catch (Exception ex)
        {
            ScanStatus = "Erro na varredura";
            AppendToConsole($"\n[ERRO] Falha na varredura: {ex.Message}", ConsoleMessageType.Error);
        }
        finally
        {
            IsScanning = false;
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }

    private void StopScan()
    {
        _cancellationTokenSource?.Cancel();
        ScanStatus = "Cancelando...";
        AppendToConsole("\n[!] Solicitação de cancelamento enviada...", ConsoleMessageType.Warning);
    }

    private void ClearResults()
    {
        ScanResults.Clear();
        AppendToConsole("[INFO] Resultados da varredura limpos.", ConsoleMessageType.Info);
    }

    private void ClearConsole()
    {
        ConsoleOutput = "";
    }

    private void AppendToConsole(string message, ConsoleMessageType type = ConsoleMessageType.Info)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        var prefix = type switch
        {
            ConsoleMessageType.Success => "[SUCCESS]",
            ConsoleMessageType.Error => "[ERROR]",
            ConsoleMessageType.Warning => "[WARNING]",
            _ => "[INFO]"
        };

        ConsoleOutput += $"{timestamp} {prefix} {message}\n";
    }

    private List<int> ParsePorts(string portsString)
    {
        var ports = new List<int>();
        var parts = portsString.Split(new char[] { 
',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var part in parts)
        {
            if (int.TryParse(part.Trim(), out int port))
            {
                ports.Add(port);
            }
        }
        
        return ports;
    }

    private string GetServiceName(int port)
    {
        return port switch
        {
            21 => "FTP",
            22 => "SSH",
            23 => "Telnet",
            25 => "SMTP",
            53 => "DNS",
            80 => "HTTP",
            110 => "POP3",
            143 => "IMAP",
            443 => "HTTPS",
            993 => "IMAPS",
            995 => "POP3S",
            3306 => "MySQL",
            3389 => "RDP",
            5432 => "PostgreSQL",
            8080 => "HTTP-Alt",
            _ => "Desconhecido"
        };
    }

    private (string Product, string Version)? ExtractProductInfo(string banner)
    {
        var parts = banner.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length > 0)
        {
            var productInfo = parts[0].Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (productInfo.Length == 2)
            {
                return (productInfo[0], productInfo[1]);
            }
        }
        return null;
    }
}

public enum ConsoleMessageType
{
    Info,
    Success,
    Warning,
    Error
}

public class ScanResultItem
{
    public string Host { get; set; } = "";
    public int Port { get; set; }
    public string Status { get; set; } = "";
    public string Service { get; set; } = "";
    public string Banner { get; set; } = "";
    public List<string> Vulnerabilities { get; set; } = new();
    public string VulnerabilitiesText => Vulnerabilities.Count > 0 ? string.Join(", ", Vulnerabilities) : "Nenhuma";
}



