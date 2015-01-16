using System.Windows;
using System.Windows.Navigation;

namespace Randomizer
{
	/// <summary>
	///     Логика взаимодействия для App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static ModelView Модель;

		private void App_OnStartup(object sender, StartupEventArgs e)
		{
			var en = Resources.GetEnumerator();
			en.MoveNext();
			App.Модель = (ModelView) en.Value;
		}
	}
}