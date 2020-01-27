using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using DataBaseAccessService;
using SharedServiceLibrary;

namespace AuthenticationService
{
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class CAuthenticationService : IAuthenticationService
    {
        private readonly HashSet<Guid> _onlineUsers;

        private SHA256 _sha256;
        private SHA256 Sha256
        {
            get { return _sha256 ??= SHA256.Create(); }
        }

        public CAuthenticationService()
        {
            _onlineUsers = new HashSet<Guid>();
        }

        public CUser TryLogIn(String nickname, String password)
        {
            IDataBaseAccessService client = SDbService.Get();
            CUser user = client.FindRegisteredUser(nickname);
            if (user == null)
                return null;
            if (_onlineUsers.Contains(user.UserId))
                return null;
            if (!ComputePasswordHash(password, user.Authentication.Salt).SequenceEqual(user.Authentication.PasswordHash))
                return null;
            _onlineUsers.Add(user.UserId);
            user.Authentication = null;
            return user;
        }

        public Boolean TrySignUp(String nickname, String password)
        {
            IDataBaseAccessService client = SDbService.Get();
            if (client.FindUserByNickname(nickname) != null)
                return false;
            Guid userId = Guid.NewGuid();
            String salt = CSaltGenerator.Instance.GetNewSalt();
            CUser user = new CUser
            {
                Nickname = nickname,
                UserId = userId,
                RegistrationDate = DateTime.UtcNow,
                Authentication = new CAuthentication()
                {
                    UserId = userId,
                    Salt = salt,
                    PasswordHash = ComputePasswordHash(password, salt)
                }
            };
            return client.AddUser(user);
        }

        public void LogOut(Guid userId)
        {
            _onlineUsers.Remove(userId);
        }

        private Byte[] ComputePasswordHash(String password, String salt)
        {
            return Sha256.ComputeHash(Encoding.UTF8.GetBytes(password + salt));
        }
    }
}
