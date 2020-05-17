using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Funda.Api
{
    //AgentModel is going to be sent to the view where we show the output
    public class AgentModel
    {
        public int RowId { get; set; }
        public string AgentName { get; set; }
        public int Total { get; set; }
    }
}