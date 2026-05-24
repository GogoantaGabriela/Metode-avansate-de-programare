using DrawingTool.Interfaces;
using DrawingTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingTool.Shapes
{
    public class Rectangle : IShape
    {
        private double _x, _y, _width, _height;

        public Rectangle(double x, double y, double w, double h)
            => (_x, _y, _width, _height) = (x, y, w, h);

        public void Draw(ICanvas canvas) => canvas.DrawRect(_x, _y, _width, _height);

        public void Move(double dx, double dy) { _x += dx; _y += dy; }

        public void Scale(double factor) { _width *= factor; _height *= factor; }

        public BoundingBox GetBoundingBox()
            => new BoundingBox(_x, _y, _x + _width, _y + _height);
    }
}
