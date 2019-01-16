using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using CsvHelper;
using GeoJSON.Net;



namespace AirportExplorer.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public string MapboxAccessToken { get; }

        public IndexModel(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;

            MapboxAccessToken = configuration["Mapbox:AccessToken"];
        }

        public IActionResult OnGetAirports()
        {
            var configuration = new Configuration
            {
                BadDataFound = ControllerContext => { }
            };

            using (var sr = new StreamReader(Path.Combine(_hostingEnvironment.WebRootPath, "airports.dat")))
            using (var reader = new CsvReader(sr, configuration))
            {
                FeatureCollection featureCollection = new FeatureCollection();

                while (reader.Read())
                {
                    string name = reader.GetField<string>(1);
                    string iataCode = reader.GetField<string>(4);
                    double latitude = reader.GetField<double>(6);
                    double longitude = reader.GetField<double>(7);

                    featureCollection.Features.Add(new Feature(
                        new Point(new Position(latitude, longitude)),
                        new Dictionary<string, object>
                        {
                            {"name", name},
                            {"iataCode", iataCode}
                        }));
                }

                return new JsonResult(featureCollection);
            }
        }

        public void OnGet()
        {

        }
    }
}
