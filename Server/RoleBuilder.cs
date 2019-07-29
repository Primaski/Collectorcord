using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;
using Discord;

namespace Collectorcord.Server {
    public class RoleBuilder {

        public enum Permission {
            InstantInvite = 0x1,
            KickMembers = 0x2,
            BanMembers = 0x4,
            Administrator = 0x8,
            ManageChannels = 0x10,
            ManageGuild = 0x20,
            AddReactions = 0x40,
            ViewAuditLog = 0x80,
            PrioritySpeaker = 0x100,
            ViewChannel = 0x400,
            SendMessages = 0x800,
            SendTTS = 0x1000,
            ManageMessages = 0x2000,
            EmbedLinks = 0x4000,
            AttachFiles = 0x8000,
            ReadHistory = 0x10000,
            MentionEveryone = 0x20000,
            UseExternalEmojis = 0x40000,
            Connect = 0x100000,
            Speak = 0x200000,
            Deafen = 0x800000,
            MoveMembers = 0x1000000,
            VAD = 0x2000000,
            ChangeNickname = 0x4000000,
            ManageNicknames = 0x8000000,
            ManageRoles = 0x10000000,
            ManageWebhooks = 0x20000000,
            ManageEmojis = 0x40000000
        }
        private List<Permission> permissions = null;
        private ulong permVal = 0;

        private SocketRole role;
        private SocketGuild guild;
        private string name = "";
        private Color color = Color.LightGrey;
        private byte[] altColor = null;
        private int pos = -1;
        private string upperRole = "";
        private string lowerRole = "";
        private bool isHoisted = false;

        /// <summary>
        /// Similar to how EmbedBuilder works, create an instance of role builder with a name and guild, then
        //  use its method to specify role attributes, and then use Build() to finalize and add to Guild.
        /// attributes, 
        /// </summary>
        public RoleBuilder(string name, SocketGuild guild = null) {
            if (name == "" || name == null) {
                this.name = "NULL";
            }
            this.name = name;
            if (guild == null) {
                this.guild = Util.Poke_Cache;
            }
            return;
        }

        /// <summary>
        /// Builds and adds role to server. IF the operation fails, an exception will be thrown
        /// with the error message.
        /// </summary>
        public Discord.Rest.RestRole Build() {
            bool hexColorUsed = false;
            bool positionedByIndex = false;
            if (altColor != null) {
                hexColorUsed = true;
            }
            if (pos != -1) {
                positionedByIndex = true;
            } else if (upperRole != "" || lowerRole != "") {
                this.pos = GetIndexForRelativePos();
                positionedByIndex = true;
            }
            try {
                GeneratePermRawValue();
            } catch (Exception e) { throw e; }

            GuildPermissions perms = new GuildPermissions(permVal);
            Discord.Rest.RestRole role = null;
            if (hexColorUsed) {
                try {
                    Color localColor = new Color(altColor[0], altColor[1], altColor[2]);
                    role = guild.CreateRoleAsync(
                        name, perms, localColor, isHoisted
                    ).Result;
                }catch(Exception e) { throw e; }
            } else {
                try {
                    role = guild.CreateRoleAsync(
                        name, perms, color, isHoisted
                    ).Result;
                }catch(Exception e) { throw e; }
            }

            if (positionedByIndex) {
                //(roles count - pos) since this class rests on supposition 
                //that top most role is 0, and not bottom as discord API does
                ReorderRoleProperties x = new
                    ReorderRoleProperties(role.Id, guild.Roles.Count() - pos);
                IEnumerable<ReorderRoleProperties> roleNewPos =
                    (IEnumerable<ReorderRoleProperties>)
                    (new List<ReorderRoleProperties> { x });
                guild.ReorderRolesAsync(roleNewPos);
            }
            return role;
        }

        public RoleBuilder WithColor(Color color) {
            this.color = color;
            return this;
        }

        /// <summary>
        /// R (max 0xFF), G (max 0xFF), B (max 0xFF)s. Override of default Color method.
        /// </summary>
        public RoleBuilder WithColor(byte[] hex) {
            if (hex.Count() != 3) {
                throw new Exception("Incorrect array size. Color hex must have exactly three elements.");
            }
            altColor = hex;
            return this;
        }

