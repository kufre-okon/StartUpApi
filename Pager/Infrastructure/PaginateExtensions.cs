using Pager.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pager.Infrastructure
{
    public static class PaginateExtensions
    {
        public static IPaginate<T> ToPaginate<T>(this IEnumerable<T> source, int index, int pageSize) => new Paginate<T>(source,  index,  pageSize);
        public static IPaginate<TResult> ToPaginate<TSource, TResult>(this IEnumerable<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter, int index, int pageSize) => new Paginate<TSource, TResult>(source, converter, index,  pageSize);
    }
}
