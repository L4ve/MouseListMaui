using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;

namespace MyszkaMauii
{
    public partial class MainPage : ContentPage
    {
        private List<Mouse> mice;
        private bool sortAscending = true; // toggle for sorting

        public MainPage()
        {
            InitializeComponent();
            LoadMice();
            DisplayMice(mice);
        }

        private void LoadMice()
        {
            mice = new List<Mouse>
            {
                new Mouse { Id=1, Model="OP1 8K", Firma="ENDGAMEGEAR", Sensor="PixArt 3389", Waga=80, Link="https://www.amazon.com/stores/ENDGAMEGEAR/page/D37B2C1B-5C1F-4974-8EE1-FEF5A7E567C0" },
                new Mouse { Id=2, Model="DeathAdder V3", Firma="Razer", Sensor="Focus+", Waga=82, Link="https://www.amazon.com/DeathAdder-Wired-Gaming-Mouse-HyperPolling/dp/B0B6XTDJS1" },
                new Mouse { Id=3, Model="Hyperlight", Firma="Hitscan", Sensor="PixArt 3389", Waga=70, Link="https://hitscan.com/products/hyperlight" },
                new Mouse { Id=4, Model="G102", Firma="Logitech", Sensor="PixArt 3327", Waga=85, Link="https://www.amazon.com/Logitech-Customizable-Lighting-Programmable-Tracking/dp/B0895BG8QP?th=1" },
                new Mouse { Id=5, Model="G Pro Superlight 2C", Firma="Logitech", Sensor="Hero 25K", Waga=62, Link="https://www.logitechg.com/en-us/shop/p/pro-x-superlight-2c" }
            };
        }

        private void DisplayMice(List<Mouse> miceToDisplay)
        {
            MyStackLayout.Children.Clear();

            // Create single grid for entire table
            var grid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = 50 },
                    new ColumnDefinition { Width = 150 },
                    new ColumnDefinition { Width = 120 },
                    new ColumnDefinition { Width = 120 },
                    new ColumnDefinition { Width = 80 },
                    new ColumnDefinition { Width = 400 }
                },
                RowSpacing = 1,
                ColumnSpacing = 1,
                BackgroundColor = Colors.Black
            };

            // Header row
            grid.RowDefinitions.Add(new RowDefinition { Height = 40 });
            grid.Children.Add(CreateHeaderLabel("Id", 0, 0));
            grid.Children.Add(CreateHeaderLabel("Model", 0, 1));
            grid.Children.Add(CreateHeaderLabel("Firma", 0, 2));
            grid.Children.Add(CreateHeaderLabel("Sensor", 0, 3));
            grid.Children.Add(CreateHeaderLabel("Waga", 0, 4, true)); // clickable for sorting
            grid.Children.Add(CreateHeaderLabel("Link", 0, 5));

            // Data rows
            int rowIndex = 1;
            foreach (var m in miceToDisplay)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = 40 });

                grid.Children.Add(CreateCellLabel(m.Id.ToString(), rowIndex, 0));
                grid.Children.Add(CreateCellLabel(m.Model, rowIndex, 1));
                grid.Children.Add(CreateCellLabel(m.Firma, rowIndex, 2));
                grid.Children.Add(CreateCellLabel(m.Sensor, rowIndex, 3));
                grid.Children.Add(CreateCellLabel(m.Waga.ToString(), rowIndex, 4));

                var linkLabel = new Label
                {
                    Text = "Otwórz link",
                    TextColor = Colors.Blue,
                    TextDecorations = TextDecorations.Underline,
                    BackgroundColor = Colors.White,
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
                BackgroundColor = Colors.White,
                TextColor = Colors.Black,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };

            Grid.SetRow(label, row);
            Grid.SetColumn(label, column);

            if (isClickable)
            {
                var tap = new TapGestureRecognizer();
                tap.Tapped += (s, e) =>
                {
                    // Toggle sorting
                    if (sortAscending)
                        mice = mice.OrderBy(m => m.Waga).ToList();
                    else
                        mice = mice.OrderByDescending(m => m.Waga).ToList();

                    sortAscending = !sortAscending;
                    DisplayMice(mice);
                };
                label.GestureRecognizers.Add(tap);
            }

            return label;
        }

        private Label CreateCellLabel(string text, int row, int column)
        {
            var label = new Label
            {
                Text = text,
                BackgroundColor = Colors.White,
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
    }

    // Mouse model
    public class Mouse
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public string Firma { get; set; }
        public string Sensor { get; set; }
        public double Waga { get; set; }
        public string Link { get; set; }
    }
}
