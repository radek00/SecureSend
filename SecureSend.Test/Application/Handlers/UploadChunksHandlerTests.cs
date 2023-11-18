using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using SecureSend.Application.Commands;
using SecureSend.Application.Commands.Handlers;
using SecureSend.Application.Exceptions;
using SecureSend.Application.Services;
using SecureSend.Domain.Entities;
using SecureSend.Domain.Factories;
using SecureSend.Domain.Repositories;

namespace SecureSend.Test.Application.Handlers;

public class UploadChunksHandlerTests
{
    #region ARRANGE
    
    private readonly Mock<ISecureSendUploadRepository> _repository;
    private readonly Mock<IFileService> _fileService;
    private readonly Mock<IFormFile> _file;
    private readonly ISecureSendUploadFactory _factory;
    private readonly ICommandHandler<UploadChunks, Unit> _commandHandler;
    private readonly SecureSendUpload upload;

    public UploadChunksHandlerTests()
    {
        _repository = new Mock<ISecureSendUploadRepository>();
        _fileService = new Mock<IFileService>();
        _commandHandler = new UploadChunksHandler(_fileService.Object, _repository.Object);
        _factory = new SecureSendUploadFactory();
        upload = _factory.CreateSecureSendUpload(Guid.NewGuid(), DateTime.Now, DateTime.Now.AddDays(5), false, String.Empty);
        _file = new Mock<IFormFile>();
        _file.Setup(x => x.FileName).Returns("file.txt");
        _file.Setup(x => x.ContentType).Returns("text/plain");
    }
    #endregion
    
    [Fact]
    public async void Handle_Succeeds()
    {
        var command = new UploadChunks(Guid.NewGuid(), 5, 5, _file.Object);
        _repository.Setup(x => x.GetAsync(command.uploadId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(upload);
        _fileService.Setup(x => x.GetChunksList(command.uploadId, It.IsAny<string>()))
            .Returns(new List<string> { "1", "2", "3", "4", "5" });

        var exception = await Record.ExceptionAsync(() => _commandHandler.Handle(command, It.IsAny<CancellationToken>()));
        Assert.Null(exception);
        Assert.Single(upload.Files);
    }
    
    [Fact]
    public async void Handle_Throws_UploadDoesNotExistException()
    {
        var command = new UploadChunks(Guid.NewGuid(), 5, 5, _file.Object);
        _repository.Setup(x => x.GetAsync(command.uploadId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(default(SecureSendUpload));
        

        var exception = await Record.ExceptionAsync(() => _commandHandler.Handle(command, It.IsAny<CancellationToken>()));
        Assert.NotNull(exception);
        Assert.IsType<UploadDoesNotExistException>(exception);

    }
    
    [Fact]
    public async void Handle_Throws_InvalidChunkCountException()
    {
        var command = new UploadChunks(Guid.NewGuid(), 5, 5, _file.Object);
        _repository.Setup(x => x.GetAsync(command.uploadId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(upload);
        _fileService.Setup(x => x.GetChunksList(It.IsAny<Guid>(), It.IsAny<string>()))
            .Returns(new List<string> { "1", "2" });

        var exception = await Record.ExceptionAsync(() => _commandHandler.Handle(command, It.IsAny<CancellationToken>()));
        Assert.NotNull(exception);
        Assert.IsType<InvalidChunkCountException>(exception);

    }
}