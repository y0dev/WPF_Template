using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.IO;
using System.ComponentModel;

namespace SampleWPF_CHeader
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class MacrosPage : Page, INotifyPropertyChanged
	{

		private bool hasChanges = false;
		public event PropertyChangedEventHandler PropertyChanged;

		public bool HasChanges
		{
			get { return hasChanges; }
			set
			{
				hasChanges = value;
				OnPropertyChanged("HasChanges");
				saveButton.IsEnabled = value;
			}
		}

		public ObservableCollection<DefineMacro> DefineMacros { get; set; } = new ObservableCollection<DefineMacro>();
		public MacrosPage()
        {
            InitializeComponent(); LoadDefines();
			DataContext = this;
		}

		private void LoadDefines()
		{
			// Read the header file and extract #define macros
			string[] lines = File.ReadAllLines("header.h");
			foreach (string line in lines)
			{
				if (line.StartsWith("#define"))
				{
					string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					if (parts.Length == 3)
					{
						string macroName = parts[1];
						string macroValue = parts[2];
						DefineMacros.Add(new DefineMacro { Name = macroName, Value = macroValue });
					}
				}
			}
		}

		private void buttonUpdate_Click(object sender, RoutedEventArgs e)
		{
			// Update the value of the selected macro
			if (listViewDefines.SelectedItem != null)
			{
				DefineMacro selectedMacro = (DefineMacro)listViewDefines.SelectedItem;
				selectedMacro.Value = textBoxNewValue.Text;
			}
		}

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			// Display confirmation dialog
			MessageBoxResult result = MessageBox.Show("Are you sure you want to save?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

			// If user confirms save
			if (result == MessageBoxResult.Yes)
			{
				// Generate backup file
				GenerateBackupFile();

				// Save the file
				SaveFile();
			}


			MessageBox.Show("Changes saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
			HasChanges = false; // Reset changes flag
		}

		private void GenerateBackupFile()
		{
			// Get the file path
			string filePath = "header.h";

			// Generate backup file name
			string backupFileName = $"{filePath}_{DateTime.Now:yyyyMMddHHmmss}.bak";

			try
			{
				// Copy the original file to the backup file
				File.Copy(filePath, backupFileName);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to generate backup file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void SaveFile()
		{
			// Save file logic goes here
			// For simplicity, let's just display a message
			MessageBox.Show("File saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			// Undo changes logic goes here
			// For simplicity, let's just reset changes flag
			HasChanges = false;
		}

		// Add your logic to track changes, e.g., in text boxes, etc.
		// For demonstration, let's track changes using a sample textbox
		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			HasChanges = true; // Set changes flag when text changes
		}
	}

	public class DefineMacro
	{
		public string Name { get; set; }
		public string Value { get; set; }
	}
}
