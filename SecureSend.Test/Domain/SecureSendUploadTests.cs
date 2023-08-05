using SecureSend.Domain.Entities;
using SecureSend.Domain.Exceptions;
using SecureSend.Domain.Factories;
using SecureSend.Domain.ValueObjects;

namespace SecureSend.Test.Domain;

public class SecureSendUploadTests
{
    #region ARRANGE

    private readonly ISecureSendUploadFactory _factory;
    private readonly SecureSendUpload upload;

    public SecureSendUploadTests()
    {
        _factory = new SecureSendUploadFactory();
        upload = _factory.CreateSecureSendUpload(Guid.NewGuid(), DateTime.Now, DateTime.Now.AddDays(5), false);
    }

    #endregion

    [Fact]
    public void AddFile_Succeeds()
    {
        upload.AddFile(new SecureSendFile("test_file", "application/octet-stream"));
        Assert.Single(upload.Files);
    }

    [Fact]
    public void AddMultipleFiles_Succeeds()
    {
        IList<SecureSendFile> files = new List<SecureSendFile>();
        for (int i = 0; i < 5; i++)
        {
            files.Add(new SecureSendFile($"{i}_test_file", "application/octet-stream"));
        }
        upload.AddMultipleFiles(files);
        Assert.Collection(upload.Files,
            x => Assert.Equal("0_test_file", x.FileName),
            x => Assert.Equal("1_test_file", x.FileName),
            x => Assert.Equal("2_test_file", x.FileName),
            x => Assert.Equal("3_test_file", x.FileName),
            x => Assert.Equal("4_test_file", x.FileName));

    }

    [Fact]
    public void AddFile_Throws_FileAlreadyExistsException()
    {
        upload.AddFile(new SecureSendFile("test_file", "application/octet-stream"));
        var exception = Record.Exception(() => upload.AddFile(new SecureSendFile("test_file", "application/octet-stream")));
        Assert.NotNull(exception);
        Assert.IsType<FileAlreadyExistsException>(exception);
    }
}