namespace SharePointAddInAssignment.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("HumanResources.CopyTable")]
    public partial class CopyTable
    {
        [Key]
        [Column(Order = 0)]
        public short DepartmentID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string Name { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(50)]
        public string GroupName { get; set; }

        [Key]
        [Column(Order = 3)]
        public DateTime ModifiedDate { get; set; }
    }
}
