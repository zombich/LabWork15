using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AuthLibrary.Models;

public partial class CinemaUserRole
{
    public int RoleId { get; set; }

    public string Name { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<CinemaUser> CinemaUsers { get; set; } = new List<CinemaUser>();

    public virtual ICollection<CinemaPrivilege> Privileges { get; set; } = new List<CinemaPrivilege>();
}
