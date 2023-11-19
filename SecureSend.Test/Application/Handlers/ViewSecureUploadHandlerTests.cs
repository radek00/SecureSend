using Moq;
using SecureSend.Application.Commands;
using SecureSend.Application.Commands.Handlers;
using SecureSend.Application.DTO;
using SecureSend.Application.Exceptions;
using SecureSend.Domain.Entities;
using SecureSend.Domain.Factories;
using SecureSend.Domain.Repositories;

namespace SecureSend.Test.Application.Handlers;

public class ViewSecureUploadHandlerTests
{
    #region ARRANGE
    
    private readonly Mock<ISecureSendUploadRepository> _repository;
    private readonly ISecureSendUploadFactory _factory;
    private readonly ICommandHandler<ViewSecureUpload, SecureUploadDto> _commandHandler;

    public ViewSecureUploadHandlerTests()
    {
        _repository = new Mock<ISecureSendUploadRepository>();
        _commandHandler = new ViewSecureUploadHandler(_repository.Object);
        _factory = new SecureSendUploadFactory();
    }
    
    #endregion
    
    [Fact]
    public async void Handle_Throws_InvalidPasswordException()
    {
        var upload = _factory.CreateSecureSendUpload(Guid.NewGuid(), DateTime.Now, DateTime.Now.AddDays(-5), false, "testing");
        var command = new ViewSecureUpload(Guid.NewGuid(), "wrong password");
        _repository.Setup(x => x.GetAsync(command.id, It.IsAny<CancellationToken>()))!
            .ReturnsAsync(upload);
        
        var exception = await Record.ExceptionAsync(() => _commandHandler.Handle(command, It.IsAny<CancellationToken>()));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPasswordException>(exception);
    }
    
    [Fact]
    public async void Handle_Succeeds_EmptyPassword()
    {
        var upload = _factory.CreateSecureSendUpload(Guid.NewGuid(), DateTime.Now, DateTime.Now.AddDays(-5), false, "");
        var command = new ViewSecureUpload(Guid.NewGuid(), "wrong password");
        _repository.Setup(x => x.GetAsync(command.id, It.IsAny<CancellationToken>()))!
            .ReturnsAsync(upload);
        
        var result  = await _commandHandler.Handle(command, It.IsAny<CancellationToken>());
        Assert.True(upload.IsViewed);
        Assert.NotNull(result);
        Assert.IsType<SecureUploadDto>(result);
    }
    
    [Fact]
    public async void Handle_Succeeds_Protected()
    {
        var upload = _factory.CreateSecureSendUpload(Guid.NewGuid(), DateTime.Now, DateTime.Now.AddDays(5), false, "testing");
        var command = new ViewSecureUpload(Guid.NewGuid(), "testing");
        _repository.Setup(x => x.GetAsync(command.id, It.IsAny<CancellationToken>()))!
            .ReturnsAsync(upload);
        
        var result  = await _commandHandler.Handle(command, It.IsAny<CancellationToken>());
        Assert.True(upload.IsViewed);
        Assert.NotNull(result);
        Assert.IsType<SecureUploadDto>(result);
    }
}