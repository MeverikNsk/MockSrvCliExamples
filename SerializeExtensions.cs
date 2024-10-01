namespace MockSrvCliExamples
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    internal static class SerializeExtensions
    {
        public static JsonSerializerOptions GetOptions()
        {
            return new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public static string SerializeToJsonString<TObject>(this TObject obj, Func<JsonSerializerOptions>? serializeOptions = null) where TObject : class
        {
            JsonSerializerOptions options = serializeOptions?.Invoke() ?? GetOptions();
            return JsonSerializer.Serialize(obj, options);
        }
    }
}
