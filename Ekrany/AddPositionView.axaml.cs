using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace katalog.Ekrany;

public partial class AddPositionView : UserControl
{
    public event Action<Pozycja>? SaveRequested;
    public event Action? BackRequested;

    public AddPositionView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public void ShowSaveError(string tekst)
    {
        ShowMessage(tekst);
    }

    private void SaveButton_Click(object? sender, RoutedEventArgs e)
    {
        var tytul = this.FindControl<TextBox>("PoleTytul")?.Text?.Trim();
        var rokTekst = this.FindControl<TextBox>("PoleRok")?.Text?.Trim();
        var ocenaTekst = this.FindControl<TextBox>("PoleOcena")?.Text?.Trim();

        if (string.IsNullOrWhiteSpace(tytul) || tytul.Contains(';') || tytul.Contains('\n') || tytul.Contains('\r'))
        {
            ShowMessage("Podaj tytuł, który nie zawiera średnika ani znaku nowej linii.");
            return;
        }

        if (!int.TryParse(rokTekst, out var rok))
        {
            ShowMessage("Podaj prawidłowy rok.");
            return;
        }

        const NumberStyles styleOceny =
            NumberStyles.AllowLeadingSign |
            NumberStyles.AllowDecimalPoint |
            NumberStyles.AllowLeadingWhite |
            NumberStyles.AllowTrailingWhite;

        if (!decimal.TryParse(ocenaTekst, styleOceny, CultureInfo.GetCultureInfo("pl-PL"), out var ocena) &&
            !decimal.TryParse(ocenaTekst, styleOceny, CultureInfo.InvariantCulture, out ocena))
        {
            ShowMessage("Podaj prawidłową ocenę.");
            return;
        }

        if (ocena < 0 || ocena > 10)
        {
            ShowMessage("Ocena musi być z zakresu od 0 do 10.");
            return;
        }

        SaveRequested?.Invoke(new Pozycja(
            tytul,
            rok,
            ocena.ToString("0.##", CultureInfo.GetCultureInfo("pl-PL"))));
    }

    private void BackButton_Click(object? sender, RoutedEventArgs e)
    {
        BackRequested?.Invoke();
    }

    private void ShowMessage(string tekst)
    {
        if (this.FindControl<TextBlock>("Komunikat") is { } komunikat)
        {
            komunikat.Text = tekst;
            komunikat.IsVisible = true;
        }
    }
}
