//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QSAP.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Section
    {
        public Section()
        {
            this.Questions = new HashSet<Question>();
        }
    
        public int Id { get; set; }
        public int Number { get; set; }
        public int QSAP_ID { get; set; }
        public string Message { get; set; }
        public string Name { get; set; }
    
        public virtual QSAP QSAP { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
    }
}