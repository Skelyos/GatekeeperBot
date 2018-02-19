using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Configuration;
using System.IO;

namespace GatekeeperBot
{
    public class MyCommands
    {
        string Blacklist = System.Configuration.ConfigurationManager.AppSettings["Blacklist"];
        string BlacklistedGuild = System.Configuration.ConfigurationManager.AppSettings["BlacklistedGuild"];

        [Group("Admin")]
        [RequirePermissions(Permissions.ManageMessages)]
        public class CRUD
        {
            string Blacklist = System.Configuration.ConfigurationManager.AppSettings["Blacklist"];
            string BlacklistedGuild = System.Configuration.ConfigurationManager.AppSettings["BlacklistedGuild"];

            public void UserUpdate(CommandContext f, string remove)
            {
                List<String> lines = new List<String>();
                using (StreamReader reader = new StreamReader(Blacklist + "-" + f.Guild.Id + ".txt"))
                {
                    String line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains(remove))
                        {
                            line = "";
                        }
                        lines.Add(line);
                    }
                }

                using (StreamWriter writer = new StreamWriter(Blacklist + "-" + f.Guild.Id + ".txt", false))
                {
                    foreach (String line in lines)
                        writer.WriteLine(line);
                }
            }

            [Command("AddWord")]
            [Aliases("addWord", "add", "Add", "Word", "word")]
            public async Task AddWord(CommandContext ctx, string BlackListedWord)
            {
                using (StreamWriter file = new StreamWriter(Blacklist + "-" + ctx.Guild.Id + ".txt", true))
                {
                    file.WriteLine(BlackListedWord);
                }
                await ctx.Message.RespondAsync($"{BlackListedWord} has been added to the blacklist");
            }

            [Command("RemoveWord")]
            [Aliases("removeWord", "remove", "Remove")]
            public async Task RemoveWord(CommandContext ctx, string BlackListedWord)
            {
                UserUpdate(ctx, BlackListedWord);
                await ctx.Message.RespondAsync($"{BlackListedWord} has been removed");
            }

            [Command("ViewWords")]
            [Aliases("viewWords", "View", "view")]
            public async Task ViewWords(CommandContext ctx)
            {
                string SingleLineRow = "";
                using (StreamReader sr = new StreamReader(Blacklist + "-" + ctx.Guild.Id + ".txt"))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        SingleLineRow += line.Replace(',', ' ') + "\n";
                    }
                }
                await ctx.Message.RespondAsync($"``` {SingleLineRow} ```");
            }

            [Command("AddToBlacklist")]
            [Aliases("addtoblacklist", "blacklist", "Blacklist", "BlackList")]
            public async Task AddToBlacklist(CommandContext ctx)
            {
                using (StreamWriter file = new StreamWriter(BlacklistedGuild, true))
                {
                    file.WriteLine(ctx.Guild.Id + "," + ctx.Guild.Name);
                }
                await ctx.Message.RespondAsync("Your guild has been added to the blacklist");
            }
        }

        [Command("Invite")]
        [Aliases("invite")]
        public async Task Invite(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync($"{ctx.User.Mention} \n https://discordapp.com/oauth2/authorize?client_id=415263314193547267&scope=bot&permissions=3072");
        }
    }
}
