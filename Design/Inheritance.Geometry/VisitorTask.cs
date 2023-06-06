using System;
using System.Collections.Generic;

namespace Inheritance.Geometry.Visitor
{
    public abstract class Body
    {
        public Vector3 Position { get; }

        protected Body(Vector3 position)
        {
            Position = position;
        }

        public abstract Body Accept(IVisitor visitor);
    }

    public class Ball : Body
    {
        public double Radius { get; }

        public Ball(Vector3 position, double radius) : base(position)
        {
            Radius = radius;
        }

        public override Body Accept(IVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class RectangularCuboid : Body
    {
        public double SizeX { get; }
        public double SizeY { get; }
        public double SizeZ { get; }

        public RectangularCuboid(Vector3 position, double sizeX, double sizeY, double sizeZ) : base(position)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            SizeZ = sizeZ;
        }

        public override Body Accept(IVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class Cylinder : Body
    {
        public double SizeZ { get; }

        public double Radius { get; }

        public Cylinder(Vector3 position, double sizeZ, double radius) : base(position)
        {
            SizeZ = sizeZ;
            Radius = radius;
        }

        public override Body Accept(IVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class CompoundBody : Body
    {
        public IReadOnlyList<Body> Parts { get; }

        public CompoundBody(IReadOnlyList<Body> parts) : base(parts[0].Position)
        {
            Parts = parts;
        }

        public override Body Accept(IVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class BoundingBoxVisitor : IVisitor
    {
        public Body Visit(Ball ball)
        {
            return new RectangularCuboid(
                ball.Position,
                ball.Radius * 2,
                ball.Radius * 2,
                ball.Radius * 2);
        }

        public Body Visit(RectangularCuboid cuboid)
        {
            return cuboid;
        }

        public Body Visit(Cylinder cylinder)
        {
            return new RectangularCuboid(
                cylinder.Position,
                cylinder.Radius * 2,
                cylinder.Radius * 2,
                cylinder.SizeZ);
        }

        public Body Visit(CompoundBody body)
        {
            double maxX, maxY, maxZ;
            double minX, minY, minZ;
            maxX = maxY = maxZ = double.MinValue;
            minX = minY = minZ = double.MaxValue;

            foreach (var part in body.Parts)
            {
                var box = part.Accept(this) as RectangularCuboid;
                maxX = Math.Max(maxX, box.Position.X + box.SizeX / 2);
                minX = Math.Min(minX, box.Position.X - box.SizeX / 2);
                maxY = Math.Max(maxY, box.Position.Y + box.SizeY / 2);
                minY = Math.Min(minY, box.Position.Y - box.SizeY / 2);
                maxZ = Math.Max(maxZ, box.Position.Z + box.SizeZ / 2);
                minZ = Math.Min(minZ, box.Position.Z - box.SizeZ / 2);
            }

            var position = new Vector3((maxX + minX) / 2, (maxY + minY) / 2, (maxZ + minZ) / 2);
            return new RectangularCuboid(
                position,
                maxX - minX,
                maxY - minY,
                maxZ - minZ);
        }
    }

    public class BoxifyVisitor : IVisitor
    {
        public Body Visit(Ball ball)
        {
            return ball.Accept(new BoundingBoxVisitor());
        }

        public Body Visit(RectangularCuboid cuboid)
        {
            return cuboid.Accept(new BoundingBoxVisitor());
        }

        public Body Visit(Cylinder cylinder)
        {
            return cylinder.Accept(new BoundingBoxVisitor());
        }

        public Body Visit(CompoundBody body)
        {
            var boxed = new List<Body>(body.Parts.Count);
            foreach (var part in body.Parts)
                boxed.Add(part.Accept(this));
            return new CompoundBody(boxed);
        }
    }

    public interface IVisitor
    {
        Body Visit(Ball ball);
        Body Visit(RectangularCuboid cuboid);
        Body Visit(Cylinder cylinder);
        Body Visit(CompoundBody body);
    }
}