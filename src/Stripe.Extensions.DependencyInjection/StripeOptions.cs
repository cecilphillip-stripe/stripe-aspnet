namespace Stripe.Extensions.DependencyInjection;

public class StripeOptions
{
    public const string ConfigurationSectionName = "Stripe";
    public string PublicKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
    public bool EnableTelemetry { get; set; } = true;
    public int MaxNetworkRetries { get; set; } = SystemNetHttpClient.DefaultMaxNumberRetries;
}