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
using Discord.WebSocket;
using static Collectorcord.TextUtil;
using Discord;

namespace Collectorcord.Server.User {
    public class Profile {

        public static string ICollect(SocketGuildUser user, string pokemon = "") {
            pokemon = pokemon.ToLower().Trim();
            if (pokemon == "") {
                return ("What Pokemon do you collect?");
            }
            if (pokemon == "nothing" || pokemon == "none") {
                if (!Util.HasRole(user, "Non Collector")) {
                    var role = Util.GetRole("Non Collector");
                    user.AddRoleAsync(role);
                }
                if (!Database.ValueExists("Users", "userID", user.Id.ToString())) {
                    bool createAccount = CreateAccount(user);
                    if (!createAccount) {
                        return ("Error in creating account. Terminating.");
                    }
                }
		bool success = false;
		try{
                success = Database.AddEntry("Collections",
                                new string[] { "userID", "pokename", "recorded" },
                                new string[] { user.Id.ToString(), "nothing", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
		}catch(Exception e){
			return e.Message + "\n" + e.StackTrace;
		}
                if (success) {
                    return "Thank you! I made you an account and specified that you collect nothing.";
                } else {
                    return "I already know you collect nothing, silly!";
                }
            }
            if (Util.HasRole(user, Capitalize(pokemon)) ||
                Util.HasRole(user, Capitalize(pokemon.Substring(0, pokemon.Length - 1)))) {
                return ("I already knew you collect " + pokemon + "!");
            }

            if (Database.ValueExists("Pokemon", "pokename", pokemon) || Database.ValueExists("Pokemon", "pokename",
                pokemon.Substring(0, pokemon.Length - 1))) {
                try {
                    if (!Database.ValueExists("Users", "userID", user.Id.ToString())) {
                        bool createAccount = CreateAccount(user);
                        if (!createAccount) {
                            return ("Error in creating account. Terminating.");
                        }
                    }
                    bool success = false;
                    if (pokemon == "eeveelution" || pokemon == "eeveelutions") {

                    } else {
			try{
                        success = Database.AddEntry(
                            "Collections",
                            new string[] { "userID", "pokename", "recorded" },
                            new string[] { user.Id.ToString(), pokemon, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
                    	}catch{
				return "I already know you collect " + pokemon + ", silly!";
			}
			}
                    if (success) {
			try{
                        AddCollectorRoles(user, pokemon);
			}catch{}
                        return ("Thank you! I now know that **" + user.Username + "** collects **" + Capitalize(pokemon) + "**!" +
                            "Type in `" + Server.Global_Variables.BOT_PREFIX + "collectors` or `" +
                            Server.Global_Variables.BOT_PREFIX + "who collects " + pokemon + "` to see your addition!");
                    } else {
                        return ("I already know you collect **" + pokemon + "**!");
                    }
                } catch (Exception e) {
                    return (e.Message + "\n `" + e.StackTrace + "`");
                }
            } else {
		try{
			return CollectSpecialCases(user,pokemon);
		}catch{
		}
                return ("That Pokemon does not exist! Make sure you're only specifying one at a time. " +
                    "Try something like, `>icollect spoink`");
            }
        }

        public static string IDontCollect(SocketGuildUser user, string pokemon = "") {
            throw new NotImplementedException();
        }

        private static string CollectSpecialCases(SocketGuildUser user, string args) {
            if (args == "eeveelution" || args == "eeveelutions") {
                string[] eeveelutions = { "eevee", "flareon", "glaceon", "jolteon", "leafeon", "sylveon", "umbreon", "vaporeon" };
                foreach (string eon in eeveelutions) {
                    if (Util.HasRole(user, Capitalize(eon))) {
                        return "I already know you collect" + Capitalize(eon) + "!";
                    }
                    Database.AddEntry(
                        "Collections",
                        new string[] { "userID", "pokename", "recorded" },
                        new string[] { user.Id.ToString(), eon, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
                }
		return "Thank you! I now know that **" + user.Username + "** collects **eeveelutions**!" +
                            "Type in `" + Server.Global_Variables.BOT_PREFIX + "collectors` or `" +
                            Server.Global_Variables.BOT_PREFIX + "who collects eeveelutions` to see your addition!";
            }
	        return ("That Pokemon does not exist! Make sure you're only specifying one at a time. " +
                    "Try something like, `>icollect spoink`");
        }

        private static void AddCollectorRoles(SocketGuildUser user, string pokemon) {
            if (!Util.HasRole((SocketGuildUser)user, "Collector")) {
                var role = Util.GetRole("Collector");
                (user as IGuildUser).AddRoleAsync(role);
            }
		/*
            if (!Util.HasRole(user, Capitalize(pokemon))) {
                if (!Util.RoleExists(Capitalize(pokemon))) {
                    try {
                        Discord.Rest.RestRole role = new RoleBuilder(Capitalize(pokemon))
                            .BetweenRoles("~~~~~~~~~Collecting Pokemon~~~~~~~~~",
                            "~~~~~~~~~Member Roles~~~~~~~~~~").Build();
                        (user as IGuildUser).AddRoleAsync(role);
                    } catch (Exception e) {
                        throw e;
                    }
                } else {
                    var role = Util.GetRole(Capitalize(pokemon));
                    (user as IGuildUser).AddRoleAsync(role);
                }

            }
		*/
        }

        public static bool CreateAccount(SocketUser user) {
            return Database.AddEntry(
               "Users",
                new string[] { "userID, cachedUsername", "joinDate" },
                new string[] { user.Id.ToString(), user.Username, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
            );
        }
    }
}
