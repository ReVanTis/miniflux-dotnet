﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Miniflux
{
    public class MinifluxClient
    {
        private const string userAgent = @"Miniflux dotNet Client Library <https://github.com/ReVanTis/miniflux-dotnet>";
        private const int defaultTimeout = 80_000;
        private static readonly KeyValuePair<string, string> ContentTypeHeader = new("Content-Type", "application/json; charset=UTF-8");
        private static readonly KeyValuePair<string, string> AcceptHeader = new("Accept", "application/json");
        public string URL { set; get; }
        public string Username { set; get; }
        public string Password { set; get; }
        public string APIToken { set; get; }
        public string Proxy { set; get; }
        public KeyValuePair<string, string> GetAuthHeader()
        {
            if (!APIToken.IsNullOrEmpty())
            {
                return new KeyValuePair<string, string>("X-Auth-Token", APIToken);
            }
            else if (!Username.IsNullOrEmpty() && !Password.IsNullOrEmpty())
            {
                return new KeyValuePair<string, string>("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(Username + ":" + Password))); ;
            }
            else throw new Exception("No auth method is valid, token and password are empty.");
        }
        public HttpWebRequest BuildRequest(string Method, string path)
        {
            if ((Username.IsNullOrEmpty() || Password.IsNullOrEmpty()) && APIToken.IsNullOrEmpty())
            {
                throw new Exception("No auth method is valid, token and password are empty.");
            }
            if (URL.IsNullOrEmpty())
            {
                throw new Exception("URL is not defined, token and password are empty.");
            }
            HttpWebRequest request = WebRequest.Create(URL + path) as HttpWebRequest;
            request.Method = Method;
            request.UserAgent = userAgent;
            request.Timeout = defaultTimeout;
            var AuthHeader = GetAuthHeader();
            request.Headers.Add(AuthHeader.Key, AuthHeader.Value);
            request.Headers.Add(ContentTypeHeader.Key, ContentTypeHeader.Value);
            request.Headers.Add(AcceptHeader.Key, AcceptHeader.Value);
            if (!Proxy.IsNullOrEmpty())
                request.Proxy = new WebProxy(Proxy);
            return request;
        }
        public string Request(string path, string method, object data)
        {
            var req = BuildRequest(method, path);
            if (data != null)
            {
                string dataStr = data.ToJson();
                var dateBytes = Encoding.UTF8.GetBytes(dataStr);

                req.ContentLength = dateBytes.Length;

                using var strm = req.GetRequestStream();
                strm.Write(dateBytes, 0, dateBytes.Length);
            }
            try
            {
                using var res = (HttpWebResponse)req.GetResponse();
                switch (res.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                    case HttpStatusCode.Forbidden:
                    case HttpStatusCode.InternalServerError:
                    case HttpStatusCode.BadRequest:
                        throw new HttpRequestException($"Code:{(int)res.StatusCode}, {res.StatusCode}");
                    default:
                        {
                            if ((int)res.StatusCode >= 400)
                            {
                                throw new HttpRequestException($"Code:{(int)res.StatusCode}, {res.StatusCode}");
                            }
                        }
                        break;
                }
                if (res.ContentType.ToLower().Contains("json") || res.ContentType.ToLower().Contains("xml") || res.ContentType.ToLower().Contains("plain"))
                {
                    using StreamReader sr = new(res.GetResponseStream());
                    return sr.ReadToEnd();
                }
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);

                using WebResponse response = e.Response;
                HttpWebResponse httpResponse = (HttpWebResponse)response;
                Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                using Stream datas = response.GetResponseStream();
                using var reader = new StreamReader(datas);
                string text = reader.ReadToEnd();
                Console.WriteLine(text);
                throw new Exception(text, e);
            }
            return null;
        }
        public string Get(string path)
        {
            return Request(path, "Get", null);
        }
        public string Post(string path, object data)
        {
            return Request(path, "Post", data);
        }
        public string Put(string path, object data)
        {
            return Request(path, "Put", data);
        }
        public string Delete(string path)
        {
            return Request(path, "Delete", null);
        }
        public List<User> GetUsers()
        {
            var json = Get("/v1/users");
            var users = json.AsType<User[]>();
            return users.ToList();
        }
        public User GetUser(Int64 ID)
        {
            var json = Get($"/v1/users/{ID}");
            var user = json.AsType<User>();
            return user;
        }
        public User GetUser(string username)
        {
            var json = Get($"/v1/users/{username}");
            var user = json.AsType<User>();
            return user;
        }
        public User CreateUser(User u)
        {
            var json = Post($"/v1/users", u);
            var user = json.AsType<User>();
            return user;
        }
        public User CreateUser(string username, string password, bool isAdmin)
        {
            User u = new()
            {
                Username = username,
                Password = password,
                IsAdmin = isAdmin,
            };
            var json = Post($"/v1/users", u);
            var user = json.AsType<User>();
            return user;
        }
        public User UpdateUser(User u)
        {
            var json = Put($"/v1/users/{u.ID}", u);
            var user = json.AsType<User>();
            return user;
        }
        public string DeleteUser(Int64 id)
        {
            return Delete($"/v1/users/{id}");
        }
        public User GetCurrentUser()
        {
            var json = Get("/v1/me");
            var user = json.AsType<User>();
            return user;
        }
        public string MarkUserEntriesAsRead(Int64 ID)
        {
            return Put($"/v1/users/${ID}/mark-all-as-read", "");
        }
        public List<Subscription> Discover(string url)
        {
            var json = Post($"/v1/discover", new { url });
            var subs = json.AsType<Subscription[]>();
            return subs.ToList();
        }
        public List<Category> GetCategories()
        {
            var json = Get($"/v1/categories");
            var obj = json.AsType<Category[]>();
            return obj.ToList();
        }
        public Category CreateCategory(string Title)
        {
            var json = Post($"/v1/categories", new Category() { Title = Title });
            var obj = json.AsType<Category>();
            return obj;
        }
        public Category UpdateCategory(Int64 ID, string Title)
        {
            var json = Put($"/v1/categories", new Category() { ID = ID, Title = Title });
            var obj = json.AsType<Category>();
            return obj;
        }
        public string DeleteCategory(Int64 ID)
        {
            return Delete($"/v1/categories/{ID}");
        }
        public string MarkCategoryEntriesAsRead(Int64 ID)
        {
            return Put($"/v1/categories/{ID}/mark-all-as-read", "");
        }
        public List<Feed> GetCategoryFeeds(Int64 ID)
        {
            return Get($"/v2/categories/{ID}").AsType<Feed[]>().ToList();
        }
        public List<Feed> GetFeeds()
        {
            var json = Get($"/v1/feeds");
            var obj = json.AsType<Feed[]>();
            return obj.ToList();
        }
        public string Export()
        {
            var opml = Get($"/v1/export");
            return opml;
        }
        public string Import(string OPMLStr)
        {
            var json = Post("/v1/import", OPMLStr);
            return json;
        }
        public Feed GetFeed(Int64 ID)
        {
            var json = Get($"/v1/feeds/{ID}");
            var obj = json.AsType<Feed>();
            return obj;
        }
        public Int64 CreateFeed(string url, Int64 CategoryID)
        {
            var json = Post($"/v1/feeds", new { feed_url = url, category_id = CategoryID });
            dynamic jsonObj = JObject.Parse(json);
            return jsonObj.feed_id;
        }
        public Feed UpdateFeed(Feed feed)
        {
            var json = Put($"/v1/feeds/{feed.ID}", feed);
            var obj = json.AsType<Feed>();
            return obj;
        }
        public string DeleteFeed(Int64 FeedID)
        {
            return Delete($"/v1/feeds/{FeedID}");
        }
        public string RefreshFeed(Int64 ID)
        {
            return Put($"/v1/feeds/{ID}/refresh", null);
        }
        public string RefreshAllFeeds()
        {
            return Put($"/v1/feeds/refresh", null);
        }
        public FeedIcon GetFeedIcon(Int64 ID)
        {
            var json = Get($"/v1/feeds/{ID}/icon");
            var obj = json.AsType<FeedIcon>();
            return obj;
        }
        public Entry GetFeedEntry(Int64 FeedID, Int64 EntryID)
        {
            var json = Get($"/v1/feeds/{FeedID}/entries/{EntryID}");
            var obj = json.AsType<Entry>();
            return obj;
        }
        public Entry GetEntry(Int64 EntryID)
        {
            var json = Get($"/v1/entries/{EntryID}");
            var obj = json.AsType<Entry>();
            return obj;
        }
        public List<Entry> GetEntries(Filter f)
        {
            var path = BuildFilterQueryString("/v1/entries", f);
            var json = Get(path);
            var obj = json.AsType<EntryResultSet>();
            return obj.Entries.ToList();
        }
        public List<Entry> GetFeedEntries(Int64 ID, Filter f)
        {
            var path = BuildFilterQueryString($"/v1/feeds/{ID}/entries", f);
            var json = Get(path);
            var obj = json.AsType<EntryResultSet>();
            return obj.Entries.ToList();
        }
        public string MarkFeedEntriesAsRead(Int64 ID)
        {
            return Put($"/v1/feeds/${ID}/mark-all-as-read", "");
        }
        public string UpdateEntries(Int64[] IDs, string status)
        {
            return Put($"/v1/entries", new { entry_ids = IDs, status });
        }
        public string ToggleEntryBookmark(Int64 ID)
        {
            return Put($"/v1/entries/{ID}/bookmark", null);
        }
        public string HealthCheck()
        {
            return Get("/healthcheck");
        }
        public string GetVersion()
        {
            return Get("/version");
        }
        public string BuildFilterQueryString(string path, Filter f)
        {
            Dictionary<string, string> valueset = new();
            if (f != null)
            {
                if (!f.Status.IsNullOrEmpty())
                {
                    valueset.Add("status", f.Status);
                }
                if (!f.Direction.IsNullOrEmpty())
                {
                    valueset.Add("direction", f.Direction);
                }
                if (!f.Order.IsNullOrEmpty())
                {
                    valueset.Add("order", f.Order);
                }
                if (f.Limit >= 0)
                {
                    valueset.Add("limit", f.Limit.ToString());
                }
                if (f.Offset >= 0)
                {
                    valueset.Add("offset", f.Offset.ToString());
                }
            }
            StringBuilder args = new();
            if (valueset.Count != 0)
            {
                foreach (var kv in valueset)
                {
                    args.Append($"&{kv.Key}={kv.Value}");
                }
                args[0] = '?';
            }
            return path + args.ToString();
        }
    }
}
