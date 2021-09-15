using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Forms2048
{
    public class Case
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Value { get; set; }

        public Case(int x, int y, int value)
        {
            X = x;
            Y = y;
            Value = value;
        }

        public override string ToString()
        {
            return $"[X:{X} | Y:{Y} | V:{Value}]";
        }
    }
}
