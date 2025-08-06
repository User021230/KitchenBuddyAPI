using System.Collections.Generic;

namespace KitchenBuddyAPI.Models;

public class Recipe
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Ingridients { get; set; }
    public  string Directions { get; set; }
    public  string NutritionalBenefits { get; set; }

    // Foreign key
    public int UserId { get; set; }

    // Navigation property
    public User User { get; set; }
}