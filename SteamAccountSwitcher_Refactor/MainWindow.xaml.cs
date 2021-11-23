using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SteamAccountSwitcher_Refactor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private SteamAccountManagerView _steamAccountManagerView;

		public MainWindow()
		{
			InitializeComponent();
			Loaded += MainWindow_Loaded;
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			_steamAccountManagerView = new SteamAccountManagerView();

			grdSteam.ItemsSource = _steamAccountManagerView.Children;
		}

		private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			string steamID = (string) ((Image) sender).Tag;

			_steamAccountManagerView.SwitchSteamAccount(steamID);

			Environment.Exit(0);
		}
	}
}
