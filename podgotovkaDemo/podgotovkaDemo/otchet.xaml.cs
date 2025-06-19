using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;

namespace podgotovkaDemo
{
    public partial class otchet : Window
    {
        int emp1;
        public SeriesCollection BarSeriesCollection { get; set; }
        public string[] PartnerLabels { get; set; }

        public otchet(int emp)
        {
            InitializeComponent();
            emp1 = emp;
            LoadData();
            ViewModeComboBox.SelectedIndex = 2;
        }

        private void LoadData()
        {
            string connectionString = @"Data Source=DESKTOP-DC3OGQJ; Initial Catalog=MasterPoll;Integrated Security=True";
            string query = @"SELECT p.name AS PartnerName, sum(r.total_amount) AS TotalAmount 
                         FROM partner p
                         JOIN order_request r ON p.id = r.partner_id 
                         GROUP BY p.name;";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Привязка данных к таблице
                    ReportDataGrid.ItemsSource = dataTable.DefaultView;

                    // Создание данных для графика
                    var partnerNames = new List<string>();
                    var totalAmounts = new ChartValues<decimal>();

                    foreach (DataRow row in dataTable.Rows)
                    {
                        partnerNames.Add(row["PartnerName"].ToString());
                        totalAmounts.Add(Convert.ToDecimal(row["TotalAmount"]));
                    }

                    // Установка данных для гистограммы
                    BarSeriesCollection = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Сумма заказов",
                        Values = totalAmounts
                    }
                };
                    PartnerLabels = partnerNames.ToArray();

