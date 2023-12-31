﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Orders = new HashSet<Order>();
        }

        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column("firstName")]
        [StringLength(50)]
        public string FirstName { get; set; }
        [Column("lastName")]
        [StringLength(50)]
        public string LastName { get; set; }
        [Column("email")]
        [StringLength(100)]
        public string Email { get; set; }
        [Column("mobile", TypeName = "int(11)")]
        public int? Mobile { get; set; }
        [Column("city")]
        [StringLength(50)]
        public string City { get; set; }
        public string Street { get; set; }
        [Column("country")]
        [StringLength(50)]
        public string Country { get; set; }
        [Column("zip", TypeName = "int(11)")]
        public int? Zip { get; set; }

        [InverseProperty("Customer")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}