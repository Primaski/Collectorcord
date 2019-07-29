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
using static Collectorcord.TextUtil;
using Collectorcord.Server;
using System.Data;
using System.Text.RegularExpressions;

namespace Collectorcord.Modules.Listings {
    public class Collections : ModuleBase<SocketCommandContext> {
        string spoink = "<:SpoinkPeek:601475058220924928>";
        string azu = "<:AzuPeek:601321380151296011>";
        [Command("whocollects")]
        public async Task Collect2([Remainder] string arg = null) {
            await Collect(arg);
        }

        [Command("collects")]
        public async Task Collect3([Remainder] string arg = null) {
            await Collect(arg);
        }

        [Command("collectors")]
        public async Task Collect([Remainder] string arg = null) {
            bool invalidPokemon = false;
            string pokemon = "";
            int pageNo = 1;
            if(arg != null) {
                Regex pageno = new Regex(@"(p(age)?\s?\d+)");
                if (pageno.IsMatch(arg)) {
                    pageNo = Int32.Parse(Regex.Match(arg,@"\d+").Value);
                    arg = Regex.Replace(arg, @"(p(age)?\s?\d+)", "");
                }
                if(arg.Contains("of ")) {
                    arg = arg.Replace("of ", "");
                }
                if(!Database.ValueExists("Pokemon", "pokename", arg.ToLower().Trim())) {
                    invalidPokemon = true;
                } else {
                    pokemon = " and c.pokename = '" + arg.ToLower().Trim() + "'";
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
                (" of " + Capitalize(arg) + ": " + azu);
            EmbedBuilder result = new EmbedBuilder()
                .WithTitle(spoink + " Collectors" + collectorsOf)
                .WithColor(Color.Purple)
                .WithFooter("This is page " + pageNo + ". Type it again with page " + 
                (pageNo+1) + " for more!");

            try {
                for (int i = 0 + ((pageNo-1)*10); 
                    (i < entriesPerPage && i < dt.Rows.Count); i++) {
                    ID = dt.Rows[i].ItemArray[0].ToString();
                    pokename = dt.Rows[i].ItemArray[1].ToString();
                    cached = dt.Rows[i].ItemArray[2].ToString();
                    var user = Util.GetUserByID(UInt64.Parse(ID));
                    if (user == null) {
                        mention = cached;
                    } else {
                        mention = user.Mention;
                    }
                    string startdesc = (pokemon == "") ? 
                        (Capitalize(pokename) + " --> ") : "" ;
                    description += startdesc + mention + "\n";
                }
            } catch (Exception e) {
                await ReplyAsync(e.Message + "\n" + e.StackTrace);
            }

            await ReplyAsync("",false,result.WithDescription(description).Build());

        }
    }
}
