```
██╗  ██╗███████╗██╗     ██╗██╗   ██╗███████╗
██║  ██║██╔════╝██║     ██║██║   ██║██╔════╝
███████║█████╗  ██║     ██║██║   ██║███████╗
██╔══██║██╔══╝  ██║     ██║██║   ██║╚════██║
██║  ██║███████╗███████╗██║╚██████╔╝███████║
╚═╝  ╚═╝╚══════╝╚══════╝╚═╝ ╚═════╝ ╚══════╝
```
                                            
[![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-green)](./LICENSE)
[![Status](https://img.shields.io/badge/status-em%20desenvolvimento-orange)]()

Helius é uma poderosa ferramenta de linha de comando desenvolvida em **C#** para escanear redes locais em busca de vulnerabilidades. Criada como um projeto educacional, ela é ideal para quem deseja aprofundar conhecimentos em **programação de rede, segurança ofensiva e concorrência com .NET**.

---

## Principais Funcionalidades

O scanner Helius é modular, dividido nos seguintes componentes:

1.  **Descoberta de Hosts**  
   Detecta dispositivos ativos em uma sub-rede via pacotes ICMP (Ping).

2.  **Varredura de Portas**  
   Identifica portas TCP abertas em um host específico.

3.  **Captura de Banner**  
   Coleta informações dos serviços rodando nas portas abertas (como versão e nome).

4.  **Análise de CVEs** *(em progresso)*  
   Compara os dados coletados com bases públicas de vulnerabilidades conhecidas (CVE).

---

##  Estrutura do Projeto

```bash
Helius/
│
├── Helius.sln
│
└───Helius/
    ├── Program.cs              # Ponto de entrada da aplicação
    ├───Core/
    │   ├── HostDiscoverer.cs   # Lógica de descoberta de hosts
    │   ├── PortScanner.cs      # Lógica de varredura de portas
    │   ├── BannerGrabber.cs    # Captura de banners de serviços
    │   └── CveChecker.cs       # Consulta e análise de vulnerabilidades (CVE)
    └───Models/
        └── ScanResult.cs       # Modelo de dados para os resultados (a ser implementado)
```

---

##  Como Executar

###  Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Um editor como [Visual Studio Code](https://code.visualstudio.com/) ou [Visual Studio](https://visualstudio.microsoft.com/)

###  Passos

```bash
# 1. Clone o repositório
git clone https://github.com/seu-usuario/helius.git
cd helius

# 2. Compile o projeto
dotnet build

# 3. Execute a aplicação
dotnet run
```

* Dica: personalize o arquivo `Program.cs` para definir os alvos da varredura.*

---

##  Exemplo de Uso

Dentro do `Program.cs`:

```csharp
string targetIp = "192.168.0.1";
List<int> targetPorts = new List<int> { 21, 22, 80, 443 };

Console.WriteLine($"Iniciando varredura completa em {targetIp}...");

// Lógica de execução personalizada aqui
```

---

##  Aviso Legal

> Esta ferramenta é **exclusivamente para fins educacionais**.  
> Utilize-a **apenas em redes que você possui ou tem permissão explícita para testar**.  
> O uso não autorizado pode ser considerado crime conforme legislações locais.  
> O autor **não se responsabiliza** por qualquer uso indevido da ferramenta.

---

##  Roadmap (Verifique o arquivo todo)

- [x] Descoberta de hosts via ICMP  
- [x] Varredura de portas TCP  
- [x] Captura de banner
- [ ] Interface gráfica multiplataforma (under development)   
- [ ] Análise automatizada de CVEs  
- [ ] Exportação de resultados (JSON/CSV)   

---

Feito com 💻, café ☕ e um toque de paranoia 🔒.
