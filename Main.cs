using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wox.Plugin;
using System.Windows;
using System.Windows.Forms;
using System.Threading;
using Wox.Plugin.Emoji;
using Wox.Infrastructure.Http;
using System.Net;
using Wox.Infrastructure.Logger;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Wox.Plugin.PasswordGenerator
{
    class Main : IPlugin
    {
        private PluginInitContext context;

        public List<Result> Query(Query query)
        {
            var keyword = query.Search;

            if (string.IsNullOrEmpty(keyword))
            {
                var result = new Result
                {
                    Title = "Search for Emoji",
                    SubTitle = "Search for Emoji and add to clipboard automatically.",
                    IcoPath = "Images\\app.png"
                };
                return new List<Result> { result };
            }
            var results = new List<Result>();

            var emojiList = SearchEmoji(keyword);
            results.AddRange(emojiList.Select(emoji => new Result
            {
                Title = emoji,
                SubTitle = "Copy to clipboard.",
                IcoPath = "Images\\copy.png",
                Action = c =>
                {
                    CopyToClipboard(emoji);
                    context.API.ShowMsg("Emoji", "Emoji " + emoji + " was coppied to the clipboard.", "Images\\app.png");
                    return true;
                }
            }));
            return results;
        }

        public void Init(PluginInitContext context)
        {
            this.context = context;
        }

        protected List<string> SearchEmoji(string query)
        {
            string result;
            const string api = "https://emoji.getdango.com/api/emoji?q=";
            try {
                result = Http.Get(api + Uri.EscapeUriString(query)).Result;
            }
            catch (WebException e)
            {
                context.API.ShowMsg("Emoji: Couldn't parse API search results.");
                return new List<string>();
            }

            if (string.IsNullOrEmpty(result)) {
                return new List<string>();
            }

            ApiResponse json;
            try
            {
                json = JsonConvert.DeserializeObject<ApiResponse>(result);
            }
            catch (JsonSerializationException e)
            {
                context.API.ShowMsg("Emoji: Couldn't parse API search results.");
                return new List<string>();
            }

            if (json != null)
            {
                return json.results.Select(o => o.text).ToList();
            }
            return new List<string>();
        }

        protected void CopyToClipboard(string content) {
            Thread thread = new Thread(() => Clipboard.SetText(content));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }
    }
}
