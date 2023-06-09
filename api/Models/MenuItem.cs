using Microsoft.EntityFrameworkCore;

namespace TheMostAmazingLunchAPI.Models;

[Owned]
public class MenuItem
{
    public string? Contents { get; set; }
    public int? Price { get; set; }
}
