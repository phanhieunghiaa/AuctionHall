using Firebase.Auth;
using Firebase.Auth.Providers;
using FirebaseAdmin.Auth;
using Google.Cloud.Storage.V1;
using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Models;
using JewelryAuction.Services.Helpers;
using JewelryAuction.Services.Services;
using JewelryAuction.Services.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Jewelry_auction_system.Controllers
{
    [Route("api/Images")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        //configure Firebase
        private readonly string _projectId = "net1701-jewelry-auction-system";
        private readonly string _bucketName = "net1701-jewelry-auction-system.appspot.com";

        private readonly IImageServices _imgServices;
        private readonly IProductServices _productServices;
        private readonly JwtTokenHelper _jwtTokenHelper;
        private readonly IConfiguration _configuration;

        public ImageController(IImageServices imgServices, JwtTokenHelper jwtTokenHelper, IConfiguration configuration, IProductServices productServices)
        {
            _imgServices = imgServices;
            _jwtTokenHelper = jwtTokenHelper;
            _configuration = configuration;
            _productServices = productServices;

        }   
        [HttpPost("upload-images")]
        public async Task<IActionResult> UploadImage([FromHeader] String productID, [FromForm] ImageUploadViewModel request)
        {
            try
            {
                if (productID == null)
                {
                    return BadRequest("ProductID is missing or wrong");
                }
                var product = await _productServices.GetProductsById(productID);
                if (product == null)
                {
                    return BadRequest("ProductID not found");
                }
                if (request == null || request.File == null || request.File.Length == 0)
                {
                    return BadRequest("Image file is missing");
                }


                using (var memoryStream = new MemoryStream())
                {
                    await request.File.CopyToAsync(memoryStream);
                    var bytes = memoryStream.ToArray();

                    // Initialize Firebase Admin SDK
                    var credential = Google.Apis.Auth.OAuth2.GoogleCredential.FromFile("C:\\Users\\Kaz\\source\\repos\\jewelry-auction-system_jas\\Jewelry auction system\\Jewelry auction system\\net1701-jewelry-auction-system-firebase-adminsdk-ncuhn-6f67e09863.json");
                    //tạm thời hardcode, thay sau FromFile thành đường dẫn file net1701-jewelry...
                    var storage = StorageClient.Create(credential);

                    // Construct the object name (path) in Firebase Storage
                    var objectName = $"images/{DateTime.Now.Ticks}_{request.File.FileName}";

                    // Upload the file to Firebase Storage
                    var response = await storage.UploadObjectAsync(
                        bucket: _bucketName,
                        objectName: objectName,
                        contentType: request.File.ContentType,
                        source: new MemoryStream(bytes)
                    );

                    // Optionally, get the public URL of the uploaded file
                    var storageObject = await storage.GetObjectAsync(_bucketName, objectName);
                    var downloadUrl = storageObject.MediaLink;

                    var result = await _imgServices.AddImage(downloadUrl, productID);
                    return Ok(result ? "Upload Successful" : "Upload failed");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #region Get all images by productId

        [HttpGet("Images/{productId}")]
        public async Task<IActionResult> GetImagesByProductId(string productId)
        {
            try
            {
                var imgUrls = await _imgServices.GetAllImagesAsync(productId);
                return Ok(imgUrls);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        #endregion
    }
}

