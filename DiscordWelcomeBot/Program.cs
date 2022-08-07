
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Discord_Chegg_Bot
{
    internal class Program
    {
        #region Values
        public static DiscordSocketClient _client;
        public static EmbedFooterBuilder SignFooter;
        public static int FileCount;

        #endregion


        public static CookieContainer cookie = new CookieContainer();


        public static void GetCookies()
        {

            cookie = new CookieContainer();
            var msgs = _client.GetGuild(0).GetTextChannel(927469168507359313).GetMessagesAsync(1).ToListAsync();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(msgs.Result.ToList()[0].ToList()[0].Attachments.ToList()[0].Url);


            WebClient myWebClient = new WebClient();



            // Download the resource and load the bytes into a buffer.
            byte[] buffer = myWebClient.DownloadData(msgs.Result.ToList()[0].ToList()[0].Attachments.ToList()[0].Url);

            // Encode the buffer into UTF-8
            string download = Encoding.UTF8.GetString(buffer);
            var CookiesString = download;

            var CookieLines = Regex.Split(CookiesString, "},");
            foreach (var text in CookieLines)
            {
                var name = text.Substring(text.IndexOf("\"name\": \"") + "\"name\": \"".Length);
                name = name.Remove(name.IndexOf("\""));

                var value = text.Substring(text.IndexOf("\"value\": \"") + "\"value\": \"".Length);
                value = value.Remove(value.IndexOf("\""));

                var domain = text.Substring(text.IndexOf("\"domain\": \"") + "\"domain\": \"".Length);
                domain = domain.Remove(domain.IndexOf("\""));

               
                cookie.Add(new System.Net.Cookie(name, value, "/", domain));
            }
        }

        public static async Task MainAsync()
        {



            SignFooter = new EmbedFooterBuilder().WithText("Powered by Meowdemia!").WithIconUrl("https://media.discordapp.net/attachments/917813714923683860/923807194640687134/Grey_Cute_Illustrated_Cat_and_Fish_Circle_Laptop_Sticker_1.png?width=559&height=559");

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents =
                GatewayIntents.Guilds |
                GatewayIntents.GuildMembers |
                GatewayIntents.GuildMessageReactions |
                GatewayIntents.GuildMessages |
                GatewayIntents.GuildVoiceStates | GatewayIntents.All

            });




            _client.Log += Log;

            await _client.LoginAsync(TokenType.Bot, "");
            await _client.StartAsync();


            _client.MessageReceived += MessageHandler;
            _client.UserJoined += UserJoinedHandler;


            await Task.Delay(-1);




        }

        

        private static Task UserJoinedHandler(SocketGuildUser e)
        {
            UserJoined(e).Wait();
            return Task.CompletedTask;
        }

        private static Task MessageHandler(SocketMessage e)
        {
            MessageRecieved(e).Wait();
            return Task.CompletedTask;
        }

        public static async Task UserJoined(SocketGuildUser e)
        {
            string dt = "";
            if(File.Exists(Environment.CurrentDirectory + $"\\{e.Id}"))
            {
                dt =  File.ReadAllText(Environment.CurrentDirectory + $"\\{e.Id}");
            }
            else
            {
                File.WriteAllText(Environment.CurrentDirectory +  $"\\{e.Id}", e.JoinedAt.Value.UtcDateTime.ToString("dddd, dd MMMM yyyy hh:mm tt", new System.Globalization.CultureInfo("en-US")));
            }

            var builder = new EmbedBuilder();
            var avatarurl = "";
            if(e.GetAvatarUrl() != null)
            {
                avatarurl = e.GetAvatarUrl();
            }
            else
            {
                avatarurl = e.GetDefaultAvatarUrl();
            }

            var username = e.Username + "#" + e.DiscriminatorValue;
            builder.WithAuthor(new EmbedAuthorBuilder().WithIconUrl(avatarurl).WithName(username));
            if (dt == "")
            {
                builder.WithTitle($"Welcome to My Server {username}, you are the {e.Guild.Users.Count}th member!");
            }
            else
            {
                builder.WithTitle($"Welcome back to My Server {username}, you are the {e.Guild.Users.Count}th member!");
            }

      dt = File.ReadAllText(Environment.CurrentDirectory + $"\\{e.Id}");
         // builder.ThumbnailUrl = "https://media.discordapp.net/attachments/917813714923683860/923392170000515082/meowdemia_circle.png?width=559&height=559";

            builder.WithFooter(SignFooter);
            builder.AddField("Account created at: ",""+ e.CreatedAt.UtcDateTime.ToString("dddd, dd MMMM yyyy hh:mm tt", new System.Globalization.CultureInfo("en-US")) +"", true);
           builder.AddField("Joined My Server at:", ""+dt+"", true);
            builder.AddField("Account's avatar url:", $" [Click]({avatarurl})", true);
            builder.Color = Color.Purple;
            // builder.WithImageUrl(avatarurl);
            await (e.Guild.Channels.ToList().Find(m => m.Id == 922634093810810883) as SocketTextChannel).SendMessageAsync("", false, builder.Build());
           var invites= await e.Guild.GetInvitesAsync();
        }
        
        public static async Task MessageRecieved(SocketMessage e)
        {
           
            


        }

        static void Main(string[] args)
        {
            MainAsync().Wait();
           
        }

        static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

    }


}
