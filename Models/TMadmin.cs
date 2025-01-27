using System;
using System.Collections.Generic;

namespace diveWebAPI.Models;

public partial class TMadmin
{
    public int AdminId { get; set; }

    public string? UserName { get; set; }

    public string? PasswordHash { get; set; }

    public string? Email { get; set; }

    public string? RoleName { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? LastLogin { get; set; }
}
