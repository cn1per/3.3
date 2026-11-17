using System.Globalization;
using System.IO;
using System.Text;
using Avalonia.Controls;
using katalog.Ekrany;

namespace katalog;

public partial class MainWindow : Window
{
    private readonly Ekrany.ListaPozycjiView _listaPozycji;

    public MainWindow()
    {
        InitializeComponent();
        _listaPozycji = new Ekrany.ListaPozycjiView();
        _listaPozycji.DetailsRequested += ShowDetails;
        _listaPozycji.AddRequested += ShowAddForm;
        Zawartosc.Content = _listaPozycji;
    }

    private void ShowAddForm()
    {
        var widok = new Ekrany.AddPositionView();
        widok.SaveRequested += SavePosition;
        widok.BackRequested += ShowList;
        Zawartosc.Content = widok;
    }

    private void SavePosition(Pozycja pozycja)
    {
        var sciezka = Path.Combine(AppContext.BaseDirectory, "dane", "pozycje.txt");
        var wiersz = string.Join(
            ";",
            pozycja.Tytul,
            pozycja.Rok.ToString(CultureInfo.InvariantCulture),
            decimal.Parse(pozycja.OcenaTekstowa, CultureInfo.GetCultureInfo("pl-PL"))
                .ToString("0.##", CultureInfo.InvariantCulture));

        try
        {
            File.AppendAllText(sciezka, Environment.NewLine + wiersz, new UTF8Encoding(false));
        }
        catch (IOException ex)
        {
            ShowSaveError($"Nie można zapisać pozycji: {ex.Message}");
            return;
        }
        catch (UnauthorizedAccessException ex)
        {
            ShowSaveError($"Brak dostępu do pliku danych: {ex.Message}");
            return;
        }

        _listaPozycji.DodajPozycje(pozycja);
        ShowList();
    }

    private void ShowSaveError(string tekst)
    {
        if (Zawartosc.Content is Ekrany.AddPositionView widok)
        {
            widok.ShowSaveError(tekst);
        }
    }

    private void ShowDetails(Pozycja pozycja)
    {
        var widok = new Ekrany.SzczegolyPozycjiView(pozycja);
        widok.BackRequested += ShowList;
        Zawartosc.Content = widok;
    }

    private void ShowList()
    {
        Zawartosc.Content = _listaPozycji;
    }
}