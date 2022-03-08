using AppCore.MvcWebUI.Utils.Bases;
using Microsoft.Extensions.Configuration;

namespace AppCore.MvcWebUI.Utils
{
    public class AppSettingsUtil : AppSettingsUtilBase
    {
        public AppSettingsUtil(IConfiguration configuration) : base(configuration)
        {

        }
    }
}
