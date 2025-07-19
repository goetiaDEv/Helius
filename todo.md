# Helius - Lista de Tarefas e Próximas Atualizações

Este documento lista as funcionalidades atuais do Helius e as próximas etapas para o desenvolvimento da ferramenta.

## Funcionalidades Atuais (Implementadas)

- [x] **Descoberta de Hosts (Ping/ICMP)**: Identificação de IPs ativos na rede.
- [x] **Varredura de Portas (TCP)**: Verificação de portas TCP abertas em hosts específicos.
- [x] **Banner Grabbing**: Captura de informações sobre serviços em portas abertas.
- [x] **Verificação de CVEs (Básica)**: Comparação de versões de serviços com APIs de vulnerabilidades (ex: cve.circl.lu).
- [x] **Estrutura de Projeto C#**: Organização do código em classes e namespaces para modularidade.
- [x] **README.md**: Documentação inicial do projeto para GitHub.
- [x] **Interface Gráfica utilizando o Avalonia UI**: Impletada a interface gráfica para facilitar a visualização dos dados e funções.

## Próximas Atualizações e Melhorias (Roadmap)

### 1. Melhorias na Varredura de Portas

- [ ] **Varredura de Portas UDP**: Implementar a varredura de portas UDP, que é mais complexa devido à natureza sem conexão do protocolo.
- [ ] **Otimização de Performance**: Ajustar o paralelismo e timeouts para varreduras mais rápidas e eficientes em redes maiores.
- [ ] **Tipos de Scan TCP**: Adicionar suporte a diferentes tipos de scan TCP (SYN scan, FIN scan, Xmas scan, etc.) para maior furtividade e precisão.

### 2. Aprimoramento do Banner Grabbing

- [ ] **Parsing Avançado de Banners**: Utilizar expressões regulares (Regex) ou bibliotecas específicas para extrair de forma mais robusta o nome do produto e a versão do banner.
- [ ] **Identificação de Serviços**: Mapear banners conhecidos para serviços específicos (ex: porta 80 com banner 'Apache' -> Servidor Web Apache).

### 3. Gerenciamento e Análise de CVEs

- [ ] **Integração com NVD (National Vulnerability Database)**: Implementar a consulta e o processamento de dados do NVD para uma base de dados de vulnerabilidades mais abrangente.
- [ ] **Banco de Dados Local de CVEs**: Criar um banco de dados local (ex: SQLite) para armazenar CVEs e permitir consultas offline e mais rápidas.
- [ ] **Relatórios de Vulnerabilidades**: Gerar relatórios claros e concisos das vulnerabilidades encontradas, incluindo CVE-IDs, descrições e links para mais informações.

### 4. Interface e Usabilidade

- [ ] **Argumentos de Linha de Comando**: Implementar um parser de argumentos (ex: `System.CommandLine` ou `CommandLineParser`) para facilitar a configuração do scan via linha de comando.
- [ ] **Modo Verbose**: Adicionar um modo detalhado para exibir mais informações durante o scan.
- [ ] **Saída para Arquivo**: Opção para salvar os resultados do scan em diferentes formatos (JSON, CSV, TXT).

### 5. Outras Funcionalidades

- [ ] **Detecção de Sistema Operacional (OS Fingerprinting)**: Tentar identificar o sistema operacional do host alvo.
- [ ] **Detecção de Firewall/IDS**: Implementar técnicas para identificar a presença de firewalls ou sistemas de detecção de intrusão.
- [ ] **Módulos de Exploração (Opcional)**: No futuro, considerar a adição de módulos básicos para testar a exploração de vulnerabilidades conhecidas (com extrema cautela e apenas em ambientes controlados).

## Contribuição

Sinta-se à vontade para contribuir com o projeto Helius! Para isso, siga os passos:

1.  Faça um fork do repositório.
2.  Crie uma nova branch para sua funcionalidade (`git checkout -b feature/nova-funcionalidade`).
3.  Faça suas alterações e commit (`git commit -m 'feat: Adiciona nova funcionalidade X'`).
4.  Envie para a branch (`git push origin feature/nova-funcionalidade`).
5.  Abra um Pull Request.

---

*Última atualização: 17 de julho de 2025*

