using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SDGWebService.Classes
{
    [DataContract]
    public class Countries
    {
        [DataMember]
        public string CountryCode { get; set; }

        [DataMember]
        public string CountryName { get; set; }
    }
}