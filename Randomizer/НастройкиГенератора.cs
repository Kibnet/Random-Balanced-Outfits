using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;

namespace Randomizer
{
	[Serializable]
	public class НастройкиГенератора
	{
		public НастройкиГенератора()
		{
			Наряды = new ObservableCollection<Наряд>();
			Подразделения = new ObservableCollection<Подразделение>();
			Усиления = new ObservableCollection<DateTime>();
			ПериодГрафика = new ObservableCollection<DateTime>();
			Праздники = new ObservableCollection<DateTime>();
			ДатыГрафика = new ObservableCollection<ДатаГрафика>();
			ЗакрываемыеЧасы = 0;
			ПроцентВыходных = 29;
			ПутьСохранения = "C:\\";
		}

		public ObservableCollection<Наряд> Наряды { get; set; }

		public ObservableCollection<Подразделение> Подразделения { get; set; }

		public ObservableCollection<DateTime> Усиления { get; set; }

		public ObservableCollection<DateTime> ПериодГрафика { get; set; }

		public ObservableCollection<DateTime> Праздники { get; set; }

		public ObservableCollection<ДатаГрафика> ДатыГрафика { get; set; }

		public int ЗакрываемыеЧасы { get; set; }

		public int ПроцентВыходных { get; set; }

		public string ПутьСохранения { get; set; }

		public static НастройкиГенератора Deserialize(string filename)
		{
			try
			{
				var fs = new FileStream(filename, FileMode.Open);
				var formatter = new BinaryFormatter();
				var ret = (НастройкиГенератора)formatter.Deserialize(fs);
				fs.Close();
				if (ret.ДатыГрафика == null || 
					ret.Наряды == null || 
					ret.ПериодГрафика == null || 
					ret.Подразделения == null || 
					ret.Праздники == null || 
					ret.Усиления == null)
					throw new Exception();
				return ret;
			}
			catch
			{
				return new НастройкиГенератора();
			}
		}

		public void Serialize(string filename)
		{
			try
			{
				var fs = new FileStream(filename, FileMode.Create);
				var formatter = new BinaryFormatter();
				formatter.Serialize(fs, this);
				fs.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("Настройки не были сохранены!\n{0}", ex.Message), "Ошибка");
			}
		}
	}
}