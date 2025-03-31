using EventManagementApi.Shared.Constants.Enums;

namespace EventManagementApi.Shared.Extensions
{
    public static class EnumExtensions
    {
        public static string ToEnumString(this EventStatus status)
        {
            return status.ToString();
        }

        public static string ToEnumString(this RegistrationStatus status)
        {
            return status.ToString();
        }

        public static string ToEnumString(this Role role)
        {
            return role.ToString();
        }
    }
}
