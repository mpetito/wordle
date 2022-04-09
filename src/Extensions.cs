namespace Wordle;

internal static class ParallelExtensions
{
    public static ParallelQuery<TSource> WithProgressReporting<TSource, TProgress>(
        this ParallelQuery<TSource> source, int itemsCount, IProgress<TProgress> progress, Func<TSource, int, int, TProgress> report)
    {
        int countShared = 0;
        return source.Select(item =>
        {
            int countLocal = Interlocked.Increment(ref countShared);
            progress.Report(report(item, countLocal, itemsCount));
            return item;
        });
    }
}