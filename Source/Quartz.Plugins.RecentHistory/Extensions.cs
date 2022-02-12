
using System.Diagnostics.CodeAnalysis;

namespace Quartz.Plugins.RecentHistory;

public static class Extensions
{
    public static void SetExecutionHistoryStore([NotNull, DisallowNull]this SchedulerContext context, IExecutionHistoryStore store)
    {
        context.Put(typeof(IExecutionHistoryStore).FullName, store);
    }

    public static IExecutionHistoryStore GetExecutionHistoryStore([NotNull, DisallowNull]this SchedulerContext context)
    {
        return (IExecutionHistoryStore)context.Get(typeof(IExecutionHistoryStore).FullName);
    }
}