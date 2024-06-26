using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Menu;

namespace CSSPanel.Menus
{
	public static class FunActionsMenu
	{
		private static Dictionary<int, CsItem>? _weaponsCache = null;

		private static Dictionary<int, CsItem> GetWeaponsCache
		{
			get
			{
				if (_weaponsCache == null)
				{
					Array weaponsArray = Enum.GetValues(typeof(CsItem));

					// avoid duplicates in the menu
					_weaponsCache = new();
					foreach (CsItem item in weaponsArray)
					{
						if (item == CsItem.Tablet)
							continue;

						_weaponsCache[(int)item] = item;
					}
				}

				return _weaponsCache;
			}
		}

		public static void OpenMenu(CCSPlayerController admin)
		{
			if (admin == null || admin.IsValid == false)
				return;

			if (AdminManager.PlayerHasPermissions(admin, "@css/generic") == false)
			{
				// TODO: Localize
				admin.PrintToChat("[Simple Admin] You do not have permissions to use this command.");
				return;
			}

			BaseMenu menu = AdminMenu.CreateMenu("Fun Actions");
			List<ChatMenuOptionData> options = new();

			// permissions
			bool hasCheats = AdminManager.PlayerHasPermissions(admin, "@css/cheats");
			bool hasSlay = AdminManager.PlayerHasPermissions(admin, "@css/slay");

			// TODO: Localize options
			// options added in order

			if (hasCheats)
			{
				options.Add(new ChatMenuOptionData("God Mode", () => PlayersMenu.OpenAliveMenu(admin, "God Mode", GodMode)));
				options.Add(new ChatMenuOptionData("No Clip", () => PlayersMenu.OpenAliveMenu(admin, "No Clip", NoClip)));
				options.Add(new ChatMenuOptionData("Respawn", () => PlayersMenu.OpenDeadMenu(admin, "Respawn", Respawn)));
				options.Add(new ChatMenuOptionData("Give Weapon", () => PlayersMenu.OpenAliveMenu(admin, "Give Weapon", GiveWeaponMenu)));
			}

			if (hasSlay)
			{
				options.Add(new ChatMenuOptionData("Strip All Weapons", () => PlayersMenu.OpenAliveMenu(admin, "Strip All Weapons", StripWeapons)));
				options.Add(new ChatMenuOptionData("Freeze", () => PlayersMenu.OpenAliveMenu(admin, "Freeze", Freeze)));
				options.Add(new ChatMenuOptionData("HP", () => PlayersMenu.OpenAliveMenu(admin, "HP", SetHpMenu)));
				options.Add(new ChatMenuOptionData("Speed", () => PlayersMenu.OpenAliveMenu(admin, "Speed", SetSpeedMenu)));
				options.Add(new ChatMenuOptionData("Gravity", () => PlayersMenu.OpenAliveMenu(admin, "Gravity", SetGravityMenu)));
				options.Add(new ChatMenuOptionData("Set Money", () => PlayersMenu.OpenMenu(admin, "Set Money", SetMoneyMenu)));
			}

			foreach (ChatMenuOptionData menuOptionData in options)
			{
				string menuName = menuOptionData.name;
				menu.AddMenuOption(menuName, (_, _) => { menuOptionData.action?.Invoke(); }, menuOptionData.disabled);
			}

			AdminMenu.OpenMenu(admin, menu);
		}

		private static void GodMode(CCSPlayerController admin, CCSPlayerController player)
		{
			CSSPanel.Instance.God(admin, player);
		}

		private static void NoClip(CCSPlayerController admin, CCSPlayerController player)
		{
			CSSPanel.Instance.NoClip(admin, player);
		}

		private static void Respawn(CCSPlayerController admin, CCSPlayerController player)
		{
			CSSPanel.Instance.Respawn(admin, player);
		}

		private static void GiveWeaponMenu(CCSPlayerController admin, CCSPlayerController player)
		{
			BaseMenu menu = AdminMenu.CreateMenu($"Give Weapon: {player.PlayerName}");

			foreach (KeyValuePair<int, CsItem> weapon in GetWeaponsCache)
			{
				menu.AddMenuOption(weapon.Value.ToString(), (_, _) => { GiveWeapon(admin, player, weapon.Value); });
			}

			AdminMenu.OpenMenu(admin, menu);
		}

		private static void GiveWeapon(CCSPlayerController admin, CCSPlayerController player, CsItem weaponValue)
		{
			CSSPanel.Instance.GiveWeapon(admin, player, weaponValue);
		}

		private static void StripWeapons(CCSPlayerController admin, CCSPlayerController player)
		{
			CSSPanel.Instance.StripWeapons(admin, player);
		}

