﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Randomizer.Annotations;

namespace Randomizer
{
	[Serializable]
	public class ДатаГрафика
	{
		public DateTime Date { get; set; }

		public bool Holyday { get { return MainWindow.Модель.Праздники.Contains(Date); } }

		public string Display { get { return Date.ToString("yyyy.MM.dd ddd"); } }

		public Dictionary<Наряд, Подразделение> Смены { get; set; }

		public List<Наряд> Блокировки = new List<Наряд>();

		public ObservableCollection<HostPod> Подразделения
		{
			get
			{
				var ret = new ObservableCollection<HostPod>();
				if (Блокировки == null)
				{
					Блокировки = new List<Наряд>();
				}
				foreach (var наряд in MainWindow.Модель.Наряды)
				{
					ret.Add(new HostPod { Nar = наряд, Parent = this, IsEnabled = Смены.ContainsKey(наряд), Locked = Блокировки.Contains(наряд) });
				}
				return ret;
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

		public class HostPod
		{
			public ДатаГрафика Parent { get; set; }

			public bool Locked
			{
				get { return Parent.Блокировки.Contains(Nar); }
				set
				{
					if (value)
						Parent.Блокировки.Add(Nar);
					else
					{
						if (Parent.Блокировки.Contains(Nar))
							Parent.Блокировки.Remove(Nar);
					}
				}
			}

			public bool IsEnabled { get; set; }

			public Наряд Nar { get; set; }

			public ObservableCollection<Подразделение> All
			{
				get
				{
					var ret = new ObservableCollection<Подразделение>(MainWindow.Модель.Подразделения);
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
		}
	}
}