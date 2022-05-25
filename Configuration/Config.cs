using System;
using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;
using SuicidePro.API;
using SuicidePro.Handlers.CustomEffectHandlers;
using SuicidePro.Handlers.CustomEffectHandlers.Effects;
using UnityEngine;

namespace SuicidePro.Configuration
{
	public sealed class Config : IConfig
	{
		public bool IsEnabled { get; set; } = true;

		[Description("例如，kill命令的名称，.kill或您可以将其更改为例如bruh，则命令将为.bruh")]
		public string CommandPrefix { get; set; } = "kill";

		[Description("如果使用其中一个，它仍然会调用kill命令.")]
		public string[] CommandAliases { get; set; } = 
		{
			"die",
			"suicide"
		};

		[Description("如果<如果使用命令前缀/别名><其中任何一个>，它将显示一条有用的消息，显示所有可用的变体。")]
		public string[] HelpCommandAliases { get; set; } =
		{
			"help",
			"all",
			"list"
		};

		[Description("默认kill配置不是特殊的，只是使用速度。您可以通过复制和粘贴来添加自己的")]
		public List<CustomHandlerCommandConfig> KillConfigs { get; set; } = new List<CustomHandlerCommandConfig>
		{
			new CustomHandlerCommandConfig(),
			new CustomHandlerCommandConfig {Name = "fling",Aliases = new[] {"wee"},Description = "Weeeeeeeeeeeeee",Permission = "default", Response = "tripping", DamageHandler = new CustomDamageHandler {Reason = "Tripped!", Velocity = new Velocity(15, 1, 0)}},
			new CustomHandlerCommandConfig {Name = "ascend", Aliases = new[] {"fly"}, Description = "Fly high up in the air.", Permission = "default", Response = "Sent to the next dimension", DamageHandler = new CustomDamageHandler {Reason = "Ascended", Velocity = new Velocity(0, 10, 0)}},
			new CustomHandlerCommandConfig {Name = "flip", Description = "Do a flip!", Permission = "default", Response = "Epic tricks", DamageHandler = new CustomDamageHandler {Reason = "Did a flip", Velocity = new Velocity(1f, 5, 0)}},
			new CustomHandlerCommandConfig {Name = "backflip", Description = "Do a backflip!", Permission = "default", Response = "Epic back tricks", DamageHandler = new CustomDamageHandler {Reason = "Did a backflip", Velocity = new Velocity(-1f, 5, 0)}},
			new CustomHandlerCommandConfig {Name = "???", Description = "I don't even know what this will do.", Permission = "default", Response = "bruh", DamageHandler = new CustomDamageHandler {Reason = "???", Velocity = new Velocity(70, 70, 70)}}
		};

		[Description("爆炸效果的配置。")]
		public Explode ExplodeEffect { get; set; } = new Explode
		{
			Config = new EffectConfig
			{
				Aliases = new[] {"boom"}, Name = "explode", Delay = 0.3f,
				Description = "爆炸（不会造成损坏或损坏车门）", Response = "Boom!",
				DamageHandler = new CustomDamageHandler {Reason = "Boom!", Velocity = new Velocity(2, 0, 0)}
			}
		};
		
		[Description("分解效果的配置。")]
		public Disintegrate DisintegrateEffect { get; set; } = new Disintegrate
		{
			Config = new EffectConfig
			{
				Aliases = new[] {"raygun"}, Name = "disintegrate",
				Description = "摧毁你的身体！", Response = "Disintegrated",
				IgnoreDamageHandlerConfigs = true
			}
		};

		[Description("在控制台中启用调试消息。")]
		public bool Debug { get; set; }

		[Description("您是否仍然能够运行由其开发人员强制注册的禁用效果。")]
		public bool AllowRunningDisabledForceRegistered { get; set; }

		[Description("安乐枪的配置，使用调用。contentgun和需要命令 .give permission")]
		public ContentGunConfigClass ContentGunConfig { get; set; } = new ContentGunConfigClass();

