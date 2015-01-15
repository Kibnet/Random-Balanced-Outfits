using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;

namespace Randomizer
{
	/// <summary>
	///     Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public static ModelView Модель;// = new ModelView("Настройки.blin");

		public MainWindow()
		{
			InitializeComponent();
			try
			{
				narydsGrid.MouseDoubleClick += (sender, args) => РедактироватьНаряд(null, null);
				districtsGrid.MouseDoubleClick += (sender, args) => РедактироватьПодразделение(null, null);
				//DataContext = Модель;
				Модель = MainGrid.DataContext as ModelView;
				DateTime fd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1);
				DateTime sd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(2).AddDays(-1);
				graficDates.DisplayDate = fd;
				graficDates.SelectedDates.AddRange(fd, sd);
				Модель.ПериодГрафика = new ObservableCollection<DateTime>(graficDates.SelectedDates);
				sealDates.DisplayDate = fd;
				foreach (var dateTime in Модель.Усиления)
				{
					sealDates.SelectedDates.Add(dateTime);
				}
				holyDates.DisplayDate = fd;
				if (!Модель.Праздники.Any(time => sealDates.SelectedDates.Contains(time)))
				{
					foreach (var date in graficDates.SelectedDates.Where(date => date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday))
					{
						holyDates.SelectedDates.Add(date);
					}
				}
				else
				{
					foreach (var date in Модель.Праздники)
					{
						holyDates.SelectedDates.Add(date);
					}
				}
				//narydsGrid.DataContext = Модель.Наряды;
				//districtsGrid.DataContext = Модель.Подразделения;
				AllHours.Value = Модель.ЗакрываемыеЧасы;
				HolyProcent.Value = Модель.ПроцентВыходных;
				//eventsGrid.DataContext = Модель.GenerateTable;
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("{0}\n{1}", ex.Message, ex.StackTrace), "Исключение");
			}
		}

		private void Button_Генерировать(object sender, RoutedEventArgs e)
		{
			try
			{
				СохранитьНастройки();
				Модель.GenerateEvents();
				//districtsGrid.DataContext = null;
				//districtsGrid.DataContext = Модель.Подразделения;
				//eventsGrid.DataContext = Модель.GenerateTable;
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("{0}\n{1}", ex.Message, ex.StackTrace), "Исключение");
			}
		}


		private void ПересчитатьНастройки(object sender, RoutedEventArgs e)
		{
			var table = eventsGrid.DataContext as DataTable;
			if (table == null)
				return;
			var dict = new Dictionary<string, List<string>>();
			var dicth = new Dictionary<string, List<string>>();
			for (int i = 0; i < table.Rows.Count; i++)
			{
				for (int j = 1; j < table.Columns.Count; j++)
				{
					var name = table.Columns[j].ColumnName;
					var distr = (string)table.Rows[i].ItemArray[j];
					if (distr == "" || distr == "---")
					{
						continue;
					}
					var outdic = Модель.ДатыГрафика[i].Holyday ? dicth : dict;
					if (!outdic.ContainsKey(distr))
						outdic[distr] = new List<string>();
					outdic[distr].Add(name);
				}
			}
			foreach (var distr in dict)
			{
				ReSettings(distr);
			}
			foreach (var distr in dicth)
			{
				ReSettings(distr, true);
			}
		}
  
		private void ReSettings(KeyValuePair<string, List<string>> distr, bool holy = false)
		{
			var Distr = Модель.Подразделения.First(подразделение => подразделение.Название == distr.Key);
			if (Distr != null)
			{
				if (!holy)
				{
					Distr.Распред12Ч = 0;
					Distr.Распред24Ч = 0;
					Distr.Часы = 0;
					Distr.ВыходныеЧасы = 0;
				}
				foreach (var nar in distr.Value)
				{
					var Nar = Модель.Наряды.First(наряд => наряд.Название == nar);
					if (Nar != null)
					{
						if (Nar.Длительность == 12)
							Distr.Распред12Ч++;
						if (Nar.Длительность == 24)
							Distr.Распред24Ч++;
						Distr.Часы += Nar.Длительность;
						if (holy)
						{
							Distr.ВыходныеЧасы += Nar.Длительность;
						}
					}
				}
			}
		}

		private void СохранитьНастройки()
		{
			try
			{
				//Модель.Наряды = (ObservableCollection<Наряд>)narydsGrid.DataContext;
				//Модель.Подразделения = (ObservableCollection<Подразделение>)districtsGrid.DataContext;
				//Модель.ЗакрываемыеЧасы = (int)AllHours.Value.GetValueOrDefault(0);
				//Модель.ПроцентВыходных = (int)HolyProcent.Value.GetValueOrDefault(0);
				//Модель.Усиления = new ObservableCollection<DateTime>(sealDates.SelectedDates);
				//Модель.Праздники = new ObservableCollection<DateTime>(holyDates.SelectedDates);
				//Модель.ПериодГрафика = new ObservableCollection<DateTime>(graficDates.SelectedDates);
				
				//narydsGrid.DataContext = null;
				//narydsGrid.DataContext = Модель.Наряды;
				//districtsGrid.DataContext = null;
				//districtsGrid.DataContext = Модель.Подразделения;
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("{0}\n{1}", ex.Message, ex.StackTrace), "Исключение");
			}
			Модель.Serialize("Настройки.bin");
		}

		private void КнопкаПересчётаЧасов(object sender, RoutedEventArgs e)
		{
			try
			{
				int mans = Модель.Подразделения.Sum(подразделение => подразделение.Люди);
				var hours = (int)AllHours.Value.GetValueOrDefault(0);
				var holy = (int)HolyProcent.Value.GetValueOrDefault(0);
				float soot = hours / (float)mans;
				foreach (Подразделение district in Модель.Подразделения)
				{
					var dihour = (int)(district.Люди * soot);
					district.ЧасыПредвар = dihour;
					district.ВыходныеЧасыПредвар = (dihour * holy) / 100;
				}
				//districtsGrid.DataContext = null;
				//districtsGrid.DataContext = Модель.Подразделения;
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("{0}\n{1}", ex.Message, ex.StackTrace), "Исключение");
			}
			СохранитьНастройки();
		}

		private void DistrictsGridLayoutUpdated(object sender, EventArgs e)
		{
			try
			{
				if (Модель == null)
				{
					return;
				}
				Распределено.Content = Модель.Распределено;
				Модель.ПериодГрафика = new ObservableCollection<DateTime>(graficDates.SelectedDates);
				Модель.Усиления = new ObservableCollection<DateTime>(sealDates.SelectedDates);
				Модель.Праздники = new ObservableCollection<DateTime>(holyDates.SelectedDates);
				ВсегоНарядов.Content = Модель.ВсегоНарядов;
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("{0}\n{1}", ex.Message, ex.StackTrace), "Исключение");
			}
		}

		private void СохранитьДокумент(object sender, RoutedEventArgs e)
		{
			try
			{
				string name = string.Format("График Нарядов на {0} года",
					graficDates.SelectedDates.FirstOrDefault().ToString("MMMM yyyy"));
				var dlg = new SaveFileDialog
				{
					InitialDirectory = Модель.ПутьСохранения,
					AddExtension = true,
					CheckPathExists = true,
					DefaultExt = "doc",
					OverwritePrompt = true,
					Filter = "Документы Word|*.doc",
					FileName = name,
					Title = "Выберите файл для сохранения"
				};
				if (dlg.ShowDialog() != true)
				{
					return;
				}

				var document = new WordDocument();

				IWSection section = document.AddSection();
				IWParagraph paragraph = section.AddParagraph();
				paragraph.ParagraphFormat.BeforeSpacing = 0f;

				IWTextRange text = paragraph.AppendText(name);
				text.CharacterFormat.Bold = true;
				text.CharacterFormat.FontName = "Cambria";
				text.CharacterFormat.FontSize = 14.0f;
				paragraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Center;

				paragraph = section.AddParagraph();
				paragraph.ParagraphFormat.BeforeSpacing = 2f;

				var table = (DataTable)eventsGrid.DataContext;

				WTextBody textBody = section.Body;
				IWTable docTable = textBody.AddTable();

				//Set the format for rows
				var format = new RowFormat();
				format.Borders.BorderType = BorderStyle.Single;
				format.Borders.LineWidth = 1.0F;
				format.IsAutoResized = true;
				format.Borders.Color = Color.Black;

				//Initialize number of rows and cloumns.
				docTable.ResetCells(table.Rows.Count + 1, table.Columns.Count, format, 74);

				//Repeat the header.
				docTable.Rows[0].IsHeader = true;

				//Format the header rows
				for (int c = 0; c <= table.Columns.Count - 1; c++)
				{
					string[] cols = table.Columns[c].ColumnName.Split('|');
					string colName = cols[cols.Length - 1];
					IWTextRange theadertext = docTable.Rows[0].Cells[c].AddParagraph().AppendText(colName);
					theadertext.CharacterFormat.FontSize = 11f;
					theadertext.CharacterFormat.Bold = true;
					theadertext.OwnerParagraph.ParagraphFormat.BeforeSpacing = 10f;
					docTable.Rows[0].Cells[c].CellFormat.BackColor = Color.Gainsboro;
					docTable.Rows[0].Cells[c].CellFormat.Borders.Color = Color.Black;
					docTable.Rows[0].Cells[c].CellFormat.Borders.BorderType = BorderStyle.Single;
					docTable.Rows[0].Cells[c].CellFormat.Borders.LineWidth = 1.0f;

					docTable.Rows[0].Cells[c].CellFormat.VerticalAlignment =
						Syncfusion.DocIO.DLS.VerticalAlignment.Middle;
				}

				//Format the table body rows
				for (int r = 0; r <= table.Rows.Count - 1; r++)
				{
					var first = (String)(table.Rows[r][0]);
					var fir = DateTime.Parse(first);
					for (int c = 0; c <= table.Columns.Count - 1; c++)
					{
						string svalue = table.Rows[r][c].ToString();
						IWTextRange theadertext = docTable.Rows[r + 1].Cells[c].AddParagraph().AppendText(svalue);
						theadertext.CharacterFormat.FontSize = 10;

						if (Модель.Праздники.Contains(fir))
						{
							docTable.Rows[r + 1].Cells[c].CellFormat.BackColor = Color.DarkGray;
						}
						else
						{
							docTable.Rows[r + 1].Cells[c].CellFormat.BackColor = ((r & 1) == 0)
																				 ? Color.FromArgb(237, 240, 246)
																				 : Color.White;
						}

						docTable.Rows[r + 1].Cells[c].CellFormat.Borders.Color = Color.Black;
						docTable.Rows[r + 1].Cells[c].CellFormat.Borders.BorderType = BorderStyle.Single;
						docTable.Rows[r + 1].Cells[c].CellFormat.Borders.LineWidth = 0.5f;
						docTable.Rows[r + 1].Cells[c].CellFormat.VerticalAlignment =
							Syncfusion.DocIO.DLS.VerticalAlignment.Middle;
					}
				}

				section.PageSetup.Orientation = PageOrientation.Landscape;
				section.PageSetup.Margins.Bottom = 20;
				section.PageSetup.Margins.Top = 20;
				section.PageSetup.Margins.Left = 50;
				section.PageSetup.Margins.Right = 50;
				section.PageSetup.VerticalAlignment = PageAlignment.Top;

				document.Save(dlg.FileName, FormatType.Doc);

				if (
					MessageBox.Show("Вы хотите открыть созданный файл?", "Документ успешно создан",
						MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
				{
					Process.Start(dlg.FileName);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("{0}\n{1}", ex.Message, ex.StackTrace), "Исключение");
			}
			СохранитьНастройки();
		}

		private void СохранитьТаблицу(object sender, RoutedEventArgs e)
		{
			try
			{
				string name = string.Format("Статистика Нарядов на {0} года",
					graficDates.SelectedDates.FirstOrDefault().ToString("MMMM yyyy"));
				var dlg = new SaveFileDialog
				{
					InitialDirectory = Модель.ПутьСохранения,
					AddExtension = true,
					CheckPathExists = true,
					DefaultExt = "doc",
					OverwritePrompt = true,
					Filter = "Документы Word|*.doc",
					FileName = name,
					Title = "Выберите файл для сохранения"
				};
				if (dlg.ShowDialog() != true)
				{
					return;
				}

				var document = new WordDocument();

				IWSection section = document.AddSection();
				IWParagraph paragraph = section.AddParagraph();
				paragraph.ParagraphFormat.BeforeSpacing = 0f;

				IWTextRange text = paragraph.AppendText(name);
				text.CharacterFormat.Bold = true;
				text.CharacterFormat.FontName = "Cambria";
				text.CharacterFormat.FontSize = 14.0f;
				paragraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Center;

				paragraph = section.AddParagraph();
				paragraph.ParagraphFormat.BeforeSpacing = 2f;

				var nar = (ICollection<Подразделение>)districtsGrid.DataContext;
				var table = new DataTable();
				table.Columns.Add("Подразделение");
				table.Columns.Add("Людей, чел");
				table.Columns.Add("Всего, ч");
				table.Columns.Add("Выходных, ч");
				table.Columns.Add("Распред 12ч, шт");
				table.Columns.Add("Распред 24ч, шт");
				table.Columns.Add("Распред Всего, ч");
				table.Columns.Add("Распред Выходных, чт");
				table.Columns.Add("Доступные Наряды");
				foreach (var distr in nar)
				{
					var row = table.NewRow();
					row.ItemArray = new object[]
					{
						distr.Название,
						distr.Люди,
						distr.ЧасыПредвар,
						distr.ВыходныеЧасыПредвар,
						distr.Распред12Ч,
						distr.Распред24Ч,
						distr.Часы,
						distr.ВыходныеЧасы,
						distr.СписокНарядов
					};
					table.Rows.Add(row);
				}
				WTextBody textBody = section.Body;
				IWTable docTable = textBody.AddTable();

				//Set the format for rows
				var format = new RowFormat();
				format.Borders.BorderType = BorderStyle.Single;
				format.Borders.LineWidth = 1.0F;
				format.IsAutoResized = true;
				format.Borders.Color = Color.Black;

				//Initialize number of rows and cloumns.
				docTable.ResetCells(table.Rows.Count + 1, table.Columns.Count, format, 74);

				//Repeat the header.
				docTable.Rows[0].IsHeader = true;

				//Format the header rows
				for (int c = 0; c <= table.Columns.Count - 1; c++)
				{
					string[] cols = table.Columns[c].ColumnName.Split('|');
					string colName = cols[cols.Length - 1];
					IWTextRange theadertext = docTable.Rows[0].Cells[c].AddParagraph().AppendText(colName);
					theadertext.CharacterFormat.FontSize = 11f;
					theadertext.CharacterFormat.Bold = true;
					theadertext.OwnerParagraph.ParagraphFormat.BeforeSpacing = 10f;
					docTable.Rows[0].Cells[c].CellFormat.BackColor = Color.Gainsboro;
					docTable.Rows[0].Cells[c].CellFormat.Borders.Color = Color.Black;
					docTable.Rows[0].Cells[c].CellFormat.Borders.BorderType = BorderStyle.Single;
					docTable.Rows[0].Cells[c].CellFormat.Borders.LineWidth = 1.0f;

					docTable.Rows[0].Cells[c].CellFormat.VerticalAlignment =
						Syncfusion.DocIO.DLS.VerticalAlignment.Middle;
				}

				//Format the table body rows
				for (int r = 0; r <= table.Rows.Count - 1; r++)
				{
					//var first = (DateString)(table.Rows[r][0]);
					for (int c = 0; c <= table.Columns.Count - 1; c++)
					{
						string svalue = table.Rows[r][c].ToString();
						IWTextRange theadertext = docTable.Rows[r + 1].Cells[c].AddParagraph().AppendText(svalue);
						theadertext.CharacterFormat.FontSize = 10;

						//if (Модель.Праздники.Contains(first.Date))
						//{
						//    docTable.Rows[r + 1].Cells[c].CellFormat.BackColor = Color.DarkGray;
						//}
						//else
						{
							docTable.Rows[r + 1].Cells[c].CellFormat.BackColor = ((r & 1) == 0)
																				 ? Color.FromArgb(237, 240, 246)
																				 : Color.White;
						}

						docTable.Rows[r + 1].Cells[c].CellFormat.Borders.Color = Color.Black;
						docTable.Rows[r + 1].Cells[c].CellFormat.Borders.BorderType = BorderStyle.Single;
						docTable.Rows[r + 1].Cells[c].CellFormat.Borders.LineWidth = 0.5f;
						docTable.Rows[r + 1].Cells[c].CellFormat.VerticalAlignment =
							Syncfusion.DocIO.DLS.VerticalAlignment.Middle;
					}
				}

				section.PageSetup.Orientation = PageOrientation.Landscape;
				section.PageSetup.Margins.Bottom = 20;
				section.PageSetup.Margins.Top = 20;
				section.PageSetup.Margins.Left = 50;
				section.PageSetup.Margins.Right = 50;
				section.PageSetup.VerticalAlignment = PageAlignment.Top;

				document.Save(dlg.FileName, FormatType.Doc);

				if (
					MessageBox.Show("Вы хотите открыть созданный файл?", "Документ успешно создан",
						MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
				{
					Process.Start(dlg.FileName);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("{0}\n{1}", ex.Message, ex.StackTrace), "Исключение");
			}
			СохранитьНастройки();
		}

		private void ДобавитьНаряд(object sender, RoutedEventArgs e)
		{
			try
			{
				var dlg = new NewNaryad();
				dlg.ShowDialog();
				if (dlg.Obj != null)
				{
					Модель.Наряды.Add(dlg.Obj);
				}
				//narydsGrid.DataContext = null;
				//narydsGrid.DataContext = Модель.Наряды;
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("{0}\n{1}", ex.Message, ex.StackTrace), "Исключение");
			}
		}

		private void ДобавитьПодразделение(object sender, RoutedEventArgs e)
		{
			try
			{
				var dlg = new NewDistrict(Модель.Наряды);
				dlg.ShowDialog();
				if (dlg.Obj != null)
				{
					Модель.Подразделения.Add(dlg.Obj);
				}
				//districtsGrid.DataContext = null;
				//districtsGrid.DataContext = Модель.Подразделения;
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("{0}\n{1}", ex.Message, ex.StackTrace), "Исключение");
			}
		}

		private void УдалитьНаряд(object sender, RoutedEventArgs e)
		{
			try
			{
				ObservableCollection<object> selected = narydsGrid.SelectedItems;
				if (selected == null || selected.Count <= 0)
				{
					return;
				}
				for (int i = 0; i < selected.Count; i++)
				{
					var selectedItem = selected[i] as Наряд;
					if (selectedItem != null)
					{
						foreach (Подразделение distr in Модель.Подразделения)
						{
							distr.Наряды.Remove(selectedItem);
						}
						Модель.Наряды.Remove(selectedItem);
					}
				}
				//narydsGrid.DataContext = null;
				//narydsGrid.DataContext = Модель.Наряды;
				//districtsGrid.DataContext = null;
				//districtsGrid.DataContext = Модель.Подразделения;
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("{0}\n{1}", ex.Message, ex.StackTrace), "Исключение");
			}
		}

		private void УдалитьПодразделение(object sender, RoutedEventArgs e)
		{
			try
			{
				ObservableCollection<object> selected = districtsGrid.SelectedItems;
				if (selected == null || selected.Count <= 0)
				{
					return;
				}
				for (int i = 0; i < selected.Count; i++)
				{
					var selectedItem = selected[i] as Подразделение;
					if (selectedItem != null)
					{
						Модель.Подразделения.Remove(selectedItem);
					}
				}
				//districtsGrid.DataContext = null;
				//districtsGrid.DataContext = Модель.Подразделения;
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("{0}\n{1}", ex.Message, ex.StackTrace), "Исключение");
			}
		}

		private void РедактироватьНаряд(object sender, RoutedEventArgs e)
		{
			try
			{
				var item = narydsGrid.SelectedItem as Наряд;
				if (item == null)
				{
					return;
				}
				var dlg = new NewNaryad(item);
				dlg.ShowDialog();
				//narydsGrid.DataContext = null;
				//narydsGrid.DataContext = Модель.Наряды;
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("{0}\n{1}", ex.Message, ex.StackTrace), "Исключение");
			}
		}

		private void РедактироватьПодразделение(object sender, RoutedEventArgs e)
		{
			try
			{
				var item = districtsGrid.SelectedItem as Подразделение;
				if (item == null)
				{
					return;
				}
				var dlg = new NewDistrict(Модель.Наряды, item);
				dlg.ShowDialog();
				//districtsGrid.DataContext = null;
				//districtsGrid.DataContext = Модель.Подразделения;
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("{0}\n{1}", ex.Message, ex.StackTrace), "Исключение");
			}
		}

		private void СохранитьНастройки(object sender, RoutedEventArgs e)
		{
			СохранитьНастройки();
		}

		private void ПрименитьИзменения(object sender, RoutedEventArgs e)
		{
			Модель.RefreshTable();
		}

		private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			
		}
	}
}