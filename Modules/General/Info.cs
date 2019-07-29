using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using System.IO;
using Discord.WebSocket;
using System.Threading;

namespace Collectorcord.Modules.General {
    public class Info : ModuleBase<SocketCommandContext> {

        [Command("ping")]
        public async Task Ping() {
            await ReplyAsync("Pong! My latency is `" + Context.Client.Latency + " ms`!");
        }

        [Command("help")]
        public async Task Help() {
            await ReplyAsync("Sorry to disappoint! I'm not capable of much yet, I was just created in fact! But the fact that you can see this means that my " +
                "server is online :) The only two commands so far are `help`, `info` and `ping`. My prefix is `" + 
                Server.Global_Variables.BOT_PREFIX + "`, and my creator is Primaski#0826.");
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
