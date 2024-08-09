namespace StudentApp.Services
{
    public class FileService
    {
        public async Task<string> SaveAttachment(IFormFile file, Guid attachmentId)
{
    string uploadFolder = Path.Combine("Uploads", attachmentId.ToString());
    
    if (!Directory.Exists(uploadFolder))
    {
        Directory.CreateDirectory(uploadFolder);
    }

    string filePath = Path.Combine(uploadFolder, file.FileName);

    using (var fileStream = new FileStream(filePath, FileMode.Create))
    {
        await file.CopyToAsync(fileStream);
    }

    return filePath;
}

    }
}