		public class BaseCommandConfig
		{
			/// <summary>
			/// Name of the command in .kill
			/// </summary>
			[Description("将使用的名称，, 比如 .kill 测试 -- 如果这是默认设置，则运行 .kill 将运行此命令。")]
			public string Name { get; set; } = "default";

			/// <summary>
			/// The description of the command in .kill list
			/// </summary>
			[Description("kill列表中的命令描述")]
            public string Description { get; set; } = "默认的kill命令。会要了你的命";

			/// <summary>
			/// The response in the console.
			/// </summary>
			[Description("使用命令后控制台中的响应")]
            public string Response { get; set; } = "寄.";

			/// <summary>
			/// The EXILED permission required for this command.
			/// </summary>
			/// <remarks>If this is none, then none is needed.
			/// If this is default, then the permission will be kl.<see cref="Name"/>
			/// If otherwise, then the permission will be that written.</remarks>
			[Description("The Exiled permission required to use this command. If this is none, then none is needed. If this is default, then it will be automatically kl.command_name. If none of these, then it will take whatever is written as permission (e.g. scpstats.hats)")]
            public string Permission { get; set; } = "none";

			/// <summary>
			/// Other <see cref="string"/>s usable for the command.
			/// </summary>
			[Description("仍将运行该命令的其他名称")]
            public string[] Aliases { get; set; } = Array.Empty<string>();

			/// <summary>
			/// A <see cref="List{T}"/> of <see cref="RoleType"/>, that when the player is a role that is contained within the list, prevents execution of the command.
			/// </summary>
			[Description("不允许使用此命令的角色类型。")]
            public List<RoleType> BannedRoles { get; set; } = new List<RoleType>();

			/// <summary>
			/// <see cref="float"/> in seconds to wait before the <see cref="Exiled.API.Features.Player"/>'s death is applied.
			/// </summary>
			[Description("玩家死亡前等待的秒数。")]
            public float Delay { get; set; }
		}

		public class CustomHandlerCommandConfig : BaseCommandConfig
		{
			[Description("死后处理大部分事情的人。")]
			public CustomDamageHandler DamageHandler { get; set; } = new CustomDamageHandler();
		}

		public class CustomDamageHandler
		{
			/*public CustomDamageHandler(int forwardVelocity, int rightVelocity, int upVelocity)
			{
				ForwardVelocity = forwardVelocity;
				RightVelocity = rightVelocity;
				UpVelocity = upVelocity;
			}*/

			[Description("分解死亡的信息.")]
			public string Reason { get; set; } = "自杀.";

			[Description("C.A.S.S.I.E. announcement if the player is an SCP.")]
			public string CassieIfScp { get; set; } = String.Empty;
			[Description("Velocity (strength, sort of) of the body when killed.")]
			public Velocity Velocity { get; set; } = new Velocity();
		}

		public class ContentGunConfigClass
		{
			[Description("如果 .contentgun 命令开启")]
			public bool Enabled { get; set; }
			[Description("射击时产生的身体速度（力量，种类）。")]
			public Velocity Velocity { get; set; } = new Velocity(10, 1, 0);
			[Description("The ragdoll's name in ragdoll info.")]
			public string RagdollName { get; set; } = "傻逼（翻译如此别骂我）";
			[Description("Death reason in ragdoll info.")]
			public string DeathCause { get; set; } = "寄于安乐枪";
			[Description("The RoleType of the created ragdoll.")]
			public RoleType RagdollRoleType { get; set; } = RoleType.Scientist;
			[Description("The size of the created ragdoll.")]
			public Vector3 Scale { get; set; } = Vector3.one;
			[Description("Cooldown before being able to use .contentgun again.")]
			public int Cooldown { get; set; } = 180;
			[Description("Times you can shoot with the content gun.")]
			public int Uses { get; set; } = 10;
			[Description("The ItemType for the gun given after using .contentgun")]
			public ItemType GunItemType { get; set; } = ItemType.GunCOM15;
			[Description("Time (in seconds) before ragdolls created by content gun are deleted.")]
			public int CleanupTime { get; set; } = 30;
		}
	}
}
