using System;
using System.Windows;

namespace Randomizer
{
	/// <summary>
	///     Логика взаимодействия для NewNaryad.xaml
	/// </summary>
	public partial class NewNaryad
	{
		public Наряд Obj;

		public NewNaryad(Наряд editobj = null)
		{
			InitializeComponent();
			days.ItemsSource = Enum.GetValues(typeof (WeekDays));
			days.SelectedItem = WeekDays.Все;
			name.Focus();
			if (editobj != null)
			{
				Obj = editobj;
				name.Text = editobj.Название;
				hours.Value = editobj.Длительность;
				seal.IsChecked = editobj.Усиление;
				days.SelectedItem = editobj.Дни;
			}
		}

		private void Сохранить(object sender, RoutedEventArgs e)
		{
			if (Obj == null)
				Obj = new Наряд();
			Obj.Название = name.Text;
			Obj.Длительность = (int) hours.Value.GetValueOrDefault(0);
			Obj.Усиление = seal.IsChecked == true;
			Obj.Дни = (WeekDays) days.SelectedItem;
			Close();
		}
	}
}