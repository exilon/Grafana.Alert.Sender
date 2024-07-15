using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Application.Abstractions;
using Conditions;
using Domain.DTOs;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Infrastructure.Services.AlertGenerators
{
    public class HtmlAlertGenerator : IAlertGenerator
    {
        private readonly GrafanaOptions  _grafanaOptions;
        public HtmlAlertGenerator(IOptions<GrafanaOptions> grafanaOptions)
        {
            _grafanaOptions = grafanaOptions.Value;
        }

        private string GetDashboardUidFromPanelUrl(string panelUrl)
        {
            string pattern = @"d\/(.*?)\?";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(panelUrl);
            string value = match.Groups[1].Value;
            return value;
        }

        private string GetOrgIdFromPanelUrl(string panelUrl)
        {
            string pattern = @"orgId=(\d+)";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(panelUrl);
            string value = match.Groups[1].Value;
            return value;
        }

        private string GetPanelIdFromPanelUrl(string panelUrl)
        {
            string pattern = @"viewPanel=(\d+)";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(panelUrl);
            string value = match.Groups[1].Value;
            return value;
        }

        private string ExtractMetricsAndValues(string valueString)
        {
            string pattern = @"metric='(?<metric>[^']*)'\s+[^}]*}\s+value=(?<value>[^ ]*)";

            StringBuilder result = new StringBuilder();
            foreach (Match match in Regex.Matches(valueString, pattern))
            {
                string metric = match.Groups["metric"].Value;
                string value = match.Groups["value"].Value;

                result.AppendLine($"{metric}: {value}");
            }

            return result.ToString();
        }

        public async Task<Stream> GenerateImage(Alert alert)
        {
            Console.WriteLine(JsonConvert.SerializeObject(alert));
            string dashboardUID = GetDashboardUidFromPanelUrl(alert.panelURL);
            string orgId = GetOrgIdFromPanelUrl(alert.panelURL);
            string panelId = GetPanelIdFromPanelUrl(alert.panelURL);
            dashboardUID.Requires().IsNotNullOrEmpty();
            panelId.Requires().IsNotNullOrEmpty();
            if (string.IsNullOrEmpty(orgId)) orgId = "1";
            var url = $"{_grafanaOptions.Url}/render/d-solo/{dashboardUID}?orgId={orgId}&panelId={panelId}&width={_grafanaOptions.ImageWidth}&height={_grafanaOptions.ImageHeight}&tz=Europe%2FMadrid";
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(30);
            var accessToken = _grafanaOptions.Token;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var contentType = response.Content.Headers.ContentType.MediaType;
                if (!contentType.StartsWith("image/")) throw new Exception("No preview for that dashboard");
                    
                var stream = await response.Content.ReadAsStreamAsync();
                return stream;
            }
            return null;
            
        }

        private string FixGrafanaUrl(string url)
        {
            var correctPort = url.Replace(":3000", "");
            var correctUrl = correctPort.Replace("http://localhost", _grafanaOptions.Url);
            return correctUrl;
        }

        public string GenerateAlert(Alert alert)
        {
            if (alert == null) throw new Exception("Alert cannot be null!");

            StringBuilder template = new StringBuilder();
            //title
            string title = (alert.status == "firing") ? "🔥 Alert" : "✅ Resolved";
            template.AppendLine($"<b>{title}</b>");
            template.AppendLine("");
            string alertname = alert.labels?.GetValueOrDefault("alertname");
            template.AppendLine($"<b>{alertname}</b>");
            string message = alert.annotations?.GetValueOrDefault("message");
            template.AppendLine($" 🔹{message}");
            template.AppendLine("");
            //body
            template.AppendLine(ExtractMetricsAndValues(alert.valueString));
            string severity = alert.labels?.GetValueOrDefault("severity");
            template.AppendLine($"Severity: {severity}");
            string host = alert.labels?.GetValueOrDefault("host");
            template.AppendLine($"Host: {host}");
            string location = alert.labels?.GetValueOrDefault("location");
            template.AppendLine($"Location: {location}");
            template.AppendLine($"Starts: {DateTimeISOParse(alert.startsAt)}");
            if (alert.status != "firing") template.AppendLine($"Ends: {DateTimeISOParse(alert.endsAt)}");

            string summary = alert.annotations?.GetValueOrDefault("summary");
            template.AppendLine($"Info: {summary}");

            //bottom part
            template.AppendLine("");
            template.Append($"<a href='{FixGrafanaUrl(alert.dashboardURL)}'>Dashboard</a> | ");
            template.Append($"<a href='{FixGrafanaUrl(alert.panelURL)}'>Panel</a> | ");
            template.AppendLine($"<a href='{FixGrafanaUrl(alert.silenceURL)}'>Silent</a>");

            return template.ToString();
        }

        private string RemovePrecissionFromDateTimeISO(string dateTimeISO)
        {
            if (dateTimeISO.Contains('.')) return dateTimeISO.Substring(0, dateTimeISO.IndexOf('.')) + 'Z';
            return dateTimeISO;
        }

        private string DateTimeISOParse(string dateTimeISO)
        { 
            try
            {
                var fixedDate = RemovePrecissionFromDateTimeISO(dateTimeISO);
                
                DateTime convertedDate;
                if (DateTime.TryParseExact(fixedDate, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out convertedDate))
                {
                    return convertedDate.ToString("dd/MM/yyyy hh:mm:ss");
                }
                return fixedDate;
            }
            catch 
            { 
                return dateTimeISO;
            }
        }
    }
}
