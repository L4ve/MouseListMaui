using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyszkaMauii
{
    public partial class MainPage : ContentPage
    {
        private List<Mouse1> mice;
        //sortowanie
        private bool sortAscending = true;

        //bierze rzeczy z bazy danych (MouseDatabase.cs)
        private MouseDatabase _db;

        public MainPage()
        {
            InitializeComponent();

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "mice.db3");
            _db = new MouseDatabase(dbPath);

            LoadMiceFromDb();
        }
        // bierze dane z bazy
        private async void LoadMiceFromDb()
        {
            mice = await _db.GetMiceAsync();

            DisplayMice(mice);
        }

        private void DisplayMice(List<Mouse1> miceToDisplay)
        {
            //stworzenie grida dla myszek
            var grid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = 50 },
                    new ColumnDefinition { Width = 150 },
                    new ColumnDefinition { Width = 120 },
                    new ColumnDefinition { Width = 120 },
                    new ColumnDefinition { Width = 80 },
                    new ColumnDefinition { Width = GridLength.Star } // miałem problem z czarnym pudłem w kodzie, to jest dodane tylko po to by tego nie pokazywało
                },
                RowSpacing = 1,
                ColumnSpacing = 1,
            };

            // Nagłówki
            grid.RowDefinitions.Add(new RowDefinition { Height = 40 });
            grid.Children.Add(CreateHeaderLabel("Id", 0, 0));
            grid.Children.Add(CreateHeaderLabel("Model", 0, 1));
            grid.Children.Add(CreateHeaderLabel("Firma", 0, 2));
            grid.Children.Add(CreateHeaderLabel("Sensor", 0, 3));
            grid.Children.Add(CreateHeaderLabel("Waga", 0, 4, true)); // po kliknięciu w "waga" sortuje myszki wagowo
            grid.Children.Add(CreateHeaderLabel("Link", 0, 5));

            int rowIndex = 1;
            foreach (var m in miceToDisplay)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = 40 });

                //zamienia kolory z jednego na drugi co drugi row z białego na lightgray
                var rowBackground = rowIndex % 2 == 0 ? Colors.White : Colors.LightGray;

                grid.Children.Add(CreateCellLabel(m.Id.ToString(), rowIndex, 0, rowBackground));
                grid.Children.Add(CreateCellLabel(m.Model, rowIndex, 1, rowBackground));
                grid.Children.Add(CreateCellLabel(m.Firma, rowIndex, 2, rowBackground));
                grid.Children.Add(CreateCellLabel(m.Sensor, rowIndex, 3, rowBackground));
                grid.Children.Add(CreateCellLabel(m.Waga.ToString(), rowIndex, 4, rowBackground));
                //Label jako link
                var linkLabel = new Label
                {
                    Text = "Otwórz link",
                    TextColor = Colors.Blue,
                    TextDecorations = TextDecorations.Underline,
                    BackgroundColor = rowBackground,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center
                };
                var tap = new TapGestureRecognizer { CommandParameter = m.Link };
                tap.Tapped += OnLinkTapped;
                linkLabel.GestureRecognizers.Add(tap);
                Grid.SetRow(linkLabel, rowIndex);
                Grid.SetColumn(linkLabel, 5);
                grid.Children.Add(linkLabel);

                rowIndex++;
            }
            //dodanie tabeli do stacklayout
            MyStackLayout.Children.Add(grid);
        }
        //tworzenie headerów
        private Label CreateHeaderLabel(string text, int row, int column, bool isClickable = false)
        {
            var label = new Label
            {
                Text = text,
                BackgroundColor = Colors.LightBlue,
                TextColor = Colors.Black,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FontAttributes = FontAttributes.Bold
            };
            Grid.SetRow(label, row);
            Grid.SetColumn(label, column);
            // kod do klikania wagi dla sortowania
            if (isClickable)
            {
                var tap = new TapGestureRecognizer();
                tap.Tapped += (s, e) =>
                {
                    mice = sortAscending
                        ? mice.OrderBy(m => m.Waga).ToList()
                        : mice.OrderByDescending(m => m.Waga).ToList();
                    sortAscending = !sortAscending;
                    DisplayMice(mice); // odświeżenie tabeli
                };
                label.GestureRecognizers.Add(tap);
            }

            return label;
        }

        private Label CreateCellLabel(string text, int row, int column, Color background)
        {
            var label = new Label
            {
                Text = text,
                BackgroundColor = background,
                TextColor = Colors.Black,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };
            Grid.SetRow(label, row);
            Grid.SetColumn(label, column);
            return label;
        }
        // funkcja otwierania linku w przeglądarce(zabrałem to z neta nie mam pojęcia co to robi)
        private async void OnLinkTapped(object sender, EventArgs e)
        {
            if (sender is Label label &&
                (label.GestureRecognizers.FirstOrDefault() as TapGestureRecognizer) is TapGestureRecognizer tap &&
                tap.CommandParameter is string url)
            {
                await Launcher.OpenAsync(url);
            }
        }
        // dodawanie nowej myszy
        private async void OnAddMouseClicked(object sender, EventArgs e)
        {   //sprawdzanie czy waga to liczba
            if (!double.TryParse(WagaEntry.Text, out double waga))
            {
                return;
            }
            //nowa mysz na podstawie danych z strony
            var newMouse = new Mouse1
            {
                Model = ModelEntry.Text ?? "",
                Firma = FirmaEntry.Text ?? "",
                Sensor = SensorEntry.Text ?? "",
                Waga = waga,
                Link = LinkEntry.Text ?? ""
            };

            await _db.SaveMouseAsync(newMouse); //dodanie do bazy danych (mouse.cs)

            mice.Add(newMouse); //dodanie do tabeli
            DisplayMice(mice); //resetowanie tabeli by sie wyświetlało
            // usuwanie danych po wpisaniu
            ModelEntry.Text = "";
            FirmaEntry.Text = "";
            SensorEntry.Text = "";
            WagaEntry.Text = "";
            LinkEntry.Text = "";
        }
    }
}
