using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using Randomizer.Annotations;

namespace Randomizer
{
	public class ModelView : INotifyPropertyChanged
	{
		private НастройкиГенератора настройки;

		public ModelView()
		{
			настройки = НастройкиГенератора.Deserialize("Настройки.bin");
		}

		public ModelView(string filename)
		{
			настройки = НастройкиГенератора.Deserialize(filename);
		}

		public ObservableCollection<Наряд> Наряды
		{
			get
			{
				return настройки.Наряды;
			}
			set
			{
				if (Equals(value, настройки.Наряды))
				{
					return;
				}
				настройки.Наряды = value;
				OnPropertyChanged("Наряды");
			}
		}

		public ObservableCollection<Подразделение> Подразделения
		{
			get
			{
				return настройки.Подразделения;
			}
			set
			{
				if (Equals(value, настройки.Подразделения))
				{
					return;
				}
				настройки.Подразделения = value;
				OnPropertyChanged("Подразделения");
				OnPropertyChanged("Распределено");
			}
		}

		public ObservableCollection<DateTime> Усиления
		{
			get
			{
				return настройки.Усиления;
			}
			set
			{
				if (Equals(value, настройки.Усиления))
				{
					return;
				}
				настройки.Усиления = value;
				OnPropertyChanged("Усиления");
			}
		}

		public ObservableCollection<DateTime> ПериодГрафика
		{
			get
			{
				return настройки.ПериодГрафика;
			}
			set
			{
				if (Equals(value, настройки.ПериодГрафика))
				{
					return;
				}
				настройки.ПериодГрафика = value;
				OnPropertyChanged("ПериодГрафика");
			}
		}

		public ObservableCollection<DateTime> Праздники
		{
			get
			{
				return настройки.Праздники;
			}
			set
			{
				if (Equals(value, настройки.Праздники))
				{
					return;
				}
				настройки.Праздники = value;
				OnPropertyChanged("Праздники");
			}
		}

		public ObservableCollection<ДатаГрафика> ДатыГрафика
		{
			get
			{
				return настройки.ДатыГрафика;
			}
			set
			{
				if (Equals(value, настройки.ДатыГрафика))
				{
					return;
				}
				настройки.ДатыГрафика = value;
				OnPropertyChanged("ДатыГрафика");
			}
		}

		public int ЗакрываемыеЧасы
		{
			get
			{
				return настройки.ЗакрываемыеЧасы;
			}
			set
			{
				if (value == настройки.ЗакрываемыеЧасы)
				{
					return;
				}
				настройки.ЗакрываемыеЧасы = value;
				OnPropertyChanged("ЗакрываемыеЧасы");
			}
		}

		public int ПроцентВыходных
		{
			get
			{
				return настройки.ПроцентВыходных;
			}
			set
			{
				if (value == настройки.ПроцентВыходных)
				{
					return;
				}
				настройки.ПроцентВыходных = value;
				OnPropertyChanged("ПроцентВыходных");
			}
		}

		public string ПутьСохранения
		{
			get
			{
				return настройки.ПутьСохранения;
			}
			set
			{
				if (value == настройки.ПутьСохранения)
				{
					return;
				}
				настройки.ПутьСохранения = value;
				OnPropertyChanged("ПутьСохранения");
			}
		}

		public string Распределено
		{
			get
			{
				int sum = Подразделения.Sum(подразделение => подразделение.Часы);
				return string.Format("Распределено {0} часов", sum);
			}
		}

		public string ВсегоНарядов
		{
			get
			{
				int people = Подразделения.Sum(подразделение => подразделение.Люди);
				int sum = Наряды.Sum(наряд => наряд.Всего);
				int count = Наряды.Sum(наряд => наряд.Количество);
				int holysum = Наряды.Sum(наряд => наряд.Выходных);
				int holcount = Наряды.Sum(наряд => наряд.КоличествоВыходных);
				foreach (var подразделение in Подразделения)
				{
					подразделение.Процент = (((float)подразделение.Люди * 100.0f / (float)people)).ToString("F1") + "%";
				}
				int hproc = 0;
				if (sum != 0)
				{
					hproc = (holysum * 100) / sum;
				}
				return string.Format("Всего {3} ({0} часов) из них {4} ({1} часов) выходных ({2}%)", sum, holysum, hproc, count, holcount);
			}
		}

