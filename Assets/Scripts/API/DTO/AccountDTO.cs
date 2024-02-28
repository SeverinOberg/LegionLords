using Newtonsoft.Json;

namespace LegionLords.API
{
    [System.Serializable]
    public class AccountDTO
    {
        [JsonProperty("id")]
        public int ID;

        [JsonProperty("steam_id")]
        public int SteamID;

        [JsonProperty("updated_at")]
        public string UpdatedAt; 

        [JsonProperty("created_at")]
        public string CreatedAt;

        [JsonProperty("deleted_at")]
        public string DeletedAt;
    }
}

