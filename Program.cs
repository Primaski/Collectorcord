using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Net;
using Discord.Rest;
using System.Threading;
using static Collectorcord.Server.Global_Variables;
using Collectorcord;
using Primbot_v._2.Server;

namespace Collectorcord {
    public class Program {
        private static string AUTH = "";
        private static DiscordSocketClient CLIENT;
        private static CommandService COMMANDS;
        private static IServiceProvider SERVICES;
        public static Program self;

        static void Main(string[] args) {
            Database.ConnectToServer();
            int iterations = 1;
            int sleepTime = 5000;
            while (true) {
                try {
                    self = new Program();
                    self.RunBotAsync().GetAwaiter().GetResult();
                } catch (Exception e) {
                    int retryTime = sleepTime * iterations;
                    if (iterations == 1) {
                        Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " --> " + e.Message);
                    }
                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " --> " + iterations + " of 6 attempts were made to connect. Retrying in " + retryTime / 1000 + " seconds...");
                    if (iterations > 6) {
                        Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " --> " + iterations + " of 6 attempts were made to connect. Discontinuing...");
                        return;
                    }
                    Thread.Sleep(retryTime);
                    ++iterations;
                }
                //AppDomain.CurrentDomain.ProcessExit += new System.EventHandler(DoThisFirst);
            }

        }

        public async Task RunBotAsync() {
            CLIENT = new DiscordSocketClient(new DiscordSocketConfig {
                AlwaysDownloadUsers = true
            });

            COMMANDS = new CommandService();
            SERVICES = new ServiceCollection()
                .AddSingleton(CLIENT)
                .AddSingleton(COMMANDS)
                .BuildServiceProvider();

            CLIENT.Log += Log;
            CLIENT.GuildAvailable += ServerUpdate;
            CLIENT.UserJoined += KillSwitchProtocol;


            await RegisterCommandsAsync();
            AUTH = GetAUTH();
            await CLIENT.LoginAsync(TokenType.Bot, AUTH);
            await CLIENT.StartAsync();
            await CLIENT.SetGameAsync(BOT_PREFIX + "help");
            
            await Task.Delay(-1);

        }

        private Task KillSwitchProtocol(SocketGuildUser user) {

            if (Killswitch.GetState() == Killswitch.Status.Off || user.Guild.Id != 597469488778182656) {
                return Task.CompletedTask;
            } else if (Killswitch.GetState() == Killswitch.Status.Mute) {
                user.AddRoleAsync(user.Guild.GetRole(472806594292482088)); //skipped
            } else if (Killswitch.GetState() == Killswitch.Status.Kick) {
                user.KickAsync();
            } else if (Killswitch.GetState() == Killswitch.Status.Ban) {
                user.Guild.AddBanAsync(user);
            }
            return Task.CompletedTask;
        }

        private Task ServerUpdate(SocketGuild arg) {
            if(arg.Id == POKECOLLECTORS_SERVER_ID) {
                Server.Util.Poke_Cache = arg;
                Console.WriteLine("Connected to " + arg.Name);
            }
            return Task.CompletedTask;
        }

        private string GetAUTH() {
            string path = DIR + "\\Server_Txt\\Key.txt";
            if (!File.Exists(path)) {
                Console.WriteLine("Error in retrieving key. Terminating.");
                Environment.Exit(0);
            }
            using (StreamReader sr = new StreamReader(path)) {
                string res = sr.ReadLine() ?? "";
                if (res != "") {
                    return res;
                }
            }
            Console.WriteLine("Error in retrieving key (value was null). Terminating.");
            Environment.Exit(0);
            return "";
        }


        private Task Log(LogMessage arg) {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }



        public async Task RegisterCommandsAsync() {
            CLIENT.MessageReceived += HandleCommandAsync;
            await COMMANDS.AddModulesAsync(Assembly.GetEntryAssembly(), SERVICES);

        }



        private async Task HandleCommandAsync(SocketMessage arg) {
            var message = (SocketUserMessage)arg;



            var context = new SocketCommandContext(CLIENT, message);

            int argPos = 0;



            //if (GuildCache.SearchAwaitedMessage(context.Guild.Id, context.Channel.Id, context.User.Id, context.Message.Content) != -1) {}

            if ((message.HasStringPrefix(BOT_PREFIX, ref argPos)) ||
                message.HasMentionPrefix(CLIENT.CurrentUser, ref argPos)) {
                var result = await COMMANDS.ExecuteAsync(context, argPos, SERVICES);

                if (!result.IsSuccess) {
                    Console.WriteLine(result.ErrorReason);
                    await context.Channel.SendMessageAsync(":exclamation: **Error:** " + result.ErrorReason);
                }
            }
            return;
        }

        public static void Exit() {
            Environment.Exit(0);
        }

        public static void Restart() {
            // Starts a new instance of the program itself
            var fileName = Assembly.GetExecutingAssembly().Location;
            System.Diagnostics.Process.Start(fileName);
            // Closes the current process
            Environment.Exit(0);
            return;
        }
    }
}
