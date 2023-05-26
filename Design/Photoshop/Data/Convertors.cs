using System;
using System.Drawing;

namespace MyPhotoshop
{
	public static class Convertors
	{
		public static Photo Bitmap2Photo(Bitmap bmp)
		{
			var photo=new Photo();
			photo.width=bmp.Width;
			photo.height=bmp.Height;
			photo.data = new Pixel[bmp.Width, bmp.Height];
			for (int x=0;x<bmp.Width;x++)
				for (int y=0;y<bmp.Height;y++)
					photo.data[x, y] = new Pixel(bmp.GetPixel(x, y));
			return photo;
		}
		
		static int ToChannel(double val)
		{
            if (val<0 || val>1)
                throw new Exception(string.Format("Wrong channel value {0} (the value must be between 0 and 1", val));
			return (int)(val * 255);
		}
		
		public static Bitmap Photo2Bitmap(Photo photo)
		{
			var bmp=new Bitmap(photo.width,photo.height);
			for (int x=0;x<bmp.Width;x++)
				for (int y=0;y<bmp.Height;y++)
					bmp.SetPixel(x, y, Color.FromArgb(
						ToChannel(photo.data[x, y].Red),
						ToChannel(photo.data[x, y].Green),
						ToChannel(photo.data[x, y].Blue)));
					       		
			return bmp;
		}
	}
}

