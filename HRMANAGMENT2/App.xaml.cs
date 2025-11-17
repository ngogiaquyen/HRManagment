using System.Configuration;
using System.Data;
using System.Windows;
using OfficeOpenXml;

namespace HRMANAGMENT2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Configure EPPlus 8 license once at app startup (NonCommercial usage)
            ExcelPackage.License.SetNonCommercialPersonal("HRMANAGMENT2");
            base.OnStartup(e);
        }
    }

}
