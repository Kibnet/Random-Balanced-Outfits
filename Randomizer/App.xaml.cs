using System.Windows;

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
			Модель = (ModelView) en.Value;
		}
	}
}