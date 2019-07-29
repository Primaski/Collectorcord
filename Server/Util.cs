using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;

namespace Collectorcord.Server {
    public static class Util {
        public static SocketGuild Poke_Cache { get; set; }

        public static List<SocketGuildUser> InterpretUserInput(string expectedUsers, SocketGuild guild = null) {
            List<SocketGuildUser> users = new List<SocketGuildUser>();
            Regex IDcapture = new Regex(@"((?:\D|^|!|@|<)\d{16,18}(?: |,|\(|>|$))");
            if (guild == null) {
                guild = Poke_Cache;
            }

            if (!expectedUsers.Contains(",")) {
                expectedUsers.Trim();
                if (IDcapture.IsMatch(expectedUsers)) {
                    ulong ID = UInt64.Parse(Regex.Match(IDcapture.Match(expectedUsers).Value, @"\d{16,18}").Value);
                    users.Add(GetUserByID(ID, guild));
                } else {
                    if (expectedUsers.IndexOf("#") != -1) {
                        expectedUsers.Substring(0, expectedUsers.IndexOf("#"));
                    }
                    users.Add(GetUserByUsername(expectedUsers, guild));
                }
                return users;
            }

            string[] userList = expectedUsers.Split(',');
            foreach (string user in userList) {
                user.Trim();
                if (IDcapture.IsMatch(user)) {
                    ulong ID = UInt64.Parse(Regex.Match(IDcapture.Match(user).Value, @"\d{16,18}").Value);
                    users.Add(GetUserByID(ID));
                } else {
                    string userm = user;
                    if (user.IndexOf("#") != -1) {
                        userm = user.Substring(0, user.IndexOf("#"));
                    }
                    users.Add(GetUserByUsername(userm));
                }
            }
            return users;
        }

        public static SocketGuildUser GetUserByID(ulong ID, SocketGuild guild = null) {
            if (guild == null) {
                guild = Poke_Cache;
            }
            SocketGuildUser user = guild.GetUser(ID);
            return user;
        }

        public static SocketGuildUser GetUserByUsername(string username, SocketGuild guild = null) {
            if (guild == null) {
                guild = Poke_Cache;
            }
            username = username.ToLower().Trim();
            var guildUsers = guild.Users;
            List<SocketGuildUser> possibleMatch = new List<SocketGuildUser>();
            foreach (SocketGuildUser user in guildUsers) {
                if (!user.IsBot) {
                    if (user.Username.ToLower() == username) {
                        return user;
                    }
                    if (user.Username.ToLower().StartsWith(username)) {
                        possibleMatch.Add(user);
                    }
                }
            }
            if (possibleMatch.Count() == 1) {
                return possibleMatch[0];
            }
            return null;
        }

        public static List<string> ExtractRoleSubsetFromUser(SocketGuildUser x, List<string> soughtRoles, bool byID = false) {
            var userRoles = x?.Roles;
            List<string> result = new List<string>();
            if (userRoles == null) {
                return result;
            }
            foreach (SocketRole role in userRoles) {
                if (!byID) {
                    if (soughtRoles.Contains(role.Name)) {
                        result.Add(role.Name);
                    }
                } else {
                    if (soughtRoles.Contains(role.Id.ToString())) {
                        result.Add(role.Id.ToString());
                    }
                }
            }
            return result;
        }

        public static bool AddRoleToUser(SocketGuildUser user, SocketRole role, SocketGuild guild = null) {
            if(guild == null) {
                guild = Poke_Cache;
            }
            throw new NotImplementedException();
        }

        public static bool HasRole(SocketGuildUser x, string roleName, bool byID = false) {
            List<string> roles = ExtractRoleSubsetFromUser(x, new List<string> { roleName }, byID);
            if(roles.Count() == 0) {
                return false;
            }
            return true;
        }

        public static SocketRole GetRole(string roleName, SocketGuild guild = null) {
            if(guild == null) {
                guild = Poke_Cache;
            }
            return guild.Roles?.FirstOrDefault(w => w.Name == roleName) ?? null;
        }

        public static bool RoleExists(string roleName, SocketGuild guild = null) {
            if(guild == null) { guild = Poke_Cache; }
            return
                (guild.Roles.FirstOrDefault(w => w.Name == roleName) == null) 
                ? false : true;
        }
    }
}
