using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.IO;

namespace SampleWPF_CHeader
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	/// 
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Item1_Click(object sender, RoutedEventArgs e)
		{
			// Navigate to the subpage
			contentFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden; // Hide history bar
			contentFrame.Navigate(new MacrosPage());
		}

		private void Item2_Click(object sender, RoutedEventArgs e)
		{
			// Navigate to the subpage
			contentFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden; // Hide history bar
			contentFrame.Navigate(new MacrosPage());
		}

		private void Item3_Click(object sender, RoutedEventArgs e)
		{
			// Navigate to the subpage
			contentFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden; // Hide history bar
			contentFrame.Navigate(new MacrosPage());
		}
	}

}
