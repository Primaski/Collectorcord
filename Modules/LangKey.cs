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
using static Collectorcord.Server.User.Profile;
using static Collectorcord.Server.Listings.Collections;

namespace Collectorcord.Modules {
    public class LangKey : ModuleBase<SocketCommandContext> {
        [Command("i")]
        public async Task I([Remainder] string args = null) {
            string check = (args == null || args == "") ? null : args.Split(' ')[0].ToLower();
            args = args.ToLower();
            string response = "";
            switch (check) {
                case null:
                case "":
                    await ReplyAsync("You...what?"); return;
                case "dont":
                case "don't":
                    await ReplyAsync("Sorry~ Primaski was in a rush today and didn't finish the `i don't collect` command. Just tell him, he'll remove it from the database.");
                    return;
                case "collect":
                    response = ICollect((SocketGuildUser)Context.User, args.Replace("collect","").Replace("collect ",""));
                    await ReplyAsync(response);
                    return;
                case "buy":
                    await ReplyAsync("Sorry! The market listing feature is not yet available! Check back later.");
                    return;
                case "sell":
                    await ReplyAsync("Sorry! The market listing feature is not yet available! Check back later.");
                    return;
                default:
                    await ReplyAsync("You...what?"); return;
            }
        }

        [Command("who")]
        public async Task Who([Remainder] string args = null) {
            string check = (args == null || args == "") ? null : args.Split(' ')[0].ToLower();
            switch (check) {
                case null:
                case "":
                    await ReplyAsync("Whomst've?!");
                    return;
                case "collects":
                    Embed response = CollectorsOf((SocketGuildUser)Context.User, args.Replace("collects","").Replace("collects ",""));
                    await ReplyAsync("",false,response);
                    return;
                default:
                    await ReplyAsync("I'm not sure what you mean.");
                    return;
            }
        }

        [Command("collects")]
        public async Task Collect2([Remainder] string args = null) {
            await Collect(args);
        }

        [Command("collectors")]
        public async Task Collect([Remainder] string args = null) {
            if(args != null) {
                args = args.ToLower();
                args = args.Replace("of ", "");
            }
            Embed response = CollectorsOf((SocketGuildUser)Context.User, args);
            await ReplyAsync("", false, response);
            return;
        }

        [Command("what")]
        public async Task What([Remainder] string args = null) {
            string check = (args == null || args == "") ? null : args.Split(' ')[0].ToLower();
            args = args.ToLower();
            switch (check) {
                case "does":
                    args.Replace("does ", "").Replace("does","");
                    Embed x = WhatDoes((SocketGuildUser)Context.User, args.Replace("does ","").Replace("does",""));
                    await ReplyAsync("", false, x);
                    return;
                default:
                    await ReplyAsync("Not sure what you mean. Maybe you meant, `what does...`?");
                    return;
            }
        }

        private Embed WhatDoes(SocketGuildUser user, string args) {
            //assumes user
            string[] splits = args.Split(' ');
            if(splits.Count() != 2) {
                return new EmbedBuilder().WithTitle("I'm not sure what you mean!").Build();
            }
            List<SocketGuildUser> mention = Util.InterpretUserInput(splits[0]);
            if(mention.Count() == 0) {
                return new EmbedBuilder().WithTitle("Who is " + splits[0] + "?").Build();
            }else if(mention[0] == null) {
                return new EmbedBuilder().WithTitle("Who is " + splits[0] + "?").Build();
            }
            switch (splits[1]) {
                case "collect": return WhatDoesUserCollect(user, mention[0].Id);
                default: return new EmbedBuilder().WithTitle("I'm not sure what you mean!").Build();
            }

        }
    }
}