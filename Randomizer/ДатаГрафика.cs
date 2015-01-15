using System;
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

		public bool Holyday { get; set; }

		public string Display { get { return Date.ToString("yyyy.MM.dd ddd"); } }

		public Dictionary<Наряд, Подразделение> Смены { get; set; }

		
		public ObservableCollection<HostPod> Подразделения
		{
			get { return new ObservableCollection<HostPod>(Смены.Select(pair => new HostPod() { Nar = pair.Key, Parent = this })); }
		}


		public override string ToString()
		{
			if (Смены == null)
			{
				Смены = new Dictionary<Наряд, Подразделение>();
			}
			return string.Format("{0} {1}({2})", Date.ToString("yyyy.MM.dd dddd"), Holyday ? "H" : "", Смены.Count);
		}

		[Serializable]
		public class HostPod
		{
			public ДатаГрафика Parent { get; set; }

			public Наряд Nar { get; set; }
			public Подразделение Pod
			{
				get { return Parent.Смены[Nar]; }
				set { Parent.Смены[Nar] = value; }
			}
		}
	}
}