using System;

namespace WebApiPractice.Persistent.DataModels
{
    public static class RowVersionGenerator
    {
        public static string GetVersion()
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(DateTime.UtcNow.ToBinary().ToString()));
        }
    }
}
