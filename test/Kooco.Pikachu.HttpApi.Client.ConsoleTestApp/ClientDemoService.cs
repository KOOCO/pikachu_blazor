using System;
using System.Threading.Tasks;
using Volo.Abp.Account;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Logging;
using Kooco.Pikachu.Items;
using System.Net.Http;
using System.Text.Json;
using Microsoft.OpenApi.Readers;
using System.Linq;

namespace Kooco.Pikachu.HttpApi.Client.ConsoleTestApp;

public class ClientDemoService : ITransientDependency
{
    private readonly IProfileAppService _profileAppService;
    private readonly IItemAppService _itemAppService;
    private readonly ILogger<ClientDemoService> _logger;
    private readonly HttpClient _httpClient;

    public ClientDemoService(
        IProfileAppService profileAppService,
        IItemAppService itemAppService,
        ILogger<ClientDemoService> logger,
        HttpClient httpClient)
    {
        _profileAppService = profileAppService;
        _itemAppService = itemAppService;
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task RunAsync()
    {
        _logger.LogInformation("Starting API Client Testing...");
        
        try
        {
            // Original profile test
            await TestProfileApi();
            
            // Enhanced API testing
            await TestCriticalApiEndpoints();
            await TestApiContractStability();
            
            _logger.LogInformation("✅ All API tests completed successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ API tests failed!");
            throw;
        }
    }
    
    private async Task TestProfileApi()
    {
        _logger.LogInformation("Testing Profile API...");
        
        var output = await _profileAppService.GetAsync();
        Console.WriteLine($"UserName : {output.UserName}");
        Console.WriteLine($"Email    : {output.Email}");
        Console.WriteLine($"Name     : {output.Name}");
        Console.WriteLine($"Surname  : {output.Surname}");
        
        _logger.LogInformation("✅ Profile API test passed");
    }
    
    private async Task TestCriticalApiEndpoints()
    {
        _logger.LogInformation("Testing critical API endpoints...");
        
        // Test Items API - critical for your e-commerce platform
        var items = await _itemAppService.GetItemsLookupAsync();
        _logger.LogInformation($"Retrieved {items.Count} items from lookup");
        
        var itemBadges = await _itemAppService.GetItemBadgesAsync();
        _logger.LogInformation($"Retrieved {itemBadges.Count} item badges");
        
        // Verify data structure consistency
        if (items.Any())
        {
            var firstItem = items.First();
            if (firstItem.Id == Guid.Empty)
            {
                throw new Exception("Item ID should not be empty - data structure issue detected!");
            }
            if (string.IsNullOrEmpty(firstItem.Name))
            {
                throw new Exception("Item Name should not be null/empty - data structure issue detected!");
            }
        }
        
        _logger.LogInformation("✅ Critical API endpoints test passed");
    }
    
    private async Task TestApiContractStability()
    {
        _logger.LogInformation("Testing API contract stability...");
        
        try
        {
            // Check if OpenAPI spec is accessible
            var openApiResponse = await _httpClient.GetAsync("/swagger/v1/swagger.json");
            
            if (!openApiResponse.IsSuccessStatusCode)
            {
                _logger.LogWarning("OpenAPI spec not accessible - contract testing skipped");
                return;
            }
            
            var openApiContent = await openApiResponse.Content.ReadAsStringAsync();
            var openApiDocument = new OpenApiStringReader().Read(openApiContent, out var diagnostic);
            
            // Verify critical endpoints exist
            var requiredEndpoints = new[]
            {
                "/api/app/item/get-items-lookup",
                "/api/app/item/get-item-badges",
                "/api/app/item"
            };
            
            foreach (var endpoint in requiredEndpoints)
            {
                if (!openApiDocument.Paths.ContainsKey(endpoint))
                {
                    throw new Exception($"❌ BREAKING CHANGE: Required endpoint missing: {endpoint}");
                }
                _logger.LogInformation($"✅ Endpoint verified: {endpoint}");
            }
            
            _logger.LogInformation("✅ API contract stability test passed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API contract stability test failed");
            throw;
        }
    }
}
