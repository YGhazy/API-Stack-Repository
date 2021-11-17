using System;
using System.Collections.Generic;
using System.Text;

namespace Stack.DTOs
{
    public class DataItemsResult<TItem>
    {
        public DataItemsResult()
        {
            Items = new List<TItem>();
        }
        public List<TItem> Items { get; set; }
        public int ItemsPerPage { get; set; }
        public int PageNumber { get; set; }
        public int TotalItems { get; set; }
        public int PagesCount
        {
            get
            {
                if (PageNumber > 0 && ItemsPerPage > 0)
                {
                    return (int)Math.Ceiling(Convert.ToDouble(TotalItems / ItemsPerPage));
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}

