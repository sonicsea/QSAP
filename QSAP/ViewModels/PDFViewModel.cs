using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QSAP.ViewModels
{
    public class PDFViewModel
    {
        public SummaryViewModel summary { get; set; }

        public Dictionary<string, string> userInputQ { get; set; }

        public Dictionary<string, string> userInputI { get; set; }
    }
}