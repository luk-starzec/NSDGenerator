using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSDGenerator.Server.DbData;

[Table("RegistrationCodes")]
public class RegistrationCode
{
    [Key]
    public string Code { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ValidTo { get; set; }
}
