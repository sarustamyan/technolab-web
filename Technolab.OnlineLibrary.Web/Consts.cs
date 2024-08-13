namespace Technolab.OnlineLibrary.Web
{
    public static class AuthorizationPolicies
    {
        public const string Admins = "Admins";
        public const string Users = "Users";
    }

    public static class ConfigurationKeys
    {
        public const string AuthCookieName = "AuthCookieName";
        public const string FileBasedDbContextDirectory = "FileBasedDbContextDirectory";
    }
}