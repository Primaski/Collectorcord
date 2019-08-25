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
    public class Info : ModuleBase<SocketCommandContext> {

	[Command("say", RunMode = RunMode.Async)]
        public async Task Say([Remainder] string args = null) {
            if (args == null) {
                await ReplyAsync("I mean, like, what do you want me to say?");
            } else {
                if (args.Contains(">")) {
                    await ReplyAsync("Why are you trying to break me?");
                    return;
                }
                if (args.Contains("@everyone") || args.Contains("@here")) {
                    await ReplyAsync("Say that again, but without the ping.");
                    return;
                }
                await Context.Message.DeleteAsync();
                await ReplyAsync(args);

            }
        }

	
        [Command("snipepass")]
        public async Task Snipepass([Remainder] string args = null) {
            Embed response = Server.User.Badges.SnipePass((SocketGuildUser)Context.User,args);
            await ReplyAsync("", false, response);
        }

        [Command("ping")]
        public async Task Ping() {
            await ReplyAsync("Pong! My latency is `" + Context.Client.Latency + " ms`!");
        }

        [Command("help")]
        public async Task Help() {
            await ReplyAsync("",false,new EmbedBuilder().WithDescription("Try writing out full sentences! Here are the sentences I currently understand.")
                .AddField("General","`ping`, `info`, `say`").AddField("Collectors","_ _")
                .AddField("_ _","`>i collect [pokemon name]` ('nothing' if no pokemon, this is required to create an account)\n" +
		"`>i don't collect [pokemon]` (to revert)\n `>who collects [pokemon name]` \n `>what does [user] collect` \n `>snipepass`").Build());
        }

        [Command("invite")]
        public async Task Inv() {
            await Information();
        }
        [Command("support")]
        public async Task Supp() {
            await Information();
        }

        [Command("info")]
        public async Task Information() {
            await ReplyAsync(
                "***Bot Name:*** Collectorcord#8724\n" +
                "***Bot Creator:*** Primaski#0826\n" +
                "***Bot Creation Date:*** 2019/07/22\n" +
                "***Bot Prefix:*** `" + Server.Global_Variables.BOT_PREFIX + "` \n" +
                "***Version:*** 1.0, 2019/07/22" +
               "***Written in:*** C#\n" +
                "***Bot Token:*** u wish\n" +
                "***Bot Source Code:*** https://github.com/Primaski/Collectorcord\n" +
                "***Bot Invite Link:*** Not invitable, and never invitable without owner consent. \n" +
                "***My home:*** https://discord.gg/fVfuuH3");
            return;
        }
    }
}