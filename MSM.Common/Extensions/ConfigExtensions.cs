namespace MSM.Common.Extensions;

public static class ConfigExtensions {
    public static T GetRequiredValue<T>(this IConfigurationSection section, string key) {
        return section.GetValue<T>(key) ??
               throw new InvalidOperationException(
                   $"Key `{key}` does not exist in the config section of `{section.Path}`"
               );
    }
}