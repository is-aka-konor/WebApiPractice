using System;

namespace WebApiPractice.Api.Enumerations
{
    [Serializable]
    public class CustomerStatus : Enumeration
    {
        public static readonly CustomerStatus Prospective = new ("Prospective", "Prospective customer");
        public static readonly CustomerStatus Current = new ("Current", "Current customer");
        public static readonly CustomerStatus NonActive = new ("NonActive", "Non-active customer");
        public static readonly CustomerStatus Unknown = new ("Unknown", "Customer status is not recognized");

        public CustomerStatus() { }
        private CustomerStatus(string value, string displayName) : base(value, displayName) { }
    }
}
