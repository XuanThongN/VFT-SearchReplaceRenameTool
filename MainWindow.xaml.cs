using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace SearchReplaceRename
{
  /// <summary>
  ///   Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    private void btnChooseFile_Click(object sender, RoutedEventArgs e)
    {
      var dialog = new FolderBrowserDialog();

      if (System.Windows.Forms.DialogResult.OK == dialog.ShowDialog())
        tbPath.Text = dialog.SelectedPath;
    }

    private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
      CheckIfReadyToGo();
    }

    private void tbReplace_TextChanged(object sender, TextChangedEventArgs e)
    {
      CheckIfReadyToGo();
    }

    private void tbPath_TextChanged(object sender, TextChangedEventArgs e)
    {
      CheckIfReadyToGo();
    }

    private void CheckIfReadyToGo()
    {
      btnGo.IsEnabled = 0 != tbSearch.Text.Length && 0 != tbReplace.Text.Length && 0 != tbPath.Text.Length;
    }

    private void btnGo_Click(object sender, RoutedEventArgs e)
    {
      if (MessageBoxResult.Yes ==
          MessageBox.Show(
            "Bạn có chắc chắn muốn tiếp tục",
            "Cảnh báo!", MessageBoxButton.YesNo))
      {
        var dialog = new ConsoleWindow(tbPath.Text, tbSearch.Text, tbReplace.Text);
        dialog.ShowDialog();
      }
    }
  }
}