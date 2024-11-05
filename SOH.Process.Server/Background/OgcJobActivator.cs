using Hangfire;
using Hangfire.Server;

namespace SOH.Process.Server.Background;

public class OgcJobActivator : JobActivator
{
    private readonly IServiceScopeFactory _scopeFactory;

    public OgcJobActivator(IServiceScopeFactory scopeFactory)
    {
        ArgumentNullException.ThrowIfNull(scopeFactory);
        _scopeFactory = scopeFactory;
    }

    public override JobActivatorScope BeginScope(PerformContext context)
    {
        return new Scope(context, _scopeFactory.CreateScope());
    }

    private sealed class Scope : JobActivatorScope, IServiceProvider
    {
        private readonly PerformContext _context;
        private readonly IServiceScope _scope;

        public Scope(PerformContext context, IServiceScope scope)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(scope);

            _context = context;
            _scope = scope;
        }

        object? IServiceProvider.GetService(Type serviceType)
        {
            return serviceType == typeof(PerformContext) ? _context : _scope.ServiceProvider.GetService(serviceType);
        }

        public override object Resolve(Type type)
        {
            return ActivatorUtilities.GetServiceOrCreateInstance(this, type);
        }
    }
}