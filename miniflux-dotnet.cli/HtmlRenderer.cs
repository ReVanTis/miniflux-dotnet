using HtmlAgilityPack;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Miniflux.CLI
{
    // credits goes to https://stackoverflow.com/users/1034136/serj-tm on https://stackoverflow.com/a/30088920
    public static class HtmlRenderer
    {
        public static void AppendDoubleLine(this StringBuilder sb)
        {
            sb.AppendLine();
            sb.AppendLine();
        }
        public static string ToPlainText(this string htmlStr)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlStr);
            return doc.ToPlainText();
        }
        //HtmlAgilityPack part
        public static string ToPlainText(this HtmlDocument doc)
        {
            var builder = new System.Text.StringBuilder();
            var state = ToPlainTextState.StartLine;

            Plain(builder, ref state, new[] { doc.DocumentNode });
            return builder.ToString();
        }
        static void Plain(StringBuilder builder, ref ToPlainTextState state, IEnumerable<HtmlNode> nodes)
        {
            foreach (var node in nodes)
            {
                if (node is HtmlTextNode text)
                {
                    Process(builder, ref state, HtmlEntity.DeEntitize(text.Text).ToCharArray());
                }
                else
                {
                    var tag = node.Name.ToLower();

                    if (tag == "br")
                    {
                        builder.AppendDoubleLine();
                        state = ToPlainTextState.StartLine;
                    }
                    else if (NonVisibleTags.Contains(tag))
                    {
                    }
                    else if (InlineTags.Contains(tag))
                    {
                        Plain(builder, ref state, node.ChildNodes);
                    }
                    else
                    {
                        if (state != ToPlainTextState.StartLine)
                        {
                            builder.AppendDoubleLine();
                            state = ToPlainTextState.StartLine;
                        }
                        Plain(builder, ref state, node.ChildNodes);
                        if (state != ToPlainTextState.StartLine)
                        {
                            builder.AppendDoubleLine();
                            state = ToPlainTextState.StartLine;
                        }
                    }

                }

            }
        }
        //System.Xml.Linq part
        public static string ToPlainText(this IEnumerable<XNode> nodes)
        {
            var builder = new StringBuilder();
            var state = ToPlainTextState.StartLine;

            Plain(builder, ref state, nodes);
            return builder.ToString();
        }
        static void Plain(StringBuilder builder, ref ToPlainTextState state, IEnumerable<XNode> nodes)
        {
            foreach (var node in nodes)
            {
                if (node is XElement element)
                {
                    var tag = element.Name.LocalName.ToLower();

                    if (tag == "br")
                    {
                        builder.AppendDoubleLine();
                        state = ToPlainTextState.StartLine;
                    }
                    else if (NonVisibleTags.Contains(tag))
                    {
                    }
                    else if (InlineTags.Contains(tag))
                    {
                        Plain(builder, ref state, element.Nodes());
                    }
                    else
                    {
                        if (state != ToPlainTextState.StartLine)
                        {
                            builder.AppendDoubleLine();
                            state = ToPlainTextState.StartLine;
                        }
                        Plain(builder, ref state, element.Nodes());
                        if (state != ToPlainTextState.StartLine)
                        {
                            builder.AppendDoubleLine();
                            state = ToPlainTextState.StartLine;
                        }
                    }

                }
                else if (node is XText text)
                {
                    Process(builder, ref state, text.Value.ToCharArray());
                }
            }
        }
        //common part
        public static void Process(StringBuilder builder, ref ToPlainTextState state, params char[] chars)
        {
            foreach (var ch in chars)
            {
                if (char.IsWhiteSpace(ch))
                {
                    if (IsHardSpace(ch))
                    {
                        if (state == ToPlainTextState.WhiteSpace)
                            builder.Append(' ');
                        builder.Append(' ');
                        state = ToPlainTextState.NotWhiteSpace;
                    }
                    else
                    {
                        if (state == ToPlainTextState.NotWhiteSpace)
                            state = ToPlainTextState.WhiteSpace;
                    }
                }
                else
                {
                    if (state == ToPlainTextState.WhiteSpace)
                        builder.Append(' ');
                    builder.Append(ch);
                    state = ToPlainTextState.NotWhiteSpace;
                }
            }
        }
        static bool IsHardSpace(char ch)
        {
            return ch == 0xA0 || ch == 0x2007 || ch == 0x202F;
        }
        private static readonly HashSet<string> InlineTags = new()
        {
            //from https://developer.mozilla.org/en-US/docs/Web/HTML/Inline_elemente
            "b",
            "big",
            "i",
            "small",
            "tt",
            "abbr",
            "acronym",
            "cite",
            "code",
            "dfn",
            "em",
            "kbd",
            "strong",
            "samp",
            "var",
            "a",
            "bdo",
            "br",
            "img",
            "map",
            "object",
            "q",
            "script",
            "span",
            "sub",
            "sup",
            "button",
            "input",
            "label",
            "select",
            "textarea"
        };
        private static readonly HashSet<string> NonVisibleTags = new()
        {
            "script",
            "style"
        };
        public enum ToPlainTextState
        {
            StartLine = 0,
            NotWhiteSpace,
            WhiteSpace,
        }
    }
}
