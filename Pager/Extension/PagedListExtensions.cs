using Pager.Infrastructure;
using Pager.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pager.Extension
{
    #region sync methods

    public static class PagedListExtensions
    {
        public static IPagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageIndex, int pageSize)
        {
            return new PagedList<T>(source, pageIndex, pageSize);
        }

        public static IPagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageIndex, int pageSize, int? totalCount = null)
        {
            return new PagedList<T>(source, pageIndex, pageSize, totalCount);
        }

        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize, int? totalCount = null)
        {
            return new PagedList<T>(source, pageIndex, pageSize, totalCount);
        }

        #endregion

        #region async methods

        public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize)
        {
            return await Task.Run(() => new PagedList<T>(source, pageIndex, pageSize));
        }

        public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize, int? totalCount = null)
        {
            return await Task.Run(() => new PagedList<T>(source, pageIndex, pageSize, totalCount));
        }

        public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IEnumerable<T> source, int pageIndex, int pageSize, int? totalCount = null)
        {
            return await Task.Run(() => new PagedList<T>(source, pageIndex, pageSize, totalCount));
        }


        #endregion
    }
}
