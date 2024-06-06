using System.Text.Json;

public static class SessionExtensions
{
    public static void SetObject<T>(this ISession session, string key, T value, ILogger logger = null)
    {
        var serializedValue = JsonSerializer.Serialize(value);
        session.SetString(key, serializedValue);
        logger?.LogInformation($"Set session key {key} with value {serializedValue}");
    }

    public static T GetObject<T>(this ISession session, string key, ILogger logger = null)
    {
        var value = session.GetString(key);
        if (value == null)
        {
            logger?.LogWarning($"Session key {key} not found");
            return default;
        }
        var deserializedValue = JsonSerializer.Deserialize<T>(value);
        logger?.LogInformation($"Get session key {key} with value {value}");
        return deserializedValue;
    }
}