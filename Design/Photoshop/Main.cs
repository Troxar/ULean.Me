using System;
using System.Drawing;
using System.Windows.Forms;

namespace MyPhotoshop
{
	class MainClass
	{
        [STAThread]
		public static void Main (string[] args)
		{
			var window = new MainWindow();
            window.AddFilter(new PixelFilter<LighteningParameters>(
				"Осветление/затемнение", 
				(pixel, parameters) => pixel * parameters.Coefficient));
			window.AddFilter(new PixelFilter<GrayscaleParameters>(
                "Оттенки серого",
				(pixel, parameters) => 
				{
                    var shade = (pixel.Red
                        + pixel.Green
                        + pixel.Blue) / 3;
                    return new Pixel(shade, shade, shade);
                }
				));

            window.AddFilter(new TransformFilter<FlipParameters>(
                "Отразить по горизонтали",
                (size, parameters) => size,
                (point, size, parameters) => new Point(size.Width - point.X - 1, point.Y)));

            Func<Size, RotationParameters, Size> sizeRotator = (size, parameters) =>
            {
                var angle = Math.PI * parameters.Angle / 180;
                return new Size(
                    (int)(size.Width * Math.Abs(Math.Cos(angle)) + size.Height * Math.Abs(Math.Sin(angle))),
                    (int)(size.Height * Math.Abs(Math.Cos(angle)) + size.Width * Math.Abs(Math.Sin(angle))));
            };

            Func<Point, Size, RotationParameters, Point?> pointRotator = (point, size, parameters) =>
            {
                var newSize = sizeRotator(size, parameters);
                var angle = Math.PI * parameters.Angle / 180;
                point = new Point(point.X - newSize.Width / 2, point.Y - newSize.Height / 2);
                var x = size.Width / 2 + (int)(point.X * Math.Cos(angle) + point.Y * Math.Sin(angle));
                var y = size.Height / 2 + (int)(-point.X * Math.Sin(angle) + point.Y * Math.Cos(angle));
                if (x < 0 || x >= size.Width || y < 0 || y >= size.Height)
                    return null;
                return new Point(x, y);
            };

            window.AddFilter(new TransformFilter<RotationParameters>(
                "Свободное вращение",
                sizeRotator,
                pointRotator));

            Application.Run(window);
        }
	}
}