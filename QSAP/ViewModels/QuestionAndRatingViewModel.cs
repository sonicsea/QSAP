using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QSAP.Models;

namespace QSAP.ViewModels
{
    public class QuestionAndRatingViewModel
    {
        public IEnumerable<Question> questions { get; set; }
        public IEnumerable<Rating> ratings { get; set; }

        //public IEnumerable<Indicator> indicators { get; set; }        

        public Section section { get; set; }
    }
}