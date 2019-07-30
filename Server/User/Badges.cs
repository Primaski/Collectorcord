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

namespace Collectorcord.Server.User {
    public class Badges {

        public static Embed SnipePass(SocketGuildUser user, string grant = "") {
            
            if (grant == "") {
                //show off
                if (Database.ValueExists("Users", "userID", user.Id.ToString())) {
                    if (Database.GetValueByPrimaryKey("Users", "snipePass", "userID",
                        user.Id.ToString()).ToString() == "True") {
                        string temp = "Temporary";
                        if (Database.GetValueByPrimaryKey("Users", "permSnipe", "userID",
                        user.Id.ToString()).ToString() == "True") {
                            temp = "Permanent";
                        }
                        return new EmbedBuilder().WithColor(Discord.Color.Purple)
                            .WithImageUrl("https://cdn.discordapp.com/attachments/597770942189535243/603670730412130334/snipepass.jpg")
                            .WithTitle(user.Username + "'s Snipe Pass")
                            .WithDescription("This is " + user.Username + "'s `" + temp + " Snipe Pass`! While held, they are legally " +
                            "allowed to snipe you (that being, catch a Pokémon in a channel they were not originally chatting in).").Build();
                    } else {
                        return new EmbedBuilder().WithTitle("You don't have a snipe pass. You can't snipe. <:sobblewhaa:593508446771478544>").Build();
                    }
                } else {
                    return new EmbedBuilder().WithTitle("You haven't made an account yet in Pokécollectors! Do `" + Global_Variables.BOT_PREFIX +
                        "` to make one! Need an invite to the server? Try " + Global_Variables.BOT_PREFIX + "info`").Build();
                }
            }
            return new EmbedBuilder().WithTitle("Prim hasn't coded a granting feature yet. Ask him to do it manually in the database.").Build();
        }
    }
}
