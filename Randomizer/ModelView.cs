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
				OnPropertyChanged("ДлительностьРаспределеныхНарядов");
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
				var midload = (double) App.Модель.ДлительностьНарядов / (double) App.Модель.Подразделения.Sum(подразделение => подразделение.Люди);
				var holyload = (double) App.Модель.ДлительностьВыходныхНарядов / (double) App.Модель.Подразделения.Sum(подразделение => подразделение.Люди);

				return string.Format("Распределено {0} часов из {1}. Общая нагрузка {2} ч/чел., на выходных {3} ч/чел.", ДлительностьРаспределеныхНарядов, ДлительностьНарядов, midload.ToString("F2"), holyload.ToString("F2"));
			}
		}

		public int ДлительностьРаспределеныхНарядов
		{
			get
			{
				return Подразделения.Sum(подразделение => подразделение.Часы);
			}
		}

		public string ВсегоНарядов
		{
			get
			{
				OnPropertyChanged("ДлительностьНарядов");
				OnPropertyChanged("ДлительностьВыходныхНарядов");
				OnPropertyChanged("КоэффициэнтВыходных");
				OnPropertyChanged("КоличествоНарядов");
				OnPropertyChanged("КоличествоВыходныхНарядов");
				return string.Format("Всего {3} шт. ({0} часов) из них {4} шт. ({1} часов) выходных ({2}%)",
					ДлительностьНарядов,
					ДлительностьВыходныхНарядов,
					(КоэффициэнтВыходных * 100).ToString("F1"),
					КоличествоНарядов,
					КоличествоВыходныхНарядов);
			}
		}

		public double КоэффициэнтВыходных
		{
			get
			{
				var hproc = 0d;
				if (ДлительностьНарядов != 0)
				{
					hproc = (double)(ДлительностьВыходныхНарядов) / (double)ДлительностьНарядов;
				}
				return hproc;
			}
		}

		public int ДлительностьНарядов
		{
			get
			{
				return Наряды.Sum(наряд => наряд.Всего);
			}
		}
		public int КоличествоНарядов
		{
			get
			{
				return Наряды.Sum(наряд => наряд.Количество);
			}
		}
		public int ДлительностьВыходныхНарядов
		{
			get
			{
				return Наряды.Sum(наряд => наряд.Выходных);
			}
		}
		public int КоличествоВыходныхНарядов
		{
			get
			{
				return Наряды.Sum(наряд => наряд.КоличествоВыходных);
			}
		}
		public void GenerateEvents()
		{
			var dates = ПериодГрафика.Select(date => new ДатаГрафика
			{
				Date = date,
				Смены =
					Наряды.Where(nar => !nar.Усиление || Усиления.Contains(date))
						  .Where(nar => nar.Дни == WeekDays.Все
							  || (nar.Дни == WeekDays.Выходные && Праздники.Contains(date))
							  || (nar.Дни == WeekDays.Будние && !Праздники.Contains(date)))
						  .ToDictionary<Наряд, Наряд, Подразделение>(nar => nar, nar => null)
			}).ToList();
			foreach (var date in dates)
			{
				var find = ДатыГрафика.FirstOrDefault(gr => gr.Date == date.Date);
				if (find != null)
				{
					foreach (var nar in find.Блокировки)
					{
						date.Блокировки.Add(nar);
						if (find.Смены.ContainsKey(nar))
						{
							if (!nar.Усиление || Усиления.Contains(date.Date))
							{
								date.Смены[nar] = find.Смены[nar];
								if (date.Смены[nar] == null)
								{
									date.Смены[nar] = new Подразделение();
								}
							}
						}
					}
				}
			}
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
				foreach (var pair in holypairs)
				{
					var day = pair.Key;
					var smen = pair.Value;
					if (district.ВыходныеЧасыПредвар >= district.ВыходныеЧасы + smen.Длительность)
					{
						day.Смены[smen] = district;
					}
				}

				//Распределение остальных нарядов не превышающих предварительных расчётов
				foreach (var pair in pairs)
				{
					var day = pair.Key;
					var smen = pair.Value;
					if (district.ЧасыПредвар >= district.Часы + smen.Длительность)
					{
						day.Смены[smen] = district;
					}
				}
			}
			RefreshTable();
		}

		public void RefreshTable()
		{
			//OnPropertyChanged("Наряды");
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
					table.Columns.Add("Дата", typeof(string));
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

		public bool ИсключитьБлокированные
		{
			get { return настройки.ИсключитьБлокированные; }
			set
			{
				if (value.Equals(настройки.ИсключитьБлокированные)) return;
				настройки.ИсключитьБлокированные = value;
				OnPropertyChanged("ИсключитьБлокированные");
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
			var handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
