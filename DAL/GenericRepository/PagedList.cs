using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Pagination
{
    public class PagedList<T> where T : class
    {
        public async static Task<IEnumerable<T>> CreatePagedList(IQueryable<T> source, int pageSize, int pageNumber)
        {
            return await Task.FromResult(source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList());
        }
    }

    public class PagedListResult<T> where T : class
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalRows { get; set; }
        public int Pages { get { return GetPagesCount(PageSize, TotalRows); } }
        public bool HasFirst { get { return CurrentPage > 1 && Pages > 1; } }
        public bool HasLast { get { return CurrentPage < Pages; } }
        public IEnumerable<T> Result { get; set; }

        private int GetPagesCount(int pageSize, int TotalRows)
        {
            try
            {
                int result = (TotalRows + PageSize - 1) / PageSize;

                return result;
            }
            catch
            {
                return 0;
            }
        }
    }

    public class PagedListInfo
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public PagedListInfo()
        {
            this.PageNumber = 1;
            this.PageSize = 10;
        }
        public PagedListInfo(int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber < 1 ? 1 : pageNumber;
            this.PageSize = pageSize > 250 ? 250 : pageSize;
        }
    }
}
