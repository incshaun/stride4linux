using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Stride.GameStudio
{
    public partial class AvaloniaMainWindow : Window
    {
        public AvaloniaMainWindow()
        {
            InitializeComponent();
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

    }
}
