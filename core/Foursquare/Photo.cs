namespace Q
{
	public class Photo
	{
		public string Id { get; set; }
		public string Prefix { get; set; }
		public string Suffix { get; set; }

		public string Url(int width) {
			return Prefix + "width" + width.ToString() + Suffix;
		}
	}
}

