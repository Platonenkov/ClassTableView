using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using AssemblyGetDataTable;
using MathCore.Annotations;
using Microsoft.Win32;
using Notification.Wpf;
using OpenXmlEx;
using OpenXmlEx.SubClasses;

namespace ClassTableView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Конструкторы
        public MainWindow()
        {
            InitializeComponent();
            var assembly = Assembly.GetEntryAssembly();
            LoadAssembly(assembly, AppDomain.CurrentDomain.BaseDirectory);
        }

        #endregion

        #region Свойства

        #region Assemblies : IEnumerable<AssemblyHelper> - Сборки

        /// <summary>Сборки</summary>
        public static readonly DependencyProperty AssembliesProperty =
            DependencyProperty.Register(
                nameof(Assemblies),
                typeof(IEnumerable<AssemblyInfo>),
                typeof(MainWindow),
                new PropertyMetadata(default(IEnumerable<AssemblyInfo>)));

        /// <summary>Сборки</summary>
        public IEnumerable<AssemblyInfo> Assemblies { get => (IEnumerable<AssemblyInfo>)GetValue(AssembliesProperty); set => SetValue(AssembliesProperty, value); }

        #endregion

        #region SelectedTypeInfo : AssemblyTypeInfo - AssemblyTypeInfo

        /// <summary>AssemblyTypeInfo</summary>
        public static readonly DependencyProperty SelectedTypeInfoProperty =
            DependencyProperty.Register(
                nameof(SelectedTypeInfo),
                typeof(AssemblyTypeInfo),
                typeof(MainWindow),
                new PropertyMetadata(default(AssemblyTypeInfo)));

        /// <summary>AssemblyTypeInfo</summary>
        public AssemblyTypeInfo SelectedTypeInfo { get => (AssemblyTypeInfo)GetValue(SelectedTypeInfoProperty); set => SetValue(SelectedTypeInfoProperty, value); }

        #endregion

        #region AssemblyText : string - Содержание имени сборки

        /// <summary>Содержание имени сборки</summary>
        public static readonly DependencyProperty AssemblyTextProperty =
            DependencyProperty.Register(
                nameof(AssemblyText),
                typeof(string),
                typeof(MainWindow),
                new PropertyMetadata(default(string)));

        /// <summary>Содержание имени сборки</summary>
        public string AssemblyText { get => (string)GetValue(AssemblyTextProperty); set => SetValue(AssemblyTextProperty, value); }

        #endregion
        #region CountSelected : int - Выбрано

        /// <summary>Выбрано</summary>
        public static readonly DependencyProperty CountSelectedProperty =
            DependencyProperty.Register(
                nameof(CountSelected),
                typeof(int),
                typeof(MainWindow),
                new PropertyMetadata(default(int)));

        /// <summary>Выбрано</summary>
        public int CountSelected { get => (int)GetValue(CountSelectedProperty); set => SetValue(CountSelectedProperty, value); }

        #endregion
        public ObservableCollection<AssemblyTypeInfo> TypesForReport { get; set; } = new();

        #endregion

        #region Методы

        private void LoadAssemblyClick(object Sender, RoutedEventArgs E)
        {
            var dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                var file = dlg.FileName;
                try
                {
                    var assembly = Assembly.LoadFrom(file);
                    Assemblies = new List<AssemblyInfo>() { new AssemblyInfo(assembly, file) };
                    TypesForReport = new();
                }
                catch (Exception e)
                {
                    App.Notifier.Show("Error", $"Ошибка загрузки сборки:{e.Message}", NotificationType.Error, trim: NotificationTextTrimType.Attach);
                }

            }
        }

        void LoadAssembly(Assembly assembly, string AssemblyPath)
        {
            Assemblies = AssemblyDataTable.GetAssemblyInfo(assembly, AssemblyPath).ToArray();
            TypesForReport = new();
        }
        private void LoadReport(object Sender, RoutedEventArgs E)
        {
            try
            {
                if (CountSelected == 0)
                    return;
                var dialog = new SaveFileDialog
                {
                    FileName = "New report",
                    Filter = @"Excel Files (*.xlsx)|*.xlsx", //|Txt Files (*.txt)|*.txt|All files (*.*)|*.*
                };
                if (dialog.ShowDialog() != true) return;

                var file_path = dialog.FileName;
                using var progress = App.Notifier.ShowProgressBar("Загрузка отчёта", true, true, TrimText: true);
                var total = CountSelected;
                var current = 0;
                using var writer = new EasyWriter(file_path, Helper.Styles);

                var is_name_was_changed = false;
                foreach (var type in TypesForReport)
                {
                    var percent = current * 100 / total;
                    progress.Report((percent, type.Type.Name, null, null));

                    string sheet_name_1;
                    if (type.Type.Name.Length > 30)
                    {
                        is_name_was_changed = true;
                        sheet_name_1 = type.Type.Name.Substring(0, 31);
                    }
                    else
                        sheet_name_1 = type.Type.Name;


                    writer.AddNewSheet(sheet_name_1);

                    ////writer.SetFilter(1, 5, 3, 5); //SetFilter(string ListName, uint FirstColumn, uint LastColumn, uint FirstRow, uint LastRow)
                    //writer.SetGrouping(false, false); // SetGrouping(bool SummaryBelow = false, bool SummaryRight = false)
                    ////writer.MergeCells(6, 3, 10, 5); //MergeCells(int StartCell, int StartRow, int EndCell, int EndRow)
                    var width_setting = new List<WidthOpenXmlEx>
                    {
                        new(1, 2, 30),
                        new(3, 4, 50),
                    };
                    writer.SetWidth(width_setting); //SetWidth(IEnumerable<WidthOpenXmlEx> settings)

                    writer.AddRow(1, 0, true, true);
                    writer.MergeCells(1, 1, 4, 1);
                    writer.AddCell($"{type.Type.Name} {type.Summary}", 1, 1, 2);
                    writer.PrintEmptyCells(2,4,1,2);
                    //writer.AddCell(type.Type.Name, 1, 1, 2);
                    //writer.AddRow(2, 0, true, true);
                    //writer.AddCell(type.Summary, 1, 2, 2);
                    //writer.MergeCells(1, 2, 4, 1);
                    writer.AddRow(3, 0, true, true);
                    writer.AddCell("Property Name", 1, 3, 3);
                    writer.AddCell("Type", 2, 3, 3);
                    writer.AddCell("Description", 3, 3, 3);
                    writer.AddCell("Summary", 4, 3, 3);
                    var row_n = 4U;

                    var current_type = type.Members.FirstOrDefault()?.DeclaringType?.BaseType;
                    var need_skip_1 = current_type is not null && current_type == typeof(Enum);

                    foreach (var property in need_skip_1 ? type.MembersInfo.Skip(1) : type.MembersInfo)
                    {
                        writer.AddRow(row_n, 0, true, true);
                        writer.AddCell(property.Name, 1, row_n, 4);
                        writer.AddCell(property.Type.Name, 2, row_n, 4);
                        writer.AddCell(property.Description, 3, row_n, 4);
                        writer.AddCell(property.Summary, 4, row_n, 4);
                        row_n++;
                    }

                    current++;
                }

                if (is_name_was_changed)
                    App.Notifier.Show("Внимание", "Часть имён страниц были сокращены, т.к. имели имя более 30 символов", NotificationType.Warning);

                ShowFilePopUpMessage(file_path);
            }
            catch (OperationCanceledException)
            {
                ShowCancellation();
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        private void LoadReport2(object Sender, RoutedEventArgs E)
        {
            try
            {
                if (CountSelected == 0)
                    return;
                var dialog = new SaveFileDialog
                {
                    FileName = "New report",
                    Filter = @"Excel Files (*.xlsx)|*.xlsx", //|Txt Files (*.txt)|*.txt|All files (*.*)|*.*
                };
                if (dialog.ShowDialog() != true) return;

                var file_path = dialog.FileName;
                using var progress = App.Notifier.ShowProgressBar("Загрузка отчёта", true, true, TrimText: true);
                var total = CountSelected;
                var current = 0;
                using var writer = new EasyWriter(file_path, Helper.Styles);
                string sheet_name_1 = "report";
                writer.AddNewSheet(sheet_name_1);
                var row_n = 2U;
                ////writer.SetFilter(1, 5, 3, 5); //SetFilter(string ListName, uint FirstColumn, uint LastColumn, uint FirstRow, uint LastRow)
                writer.SetGrouping(false, false); // SetGrouping(bool SummaryBelow = false, bool SummaryRight = false)
                ////writer.MergeCells(6, 3, 10, 5); //MergeCells(int StartCell, int StartRow, int EndCell, int EndRow)
                var width_setting = new List<WidthOpenXmlEx>
                {
                    new(1, 2, 30),
                    new(3, 4, 50),
                };
                writer.SetWidth(width_setting); //SetWidth(IEnumerable<WidthOpenXmlEx> settings)

                foreach (var type in TypesForReport)
                {
                    var percent = current * 100 / total;
                    progress.Report((percent, type.Type.Name, null, null));


                    writer.AddRow(row_n, 0, true, true);
                    writer.AddCell($"{type.Type.Name} {type.Summary}", 1, row_n, 2);
                    writer.PrintEmptyCells(2,4, row_n, 2);
                    writer.MergeCells(1, row_n, 4, row_n);
                    row_n++;
                    //writer.AddRow(row_n, 0, true, true);
                    //writer.MergeCells(1, row_n, 4, row_n);
                    //writer.AddCell(type.Summary, 1, row_n, 2);
                    //row_n++;

                    writer.AddRow(row_n, 1, true, true);
                    writer.AddCell("Property Name", 1, row_n, 3);
                    writer.AddCell("Type", 2, row_n, 3);
                    writer.AddCell("Description", 3, row_n, 3);
                    writer.AddCell("Summary", 4, row_n, 3);
                    row_n++;

                    var current_type = type.Members.FirstOrDefault()?.DeclaringType?.BaseType;
                    var need_skip_1 = current_type is not null && current_type == typeof(Enum);

                    foreach (var property in need_skip_1 ? type.MembersInfo.Skip(1) : type.MembersInfo)
                    {
                        writer.AddRow(row_n, 1, true, true);
                        writer.AddCell(property.Name, 1, row_n, 4);
                        writer.AddCell(property.Type.Name, 2, row_n, 4);
                        writer.AddCell(property.Description, 3, row_n, 4);
                        writer.AddCell(property.Summary, 4, row_n, 4);
                        row_n++;
                    }

                    current++;
                }

                ShowFilePopUpMessage(file_path);
            }
            catch (OperationCanceledException)
            {
                ShowCancellation();
            }
            catch (Exception e)
            {
                ShowError(e);
            }

        }

        private void ToggleButton_OnChecked(object Sender, RoutedEventArgs E)
        {
            if (Sender is not CheckBox { DataContext: AssemblyTypeInfo type } check)
                return;
            switch (check.IsChecked)
            {
                case false:
                    var success = TypesForReport.Remove(type);
                    if (success)
                        CountSelected -= 1;
                    break;
                case true:
                    TypesForReport.Add(type);
                    CountSelected += 1;
                    break;
            }
        }

        #region Notifications

        /// <summary> Показывает всплывающее окно вывода информации об ошибке </summary>
        /// <param name="e">ошибка</param>
        public static void ShowError([NotNull] Exception e) => App.Notifier.Show("Error", $"{e.Message}\n\r{e}", type: NotificationType.Error,
            expirationTime: TimeSpan.MaxValue, RowsCountWhenTrim: 5, trim: NotificationTextTrimType.Attach);

        /// <summary> Сообщение об отмене операции </summary>
        public static void ShowCancellation() => App.Notifier.Show(null, "Операция отменена", NotificationType.Notification);

        /// <summary>
        /// Показывает всплывающее окно для открытия файла или директории
        /// </summary>
        /// <param name="FilePath">путь к файлу (директории)</param>
        /// <param name="ShowFile">показывать кнопку пути к файлу</param>
        /// <param name="ShowDirectory">показывать кнопку пути к директории</param>
        /// <param name="Area">зона для показа сообщения</param>
        public static void ShowFilePopUpMessage(string FilePath, bool ShowFile = true, bool ShowDirectory = true, string Area = "") =>
            ShowButtonWindow("Открыть файл?", "Отчёт",
                ShowFile ? (_, _) =>
                {
                    try
                    {
                        new Process { StartInfo = new ProcessStartInfo(FilePath) { UseShellExecute = true } }.Start();

                        //Process.Start(FilePath);
                    }
                    catch (Exception exc)
                    {
                        ShowError(exc);
                    }
                }
        : null
              , ShowFile ? "Открыть файл" : null,
                ShowDirectory ? (_, _) =>
                {
                    try
                    {
                        new Process
                        {
                            StartInfo = new ProcessStartInfo(Path.GetDirectoryName(FilePath) ??
                                                             throw new ArgumentNullException(nameof(FilePath), "Имя файла не может быть null"))
                            { UseShellExecute = true }
                        }.Start();

                        //Process.Start(Path.GetDirectoryName(FilePath) ?? throw new ArgumentNullException(nameof(FilePath), "Имя файла не может быть null"));
                    }
                    catch (Exception exc)
                    {
                        ShowError(exc);
                    }

                }
        : null, ShowDirectory ? "Директория" : null, AreaName: Area, TimeSpan: TimeSpan.FromSeconds(10));
        /// <summary>
        /// Показывает всплывающее окно для открытия файла или директории
        /// </summary>
        /// <param name="FilePath">путь к файлу </param>
        /// <param name="DirectoryPath">путь к директории</param>
        /// <param name="Area">зона для показа сообщения</param>
        public static void ShowFilePopUpMessage(string FilePath, string DirectoryPath, string Area = "") =>
            ShowButtonWindow("Открыть файл?", "Отчёт",
                !string.IsNullOrWhiteSpace(FilePath) ? (_, _) =>
                {
                    try
                    {
                        new Process { StartInfo = new ProcessStartInfo(FilePath) { UseShellExecute = true } }.Start();

                        //Process.Start(FilePath);
                    }
                    catch (Exception exc)
                    {
                        ShowError(exc);
                    }
                }
        : null, !string.IsNullOrWhiteSpace(FilePath) ? "Открыть файл" : null,
                !string.IsNullOrWhiteSpace(DirectoryPath) ? (_, _) =>
                {
                    try
                    {
                        new Process
                        {
                            StartInfo = new ProcessStartInfo(Path.GetDirectoryName(FilePath) ??
                                                             throw new ArgumentNullException(nameof(FilePath), "Имя файла не может быть null"))
                            { UseShellExecute = true }
                        }.Start();

                        //Process.Start(Path.GetDirectoryName(FilePath) ?? throw new ArgumentNullException(nameof(FilePath), "Имя файла не может быть null"));
                    }
                    catch (Exception exc)
                    {
                        ShowError(exc);
                    }

                }
        : null, !string.IsNullOrWhiteSpace(DirectoryPath) ? "Директория" : null, AreaName: Area);
        /// <summary>
        /// Всплывающее окно с кнопками
        /// </summary>
        /// <param name="Message">Текст сообщения</param>
        /// <param name="Title">Заголовок</param>
        /// <param name="LeftButton">Действие при нажатии на левую кнопку (если не задано кнопка не отображается)</param>
        /// <param name="LeftButtonContent">Содержимое кнопки (например текст) - если не задано - "Cancel"</param>
        /// <param name="RightButton">Действие при нажатии на правую кнопку (если не задано кнопка не отображается)</param>
        /// <param name="RightButtonContent">Содержимое кнопки (например текст) - если не задано - "Ok"</param>
        /// <param name="TimeSpan">время которое будет показано окно, если не задано - 5 секунд</param>
        /// <param name="AreaName">зона окна при необходимости</param>
        /// <example>
        /// <code>
        /// Notifier.ShowInfoWindow("The picture is successfully added","Success",
        /// RightButton:(Sender, Args) =&gt; _ = Process.Start(report_file.FullName), RightButtonContent:"Open file",
        /// TimeSpan:TimeSpan.FromSeconds(10));
        /// </code>
        /// </example>
        public static void ShowButtonWindow(string Message, [CanBeNull] string Title = null,
            [CanBeNull] RoutedEventHandler LeftButton = null, string LeftButtonContent = null,
            [CanBeNull] RoutedEventHandler RightButton = null, string RightButtonContent = null,
            TimeSpan? TimeSpan = null, string AreaName = "") =>
            App.Notifier.Show(Title, Message, NotificationType.Notification, AreaName, TimeSpan, null, null,
                LeftButton is null
                    ? null
                    : () => LeftButton.Invoke(null, null), LeftButtonContent,
                RightButton is null
                    ? null
                    : () => RightButton.Invoke(null, null), RightButtonContent);


        #endregion

        #endregion


    }
}
