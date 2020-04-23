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
        private const string user = "User";
        private const string typeUser = "-1";
        private const string cantidadEscaneo = "0";
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

        public static string User
        {
            get
            {
                return AppSettings.GetValueOrDefault(user, stringDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(user, value);
            }
        }

        public static string TypeUser
        {
            get
            {
                return AppSettings.GetValueOrDefault(typeUser, "0");
            }
            set
            {
                AppSettings.AddOrUpdateValue(typeUser, value);
            }
        }

        public static string CantidadEscaneo
        {
            get
            {
                return AppSettings.GetValueOrDefault(cantidadEscaneo, "0");
            }
            set
            {
                AppSettings.AddOrUpdateValue(cantidadEscaneo, value);
            }
        }
    }
}