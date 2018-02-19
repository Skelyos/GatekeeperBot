using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;

namespace GatekeeperBot
{
    class Program
    {
        public DiscordClient Client { get; set; }
        public CommandsNextModule Commands { get; set; }

        string BlacklistedGuild = System.Configuration.ConfigurationManager.AppSettings["BlacklistedGuild"];
        string Blacklist = System.Configuration.ConfigurationManager.AppSettings["Blacklist"];
        

        static void Main(string[] args)
        {
            var prog = new Program();
            prog.MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {
            string BotToken = System.Configuration.ConfigurationManager.AppSettings["BotToken"];
            //Initialize the Discord Client
            Client = new DiscordClient(new DiscordConfiguration
            {
                Token = BotToken,
                TokenType = TokenType.Bot,
                LogLevel = LogLevel.Info,
                UseInternalLogHandler = true,
                AutoReconnect = true
            });

            this.Client.Ready += this.Client_Ready;
            this.Client.Ready += this.SetStatus;
            this.Client.GuildAvailable += this.Client_GuildAvailable;
            this.Client.GuildCreated += this.Client_JoinedGuild;
            this.Client.GuildDeleted += this.Client_RemovedGuild;
            this.Client.ClientErrored += this.Client_ClientError;

            //Set command prefix
            Client.DebugLogger.LogMessage(LogLevel.Info, "Gatekeeper", "Setting command prefix", DateTime.Now);
            var cfg = new CommandsNextConfiguration
            {
                StringPrefix = "//",
                CaseSensitive = false,
                EnableDms = false
            };

            // Checks if your guild is on the blacklist and if the message contains a word on the blacklist
            Client.DebugLogger.LogMessage(LogLevel.Info, "Gatekeeper", "Starting listener for blacklisted words", DateTime.Now);
            Client.MessageCreated += async e =>
            {
                using (StreamReader sr = new StreamReader(BlacklistedGuild))
                {
                    string line;
                    string guildID = Convert.ToString(e.Guild.Id);
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.ToLower().Contains(guildID))
                        {
                            try
                            {
                                using (StreamReader sr1 = new StreamReader(Blacklist + "-" + e.Guild.Id + ".txt"))
                                {
                                    string line1;
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if(line1 != "")
                                        {
                                            if (e.Message.Content.ToLower().Contains(line1.ToLower()))
                                            {
                                                await e.Message.DeleteAsync("Violated blacklist");
                                            }
                                        }
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                }
            };
            Commands = Client.UseCommandsNext(cfg);

            //Check for commands executed and when they error
            this.Commands.CommandExecuted += this.Commands_CommandExecuted;
            this.Commands.CommandErrored += this.Commands_CommandErrored;

            //Enables all commmands
            Client.DebugLogger.LogMessage(LogLevel.Info, "Gatekeeper", "Enabling commmands", DateTime.Now);
            Commands.RegisterCommands<MyCommands>();

            //Connect the bot
            Client.DebugLogger.LogMessage(LogLevel.Info, "Gatekeeper", "Connecting...", DateTime.Now);
            await Client.ConnectAsync();

            //Delay the bot
            await Task.Delay(-1);
        }

        private Task Client_Ready(ReadyEventArgs e)
        {
            // let's log the fact that this event occured
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Gatekeeper", "Client is ready to process events.", DateTime.Now);

            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }

        private Task SetStatus(ReadyEventArgs e)
        {
            DiscordGame discordGame = new DiscordGame("Gatekeeping");
            Client.DebugLogger.LogMessage(LogLevel.Info, "Gatekeeper", "Setting user status", DateTime.Now);
            Client.UpdateStatusAsync(discordGame, UserStatus.Online, DateTimeOffset.UtcNow);

            return Task.CompletedTask;
        }

        private Task Client_GuildAvailable(GuildCreateEventArgs e)
        {
            // let's log the name of the guild that was just
            // sent to our client
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Gatekeeper", $"Guild available: {e.Guild.Name}", DateTime.Now);

            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }

        private Task Client_JoinedGuild(GuildCreateEventArgs e)
        {
            // let's log the name of the guild that was just
            // sent to our client
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Gatekeeper", $"Joined guild: {e.Guild.Name}", DateTime.Now);

            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }

        private Task Client_RemovedGuild(GuildDeleteEventArgs e)
        {
            // let's log the name of the guild that was just
            // sent to our client
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Gatekeeper", $"Removed from guild: {e.Guild.Name}", DateTime.Now);

            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }

        private Task Client_ClientError(ClientErrorEventArgs e)
        {
            // let's log the details of the error that just 
            // occured in our client
            e.Client.DebugLogger.LogMessage(LogLevel.Error, "Gatekeeper", $"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}", DateTime.Now);

            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }

        private Task Commands_CommandExecuted(CommandExecutionEventArgs e)
        {
            // let's log the name of the command and user
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Debug, "Gatekeeper", $"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'", DateTime.Now);

            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }

        private async Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            Console.WriteLine();
            // let's log the error details
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Error, "Gatekeeper", $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}", DateTime.Now);

            // let's check if the error is a result of lack
            // of required permissions
            if (e.Exception is ChecksFailedException ex)
            {
                // yes, the user lacks required permissions, 
                // let them know

                var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");

                // let's wrap the response into an embed
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Access denied",
                    Description = $"{emoji} You do not have the permissions required to execute this command.",
                    Color = new DiscordColor(0xFF0000) // red
                    // there are also some pre-defined colors available
                    // as static members of the DiscordColor struct
                };
                await e.Context.TriggerTypingAsync();
                await e.Context.RespondAsync("", embed: embed);
            }

            else if (e.Exception is CommandNotFoundException)
            {
                var emoji = DiscordEmoji.FromName(e.Context.Client, ":question:");

                var embed = new DiscordEmbedBuilder
                {
                    Title = "Command does not exist",
                    Description = $"{emoji} Use !!Help for more information.",
                    Color = new DiscordColor(0xFF0000) // red
                };
                await e.Context.TriggerTypingAsync();
                await e.Context.RespondAsync("", embed: embed);
            }

            else if (e.Exception is ArgumentException)
            {
                var emoji = DiscordEmoji.FromName(e.Context.Client, ":question:");

                var embed = new DiscordEmbedBuilder
                {
                    Title = "Missing information",
                    Description = $"{emoji} Seems you're missing something from the command you just triggered",
                    Color = new DiscordColor(0xFF0000) // red
                };
                await e.Context.TriggerTypingAsync();
                await e.Context.RespondAsync("", embed: embed);
            }

            else if (e.Exception is IndexOutOfRangeException)
            {
                var emoji = DiscordEmoji.FromName(e.Context.Client, ":question:");

                var embed = new DiscordEmbedBuilder
                {
                    Title = "Out of range values information",
                    Description = $"{emoji} Seems the values you entered are out of the command's range",
                    Color = new DiscordColor(0xFF0000) // red
                };
                await e.Context.TriggerTypingAsync();
                await e.Context.RespondAsync("", embed: embed);
            }

            else if (e.Exception is IOException)
            {
                var emoji = DiscordEmoji.FromName(e.Context.Client, ":sweat_smile:");

                var embed = new DiscordEmbedBuilder
                {
                    Title = "My Bad",
                    Description = $"{emoji} Seems we messed some files up, try again or use !!Credits to message creator",
                    Color = new DiscordColor(0xFF0000) // red
                };
                await e.Context.TriggerTypingAsync();
                await e.Context.RespondAsync("", embed: embed);
            }
        }
    }
}
