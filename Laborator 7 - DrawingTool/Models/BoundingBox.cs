using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrawingTool.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
namespace DrawingTool.Models
{
    public class BoundingBox
    {
        private readonly double _xmin;
        private readonly double _ymin;
        private readonly double _xmax;
        private readonly double _ymax;

        public double Xmin { get { return _xmin; } }
        public double Ymin { get { return _ymin; } }
        public double Xmax { get { return _xmax; } }
        public double Ymax { get { return _ymax; } }

        public BoundingBox(double xmin, double ymin, double xmax, double ymax)
        {
            _xmin = xmin;
            _ymin = ymin;
            _xmax = xmax;
            _ymax = ymax;
        }
    }

}