using MediatR;
using Moq;
using SecureSend.Application.Commands;
using SecureSend.Application.Commands.Handlers;
using SecureSend.Application.Exceptions;
using SecureSend.Application.Services;
using SecureSend.Domain.Entities;
using SecureSend.Domain.Repositories;

namespace SecureSend.Test.Application.Handlers;

public class DeleteSecureUploadHandlerTest
{
    #region ARRANGE
    
    private readonly Mock<ISecureSendUploadRepository> _repository;
    private readonly Mock<IFileService> _fileService;
    private readonly ICommandHandler<DeleteSecureUpload, Unit> _commandHandler;

    public DeleteSecureUploadHandlerTest()
    {
        _repository = new Mock<ISecureSendUploadRepository>();
        _fileService = new Mock<IFileService>();
        _commandHandler = new DeleteSecureUploadHandler(_repository.Object, _fileService.Object);
    }
    #endregion
    
    [Fact]
    public async void Handle_Succeeds()
    {
        var command = new DeleteSecureUpload(Guid.NewGuid());
        _repository.Setup(x => x.GetAsync(command.id, It.IsAny<CancellationToken>())).ReturnsAsync(new SecureSendUpload());
        var exception = await Record.ExceptionAsync(() => _commandHandler.Handle(command, It.IsAny<CancellationToken>()));
        Assert.Null(exception);
    }
    
    [Fact]
    public async void Handle_Throws_UploadDoesNotExistException()
    {
        var command = new DeleteSecureUpload(Guid.NewGuid());
        _repository.Setup(x => x.GetAsync(command.id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SecureSendUpload)null);
        

        var exception = await Record.ExceptionAsync(() => _commandHandler.Handle(command, It.IsAny<CancellationToken>()));
        Assert.NotNull(exception);
        Assert.IsType<UploadDoesNotExistException>(exception);

    }
}