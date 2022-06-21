using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace CarWarp
{
	internal class AssetLoader : IAssetLoader
	{
		/// <summary>
		/// Only allows loading if the asset being loaded is sophie.ConversationTopics
		/// </summary>
		public bool CanLoad<T>(IAssetInfo asset)
		{
			return asset.Name.IsEquivalentTo(Globals.WarpLocationsContentPath);
		}

		/// <summary>
		/// Creates default Dictionary<string, string> when the asset is loaded (before it is edited by any Content Patcher patches).
		/// </summary>
		public T Load<T>(IAssetInfo asset)
		{
			if (asset.Name.IsEquivalentTo(Globals.WarpLocationsContentPath))
			{
				return (T)(object)new Dictionary<string, WarpLocationModel>();
			}

			return default;
		}
	}
}