using System;
using System.Collections.Generic;

namespace WebMVC.Models.Pagination
{
    public class PagingInfo<TEntity> where TEntity : class
    {
        public long TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages => (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);
        public IEnumerable<TEntity> Items { get; set; }

        public bool CanNext() => CurrentPage < TotalPages;
        public bool CanPrevious() => CurrentPage == 1;
    }
}