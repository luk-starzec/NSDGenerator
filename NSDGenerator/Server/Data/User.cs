﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSDGenerator.Server.Data;

[Table("Users")]
public class User
{
    [Key, MaxLength(500)]
    public string Name { get; set; }

    [Column(TypeName = "nvarchar(100)"), Required]
    public string Password { get; set; }

    public DateTime Created { get; set; }

    public bool IsEnabled { get; set; }
}
