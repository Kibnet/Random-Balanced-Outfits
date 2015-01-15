using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Randomizer
{
	[Serializable]
	public class Подразделение
	{
		public Подразделение()
		{
			Наряды = new ObservableCollection<Наряд>();
		}

		public ObservableCollection<Наряд> Наряды { get; set; }

		public string Название { get; set; }

		public int Люди { get; set; }

        public string Процент { get; set; }

        public int ЧасыПредвар { get; set; }

        public int ВыходныеЧасыПредвар { get; set; }

        public int Распред12Ч { get; set; }

        public int Распред24Ч { get; set; }

		public int Часы { get; set; }
        
		public int ВыходныеЧасы { get; set; }

		public string СписокНарядов
		{
			get
			{
				return Наряды != null
					? Наряды.Aggregate("", (current, наряд) => string.Format("{0}{1}; ", current, наряд.Название))
					: "";
			}
		}

		public override string ToString()
		{
			return Название;
		}
	}
}