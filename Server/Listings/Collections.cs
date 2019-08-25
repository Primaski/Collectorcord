using Collectorcord;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using Collectorcord.Server;
using MySql.Data.MySqlClient;
using System.Reflection;
using System;
using System.Data;
using Primbot_v._2.Server;
using Discord.WebSocket;
using Discord;
using System.Text.RegularExpressions;

namespace Collectorcord.Server.Listings {
    public static class Collections {
        static string spoink = "<:SpoinkPeek:601475058220924928>";
        static string azu = "<:AzuPeek:601321380151296011>";

        public static Embed CollectorsOf(SocketGuildUser user, string arg = null) {
            bool invalidPokemon = false;
            string pokemon = "";
            int pageNo = 1;
            if (arg != null) {
                Regex pageno = new Regex(@"(p(age)?\s?\d+)");
                if (pageno.IsMatch(arg)) {
                    pageNo = Int32.Parse(Regex.Match(arg, @"\d+").Value);
                    arg = Regex.Replace(arg, @"(p(age)?\s?\d+)", "");
                }
                if (arg.Contains("of ")) {
                    arg = arg.Replace("of ", "");
                }
                if (Database.ValueExists("Pokemon", "pokename", arg.ToLower().Trim())) {
                    pokemon = " and c.pokename = '" + arg.ToLower().Trim() + "'";
                } else if (Database.ValueExists("Pokemon", "pokename", arg.ToLower().Trim().Substring(0, arg.Length - 1))) {
                    arg = arg.ToLower().Trim().Substring(0, arg.Length - 1);
                    pokemon = " and c.pokename = '" + arg.ToLower().Trim() + "'";
                } else {
                    invalidPokemon = true;
                }
            }
            int entriesPerPage = 10 * pageNo;

            string query = "SELECT c.userID, c.pokename, u.cachedUsername " +
                "FROM Users u, Collections c " +
                "WHERE u.userID = c.userID" + pokemon + " " +
                "ORDER BY c.pokename, u.cachedUsername";
            DataTable dt = Database.Query(query);



            string ID, pokename, cached; string mention = ""; string description = "";
            string collectorsOf = (pokemon == "") ? ": " + azu :
                (" of " + TextUtil.Capitalize(arg) + ": " + azu);
            EmbedBuilder result = new EmbedBuilder()
                .WithTitle(spoink + " Collectors" + collectorsOf)
                .WithColor(Color.Purple)
                .WithFooter("This is page " + pageNo + ". Type it again with page " +
                (pageNo + 1) + " for more!");

            try {
                for (int i = 0 + ((pageNo - 1) * 10);
                    (i < entriesPerPage && i < dt.Rows.Count); i++) {
                    ID = dt.Rows[i].ItemArray[0].ToString();
                    pokename = dt.Rows[i].ItemArray[1].ToString();
                    cached = dt.Rows[i].ItemArray[2].ToString();
                    var userm = Util.GetUserByID(UInt64.Parse(ID));
                    if (userm == null) {
                        mention = cached;
                    } else {
                        mention = userm.Mention + " (" + userm.Username + ")";
                    }
                    string startdesc = (pokemon == "") ?
                        (TextUtil.Capitalize(pokename) + " --> ") : "";
                    description += startdesc + mention + "\n";
                }
            } catch (Exception e) {
                return (new EmbedBuilder().WithTitle("No one collects this Pokemon yet!").Build());
            }

            return (result.WithDescription(description).Build());

        }

        public static Embed WhatDoesUserCollect(SocketGuildUser context, ulong userID) {
	    //Console.WriteLine("got here");
            string query = "SELECT c.pokename " +
                "FROM Collections c " +
                "WHERE c.userID = " + userID.ToString();
            DataTable dt = Database.Query(query);
	    Console.WriteLine(dt);

            string ID, pokename, cached; string mention = ""; string description = "";
            EmbedBuilder result = new EmbedBuilder()
                .WithTitle(spoink + " They collect: " + azu)
                .WithColor(Color.Purple);
	    Console.WriteLine("got here");
            try {
                for (int i = 0; i < 10 && i < dt.Rows.Count; i++) {
                    string pokemon = dt.Rows[i].ItemArray[0].ToString();
                    description += TextUtil.Capitalize(pokemon) + "\n";
                }
            } catch (Exception e) {
                return (new EmbedBuilder().WithTitle(e.Message + "\n" + e.StackTrace)).Build();
            }

            return result.WithDescription(description).Build();
        }
    }
}
