using System.Text.RegularExpressions;
using Domain.Common;

namespace Domain.ValueObjects
{

    public class Email : ValueObject
    {
        public string EmailAddress { get; private set; }
        private Email()
        {

        }

        private Email(string input)
        {
            EmailAddress = input;
        }
        public static Email From(string input)
        {
            if (IsValidEmailAddress(input) == false)
                throw new NotSupportedException("Invalid email");

            var email = new Email(input);
            return email;
        }

        private static bool IsValidEmailAddress(string input)
        {
            // logic to check if provided string is a valid email
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(input);
            if (match.Success)
                return true;
            else
                return false;
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return EmailAddress;
        }

        public static implicit operator string(Email Email)
        {
            return Email.ToString();
        }

        public static explicit operator Email(string email)
        {
            return From(email);
        }
        public override string ToString()
        {
            return this.EmailAddress;
        }
    }
}