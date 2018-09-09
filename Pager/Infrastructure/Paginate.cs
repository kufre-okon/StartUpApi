using Pager.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections;

namespace Pager.Infrastructure
{
    public class Paginate<T> : IPaginate<T>
    {
        public int PageCount
        {
            get;
            private set;
        }

        public int TotalItemCount
        {
            get;
            private set;
        }

        private int PageIndex
        {
            get;
            set;
        }

        public int PageNumber
        {
            get
            {
                return PageIndex + 1;
            }
        }

        public int PageSize
        {
            get;
            private set;
        }

        public bool HasPreviousPage
        {
            get;
            private set;
        }

        public bool HasNextPage
        {
            get;
            private set;
        }

        public bool IsFirstPage
        {
            get;
            private set;
        }

        public bool IsLastPage
        {
            get;
            private set;
        }

        public int ItemStart
        {
            get;
            private set;
        }

        public int ItemEnd
        {
            get;
            private set;
        }

        public IList<T> Items
        {
            get;
            set;
        }

        internal Paginate() => Items = new T[0];

        internal Paginate(IEnumerable<T> source, int index, int pageSize)
        {
            var enumerable = source as T[] ?? source.ToArray();

            if (index != 0)
                index--;
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("PageIndex cannot be below 0.");
            }
            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException("PageSize cannot be less than 1.");
            }
            PageSize = pageSize;
            PageIndex = index;
            if (source is IQueryable<T> querable)
            {
                TotalItemCount = querable.Count();
               
                Items = querable.Skip((index) * pageSize).Take(pageSize).ToList();
            }
            else
            {
                TotalItemCount = enumerable.Count();
                Items = enumerable.Skip((index) * pageSize).Take(pageSize).ToList();
            }

            PageCount = (int)Math.Ceiling(TotalItemCount / (double)PageSize);

            HasPreviousPage = (PageIndex > 0);
            HasNextPage = (PageIndex < PageCount - 1);
            IsFirstPage = (PageIndex <= 0);
            IsLastPage = (PageIndex >= PageCount - 1);
            ItemStart = PageIndex * PageSize + 1;
            ItemEnd = Math.Min(PageIndex * PageSize + PageSize, TotalItemCount);
        }
    }

    internal class Paginate<TSource, TResult> : IPaginate<TResult>
    {

        public int PageCount
        {
            get;
            private set;
        }

        public int TotalItemCount
        {
            get;
            private set;
        }

        private int PageIndex
        {
            get;
            set;
        }

        public int PageNumber
        {
            get
            {
                return PageIndex + 1;
            }
        }

        public int PageSize
        {
            get;
            private set;
        }

        public bool HasPreviousPage
        {
            get;
            private set;
        }

        public bool HasNextPage
        {
            get;
            private set;
        }

        public bool IsFirstPage
        {
            get;
            private set;
        }

        public bool IsLastPage
        {
            get;
            private set;
        }

        public int ItemStart
        {
            get;
            private set;
        }

        public int ItemEnd
        {
            get;
            private set;
        }

        public IList<TResult> Items
        {
            get;
            set;
        }

        public Paginate(IEnumerable<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter, int index, int pageSize)
        {
            var enumerable = source as TSource[] ?? source.ToArray();

            if (index != 0)
                index--;
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("PageIndex cannot be below 0.");
            }
            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException("PageSize cannot be less than 1.");
            }
            PageSize = pageSize;
            PageIndex = index;
            if (source is IQueryable<TSource> querable)
            {
                TotalItemCount = querable.Count();
                var items = querable.Skip((index) * pageSize).Take(pageSize).ToArray();
                Items = new List<TResult>(converter(items));
            }
            else
            {
                TotalItemCount = enumerable.Count();
                var items = enumerable.Skip((index) * pageSize).Take(pageSize).ToArray();
                Items = new List<TResult>(converter(items));
            }

            PageCount = (int)Math.Ceiling(TotalItemCount / (double)PageSize);

            HasPreviousPage = (PageIndex > 0);
            HasNextPage = (PageIndex < PageCount - 1);
            IsFirstPage = (PageIndex <= 0);
            IsLastPage = (PageIndex >= PageCount - 1);
            ItemStart = PageIndex * PageSize + 1;
            ItemEnd = Math.Min(PageIndex * PageSize + PageSize, TotalItemCount);
        }

        public Paginate(IPaginate<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter)
        {
            PageIndex = source.PageNumber - 1; // Get PageNumber = PageIndex +1
            PageSize = source.PageSize;
            IsFirstPage = source.IsFirstPage;
            HasPreviousPage = source.HasPreviousPage;
            TotalItemCount = source.TotalItemCount;
            PageCount = source.PageCount;
            HasNextPage = source.HasNextPage;
            IsLastPage = source.IsLastPage;
            ItemStart = source.ItemStart;
            ItemEnd = source.ItemEnd;

            Items = new List<TResult>(converter(source.Items));
        }
    }

    public static class Paginate
    {

        public static IPaginate<T> Empty<T>() => new Paginate<T>();

        public static IPaginate<TResult> From<TResult, TSource>(IPaginate<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter) => new Paginate<TSource, TResult>(source, converter);
    }
}
