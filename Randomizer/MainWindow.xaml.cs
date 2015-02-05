using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows;
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
			var ass = Assembly.GetExecutingAssembly().FullName;
			Title += " " +ass.Split(',')[1].Split('=')[1];
			try
			{
				narydsGrid.MouseDoubleClick += (sender, args) => РедактироватьНаряд(null, null);
				districtsGrid.MouseDoubleClick += (sender, args) => РедактироватьПодразделение(null, null);
				
				foreach (var dateTime in App.Модель.ПериодГрафика)
				{
					graficDates.SelectedDates.Add(dateTime);
				}

				if (graficDates.SelectedDates.Count == 0)
				{
					var fd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1);
					var sd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(2).AddDays(-1);
					graficDates.DisplayDate = fd;
					graficDates.SelectedDates.AddRange(fd, sd);
					App.Модель.ПериодГрафика = new ObservableCollection<DateTime>(graficDates.SelectedDates);
					sealDates.DisplayDate = fd;
					sealDates.DisplayDate = fd;
					holyDates.DisplayDate = fd;
					if (!App.Модель.Праздники.Any(time => sealDates.SelectedDates.Contains(time)))
					{
						foreach (
							var date in
								graficDates.SelectedDates.Where(
									date => date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday))
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

				graficDates.DisplayDate = App.Модель.ПериодГрафика.LastOrDefault();
				foreach (var dateTime in App.Модель.Усиления)
				{
					sealDates.SelectedDates.Add(dateTime);
				}
				sealDates.DisplayDate = App.Модель.ПериодГрафика.LastOrDefault();
				foreach (var date in App.Модель.Праздники)
				{
					holyDates.SelectedDates.Add(date);
				}
				holyDates.DisplayDate = App.Модель.ПериодГрафика.LastOrDefault();
				
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
				//СохранитьНастройки();
				if (App.Модель.IsBusy)
				{
					return;
				}
				App.Модель.IsBusy = true;
				GraphList.BeginInit();
				districtsGrid.BeginInit();
				narydsGrid.BeginInit();
				var wor = new BackgroundWorker();
				wor.DoWork+= (o, args) => App.Модель.GenerateEvents();
				wor.RunWorkerCompleted += (o, args) =>
				{
					ОбновитьВсё(null, null);
					GraphList.EndInit();
					districtsGrid.EndInit();
					narydsGrid.EndInit();
					App.Модель.IsBusy = false;
				};
				wor.RunWorkerAsync();
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("{0}\n{1}", ex.Message, ex.StackTrace), "Исключение");
			}
		}

		private void СохранитьНастройки()
		{
			App.Модель.Serialize(App.Модель.SettingsName);
		}

		private void DistrictsGridLayoutUpdated(object sender, EventArgs e)
		{
			try
			{
				if (App.Модель == null || App.Модель.IsBusy)
				{
					return;
				}

				App.Модель.ПериодГрафика = new ObservableCollection<DateTime>(graficDates.SelectedDates);
				App.Модель.Усиления = new ObservableCollection<DateTime>(sealDates.SelectedDates);
				App.Модель.Праздники = new ObservableCollection<DateTime>(holyDates.SelectedDates);
				ВсегоНарядов.Content = App.Модель.ВсегоНарядов;
				ИтогоДаты.Content = App.Модель.ИтогоДаты;
				Распределено.Content = App.Модель.Распределено;
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("{0}\n{1}", ex.Message, ex.StackTrace), "Исключение");
			}
		}

		private void СохранитьГрафикНарядов(object sender, RoutedEventArgs e)
		{
			try
			{
				var name = string.Format("График Нарядов на {0} года",
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

				var document = СформироватьДокумент(name);

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

		private static WordDocument СформироватьДокумент(string name)
		{
			var document = new WordDocument();

			var section = document.AddSection();
			var paragraph = section.AddParagraph();
			paragraph.ParagraphFormat.BeforeSpacing = 0f;

			var text = paragraph.AppendText(name);
			text.CharacterFormat.Bold = true;
			text.CharacterFormat.FontName = "Cambria";
			text.CharacterFormat.FontSize = 14.0f;
			paragraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Center;

			paragraph = section.AddParagraph();
			paragraph.ParagraphFormat.BeforeSpacing = 2f;

			var textBody = section.Body;
			var docTable = textBody.AddTable();

			//Задание свойств строк
			var format = new RowFormat();
			format.Borders.BorderType = BorderStyle.Single;
			format.Borders.LineWidth = 1.0F;
			format.IsAutoResized = true;
			format.Borders.Color = Color.Black;

			//Проставление количества строк и столбцов
			docTable.ResetCells(App.Модель.ДатыГрафика.Count + 1, App.Модель.Наряды.Count + 1, format, 74);

			//Повторение заголовка
			docTable.Rows[0].IsHeader = true;

			//Формирование заголовка
			AddColumnHeader(docTable.Rows[0].Cells[0], "Число");
			for (var c = 0; c < App.Модель.Наряды.Count; c++)
			{
				AddColumnHeader(docTable.Rows[0].Cells[c + 1], App.Модель.Наряды[c].Название);
			}

			//Формирование строк таблицы
			for (var c = 0; c < App.Модель.ДатыГрафика.Count; c++)
			{
				var eventDate = App.Модель.ДатыГрафика[c];
				var row = docTable.Rows[c + 1];

				AddRow(row.Cells[0], eventDate.Date.ToString("dd ddd"));
				row.Cells[0].CellFormat.BackColor = eventDate.Holyday
					? Color.FromArgb(140, 140, 140)
					: Color.FromArgb(230, 230, 230);

				for (var i = 0; i < App.Модель.Наряды.Count; i++)
				{
					var naryad = App.Модель.Наряды[i];
					var add = "---";
					if (eventDate.Смены.ContainsKey(naryad))
					{
						add = eventDate.Смены[naryad] == null ? "" : eventDate.Смены[naryad].Название;
					}
					if (add != null)
					{
						AddRow(row.Cells[i + 1], add);
					}

					if ((i + 1 & 1) == 0)
					{
						row.Cells[i + 1].CellFormat.BackColor = eventDate.Holyday
							? Color.FromArgb(140, 140, 140)
							: Color.FromArgb(230, 230, 230);
					}
					else
					{
						row.Cells[i + 1].CellFormat.BackColor = eventDate.Holyday
							? Color.FromArgb(169, 169, 169)
							: Color.FromArgb(255, 255, 255);
					}
				}
			}

			//Задание параметров страницы
			section.PageSetup.Orientation = PageOrientation.Landscape;
			section.PageSetup.Margins.Bottom = 20;
			section.PageSetup.Margins.Top = 20;
			section.PageSetup.Margins.Left = 50;
			section.PageSetup.Margins.Right = 50;
			section.PageSetup.VerticalAlignment = PageAlignment.Top;
			return document;
		}

		private static void AddColumnHeader(WTableCell cell, string colname)
		{
			var theadertext = cell.AddParagraph().AppendText(colname);
			theadertext.CharacterFormat.FontSize = 11f;
			theadertext.CharacterFormat.Bold = true;
			cell.CellFormat.BackColor = Color.Gainsboro;
			cell.CellFormat.Borders.Color = Color.Black;
			cell.CellFormat.Borders.BorderType = BorderStyle.Single;
			cell.CellFormat.Borders.LineWidth = 1.0f;
			cell.CellFormat.VerticalAlignment = Syncfusion.DocIO.DLS.VerticalAlignment.Middle;
		}

		private static void AddRow(WTableCell cell, string colname)
		{
			var theadertext = cell.AddParagraph().AppendText(colname);
			theadertext.CharacterFormat.FontSize = 10;
			theadertext.CharacterFormat.Bold = true;
			cell.CellFormat.BackColor = Color.Gainsboro;
			cell.CellFormat.Borders.Color = Color.Black;
			cell.CellFormat.Borders.BorderType = BorderStyle.Single;
			cell.CellFormat.Borders.LineWidth = 0.5f;
			cell.CellFormat.VerticalAlignment = Syncfusion.DocIO.DLS.VerticalAlignment.Middle;
		}

		private void СохранитьСтатистику(object sender, RoutedEventArgs e)
		{
			try
			{
				var name = string.Format("Статистика Нарядов на {0} года",
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

				var section = document.AddSection();
				var paragraph = section.AddParagraph();
				paragraph.ParagraphFormat.BeforeSpacing = 0f;

				var text = paragraph.AppendText(name);
				text.CharacterFormat.Bold = true;
				text.CharacterFormat.FontName = "Cambria";
				text.CharacterFormat.FontSize = 14.0f;
				paragraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Center;

				paragraph = section.AddParagraph();
				paragraph.ParagraphFormat.BeforeSpacing = 2f;

				var nar = App.Модель.Подразделения;
				var first = nar.FirstOrDefault().Fields;
				var checker = new FieldChecker(first.Keys.ToArray());
				checker.ShowDialog();
				if (checker.Selected().Length == 0)
				{
					MessageBox.Show("Вы должны выбрать по крайней мере один столбец.", "Не сохранено");
					return;
				}

				string[] selected = checker.Selected();
				checker.Close();
				var table = new DataTable();
				foreach (var o in selected)
				{
					table.Columns.Add(o);
				}
				//table.Columns.Add("Подразделение");
				//table.Columns.Add("Людей, чел");
				//table.Columns.Add("Всего, ч");
				//table.Columns.Add("Выходных, ч");
				//table.Columns.Add("Распред 24ч, шт");
				//table.Columns.Add("Распред 12ч, шт");
				//table.Columns.Add("Распред 4ч, шт");
				//table.Columns.Add("Доступные Наряды");
				foreach (var distr in nar)
				{
					var row = table.NewRow();
					row.ItemArray = distr.Fields.Where(pair => selected.Contains(pair.Key)).Select(pair => pair.Value).ToArray();
					//	new object[]
					//{
					//	distr.Название,
					//	distr.Люди,
					//	distr.Часы,
					//	distr.ВыходныеЧасы,
					//	distr.Распред24Ч,
					//	distr.Распред12Ч,
					//	distr.Распред4Ч,
					//	//distr.СписокНарядов
					//};
					table.Rows.Add(row);
				}
				var textBody = section.Body;
				var docTable = textBody.AddTable();

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
				for (var c = 0; c <= table.Columns.Count - 1; c++)
				{
					var cols = table.Columns[c].ColumnName.Split('|');
					var colName = cols[cols.Length - 1];
					var theadertext = docTable.Rows[0].Cells[c].AddParagraph().AppendText(colName);
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
				for (var r = 0; r <= table.Rows.Count - 1; r++)
				{
					//var first = (DateString)(table.Rows[r][0]);
					for (var c = 0; c <= table.Columns.Count - 1; c++)
					{
						var svalue = table.Rows[r][c].ToString();
						var theadertext = docTable.Rows[r + 1].Cells[c].AddParagraph().AppendText(svalue);
						theadertext.CharacterFormat.FontSize = 10;

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
				MessageBox.Show(string.Format("{0}\n{1}", ex.Message), "Исключение");

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
				var selected = narydsGrid.SelectedItems;
				if (selected == null || selected.Count <= 0)
				{
					return;
				}
				for (var i = 0; i < selected.Count; i++)
				{
					var selectedItem = selected[i] as Наряд;
					if (selectedItem != null)
					{
						foreach (var distr in App.Модель.Подразделения)
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
				var selected = districtsGrid.SelectedItems;
				if (selected == null || selected.Count <= 0)
				{
					return;
				}
				for (var i = 0; i < selected.Count; i++)
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
			AllBlock.Text = "Разблокировать Все";
			BlockPath.Visibility = Visibility.Hidden;
			UnblockPath.Visibility = Visibility.Visible;
		}

		private void ToggleButton_Разблокировать(object sender, RoutedEventArgs e)
		{
			foreach (var data in App.Модель.ДатыГрафика)
			{
				data.Блокировки.Clear();
			}
			App.Модель.ДатыГрафика = new ObservableCollection<ДатаГрафика>(App.Модель.ДатыГрафика);
			AllBlock.Text = "Заблокировать Все";
			BlockPath.Visibility = Visibility.Visible;
			UnblockPath.Visibility = Visibility.Hidden;
		}

		private void КнопкаКалькулятора(object sender, RoutedEventArgs e)
		{
			Process.Start("calc");
		}

		private void ВверхНаряд(object sender, RoutedEventArgs e)
		{
			try
			{
				var selected = narydsGrid.SelectedItems;
				if (selected == null || selected.Count <= 0)
				{
					return;
				}
				foreach (var t in selected)
				{
					var selectedItem = t as Наряд;
					if (selectedItem != null)
					{
						var index = App.Модель.Наряды.IndexOf(selectedItem);
						if (index > 0)
							App.Модель.Наряды.Move(index, index - 1);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("{0}\n{1}", ex.Message, ex.StackTrace), "Исключение");
			}
		}

		private void ВнизНаряд(object sender, RoutedEventArgs e)
		{
			try
			{
				var selected = narydsGrid.SelectedItems;
				if (selected == null || selected.Count <= 0)
				{
					return;
				}
				foreach (var t in selected)
				{
					var selectedItem = t as Наряд;
					if (selectedItem != null)
					{
						var index = App.Модель.Наряды.IndexOf(selectedItem);
						if (index + 1 < App.Модель.Наряды.Count)
							App.Модель.Наряды.Move(index, index + 1);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("{0}\n{1}", ex.Message, ex.StackTrace), "Исключение");
			}
		}

		private void Button_ОбновитьНаряды(object sender, RoutedEventArgs e)
		{
			App.Модель.ОбновитьНаряды();
		}

		private void Button_ОбновитьПодразделения(object sender, RoutedEventArgs e)
		{
			App.Модель.ОбновитьПодразделения();
		}

		private void СохранитьКак(object sender, RoutedEventArgs e)
		{
			var dlg = new SaveFileDialog
			{
				AddExtension = true,
				DefaultExt = ".bin",
				FileName = App.Модель.SettingsName,
				OverwritePrompt = true,
				Filter = "Файл настроек|*.bin"
			};
			if (dlg.ShowDialog() == true)
			{
				App.Модель.Serialize(dlg.FileName);
				App.Модель.SettingsName = dlg.FileName;
			}
		}

		private void ОткрытьКак(object sender, RoutedEventArgs e)
		{
			var dlg = new OpenFileDialog
			{
				AddExtension = true,
				DefaultExt = ".bin",
				FileName = App.Модель.SettingsName,
				Filter = "Файл настроек|*.bin"
			};
			if (dlg.ShowDialog() == true)
			{
				App.Модель.Deserialize(dlg.FileName);
				App.Модель.SettingsName = dlg.FileName;
				ОбновитьВсё(null, null);
			}
		}

		private void ОбновитьВсё(object sender, RoutedEventArgs e)
		{
			graficDates.SelectedDates.Clear();
			sealDates.SelectedDates.Clear();
			holyDates.SelectedDates.Clear();
			foreach (var dateTime in App.Модель.ПериодГрафика)
			{
				graficDates.SelectedDates.Add(dateTime);
			} 
			graficDates.DisplayDate = App.Модель.ПериодГрафика.LastOrDefault();
			foreach (var dateTime in App.Модель.Усиления)
			{
				sealDates.SelectedDates.Add(dateTime);
			}
			sealDates.DisplayDate = App.Модель.ПериодГрафика.LastOrDefault();
			foreach (var date in App.Модель.Праздники)
			{
				holyDates.SelectedDates.Add(date);
			}
			holyDates.DisplayDate = App.Модель.ПериодГрафика.LastOrDefault();

			App.Модель.ОбновитьНаряды();
			App.Модель.ОбновитьПодразделения();
			App.Модель.ОбновитьДаты();
			App.Модель.RefreshTable();
		}
	}
}