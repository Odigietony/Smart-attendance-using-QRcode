//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AttendanceWebApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    public partial class Lecturer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Lecturer()
        {
            this.LecturerRegs = new HashSet<LecturerReg>();
        }
    
        public int lecturerID { get; set; }

        [Display(Name ="Full Name")]
        public string lecturer_name { get; set; }

        [Display(Name ="Email Address")]
        [DataType(DataType.EmailAddress)]
        public string lecturer_email { get; set; }

        [Display(Name ="Password")]
        [DataType(DataType.Password)]
        public string lecturer_password { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LecturerReg> LecturerRegs { get; set; }
    }
}