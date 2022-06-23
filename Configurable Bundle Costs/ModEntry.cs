using StardewModdingAPI;
using StardewValley;
using System;

namespace ConfigurableBundleCosts
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{
		private AssetEditor modAssetEditor;

		/// <summary>The mod entry point.</summary>
		/// <param name="helper" />
		public override void Entry(IModHelper helper)
		{
			DeclareGlobals(helper);

			modAssetEditor = new AssetEditor();
			helper.Content.AssetEditors.Add(modAssetEditor);

			helper.Events.GameLoop.GameLaunched += (sender, args) =>
			{
				CheckForContentPatcherAPI();
				ContentPackHelper.RegisterTokens();
				ContentPackHelper.CheckForValidContentPacks();
				ModConfigMenuHelper.TryLoadModConfigMenu();
				LoadAssets();
			};
			helper.Events.GameLoop.SaveLoaded += (sender, args) =>
			{
				ContentPackHelper.ReloadContentPacks();
				ContentPackHelper.ProcessConfigOverrides();
				CheckBundleData();
			};
			helper.Events.GameLoop.DayStarted += (sender, args) =>
			{
				ContentPackHelper.ProcessDailyUpdates();
				CheckBundleData();
			};

			AddConsoleCommands();

			ApplyPatches();
		}

		private void ApplyPatches()
		{
			Monitor.Log(HarmonyPatches.ApplyHarmonyPatches() ? "Patches successfully applied" : "Failed to apply patches");
		}
		private void LoadAssets()
		{
			Monitor.Log(AssetEditor.LoadAssets() ? "Loaded assets" : "Failed to load assets");
		}

		private void CheckBundleData()
		{
			try
			{
				Game1.netWorldState?.Value?.SetBundleData(AssetEditor.bundleData);
			}
			catch (Exception ex)
			{
				Monitor.Log($"Exception encountered while updating bundle data in {nameof(CheckBundleData)}: {ex}", LogLevel.Error);
			}
		}

		private void CheckForContentPatcherAPI()
		{
			Monitor.Log(ContentPackHelper.TryLoadContentPatcherAPI() ? "Content Patcher API loaded" : "Failed to load Content Patcher API - ignoring extended content pack functionality");
		}

		private void AddConsoleCommands()
		{
			Globals.Helper.ConsoleCommands.Add("cbc_dump", "Dumps the current config values from Configurable Bundle Costs", (name, arg) => Globals.Monitor.Log(Globals.CurrentValues.ToString()));

			Globals.Helper.ConsoleCommands.Add("cbc_reload_packs", "Reloads the internal list of content packs", (name, arg) => ContentPackHelper.ReloadContentPacks());
			Globals.Helper.ConsoleCommands.Add("cbc_list_packs", "Lists the currently loaded content packs", (name, arg) =>
				{
					foreach (ContentPackData data in ContentPackHelper.GetContentPackList()) Globals.Monitor.Log(data.GetFolderName());
				}
			);
			Globals.Helper.ConsoleCommands.Add("cbc_reload_patches", "Reloads the internal list of patches", (name, arg) => ContentPackHelper.ReloadContentPacks(forcePatchReload: true));
			Globals.Helper.ConsoleCommands.Add("cbc_list_patches", "Lists the currently parsed patches", (name, arg) =>
				{
					foreach (ContentPackItem patch in ContentPackHelper.GetPatchList()) Globals.Monitor.Log(patch.Name);
				}
			);

			Globals.Helper.ConsoleCommands.Add("cbc_reload_config", "Forces the current config values to be reloaded. Note that this will overwrite any changes which have occurred on this save's config file." +
				"This command will have no effect outside of a loaded save.",
				(name, arg) =>
				{
					if (Context.IsWorldReady)
					{
						Globals.Monitor.Log("Reloading config values from file."); 
						ContentPackHelper.ProcessConfigOverrides(forceReload: true);
					}
					else
					{
						Globals.Monitor.Log("This console command should only be used inside a loaded save.");
					}
				}
			);
		}

		private void DeclareGlobals(IModHelper helper)
		{
			Globals.InitialValues = helper.ReadConfig<ModConfig>();
			Globals.CurrentValues = new ModConfig();
			Globals.Helper = helper;
			Globals.Monitor = Monitor;
			Globals.Manifest = ModManifest;
			Globals.PackHelper = new ContentPackHelper();
		}
	}
}
