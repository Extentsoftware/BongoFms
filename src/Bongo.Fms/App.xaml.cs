namespace Bongo.Fms
{
    public partial class App : Application
    {
        public static IServiceProvider Services;
        
        public App(IServiceProvider services)
        {
            InitializeComponent();

            Services = services;

            MainPage = new AppShell();
        }
    }
}
