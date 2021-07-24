using System;

namespace WebApiPractice.Api.Enumerations
{
    [Serializable]
    public class ContactDetailsType : Enumeration
    {
        public static readonly ContactDetailsType Phone = new("Phone", "Phone number");
        public static readonly ContactDetailsType Email = new("Email", "Email address");
        public static readonly ContactDetailsType Website = new("Website", "Website");
        public static readonly ContactDetailsType Unknown = new("Unknown", "Contact details are not recognized");

        public ContactDetailsType() { }
        private ContactDetailsType(string value, string displayName) : base(value, displayName) { }
    }
}
