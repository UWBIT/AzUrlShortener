namespace Cloud5mins.ShortenerTools.Core.Messages
{
    public class ShortResponse
    {
        public string ShortUrl { get; set; }
        public string LongUrl { get; set; }
        public string Title { get; set; }
        public string CreatedBy { get; set; } //added 3/28/2025 by Yuping

        public ShortResponse() { }
        public ShortResponse(string host, string longUrl, string endUrl, string title, string createdBy)
        {
            LongUrl = longUrl;
            ShortUrl = string.Concat(host, "/", endUrl);
            Title = title;
            CreatedBy = createdBy; //added 3/28/2025 by Yuping

        }
    }
}