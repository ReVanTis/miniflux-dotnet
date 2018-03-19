using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Miniflux
{
    public class Consts
    {
        private const string EntryStatusUnread = "unread";
        private const string EntryStatusRead = "read";
        private const string EntryStatusRemoved = "removed";
    }

    public class User
    {
        public Int64 ID { set; get; }
        public string Username { set; get; }
        public string Password { set; get; }
        public bool IsAdmin { set; get; }
        public string Theme { set; get; }
        public string Language { set; get; }
        public string Timezone { set; get; }

        [JsonProperty("entry_sorting_direction")]
        public string EntryDirection { set; get; }

        [JsonProperty("last_login_at")]
        public DateTime LastLoginAt { set; get; }

        public Dictionary<string, string> Extra { set; get; }
    }

    public class Category
    {
        public Int64 ID { set; get; }
        public string Title { set; get; }

        [JsonProperty("user_id")]
        public Int64 UserID { set; get; }
    }

    public class Subscription
    {
        public string Title { set; get; }
        public string URL { set; get; }
        public string Type { set; get; }
    }

    public class Feed
    {
        public Int64 ID { set; get; }

        [JsonProperty("user_id")]
        public Int64 UserID { set; get; }

        [JsonProperty("feed_url")]
        public string FeedURL { set; get; }

        [JsonProperty("site_url")]
        public string SiteURL { set; get; }

        public string Title { set; get; }

        [JsonProperty("checked_at")]
        public DateTime CheckedAt { set; get; }

        [JsonProperty("etag_header")]
        public string EtagHeader { set; get; }

        [JsonProperty("last_modified_header")]
        public string LastModifiedHeader { set; get; }

        [JsonProperty("parsing_error_message")]
        public string ParsingErrorMsg { set; get; }

        [JsonProperty("parsing_error_count")]
        public int ParsingErrorCount { set; get; }

        public Category Category { set; get; }
        public Entry[] Entries { set; get; }
    }

    public class FeedIcon
    {
        public Int64 ID { set; get; }

        [JsonProperty("mime_type")]
        public string Mimetype { set; get; }

        public string Data { set; get; }
    }

    public class Entry
    {
        public Int64 ID { set; get; }

        [JsonProperty("user_id")]
        public Int64 UserID { set; get; }

        [JsonProperty("feed_id")]
        public Int64 FeedID { set; get; }

        public string Status { set; get; }
        public string Hash { set; get; }
        public string Title { set; get; }
        public string URL { set; get; }

        [JsonProperty("published_at")]
        public DateTime Date { set; get; }

        public string Content { set; get; }
        public string Author { set; get; }
        public bool Starred { set; get; }
        public Enclosure[] Enclosures { set; get; }
        public Feed Feed { set; get; }
        public Category Category { set; get; }
    }

    public class Enclosure
    {
        public Int64 ID { set; get; }

        [JsonProperty("user_id")]
        public Int64 UserID { set; get; }

        [JsonProperty("entry_id")]
        public Int64 EntryID { set; get; }

        public string URL { set; get; }

        [JsonProperty("mime_type")]
        public string MimeType { set; get; }

        public int Size { set; get; }
    }

    public class Filter
    {
        public string Status { set; get; }
        public int Offset { set; get; }
        public int Limit { set; get; }
        public string Order { set; get; }
        public string Direction { set; get; }
    }

    public class EntryResultSet
    {
        public int Total { set; get; }
        public Entry[] Entries { set; get; }
    }
}