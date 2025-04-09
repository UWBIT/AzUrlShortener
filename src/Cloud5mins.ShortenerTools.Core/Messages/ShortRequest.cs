using Cloud5mins.ShortenerTools.Core.Domain;

namespace Cloud5mins.ShortenerTools.Core.Messages
{
    public class ShortRequest
    {
        public string Vanity { get; set; } = "";

        public string Url { get; set; } = "";

        public string Title { get; set; } = "";

        public string CreatedBy { get; set; } = ""; //added 3/28/2025 by Yuping

        public Schedule[] Schedules { get; set; }
    }
}