using AppCore.Business.Utils.JsonWebToken.Bases;
using AppCore.MvcWebUI.Utils.Bases;

namespace AppCore.Business.Utils.JsonWebToken
{
    public class JwtUtil : JwtUtilBase
    {
        public JwtUtil(AppSettingsUtilBase appSettingsUtil) : base(appSettingsUtil)
        {

        }
    }
}
