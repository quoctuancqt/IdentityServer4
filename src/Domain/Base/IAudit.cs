using System;

namespace Domain.Base
{
    public interface IAudit
    {
        string CreatedBy { get; set; }

        DateTime? CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

    }
}
