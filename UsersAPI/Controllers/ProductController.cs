using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using UsersAPI.Data;
using UsersAPI.Models;
using UsersAPI.Services;

namespace UsersAPI.Controllers;

public class ProductController : Controller
{
    private IWebHostEnvironment _hostEnvironment;
    private readonly IUnitOfWork _unitOfWork;

    public ProductController(IWebHostEnvironment hostEnvironment, IUnitOfWork unitOfWork)
    {
        _hostEnvironment = hostEnvironment;
        _unitOfWork = unitOfWork;
    }

    [HttpGet("Products/GetAll")]
    public async  Task<ActionResult<List<Product>>> GetAllProducts()
    {
        var products =  await _unitOfWork.Product.GetAll();
        return Ok(products);
    }
    
    [HttpPost("Create/Product")]
    public  async Task<ApiResponse<Product>> CreateProduct([FromBody] Product product)
    {
        ApiResponse<Product> response = new ApiResponse<Product>();
        try
        {
            var requestProductCode = await  _unitOfWork.Product.Get(m => m.ProductCode == product.ProductCode);
            if (requestProductCode != null)
            {
                return new ApiResponse<Product>()
                {
                    StatusCode = 201,
                    Message = "Product With same Product Code entered already exists!"
                };
             
            }
            if (!ModelState.IsValid)
            {
                response.StatusCode = 303;
                response.Message = "All Fields are required!";
            }

            await _unitOfWork.Product.Add(product);
            _unitOfWork.Save();
            response.StatusCode = 200;
            response.Message = "Product created!";
            response.Data = product;
        }
        catch (Exception e)
        {
            response.StatusCode = 404;
            response.Message = e.Message;
        }

        return response;
    }

      
    [HttpPut("UpdateProduct/{id}")]
    public async Task<ApiResponse<Product>> UpdateProduct([FromBody] Product product, int id)
    {
        ApiResponse<Product> response = new ApiResponse<Product>();
        try
        {

            if (ModelState.IsValid)
            {
                var productDb = await _unitOfWork.Product.Get(p => p.Id == id);
                if (productDb == null)
                {
                    response.StatusCode = 404;
                    response.Message = "Product not found!";
                }
                await _unitOfWork.Product.Update(product);
                _unitOfWork.Save();
                response.StatusCode = 200;
                response.Message = "Product updated";
                response.Data = product;
            }
            else
            {
                response.StatusCode = 300;
                response.Message = "All fields are required!";
            }
        }
        catch (Exception e)
        {
            response.StatusCode = 300;
            response.Message = e.Message;
        }

        return response;
    }

    [HttpGet("GetProduct/{productCode}")]
    public async Task<ApiResponse<Product>> GetProduct(string productCode)
    {
        ApiResponse<Product> apiResponse = new ApiResponse<Product>();
        try
        {
            var product =  await _unitOfWork.Product.Get(p => p.ProductCode == productCode);
            if (product == null)
            {
                apiResponse.StatusCode = 404;
                apiResponse.Message ="Product not found on db";
            }

            apiResponse.StatusCode = 200;
            apiResponse.Data = product;
        }
        catch (Exception e)
        {
            apiResponse.Message = e.Message;
        }

        return apiResponse;
    }

