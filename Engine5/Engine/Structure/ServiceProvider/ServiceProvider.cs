using System.Collections.Concurrent;
using System.Reflection;
using Engine.Structure.Attributes;

namespace Engine.Structure.ServiceProvider;

public class ServiceProvider : Identifiable
{

    //Service provider should not dispose, update or initialize services. Those should be external managers, using the "ServiceAdded" and "ServiceRemoved" event

    //Service Providers should load new types, but not allow "adding". Peeking should be allowed.

    private readonly ConcurrentDictionary<Type, WeakReference> _constants;
    private readonly ConcurrentDictionary<Type, object> _services;

    public delegate void ServiceEventHandler(object service);

    public event ServiceEventHandler? ServiceAdded;

    public ServiceProvider()
    {
        _constants = new();
        _services = new();
    }

    protected virtual bool CanLoad(Type t) => true;

    public void AddConstant(object o) => _constants.TryAdd(o.GetType(), new(o));

    private object? Load(Type t)
    {
        if (!CanLoad(t))
        {
            this.LogWarning($"Cannot load {t.FullName}!");
            return null;
        }

        if (t.IsAbstract || t.IsInterface)
            throw new Exception($"{t.FullName} is not implemented and cannot be loaded.");

        ConstructorInfo[]? ctors = t.GetConstructors();
        if (ctors.Length == 0)
            throw new InvalidOperationException("Type must have a valid constructor");

        ConstructorInfo? ctor = ctors[0];
        if (ctors.Length > 1)
            this.LogLine($"Found multiple constructors for type {t.FullName}. Using {t.Name}({string.Join(", ", ctor.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"))})!", Log.Level.CRITICAL);

        Type[] parameterTypes = ctor.GetParameters().Select(p => p.ParameterType).ToArray();

        if (parameterTypes.Any(p => p == t))
            throw new Exception($"Service {t.FullName} can't depend on itself!");

        Type selfType = GetType();
        object?[]? parameters = parameterTypes.Select(p => p == selfType ? this : _constants.TryGetValue(p, out var o) ? o.Target : Get(p)).ToArray();

        if (parameters.Any(p => p is null))
            throw new Exception($"Unable to load all dependencies for {t.FullName}!");
        var service = ctor.Invoke(parameters);

        foreach (Type requiredServiceType in t.GetCustomAttributes<RequireAttribute>().Select(p => p.RequiredType))
            Get(requiredServiceType);

        if (_services.TryAdd(t, service))
        {
            ServiceAdded?.Invoke(service);
            return service;
        }

        if (!_services.TryGetValue(t, out service))
            throw new Exception("Service does not exist, but should.");
        return service;
    }

    public object? Get(Type t)
    {
        if (_services.TryGetValue(t, out var service))
            return service;
        return Load(t);
    }

    public T Get<T>() => Get(typeof(T)) is T t ? t : throw new InvalidOperationException("Unable to properly load service.");

    public object? Peek(Type t)
    {
        if (_services.TryGetValue(t, out var service))
            return service;
        return null;
    }

    public T? Peek<T>() => Peek(typeof(T)) is T t ? t : default;

}