        public RoleBuilder WithPermission(Permission perm) {
            if (permissions == null) {
                permissions = new List<Permission>();
            }
            permissions.Add(perm);
            return this;
        }

        public RoleBuilder WithPermissions(List<Permission> perms) {
            foreach (Permission perm in perms) {
                try {
                    WithPermission(perm);
                } catch (Exception e) {
                    throw e;
                }
            }
            return this;
        }

        /// <summary>
        /// Count starts from 1. An exact constant is expected, and will throw an exception 
        /// if exceeds count of roles + 1.
        /// </summary>
        public RoleBuilder AtPosition(int pos) {
            if (pos < 1) {
                return this;
            }
            return this;
        }

        /// <summary>
        /// Places immediately under role in role menu. Exceptions may be thrown at compile time.
        /// </summary>
        public RoleBuilder BelowRole(string roleName) {
            upperRole = roleName;
            return this;
        }

        /// <summary>
        /// Places immediately above role in role menu. Exceptions may be thrown at compile time.
        /// </summary>
        public RoleBuilder AboveRole(string roleName) {
            lowerRole = roleName;
            return this;
        }

        /// <summary>
        /// If several entries exist between two roles, it will be placed alphabetically. Checked after
        /// build, so exceptions may be thrown if role does not exist or upper < lower.
        /// </summary>
        public RoleBuilder BetweenRoles(string upperRole, string lowerRole) {
            this.upperRole = upperRole;
            this.lowerRole = lowerRole;
            return this;
        }

        public RoleBuilder HoistRole(bool isHoisted) {
            this.isHoisted = isHoisted;
            return this;
        }


        private int GetIndexForRelativePos() {
            //nested
            var guildRole = guild.Roles;
            var guildRoles = guildRole.OrderByDescending(w => w.Position);
            var range = Tuple.Create(0, guildRoles.Count()); //default: lowest to max insertable index
            int pos = 0;
            if (upperRole != "") {
                try {
                    pos = GetRolePos(upperRole);
                } catch (Exception e) { throw e; }
                range = Tuple.Create(pos, range.Item2);
            }
            if (lowerRole != "") {
                try {
                    pos = GetRolePos(lowerRole);
                } catch (Exception e) { throw e; }
                range = Tuple.Create(range.Item1, pos);
                if (range.Item1 >= range.Item2) {
                    throw new Exception("A role between " + upperRole + " and " + lowerRole +
                        " would be impossible, because the upper role is at index " + range.Item1 +
                        " which is greater than or equal to the lower role at index " + range.Item2);
                }
            }

            if (range.Item1 + 1 == range.Item2) { //exact spot, put between two roles
                //push lowerRole down one index, and replace its current position with new Role
                return range.Item2;
            }
            //range is (x,y) s.t. x < y by at least 2 entries. Place in the next lexicographically available spot.
            for (int i = range.Item1; i < guildRoles.Count() && (i != range.Item2); ++i) {
                if (String.Compare(this.name, guildRoles.ElementAt(i).Name) <= 0) {
                    //we have found the first element that is lexicographically later than insert.
                    //replace the later element's index with insert's, and push rest down.
                    return i;
                }
            }
            return range.Item2; //place at index of upper bound
        }

        private int GetRolePos(string name) {
            var guildRole = guild.Roles;
            var guildRoles = guildRole.OrderByDescending(w => w.Position);
            int count = guildRoles.Count();

            for (int i = 0; i < count; ++i) {
                if (guildRoles.ElementAt(i).Name == name) {
                    return i;
                }
            }
            throw new Exception("Role **" + name + "** does not exist.");
        }

        private void GeneratePermRawValue() {
            if(permissions == null) {
                return;
            }
            if (permissions.Count() == 0) {
                return;
            }
            foreach (var currentPerm in permissions) {
                this.permVal |= (ulong)currentPerm;
            }
            return;
        }
    }
}
