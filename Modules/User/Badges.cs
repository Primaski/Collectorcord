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
    public class Badges : ModuleBase<SocketCommandContext> {
        [Command("snipepass")]
        public async Task SnipePass([Remainder] string grant = "") {
            
            if (grant == "") {
                //show off
                if (Database.ValueExists("Users", "userID", Context.User.Id.ToString())) {
                    if (Database.GetValueByPrimaryKey("Users", "snipePass", "userID",
                        Context.User.Id.ToString()).ToString() == "True") {
                        string temp = "Temporary";
                        if (Database.GetValueByPrimaryKey("Users", "permSnipe", "userID",
                        Context.User.Id.ToString()).ToString() == "True") {
                            temp = "Permanent";
                        }
                        await ReplyAsync("", false, new EmbedBuilder().WithColor(Color.Purple)
                            .WithImageUrl("https://cdn.discordapp.com/attachments/597770942189535243/603670730412130334/snipepass.jpg")
                            .WithTitle(Context.User.Username + "'s Snipe Pass")
                            .WithDescription("This is " + Context.User.Username + "'s `" + temp + " Snipe Pass`! While held, they are legally " +
                            "allowed to snipe you (that being, catch a Pokémon in a channel they were not originally chatting in).").Build());
                        return;
                    } else {
                        await ReplyAsync("You don't have a snipe pass. You can't snipe. <:sobblewhaa:593508446771478544>"); return;
                    }
                } else {
                    await ReplyAsync("You haven't made an account yet in Pokécollectors! Do `" + Global_Variables.BOT_PREFIX +
                        "` to make one! Need an invite to the server? Try " + Global_Variables.BOT_PREFIX + "info`");
                }
            }
            await ReplyAsync("Prim hasn't coded a granting feature yet. Ask him to do it manually in the database.");
        }
    }
}
