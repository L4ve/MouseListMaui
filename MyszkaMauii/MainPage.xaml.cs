namespace MyszkaMauii
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnLinkTapped(object sender, EventArgs e)
        {
            if (sender is Label label &&
                (label.GestureRecognizers.FirstOrDefault() as TapGestureRecognizer) is TapGestureRecognizer tap &&
                tap.CommandParameter is string url)
            {
                await Launcher.OpenAsync(url);
            }
        }
    }
}
