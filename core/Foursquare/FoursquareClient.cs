using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Q.Foursquare
{
	public static class FoursquareClient
	{
		const string BASE_URL = "https://api.foursquare.com/v2/";
		const string CLIENT_ID = "IH4AJSRPUQWTRTVRRGKLGQWZVAAT3ZMWRUATXPEDU3EH0ZPG";
		const string CLIENT_SECRET = "OFWWDG4LDMN14CAXYTHHVICVNZQLF3GSX4MEGLAAX5N32EHX";

		readonly static RestClient client = new RestClient (BASE_URL);

		public static Task<List<Venue>> SuggestCompletion (string searchText)
		{
			var request = makeRequest ("venues/suggestcompletion");
			request.AddQueryParameter ("near", "New York, NY");
			request.AddQueryParameter ("query", searchText);
			request.AddQueryParameter ("limit", "3");

			return client.ExecuteGetTaskAsync<FoursquareResponse<List<Venue>>> (request).ContinueWith (t => {
				if (t.Result != null) {
					return t.Result.Data.Response ["minivenues"];
				} else {
					return null;
				}
			});
		}

		static RestRequest makeRequest(string endpoint)
		{
			var request = new RestRequest (endpoint, Method.GET);
			request.AddQueryParameter ("client_id", CLIENT_ID);
			request.AddQueryParameter ("client_secret", CLIENT_SECRET);
			request.AddQueryParameter ("v", "20150301");
			request.AddQueryParameter ("m", "foursquare");

			return request;
		}
	}
}

