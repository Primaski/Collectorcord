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

namespace Collectorcord.Server.General {
    public class Admin  {

        public static bool GiveSnipePass(ulong userID) {
            string result = Query("UPDATE Users u set u.permSnipe = true WHERE u.userID = " + userID.ToString());
            if(result == "RecordsAffected: 1") {
                return true;
            }
            return false;
        }

        public static string Killswitchactivate(string args) {
            if (args == null) {
                string u =(Killswitch.GetHelpMenu() + "\n" + "The current state of the Killswitch is: `" + Killswitch.GetState() + "`.");
                return u;
            }
            args = args.ToLower();
            if (args.Contains("off")) {
                Killswitch.SetStatus(Killswitch.Status.Off);
            } else if (args.Contains("mute")) {
                Killswitch.SetStatus(Killswitch.Status.Mute);
            } else if (args.Contains("kick")) {
                Killswitch.SetStatus(Killswitch.Status.Kick);
            } else if (args.Contains("ban")) {
                Killswitch.SetStatus(Killswitch.Status.Ban);
            }
            string w = ("The current state of the Killswitch is: `" + Killswitch.GetState() + "`.");
            return w;
        }

        public static string Query(string query) {
            if (query == null || query == "") {
                return "Argument `query` cannot be null.";
            }
            DataTable dt = Database.Query(query);
            string returning = dt.Rows[0]?.ItemArray[0]?.ToString() ?? "null";
            return (returning);
        }

        public static string Restart() {
            Console.WriteLine("`Restarting...`");
            Database.DisconnectFromServer();
            Program.Restart();
            return "`Restarting...`";
        }

        public static string Exit() {
            Console.WriteLine("`Exiting...`");
            Database.DisconnectFromServer();
            // Closes the current process
            Program.Exit();
            return "`Exiting...`";
        }
    }
}
