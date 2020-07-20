﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;

namespace Lab2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        /// <summary>
        /// Лист угроз.
        /// </summary>
        public static List<Threat> threats = new List<Threat>();
        /// <summary>
        /// Путь к локальной базе данных.
        /// </summary>
        public readonly string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\Волейко_Lab2\thrlist.xlsx";
        public readonly string pathTxt = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\Волейко_Lab2\localDB.txt";
        /// <summary>
        /// Страница DataGrid.
        /// </summary>
        internal int page = 0;
        /// <summary>
        /// Количество записей, показываемых на странице.
        /// </summary>
        internal int threatsCount;

        public MainWindow()
        {
            InitializeComponent();
            threatsCount = 15;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bool downloadedStatus = false;
            while (!downloadedStatus)
            {
                try
                {
                    if (!File.Exists(path))
                    {
                        if (MessageBox.Show("Локальной базы данных не обнаружено. Хотите провести первичную загрузку данных?", "Отсутствие локальной базы данных", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                        {
                            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\Волейко_Lab2");
                            WebClient webClient = new WebClient();
                            webClient.DownloadFile(@"https://bdu.fstec.ru/files/documents/thrlist.xlsx", path);
                            MessageBox.Show("На рабочем столе была создана папка 'Волейко_Lab2'!", "Обратите внимание!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            /*Microsoft.Office.Interop.Excel.Application xlsApplication = new Microsoft.Office.Interop.Excel.Application();
                            Workbook xlsWorkbook = xlsApplication.Workbooks.Open(path);
                            Worksheet xlsWorksheet = xlsWorkbook.Worksheets["Sheet"];
                            Range xlsRange = xlsWorksheet.UsedRange;
                            for (int i = 3; i <= xlsRange.Rows.Count; i++)
                            {
                                if (xlsRange.Cells[i, 1].Text != "")
                                {
                                    threats.Add(new Threat(xlsRange.Cells[i, 1].Text,
                                        xlsRange.Cells[i, 2].Text,
                                        xlsRange.Cells[i, 3].Text,
                                        xlsRange.Cells[i, 4].Text,
                                        xlsRange.Cells[i, 5].Text,
                                        xlsRange.Cells[i, 6].Text,
                                        xlsRange.Cells[i, 7].Text,
                                        xlsRange.Cells[i, 8].Text));
                                }
                                else
                                {
                                    break;
                                }
                            }
                            xlsWorkbook.Close();
                            xlsApplication.Quit();
                            string localDataBase = "";
                            foreach (var threat in threats)
                            {
                                localDataBase += threat.ToString() + "@";
                            }
                            File.WriteAllText(pathTxt, localDataBase);
                            File.Delete(path);*/
                        }
                    }
                    downloadedStatus = true;
                }
                catch (WebException webEx)
                {
                    if (MessageBox.Show("Проблемы с Интернет-соединением! Проверьте подключение и попробуйте ещё раз, нажав кнопку 'ОК'!", webEx.GetType().Name, MessageBoxButton.OKCancel, MessageBoxImage.Error) == MessageBoxResult.Cancel)
                    {
                        Environment.Exit(0);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            Microsoft.Office.Interop.Excel.Application xlsApplication = new Microsoft.Office.Interop.Excel.Application();
            Workbook xlsWorkbook = xlsApplication.Workbooks.Open(path);
            Worksheet xlsWorksheet = xlsWorkbook.Worksheets["Sheet"];
            Range xlsRange = xlsWorksheet.UsedRange;
            for (int i = 3; i <= xlsRange.Rows.Count; i++)
            {
                if (xlsRange.Cells[i, 1].Text != "")
                {
                    threats.Add(new Threat(xlsRange.Cells[i, 1].Text,
                        xlsRange.Cells[i, 2].Text,
                        xlsRange.Cells[i, 3].Text,
                        xlsRange.Cells[i, 4].Text,
                        xlsRange.Cells[i, 5].Text,
                        xlsRange.Cells[i, 6].Text,
                        xlsRange.Cells[i, 7].Text,
                        xlsRange.Cells[i, 8].Text));
                }
                else
                {
                    break;
                }
            }
            xlsWorkbook.Close();
            xlsApplication.Quit();
            /*threats.Clear();
            using (StreamReader streamReader = new StreamReader(pathTxt))
            {
                string line = streamReader.ReadToEnd();
                for (int i = 0; i < line.Split('@').Length - 1; i++)
                {
                    string temp = line.Split('@')[i];
                    threats.Add(new Threat(temp.Split('♪')[0],
                       temp.Split('♪')[1],
                       temp.Split('♪')[2],
                       temp.Split('♪')[3],
                       temp.Split('♪')[4],
                       temp.Split('♪')[5],
                       temp.Split('♪')[6],
                       temp.Split('♪')[7]));
                }
                streamReader.Close();
            }*/
            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Идентификатор угрозы",
                Binding = new Binding("ID")
            });
            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Наименование угрозы",
                Binding = new Binding("Name")
            });
            RefreshDataGrid();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            List<Threat> oldThreats = new List<Threat>(threats);
            bool downloadedStatus = false;
            while (!downloadedStatus)
            {
                try
                {
                    if (!File.Exists(path))
                    {
                        if (MessageBox.Show("Локальной базы данных не обнаружено. Хотите провести загрузку данных?", "Отсутствие локальной базы данных", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                        {
                            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\Волейко_Lab2");
                            WebClient webClient = new WebClient();
                            webClient.DownloadFile(@"https://bdu.fstec.ru/files/documents/thrlist.xlsx", path);
                            /*MessageBox.Show("На рабочем столе была создана папка 'Волейко_Lab2'!", "Обратите внимание!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            Microsoft.Office.Interop.Excel.Application xlsApplication = new Microsoft.Office.Interop.Excel.Application();
                            Workbook xlsWorkbook = xlsApplication.Workbooks.Open(path);
                            Worksheet xlsWorksheet = xlsWorkbook.Worksheets["Sheet"];
                            Range xlsRange = xlsWorksheet.UsedRange;
                            for (int i = 3; i <= xlsRange.Rows.Count; i++)
                            {
                                if (xlsRange.Cells[i, 1].Text != "")
                                {
                                    threats.Add(new Threat(xlsRange.Cells[i, 1].Text,
                                        xlsRange.Cells[i, 2].Text,
                                        xlsRange.Cells[i, 3].Text,
                                        xlsRange.Cells[i, 4].Text,
                                        xlsRange.Cells[i, 5].Text,
                                        xlsRange.Cells[i, 6].Text,
                                        xlsRange.Cells[i, 7].Text,
                                        xlsRange.Cells[i, 8].Text));
                                }
                                else
                                {
                                    break;
                                }
                            }
                            xlsWorkbook.Close();
                            xlsApplication.Quit();
                            string localDataBase = "";
                            foreach (var threat in threats)
                            {
                                localDataBase += threat.ToString() + "@";
                            }
                            File.WriteAllText(pathTxt, localDataBase);
                            File.Delete(path);*/
                        }
                    }
                    /*else
                    {
                        WebClient webClient = new WebClient();
                        webClient.DownloadFile(@"https://bdu.fstec.ru/files/documents/thrlist.xlsx", path);
                        Microsoft.Office.Interop.Excel.Application xlsApplication = new Microsoft.Office.Interop.Excel.Application();
                        Workbook xlsWorkbook = xlsApplication.Workbooks.Open(path);
                        Worksheet xlsWorksheet = xlsWorkbook.Worksheets["Sheet"];
                        Range xlsRange = xlsWorksheet.UsedRange;
                        for (int i = 3; i <= xlsRange.Rows.Count; i++)
                        {
                            if (xlsRange.Cells[i, 1].Text != "")
                            {
                                threats.Add(new Threat(xlsRange.Cells[i, 1].Text,
                                    xlsRange.Cells[i, 2].Text,
                                    xlsRange.Cells[i, 3].Text,
                                    xlsRange.Cells[i, 4].Text,
                                    xlsRange.Cells[i, 5].Text,
                                    xlsRange.Cells[i, 6].Text,
                                    xlsRange.Cells[i, 7].Text,
                                    xlsRange.Cells[i, 8].Text));
                            }
                            else
                            {
                                break;
                            }
                        }
                        xlsWorkbook.Close();
                        xlsApplication.Quit();
                        string localDataBase = "";
                        foreach (var threat in threats)
                        {
                            localDataBase += threat.ToString() + "@";
                        }
                        File.WriteAllText(pathTxt, localDataBase);
                        File.Delete(path);
                    }*/
                    downloadedStatus = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            Microsoft.Office.Interop.Excel.Application xlsApplication = new Microsoft.Office.Interop.Excel.Application();
            Workbook xlsWorkbook = xlsApplication.Workbooks.Open(path);
            Worksheet xlsWorksheet = xlsWorkbook.Worksheets["Sheet"];
            Range xlsRange = xlsWorksheet.UsedRange;
            try
            {
                uint countNewThreats = 0;
                /*using (StreamReader streamReader = new StreamReader(pathTxt))
                {
                    string line = streamReader.ReadToEnd();*/
                for (int i = 0; i < threats.Count; i++)
                {
                    if (xlsRange.Cells[i + 3, 1].Text != "")
                    {
                        /*byte privacy;
                        byte integrity;
                        byte availability;
                        if (threats[i].Privacy)
                        {
                            privacy = 1;
                        }
                        else
                        {
                            privacy = 0;
                        }
                        if (threats[i].Integrity)
                        {
                            integrity = 1;
                        }
                        else
                        {
                            integrity = 0;
                        }
                        if (threats[i].Availability)
                        {
                            availability = 1;
                        }
                        else
                        {
                            availability = 0;
                        }*/
                        string privacy;
                        string integrity;
                        string availability;
                        if (threats[i].Privacy)
                        {
                            privacy = "1";
                        }
                        else
                        {
                            privacy = "0";
                        }
                        if (threats[i].Integrity)
                        {
                            integrity = "1";
                        }
                        else
                        {
                            integrity = "0";
                        }
                        if (threats[i].Availability)
                        {
                            availability = "1";
                        }
                        else
                        {
                            availability = "0";
                        }
                        if (threats[i].ID.ToString().Split('.')[1] != xlsRange.Cells[i + 3, 1].Text ||
                                threats[i].Name.ToString() != xlsRange.Cells[i + 3, 2].Text ||
                                threats[i].Description.ToString() != xlsRange.Cells[i + 3, 3].Text ||
                                threats[i].Source.ToString() != xlsRange.Cells[i + 3, 4].Text ||
                                threats[i].ImpactObject.ToString() != xlsRange.Cells[i + 3, 5].Text ||
                                privacy.ToString() != xlsRange.Cells[i + 3, 6].Text ||
                                integrity.ToString() != xlsRange.Cells[i + 3, 7].Text ||
                                availability.ToString() != xlsRange.Cells[i + 3, 8].Text)
                        {
                            threats.RemoveAt(i);
                            threats.Insert(i, new Threat(xlsRange.Cells[i + 3, 1].Text,
                            xlsRange.Cells[i + 3, 2].Text,
                            xlsRange.Cells[i + 3, 3].Text,
                            xlsRange.Cells[i + 3, 4].Text,
                            xlsRange.Cells[i + 3, 5].Text,
                            xlsRange.Cells[i + 3, 6].Text,
                            xlsRange.Cells[i + 3, 7].Text,
                            xlsRange.Cells[i + 3, 8].Text));
                        }
                    }
                    else
                    {
                        break;
                    }
                    /*string privacy;
                    string integrity;
                    string availability;
                    if (threats[i].Privacy)
                    {
                        privacy = "True";
                    }
                    else
                    {
                        privacy = "False";
                    }
                    if (threats[i].Integrity)
                    {
                        integrity = "True";
                    }
                    else
                    {
                        integrity = "False";
                    }
                    if (threats[i].Availability)
                    {
                        availability = "True";
                    }
                    else
                    {
                        availability = "False";
                    }
                    string temp = line.Split('@')[i];
                    if (threats[i].ID.ToString() != temp.Split('♪')[0] ||
                            threats[i].Name.ToString() != temp.Split('♪')[1] ||
                            threats[i].Description.ToString() != temp.Split('♪')[2] ||
                            threats[i].Source.ToString() != temp.Split('♪')[3] ||
                            threats[i].ImpactObject.ToString() != temp.Split('♪')[4] ||
                            privacy.ToString() != temp.Split('♪')[5] ||
                            integrity.ToString() != temp.Split('♪')[6] ||
                            availability.ToString() != temp.Split('♪')[7])
                    {
                        threats.RemoveAt(i);
                        threats.Insert(i, new Threat(temp.Split('♪')[0],
                   temp.Split('♪')[1],
                   temp.Split('♪')[2],
                   temp.Split('♪')[3],
                   temp.Split('♪')[4],
                   temp.Split('♪')[5],
                   temp.Split('♪')[6],
                   temp.Split('♪')[7]));
                    }
                }
                streamReader.Close();
                for (int i = threats.Count; line.Split('@')[i] != ""; i++)
                {
                    countNewThreats++;
                    string temp = line.Split('@')[i];
                    threats.Add(new Threat(temp.Split('♪')[0],
                       temp.Split('♪')[1],
                       temp.Split('♪')[2],
                       temp.Split('♪')[3],
                       temp.Split('♪')[4],
                       temp.Split('♪')[5],
                       temp.Split('♪')[6],
                       temp.Split('♪')[7]));
                }
            }*/
                }

                var oldDifferences = oldThreats.Except(threats);
                var newDifferences = threats.Except(oldThreats);
                MessageBox.Show($"Количество новых угроз: {countNewThreats}.\nОни добавлены в конце.", "Новые угрозы", MessageBoxButton.OK, MessageBoxImage.Information);
                beforeLabel.IsEnabled = true;
                beforeLabel.Visibility = Visibility.Visible;
                afterLabel.IsEnabled = true;
                afterLabel.Visibility = Visibility.Visible;
                beforeListBox.IsEnabled = true;
                beforeListBox.Visibility = Visibility.Visible;
                beforeListBox.ItemsSource = oldDifferences;
                afterListBox.IsEnabled = true;
                afterListBox.Visibility = Visibility.Visible;
                afterListBox.ItemsSource = newDifferences;
                countDifferenceLabel.IsEnabled = true;
                countDifferenceLabel.Visibility = Visibility.Visible;
                countDifferenceLabel.Content = $"Количество изменений: {oldDifferences.Count().ToString()}";
                countNewLabel.IsEnabled = true;
                countNewLabel.Visibility = Visibility.Visible;
                countNewLabel.Content = $"Количество новых: {countNewThreats}";
                MessageBox.Show("УСПЕШНО", "Обновление", MessageBoxButton.OK, MessageBoxImage.None);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ОШИБКА\n" + ex.Message, "Обновление", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                xlsWorkbook.Close();
                xlsApplication.Quit();
                RefreshDataGrid();
            }
        }
        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (dataGrid.SelectedIndex != -1)
            {
                MessageBox.Show(dataGrid.SelectedItem.ToString());
            }
        }

        private void SaveAsFileButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "Text file (*.txt)|*.txt|Excel file (*.xls)|*.xls"
            };
            string localDataBase = "";
            foreach (var threat in threats)
            {
                localDataBase += threat.ToString() + "\n----------------\n";
            }
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, localDataBase);
            }
        }

        private void LeftPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (page > 0)
            {
                page--;
                pageLabel.Content = (page + 1).ToString();
                RefreshDataGrid();
            }
        }

        private void RightPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (page < threats.Count / threatsCount)
            {
                try
                {
                    page++;
                    pageLabel.Content = (page + 1).ToString();
                    if (page != threats.Count / threatsCount)
                    {
                        RefreshDataGrid();
                    }
                    else
                    {
                        dataGrid.Items.Clear();
                        for (int i = 0; page * threatsCount + i < threats.Count; i++)
                        {
                            dataGrid.Items.Add(threats[page * threatsCount + i]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                    page = 0;
                    pageLabel.Content = (page + 1).ToString();
                    RefreshDataGrid();
                }
            }
        }

        private void CountSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            page = 0;
            pageLabel.Content = (page + 1).ToString();
            threatsCount = (int)countSlider.Value + 15;
            countLabel.Content = "Количество записей: " + threatsCount.ToString();
            RefreshDataGrid();
        }

        public void RefreshDataGrid()
        {
            if (page != threats.Count / threatsCount)
            {
                dataGrid.Items.Clear();
                for (int i = 0; i < threatsCount; i++)
                {
                    dataGrid.Items.Add(threats[page * threatsCount + i]);
                }
            }
            else
            {
                dataGrid.Items.Clear();
                for (int i = 0; page * threatsCount + i < threats.Count; i++)
                {
                    dataGrid.Items.Add(threats[page * threatsCount + i]);
                }
            }
        }
    }
}
