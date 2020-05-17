using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Funda.Api
{
    //AdvertisementModel class is going to wrap the json coming from API. I eliminated the properties we might not need

    public class AdvertisementModel
    {
        public int AccountStatus { get; set; }
        public bool EmailNotConfirmed { get; set; }
        public bool ValidationFailed { get; set; }
        public object ValidationReport { get; set; }
        public int Website { get; set; }
        public Metadata Metadata { get; set; }
        public Object[] Objects { get; set; }
        public Paging Paging { get; set; }
        public int TotaalAantalObjecten { get; set; }
    }

    public class Metadata
    {
        public string ObjectType { get; set; }
        public string Omschrijving { get; set; }
        public string Titel { get; set; }
    }

    public class Paging
    {
        public int AantalPaginas { get; set; }
        public int HuidigePagina { get; set; }
        public string VolgendeUrl { get; set; }
        public object VorigeUrl { get; set; }
    }

    public class Object
    {
        public string Id { get; set; }
        public int MakelaarId { get; set; }
        public string MakelaarNaam { get; set; }
    }


}