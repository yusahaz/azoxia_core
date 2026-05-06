namespace Azoxia.Core.Extensions
{
    using System.Collections;
    using System.Linq;

    /// <summary>
    /// Provides extension methods for collections.
    /// </summary>
    public static class CollectionExtensions
    {
        #region Methods

        /// <summary>
        /// Adds a range of items to the collection.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="items">The items to add.</param>
        /// <exception cref="AzoxiaException"><paramref name="collection"/> or <paramref name="items"/> is <c>null</c>.</exception>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            collection.ThrowIfNull();
            items.ThrowIfNull();

            foreach (T item in items)
            {
                collection.Add(item);
            }
        }

        /// <summary>
        /// Returns an empty sequence when the source is <c>null</c>; otherwise returns the source.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="source">The sequence.</param>
        /// <returns>An empty sequence or <paramref name="source"/>.</returns>
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? source)
            => source ?? Enumerable.Empty<T>();

        /// <summary>
        /// Invokes <paramref name="action"/> for each element.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="source">The sequence.</param>
        /// <param name="action">The action.</param>
        /// <exception cref="AzoxiaException"><paramref name="source"/> or <paramref name="action"/> is <c>null</c>.</exception>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            source.ThrowIfNull();
            action.ThrowIfNull();

            foreach (T item in source)
            {
                action(item);
            }
        }

        /// <summary>
        /// Determines whether the sequence is <c>null</c> or contains no elements.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="source">The sequence.</param>
        /// <returns><c>true</c> if the sequence is null or empty; otherwise, <c>false</c>.</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
        {
            if (source is null)
            {
                return true;
            }

            if (source is ICollection<T> genericCollection)
            {
                return genericCollection.Count == 0;
            }

            if (source is ICollection nonGenericCollection)
            {
                return nonGenericCollection.Count == 0;
            }

            using IEnumerator<T> e = source.GetEnumerator();
            return !e.MoveNext();
        }

        /// <summary>
        /// Concatenates the string sequence using <paramref name="separator"/>. A <c>null</c> sequence is treated as empty.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>The joined string, or <see cref="string.Empty"/> when there are no values.</returns>
        public static string Join(this IEnumerable<string?>? values, char separator)
            => string.Join(separator, values.EmptyIfNull());

        /// <summary>
        /// Concatenates the string sequence using <paramref name="separator"/>. A <c>null</c> sequence is treated as empty.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>The joined string, or <see cref="string.Empty"/> when there are no values.</returns>
        public static string Join(this IEnumerable<string?>? values, string? separator)
            => string.Join(separator, values.EmptyIfNull());

        /// <summary>
        /// Concatenates the string sequence using <see cref="Environment.NewLine"/>. A <c>null</c> sequence is treated as empty.
        /// </summary>
        /// <param name="lines">The lines.</param>
        /// <returns>The joined string.</returns>
        public static string JoinLines(this IEnumerable<string?>? lines)
            => Join(lines, Environment.NewLine);

        /// <summary>
        /// Concatenates non-null, non-white-space entries using <paramref name="separator"/>. A <c>null</c> sequence is treated as empty.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>The joined string, or <see cref="string.Empty"/> when no usable values exist.</returns>
        public static string JoinNonWhiteSpace(this IEnumerable<string?>? values, char separator)
            => string.Join(separator, values.EmptyIfNull().Where(v => !v.IsNullOrWhiteSpace()));

        /// <summary>
        /// Concatenates non-null, non-white-space entries using <paramref name="separator"/>. A <c>null</c> sequence is treated as empty.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>The joined string, or <see cref="string.Empty"/> when no usable values exist.</returns>
        public static string JoinNonWhiteSpace(this IEnumerable<string?>? values, string? separator)
            => string.Join(separator, values.EmptyIfNull().Where(v => !v.IsNullOrWhiteSpace()));

        #endregion Methods
    }
}
