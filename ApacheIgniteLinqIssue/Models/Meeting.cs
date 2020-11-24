using Apache.Ignite.Core.Cache.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApacheIgniteLinqIssue.Models
{
    public class Meeting
    {
        [QuerySqlField]
        public long MeetingId { get; set; }
        [QuerySqlField]
        public string MeetingName { get; set; }
    }
}
