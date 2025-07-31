using System.Collections.Generic;

namespace KitchenBuddyAPI.Models;

public class Recipe
{
    public int Id { get; set; }
    public required string Ingridients { get; set; }
    public required string Directions { get; set; }
    public required string NutritionalBenefits { get; set; }
}