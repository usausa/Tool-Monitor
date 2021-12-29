namespace WorkPipeline;

using System.Diagnostics;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

public static class Program
{
    public static async Task Main()
    {
        var services = new ServiceCollection();
        services.AddSingleton<Logger>();

        var provider = services.BuildServiceProvider();

        var builder = new PipelineBuilder(provider);
        builder.Use<TimeLoggingMiddleware>();
        builder.Use<AddParameterMiddleware>();

        var app = builder.Build();

        var context = new PipelineContext();
        await app(context);

        Debug.WriteLine(context.Items.Count);
    }
}

public sealed class PipelineContext
{
    public Dictionary<string, object> Items { get; } = new();

    public object? Result { get; set; }
}

//--------------------------------------------------------------------------------

public class Logger
{
    public void Log(string message) => Debug.WriteLine(message);
}

public class TimeLoggingMiddleware
{
    private readonly PipelineDelegate next;

    private readonly Logger logger;

    public TimeLoggingMiddleware(PipelineDelegate next, Logger logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task Execute(PipelineContext context)
    {
        var start = DateTime.Now;

        await next(context).ConfigureAwait(false);

        logger.Log($"Time: {(DateTime.Now - start).TotalMilliseconds}ms");
    }
}

public class AddParameterMiddleware
{
    private readonly PipelineDelegate next;

    public AddParameterMiddleware(PipelineDelegate next)
    {
        this.next = next;
    }

    public async Task Execute(PipelineContext context)
    {
        context.Items.Add("Parameter", new object());

        await next(context).ConfigureAwait(false);
    }
}

//--------------------------------------------------------------------------------

public delegate Task PipelineDelegate(PipelineContext context);

public sealed class PipelineBuilder
{
    private readonly IList<Func<PipelineDelegate, PipelineDelegate>> components = new List<Func<PipelineDelegate, PipelineDelegate>>();

    public IServiceProvider Services { get; }

    public PipelineBuilder(IServiceProvider services)
    {
        Services = services;
    }

    public PipelineBuilder Use(Func<PipelineDelegate, PipelineDelegate> middleware)
    {
        components.Add(middleware);
        return this;
    }

    public PipelineBuilder Use<T>()
    {
        return Use(next =>
        {
            var type = typeof(T);

            var ctor = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .OrderByDescending(x => x.GetParameters().Length)
                .FirstOrDefault(MatchConstructor);
            if (ctor is null)
            {
                throw new InvalidOperationException("Invalid middleware");
            }

            var method = type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(MatchMethod);
            if (method is null)
            {
                throw new InvalidOperationException("Invalid middleware");
            }

            var parameters = ctor.GetParameters();
            var args = new object?[parameters.Length];
            args[0] = next;
            for (var i = 1; i < args.Length; i++)
            {
                args[i] = Services.GetService(parameters[i].ParameterType);
            }

            var instance = Activator.CreateInstance(type, args);

            return (PipelineDelegate)method.CreateDelegate(typeof(PipelineDelegate), instance);
        });
    }

    private static bool MatchConstructor(ConstructorInfo ci)
    {
        return (ci.GetParameters().Length > 0) &&
               (ci.GetParameters()[0].ParameterType == typeof(PipelineDelegate));
    }

    private static bool MatchMethod(MethodInfo mi)
    {
        return (mi.Name == "Execute") &&
               typeof(Task).IsAssignableFrom(mi.ReturnType) &&
               (mi.GetParameters().Length == 1) &&
               (mi.GetParameters()[0].ParameterType == typeof(PipelineContext));
    }

    public PipelineDelegate Build()
    {
        PipelineDelegate app = _ => Task.CompletedTask;

        foreach (var component in components.Reverse())
        {
            app = component(app);
        }

        return app;
    }
}
