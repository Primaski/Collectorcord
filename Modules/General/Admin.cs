using Collectorcord;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using System.IO;
using Discord.WebSocket;
using System.Threading;
using Collectorcord.Server;
using MySql.Data.MySqlClient;
using System.Reflection;
using System;
using System.Data;
using Primbot_v._2.Server;

namespace Collectorcord.Modules.Admin {
    public class Admin : ModuleBase<SocketCommandContext> {

        [Command("killswitch")]
        public async Task Killswitchactivate([Remainder] string args = null) {
            if (Util.HasRole((SocketGuildUser)Context.User,"Moderator")) {
                await ReplyAsync("This role was intended for Moderators, and you lack the appropriate permissions."); return;
            }
            if (args == null) {
                await ReplyAsync(Killswitch.GetHelpMenu());
                await ReplyAsync("The current state of the Killswitch is: `" + Killswitch.GetState() + "`.");
                return;
            }
            args = args.ToLower();
            if (args.Contains("off")) {
                Killswitch.SetStatus(Killswitch.Status.Off);
            } else if (args.Contains("mute")) {
                Killswitch.SetStatus(Killswitch.Status.Mute);
            } else if (args.Contains("kick")) {
                Killswitch.SetStatus(Killswitch.Status.Kick);
            } else if (args.Contains("ban")) {
                Killswitch.SetStatus(Killswitch.Status.Ban);
            }
            await ReplyAsync("The current state of the Killswitch is: `" + Killswitch.GetState() + "`.");
            return;
        }

        [Command("temp")]
        public async Task Testing([Remainder] string arg = null) {
            //Database.PrintTable("Collections", new string[] { "userID", "pokename", "recorded", "quantity" });

            Random rand = new Random();
            char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            char[] num = "1234567890".ToCharArray();
            char[] alphanum = alpha.Concat(num).ToArray();
            string[] randomEntires = new string[50];

            for(int i = 0; i < 50; i++) {
                StringBuilder curr = new StringBuilder().Append("N");
                curr.Append(num[rand.Next(0, num.Length - 1)]);
                curr.Append(num[rand.Next(0, num.Length - 1)]);
                curr.Append(num[rand.Next(0, num.Length - 1)]);
                curr.Append(alphanum[rand.Next(0, alphanum.Length - 1)]);
                curr.Append(alpha[rand.Next(0, alpha.Length - 1)]);
                randomEntires[i] = curr.ToString();
            }
            StringBuilder deliv = new StringBuilder();
            foreach(var randomentry in randomEntires) {
                deliv.AppendLine(randomentry);
            }

            await ReplyAsync(deliv.ToString());

        }

        [Command("rawval")]
        public async Task Test([Remainder] string arg = null) {
            try {
                
                int args = Int32.Parse(arg);
                var oo = Util.Poke_Cache.Roles.ElementAt(args);
                await ReplyAsync("Raw value for " + oo.Name + ":\n" + oo.Permissions.RawValue.ToString());
            } catch (Exception e) {
                await ReplyAsync(e.Message);
            }
        }

        [Command("query")]
        public async Task Query([Remainder] string query = null) {
            if(Context.User.Id != Global_Variables.MY_ID) {
                return;
            }
            if(query == null) {
                await ReplyAsync("Please provide a query.");
            }
            DataTable dt = Database.Query(query);
            await ReplyAsync(dt.ToString());
        }

        [Command("res")]
        public async Task Restart() {
            if (Context.User.Id != Global_Variables.MY_ID) {
                return;
            }
            Console.WriteLine("`Restarting...`");
            Database.DisconnectFromServer();
            Program.Restart();
            return;
        }

        [Command("exit")]
        public async Task Exit() {
            if (Context.User.Id != Global_Variables.MY_ID) {
                return;
            }
            Console.WriteLine("`Exiting...`");
            Database.DisconnectFromServer();
            // Closes the current process
            Program.Exit();
            return;
        }
    }
}
