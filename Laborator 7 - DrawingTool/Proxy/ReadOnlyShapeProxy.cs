using System;
using DrawingTool.Interfaces;
using DrawingTool.Models;

namespace DrawingTool.Proxy
{

    public class ReadOnlyShapeProxy : IShape
    {
        private readonly IShape _innerShape;

        public ReadOnlyShapeProxy(IShape shape)
        {
            _innerShape = shape ?? throw new ArgumentNullException(nameof(shape));
        }

        public void Draw(ICanvas canvas)
        {
            _innerShape.Draw(canvas);
        }

        public void Move(double dx, double dy)
        {
            throw new InvalidOperationException("Shape is locked: Move operation is not allowed.");
        }

        public void Scale(double factor)
        {
            throw new InvalidOperationException("Shape is locked: Scale operation is not allowed.");
        }

        public BoundingBox GetBoundingBox()
        {
            return _innerShape.GetBoundingBox();
        }
    }
}
