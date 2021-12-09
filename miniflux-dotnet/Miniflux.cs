using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Miniflux
{
    public class Consts
    {
        public const string EntryStatusUnread = "unread";
        public const string EntryStatusRead = "read";
        public const string EntryStatusRemoved = "removed";
    }

    public class User
    {
        public Int64 ID { set; get; }
        public string Username { set; get; }
        public string Password { set; get; }

        [JsonProperty("is_admin")]
        public bool IsAdmin { set; get; }

        public string Theme { set; get; }
        public string Language { set; get; }
        public string Timezone { set; get; }

        [JsonProperty("entry_sorting_direction")]
        public string EntryDirection { set; get; }
        public string Stylesheet { set; get; }

        [JsonProperty("google_id")]
        public string GoogleID { set; get; }

        [JsonProperty("openid_connect_id")]
        public string OpenIDConnectID { set; get; }

        [JsonProperty("entries_per_page")]
        public Int64 EntriesPerPage { set; get; }

        [JsonProperty("keyboard_shortcuts")]
        public bool KeyboardShortcut { set; get; }

        [JsonProperty("show_reading_time")]
        public bool ShowReadingTime { set; get; }

        [JsonProperty("entry_swipe")]
        public bool EntrySwipe { set; get; }

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

        [JsonProperty("scraper_rules")]
        public string ScrapperRules { set; get; }


        [JsonProperty("rewrite_rules")]
        public string RewriteRules { set; get; }

        public bool Crawler { set; get; }
        [JsonProperty("blocklist_rules")]
        public string BlocklistRules { set; get; }

        [JsonProperty("keeplist_rules")]
        public string KeeplistRules { set; get; }

        [JsonProperty("user_agent")]
        public string UserAgent { set; get; }

        public string Username { set; get; }

        public string Password { set; get; }
        public bool Disabled { set; get; }

        [JsonProperty("ignore_http_cache")]
        public bool IgnoreHttpCache { set; get; }

        [JsonProperty("fetch_via_proxy")]
        public bool FetchViaProxy { set; get; }

        public Category Category { set; get; }
        public Icon Icon { set; get; }
        public Entry[] Entries { set; get; }

    }

    public class FeedIcon
    {
        public Int64 ID { set; get; }

        [JsonProperty("mime_type")]
        public string Mimetype { set; get; }

        public string Data { set; get; }
    }
    public class Icon
    {
        [JsonProperty("feed_id")]
        public Int64 FeedID { set; get; }
        [JsonProperty("icon_id")]
        public Int64 IconID { set; get; }
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

        [JsonProperty("comments_url")]
        public string CommentsURL { set; get; }

        [JsonProperty("published_at")]
        public DateTime Date { set; get; }

        public string Content { set; get; }
        public string Author { set; get; }
        public bool Starred { set; get; }

        [JsonProperty("reading_time")]
        public Int64 ReadingTime { set; get; }

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