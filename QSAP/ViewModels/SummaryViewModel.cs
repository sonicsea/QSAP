using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QSAP.Models;

namespace QSAP.ViewModels
{
    public class SummaryViewModel
    {
        public List<Question> questions { get; set; }

        public List<Section> sections { get; set; }

        public IEnumerable<Rating> ratings { get; set; }
    }
}