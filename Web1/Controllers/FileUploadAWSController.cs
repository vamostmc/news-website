using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web1.Service.AWS;

namespace Web1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadAWSController : ControllerBase
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IConfiguration _config;
        private readonly IUploadAWSService _uploadAWS;

        public FileUploadAWSController(IAmazonS3 s3Client, IConfiguration config, IUploadAWSService uploadAWS) 
        {
            _s3Client = s3Client;
            _config = config;
            _uploadAWS = uploadAWS;
        }

        [HttpPost("upload-AWS")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            var URL = await _uploadAWS.UploadFileToAWS(file);
            return Ok(URL);
        }

        [HttpPost("delete-AWS")]
        public async Task<IActionResult> DeleteFIle(string name)
        {
            var mess = await _uploadAWS.DeleteFileAWS(name);
            return Ok(mess);
        }
    }
}
