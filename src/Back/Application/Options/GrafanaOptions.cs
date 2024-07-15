namespace Application.Options;

public class GrafanaOptions
{
    public string Url { get; set; }
    public string Token { get; set; }
    public int ImageWidth { get; set; }
    public int ImageHeight { get; set; }
    public bool RejectNoData { get; set; }
    public string RedirectNoDataToContact { get; set; }
}