using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace katalog.Ekrany;

public partial class ListaPozycjiView : UserControl
{
    private void OnCountClick(object? sender, RoutedEventArgs e)
{
    var lines = File.ReadAllLines("dane/pozycje.txt");
    CountText.Text = $"Pozycji w pliku: {lines.Length}";
}
    public ObservableCollection<Pozycja> Pozycje { get; } = new();
    public event Action<Pozycja>? DetailsRequested;
    public event Action? AddRequested;

    public ListaPozycjiView()
    {
        InitializeComponent();
        DataContext = this;
        WczytajPozycje();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void WczytajPozycje()
    {
        var sciezka = Path.Combine(AppContext.BaseDirectory, "dane", "pozycje.txt");

        try
        {
            var linie = File.ReadAllLines(sciezka);
            for (var i = 0; i < linie.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(linie[i]))
                {
                    continue;
                }

                var pola = linie[i].Split(';');
                if (pola.Length != 3 ||
                    string.IsNullOrWhiteSpace(pola[0]) ||
                    !int.TryParse(pola[1], out var rok) ||
                    !decimal.TryParse(pola[2], NumberStyles.Number, CultureInfo.InvariantCulture, out var ocena))
                {
                    throw new InvalidDataException($"Nieprawidłowe dane w wierszu {i + 1}.");
                }

                Pozycje.Add(new Pozycja(
                    pola[0].Trim(),
                    rok,
                    ocena.ToString("0.##", CultureInfo.GetCultureInfo("pl-PL"))));
            }

            if (Pozycje.Count == 0)
            {
                PokazKomunikat("Brak pozycji do wyświetlenia.");
            }
        }
        catch (InvalidDataException ex)
        {
            Pozycje.Clear();
            PokazKomunikat($"Błąd w pliku dane/pozycje.txt: {ex.Message}");
        }
        catch (IOException ex)
        {
            PokazKomunikat($"Nie można odczytać pliku danych: {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            PokazKomunikat($"Brak dostępu do pliku danych: {ex.Message}");
        }
    }

    private void PokazKomunikat(string tekst)
    {
        if (this.FindControl<TextBlock>("Komunikat") is { } komunikat)
        {
            komunikat.Text = tekst;
            komunikat.IsVisible = true;
        }
    }

    public void DodajPozycje(Pozycja pozycja)
    {
        Pozycje.Add(pozycja);
        if (this.FindControl<TextBlock>("Komunikat") is { } komunikat)
        {
            komunikat.IsVisible = false;
        }
    }

    private void AddButton_Click(object? sender, RoutedEventArgs e)
    {
        AddRequested?.Invoke();
    }

    private void DetailsButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: Pozycja pozycja })
        {
            DetailsRequested?.Invoke(pozycja);
        }
    }
}

public sealed record Pozycja(string Tytul, int Rok, string OcenaTekstowa);
