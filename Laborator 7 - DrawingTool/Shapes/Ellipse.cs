using DrawingTool.Interfaces;
using DrawingTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingTool.Shapes
{
    public class Ellipse : IShape
    {
        private double _cx, _cy, _rx, _ry;

        public Ellipse(double cx, double cy, double rx, double ry)
        {
            _cx = cx;
            _cy = cy;
            _rx = rx;
            _ry = ry;
        }

        public void Draw(ICanvas canvas)
        {
            canvas.DrawEllipse(_cx, _cy, _rx, _ry);
        }

        public void Move(double dx, double dy)
        {
            _cx += dx;
            _cy += dy;
        }

        public void Scale(double factor)
        {
            _rx *= factor;
            _ry *= factor;
        }

        public BoundingBox GetBoundingBox()
        {
            return new BoundingBox(
                _cx - _rx,
                _cy - _ry,
                _cx + _rx,
                _cy + _ry
            );
        }
    }
}
