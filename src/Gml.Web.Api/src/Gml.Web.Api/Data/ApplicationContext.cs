using System.Reactive.Subjects;
using Gml.Web.Api.Domains.Settings;

namespace Gml.Web.Api.Data;

public class ApplicationContext : IDisposable
{
    private readonly IDisposable _subscription;

    public ApplicationContext(ISubject<Settings> settingsObservable)
    {
        _subscription = settingsObservable.Subscribe(s => Settings = s);
    }

    public Settings Settings { get; private set; } = new();

    public void Dispose()
    {
        _subscription.Dispose();
    }
}

