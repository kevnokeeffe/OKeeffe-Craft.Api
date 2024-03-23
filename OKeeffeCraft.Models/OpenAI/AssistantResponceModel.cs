using System.Text.Json.Serialization;

namespace OKeeffeCraft.Models.OpenAI
{

    public class AssistantResponceModel
    {
        public required object Root { get; set; }
    }

    public class Root
    {
        public required Data data { get; set; }
        public required bool success { get; set; }
        public required string message { get; set; }

    }

    public class Data
    {
        [JsonPropertyName("object")]
        public required string ObjectType { get; set; }
        public required List<Assistant> data { get; set; }
        public bool has_more { get; set; }
        public required string first_id { get; set; }
        public required string last_id { get; set; }
    }

    public class Assistant
    {
        [JsonPropertyName("object")]
        public required string ObjectType { get; set; }
        public required string id { get; set; }
        public required int created_at { get; set; }
        public required string name { get; set; }
        public required object description { get; set; }
        public required string model { get; set; }
        public required string instructions { get; set; }
        public required List<Tool> tools { get; set; }
        public required List<object> file_ids { get; set; }
        public required Metadata metadata { get; set; }
    }

    public class Tool
    {
        public required object id { get; set; }
        public required string type { get; set; }
    }

    public class Metadata
    {
    }
}
