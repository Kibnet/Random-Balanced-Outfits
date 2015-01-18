using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Randomizer
{
	/// <summary>
	///     Логика взаимодействия для NewDistrict.xaml
	/// </summary>
	public partial class NewDistrict
	{
		public Подразделение Obj;

		public NewDistrict(IEnumerable<Наряд> naryads, Подразделение editobj = null)
		{
			InitializeComponent();
			NameBox.Focus();
			if (editobj != null)
			{
				if (editobj.Наряды != null)
				{
					foreach (var naryad in naryads)
					{
						NarydsPanel.Children.Add(
							new CheckBox {Content = naryad, IsChecked = editobj.Наряды.IndexOf(naryad) != -1});
					}
				}
				Obj = editobj;
				NameBox.Text = editobj.Название;
				PeopleBox.Value = editobj.Люди;
			}
			else
			{
				foreach (var naryad in naryads)
				{
					NarydsPanel.Children.Add(
						new CheckBox {Content = naryad, IsChecked = false});
				}
			}
		}

		private void Save(object sender, RoutedEventArgs e)
		{
			var list = (from CheckBox child in NarydsPanel.Children
				where child.IsChecked == true
				select (Наряд) child.Content).ToList();
			if (Obj == null)
			{
				Obj = new Подразделение();
			}
			Obj.Наряды = new ObservableCollection<Наряд>(list);
			Obj.Название = NameBox.Text;
			Obj.Люди = (int) PeopleBox.Value.GetValueOrDefault(0);

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