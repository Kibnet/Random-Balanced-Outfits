using System;
using System.ComponentModel;
using System.Linq;
using Randomizer.Annotations;

namespace Randomizer
{
	[Serializable]
	public class Наряд:INotifyPropertyChanged
	{
		private bool _усиление;
		private int _длительность;
		private string _название;

		public string Название
		{
			get { return _название; }
			set
			{
				if (value == _название) return;
				_название = value;
				OnPropertyChanged("Название");
			}
		}

		public int Длительность
		{
			get { return _длительность; }
			set
			{
				if (value == _длительность) return;
				_длительность = value;
				OnPropertyChanged("Длительность");
				OnPropertyChanged("Всего");
				OnPropertyChanged("Выходных");
			}
		}


		public int Количество
		{
			get
			{
				return MainWindow.Модель.ПериодГрафика
					.Count(dateTime => !Усиление || MainWindow.Модель.Усиления.Contains(dateTime));
			}
		}

		public int КоличествоВыходных
		{
			get
			{
				return MainWindow.Модель.ПериодГрафика
					.Where(time => time.Date.DayOfWeek == DayOfWeek.Saturday || time.Date.DayOfWeek == DayOfWeek.Sunday)
					.Count(dateTime => !Усиление || MainWindow.Модель.Усиления.Contains(dateTime));
			}
		}

		public int Всего
		{
			get { return Количество*Длительность; }
		}

		public int Выходных
		{
			get { return КоличествоВыходных * Длительность; }
		}

		public bool Усиление
		{
			get { return _усиление; }
			set
			{
				if (value.Equals(_усиление)) return;
				_усиление = value;
				OnPropertyChanged("Усиление");
				OnPropertyChanged("Количество");
				OnPropertyChanged("КоличествоВыходных");
			}
		}

		public override string ToString()
		{
			return string.Format("{0} ({1} часов{2})", Название, Длительность, Усиление ? " только в Усиление" : "");
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged(string propertyName)
		{
			var handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}