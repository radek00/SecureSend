using MediatR;
using Moq;
using SecureSend.Application.Commands;
using SecureSend.Application.Commands.Handlers;
using SecureSend.Application.Services;
using SecureSend.Domain.Entities;
using SecureSend.Domain.Factories;
using SecureSend.Domain.Repositories;
using SecureSend.Domain.ValueObjects;

namespace SecureSend.Test.Application.Handlers;

public class CancelUploadHandlerTests
{
    private readonly Mock<ISecureSendUploadRepository> _repository;
    private readonly Mock<IFileService> _fileService;
    private readonly SecureSendUpload upload;
    private readonly ISecureSendUploadFactory _factory;
    private readonly ICommandHandler<CancelUpload, Unit> _commandHandler;
    
    public CancelUploadHandlerTests()
    {
        _repository = new Mock<ISecureSendUploadRepository>();
        _factory = new SecureSendUploadFactory();
        _fileService = new Mock<IFileService>();
        _commandHandler = new CancelUploadHandler(_fileService.Object, _repository.Object);
        upload = _factory.CreateSecureSendUpload(Guid.NewGuid(), DateTime.Now, DateTime.Now.AddDays(5), false, String.Empty);
    }
    
    [Fact]
    public async void Handle_Succeeds()
    {
        upload.AddFile(new SecureSendFile("test.txt", "text/plain", new long()));
        var command = new CancelUpload(Guid.NewGuid(), "test.txt");
        _repository.Setup(x => x.GetAsync(command.id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(upload);

        Assert.Single(upload.Files);
        var exception = await Record.ExceptionAsync(() => _commandHandler.Handle(command, It.IsAny<CancellationToken>()));
        Assert.Null(exception);
        Assert.Empty(upload.Files);
    }
}