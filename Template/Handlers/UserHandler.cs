using Template.Models;
using Template.Repository;
using Template.Services;

namespace Template.Handlers
{
    public class UserHandler
    {
        private UserRepository UserRepository;
        private JWTService JWTService;

        public UserHandler()
        {
            UserRepository = new UserRepository();
            JWTService = new JWTService();
        }

        public string GetUserToken(UserInfo userInfo)
        {
            if (UserRepository.ValidateCredentials(userInfo))
                return JWTService.GenerateToken(userInfo);
            else
                return null;
        }
    }
}
