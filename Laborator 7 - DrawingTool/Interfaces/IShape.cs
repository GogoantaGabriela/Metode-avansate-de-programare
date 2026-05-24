using DrawingTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingTool.Interfaces
{
    public interface IShape
    {
        void Draw(ICanvas canvas);
        void Move(double dx, double dy);
        void Scale(double factor);
        BoundingBox GetBoundingBox();
    }
}
