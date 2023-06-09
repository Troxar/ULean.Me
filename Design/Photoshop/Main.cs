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
            window.AddFilter(new TransformFilter(
                "Отразить по горизонтали",
                (size) => size,
                (point, size) => new Point(size.Width - point.X - 1, point.Y)));
            window.AddFilter(new TransformFilter(
                "Поворот против часовой стрелки",
                (size) => new Size(size.Height, size.Width),
                (point, size) => new Point(size.Width - point.Y - 1, point.X)));
            Application.Run(window);
        }
	}
}