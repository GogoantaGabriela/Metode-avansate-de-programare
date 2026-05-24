using DrawingTool.Interfaces;
using DrawingTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingTool.Composite
{
    public class Picture : IShape
    {
        private readonly List<IShape> _children = new();

        public void Add(IShape shape) => _children.Add(shape);
        public void Remove(IShape shape) => _children.Remove(shape);

        public void Draw(ICanvas canvas)
        {
            foreach (var child in _children) child.Draw(canvas);
        }

        public void Move(double dx, double dy)
        {
            foreach (var child in _children) child.Move(dx, dy);
        }

        public void Scale(double factor)
        {
            foreach (var child in _children) child.Scale(factor);
        }

        public BoundingBox GetBoundingBox()
        {
            if (!_children.Any()) return new BoundingBox(0, 0, 0, 0);

            var boxes = _children.Select(c => c.GetBoundingBox());
            return new BoundingBox(
                boxes.Min(b => b.Xmin),
                boxes.Min(b => b.Ymin),
                boxes.Max(b => b.Xmax),
                boxes.Max(b => b.Ymax)
            );
        }
    }
}
