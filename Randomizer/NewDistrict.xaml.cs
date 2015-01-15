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
				HoursBox.Value = editobj.ЧасыПредвар;
				HolyHoursBox.Value = editobj.ВыходныеЧасыПредвар;
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
			List<Наряд> list = (from CheckBox child in NarydsPanel.Children
				where child.IsChecked == true
				select (Наряд) child.Content).ToList();
			if (Obj == null)
			{
				Obj = new Подразделение
				      {
					      Наряды = new ObservableCollection<Наряд>(list),
					      Название = NameBox.Text,
					      Люди = (int) PeopleBox.Value.GetValueOrDefault(0),
					      ЧасыПредвар = (int) HoursBox.Value.GetValueOrDefault(0),
                          ВыходныеЧасыПредвар = (int) HolyHoursBox.Value.GetValueOrDefault(0)
				      };
			}
			else
			{
				Obj.Наряды = new ObservableCollection<Наряд>(list);
				Obj.Название = NameBox.Text;
				Obj.Люди = (int) PeopleBox.Value.GetValueOrDefault(0);
				Obj.ЧасыПредвар = (int) HoursBox.Value.GetValueOrDefault(0);
				Obj.ВыходныеЧасыПредвар = (int) HolyHoursBox.Value.GetValueOrDefault(0);
			}

			Close();
		}
	}
}