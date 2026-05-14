using System.Windows;
using WpfApp4.ViewModels;

namespace WpfApp4;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var viewModel = new MainViewModel();
        DataContext = viewModel;

        Loaded += async (_, _) =>
        {
            try
            {
                await viewModel.LoadUsersAsync();
            }
            catch (Exception ex)
            {
                viewModel.StatusMessage = $"Ошибка загрузки пользователей: {ex.Message}";
            }
        };
    }
}
