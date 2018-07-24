using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pager.Interface
{
    public interface IPagedList<T> //: IQueryable<T>, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
    {
        List<T> Items { get; set; }

        int PageCount
        {
            get;
        }

        int TotalItemCount
        {
            get;
        }       

        int PageNumber
        {
            get;
        }

        int PageSize
        {
            get;
        }

        bool HasPreviousPage
        {
            get;
        }

        bool HasNextPage
        {
            get;
        }

        bool IsFirstPage
        {
            get;
        }

        bool IsLastPage
        {
            get;
        }

        int ItemStart
        {
            get;
        }

        int ItemEnd
        {
            get;
        }        
    }
}
