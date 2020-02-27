namespace EA.UsageTracking.Infrastructure.Data
{
    public interface IUsageTrackingContextFactory
    {
        UsageTrackingContext UsageTrackingContext { get; }
    }
}