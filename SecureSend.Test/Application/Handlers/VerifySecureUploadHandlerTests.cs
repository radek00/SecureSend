using Moq;
using SecureSend.Application.DTO;
using SecureSend.Application.Exceptions;
using SecureSend.Application.Queries;
using SecureSend.Application.Queries.Handlers;
using SecureSend.Application.Services;
using SecureSend.Domain.ReadModels;

namespace SecureSend.Test.Application.Handlers;

public class VerifySecureUploadHandlerTests
{
    #region ARRANGE
    
    private readonly Mock<ISecureUploadReadService> _readService;
    private readonly IQueryHandler<VerifyUpload, UploadVerifyResponseDTO> _commandHandler;

    public VerifySecureUploadHandlerTests()
    {
        _readService = new Mock<ISecureUploadReadService>();
        _commandHandler = new VerifyUploadHandler(_readService.Object);
    }
    
    #endregion
    
    [Fact]
    public async Task Handle_Throws_UploadDoesNotExistException()
    {
        var query = new VerifyUpload(Guid.NewGuid());
        _readService.Setup(x => x.GetSecureUpload(query.id, It.IsAny<CancellationToken>()))!
            .ReturnsAsync(default(SecureUploadsReadModel));
        
        var exception = await Record.ExceptionAsync(() => _commandHandler.Handle(query, It.IsAny<CancellationToken>()));
        Assert.NotNull(exception);
        Assert.IsType<UploadDoesNotExistException>(exception);

    }
    
    [Fact]
    public async Task Handle_Throws_UploadExpiredException()
    {
        var upload = new SecureUploadsReadModel()
        {
            Id = Guid.NewGuid(), UploadDate = DateTime.Now, ExpiryDate = DateTime.Now.AddDays(-5), IsViewed = false
        };
        var query = new VerifyUpload(Guid.NewGuid());
        _readService.Setup(x => x.GetSecureUpload(query.id, It.IsAny<CancellationToken>()))!
            .ReturnsAsync(upload);
        
        var exception = await Record.ExceptionAsync(() => _commandHandler.Handle(query, It.IsAny<CancellationToken>()));
        Assert.NotNull(exception);
        Assert.IsType<UploadExpiredException>(exception);

    }
    
    [Fact]
    public async Task Handle_Returns_Protected()
    {
        var upload = new SecureUploadsReadModel()
        {
            Id = Guid.NewGuid(), UploadDate = DateTime.Now, ExpiryDate = DateTime.Now.AddDays(5), IsViewed = false, PasswordHash = Array.Empty<byte>()
        };
        var query = new VerifyUpload(Guid.NewGuid());
        _readService.Setup(x => x.GetSecureUpload(query.id, It.IsAny<CancellationToken>()))!
            .ReturnsAsync(upload);
        
        var result  = await _commandHandler.Handle(query, It.IsAny<CancellationToken>());
        Assert.NotNull(result);
        Assert.IsType<UploadVerifyResponseDTO>(result);
        Assert.True(result.IsProtected);

    }
    
    [Fact]
    public async Task Handle_Returns_NotProtected()
    {
        var upload = new SecureUploadsReadModel()
        {
            Id = Guid.NewGuid(), UploadDate = DateTime.Now, ExpiryDate = DateTime.Now.AddDays(5), IsViewed = false
        };
        var query = new VerifyUpload(Guid.NewGuid());
        _readService.Setup(x => x.GetSecureUpload(query.id, It.IsAny<CancellationToken>()))!
            .ReturnsAsync(upload);
        
        var result  = await _commandHandler.Handle(query, It.IsAny<CancellationToken>());
        Assert.NotNull(result);
        Assert.IsType<UploadVerifyResponseDTO>(result);
        Assert.False(result.IsProtected);

    }
}