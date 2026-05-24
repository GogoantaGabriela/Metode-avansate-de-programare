using DrawingTool.Interfaces;
using DrawingTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingTool.Shapes
{
    public class Line : IShape
    {
        private double _x1, _y1, _x2, _y2;

        public Line(double x1, double y1, double x2, double y2)
        {
            _x1 = x1;
            _y1 = y1;
            _x2 = x2;
            _y2 = y2;
        }

        public void Draw(ICanvas canvas)
        {
            canvas.DrawLine(_x1, _y1, _x2, _y2);
        }

        public void Move(double dx, double dy)
        {
            _x1 += dx;
            _y1 += dy;
            _x2 += dx;
            _y2 += dy;
        }

        public void Scale(double factor)
        {
            _x2 = _x1 + (_x2 - _x1) * factor;
            _y2 = _y1 + (_y2 - _y1) * factor;
        }

        public BoundingBox GetBoundingBox()
        {
            return new BoundingBox(
                Math.Min(_x1, _x2),
                Math.Min(_y1, _y2),
                Math.Max(_x1, _x2),
                Math.Max(_y1, _y2)
            );
        }
    }
}
