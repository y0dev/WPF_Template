using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.IO;
using System.ComponentModel;
using System.Xml.Linq;

namespace SampleWPF_CHeader
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class MacrosPage : Page
	{

		private bool hasChanges = false;
		private DefineMacro originalMacro;
		private string filePath = "header.h";
		public event PropertyChangedEventHandler PropertyChanged;
		private Dictionary<string, List<string>> validValuesMap = new Dictionary<string, List<string>>();


		public ObservableCollection<DefineMacro> DefineMacros { get; set; } = new ObservableCollection<DefineMacro>();
		public MacrosPage(string filePath)
        {
            InitializeComponent();
			this.filePath = filePath;
			LoadValidValues("config.xml");
			LoadDefines();
			DataContext = this;
		}

		private void LoadValidValues(string configFilePath)
		{
			if (File.Exists(configFilePath))
			{
				string fileName = Path.GetFileName(this.filePath);
				XDocument doc = XDocument.Load(configFilePath);
				foreach (XElement fileElement in doc.Root.Elements("file"))
				{
					string fileNameAttribute = fileElement.Attribute("name")?.Value;
					var macros = new List<string>();
					if(fileNameAttribute == fileName)
					{
						foreach (XElement macroElement in fileElement.Elements("macro"))
						{
							string macroName = macroElement.Attribute("name").Value;
							string allowedValues = macroElement.Attribute("allowedValues").Value;
							validValuesMap.Add(macroName, allowedValues.Split(',').ToList());
						}
					}
				}
			}
		}

		private void LoadDefines()
		{
			// Read the header file and extract #define macros
			string[] lines = File.ReadAllLines(this.filePath);
			foreach (string line in lines)
			{
				if (line.StartsWith("#define"))
				{
					string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					if (parts.Length == 3)
					{
						string macroName = parts[1];
						string macroValue = parts[2];
						List<string> validValues = validValuesMap.ContainsKey(macroName) ? validValuesMap[macroName] : null;
						DefineMacros.Add(new DefineMacro { Name = macroName, Value = macroValue, ValidValues = validValues });
					}
				}
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
			string filePath = this.filePath;
			string[] parts = filePath.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

			// Generate backup file name
			string backupFileName = $"{parts[0]}_{DateTime.Now:yyyyMMddHHmmss}.h.bak";

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
			string filePath = this.filePath;
			try
			{
				// Write the updated macros to the file
				using (StreamWriter writer = new StreamWriter(filePath))
				{
					foreach (DefineMacro macro in dataGridDefines.Items)
					{
						writer.WriteLine($"#define {macro.Name} {macro.Value}");
					}
				}

				MessageBox.Show("File saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch (IOException ex)
			{
				MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			// For simplicity, let's just reset changes flag and revert the edited value
			var selectedMacro = (DefineMacro)dataGridDefines.SelectedItem;
			if (selectedMacro != null && originalMacro != null)
			{
				selectedMacro.Value = originalMacro.Value; // Revert back to the original value
				originalMacro = null; // Clear the original value
			}
			HasChanges = false; // Reset changes flag
		}
		

		private void dataGridDefines_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{
			if (e.EditAction == DataGridEditAction.Commit)
			{
				// Update the source only if the cell is focused
				var comboBox = e.EditingElement as ComboBox;
				if (comboBox != null && comboBox.IsFocused)
				{
					// Store the original value before editing
					originalMacro = new DefineMacro
					{
						Name = ((DefineMacro)dataGridDefines.SelectedItem).Name,
						Value = ((DefineMacro)dataGridDefines.SelectedItem).Value,
						ValidValues = ((DefineMacro)dataGridDefines.SelectedItem).ValidValues
					};

					// Update the selected macro value
					var selectedMacro = (DefineMacro)dataGridDefines.SelectedItem;
					selectedMacro.Value = comboBox.SelectedItem.ToString();
					HasChanges = true; // Set changes flag when text changes
				}
			}
		}

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

	}

	public class DefineMacro: INotifyPropertyChanged
	{
		private string _value;
		public string Name { get; set; }
		public string Value
		{
			get => _value;
			set
			{
				if(_value != value)
				{
					_value = value;
					OnPropertyChanged(nameof(Value));
				}
			}
		}
		public List<string> ValidValues { get; set; }
		public event PropertyChangedEventHandler PropertyChanged;

		public DefineMacro()
		{
			ValidValues = new List<string>();
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
