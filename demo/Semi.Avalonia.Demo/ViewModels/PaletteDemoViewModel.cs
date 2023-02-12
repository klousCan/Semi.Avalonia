using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace Semi.Avalonia.Demo.ViewModels;

public class PaletteDemoViewModel: ObservableObject
{
    private readonly string[] _predefinedColorNames = { "Amber","Blue","Cyan","Green","Grey","Indigo","LightBlue","LightGreen","Lime","Orange","Pink","Purple","Red","Teal","Violet","Yellow" };
    private IResourceDictionary _lightResourceDictionary;
    private IResourceDictionary _darkResourceDictionary;
    
    private ColorItemViewModel _selectedColor;

    public ColorItemViewModel SelectedColor
    {
        get => _selectedColor;
        set => SetProperty(ref _selectedColor, value);
    }
    
    
    private ObservableCollection<ColorListViewModel> _lightLists;
    public ObservableCollection<ColorListViewModel> LightLists
    {
        get => _lightLists;
        set => SetProperty(ref _lightLists, value);
    }
    private ObservableCollection<ColorListViewModel> _darkLists;
    public ObservableCollection<ColorListViewModel> DarkLists
    {
        get => _darkLists;
        set => SetProperty(ref _darkLists, value);
    }

    public ObservableCollection<FunctionalColorGroupViewModel> FunctionalColors { get; set; } = new();

    public PaletteDemoViewModel()
    {
        _lightResourceDictionary = (ResourceDictionary)AvaloniaXamlLoader.Load(new Uri("avares://Semi.Avalonia/Themes/Light/Palette.axaml"));
        _darkResourceDictionary = (ResourceDictionary)AvaloniaXamlLoader.Load(new Uri("avares://Semi.Avalonia/Themes/Dark/Palette.axaml"));
        InitializePalette();
        InitializeFunctionalColors();
        WeakReferenceMessenger.Default.Register<PaletteDemoViewModel, ColorItemViewModel>(this, OnClickColorItem);
    }

    private void InitializePalette()
    {
        LightLists = new ObservableCollection<ColorListViewModel>();
        foreach (var color in _predefinedColorNames)
        {
            ColorListViewModel s = new ColorListViewModel();
            s.Initialize(_lightResourceDictionary, color, true);
            LightLists.Add(s);
        }
        DarkLists = new ObservableCollection<ColorListViewModel>();
        foreach (var color in _predefinedColorNames)
        {
            ColorListViewModel s = new ColorListViewModel();
            s.Initialize(_darkResourceDictionary, color, false);
            DarkLists.Add(s);
        }
    }

    private void InitializeFunctionalColors()
    {
        FunctionalColors.Add(new FunctionalColorGroupViewModel("Primary", _lightResourceDictionary, _darkResourceDictionary, ColorTokens.PrimaryTokens));
    }
    private void OnClickColorItem(PaletteDemoViewModel vm, ColorItemViewModel item)
    {
        SelectedColor = item;
    }
}

public class ColorListViewModel: ObservableObject
{
    private ObservableCollection<ColorItemViewModel>? _colors;

    public ObservableCollection<ColorItemViewModel>? Color
    {
        get => _colors;
        set => SetProperty(ref _colors, value);
    }

    private string? _seriesName;

    public string? SeriesName
    {
        get => _seriesName;
        set => SetProperty(ref _seriesName, value);
    }
    
    internal void Initialize(IResourceDictionary resourceDictionary, string color, bool light)
    {
        SeriesName = color;
        Color = new ObservableCollection<ColorItemViewModel>();
        
        for (int i = 0; i < 10; i++)
        {
            var key = "Semi" + color + i;
            if (resourceDictionary.TryGetValue(key, out var value))
            {
                if (value is ISolidColorBrush brush)
                {
                    string name = color + " " + i;
                    var item = new ColorItemViewModel(name, brush, key, light, i);
                    Color.Add(item);
                } 
            }
        }
    }
}

public class ColorItemViewModel : ObservableObject
{

    private IBrush _brush;
    public IBrush Brush
    {
        get => _brush;
        set => SetProperty(ref _brush, value);
    }
    
    private IBrush _textBrush;
    public IBrush TextBrush
    {
        get => _textBrush;
        set => SetProperty(ref _textBrush, value);
    }

    private string _colorDisplayName;
    public string ColorDisplayName
    {
        get => _colorDisplayName;
        set => SetProperty(ref _colorDisplayName, value);
    }

    private string _resourceKey;

    public string ResourceKey
    {
        get => _resourceKey;
        set => SetProperty(ref _resourceKey, value);
    }

    private string _hex;

    public string Hex
    {
        get => _hex;
        set => SetProperty(ref _hex, value);
    }
    
    public ColorItemViewModel(string colorDisplayName, ISolidColorBrush brush, string resourceKey, bool light, int index)
    {
        ColorDisplayName = colorDisplayName;
        Brush = brush;
        ResourceKey = resourceKey;
        Hex = brush.ToString().ToUpperInvariant();
        if ((light && index < 5) || (!light && index > 5))
        {
            TextBrush = Brushes.Black;
        }
        else
        {
            TextBrush = Brushes.White;
        }
    }
}

public class FunctionalColorGroupViewModel : ObservableObject
{
    private string _title;
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public ObservableCollection<ColorItemViewModel> LightColors { get; set; } = new();
    public ObservableCollection<ColorItemViewModel> DarkColors { get; set; } = new();

    public FunctionalColorGroupViewModel(string title, IResourceDictionary lightDictionary, IResourceDictionary darkDictionary, IReadOnlyList<Tuple<string, string>> tokens)
    {
        Title = title;
        foreach (var token in tokens)
        {
            string key = token.Item1;
            string name = token.Item2;
            if (lightDictionary.TryGetValue(key, out var lightValue))
            {
                if (lightValue is ISolidColorBrush lightBrush)
                {
                    LightColors.Add(new ColorItemViewModel(name, lightBrush, key, true, 0));
                }
            }
            if (darkDictionary.TryGetValue(key, out var darkValue))
            {
                if (darkValue is ISolidColorBrush darkBrush)
                {
                    DarkColors.Add(new ColorItemViewModel(name, darkBrush, key, true, 0));
                }
            }
        }
    }
}

public static class ColorTokens
{
    public static IReadOnlyList<Tuple<string, string>> PrimaryTokens { get; } = new List<Tuple<string, string>>
    {
        new ("SemiColorPrimary", "Primary"),
        new ("SemiColorPrimaryPointerover", "Primary Pointerover"),
        new ("SemiColorPrimaryPressed", "Primary Pressed"),
        new ("SemiColorPrimaryDisabled", "Primary Disabled"),
        new ("SemiColorPrimaryLight", "Primary Light"),
        new ("SemiColorPrimaryLightPointerover", "Primary Light Pointerover"),
        new ("SemiColorPrimaryLightActive", "Primary Light Active"),
    };
}