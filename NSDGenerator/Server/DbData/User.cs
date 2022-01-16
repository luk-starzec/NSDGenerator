using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSDGenerator.Server.DbData;

[Table("Users")]
public class User
{
    [Key, MaxLength(400)]
    public string Name { get; set; }

    [Column(TypeName = "nvarchar(100)"), Required]
    public string Password { get; set; }

    public DateTime Created { get; set; }

    public bool IsEnabled { get; set; }
}
