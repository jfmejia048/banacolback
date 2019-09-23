using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace PerfilacionDeCalidad.Movil.Helpers
{
    public class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants

        private const string accessToken = "AccessToken";
        private static readonly string stringDefault = string.Empty;
        //private static readonly bool booleanDefault = false;

        #endregion

        public static string AccessToken
        {
            get
            {
                return AppSettings.GetValueOrDefault(accessToken, stringDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(accessToken, value);
            }
        }
    }
}