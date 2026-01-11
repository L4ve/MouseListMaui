using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyszkaMauii
{
    public partial class MainPage : ContentPage
    {
        private List<Mouse1> mice;

        // do sortowania
        private bool sortAscending = true;

        // API
        private MouseApiService _api;

        // Referencja do tabeli
        private Grid miceGrid;

        public MainPage()
        {
            InitializeComponent(); 

            _api = new MouseApiService(); // instancja api
            LoadMiceFromApi(); // pobiera dane z api przy starcie
        }

        // pobieranie myszy z api
        private async void LoadMiceFromApi()
        {
            mice = await _api.GetMiceAsync();
            DisplayMice(mice); // wyświetlenie myszy w tabeli
        }

        // wyświetlanie tabeli
        private void DisplayMice(List<Mouse1> miceToDisplay)
        {
            // usuń starą tabelę, jeśli istnieje, aby sie nie kopiowała bo stworzeniu nowej myszy
            if (miceGrid != null)
            {
                MyStackLayout.Children.Remove(miceGrid);
            }

            miceGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = 50 }, 
                    new ColumnDefinition { Width = 150 },
                    new ColumnDefinition { Width = 120 },  
                    new ColumnDefinition { Width = 120 },  
                    new ColumnDefinition { Width = 80 },    
                    new ColumnDefinition { Width = GridLength.Star }// było czarne pole bez tego jest to użyte jako filler
                },
                RowSpacing = 1,
                ColumnSpacing = 1
            };

            miceGrid.RowDefinitions.Add(new RowDefinition { Height = 40 });
            miceGrid.Children.Add(CreateHeaderLabel("Id", 0, 0));
            miceGrid.Children.Add(CreateHeaderLabel("Model", 0, 1));
            miceGrid.Children.Add(CreateHeaderLabel("Firma", 0, 2));
            miceGrid.Children.Add(CreateHeaderLabel("Sensor", 0, 3));
            miceGrid.Children.Add(CreateHeaderLabel("Waga", 0, 4, true)); // Klikany do sortowania
            miceGrid.Children.Add(CreateHeaderLabel("Link", 0, 5));

            int rowIndex = 1;

            // wypełnianie tabeli
            foreach (var m in miceToDisplay)
            {
                miceGrid.RowDefinitions.Add(new RowDefinition { Height = 40 });

                // kolorowanie wierszy na przemian(to coś jest z chata bo wyglądało brzydko z jednym kolorem)
                var bg = rowIndex % 2 == 0 ? Colors.White : Colors.LightGray;

                // dodanie komórek do tabeli
                miceGrid.Children.Add(CreateCellLabel(m.Id.ToString(), rowIndex, 0, bg));
                miceGrid.Children.Add(CreateCellLabel(m.Model, rowIndex, 1, bg));
                miceGrid.Children.Add(CreateCellLabel(m.Firma, rowIndex, 2, bg));
                miceGrid.Children.Add(CreateCellLabel(m.Sensor, rowIndex, 3, bg));
                miceGrid.Children.Add(CreateCellLabel(m.Waga.ToString(), rowIndex, 4, bg));

                // funkcja linku
                var linkLabel = new Label
                {
                    Text = "Otwórz link",
                    TextColor = Colors.Black,
                    BackgroundColor = bg,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    TextDecorations = TextDecorations.Underline
                };

                // działanie kliknięcia w link
                var tap = new TapGestureRecognizer { CommandParameter = m.Link };
                tap.Tapped += OnLinkTapped;
                linkLabel.GestureRecognizers.Add(tap);

                Grid.SetRow(linkLabel, rowIndex);
                Grid.SetColumn(linkLabel, 5);
                miceGrid.Children.Add(linkLabel);

                rowIndex++;
            }

            MyStackLayout.Children.Add(miceGrid);
        }

        // nagłówki
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

            // waga jest do kliknięcia, wtedy sortuje
            if (isClickable)
            {
                var tap = new TapGestureRecognizer();
                tap.Tapped += (s, e) =>
                {
                    mice = sortAscending
                        ? mice.OrderBy(m => m.Waga).ToList() : mice.OrderByDescending(m => m.Waga).ToList();

                    sortAscending = !sortAscending; // można klikać do woli i działa
                    DisplayMice(mice); // odświeżenie tabeli
                };
                label.GestureRecognizers.Add(tap);
            }

            return label;
        }

        // komórki
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

        // klikanie w link
        private async void OnLinkTapped(object sender, EventArgs e)
        {
            if (sender is Label label &&
                label.GestureRecognizers.FirstOrDefault() is TapGestureRecognizer tap &&
                tap.CommandParameter is string url)
            {
                // Otwieranie linku w przeglądarce
                await Launcher.OpenAsync(url);
            }
        }

        // funkcja dodawania nowej myszy
        private async void OnAddMouseClicked(object sender, EventArgs e)
        {
            // sprawdza czy waga to liczba
            if (!double.TryParse(WagaEntry.Text, out double waga))
                return;

            // dane nowej myszy
            var newMouse = new Mouse1
            {
                Model = ModelEntry.Text ?? "",
                Firma = FirmaEntry.Text ?? "",
                Sensor = SensorEntry.Text ?? "",
                Waga = waga,
                Link = LinkEntry.Text ?? ""
            };

                // wywołanie api dodające mysz
                var addedMouse = await _api.AddMouseAsync(newMouse);

                mice.Add(addedMouse); // dodajemy do lokalnej listy
                DisplayMice(mice); // odświeżamy tabelę



            // czysci formularz po wysłaniu do api
            ModelEntry.Text = "";
            FirmaEntry.Text = "";
            SensorEntry.Text = "";
            WagaEntry.Text = "";
            LinkEntry.Text = "";
        }
    }
}
