using Newtonsoft.Json;

namespace LegionLords.API
{
    [System.Serializable]
    public class AccountModel
    {
        [JsonProperty("id")]
        public int Id;

        [JsonProperty("steam_id")]
        public ulong SteamId;
    }
}

