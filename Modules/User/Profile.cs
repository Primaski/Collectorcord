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

namespace Collectorcord.Modules.User {
    public class Profile : ModuleBase<SocketCommandContext> {

        [Command("imcollecting")]
        public async Task ICollect2([Remainder] string pokemon = "") {
            await ICollect(pokemon);
        }

        [Command("icollect")]
        public async Task ICollect([Remainder] string pokemon = "") {
            pokemon = pokemon.ToLower().Trim();
            if(Context.Guild.Id != Server.Global_Variables.POKECOLLECTORS_SERVER_ID) {
                await ReplyAsync("Please ensure you are in the Pokecollectors server.");
                return;
            }
            if(pokemon == "") {
                await ReplyAsync("What Pokemon do you collect?");
                return;
            }
            if(pokemon == "nothing" || pokemon == "none") {
                if(!Server.Util.HasRole((SocketGuildUser)Context.User,"Non Collector")) {
                    var role = Server.Util.GetRole("Non Collector");
                    await (Context.User as IGuildUser).AddRoleAsync(role);
                }
            }
	    if(Util.HasRole((SocketGuildUser)Context.User, Capitalize(pokemon)) ||
	       Util.HasRole((SocketGuildUser)Context.User, Capitalize(pokemon.Substring(0,pokemon.Length-1)))){
		await ReplyAsync("I already knew you collect " + pokemon + "!"); return;
	    }

            if (Database.ValueExists("Pokemon", "pokename", pokemon) || Database.ValueExists("Pokemon", "pokename", 
                pokemon.Substring(0,pokemon.Length-1)) || pokemon == "eeveelution" || pokemon == "eeveelutions") {
                try {
                    if(!Database.ValueExists("Users", "userID", Context.User.Id.ToString())) {
                        bool createAccount = CreateAccount(Context.User);
                        if (!createAccount) {
                            await ReplyAsync("Error in creating account. Terminating.");
                            return;
                        }
                    }
		    bool success = false;
                    if(pokemon == "eeveelution" || pokemon == "eeveelutions") {
                        string[] eeveelutions = { "eevee", "flareon", "glaceon", "jolteon", "leafeon", "sylveon", "umbreon", "vaporeon" };
                        foreach(string eon in eeveelutions) {
			    if(Server.Util.HasRole((SocketGuildUser)Context.User,Capitalize(eon))){
				return;
		            }
                            success = Database.AddEntry(
                                "Collections",
                                new string[] { "userID", "pokename", "recorded" },
                                new string[] { Context.User.Id.ToString(), eon, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
                        }
                    }else{
                    success = Database.AddEntry(
                        "Collections", 
                        new string[] { "userID", "pokename", "recorded" }, 
                        new string[] { Context.User.Id.ToString(), pokemon, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
		    }
                    if (success) {
                        await ReplyAsync("Thank you! I now know that **" + Context.User.Username + "** collects **" + Capitalize(pokemon) + "**!" +
                            "Check back later and type in `" + Server.Global_Variables.BOT_PREFIX + "collectors` or `" +
                            Server.Global_Variables.BOT_PREFIX + "collectors of " + pokemon + "` to see your addition! (Under construction)");
                        AddCollectorRoles(pokemon);
                        return;
                    } else {
                        await ReplyAsync("I already know you collect **" + pokemon + "**!");
                        return;
                    }
                }catch(Exception e) {
                    await ReplyAsync(e.Message + "\n `" + e.StackTrace + "`");
                }
            } else { 
                await ReplyAsync("That Pokemon does not exist! Make sure you're only specifying one at a time. " +
                    "Try something like, `/icollect spoink`");
                return;
            }


        }

        private void AddCollectorRoles(string pokemon) {
            if(!Util.HasRole((SocketGuildUser)Context.User, "Collector")) {
                var role = Util.GetRole("Collector");
                (Context.User as IGuildUser).AddRoleAsync(role);
            }
            if (!Util.HasRole((SocketGuildUser)Context.User, Capitalize(pokemon))){
                if (!Util.RoleExists(Capitalize(pokemon))){
                    try {
                        Discord.Rest.RestRole role = new RoleBuilder(Capitalize(pokemon))
                            .BetweenRoles("~~~~~~~~~Collecting Pokemon~~~~~~~~~",
                            "~~~~~~~~~Member Roles~~~~~~~~~~").Build();
                        (Context.User as IGuildUser).AddRoleAsync(role);
                    } catch (Exception e){
                        throw e;
                    }
                } else {
                    var role = Util.GetRole(Capitalize(pokemon));
                    (Context.User as IGuildUser).AddRoleAsync(role);
                }
                
            }
        }

        public bool CreateAccount(SocketUser user) {
            return Database.AddEntry(
               "Users",
                new string[] { "userID, cachedUsername", "joinDate" },
                new string[] { Context.User.Id.ToString(), Context.User.Username, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
            );
        }
    }
}
