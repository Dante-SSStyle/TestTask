using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Assert = Xunit.Assert;

namespace IntegrationTest.API;

/// <summary>
/// Class for integration tests.
/// </summary>
public class DataModelApiTest: IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public DataModelApiTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }
    
    /// <summary>
    /// Test for the RightDataModelRequest.
    /// Should return error 415 because of the wrong media type.
    /// </summary>
    [Xunit.Theory]
    [InlineData("PUT")]
    public async Task ErrorModelLeftTestAsync(string method)
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new HttpRequestMessage(new HttpMethod(method), "/v1/diff/1/right");
        request.Content = new StringContent("{\"Data\": \"dGVzdA==\"}", mediaType: new MediaTypeHeaderValue("application/text"));
        
        // Act
        var response = await client.SendAsync(request);
        
        // Assert
        Assert.Equal(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
    }
    
    // /// <summary>
    // /// Test for the LeftDataModelRequest.
    // /// This doesn't work because of the some db issues.
    // /// </summary>
    // /// <param name="method"></param>
    // [Xunit.Theory]
    // [InlineData("PUT")]
    // public async Task PutDataModelLeftTestAsync(string method)
    // {
    //     // Arrange
    //     var client = _factory.CreateClient();
    //     var request = new HttpRequestMessage(new HttpMethod(method), "/v1/diff/1/left");
    //     request.Content = new StringContent("{\"Data\": \"dGVzdA==\"}", mediaType: new MediaTypeHeaderValue("application/json"));
    //     
    //     // Act
    //     var response = await client.SendAsync(request);
    //     
    //     // Assert
    //     response.EnsureSuccessStatusCode();
    //     Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    // }
}