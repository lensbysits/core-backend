using System.Text.Json.Nodes;

namespace Lens.Core;

public class JsonNodeUtilities
{
    public const string EmptyObjectJson = "{}";
    public static JsonNode EmptyObjectNode => JsonNode.Parse(EmptyObjectJson)!;
}
