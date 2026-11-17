using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace katalog.Ekrany;

public partial class SzczegolyPozycjiView : UserControl
{
    public event Action? BackRequested;

    public SzczegolyPozycjiView(Pozycja pozycja)
    {
        InitializeComponent();
        DataContext = pozycja;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void BackButton_Click(object? sender, RoutedEventArgs e)
    {
        BackRequested?.Invoke();
    }
}