		public void GenerateEvents()
		{
			var dates = ПериодГрафика.Select(date => new ДатаГрафика
			{
				Date = date,
				Holyday = Праздники.Contains(date),
				Смены =
					Наряды.Where(nar => !nar.Усиление || Усиления.Contains(date))
						  .ToDictionary<Наряд, Наряд, Подразделение>(nar => nar, nar => null)
			}).ToList();
			ДатыГрафика = new ObservableCollection<ДатаГрафика>(dates);
			var rand = new Random();
			foreach (var district in Подразделения)
			{
				//Генерация пар дней и нарядов подразделений в случайном порядке. Отдельно выходные и остальные.
				var pairs = new List<KeyValuePair<ДатаГрафика, Наряд>>();
				var holypairs = new List<KeyValuePair<ДатаГрафика, Наряд>>();
				foreach (var day in ДатыГрафика)
				{
					var thisday = day;
					foreach (var smen in district.Наряды.Where(smen => thisday != null && thisday.Смены.ContainsKey(smen) && thisday.Смены[smen] == null))
					{
						if (day.Holyday)
						{
							holypairs.Insert(rand.Next(holypairs.Count), (new KeyValuePair<ДатаГрафика, Наряд>(day, smen)));
						}
						else
						{
							pairs.Insert(rand.Next(pairs.Count), (new KeyValuePair<ДатаГрафика, Наряд>(day, smen)));
						}
					}
				}

				//Проверка что выходных не больше чем всего
				if (district.ЧасыПредвар < district.ВыходныеЧасыПредвар)
				{
					district.ВыходныеЧасыПредвар = district.ЧасыПредвар;
				}

				//Распределение нарядов на выходных не превышающих предварительных расчётов
				district.ВыходныеЧасы = 0;
				district.Распред12Ч = 0;
				district.Распред24Ч = 0;
				foreach (var pair in holypairs)
				{
					var day = pair.Key;
					var smen = pair.Value;
					if (district.ВыходныеЧасыПредвар >= district.ВыходныеЧасы + smen.Длительность)
					{
						day.Смены[smen] = district;
						district.ВыходныеЧасы += smen.Длительность;
						if (smen.Длительность == 12)
							district.Распред12Ч++;
						if (smen.Длительность == 24)
							district.Распред24Ч++;
					}
				}

				//Распределение остальных нарядов не превышающих предварительных расчётов
				district.Часы = district.ВыходныеЧасы;
				foreach (var pair in pairs)
				{
					var day = pair.Key;
					var smen = pair.Value;
					if (district.ЧасыПредвар >= district.Часы + smen.Длительность)
					{
						day.Смены[smen] = district;
						district.Часы += smen.Длительность;
						if (smen.Длительность == 12)
							district.Распред12Ч++;
						if (smen.Длительность == 24)
							district.Распред24Ч++;
					}
				}
			}
			OnPropertyChanged("Подразделения");
			OnPropertyChanged("GenerateTable");
		}
		
		public void RefreshTable()
		{
			OnPropertyChanged("Подразделения");
			OnPropertyChanged("GenerateTable");
		}

		public DataTable GenerateTable
		{
			get
			{
				var table = new DataTable();
				try
				{
					table.Columns.Add("Число", typeof (string));
					foreach (var naryad in Наряды)
					{
						table.Columns.Add(naryad.Название);
					}
					foreach (var eventDate in ДатыГрафика)
					{
						var row = table.NewRow();
						var obj = new List<object>
						          {
							          eventDate.Date.ToString("yyyy.MM.dd ddd")
						          };
						foreach (var naryad in Наряды)
						{
							var add = "---";
							if (eventDate.Смены.ContainsKey(naryad))
							{
								add = eventDate.Смены[naryad] == null ? "" : eventDate.Смены[naryad].Название;
							}
							obj.Add(add);
						}
						row.ItemArray = obj.ToArray();
						table.Rows.Add(row);
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(string.Format("{0}\n{1}", ex.Message, ex.StackTrace), "Исключение");
				}
				return table;
			}
		}
		
		public void Serialize(string filename)
		{
			настройки.Serialize(filename);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
