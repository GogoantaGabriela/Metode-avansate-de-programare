using DrawingTool.Interfaces;
using DrawingTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingTool.Shapes
{
    public class Circle : IShape
    {
        private double _cx, _cy, _radius;

        public Circle(double cx, double cy, double r)
            => (_cx, _cy, _radius) = (cx, cy, r);

        public void Draw(ICanvas canvas) => canvas.DrawCircle(_cx, _cy, _radius);

        public void Move(double dx, double dy) { _cx += dx; _cy += dy; }

        public void Scale(double factor) => _radius *= factor;

        public BoundingBox GetBoundingBox()
            => new BoundingBox(_cx - _radius, _cy - _radius, _cx + _radius, _cy + _radius);
    }
}
