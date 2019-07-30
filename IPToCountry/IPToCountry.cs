using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPToCountry
{
    public class IPToCountry
    {
        public string GetCountryNameByIP(string ipAddress)
        {
            string countryName;
            GeoService.GeoIPService service = new GeoService.GeoIPService();
            GeoService.GeoIP output = service.GetGeoIP(ipAddress);
            
            countryName = output.CountryName;

            return countryName;
        }
    }
}
