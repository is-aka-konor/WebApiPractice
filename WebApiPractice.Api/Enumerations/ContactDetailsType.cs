using System;

namespace WebApiPractice.Api.Enumerations
{
    [Serializable]
    public class ContactDetailsType : Enumeration
    {
        public static readonly ContactDetailsType Phone = new ContactDetailsType("Phone", "Phone number");
        public static readonly ContactDetailsType Email = new ContactDetailsType("Email", "Email address");
        public static readonly ContactDetailsType Website = new ContactDetailsType("Website", "Website");
        public static readonly ContactDetailsType Unknown = new ContactDetailsType("Unknown", "Contact details are not recognized");

        public ContactDetailsType() { }
        private ContactDetailsType(string value, string displayName) : base(value, displayName) { }
    }
}
