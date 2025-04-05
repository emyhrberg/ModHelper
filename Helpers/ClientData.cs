﻿using System.Text.Json.Serialization;

namespace ModHelper.Helpers
{
    public class ClientDataJson
    {
        public int ProcessID { get; set; }

        // This is needed to serialize enums as strings
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ClientMode ClientMode { get; set; }
        public int PlayerID { get; set; }
        public int WorldID { get; set; }
    }
}