		private static void Freeze(CCSPlayerController admin, CCSPlayerController player)
		{
			if (!(player.PlayerPawn?.Value?.IsValid ?? false))
				return;

			if (player.PlayerPawn.Value.MoveType != MoveType_t.MOVETYPE_OBSOLETE)
				CSSPanel.Instance.Freeze(admin, player, -1);
			else
				CSSPanel.Instance.Unfreeze(admin, player);
		}

		private static void SetHpMenu(CCSPlayerController admin, CCSPlayerController player)
		{
			Tuple<string, int>[] _hpArray = new[]
			{
				new Tuple<string, int>("1", 1),
				new Tuple<string, int>("10", 10),
				new Tuple<string, int>("25", 25),
				new Tuple<string, int>("50", 50),
				new Tuple<string, int>("100", 100),
				new Tuple<string, int>("200", 200),
				new Tuple<string, int>("500", 500),
				new Tuple<string, int>("999", 999)
			};

			BaseMenu menu = AdminMenu.CreateMenu($"Set HP: {player.PlayerName}");

			foreach (Tuple<string, int> hpTuple in _hpArray)
			{
				string optionName = hpTuple.Item1;
				menu.AddMenuOption(optionName, (_, _) => { SetHP(admin, player, hpTuple.Item2); });
			}

			AdminMenu.OpenMenu(admin, menu);
		}

		private static void SetHP(CCSPlayerController admin, CCSPlayerController player, int hp)
		{
			CSSPanel.Instance.SetHp(admin, player, hp);
		}

		private static void SetSpeedMenu(CCSPlayerController admin, CCSPlayerController player)
		{
			Tuple<string, float>[] _speedArray = new[]
			{
				new Tuple<string, float>("0.1", .1f),
				new Tuple<string, float>("0.25", .25f),
				new Tuple<string, float>("0.5", .5f),
				new Tuple<string, float>("0.75", .75f),
				new Tuple<string, float>("1", 1),
				new Tuple<string, float>("2", 2),
				new Tuple<string, float>("3", 3),
				new Tuple<string, float>("4", 4)
			};

			BaseMenu menu = AdminMenu.CreateMenu($"Set Speed: {player.PlayerName}");

			foreach (Tuple<string, float> speedTuple in _speedArray)
			{
				string optionName = speedTuple.Item1;
				menu.AddMenuOption(optionName, (_, _) => { SetSpeed(admin, player, speedTuple.Item2); });
			}

			AdminMenu.OpenMenu(admin, menu);
		}

		private static void SetSpeed(CCSPlayerController admin, CCSPlayerController player, float speed)
		{
			CSSPanel.Instance.SetSpeed(admin, player, speed);
		}


		private static void SetGravityMenu(CCSPlayerController admin, CCSPlayerController player)
		{
			Tuple<string, float>[] _gravityArray = new[]
			{
				new Tuple<string, float>("0.1", .1f),
				new Tuple<string, float>("0.25", .25f),
				new Tuple<string, float>("0.5", .5f),
				new Tuple<string, float>("0.75", .75f),
				new Tuple<string, float>("1", 1),
				new Tuple<string, float>("2", 2)
			};

			BaseMenu menu = AdminMenu.CreateMenu($"Set Gravity: {player.PlayerName}");

			foreach (Tuple<string, float> gravityTuple in _gravityArray)
			{
				string optionName = gravityTuple.Item1;
				menu.AddMenuOption(optionName, (_, _) => { SetGravity(admin, player, gravityTuple.Item2); });
			}

			AdminMenu.OpenMenu(admin, menu);
		}

		private static void SetGravity(CCSPlayerController admin, CCSPlayerController player, float gravity)
		{
			CSSPanel.Instance.SetGravity(admin, player, gravity);
		}

		private static void SetMoneyMenu(CCSPlayerController admin, CCSPlayerController player)
		{
			Tuple<string, int>[] _moneyArray = new[]
			{
				new Tuple<string, int>("$0", 0),
				new Tuple<string, int>("$1000", 1000),
				new Tuple<string, int>("$2500", 2500),
				new Tuple<string, int>("$5000", 5000),
				new Tuple<string, int>("$10000", 10000),
				new Tuple<string, int>("$16000", 16000)
			};

			BaseMenu menu = AdminMenu.CreateMenu($"Set Money: {player.PlayerName}");

			foreach (Tuple<string, int> moneyTuple in _moneyArray)
			{
				string optionName = moneyTuple.Item1;
				menu.AddMenuOption(optionName, (_, _) => { SetMoney(admin, player, moneyTuple.Item2); });
			}

			AdminMenu.OpenMenu(admin, menu);
		}

		private static void SetMoney(CCSPlayerController admin, CCSPlayerController player, int money)
		{
			CSSPanel.Instance.SetMoney(admin, player, money);
		}
	}
}