    [HttpDelete("Product/Remove/{id}")]
    public async Task<ActionResult<Product>> DeleteProduct(int id)
    {
        try
        {
            var product =  await _unitOfWork.Product.Get(m => m.Id == id);
            if (product == null) return NotFound("Product not found on db");
            var images = await _unitOfWork.ProductImage.GetAllById(m=>m.ProductCode==product.ProductCode);
            foreach (var image in images)
            {
                _unitOfWork.ProductImage.Remove(image);
            }
            string filePath = GetFilePath(product.ProductCode);
            if (System.IO.Directory.Exists(filePath))
            {
                System.IO.Directory.Delete(filePath, recursive:true);
            }
            else
            {
                return NotFound("Image not found on directory!");
            }
            _unitOfWork.Product.Remove(product);
            _unitOfWork.Save();
            var response = new
            {
                Message = "Product and Its Images deleted!"
            };
            return Ok(response);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
        
    }
    

    [HttpGet("GetImages")]
    public async Task<ActionResult<List<ProductImage>>> GetImages()
    {
        var productImages = await _unitOfWork.ProductImage.GetAll();
        return Ok(productImages);
    }
 
    [HttpGet("GetByProduct/{productCode}")]
    public async Task<ActionResult<List<ProductImage>>> GetProductImages(string productCode)
    {
        var productImages =
           await  _unitOfWork.ProductImage.GetAllById(p => p.ProductCode == productCode);
        if (productImages == null)
        {
            return null;
        }
        return Ok(productImages);
    }
  
    [HttpGet("ProductImage/Download/{id}")]
    public async Task<ActionResult<ProductImage>> Download(int id)
    {
      
        try
        {
            var image =  await _unitOfWork.ProductImage.Get(m => m.Id == id);
            if (image == null) return NotFound("Image not found!");
            string filePath = GetFilePath(image.ProductCode);
            string dbImageName = Strings.Split(image.ProductFile, "/")[1];
            string imgPath = filePath + dbImageName;
            if (System.IO.File.Exists(imgPath))
            {
                MemoryStream stream = new MemoryStream();
                using (FileStream fileStream = new FileStream(imgPath, FileMode.Open))
                {
                   await  fileStream.CopyToAsync(stream);
                }

                stream.Position = 0;
                return File(stream, "image/*", dbImageName);

            }
            else
            {
                return NotFound();
            }

        }
        catch (Exception e)
        {
            return NotFound();
        }
        
    }
    
        
    [HttpDelete("ProductImage/Remove/{id}")]
    public async Task<ActionResult<ProductImage>> Delete(int id)
    {

        try
        {
            var image = await _unitOfWork.ProductImage.Get(m => m.Id == id);
            if (image == null) return NotFound("Image not found on db");
           
            string filePath = GetFilePath(image.ProductCode);
            string dbImageName = Strings.Split(image.ProductFile, "/")[1];
            string imgPath = filePath + dbImageName;
            if (System.IO.File.Exists(imgPath))
            {
                _unitOfWork.ProductImage.Remove(image);
                 _unitOfWork.Save();
                System.IO.File.Delete(imgPath);
                return Ok("Image deleted!");

            }
            else
            {
                return NotFound("Image not found on directory!");
            }
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
        
    }
   
    
    [HttpPost("UploadImage")]
    public async Task<ActionResult<BaseResponse>> UploadImage(IFormFileCollection formFileCollection, string productCode)
    {
        BaseResponse response = new BaseResponse();
        int errorCount = 0;
        int successCount = 0;
        try
        {
            var code = await _unitOfWork.Product.Get(p => p.ProductCode == productCode);
            if (code == null)
            {
                return NotFound("Product code does not exist!");
            }
            foreach (var file in formFileCollection)
            {
                string filePath = GetFilePath(productCode);
                if (!System.IO.Directory.Exists(filePath))
                {
                    System.IO.Directory.CreateDirectory(filePath);
                }
                string imgPath = filePath + file.FileName;
                string dbPath = productCode+"/"+ file.FileName;
                if (System.IO.File.Exists(imgPath))
                {
                    ProductImage? dbImg = await  _unitOfWork.ProductImage.Get(m => m.ProductFile == dbPath);
                    if (dbImg != null)
                    {
                        _unitOfWork.ProductImage.Remove(dbImg);
                         _unitOfWork.Save();
                    }
                    System.IO.File.Delete(imgPath);
                    
                }

                using (FileStream stream = System.IO.File.Create(imgPath))
                {
                    await file.CopyToAsync(stream);
                    await _unitOfWork.ProductImage.Add(new ProductImage
                    {
                        ProductCode = productCode,
                        ProductFile = dbPath
                    });
                     _unitOfWork.Save();
                    successCount++;
                }
            }
           
        }
        catch (Exception e)
        {
            errorCount++;
            response.StatusCode = 401;
            response.Message = e.Message;
        }
        response.StatusCode = 200;
        response.Message = successCount + " Image(s) uploaded successfully! && " + errorCount + " Failed!" ;
        return response;
    }


    // [HttpPut("UpdateImage/{id}")]
    // public async Task<ApiResponse<ProductImage>> UpdateImage([FromBody] IFormFile file, int id)
    // {
    //     ApiResponse<ProductImage> apiResponse = new ApiResponse<ProductImage>();
    //     try
    //     {
    //         //check if the id is existing 
    //         var image = await _unitOfWork.ProductImage.Get(m => m.Id == id);
    //         if (image == null)
    //         {
    //             // return 404 if its not
    //             apiResponse.StatusCode = 404;
    //             apiResponse.Message = "Image not found!";
    //         }
    //         //get the file path and image
    //         string filePath = GetFilePathTrimCode(image.ProductCode);
    //         string imagePath = filePath + image.ProductFile;
    //         if (System.IO.File.Exists(imagePath))
    //         {
    //             // if file exist then delete from directory
    //             System.IO.File.Delete(imagePath);
    //             
    //         }
    //         else
    //         {
    //             //return 404 if image does not exist on the directory
    //             apiResponse.StatusCode = 404;
    //             apiResponse.Message = "Image does not exist";
    //         }
    //         //upload new image
    //         string newFilePath = GetFilePath(image.ProductCode);
    //         string newImagePath = newFilePath +file.FileName;
    //         string newDbPath = image.ProductCode + "/" + file.FileName;
    //
    //         using (System.IO.FileStream fileStream = System.IO.File.Create(newImagePath))
    //         {
    //             await file.CopyToAsync(fileStream);
    //         }
    //         _unitOfWork.ProductImage.Update( new ProductImage
    //         {
    //             ProductCode = image.ProductCode,
    //             ProductFile = newDbPath
    //         });
    //         _unitOfWork.Save();
    //         apiResponse.StatusCode = 200;
    //         apiResponse.Message = "Image Updated!";
    //         
    //
    //     }
    //     catch (Exception e)
    //     {
    //         apiResponse.StatusCode = 300;
    //         apiResponse.Message = e.Message;
    //     }
    //
    //     return apiResponse;
    // }
    
    
    [NonAction]
    private string GetFilePath(string productCode)
    {
        return _hostEnvironment.WebRootPath + "/uploads/products/" + productCode + "/";
    }

      
    [NonAction]
    private string GetFilePathTrimCode(string productCode)
    {
        return _hostEnvironment.WebRootPath + "/uploads/products/";
    }
}