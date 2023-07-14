﻿using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Bizer.Client;
internal class DynamicHttpInterceptor<TService> : IAsyncInterceptor where TService : class
{
    public DynamicHttpInterceptor(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public IServiceProvider ServiceProvider { get; }

    public IHttpClientFactory HttpClientFactory { get; }

    protected ILoggerFactory? LoggerFactory => ServiceProvider.GetService<ILoggerFactory>();

    public ILogger? Logger => LoggerFactory?.CreateLogger("DynamicHttpProxy");

    protected IHttpRemotingResolver Converter => ServiceProvider.GetRequiredService<IHttpRemotingResolver>();

    DynamicHttpClientProxy<TService> DynamicHttpClientProxy => (DynamicHttpClientProxy<TService>)ServiceProvider.GetRequiredService(typeof(DynamicHttpClientProxy<>).MakeGenericType(typeof(TService)));

    public void InterceptAsynchronous(IInvocation invocation)
    {
        throw new NotSupportedException($"要求方法 {invocation.Method.Name} 必须具有返回类型，不能是 void 或 Task");
    }

    public void InterceptAsynchronous<TResult>(IInvocation invocation)
    {
        var result= DynamicHttpClientProxy.SendAsync<TResult>(CreateRequestMessage(invocation));
        invocation.ReturnValue = result;
    }

    public void InterceptSynchronous(IInvocation invocation)
    {
        throw new NotSupportedException($"不支持同步方法，请改用异步方法");
    }


    HttpRequestMessage CreateRequestMessage(IInvocation invocation)
    {
        if ( invocation is null )
        {
            throw new ArgumentNullException(nameof(invocation));
        }

        var request = new HttpRequestMessage
        {
            Method = Converter.GetHttpMethod(invocation.Method)
        };

        var pathBuilder = new StringBuilder();
        var apiRoute = Converter.GetApiRoute(typeof(TService), invocation.Method); 

        if ( !apiRoute.StartsWith("/") )
        {
            pathBuilder.Append('/');
        }
        pathBuilder.Append(apiRoute);

        var queryParameters = new List<string>();
        var parameters = Converter.GetParameters(invocation.Method);
        var key = DefaultHttpRemotingResolver.GetMethodCacheKey(invocation.Method);
        var parameterInfoList = parameters[key];

        foreach ( var param in parameterInfoList )
        {            
            var name = param.GetParameterNameInHttpRequest();
            var value = param.Value?.ToString() ?? invocation.GetArgumentValue(param.Position)?.ToString();

            switch ( param.Type )
            {
                case HttpParameterType.FromBody:
                    var json = JsonSerializer.Serialize(value);
                    request.Content = new StringContent(json, Encoding.Default, "application/json");
                    break;
                case HttpParameterType.FromHeader:
                    request.Headers.Add(name, value);
                    break;
                case HttpParameterType.FromForm:
                    break;
                case HttpParameterType.FromPath://路由替换
                    var match = Regex.Match(pathBuilder.ToString(), @"{\w+}");
                    if ( match.Success )
                    {
                        pathBuilder.Replace(match.Value, match.Result(value));
                    }
                    break;
                case HttpParameterType.FromQuery:
                    if (param.ValueType!=typeof(string) && param.ValueType.IsClass )
                    {
                        foreach ( var property in param.ValueType.GetProperties() )
                        {
                            if ( !property.CanRead )
                            {
                                continue;
                            }

                            if ( property.TryGetCustomAttribute<JsonPropertyNameAttribute>(out var jsonNameProperty) )
                            {
                                name = jsonNameProperty!.Name;
                            }
                            else
                            {
                                name = property.Name;
                            }

                            var propertyValue = property.GetValue(value);
                            if ( propertyValue is not null )
                            {
                                queryParameters.Add($"{name}={propertyValue}");
                            }
                        }
                    }
                    else
                    {
                        queryParameters.Add($"{name}={value}");
                    }
                    break;
                default:
                    break;
            }
        }

        var uriString = $"{pathBuilder}{(queryParameters.Count > 0 ? $"?{string.Join("&", queryParameters)}" : String.Empty)}";


        request.RequestUri = new(uriString, UriKind.Relative);
        Logger?.LogDebug($"请求的 uri 资源路径：{uriString}");
        return request;
    }


}
