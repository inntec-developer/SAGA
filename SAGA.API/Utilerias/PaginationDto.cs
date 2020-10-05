using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Utilerias
{
    public class PaginationDto
    {
        public int totalCount { get; set; }
        public int pageSize { get; set; }
        public int currentPage { get; set; }
        public int totalPages { get; set; }
    }
    public class QueryParameters
    {
        private const int maxPageCount = 50;
        public int Page { get; set; } = 1;
        public Guid Id { get; set; }

        private int _pageCount = maxPageCount;
        public int PageCount
        {
            get { return _pageCount; }
            set { _pageCount = (value > maxPageCount) ? maxPageCount : value; }
        }

        public string Query { get; set; }

        public string OrderBy { get; set; } = "Fecha";

    }
}