namespace FaceAuth.Contracts.Admin;

public sealed class UsageSummaryResponse
{
    public Guid TenantId { get; set; }
    public int TotalRequests { get; set; }
    public IReadOnlyList<UsageEndpointCount> ByEndpoint { get; set; } = [];
}

public sealed class UsageEndpointCount
{
    public string Endpoint { get; set; } = string.Empty;
    public int Count { get; set; }
}
