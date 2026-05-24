using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MusicPlayer.ViewModels;
using Track = MusicPlayer.Models.Track;

namespace MusicPlayer.Views;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _vm;
    public MainWindowViewModel ViewModel => _vm;

    public MainWindow()
    {
        InitializeComponent();
        _vm = new MainWindowViewModel();
        DataContext = _vm;
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        => _vm.Dispose();


    private void TrackItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is ListViewItem { DataContext: Track track })
            _vm.SelectTrackCommand.Execute(track);
    }

    private void TrackItem_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && sender is ListViewItem { DataContext: Track track })
        {
            _vm.SelectTrackCommand.Execute(track);
            e.Handled = true;
        }
    }

    private void SeekSlider_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        if (sender is Slider slider)
            _vm.PositionSeconds = slider.Value;
    }

    private bool _seeking;

    private void SeekSlider_DragStarted(object sender, DragStartedEventArgs e)
        => _seeking = true;

    private void SeekSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!_seeking && sender is Slider { IsMouseOver: true } slider)
            _vm.PositionSeconds = slider.Value;
    }
}
