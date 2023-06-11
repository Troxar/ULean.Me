using System;
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
                new FlipTransformer()));
            window.AddFilter(new TransformFilter<RotationParameters>(
                "Свободное вращение",
                new RotationTransformer()));
            Application.Run(window);
        }
	}
}