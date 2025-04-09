using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Cloud5mins.ShortenerTools.Core.Domain
{
    public class ShortUrlEntity : TableEntity
    {
        public string Url { get; set; }
        private string _activeUrl { get; set; }

        public string ActiveUrl
        {
            get
            {
                if (String.IsNullOrEmpty(_activeUrl))
                    _activeUrl = GetActiveUrl();
                return _activeUrl;
            }
        }


        public string Title { get; set; } = "";

        public string ShortUrl { get; set; } 

        public string CreatedBy { get; set; } = ""; // added 3/28/2025 by Yuping

        public int Clicks { get; set; }

        public bool? IsArchived { get; set; }
        public string SchedulesPropertyRaw { get; set; }

        private List<Schedule> _schedules { get; set; }

        [IgnoreProperty]
        public List<Schedule> Schedules
        {
            get
            {
                if (_schedules == null)
                {
                    if (String.IsNullOrEmpty(SchedulesPropertyRaw))
                    {
                        _schedules = new List<Schedule>();
                    }
                    else
                    {
                        _schedules = JsonSerializer.Deserialize<Schedule[]>(SchedulesPropertyRaw).ToList<Schedule>();
                    }
                }
                return _schedules;
            }
            set
            {
                _schedules = value;
            }
        }

        public ShortUrlEntity() { }

        public ShortUrlEntity(string longUrl, string endUrl)
        {
            Initialize(longUrl, endUrl, string.Empty, string.Empty, null);
        }

        public ShortUrlEntity(string longUrl, string endUrl, Schedule[] schedules)
        {
            Initialize(longUrl, endUrl, string.Empty, string.Empty, schedules);
        }

        public ShortUrlEntity(string longUrl, string endUrl, string title, Schedule[] schedules)
        {
            Initialize(longUrl, endUrl, title, string.Empty, schedules);
        }

        //added 3/28/2025 by Yuping
        public ShortUrlEntity(string longUrl, string endUrl, string title, string createdBy,  Schedule[] schedules) 
        {
            Initialize(longUrl, endUrl, title, createdBy, schedules);
        }

        private void Initialize(string longUrl, string endUrl, string title, string createdBy, Schedule[] schedules) ///added user 3/28/2025 by Yuping
        {
            PartitionKey = endUrl.First().ToString();
            RowKey = endUrl;
            Url = longUrl;
            Title = title;
            CreatedBy = createdBy; //added by Yuiping
            Clicks = 0;
            IsArchived = false;

            if(schedules?.Length>0)
            {
                Schedules = schedules.ToList<Schedule>();
                SchedulesPropertyRaw = JsonSerializer.Serialize<List<Schedule>>(Schedules);
            }
        }

        //Added CreatedBy by Yuping 4/3/2025
        public static ShortUrlEntity GetEntity(string longUrl, string endUrl, string title, string createdBy, Schedule[] schedules)
        {
            return new ShortUrlEntity
            {
                PartitionKey = endUrl.First().ToString(),
                RowKey = endUrl,
                Url = longUrl,
                Title = title,
                CreatedBy = createdBy, //Added by Yuping 4/3/2025
                Schedules = schedules.ToList<Schedule>()
            };
        }

        private string GetActiveUrl()
        {
            if (Schedules != null)
                return GetActiveUrl(DateTime.UtcNow);
            return Url;
        }
        private string GetActiveUrl(DateTime pointInTime)
        {
            var link = Url;
            var active = Schedules.Where(s =>
                s.End > pointInTime && //hasn't ended
                s.Start < pointInTime //already started
                ).OrderBy(s => s.Start); //order by start to process first link

            foreach (var sched in active.ToArray())
            {
                if (sched.IsActive(pointInTime))
                {
                    link = sched.AlternativeUrl;
                    break;
                }
            }
            return link;
        }
    }

}