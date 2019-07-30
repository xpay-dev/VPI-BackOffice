using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGAdmin.Models
{
    public class DataTableModel
    {
        public int sEcho { get; set; }
        public int iDisplayStart { get; set; }
        public int iDisplayLength { get; set; }
        public int iSortCol_0 { get; set; }
        public string sSortDir_0 { get; set; }
        public string sSearch { get; set; }

        public int pageIndex
        {
            get
            {
                if (this.iDisplayLength == 0)
                    this.iDisplayLength = 5;

                return (this.iDisplayStart - (this.iDisplayStart % this.iDisplayLength)) / this.iDisplayLength;
            }
        }

        public DataTableModel(HttpRequestBase request)
        {
            sEcho = Convert.ToInt32(request["draw"]);
            iDisplayLength = Convert.ToInt32(request["length"]);
            iDisplayStart = Convert.ToInt32(request["start"]);
            iSortCol_0 = Convert.ToInt32(request["order[0][column]"]);
            sSortDir_0 = request["order[0][dir]"];
            sSearch = request["search[value]"];
        }
    }
}