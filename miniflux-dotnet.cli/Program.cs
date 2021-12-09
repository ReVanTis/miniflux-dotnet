using Mono.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Miniflux.CLI
{
    public enum CLIStates
    {
        StateStart,
        StateStop,
        StateShowTitle,
        StateShowContent,
    }
    internal class Program
    {
        public const string DefaultConfigFileName = ".miniflux_cli_config.json";
        public static CLIStates currState = CLIStates.StateStart;
        private static void Main(string[] args)
        {
            // For CJK compatibility.
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            string homePath = (Environment.OSVersion.Platform == PlatformID.Unix ||
                   Environment.OSVersion.Platform == PlatformID.MacOSX)
                   ? Environment.GetEnvironmentVariable("HOME")
                   : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");

            string configFilePath = Path.Combine(homePath, DefaultConfigFileName);
            var optionSet = new OptionSet()
            {
                { "c|config=", f => { configFilePath = f; } },
            };

            optionSet.Parse(args);

            if (!File.Exists(configFilePath))
            {
                Console.WriteLine($"Config file not found at {configFilePath}");
                Environment.Exit(1);
            }
            var configJson = File.ReadAllText(configFilePath);
            MinifluxCliConfig config = configJson.AsType<MinifluxCliConfig>();
            if (config.URL.IsNullOrEmpty() || config.Token.IsNullOrEmpty())
            {
                Console.WriteLine($"Server URL or token is invalid in config file: {configFilePath}");
                Environment.Exit(1);
            }
            MinifluxClient client = new()
            {
                URL = config.URL,
                APIToken = config.Token,
                Proxy = config.Proxy,
            };
            var version = client.GetVersion();
            var health = client.HealthCheck();
            User me = client.GetCurrentUser();
            if (me.Username.IsNullOrEmpty())
            {
                Console.WriteLine("Login failed, check your token.");
                Environment.Exit(1);
            }
            Console.WriteLine($"Hi {(me.IsAdmin ? "Admin " : "")}{me.Username}!\nYou are now checking new feeds from {client.URL}, with Miniflux ver.{version}.\nCurrent server health status is: {health}.");
            List<Entry> entries = new();
            int remaining = -1;
            var currentEntryEnumerator = entries.GetEnumerator();
            while (true)
            {
                if (remaining == 0)
                    currState = CLIStates.StateStop;

                switch (currState)
                {
                    case CLIStates.StateStart:
                        {
                            entries = client.GetEntries(new Filter()
                            {
                                Status = "unread",
                                Direction = "desc",
                            });
                            Console.WriteLine($"You now have {entries.Count} unread entries.\nEnter to continue reading.\nR to refresh right now\n");
                            string res = Console.ReadLine();
                            if (res.ToLower().Contains('r'))
                            {
                                Console.WriteLine("Refreshing...");
                                client.RefreshAllFeeds();
                                Thread.Sleep(5000);
                            }
                            else currState = CLIStates.StateShowTitle;
                            remaining = entries.Count;
                            currentEntryEnumerator = entries.GetEnumerator();
                            currentEntryEnumerator.MoveNext();
                            break;
                        }
                    case CLIStates.StateShowTitle:
                        {
                            remaining--;
                            var entry = currentEntryEnumerator.Current;
                            if (entry == null)
                            {
                                Console.WriteLine("Entry is null and I could not handle this one... Shutting down now...");
                                currState = CLIStates.StateStop;
                                continue;
                            }
                            string minifluxURL = $"{client.URL}/unread/entry/{entry.ID}";
                            Console.WriteLine(@$"
{remaining} entries remaining
----------------------------------------------------------------
{entry.Feed.Title} - {entry.Title} - {entry.ReadingTime} mins ETR
{entry.Date:yyyy.MM.dd HH:mm:ss} - Auhor: {entry.Author}
Entry Link  : {minifluxURL}
Origin Link : {entry.URL}

Enter: mark as read & next R:show content K:skip to next O:show content and open in browser");
                            var input = Console.ReadLine();
                            if (input.ToLower().Contains('o'))
                            {
                                try
                                {
                                    Process.Start(new ProcessStartInfo()
                                    {
                                        FileName = entry.URL,
                                        UseShellExecute = true,
                                    });
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Exception occured invoking browser:");
                                    Console.WriteLine(e.ToString());
                                    Console.WriteLine("Show Content in CLI instead:");
                                    currState = CLIStates.StateShowContent;
                                    continue;
                                }
                            }
                            if (input.ToLower().Contains('r'))
                            {
                                currState = CLIStates.StateShowContent;
                                continue;
                            }

                            if (!input.ToLower().Contains('k'))
                            {
                                var response = client.UpdateEntries(new Int64[] { entry.ID }, Consts.EntryStatusRead);
                                Console.WriteLine("Marked as read. " + response.ToString());
                            }
                            currentEntryEnumerator.MoveNext();
                            break;
                        }
                    case CLIStates.StateShowContent:
                        {
                            var entry = currentEntryEnumerator.Current;
                            Console.WriteLine(entry.Content.ToPlainText());
                            Console.WriteLine(@$"
Enter: mark as read & next  S:mark as star  K:skip to next O:open in browser");
                            var input = Console.ReadLine();
                            if (input.ToLower().Contains('o'))
                            {
                                Process.Start(new ProcessStartInfo()
                                {
                                    FileName = entry.URL,
                                    UseShellExecute = true,
                                });
                            }
                            if (input.ToLower().Contains('s') && !entry.Starred)
                            {
                                var response = client.ToggleEntryBookmark(entry.ID);
                                Console.WriteLine("Marked as starred." + response.ToString());
                            }
                            if (!input.ToLower().Contains('k'))
                            {
                                var response = client.UpdateEntries(new Int64[] { entry.ID }, Consts.EntryStatusRead);
                                Console.WriteLine("Marked as read. " + response.ToString());
                            }
                            currentEntryEnumerator.MoveNext();
                            currState = CLIStates.StateShowTitle;
                            break;
                        }
                    case CLIStates.StateStop:
                        {
                            Console.WriteLine("You've finished your feeds. Goodbye and have a nice day!");
                            Environment.Exit(0);
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("Invalid CLI state, exiting...");
                            Environment.Exit(1);
                            break;
                        }
                }
            }
        }
    }
}