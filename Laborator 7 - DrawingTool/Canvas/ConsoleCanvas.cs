using DrawingTool.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingTool.Canvas
{
    public class ConsoleCanvas : ICanvas
    {
        public void DrawLine(double x1, double y1, double x2, double y2)
            => Console.WriteLine($"[Console] Line: ({x1},{y1}) to ({x2},{y2})");

        public void DrawCircle(double cx, double cy, double r)
            => Console.WriteLine($"[Console] Circle: Center({cx},{cy}), Radius({r})");

        public void DrawRect(double x, double y, double w, double h)
            => Console.WriteLine($"[Console] Rect: X={x}, Y={y}, W={w}, H={h}");

        public void DrawEllipse(double cx, double cy, double rx, double ry)
            => Console.WriteLine($"[Console] Ellipse: Center({cx},{cy}), Radii({rx},{ry})");
    }
}
