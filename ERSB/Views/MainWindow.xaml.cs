using HandyControl.Data;
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
            if (!(e.OriginalSource is Button button) || !(button.Tag is SkinType tag)) return;
            PopupConfig.IsOpen = false;
            ((App)Application.Current).UpdateSkin(tag);
        }
        #endregion
    }
}
