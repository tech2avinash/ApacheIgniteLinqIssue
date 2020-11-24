using Apache.Ignite.Core.Cache.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApacheIgniteLinqIssue.Models
{
    public class MeetingAttachment
    {
        [QuerySqlField]
        public long AttachmentId { get; set; }
        [QuerySqlField]
        public string AttachmentName { get; set; }
        [QuerySqlField]
        public DateTime AttachmentDate { get; set; }

        [QuerySqlField]
        public long MeetingId { get; set; }
    }
}
