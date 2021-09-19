using System;
using System.ComponentModel;

namespace AssemblyGetDataTable
{
    /// <summary>
    /// Тестовый класс
    /// </summary>
    public class TestClass
    {
        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; }
        /// <summary> Возраст </summary>
        private int Age;
        /// <summary> Дни </summary>
        [Description("Days")]
        public int Day { get; }
        /// <summary>
        /// родитель
        /// </summary>
        public virtual TestClass Parent { get; set; }
        /// <summary>
        /// потомок
        /// </summary>
        public virtual TestClass Child { get; set; }
        public TestClass()
        {
            
        }
    }
    /// <summary>
    /// Тестовый интерфейс
    /// </summary>
    public interface TestInterface
    {
        /// <summary>
        /// число
        /// </summary>
        public int I { get; set; }
        /// <summary>
        /// строка
        /// </summary>
        public string R { get; set; }
        /// <summary>
        /// класс
        /// </summary>
        public TestClass Test { get; set; }
        /// <summary>
        /// Акшен
        /// </summary>
        public Action Asct { get; set; }
        /// <summary>
        /// метод
        /// </summary>
        public void Method();
    }
    /// <summary>
    /// Тестовый енам
    /// </summary>
    public enum TestEnum
    {
        /// <summary>
        /// строка
        /// </summary>
        row,
        /// <summary>
        /// число
        /// </summary>
        num,
        /// <summary>
        /// все
        /// </summary>
        any
    }
}
