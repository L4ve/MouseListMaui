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
        private bool sortAscending = true;
        private MouseDatabase _db;

        public MainPage()
        {
            InitializeComponent();

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "mice.db3");
            _db = new MouseDatabase(dbPath);

            LoadMiceFromDb();
        }

        private async void LoadMiceFromDb()
        {
            mice = await _db.GetMiceAsync();

            if (!mice.Any())
            {
                mice = new List<Mouse1>
                {
                    new Mouse1 { Model="OP1 8K", Firma="ENDGAMEGEAR", Sensor="PixArt 3389", Waga=80, Link="https://www.amazon.com/stores/ENDGAMEGEAR/page/D37B2C1B-5C1F-4974-8EE1-FEF5A7E567C0" },
                    new Mouse1 { Model="DeathAdder V3", Firma="Razer", Sensor="Focus+", Waga=82, Link="https://www.amazon.com/DeathAdder-Wired-Gaming-Mouse-HyperPolling/dp/B0B6XTDJS1" },
                    new Mouse1 { Model="Hyperlight", Firma="Hitscan", Sensor="PixArt 3389", Waga=70, Link="https://hitscan.com/products/hyperlight" },
                    new Mouse1 { Model="G102", Firma="Logitech", Sensor="PixArt 3327", Waga=85, Link="https://www.amazon.com/Logitech-Customizable-Lighting-Programmable-Tracking/dp/B0895BG8QP?th=1" },
                    new Mouse1 { Model="G Pro Superlight 2C", Firma="Logitech", Sensor="Hero 25K", Waga=62, Link="https://www.logitechg.com/en-us/shop/p/pro-x-superlight-2c" }
                };

                foreach (var m in mice)
                    await _db.SaveMouseAsync(m);
            }

            DisplayMice(mice);
        }

        private void DisplayMice(List<Mouse1> miceToDisplay)
        {
            int tableIndex = 2; // input + button
            while (MyStackLayout.Children.Count > tableIndex)
                MyStackLayout.Children.RemoveAt(tableIndex);

            var grid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = 50 },
                    new ColumnDefinition { Width = 150 },
                    new ColumnDefinition { Width = 120 },
                    new ColumnDefinition { Width = 120 },
                    new ColumnDefinition { Width = 80 },
                    new ColumnDefinition { Width = GridLength.Star } // last column fills remaining space
                },
                RowSpacing = 1,
                ColumnSpacing = 1,
                BackgroundColor = Colors.Transparent
            };

            // Header row
            grid.RowDefinitions.Add(new RowDefinition { Height = 40 });
            grid.Children.Add(CreateHeaderLabel("Id", 0, 0));
            grid.Children.Add(CreateHeaderLabel("Model", 0, 1));
            grid.Children.Add(CreateHeaderLabel("Firma", 0, 2));
            grid.Children.Add(CreateHeaderLabel("Sensor", 0, 3));
            grid.Children.Add(CreateHeaderLabel("Waga", 0, 4, true));
            grid.Children.Add(CreateHeaderLabel("Link", 0, 5));

            int rowIndex = 1;
            foreach (var m in miceToDisplay)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = 40 });

                // Alternating row colors
                var rowBackground = rowIndex % 2 == 0 ? Colors.White : Colors.LightGray;

                grid.Children.Add(CreateCellLabel(m.Id.ToString(), rowIndex, 0, rowBackground));
                grid.Children.Add(CreateCellLabel(m.Model, rowIndex, 1, rowBackground));
                grid.Children.Add(CreateCellLabel(m.Firma, rowIndex, 2, rowBackground));
                grid.Children.Add(CreateCellLabel(m.Sensor, rowIndex, 3, rowBackground));
                grid.Children.Add(CreateCellLabel(m.Waga.ToString(), rowIndex, 4, rowBackground));

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

            MyStackLayout.Children.Add(grid);
        }

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

            if (isClickable)
            {
                var tap = new TapGestureRecognizer();
                tap.Tapped += (s, e) =>
                {
                    mice = sortAscending
                        ? mice.OrderBy(m => m.Waga).ToList()
                        : mice.OrderByDescending(m => m.Waga).ToList();
                    sortAscending = !sortAscending;
                    DisplayMice(mice);
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

        private async void OnLinkTapped(object sender, EventArgs e)
        {
            if (sender is Label label &&
                (label.GestureRecognizers.FirstOrDefault() as TapGestureRecognizer) is TapGestureRecognizer tap &&
                tap.CommandParameter is string url)
            {
                await Launcher.OpenAsync(url);
            }
        }

        private async void OnAddMouseClicked(object sender, EventArgs e)
        {
            if (!double.TryParse(WagaEntry.Text, out double waga))
            {
                await DisplayAlert("Error", "Waga must be a number", "OK");
                return;
            }

            var newMouse = new Mouse1
            {
                Model = ModelEntry.Text ?? "",
                Firma = FirmaEntry.Text ?? "",
                Sensor = SensorEntry.Text ?? "",
                Waga = waga,
                Link = LinkEntry.Text ?? ""
            };

            await _db.SaveMouseAsync(newMouse);

            mice.Add(newMouse);
            DisplayMice(mice);

            ModelEntry.Text = "";
            FirmaEntry.Text = "";
            SensorEntry.Text = "";
            WagaEntry.Text = "";
            LinkEntry.Text = "";
        }
    }
}
