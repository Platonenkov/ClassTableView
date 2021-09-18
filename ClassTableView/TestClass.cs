using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassTableView
{
    public class TestClass
    {
        public string Name { get; }
        private int Age;
        [Description("Days")]
        public int Day { get; }

        public virtual TestClass Parent { get; set; }
        public virtual TestClass Child { get; set; }
        public TestClass()
        {
            
        }
    }
}
