using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StartUpApi.Models
{
    public class ResponseBase<T> : ResponseBase
    {
        public T Payload
        {
            get;
            set;
        }
    }

    public enum ResponseCode
    {
        OK = 200,
        Unauthorized = 401,
        Not_Found = 404,

        /// <summary>
        /// Indicate that the request failed backend validation
        /// </summary>
        Invalid_Request = 600,
        Bad_Request = 400

    }



    public class ResponseBase
    {
        public ResponseBase()
        {
            Message = string.Empty;
            Status = true;
            StatusCode = ResponseCode.OK;
        }

        /// <summary>
        /// Use on the frontend to distinguish api response and other responses
        /// </summary>
        public bool Status { get; set; }

        public ResponseCode StatusCode { get; set; }

        public string Message
        {
            get;
            set;
        }
    }
}
