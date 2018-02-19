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
        [Command("AddToBlacklist")]
        [Aliases("addtoblacklist", "blacklist", "Blacklist", "BlackList")]
        public async Task AddToBlacklist(CommandContext ctx)
        {
            string BlacklistedGuild = System.Configuration.ConfigurationManager.AppSettings["BlacklistedGuild"];
            using (StreamWriter file = new StreamWriter(BlacklistedGuild, true))
            {
                file.WriteLine(ctx.Guild.Id + "," + ctx.Guild.Name);
            }
            await ctx.Message.RespondAsync("Your guild has been added to the blacklist");
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
