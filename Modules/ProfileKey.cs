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
    class ProfileKey : ModuleBase<SocketCommandContext> {
        [Command("snipepass")]
        public async Task Snipepass([Remainder] string args = null) {
            Embed response = Server.User.Badges.SnipePass((SocketGuildUser)Context.User,args);
            await ReplyAsync("", false, response);
        }
    }
}
