using Pager.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pager.Infrastructure
{
    public static class IQueryablePaginateExtensions
    {
        public static async Task<IPaginate<T>> ToPaginateAsync<T>(this IQueryable<T> source, int index, int pageSize,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            //var count = await source.CountAsync(cancellationToken).ConfigureAwait(false);
            // var items = await source.Skip((index) * pageSize).Take(pageSize).ToListAsync(cancellationToken).ConfigureAwait(false);

            return await Task.Run(() =>
             {
                 return source.ToPaginate(index, pageSize);
             });
            //var list = new Paginate<T>()
            //{
            //    Index = index,
            //    Size = size,
            //    From = from,
            //    Count = count,
            //    Items = items,
            //    Pages = (int)Math.Ceiling(count / (double)size)
            //};

            //return list;
        }
    }
}
