using System;
using System.Collections.Generic;

namespace BuzzBid.Models;

public partial class Admin
{
    public string? UserName { get; set; }

    public string Position { get; set; } = null!;

    public virtual User? UserNameNavigation { get; set; }
}
