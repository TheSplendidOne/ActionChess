using System;
using ActionChessClient.AuthenticationService;

namespace ActionChessClient
{
    internal class CAuthenticationStaff
    {
        public CUser User { get; set; }
        
        public AuthenticationServiceClient Client { get; }

        private static readonly Lazy<CAuthenticationStaff> _instance = new Lazy<CAuthenticationStaff>(() => new CAuthenticationStaff());

        private CAuthenticationStaff()
        {
            Client = new AuthenticationServiceClient();
        }

        public static CAuthenticationStaff Instance => _instance.Value;
    }
}
