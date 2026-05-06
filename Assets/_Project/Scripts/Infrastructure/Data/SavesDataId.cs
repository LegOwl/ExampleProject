namespace Saves
{
    public static class SavesDataId
    {
        // Save Profile
        public static readonly string IslandProfile = "ISLAND_SAVES_UPD";
        public static readonly string SlimesProfile = "SLIMES_SAVES";
        public static readonly string GuessWhoProfile = "GUESSWHO_SAVES";
        public static readonly string FindTheBrainrotsProfile = "FINDTHEBRAINROTS_SAVES";
        public static readonly string Obby1Profile = "ObbyPlatforming1Mode_SAVES";
        public static readonly string Obby2Profile = "ObbyPlatforming2Mode_SAVES";

        // Player Data
        public static readonly string GlobalUserData = "GlobalUserData";
        public static readonly string PlayerUserData = "Island_PlayerUserData";
        public static readonly string GuessWhoUserData = "GuessWho_PlayerUserData";
        public static readonly string FindTheBrainrotUserData = "FindTheBrainrot_PlayerUserData";
        public static readonly string PlaneUserData = "Plane_PlayerUserData";
        public static readonly string PlaneBuilderUserData = "PlaneBuilder_UserData";
        public static readonly string ClickerUserData = "Clicker_PlayerUserData";
        public static readonly string SlimesPlayerStatsData = "Slimes_PlayerStats";
        public static readonly string PlayerInventoryData = "PlayerInventory";
        public static readonly string PlayerIAPData = "PlayerIAPData";
        

        // Quests
        public static readonly string IslandQuestsData = "IslandQuests";
        public static readonly string SlimesQuestsData = "SlimesQuests";

        // Other Data
        public static readonly string IdleTimeData = "IdleTimeData";
        public static readonly string GardenBedsData = "GardenBedsData";
        public static readonly string InteractiveSlimeData = "InteractiveSlimeData";
        /// <summary> Single save blob for all interactive slimes (one key, one write). </summary>
        public static readonly string InteractiveSlimesData = "InteractiveSlimesData";
        public static readonly string BasicShopProgressData = "BasicShopProgressData";
        public static readonly string SettingsData = "SettingsData";
        public static readonly string PetsManagerData = "PetsManagerData";
        public static readonly string CollectionsData = "CollectionsData";
        public static readonly string StatuesProgressData = "Island_StatuesProgressData";
        public static readonly string HubPlayerStatueProgressData = "Hub_PlayerStatueProgressData";

        // Analytics
        public static readonly string GameTimeData = "AnaliticsData";
        public static readonly string SellAllProductionData = "SellAllProductionData";

        // Daily Reward
        public static readonly string DailyRewardData = "DailyRewardData";

        // Statue Reward (hub statue level 10 rewards)
        public static readonly string StatueRewardData = "StatueRewardData";
    }
}
