using System;
using System.ServiceModel;
using DataBaseAccessService;

namespace AuthenticationService
{
    [ServiceContract]
    public interface IAuthenticationService
    {
        [OperationContract]
        CUser TryLogIn(String nickname, String password);

        [OperationContract]
        Boolean TrySignUp(String nickname, String password);

        [OperationContract(IsOneWay = true)]
        void LogOut(Guid userId);
    }
}
