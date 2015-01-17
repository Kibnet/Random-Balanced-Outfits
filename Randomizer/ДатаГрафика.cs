﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Randomizer.Annotations;

namespace Randomizer
{
	[Serializable]
	public class ДатаГрафика
	{
		public DateTime Date { get; set; }

		public bool Holyday { get { return App.Модель.Праздники.Contains(Date); } }

		public string Display { get { return Date.ToString("yyyy.MM.dd ddd"); } }

		public Dictionary<Наряд, Подразделение> Смены { get; set; }

		public HashSet<Наряд> Блокировки = new HashSet<Наряд>();

		public ObservableCollection<HostPod> Подразделения
		{
			get
			{
				var ret = new ObservableCollection<HostPod>();
				if (Блокировки == null)
				{
					Блокировки = new HashSet<Наряд>();
				}
				foreach (var наряд in App.Модель.Наряды)
				{
					ret.Add(new HostPod { Nar = наряд, Parent = this, IsEnabled = Смены.ContainsKey(наряд), Locked = Блокировки.Contains(наряд) });
				}
				return ret;
			}
		}

		public Brush BackColor
		{
			get
			{
				if (Holyday)
				{
					return Brushes.LightGray;
				}
				else
				{
					return Brushes.Transparent;
				}
			}
		}


		public override string ToString()
		{
			if (Смены == null)
			{
				Смены = new Dictionary<Наряд, Подразделение>();
			}
			return string.Format("{0} {1}({2})", Date.ToString("yyyy.MM.dd dddd"), Holyday ? "H" : "", Смены.Count);
		}

		public class HostPod: INotifyPropertyChanged
		{
			private bool _isEnabled;
			private Brush _color;
			public ДатаГрафика Parent { get; set; }

			public bool Locked
			{
				get
				{
					var res = Parent.Блокировки.Contains(Nar);
					var brush = res ? Brushes.Firebrick.Clone() : Brushes.LawnGreen.Clone();
					brush.Opacity = 0.1;
					Color = brush;
					return res;
				}
				set
				{
					var brush = value ? Brushes.Firebrick.Clone() : Brushes.LawnGreen.Clone();
					brush.Opacity = 0.1;
					Color = brush;

					if (value)
						Parent.Блокировки.Add(Nar);
					else
					{
						if (Parent.Блокировки.Contains(Nar))
							Parent.Блокировки.Remove(Nar);
					}
				}
			}

			public bool IsEnabled
			{
				get { return _isEnabled; }
				set
				{
					if (value.Equals(_isEnabled)) return;
					_isEnabled = value;
					OnPropertyChanged("IsEnabled");
					OnPropertyChanged("Visibility");
				}
			}

			public Наряд Nar { get; set; }

			public ObservableCollection<Подразделение> All
			{
				get
				{
					var ret = new ObservableCollection<Подразделение>(App.Модель.Подразделения);
					ret.Insert(0, new Подразделение());
					return ret;
				}
			}

			public Подразделение Pod
			{
				get
				{
					if (Parent.Смены.ContainsKey(Nar))
					{
						return Parent.Смены[Nar];
					}
					return null;
				}
				set
				{
					if (Parent.Смены.ContainsKey(Nar))
					{
						Parent.Смены[Nar] = value;
					}
				}
			}

			public Visibility Visibility
			{
				get { return IsEnabled ? Visibility.Visible : Visibility.Hidden; }
			}

			public Brush Color
			{
				get { return _color; }
				set
				{
					if (Equals(value, _color)) return;
					_color = value;
					OnPropertyChanged("Color");
				}
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
}