using Web1.Models;

namespace Web1.Service.AWS
{
    public interface IUploadAWSService
    {
        public Task<string> UploadFileToAWS(IFormFile file);

        public Task<Success> DeleteFileAWS(string nameFile);
    }
}
