using System.Conllections.Generic;

namespace KitchenBuddyAPI.Models;

public class Recipe
{
    public int Id { get; set; }
    public List<string> Ingridients { get; set; }
    public List<string> Directions { get; set; }
    public string NutritionalBenefits { get; set; }
}