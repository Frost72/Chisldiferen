using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chisldiferen
{
    public class MyPointData
    {
        public double X { get; set; }
        public double Y { get; set; }
        public string Derivative { get; set; } = "—";

        public override string ToString()
        {
            return $"x = {X:F2}, y = {Y:F2}, f'(x) = {Derivative}";
        }
    }
}
