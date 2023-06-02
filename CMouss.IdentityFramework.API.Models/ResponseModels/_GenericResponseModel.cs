using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework.API.Models
{
    //public class GenericResponseModel
    //{
    //    public string Message { get; set; }
    //    public string ReferenceId { get; set; }
    //    public bool IsSuccess { get; set; }


    //    public GenericResponseModel(bool isSuccess)
    //    {
    //        IsSuccess = isSuccess;
    //        Message = "";
    //        ReferenceId = "";
    //    }
    //    public GenericResponseModel(bool isSuccess, string message)
    //    {
    //        IsSuccess = isSuccess;
    //        Message = message;
    //        ReferenceId = "";
    //    }
    //    public GenericResponseModel(bool isSuccess, string message, string referenceId)
    //    {
    //        IsSuccess = isSuccess;
    //        Message = message;
    //        ReferenceId = referenceId;

    //    }
    //}

    public class ErrorModel
    {
        public string Code { get; set; }
        public string Message { get; set; }

    }

    public class BooleanResponseModel
    {
        public ResponseStatusModel ResponseStatus { get; set; } = new ResponseStatusModel();
        public bool Result { get; set; }
    }


    public class GenericResponseModel
    {
        public ResponseStatusModel ResponseStatus { get; set; } = new ResponseStatusModel();
    }

    public class ResponseStatusModel
    {
        public string Message { get; set; } = "";
        public bool IsSuccess { get; set; } = false;
        public List<string> references { get; set; } = new List<string>();
        public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();

        public void SetAsSuccess()
        {
            IsSuccess = true;
        }
        public void SetAsSuccess(string message)
        {
            IsSuccess = true;
            Message = message;
        }
        public void SetAsSuccess(string message, string reference)
        {
            IsSuccess = true;
            Message = message;
            this.references = new List<string> { reference };
        }
        public void SetAsSuccess(string message, List<string> references)
        {
            IsSuccess = true;
            Message = message;
            this.references = references;
        }


        public void SetAsFailed(ErrorModel error)
        {
            IsSuccess = false;
            Errors.Add(error);
        }

        public void SetAsFailed(string message)
        {
            IsSuccess = false;
            Errors.Add(new ErrorModel { Message = message });
        }
        public void SetAsFailed(int code, string error)
        {
            IsSuccess = false;
            Errors.Add(new ErrorModel { Code = code.ToString(), Message = error });
        }

        public void SetAsFailed(List<ErrorModel> errors)
        {
            IsSuccess = false;
            Errors.AddRange(errors);
        }


    }
}
