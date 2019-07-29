using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Primbot_v._2;

namespace Primbot_v._2.Server {
    public static class Killswitch {
        public enum Status {
            Off, Mute, Kick, Ban
        }
        private static Status state = Status.Off;
        private static string role;


        public static void SetStatus(Status newState) {
            state = newState;
        }

        public static Status GetState() {
            return state;
        }


        public static string GetHelpMenu() {
            return "Killswitch (used for raids):\n" +
                "`/killswitch off` - turns killswitch off (default)\n" +
                "`/killswitch state` - returns the state of the killswitch" +
                "`/killswitch mute` - mutes all users upon joining\n" +
                "`/killswitch kick` - kicks all users upon joining\n" +
                "`/killswitch ban` - bans all users upon joining\n";
        }
    }
}
