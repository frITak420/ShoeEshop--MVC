﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web.Models
{

    public partial class Order
    {
        public Order()
        {
            CartItems = new HashSet<CartItem>();
        }

        [Key]
        [Column("orderNumber", TypeName = "int(11)")]
        public int OrderNumber { get; set; }
        [Column("total", TypeName = "int(11)")]
        public int? Total { get; set; }
        [Column("created", TypeName = "datetime")]
        public DateTime? Created { get; set; }
        [Column("isCompleted")]
        public bool? IsCompleted { get; set; } = false;
        [Column("customerId", TypeName = "int(11)")]
        public int? CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        [InverseProperty("Orders")]
        public virtual Customer Customer { get; set; }
        [InverseProperty("Order")]
        public virtual ICollection<CartItem> CartItems { get; set; }
    }
}