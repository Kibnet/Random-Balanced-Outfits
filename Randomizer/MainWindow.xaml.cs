using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
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
		public MainWindow()
		{
			InitializeComponent();
			try
			{
				narydsGrid.MouseDoubleClick += (sender, args) => РедактироватьНаряд(null, null);
				districtsGrid.MouseDoubleClick += (sender, args) => РедактироватьПодразделение(null, null);
				DateTime fd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1);
				DateTime sd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(2).AddDays(-1);
				graficDates.DisplayDate = fd;
				graficDates.SelectedDates.AddRange(fd, sd);
				App.Модель.ПериодГрафика = new ObservableCollection<DateTime>(graficDates.SelectedDates);
				sealDates.DisplayDate = fd;
				foreach (var dateTime in App.Модель.Усиления)
				{
					sealDates.SelectedDates.Add(dateTime);
				}
				holyDates.DisplayDate = fd;
				if (!App.Модель.Праздники.Any(time => sealDates.SelectedDates.Contains(time)))
				{
					foreach (var date in graficDates.SelectedDates.Where(date => date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday))
					{
						holyDates.SelectedDates.Add(date);
					}
				}
				else
				{
					foreach (var date in App.Модель.Праздники)
					{
						holyDates.SelectedDates.Add(date);
					}
				}
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
				App.Модель.GenerateEvents();
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("{0}\n{1}", ex.Message, ex.StackTrace), "Исключение");
			}
		}

		private void СохранитьНастройки()
		{
			App.Модель.Serialize("Настройки.bin");
		}

		private void КнопкаПересчётаЧасов(object sender, RoutedEventArgs e)
		{
			try
			{
				var peoples = (double) App.Модель.Подразделения.Sum(подразделение => подразделение.Люди);
				//var midload = (double) App.Модель.ДлительностьНарядов/peoples;
				//var holyload = (double) App.Модель.ДлительностьВыходныхНарядов / peoples;

				foreach (Подразделение district in App.Модель.Подразделения)
				{
					var proc = district.Люди/peoples;
					//var toch = (double)krat / (double)district.Люди;

					{
						//var mnoj = (double)midload / (double)toch;
						//var valmnoj = Math.Round(mnoj);
						//var valload = valmnoj * toch;
						//var dihour = (int)(district.Люди * valload);



						//TODO искать кратное из доступных нарядов
						var krat = (double)district.Наряды.Min(наряд => наряд.Длительность);
						var predv = (double)(App.Модель.ДлительностьНарядов) * proc;
						var nars = predv/krat;
						var narsr = Math.Round(nars);
						var hours = narsr*krat;
						district.ЧасыПредвар = (int) hours;
					}
					{
						//var mnoj = (double)holyload / (double)toch;
						//var valmnoj = Math.Round(mnoj);
						//var valload = valmnoj * toch;
						//var dihour = (int)(district.Люди * valload);

						//TODO искать кратное из доступных нарядов
						var krat = (double)district.Наряды.Min(наряд => наряд.Длительность);
						var predv = (double)App.Модель.ДлительностьВыходныхНарядов * proc;
						var nars = predv / krat;
						var narsr = Math.Round(nars);
						var hours = narsr * krat;
						district.ВыходныеЧасыПредвар = (int) hours;
					}

				}

				while (App.Модель.Подразделения.Sum(d => d.ЧасыПредвар) < App.Модель.ДлительностьНарядов)
				{
					var dict = new Dictionary<Подразделение,double>();
					foreach (var d in App.Модель.Подразделения)
					{
						var krat = (double)d.Наряды.Min(наряд => наряд.Длительность);
						var load = (d.ЧасыПредвар + krat)/d.Люди;
						dict[d] = load;
					}
					var min = dict.Min(pair => pair.Value);
					var first = dict.First(pair => pair.Value <= min).Key;
					first.ЧасыПредвар += first.Наряды.Min(наряд => наряд.Длительность);
				}

				while (App.Модель.Подразделения.Sum(d => d.ВыходныеЧасыПредвар) < App.Модель.ДлительностьВыходныхНарядов)
				{
					var dict = new Dictionary<Подразделение, double>();
					foreach (var d in App.Модель.Подразделения)
					{
						var krat = (double)d.Наряды.Min(наряд => наряд.Длительность);
						var load = (d.ВыходныеЧасыПредвар + krat) / d.Люди;
						dict[d] = load;
					}
					var min = dict.Min(pair => pair.Value);
					var first = dict.First(pair => pair.Value <= min).Key;
					first.ВыходныеЧасыПредвар += first.Наряды.Min(наряд => наряд.Длительность);
				}
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
				if (App.Модель == null)
				{
					return;
				}

				Распределено.Content = App.Модель.Распределено;
				App.Модель.ПериодГрафика = new ObservableCollection<DateTime>(graficDates.SelectedDates);
				App.Модель.Усиления = new ObservableCollection<DateTime>(sealDates.SelectedDates);
				App.Модель.Праздники = new ObservableCollection<DateTime>(holyDates.SelectedDates);
				ВсегоНарядов.Content = App.Модель.ВсегоНарядов;
				//App.Модель.RefreshTable();
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
					InitialDirectory = App.Модель.ПутьСохранения,
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

				var table = App.Модель.GenerateTable;

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

						if (App.Модель.Праздники.Contains(fir))
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
					InitialDirectory = App.Модель.ПутьСохранения,
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

				var nar = App.Модель.Подразделения;
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
					App.Модель.Наряды.Add(dlg.Obj);
				}
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
				var dlg = new NewDistrict(App.Модель.Наряды);
				dlg.ShowDialog();
				if (dlg.Obj != null)
				{
					App.Модель.Подразделения.Add(dlg.Obj);
				}
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
						foreach (Подразделение distr in App.Модель.Подразделения)
						{
							distr.Наряды.Remove(selectedItem);
						}
						App.Модель.Наряды.Remove(selectedItem);
					}
				}
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
						App.Модель.Подразделения.Remove(selectedItem);
					}
				}
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
				var dlg = new NewDistrict(App.Модель.Наряды, item);
				dlg.ShowDialog();
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
			App.Модель.RefreshTable();
		}

		private void ВкладкаВывод_OnGotFocus(object sender, RoutedEventArgs e)
		{
			App.Модель.RefreshTable();
		}

		private void ToggleButton_Заблокировать(object sender, RoutedEventArgs e)
		{
			foreach (var data in App.Модель.ДатыГрафика)
			{
				data.Блокировки.Clear();
				foreach (var key in data.Смены.Keys)
				{
					data.Блокировки.Add(key);
				}
			}
			App.Модель.ДатыГрафика = new ObservableCollection<ДатаГрафика>(App.Модель.ДатыГрафика);
		}

		private void ToggleButton_Разблокировать(object sender, RoutedEventArgs e)
		{
			foreach (var data in App.Модель.ДатыГрафика)
			{
				data.Блокировки.Clear();
			}
			App.Модель.ДатыГрафика = new ObservableCollection<ДатаГрафика>(App.Модель.ДатыГрафика);
		}

		private void КнопкаКалькулятора(object sender, RoutedEventArgs e)
		{
			Process.Start("calc");
		}
	}
}