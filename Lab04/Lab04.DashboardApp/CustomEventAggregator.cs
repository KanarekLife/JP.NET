using System.Composition;

namespace Lab04.DashboardApp;

[Export(typeof(IEventAggregator))]
[Shared]
public class CustomEventAggregator : EventAggregator;
