using System.Net;
using System.Net.Http.Json;
using SecureSend.Application.DTO;
using Xunit;

namespace SecureSend.IntegrationTests.Controllers;

public class SecureSendControllerTests(SecureSendWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task UploadFlow_ShouldSuccessfully_CreateUpload_UploadFile_Verify_View_Download_Delete()
    {
        // 1. Create secure upload
        var uploadId = Guid.NewGuid();
        var createPayload = new
        {
            uploadId,
            expiryDate = (DateTime?)null,
            password = (string?)null
        };

        var createResponse = await _client.PostAsJsonAsync("/api/SecureSend", createPayload);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        // 2. Upload chunk
        var chunkId = Guid.NewGuid();
        var metadata = "testfile.txt";
        var fileContent = new byte[] { 1, 2, 3, 4, 5 };
        
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(uploadId.ToString()), "uploadId");
        content.Add(new StringContent("1"), "chunkNumber");
        content.Add(new StringContent("1"), "totalChunks");
        content.Add(new StringContent(chunkId.ToString()), "chunkId");
        content.Add(new StringContent(metadata), "metadata");
        
        var fileBytes = new ByteArrayContent(fileContent);
        content.Add(fileBytes, "chunk", "testfile.txt");

        var uploadChunkResponse = await _client.PostAsync("/api/SecureSend/uploadChunks", content);
        Assert.Equal(HttpStatusCode.NoContent, uploadChunkResponse.StatusCode);

        // 3. Verify upload
        var verifyResponse = await _client.GetAsync($"/api/SecureSend?id={uploadId}");
        Assert.Equal(HttpStatusCode.OK, verifyResponse.StatusCode);

        var verifyData = await verifyResponse.Content.ReadFromJsonAsync<UploadVerifyResponseDTO>();
        Assert.NotNull(verifyData);
        Assert.Equal(uploadId, verifyData.SecureUploadId);
        Assert.False(verifyData.IsProtected);

        // 4. View upload
        var viewPayload = new { id = uploadId, password = (string?)null };
        var viewResponse = await _client.PutAsJsonAsync("/api/SecureSend", viewPayload);
        Assert.Equal(HttpStatusCode.OK, viewResponse.StatusCode);

        var viewData = await viewResponse.Content.ReadFromJsonAsync<SecureUploadDto>();
        Assert.NotNull(viewData);
        Assert.Equal(uploadId, viewData.SecureUploadId);
        Assert.NotNull(viewData.Files);
        Assert.Single(viewData.Files);

        var uploadedFile = viewData.Files.First();
        Assert.Equal(metadata, uploadedFile.Metadata);
        var generatedFileName = uploadedFile.FileName;

        // 5. Download file
        var downloadResponse = await _client.GetAsync($"/api/SecureSend/download?id={uploadId}&fileName={generatedFileName}");
        Assert.Equal(HttpStatusCode.OK, downloadResponse.StatusCode);
        
        var downloadedBytes = await downloadResponse.Content.ReadAsByteArrayAsync();
        Assert.Equal(fileContent, downloadedBytes);

        // 6. Delete upload
        var deleteResponse = await _client.DeleteAsync($"/api/SecureSend?id={uploadId}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // 7. Verify deletion
        var verifyDeletedResponse = await _client.GetAsync($"/api/SecureSend?id={uploadId}");
        Assert.Equal(HttpStatusCode.NotFound, verifyDeletedResponse.StatusCode);
    }
}
