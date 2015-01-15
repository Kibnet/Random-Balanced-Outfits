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
			name.Focus();
			if (editobj != null)
			{
				Obj = editobj;
				name.Text = editobj.Название;
				hours.Value = editobj.Длительность;
				seal.IsChecked = editobj.Усиление;
			}
		}

		private void Сохранить(object sender, RoutedEventArgs e)
		{
			if (Obj == null)
			{
				Obj = new Наряд
				      {
					      Название = name.Text,
					      Длительность = (int) hours.Value.GetValueOrDefault(0),
					      Усиление = seal.IsChecked == true
				      };
			}
			else
			{
				Obj.Название = name.Text;
				Obj.Длительность = (int) hours.Value.GetValueOrDefault(0);
				Obj.Усиление = seal.IsChecked == true;
			}

			Close();
		}
	}
}