using System;
using System.Text;

namespace AuthenticationService
{
    internal class CSaltGenerator
    {
        private readonly Random _random;

        public static readonly Int32 SaltCharLeftBorder = 97; // ASCII(97) = 'a'

        public static readonly Int32 SaltCharRightBorder = 123; // ASCII(122) = 'z'

        public static readonly Int32 SaltLength = 32;

        private static readonly Lazy<CSaltGenerator> _instance = new Lazy<CSaltGenerator>(() => new CSaltGenerator());

        public static CSaltGenerator Instance => _instance.Value;

        private CSaltGenerator()
        {
            _random = new Random();
        }

        public String GetNewSalt()
        {
            StringBuilder salt = new StringBuilder(SaltLength);
            for (Int32 i = 0; i < SaltLength; i++)
            {
                salt.Append((Char)_random.Next(SaltCharLeftBorder, SaltCharRightBorder));
            }

            return salt.ToString();
        }

    }
}
