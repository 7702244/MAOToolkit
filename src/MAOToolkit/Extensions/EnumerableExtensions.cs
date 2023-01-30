namespace MAOToolkit.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Performs the specified action on each element of the System.Collections.Generic.IEnumerable.
        /// </summary>
        public static IEnumerable<T> ForEach<T>(
            this IEnumerable<T> source,
            Action<T> act)
        {
            foreach (T element in source)
            {
                act(element);
            }
            return source;
        }
    }
}