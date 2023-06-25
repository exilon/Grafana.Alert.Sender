namespace Domain.DTOs
{
    public class Alert
    {
        public string? status { get; set; }
        public Dictionary<string, string>? labels { get; set; }
        public Dictionary<string, string>? annotations { get; set; }
        public string? startsAt { get; set; }
        public string? endsAt { get; set; }
        public string? generatorURL { get; set; }
        public string? fingerprint { get; set; }
        public string? silenceURL { get; set; }
        public string? dashboardURL { get; set; }
        public string? panelURL { get; set; }
        public Dictionary<string, double>? values { get; set; }
        public string? valueString { get; set; }
    }

    public class GrafanaAlert
    {
        public string? receiver { get; set; }
        public string? status { get; set; }
        public List<Alert>? alerts { get; set; }
        public Dictionary<string, string>? groupLabels { get; set; }
        public Dictionary<string, string>? commonLabels { get; set; }
        public Dictionary<string, string>? commonAnnotations { get; set; }
        public string? externalURL { get; set; }
        public string? version { get; set; }
        public string? groupKey { get; set; }
        public int truncatedAlerts { get; set; }
        public int orgId { get; set; }
        public string? title { get; set; }
        public string? state { get; set; }
        public string? message { get; set; }
    }




}