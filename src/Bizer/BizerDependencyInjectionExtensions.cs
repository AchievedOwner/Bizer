﻿using Bizer;
using Bizer.Security;

using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;
public static class BizerDependencyInjectionExtensions
{
    /// <summary>
    /// 添加 Bizer 服务并配置自动发现服务。
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configure">一个进行自动发现的配置委托。</param>
    /// <returns></returns>
    public static BizerBuilder AddBizer(this IServiceCollection services, Action<AutoDiscoveryOptions>? configure = default)
    {
        var builder = new BizerBuilder(services);

            builder.AddAutoDiscovery(configure);
        return builder;
    }
    /// <summary>
    /// 添加当前线程作为主体的访问器。
    /// </summary>
    /// <param name="builder"></param>
    public static BizerBuilder AddThreadCurrentPrincipalAccessor(this BizerBuilder builder)
        => builder.AddCurrentPrincipalAccessor<ThreadCurrentPrincipalAccessor>();

    /// <summary>
    /// 构造 Bizer 框架。
    /// </summary>
    /// <param name="host"></param>
    /// <returns></returns>
    public static IHost WithBizer(this IHost host)
    {
        ApplicationContext.SetApplicationService(host.Services);
        return host;
    }
}