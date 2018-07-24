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
    public class PagedList<T> : IPagedList<T>
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
                return this.PageIndex + 1;
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

        public List<T> Items
        {
            get;
            set;
        }

        /// <summary>
        /// Manages pagination for list of objects.
        /// 
        /// </summary>       
        /// <param name="totalCount">If not specified, 'source.Count' will be used </param>

        public PagedList(IEnumerable<T> source, int index, int pageSize, int? totalCount= null)
        {
            this.Initialize(source.AsQueryable<T>(), index, pageSize, totalCount);
        }

        /// <summary>
        /// Manages pagination for list of objects
        /// </summary>        
        public PagedList(IQueryable<T> source, int index, int pageSize)
            : this(source, index, pageSize, null)
        {
        }

        /// <summary>
        ///  Manages pagination for list of objects.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount">If not specified, 'source.Count' will be used </param>
        public PagedList(IQueryable<T> source, int index, int pageSize, int? totalCount= null)
        {
            this.Initialize(source, index, pageSize, totalCount);
        }

        protected void Initialize(IQueryable<T> source, int index, int pageSize, int? totalCount)
        {
            Items = new List<T>();
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
            if (source == null)
            {
                source = new List<T>().AsQueryable<T>();
            }

            this.TotalItemCount = totalCount ?? source.Count<T>();
            
            this.PageSize = pageSize;
            this.PageIndex = index;
            if (this.TotalItemCount > 0)
            {
                this.PageCount = (int)Math.Ceiling(this.TotalItemCount / (double)this.PageSize);
            }
            else
            {
                this.PageCount = 0;
            }
            this.HasPreviousPage = (this.PageIndex > 0);
            this.HasNextPage = (this.PageIndex < this.PageCount - 1);
            this.IsFirstPage = (this.PageIndex <= 0);
            this.IsLastPage = (this.PageIndex >= this.PageCount - 1);
            this.ItemStart = this.PageIndex * this.PageSize + 1;
            this.ItemEnd = Math.Min(this.PageIndex * this.PageSize + this.PageSize, this.TotalItemCount);
            if (this.TotalItemCount > 0)
            {
                var s = source.Skip(index * pageSize).Take(pageSize);
                Items.AddRange(s.ToList<T>());
            }
        }
    }
}
