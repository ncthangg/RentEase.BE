using RentEase.Service.Helper;

namespace RentEase.Service
{
    public class HelperWrapper
    {
        public IEmailHelper EmailHelper { get; set; }
        public ITokenHelper TokenHelper { get; set; }
        public IPasswordHelper PasswordHelper { get; set; }
        public HelperWrapper(
            IEmailHelper emailHelper,
            ITokenHelper tokenHelper,
            IPasswordHelper passwordHelper
            )
        {
            EmailHelper = emailHelper;
            TokenHelper = tokenHelper;
            PasswordHelper = passwordHelper;
        }
    }
}
