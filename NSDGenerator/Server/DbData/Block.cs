using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSDGenerator.Server.DbData;

[Table("Blocks")]
public class Block
{
    public Guid Id { get; set; }
    [MaxLength(50)]
    public EnumBlockType BlockType { get; set; }
    [Required]
    public string JsonData { get; set; }

    public Guid DiagramId { get; set; }
    public Diagram Diagram { get; set; }
}
