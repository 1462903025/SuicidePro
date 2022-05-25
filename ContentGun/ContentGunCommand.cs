using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Permissions.Extensions;

namespace SuicidePro.ContentGun
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ContentGunCommand : ICommand
    {
        public string Command { get; } = "contentgun";
        public string[] Aliases { get; } = {"cg", "givecontentgun", "givecontent", "cgun", "givecg", "givecgun"};
        public string Description { get; } = "Gives you the content gun.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Plugin.Instance.Config.ContentGunConfig.Enabled)
            {
                response = "安乐枪未启用。";
                return false;
            }

            var player = Player.Get(sender);
            if (player == null)
            {
                response = "您不能在服务器用此命令。";
                return false;
            }

            if (!player.CheckPermission("cg.give"))
            {
                response = "您没有使用内容枪的权限。";
                return false;
            }

            if (TryFind(Handler.Cooldowns, x => x.UserId == player.UserId, out var cooldown))
            {
                var value = (DateTime.Now - cooldown.DeletedAt).TotalSeconds;
                if (value <= Plugin.Instance.Config.ContentGunConfig.Cooldown && cooldown.UsesLeft <= 0)
                {
                    player.ShowHint($"需等待<b><color=red>{value}</color>秒</b> 才能再次使用 <b><color=red>安乐枪</color></b>.");
                    response = $"你必须等待 {value}秒 才能使用安乐枪.";
                    return false;
                }
            }

            foreach (var item in player.Items)
            {
                if (Handler.ContentGuns.Contains(item.Base))
                {
                    player.ShowHint("你<b>已经</b> 有<b><color=red>安乐枪</color></b>.");
                    response = "你已经有一把安乐枪了。";
                    return false;
                }
            }

            if (cooldown == null)
                Handler.Cooldowns.Add(new ContentGunCooldown(player.UserId));

            var cg = player.AddItem(Plugin.Instance.Config.ContentGunConfig.GunItemType) as Firearm;
            Handler.ContentGuns.Add(cg.Base);
            cg.Ammo = (byte)Plugin.Instance.Config.ContentGunConfig.Uses;

            player.ShowHint("You <b>now</b> have a <b><color=red>Content Gun</color></b>.");
            response = "给了你一把安乐枪";
            return true;
        }

        public bool TryFind<T>(IEnumerable<T> enumerable, Func<T, bool> func, out T item)
        {
            item = enumerable.FirstOrDefault(func);
            if (item == null)
                return false;

            return true;
        }
    }
}