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

namespace Collectorcord.Modules {
    public class AdminKey : ModuleBase<SocketCommandContext> {
        [Command("killswitch")]
        public async Task Killswitch([Remainder] string args = null) {
            if (!Util.HasRole((SocketGuildUser)Context.User, "Moderator")) {
                await ReplyAsync("This role was intended for Moderators, and you lack the appropriate permissions."); return;
            } else {
                string res = Server.General.Admin.Killswitchactivate(args);
                await ReplyAsync(res);
            }
        }

        [Command("query")]
        public async Task Query([Remainder] string args = null) {
            if (Context.User.Id != Global_Variables.MY_ID) {
                await ReplyAsync("This role was intended for Moderators, and you lack the appropriate permissions."); return;
            } else {
                string res = Server.General.Admin.Query(args);
                await ReplyAsync(res);
            }
        }

        [Command("res")]
        public async Task Restart() {
            if (!Util.HasRole((SocketGuildUser)Context.User, "Moderator") &&
		!Util.HasRole((SocketGuildUser)Context.User, "Helper") &&
		!Util.HasRole((SocketGuildUser)Context.User, "Trial Mod")) {
                await ReplyAsync("This role was intended for Moderators, and you lack the appropriate permissions."); return;
            } else {
                await ReplyAsync("`Restarting...`");
                string res = Server.General.Admin.Restart();
            }
        }

        [Command("exit")]
        public async Task Exit([Remainder] string args = null) {
            if (!Util.HasRole((SocketGuildUser)Context.User, "Moderator")) {
                await ReplyAsync("This role was intended for Moderators, and you lack the appropriate permissions."); return;
            } else {
                string res = Server.General.Admin.Exit();
                await ReplyAsync(res);
            }
        }
    }
}