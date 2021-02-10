using System.Windows;
using System.Windows.Controls;

namespace ERSB.Views
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Change Skin
        private void ButtonConfig_OnClick(object sender, RoutedEventArgs e) => PopupConfig.IsOpen = true;

        private void ButtonSkins_OnClick(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is not Button button || button.Tag is null) return;
            PopupConfig.IsOpen = false;
            App.UpdateSkin(button.Tag.ToString());
        }
        #endregion
    }
}
