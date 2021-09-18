using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Reflection;
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
using System.Windows.Threading;
using AssemblyGetDataTable;
using MathCore.ViewModels;
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
            LoadAssembly(Assembly.GetEntryAssembly());
        }

        #endregion

        #region Свойства

        #region Assemblies : IEnumerable<AssemblyHelper> - Сборки

        /// <summary>Сборки</summary>
        public static readonly DependencyProperty AssembliesProperty =
            DependencyProperty.Register(
                nameof(Assemblies),
                typeof(IEnumerable<AssemblyInfoModel>),
                typeof(MainWindow),
                new PropertyMetadata(default(IEnumerable<AssemblyInfoModel>)));

        /// <summary>Сборки</summary>
        public IEnumerable<AssemblyInfoModel> Assemblies { get => (IEnumerable<AssemblyInfoModel>)GetValue(AssembliesProperty); set => SetValue(AssembliesProperty, value); }

        #endregion

        #region SelectedTypeInfoModel : AssemblyTypeInfoModel - AssemblyTypeInfoModel

        /// <summary>AssemblyTypeInfoModel</summary>
        public static readonly DependencyProperty SelectedTypeInfoProperty =
            DependencyProperty.Register(
                nameof(SelectedTypeInfoModel),
                typeof(AssemblyTypeInfoModel),
                typeof(MainWindow),
                new PropertyMetadata(default(AssemblyTypeInfoModel)));

        /// <summary>AssemblyTypeInfoModel</summary>
        public AssemblyTypeInfoModel SelectedTypeInfoModel { get => (AssemblyTypeInfoModel)GetValue(SelectedTypeInfoProperty); set => SetValue(SelectedTypeInfoProperty, value); }

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
                    Assemblies = new List<AssemblyInfoModel>() { new AssemblyInfoModel(assembly) };
                }
                catch (Exception e)
                {
                    App.Notifier.Show("Error", $"Ошибка загрузки сборки:{e.Message}", NotificationType.Error,trim:NotificationTextTrimType.Attach);
                }

            }
        }

        void LoadAssembly(Assembly assembly)
        {
            Assemblies = AssemblyDataTable.GetAssemblyInfo(assembly).ToArray();
        }

        #endregion

        private void LoadReport(object Sender, RoutedEventArgs E)
        {
            //if(TreeViewData.SelectedItem is not AssemblyInfo assembly)
            //    return;
            //var dlg = new SaveFileDialog();
            //if(dlg.ShowDialog() != true)
            //    return;
            //using var writer = new EasyWriter($"{dlg.FileName}.xlsm", Helper.Styles);
            var assembly = TreeViewData.ItemsSource as IEnumerable<AssemblyInfoModel>;
            foreach (var assembly_info_model in assembly.SelectMany(s => s.TypesInfo).Where(s => s.IsSelected))
            {

            }

            foreach (var result in Assemblies.SelectMany(s => s.TypesInfo).Where(s => s.IsSelected))
            {

            }

            //foreach (var type in assembly.SelectedTypes?.Where(t=>t.IsSelected))
            //{

            //}

            //var sheet_name_1 = "Test_sheet_name";
            //writer.AddNewSheet(sheet_name_1);
            ////writer.SetFilter(1, 5, 3, 5); //SetFilter(string ListName, uint FirstColumn, uint LastColumn, uint FirstRow, uint LastRow)
            //writer.SetGrouping(false, false); // SetGrouping(bool SummaryBelow = false, bool SummaryRight = false)
            ////writer.MergeCells(6, 3, 10, 5); //MergeCells(int StartCell, int StartRow, int EndCell, int EndRow)
            //var width_setting = new List<WidthOpenXmlEx>
            //{
            //    new (1, 2, 7),
            //    new (3, 3, 11),
            //    new (4, 12, 9.5),
            //    new (13, 13, 17),
            //    new (14, 14, 40),
            //    new (15, 16, 15),
            //    new (18, 20, 15)
            //};
            //writer.SetWidth(width_setting); //SetWidth(IEnumerable<WidthOpenXmlEx> settings)
            //writer.AddRow(3, 0, true, true);
            ////AddRow(uint RowIndex, uint CollapsedLvl = 0, bool ClosePreviousIfOpen = false, bool AddSkipedRows = false)
            ////CloseRow(uint RowNumber)
            //writer.AddCell("Test", 1, 3, 0);
            ////AddCell(string text, uint CellNum, uint RowNum, uint StyleIndex = 0, CellValues Type = CellValues.String, bool CanReWrite = false)
        }
    }
}
