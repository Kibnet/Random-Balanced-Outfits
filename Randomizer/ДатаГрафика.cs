using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Syncfusion.Windows.Shared;

namespace Randomizer
{
	[Serializable]
	public class ДатаГрафика : NotificationObject
	{
		public HashSet<Наряд> Блокировки = new HashSet<Наряд>();
		public DateTime Date { get; set; }

		public void Refresh()
		{
			RaisePropertyChanged(() => BackColor);
			RaisePropertyChanged(() => Date);
			RaisePropertyChanged(() => Display);
			RaisePropertyChanged(() => Holyday);
			RaisePropertyChanged(() => Подразделения);
			RaisePropertyChanged(() => Смены);
		}

		public bool Holyday
		{
			get { return App.Модель.Праздники.Contains(Date); }
		}

		public TextBlock Display
		{
			get
			{
				var tb = new TextBlock();
				tb.Inlines.Add(new Run(Date.ToString("yyyy.MM.")));
				tb.Inlines.Add(new Run(Date.ToString("dd")) { FontWeight = FontWeights.ExtraBlack });
				tb.Inlines.Add(new Run(Date.ToString(" ddd")));
				return tb;
			}
		}

		public Dictionary<Наряд, Подразделение> Смены { get; set; }

		
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
					ret.Add(new HostPod
					{
						Nar = наряд,
						Parent = this,
						IsEnabled = Смены.ContainsKey(наряд),
						Locked = Блокировки.Contains(наряд)
					});
				}
				return ret;
			}
		}

		public Brush BackColor
		{
			get
			{
				return Holyday ? Brushes.LightGray : Brushes.Transparent;
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

		public class HostPod : NotificationObject
		{
			private Brush _color;
			private bool _isEnabled;
			private Brush _mark;
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

			public Brush Mark
			{
				get { return _mark; }
				set
				{
					if (Equals(value, _mark)) return;
					_mark = value;
					RaisePropertyChanged(() => Mark);
				}
			}

			public bool IsEnabled
			{
				get { return _isEnabled; }
				set
				{
					if (value.Equals(_isEnabled)) return;
					_isEnabled = value;
					RaisePropertyChanged(() => IsEnabled);
					RaisePropertyChanged(() => Visibility);
				}
			}

			public Наряд Nar { get; set; }

			public void Обновить()
			{
				RaisePropertyChanged(() => IsEnabled);
				RaisePropertyChanged(() => Visibility);
				RaisePropertyChanged(() => Parent);
				RaisePropertyChanged(() => Locked);
				RaisePropertyChanged(() => Color);
			}

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
					RaisePropertyChanged(() => Color);
				}
			}

		}
	}
}