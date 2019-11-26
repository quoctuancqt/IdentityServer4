namespace IdentityServer.Dtos.Base
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PageResultDto<T>
    {
        public PageResultDto()
        {
            Items = new List<T>();
        }

        public PageResultDto(IEnumerable<T> items)
        {
            Items = items;
            TotalPage = 1;
            TotalRecord = items.Count();
        }

        public IEnumerable<T> Items { get; set; }

        public long TotalRecord { get; set; }

        public int TotalPage { get; set; }


        public PageResultDto(long totalRecord, int take, IEnumerable<T> items)
        {
            TotalRecord = totalRecord;
            TotalPage = (int)Math.Ceiling((double)((double)totalRecord / (double)take));
            Items = items;
        }

    }
}
