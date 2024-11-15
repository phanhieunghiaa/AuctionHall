using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Services.Services
{
    public class ServiceResponse<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public static ServiceResponse<T> SuccessResponse(T data)
        {
            return new ServiceResponse<T> { Data = data, Success = true };
        }

        public static ServiceResponse<T> SuccessResponseWithMessage(T data, string successMessage = "Success")
        {
            return new ServiceResponse<T> { Data = data, Success = true, SuccessMessage = successMessage };
        }


        public static ServiceResponse<T> ErrorResponse(string errorMessage)
        {
            return new ServiceResponse<T> { Success = false, ErrorMessage = errorMessage };
        }
    }
}
