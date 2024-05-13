using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BuzzBid.Models;

public partial class User
{
    [Key]
    public string UserName { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
    [Required]
    public string FirstName { get; set; } = null!;
    [Required]
    public string LastName { get; set; } = null!;

    public virtual ICollection<Bidding> Biddings { get; set; } = new List<Bidding>();

    public virtual ICollection<Item> ItemListByNavigations { get; set; } = new List<Item>();

    public virtual ICollection<Item> ItemWinnerNavigations { get; set; } = new List<Item>();
}
