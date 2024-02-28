using Newtonsoft.Json;

namespace LegionLords.API
{
    [System.Serializable]
    public class LegionModel
    {
        [JsonProperty("id")]
        public int ID;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("is_active")]
        public bool IsActive;

        [JsonProperty("unit_ids")]
        public int[] UnitIDs;
    }

}

