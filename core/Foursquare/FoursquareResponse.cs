using System.Collections.Generic;

namespace Q.Foursquare
{
	public class FoursquareResponse<T>
	{
		public Dictionary<string, object> Meta {get; set;}
		public Dictionary<string, T> Response {get; set;}
	}
}

