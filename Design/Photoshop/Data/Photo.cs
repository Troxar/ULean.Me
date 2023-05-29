namespace MyPhotoshop
{
	public class Photo
	{
		private readonly int _width;
		public int Width
		{
			get { return _width; } 
		}

		private readonly int _height;
		public int Height
		{
			get { return _height; }
		}

		private readonly Pixel[,] _data;

		public Photo(int width, int height)
		{
			_width = width;
			_height = height;
			_data = new Pixel[width, height];

			for (int x = 0; x < width; x++)
				for (int y = 0; y < height; y++)
					_data[x, y] = new Pixel();
		}

		public Pixel this[int height, int width]
		{
			get { return _data[height, width]; }
		}
	}
}