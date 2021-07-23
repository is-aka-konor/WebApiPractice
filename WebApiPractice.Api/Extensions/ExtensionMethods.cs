using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiPractice.Api.Extensions
{
    /// <summary>
    /// Extensions methods for Linq
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Extension method to get list of items with a limit and invoke the action with the limit+1 item
        /// </summary>
        /// <typeparam name="TSource">IQuerable source of data</typeparam>
        /// <param name="source">The source data to query from</param>
        /// <param name="limit">Number of records to take from source</param>
        /// <param name="getNextCursor">The action to work out the next cursor</param>
        /// <param name="cancellationToken">Cancellation token for the async call</param>
        /// <returns>Returns a list within a specified limit</returns>
        public static async Task<List<TSource>> ToListAsync<TSource>(this IQueryable<TSource> source,
            int limit,
            Action<TSource> getNextCursor,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if(source is null || limit <0) return new List<TSource>();

            // Tale limit and extra 1 record to check if there are more to take and move the next cursor
            var result = await source.Take(limit + 1).ToListAsync(cancellationToken);
            if(result.Count > limit)
            {
                getNextCursor(result[limit]);
                // Remove the additional item
                result.RemoveAt(limit);
            }

            return result;
        }

        /// <summary>
        /// Encodes an integer value to a Base64 string value
        /// </summary>
        /// <param name="value">The integer to encode</param>
        /// <returns>Returns the Base64 string value</returns>
        public static string Base64Encode(this int value)
        {
            var stringBytes = System.Text.Encoding.UTF8.GetBytes(value.ToString());
            return Convert.ToBase64String(stringBytes);
        }

        /// <summary>
        /// Decode a Base64 encoded string to the integer value
        /// </summary>
        /// <param name="base64EncodedString">The Base64 encoded string</param>
        /// <returns>Returns the integer value of the Base64 encoded string</returns>
        public static int Base64DecodeInt(this string base64EncodedString)
        {
            if(string.IsNullOrWhiteSpace(base64EncodedString)) return 0;

            try
            {
                var encodedBytes = Convert.FromBase64String(base64EncodedString);
                return int.TryParse(System.Text.Encoding.UTF8.GetString(encodedBytes), out var value) ? value : 0;
            }catch
            {
                return 0;
            }
        }
    }
}
