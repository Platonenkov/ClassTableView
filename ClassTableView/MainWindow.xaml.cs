using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Microsoft.Win32;
using Notification.Wpf;

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
            InitAssembly(null,null);
        }

        #endregion

        #region Свойства

        #region Assemblies : IEnumerable<string> - Сборки

        /// <summary>Сборки</summary>
        public static readonly DependencyProperty AssembliesProperty =
            DependencyProperty.Register(
                nameof(Assemblies),
                typeof(IEnumerable<string>),
                typeof(MainWindow),
                new PropertyMetadata(default(IEnumerable<string>)));

        /// <summary>Сборки</summary>
        public IEnumerable<string> Assemblies { get => (IEnumerable<string>)GetValue(AssembliesProperty); set => SetValue(AssembliesProperty, value); }

        #endregion

        #region SelectedAssembly : string - Выбранная сборка

        /// <summary>Выбранная сборка</summary>
        public static readonly DependencyProperty SelectedAssemblyProperty =
            DependencyProperty.Register(
                nameof(SelectedAssembly),
                typeof(string),
                typeof(MainWindow),
                new PropertyMetadata(default(string)));

        /// <summary>Выбранная сборка</summary>
        public string SelectedAssembly { get => (string)GetValue(SelectedAssemblyProperty); set => SetValue(SelectedAssemblyProperty, value); }

        #endregion

        #region Types : IEnumerable<Type> - Типы

        /// <summary>Типы</summary>
        public static readonly DependencyProperty TypesProperty =
            DependencyProperty.Register(
                nameof(Types),
                typeof(IEnumerable<Type>),
                typeof(MainWindow),
                new PropertyMetadata(default(IEnumerable<Type>)));

        /// <summary>Типы</summary>
        public IEnumerable<Type> Types { get => (IEnumerable<Type>)GetValue(TypesProperty); set => SetValue(TypesProperty, value); }

        #endregion

        #region SelectedType : Type - Выбранный тип

        /// <summary>Выбранный тип</summary>
        public static readonly DependencyProperty SelectedTypeProperty =
            DependencyProperty.Register(
                nameof(SelectedType),
                typeof(Type),
                typeof(MainWindow),
                new PropertyMetadata(default(Type)));

        /// <summary>Выбранный тип</summary>
        public Type SelectedType { get => (Type)GetValue(SelectedTypeProperty); set => SetValue(SelectedTypeProperty, value); }

        #endregion

        #region Members : IEnumerable<MemberInfo> - Список публичных полей

        /// <summary>Список публичных полей</summary>
        public static readonly DependencyProperty MembersProperty =
            DependencyProperty.Register(
                nameof(Members),
                typeof(IEnumerable<MemberInfo>),
                typeof(MainWindow),
                new PropertyMetadata(default(IEnumerable<MemberInfo>)));

        /// <summary>Список публичных полей</summary>
        public IEnumerable<MemberInfo> Members { get => (IEnumerable<MemberInfo>)GetValue(MembersProperty); set => SetValue(MembersProperty, value); }

        #endregion

        #region SelectedMember : MemberInfo - Выбранное поле

        /// <summary>Выбранное поле</summary>
        public static readonly DependencyProperty SelectedMemberProperty =
            DependencyProperty.Register(
                nameof(SelectedMember),
                typeof(MemberInfo),
                typeof(MainWindow),
                new PropertyMetadata(default(MemberInfo)));

        /// <summary>Выбранное поле</summary>
        public MemberInfo SelectedMember { get => (MemberInfo)GetValue(SelectedMemberProperty); set => SetValue(SelectedMemberProperty, value); }

        #endregion

        #endregion

        #region Поля



        #endregion

        #region Команды



        #endregion

        #region Методы

        void InitAssembly(object Sender, RoutedEventArgs RoutedEventArgs)
        {
            Assembly = Assembly.GetEntryAssembly();
            var assemblies = new List<string>();
            var entry = Assembly.GetEntryAssembly()?.GetReferencedAssemblies().Select(a=>a.Name).ToList();
            if(entry?.Count>0)
                assemblies.AddRange(entry);
            assemblies.Add(Assembly.GetEntryAssembly().GetName().Name);
            Assemblies = assemblies;
        }


        #endregion

        private void ReadAssembly(object Sender, RoutedEventArgs E)
        {
            try
            {
                if (SelectedAssembly is null)
                {
                    App.Notifier.Show("Ошибка","Выберите сборку",NotificationType.Error);
                    return;
                }
                Assembly assembly = Assembly.Load(SelectedAssembly);
                Types = assembly.GetTypes().Where(t=>t.IsPublic && t.IsClass || t.IsEnum);
            }
            catch (Exception e)
            {
                App.Notifier.Show("Error", $"Ошибка загрузки сборки: {e.Message}",type:NotificationType.None ,trim:NotificationTextTrimType.Attach);
            }
        }

        private void GetMembers(object Sender, RoutedEventArgs E)
        {
            if (SelectedType is null)
            {
                App.Notifier.Show("Ошибка", "Выберите тип", NotificationType.Error);
                return;
            }

            AssemblyText = SelectedType.Name;

            if (SelectedType.BaseType == typeof(Enum))
            {
                Members = SelectedType.GetMembers(
                        //BindingFlags.Instance
                        //| BindingFlags.Static
                        //| BindingFlags.Public
                        //| BindingFlags.NonPublic
                        //| BindingFlags.DeclaredOnly
                        ).Where(v=>v.MemberType== MemberTypes.Field)
                    //.Where(v=>v.Name.ToUpper().StartsWith("GET"))
                    ;

            }
            else Members = SelectedType.GetMembers(
                //BindingFlags.Instance
                //| BindingFlags.Static
                //| BindingFlags.Public
                //| BindingFlags.NonPublic
                //| BindingFlags.DeclaredOnly
                )
               .Where(v=>v.MemberType == MemberTypes.Property)
               //.Where(v=>v.Name.ToUpper().StartsWith("GET"))
                ;

            foreach (var m in Members)
            {
                var attribute = m.GetCustomAttribute<DescriptionAttribute>();
                if (attribute is not null)
                {

                }
            }
        }

        private Assembly Assembly;
        private void GetFields(object Sender, SelectionChangedEventArgs SelectionChangedEventArgs)
        {
            if (SelectedType is null)
            {
                App.Notifier.Show("Ошибка", "Выберите тип", NotificationType.Error);
                return;
            }

            Fields = SelectedType.GetFields();
        }

        #region Fields : IEnumerable<FieldInfo>  - Поля

        /// <summary>Поля</summary>
        public static readonly DependencyProperty FieldsProperty =
            DependencyProperty.Register(
                nameof(Fields),
                typeof(IEnumerable<FieldInfo> ),
                typeof(MainWindow),
                new PropertyMetadata(default(IEnumerable<FieldInfo> )));

        /// <summary>Поля</summary>
        public IEnumerable<FieldInfo> Fields { get => (IEnumerable<FieldInfo> )GetValue(FieldsProperty); set => SetValue(FieldsProperty, value); }

        #endregion

        #region SelectedField : FieldInfo  - Выбранное поле

        /// <summary>Выбранное поле</summary>
        public static readonly DependencyProperty SelectedFieldProperty =
            DependencyProperty.Register(
                nameof(SelectedField),
                typeof(FieldInfo ),
                typeof(MainWindow),
                new PropertyMetadata(default(FieldInfo )));

        /// <summary>Выбранное поле</summary>
        public FieldInfo SelectedField { get => (FieldInfo )GetValue(SelectedFieldProperty); set => SetValue(SelectedFieldProperty, value); }

        #endregion
        private void GetProperties(object Sender, SelectionChangedEventArgs SelectionChangedEventArgs)
        {
            if (SelectedType is null)
            {
                App.Notifier.Show("Ошибка", "Выберите тип", NotificationType.Error);
                return;
            }

            Properties = SelectedType.GetProperties();
        }

        #region Properties : IEnumerable<PropertyInfo> - Свойства

        /// <summary>Свойства</summary>
        public static readonly DependencyProperty PropertiesProperty =
            DependencyProperty.Register(
                nameof(Properties),
                typeof(IEnumerable<PropertyInfo>),
                typeof(MainWindow),
                new PropertyMetadata(default(IEnumerable<PropertyInfo>)));

        /// <summary>Свойства</summary>
        public IEnumerable<PropertyInfo> Properties { get => (IEnumerable<PropertyInfo>)GetValue(PropertiesProperty); set => SetValue(PropertiesProperty, value); }

        #endregion

        #region SelectedProperty : PropertyInfo - Выбранное свойство

        /// <summary>Выбранное свойство</summary>
        public static readonly DependencyProperty SelectedPropertyProperty =
            DependencyProperty.Register(
                nameof(SelectedProperty),
                typeof(PropertyInfo),
                typeof(MainWindow),
                new PropertyMetadata(default(PropertyInfo)));

        /// <summary>Выбранное свойство</summary>
        public PropertyInfo SelectedProperty { get => (PropertyInfo)GetValue(SelectedPropertyProperty); set => SetValue(SelectedPropertyProperty, value); }

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
        private void LoadAssemblyClick(object Sender, RoutedEventArgs E)
        {
            if (string.IsNullOrWhiteSpace(AssemblyText))
            {
                App.Notifier.Show("Error", "Введите имя сборки", NotificationType.Error);
                return;
            }
            var type = Type.GetType(AssemblyText);
            if (type is null)
            {
                App.Notifier.Show("Error", "Тип не опознан, выберите файл", NotificationType.Error);

                var dlg = new OpenFileDialog();
                if (dlg.ShowDialog() == true)
                {
                    var file = dlg.FileName;
                    try
                    {
                        var assembly = Assembly.LoadFrom(file);
                        LoadAssembly(assembly);
                    }
                    catch (Exception e)
                    {
                        App.Notifier.Show("Error", $"Ошибка загрузки сборки:{e.Message}", NotificationType.Error);
                    }

                }


                return;
            }

            LoadAssembly(type.Assembly);
        }

        void LoadAssembly(Assembly assembly)
        {
            Assembly = assembly;
            var assemblies = new List<string>();
            assemblies.Add(assembly.GetName().Name);
            var entry = assembly.GetReferencedAssemblies().Select(a => a.Name).ToList();
            if (entry?.Count > 0)
                assemblies.AddRange(entry);
            Assemblies = assemblies;

        }
    }
}
