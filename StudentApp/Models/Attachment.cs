﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StudentApp.Models;

[Table("Attachment")]
public partial class Attachment
{
    [Key]
    public int Id { get; set; }

    [Unicode(false)]
    public string FileName { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string FileType { get; set; }

    public byte[] FileContent { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateCreate { get; set; } = DateTime.Now;

    [InverseProperty("Attachment")]
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}