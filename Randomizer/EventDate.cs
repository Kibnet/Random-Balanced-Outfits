using System;
using System.Collections.Generic;

namespace Randomizer
{
	[Serializable]
	public class EventDate
	{
		public DateTime Date { get; set; }

		public bool Holyday { get; set; }

		public Dictionary<Наряд, Подразделение> Смены { get; set; }
	}
}