using System;
using System.Collections.Generic;

namespace BuzzBid.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string Description { get; set; } = null!;

    public Category(int id, string Description) : this()
    {
        this.CategoryId = id;
        this.Description = Description;
       
    }

    public Category()
    {
    }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
