using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Client.Cache;
using Apache.Ignite.Linq;
using ApacheIgniteLinqIssue.Models;
using System;
using System.Linq;

namespace ApacheIgniteLinqIssue
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var ignite = Ignition.Start())
            {
                using (var igniteClient = Ignition.StartClient(new Apache.Ignite.Core.Client.IgniteClientConfiguration
                {
                    Endpoints = new[] { "127.0.0.1" }
                }))
                {
                    var meetingCache = igniteClient.GetOrCreateCache<long, Meeting>(new CacheClientConfiguration
                    {
                        Name = typeof(Meeting).Name,
                        SqlSchema = "USERSCHEMA",
                        QueryEntities = new[] { new QueryEntity(typeof(long), typeof(Meeting)) }
                    }).AsCacheQueryable(new QueryOptions { EnableDistributedJoins = true }); ;
                    var meetingAttachmentCache = igniteClient.GetOrCreateCache<long, MeetingAttachment>(new CacheClientConfiguration
                    {
                        Name = typeof(MeetingAttachment).Name,
                        SqlSchema = "USERSCHEMA",
                        QueryEntities = new[] { new QueryEntity(typeof(long), typeof(MeetingAttachment)) }
                    }).AsCacheQueryable(new QueryOptions { EnableDistributedJoins = true });

                    var query = (from meeting in meetingCache
                                 join meetingAttachment in meetingAttachmentCache on meeting.Value.MeetingId equals meetingAttachment.Value.MeetingId
                                 group new { meeting, meetingAttachment } by meeting.Value.MeetingId into g
                                 select new
                                 {
                                     g.Key,
                                     attachmentDate = g.Max(x => x.meetingAttachment.Value.AttachmentDate)
                                 }).ToCacheQueryable().GetFieldsQuery().Sql;
                    // as a test case in platform tests
                    var query1 = (meetingCache.Join(meetingAttachmentCache,
                                 meeting => meeting.Value.MeetingId,
                                 meetingAttachment => meetingAttachment.Value.MeetingId,
                                 (meeting, meetingAttachment) => new { meeting, meetingAttachment })
                                 .GroupBy(x => x.meeting.Value.MeetingId)
                                 .Select(g => new
                                 {
                                     g.Key,
                                     attachmentDate = g.Max(x => x.meetingAttachment.Value.AttachmentDate)
                                 })).ToCacheQueryable().GetFieldsQuery().Sql;
                }
            }
        }
    }
}
