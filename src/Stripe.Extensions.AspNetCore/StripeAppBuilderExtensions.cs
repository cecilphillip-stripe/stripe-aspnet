﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Stripe.Extensions.DependencyInjection;

namespace Stripe.Extensions.AspNetCore;

public static class StripeAppBuilderExtensions
{
    public static IEndpointRouteBuilder MapStripeWebhookHandler<T>(this IEndpointRouteBuilder endpointRouteBuilder,
        string namedConfiguration = StripeOptions.DefaultClientConfigurationSectionName)
        where T : StripeWebhookHandler
    {
        if (namedConfiguration == null)
            throw new ArgumentNullException(nameof(namedConfiguration));

        return endpointRouteBuilder.MapStripeWebhookHandler<T>("/stripe/webhook", namedConfiguration);
    }

    public static IEndpointRouteBuilder MapStripeWebhookHandler<T>(this IEndpointRouteBuilder endpointRouteBuilder,
        string pattern, string namedConfiguration)
        where T : StripeWebhookHandler
    {
        if (pattern == null)
            throw new ArgumentNullException(nameof(pattern));

        if (namedConfiguration == null)
            throw new ArgumentNullException(nameof(namedConfiguration));

        endpointRouteBuilder.MapPost(pattern, async (HttpContext context) =>
        {
            var handlerFactory = ActivatorUtilities.CreateFactory(typeof(T), [typeof(StripeWebhookContext)]);
            var stripeClient = context.RequestServices.GetRequiredKeyedService<StripeClient>(namedConfiguration);
            var options = context.RequestServices.GetRequiredService<IOptionsSnapshot<StripeOptions>>()
                .Get(namedConfiguration);

            var stripeWebhookContext = new StripeWebhookContext(context, options, stripeClient);
            var handler = (T)handlerFactory(context.RequestServices, [stripeWebhookContext]);
            var result = await handler.ExecuteAsync();
            return result;
        });

        return endpointRouteBuilder;
    }
}