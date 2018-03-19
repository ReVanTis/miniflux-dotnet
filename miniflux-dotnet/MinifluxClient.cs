using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Miniflux
{
    public class MinifluxClient
    {
        private const string userAgent = @"Miniflux dotNet Client Library <https://github.com/ReVanTis/miniflux-dotnet>"; 
        private const int defaultTimeout = 80_000; 

        public string URL { set; get; }
        public string Username { set; get; }
        public string Password { set; get; }
        public KeyValuePair<string, string> AuthHeader { get { return new KeyValuePair<string, string>("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(Username + ":" + Password))); } }

        public MinifluxClient(string url, string username, string password)
        {
            URL = url;
            Username = username;
            Password = password;
        }

        public HttpWebRequest BuildRequest(string Method, string path)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL + path);
            request.Method = Method;
            request.UserAgent = userAgent;
            request.Timeout = defaultTimeout;
            request.Headers.Add(AuthHeader.Key, AuthHeader.Value);
            request.Headers.Add("Content-Type", "application/json; charset=UTF-8");
            request.Headers.Add("Accept", "application/json");
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

                using (var strm = req.GetRequestStream())
                {
                    strm.Write(dateBytes, 0, dateBytes.Length);
                }
            }
            using (var res = (HttpWebResponse)req.GetResponse())
            {
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
                if (res.ContentType.ToLower().Contains("json") || res.ContentType.ToLower().Contains("xml"))
                {
                    using (StreamReader sr = new StreamReader(res.GetResponseStream()))
                    {
                        var jsonDoc = sr.ReadToEnd();
                        return jsonDoc;
                    }
                }
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
            User u = new User()
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

        public List<Subscription> Discover(string url)
        {
            var json = Post($"/v1/discover", new { url = url });
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
            return Put($"/v1/feeds/{ID}", null);
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

        public string UpdateEntries(Int64[] IDs, string status)
        {
            return Put($"/v1/entries", new { EntryIDs = IDs, Status = status });
        }

        public string ToggleEntryBookmark(Int64 ID)
        {
            return Put($"/v1/entries{ID}/bookmark", null);
        }

        public string BuildFilterQueryString(string path, Filter f)
        {
            Dictionary<string, string> valueset = new Dictionary<string, string>();
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
            StringBuilder args = new StringBuilder();
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
