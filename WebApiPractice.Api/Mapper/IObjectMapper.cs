using System.Collections.Generic;

namespace WebApiPractice.Api.Mapper
{
    public interface IObjectMapper
    {
        IEnumerable<TDest> Map<TSource, TDest>(IEnumerable<TSource> sourceCollection);
        TDest Map<TSource, TDest>(TSource item);
    }
}