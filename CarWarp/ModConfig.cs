namespace CarWarp
{
    class ModConfig
    {
        public string Configuration { get; set; }
        public bool SeasonalOverlay { get; set; }

        public ModConfig()
        {
            // options:
            // Right - steering wheel on right side
            // Left - steering wheel on left side
            // None - no steering wheel, only dashboard
            // Empty - no dashboard

            Configuration = "Right";
            SeasonalOverlay = true;
        }
    }
}
