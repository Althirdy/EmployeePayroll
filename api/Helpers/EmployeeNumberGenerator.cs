using System.Globalization;

namespace EmployeePayroll.Helpers
{
    public class EmployeeNumberGenerator
    {
        private static readonly Random _random = new Random();
        public static string Generate(string lastName, DateTime dateOfBirth)
        {
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last Name is required");
            var prefix = lastName.Replace(" ","").ToUpper();

            if(prefix.Length < 3 )
            {
                prefix = prefix.PadRight(3, '*');
            }
            else
            {
                prefix = prefix.Substring(0, 3);
            }

            var randomNumber = _random.Next(0,99999).ToString("D5");

            var dobPart = dateOfBirth.ToString("ddMMMyyyy", CultureInfo.InvariantCulture).ToUpper();

            return $"{prefix}-{randomNumber}-{dobPart}";
        }
    }
}
