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
    
    public partial class QSAP
    {
        public QSAP()
        {
            this.Sections = new HashSet<Section>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    
        public virtual ICollection<Section> Sections { get; set; }
    }
}