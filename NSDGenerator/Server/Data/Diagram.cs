using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSDGenerator.Server.Data;

[Table("Diagram")]
public class Diagram
{
    [Key]
    public Guid Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; }

    public bool IsPrivate { get; set; }

    public DateTime Created { get; set; }

    public DateTime Modified { get; set; }


    [Required]
    public string UserName { get; set; }
    public User User { get; set; }

    public Guid? RootBlockId { get; set; }
    [ForeignKey("RootBlockId")]
    public Block RootBlock { get; set; }
}
