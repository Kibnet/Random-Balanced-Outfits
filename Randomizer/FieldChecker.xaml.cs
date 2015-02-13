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
using System.Windows.Shapes;

namespace Randomizer
{
	/// <summary>
	/// Логика взаимодействия для FieldChecker.xaml
	/// </summary>
	public partial class FieldChecker : Window
	{
		public FieldChecker(string[] toArray)
		{
			InitializeComponent();
			AllCheck.Focus();
			if (toArray != null)
			{
				foreach (var naryad in toArray)
				{
					NarydsPanel.Children.Add(new CheckBox { Content = naryad, IsChecked = false });
				}
			}
		}

		public string[] Selected()
		{
			return (from CheckBox child in NarydsPanel.Children
					where child.IsChecked == true
					select (string)child.Content).ToArray();
		}
		
		private void Save(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
		{
			foreach (CheckBox child in NarydsPanel.Children)
			{
				child.IsChecked = true;
			}
		}

		private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
		{
			foreach (CheckBox child in NarydsPanel.Children)
			{
				child.IsChecked = false;
			}
		}
	}
}
