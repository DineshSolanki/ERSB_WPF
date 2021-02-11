using System.Windows.Controls;
using System.Windows.Input;

namespace ERSB.Views
{
    /// <summary>
    /// Interaction logic for AboutBox
    /// </summary>
    public partial class AboutBox : UserControl
    {
        public AboutBox()
        {
            InitializeComponent();
        }
        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }
    }
}
