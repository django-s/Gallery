using System.Reflection;
using System.Text.Json;

namespace Gallery;

public sealed class Configuration
{
    [EnvironmentVariable("DB_CONNECTION_STRING")]
    public string DatabaseConnectionString { get; init; } =
        "Host=localhost:5433;Username=admin;Password=admin;Database=postgres";

    [EnvironmentVariable("IMAGE_DIRECTORY")]
    public string ImageDirectory { get; init; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Gallery/Images");

    [EnvironmentVariable("IMAGE_REQUEST_PATH")]
    public string ImageRequestPath { get; init; } = "/images";

    public static Configuration ImportFromEnvironmentVariables() => ImportFromEnvironmentVariables<Configuration>();

    private static T ImportFromEnvironmentVariables<T>()
    {
        Type type = typeof(T);

        Dictionary<string, object?> values = new();

        foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            EnvironmentVariableAttribute? attr = property.GetCustomAttribute<EnvironmentVariableAttribute>();
            if (attr == null)
            {
                continue;
            }

            string? envValue = Environment.GetEnvironmentVariable(attr.EnvironmentVariable);
            if (envValue == null)
            {
                continue;
            }

            object convertedValue = Convert.ChangeType(envValue, property.PropertyType);
            values[property.Name] = convertedValue;
        }

        // Serialise using JSON because it supports init properties.
        string json = JsonSerializer.Serialize(values);
        T? instance = JsonSerializer.Deserialize<T>(json);
        if (instance == null)
        {
            throw new InvalidOperationException("Failed to deserialize configuration.");
        }

        return instance;
    }

    [AttributeUsage(AttributeTargets.Property)]
    private sealed class EnvironmentVariableAttribute(string environmentVariable) : Attribute
    {
        public string EnvironmentVariable { get; } = environmentVariable;
    }
}
