using StardewModdingAPI;

namespace ConfigurableBundleCosts
{
	internal class Globals
	{
		public static IManifest Manifest { get; set; }
		public static ModConfig InitialValues { get; set; }
		public static ModConfig CurrentValues { get; set; }
		public static ContentPackConfig Override { get; set; }
		public static IModHelper Helper { get; set; }
		public static IMonitor Monitor { get; set; }
		public static ContentPackHelper PackHelper { get; set; }
	}
}
