using System;
using System.Linq;
using System.Text;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using RemoteAdmin;
using SuicidePro.Configuration;
using Log = Exiled.API.Features.Log;

namespace SuicidePro.Handlers
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class KillCommand : ICommand
	{
		public string Command { get; } = Plugin.Instance.Config.CommandPrefix;
		public string[] Aliases { get; } = Plugin.Instance.Config.CommandAliases;
		public string Description { get; } = "具有更多功能的kill命令。";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			
			PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;
			if (playerCommandSender == null)
			{
				response = "您必须作为客户端而不是服务器运行此命令。";
				return false;
			}

			string arg = arguments.FirstOrDefault();
			Player player = Player.Get(playerCommandSender);

			if (Plugin.Instance.Config.HelpCommandAliases.Contains(arg))
			{
				var build = new StringBuilder("以下是您可以使用的所有kill命令:\n\n");
				foreach (var commandConfig in Plugin.Instance.Config.KillConfigs)
				{
					if (commandConfig.Permission == "none" || player.CheckPermission(FormatPermission(commandConfig)))
						build.Append($"<b><color=white>.{Plugin.Instance.Config.CommandPrefix}</color> <color=yellow>{commandConfig.Name}</color></b> {(commandConfig.Aliases.Any() ? $"<color=#3C3C3C>({String.Join(", ", commandConfig.Aliases)})</color>" : String.Empty)}\n<color=white>{commandConfig.Description}</color>\n\n");
				}

				foreach (var effect in API.CustomEffect.Effects)
				{
					if (effect.Config.Enabled && (effect.Config.Permission == "none" || player.CheckPermission(FormatPermission(effect.Config))))
						build.Append($"<b><color=white>.{Plugin.Instance.Config.CommandPrefix}</color> <color=yellow>{effect.Config.Name}</color></b> {(effect.Config.Aliases.Any() ? $"<color=#3C3C3C>({String.Join(", ", effect.Config.Aliases)})</color>" : String.Empty)}\n<color=white>{effect.Config.Description}</color>\n\n");
				}

				response = build.ToString();
				return true;
			}

			if (arg == null)
				arg = "default";

			Config.BaseCommandConfig config = Plugin.Instance.Config.KillConfigs.FirstOrDefault(x => x.Name == arg || x.Aliases.Contains(arg));
			var customConfig = API.CustomEffect.Effects.FirstOrDefault(x => x.Config.Name == arg || x.Config.Aliases.Contains(arg));

			if (config == null && customConfig == null)
			{
				response = $"找不到任何具有名称或别名的kill命令 {arg}.";
				return false;
			}

			if (customConfig != null)
				config = customConfig.Config;

			if (config.Permission != "none" && !player.CheckPermission(FormatPermission(config)))
			{
				response = "您没有此命令所需的权限";
				return false;
			}
	
			if (!Round.IsStarted)
			{
				response = "回合尚未开始";
				return false;
			}

			if (config.BannedRoles.Contains(player.Role) || player.IsDead)
			{
				response = "您不能以您的角色运行此kill变体";
				return false;
			}

			if (customConfig == null)
			{
				((Config.CustomHandlerCommandConfig) config).Run(player);
			}
			else
			{
				var ans = customConfig.Run(player);
				if (!ans && !Plugin.Instance.Config.AllowRunningDisabledForceRegistered)
				{
					response = "此效果已禁用。";
					return false;
				}
			}

			response = config.Response;
			return true;
		}

		public string FormatPermission(Config.BaseCommandConfig config)
		{
			if (config.Permission == "default")
			{
				Log.Debug("权限名称为 default ，返回kl。" + config.Name, Plugin.Instance.Config.Debug);
				return $"kl.{config.Name}";
			}
			Log.Debug("权限名称不是 default ，正在返回 " + config.Permission, Plugin.Instance.Config.Debug);
			return config.Permission;
		}
	}
}
