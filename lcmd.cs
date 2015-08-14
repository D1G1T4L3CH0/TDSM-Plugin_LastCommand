using TDSM.API;
using TDSM.API.Command;
using TDSM.API.Plugin;
using TDSM.API.Callbacks;
using Terraria;
using System.Collections.Generic;

namespace D1G1T4L3CH0
{
    public class LastCommand : BasePlugin
    {
		// Create our dictionaries for later usage.
		Dictionary<string, string> history =
			new Dictionary<string, string>(); // command history
		
        public LastCommand()
        {
            this.TDSMBuild = 5;
            this.Version = "1.0";
            this.Author = "D1G1T4L3CH0";
            this.Name = "Last Command";
            this.Description = "Re-type that last command with ease!";
        }

        protected override void Initialized(object state)
        {
            AddCommand("!")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Re-types the last command you entered.")
                .WithPermissionNode("lcmd.issue")
                .Calls(IssueLastCommand);
        }

        void IssueLastCommand( ISender sender, ArgumentList args )
		{
			// If the sender has already saved a commmand...
			if ( history.ContainsKey( sender.SenderName ) ) {
				string commandStr = history[ sender.SenderName ];
				
				// Execute the command as the sender.
				Player playerObj = sender as Player;
				if ( playerObj != null ) {
					UserInput.CommandParser.ParsePlayerCommand( playerObj, "/" + commandStr );
				} else if ( sender is ConsoleSender ) {
					UserInput.CommandParser.ParseConsoleCommand( commandStr );
				}
			
			} else { // There is no command history for the sender.
				throw new CommandError ("This can't be your first command. :p");
			}
		}

        [Hook(HookOrder.NORMAL)]
        void CommmandHook(ref HookContext ctx, ref HookArgs.Command args)
        {
			// If the command issued is not !...
			if ( args.Prefix != "!" ) {
				string WhoCalled = ctx.Sender.SenderName; // get the name of the sender
				// If there is a command saved for the sender already...
				if ( history.ContainsKey( WhoCalled ) ) {
					history[WhoCalled] = args.Prefix + " " + args.ArgumentString; // Replace the sender's last command.
				} else {
					history.Add( WhoCalled, args.Prefix + " " + args.ArgumentString ); // Record the sender's command for the first time.
				}
			}
        }
        [Hook(HookOrder.NORMAL)]
        void LeftHook(ref HookContext ctx, ref HookArgs.PlayerLeftGame args)
        {
			// If player has a last command stored, remove it now.
			if ( history.ContainsKey(ctx.Sender.SenderName) ) {
				history.Remove(ctx.Sender.SenderName);
			}
        }
    }
}
