namespace MockSrvCliExamples
{
    using Refit;
    using System.Net.Http;
    using System.Reflection;
    using System.Text.Encodings.Web;
    using System.Text.Json.Serialization;
    using System.Text.Json;
    using System.Text.Unicode;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    internal class CustomContentSerializer : IHttpContentSerializer
    {
        private readonly JsonSerializerOptions _serializerOptions;

        private readonly SystemTextJsonContentSerializer _systemTextJsonContentSerializer;

        private readonly List<string> contentMediaTypes = ["application/json", "text/plain"];

        public CustomContentSerializer(JsonSerializerOptions? serializerOptions = null)
        {
            _serializerOptions = serializerOptions ?? new JsonSerializerOptions
            {
                Converters = { (JsonConverter)new JsonStringEnumConverter() },
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
            };
            _systemTextJsonContentSerializer = new SystemTextJsonContentSerializer(_serializerOptions);
        }

        public async Task<T?> FromHttpContentAsync<T>(HttpContent content, CancellationToken cancellationToken = default)
        {
            if (content.Headers.ContentType != null && contentMediaTypes.Contains(content.Headers.ContentType.MediaType ?? string.Empty))
            {
                using Stream jsonContent = await content.ReadAsStreamAsync(cancellationToken);
                return await JsonSerializer.DeserializeAsync<T>(jsonContent, _serializerOptions, cancellationToken);
            }

            return default;
        }

        public string? GetFieldNameForProperty(PropertyInfo propertyInfo)
        {
            return _systemTextJsonContentSerializer.GetFieldNameForProperty(propertyInfo);
        }

        public HttpContent ToHttpContent<T>(T item)
        {
            string content = JsonSerializer.Serialize(item, _serializerOptions);
            return new StringContent(content, Encoding.UTF8, "application/json");
        }
    }
}
