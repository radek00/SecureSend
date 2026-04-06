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
    private const string SampleMetadata = "{\"fileName\":\"test_file\",\"contentType\":\"application/octet-stream\",\"fileSize\":0}";

    public SecureSendUploadTests()
    {
        _factory = new SecureSendUploadFactory();
        upload = _factory.CreateSecureSendUpload(Guid.NewGuid(), DateTime.Now.AddDays(5), "testing");
    }

    #endregion

    [Fact]
    public void AddFile_Succeeds()
    {
        upload.AddFile(SecureSendFile.Create(SampleMetadata));
        Assert.Single(upload.Files);
    }

    [Fact]
    public void AddMultipleFiles_Succeeds()
    {
        IList<SecureSendFile> files = new List<SecureSendFile>();
        for (int i = 0; i < 5; i++)
        {
            var metadata = $"{{\"fileName\":\"{i}_test_file\",\"contentType\":\"application/octet-stream\",\"fileSize\":0}}";
            files.Add(SecureSendFile.Create(metadata));
        }
        upload.AddMultipleFiles(files);
        Assert.Equal(5, upload.Files.Count);
    }

    [Fact]
    public void SecureSendFile_Has_Random_File_Names()
    {
        var file1 = SecureSendFile.Create(SampleMetadata);
        var file2 = SecureSendFile.Create(SampleMetadata);
        
        Assert.NotEqual(file1.FileName, file2.FileName);
    }

    [Fact]
    public void AddFile_Throws_FileAlreadyExistsException()
    {
        var file = SecureSendFile.Create(SampleMetadata);
        upload.AddFile(file);
        var exception = Record.Exception(() => upload.AddFile(file));
        Assert.NotNull(exception);
        Assert.IsType<FileAlreadyExistsException>(exception);
    }
    
    [Fact]
    public void RemoveFile_Succeeds()
    {
        var file = SecureSendFile.Create(SampleMetadata);
        upload.AddFile(file);
        Assert.Single(upload.Files);
        upload.RemoveFile(file.FileName);
        Assert.Empty(upload.Files);
    }

    [Fact]
    public void VerifyHash_Throws_InvalidPasswordException()
    {
        var exception = Record.Exception(() => upload!.PasswordHash!.VerifyHash("wrong password"));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPasswordException>(exception);
    }
    
    [Fact]
    public void VerifyHash_Succeeds()
    {
        upload!.PasswordHash!.VerifyHash("testing");
    }

    [Fact]
    public void Create_GeneratesRandomFileName()
    {
        var file1 = SecureSendFile.Create(SampleMetadata);
        var file2 = SecureSendFile.Create(SampleMetadata);
        Assert.NotEqual(file1.FileName, file2.FileName);
    }

    [Fact]
    public void Create_ThrowsException_WhenMetadataEmpty()
    {
        var exception = Record.Exception(() => SecureSendFile.Create(""));
        Assert.IsType<MissingMetadataException>(exception);
    }

    [Fact]
    public void Create_ThrowsException_WhenMetadataNull()
    {
        var exception = Record.Exception(() => SecureSendFile.Create(null!));
        Assert.IsType<MissingMetadataException>(exception);
    }

    [Fact]
    public void Create_StoresMetadata()
    {
        var metadata = "{\"fileName\":\"test.txt\",\"contentType\":\"text/plain\",\"fileSize\":1024}";
        var file = SecureSendFile.Create(metadata);
        Assert.Equal(metadata, file.Metadata);
    }
}