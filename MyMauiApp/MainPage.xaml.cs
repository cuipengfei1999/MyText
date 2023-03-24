using MyMauiApp.ViewModels;

namespace MyMauiApp
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = new MainViewModel();
        }

    }
}