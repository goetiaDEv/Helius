using System.Collections.Generic;

namespace Helius.Models
{
    public class ScanResult
    {
        public string Host { get; set; } = "";
        public List<PortResult> Ports { get; set; } = new();
        public bool IsActive { get; set; }
        public string OperatingSystem { get; set; } = "";
        public string ScanTimestamp { get; set; } = "";
    }

    public class PortResult
    {
        public int Port { get; set; }
        public string Status { get; set; } = "";
        public string Service { get; set; } = "";
        public string Banner { get; set; } = "";
        public List<string> Vulnerabilities { get; set; } = new();
        public string Product { get; set; } = "";
        public string Version { get; set; } = "";
    }

    public class VulnerabilityInfo
    {
        public string CveId { get; set; } = "";
        public string Summary { get; set; } = "";
        public double CvssScore { get; set; }
        public string Severity { get; set; } = "";
        public string PublishedDate { get; set; } = "";
    }
}