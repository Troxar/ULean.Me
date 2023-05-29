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
		}

		public Pixel this[int width, int height]
		{
			get { return _data[width, height]; }
			set { _data[width, height] = value; }
		}
	}
}