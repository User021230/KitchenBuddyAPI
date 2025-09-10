using System.Text.Json;
using KitchenBuddyAPI.Data;
using KitchenBuddyAPI.Models;
using Microsoft.AspNetCore.JsonPatch; //*
using Microsoft.AspNetCore.Mvc; 
using Microsoft.EntityFrameworkCore;


namespace KitchenBuddyAPI.Controllers;

public class RecipeRequest
{
    public required List<string> Ingredients { get; set; }
}
public class OpenAIResponse
{
    public required List<Choice> Choices { get; set; }
}

public class Choice
{
    public required Message Message { get; set; }
}

public class Message
{
    public required string Role { get; set; }
    public required string Content { get; set; }
}

[ApiController]
[Route("Recipe")]
public class RecipeController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly KitchenBuddyDbContext _context;
    private readonly IConfiguration _config;
    public RecipeController(KitchenBuddyDbContext context, IConfiguration config)
    {
        _context = context;
        _httpClient = new HttpClient();
        _config = config;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateRecipes([FromBody] RecipeRequest request)
    {
        string apiKey = _config["DeepSeek:ApiKey"];
        string prompt = GeneratePrompt(request.Ingredients);

        var requestBody = new
        {
            model = "deepseek/deepseek-r1:free",
            messages = new[]
            {
                new { role = "system", content = "You are a professional chef and nutritionist." },
                new { role = "user", content = prompt }
            },
            temperature = 0.7
        };

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "http://localhost:5108");

        var response = await _httpClient.PostAsJsonAsync("https://openrouter.ai/api/v1/chat/completions", requestBody);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return StatusCode(500, $"OpenRouter API error: {error}");
        }

        var result = await response.Content.ReadFromJsonAsync<OpenAIResponse>();

        var content = result.Choices.First().Message.Content;
        var recipes = JsonSerializer.Deserialize<List<Recipe>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return Ok(recipes);
    }

    [HttpGet]
    public async Task<IActionResult> GetRecipes()
    {
        var recipes = await _context.Recipes.ToListAsync();
        return Ok(recipes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRecipe(int id)
    {
        var recipe = await _context.Recipes.FindAsync(id);
        if (recipe == null)
        {
            return NotFound();
        }
        return Ok(recipe);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRecipe([FromBody] Recipe recipe)
    {

        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, recipe);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRecipe(int id, Recipe recipe)
    {
        if (id != recipe.Id)
        {
            return BadRequest();
        }
        _context.Entry(recipe).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateRecipe(int id, JsonPatchDocument<Recipe> patchDoc)
    {
        var recipe = await _context.Recipes.FindAsync(id);
        if (recipe == null)
        {
            return NotFound();
        }
        patchDoc.ApplyTo(recipe);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecipe(int id)
    {
        var recipe = await _context.Recipes.FindAsync(id);
        if (recipe == null)
        {
            return NotFound();
        }
        _context.Recipes.Remove(recipe);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private string GeneratePrompt(List<string> ingredients)
    {
        string ingList = string.Join(", ", ingredients);
        return $@"
        You are a professional chef and nutritionist.

        Given the following ingredients: {ingList}

        Generate as many healthy and creative recipes as possible using the provided ingredients.

        Each recipe must be a JSON object with:
        - 'name' (string),
        - 'ingredients' (string),
        - 'directions' (string),
        - 'nutritionalBenefits' (string)

        Respond with as many valid JSON array of recipe objects.
        no  '`'
        ";
    }
}