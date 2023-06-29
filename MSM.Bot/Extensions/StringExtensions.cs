using MSM.Bot.Enums;

namespace MSM.Bot.Extensions;

public static class StringExtensions {
    private static T? EnumToString<T>(this string value) where T : struct, Enum {
        var converted = Enum.TryParse(value, out T outSummaryKey);

        if (!converted) {
            return null;
        }

        return outSummaryKey;
    }

    public static ModalId? ToModalId(this string modalId) {
        return EnumToString<ModalId>(modalId);
    }

    public static ModalFieldId? ToModalFieldId(this string modalFieldId) {
        return EnumToString<ModalFieldId>(modalFieldId);
    }
}