                    // Привязка данных к графику
                    BarChart.Series = BarSeriesCollection;
                    BarChart.AxisX[0].Labels = PartnerLabels;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            UserWindow userWindow = new UserWindow(emp1);
            userWindow.Show();
            Close();
        }

        private void ViewModeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ViewModeComboBox.SelectedItem != null)
            {
                string selectedMode = (ViewModeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

                switch (selectedMode)
                {
                    case "Таблица":
                        ReportDataGrid.Visibility = Visibility.Visible;
                        BarChart.Visibility = Visibility.Collapsed;
                        break;

                    case "Гистограмма":
                        ReportDataGrid.Visibility = Visibility.Collapsed;
                        BarChart.Visibility = Visibility.Visible;
                        break;

                    case "Таблица и Гистограмма":
                        ReportDataGrid.Visibility = Visibility.Visible;
                        BarChart.Visibility = Visibility.Visible;
                        break;

                    default:
                        ReportDataGrid.Visibility = Visibility.Collapsed;
                        BarChart.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }

        //private void WordReportButton_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        // Создаем приложение Word
        //        Word.Application wordApp = new Word.Application();
        //        Word.Document doc = wordApp.Documents.Add();

        //        // Добавление заголовка
        //        Word.Paragraph header = doc.Paragraphs.Add();
        //        header.Range.Text = "Отчет по партнерам";
        //        header.Range.Font.Size = 18;
        //        header.Range.Font.Bold = 1;
        //        header.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
        //        header.Range.InsertParagraphAfter();

        //        // Добавление подписи таблицы
        //        Word.Paragraph tableTitle = doc.Paragraphs.Add();
        //        tableTitle.Range.Text = "Таблица данных:";
        //        tableTitle.Range.Font.Size = 14;
        //        tableTitle.Range.Font.Bold = 1;
        //        tableTitle.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
        //        tableTitle.Range.InsertParagraphAfter();

        //        // Добавление таблицы
        //        Word.Paragraph tableParagraph = doc.Paragraphs.Add();
        //        Word.Table table = doc.Tables.Add(tableParagraph.Range, ReportDataGrid.Items.Count + 1, 2);
        //        table.Borders.Enable = 1;

        //        // Заголовки таблицы
        //        table.Cell(1, 1).Range.Text = "Партнер";
        //        table.Cell(1, 2).Range.Text = "Сумма заказов";
        //        table.Rows[1].Range.Font.Bold = 1;

        //        // Заполнение данных таблицы
        //        int rowIndex = 2;
        //        foreach (var item in ReportDataGrid.Items)
        //        {
        //            if (item is DataRowView rowView)
        //            {
        //                DataRow row = rowView.Row;
        //                table.Cell(rowIndex, 1).Range.Text = row["PartnerName"].ToString();
        //                table.Cell(rowIndex, 2).Range.Text = row["TotalAmount"].ToString();
        //                rowIndex++;
        //            }
        //        }

        //        // Добавление пустого абзаца перед гистограммой
        //        Word.Paragraph emptyParagraph = doc.Paragraphs.Add();
        //        emptyParagraph.Range.InsertParagraphAfter();

        //        // Добавление подписи гистограммы
        //        Word.Paragraph chartTitle = doc.Paragraphs.Add();
        //        chartTitle.Range.Text = "Гистограмма данных:";
        //        chartTitle.Range.Font.Size = 14;
        //        chartTitle.Range.Font.Bold = 1;
        //        chartTitle.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
        //        chartTitle.Range.InsertParagraphAfter();

        //        // Сохранение диаграммы как изображения
        //        string tempFilePath = Path.Combine(Path.GetTempPath(), "chart.png");
        //        var encoder = new PngBitmapEncoder();
        //        encoder.Frames.Add(BitmapFrame.Create(BarChart.ToImage()));
        //        using (var fileStream = new FileStream(tempFilePath, FileMode.Create))
        //        {
        //            encoder.Save(fileStream);
        //        }

        //        // Вставка изображения гистограммы в Word
        //        Word.Paragraph chartParagraph = doc.Paragraphs.Add();
        //        chartParagraph.Range.InlineShapes.AddPicture(tempFilePath);
        //        chartParagraph.Range.InsertParagraphAfter();

        //        // Сохранение документа Word
        //        string wordFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Report.docx");
        //        doc.SaveAs2(wordFilePath);
        //        doc.Close();
        //        wordApp.Quit();

        //        MessageBox.Show($"Отчет успешно экспортирован в {wordFilePath}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Ошибка экспорта в Word: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        //private void ExcelReportButton_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        Excel.Application excelApp = new Excel.Application();
        //        Excel.Workbook workbook = excelApp.Workbooks.Add();
        //        Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1];

        //        // Заполняем таблицу
        //        worksheet.Cells[1, 1] = "Партнер";
        //        worksheet.Cells[1, 2] = "Сумма заказов";

        //        int rowIndex = 2;
        //        foreach (var item in ReportDataGrid.Items)
        //        {
        //            if (item is DataRowView rowView)
        //            {
        //                DataRow row = rowView.Row;
        //                worksheet.Cells[rowIndex, 1] = row["PartnerName"].ToString();
        //                worksheet.Cells[rowIndex, 2] = row["TotalAmount"].ToString();
        //                rowIndex++;
        //            }
        //        }

        //        // Сохранение диаграммы как изображения
        //        string tempFilePath = Path.Combine(Path.GetTempPath(), "chart.png");
        //        var encoder = new PngBitmapEncoder();
        //        encoder.Frames.Add(BitmapFrame.Create(BarChart.ToImage()));
        //        using (var fileStream = new FileStream(tempFilePath, FileMode.Create))
        //        {
        //            encoder.Save(fileStream);
        //        }

        //        // Вставка изображения диаграммы в Excel
        //        Excel.Range chartRange = worksheet.Cells[1, 3];
        //        worksheet.Shapes.AddPicture(tempFilePath, 0, (Microsoft.Office.Core.MsoTriState)1, chartRange.Left, chartRange.Top, 300, 200);

        //        // Сохранение файла Excel
        //        string excelFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Report.xlsx");
        //        workbook.SaveAs(excelFilePath);
        //        workbook.Close();
        //        excelApp.Quit();

        //        MessageBox.Show($"Отчет успешно экспортирован в {excelFilePath}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Ошибка экспорта в Excel: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}
    }
}
