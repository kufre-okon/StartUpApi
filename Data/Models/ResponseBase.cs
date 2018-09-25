namespace Data.Models
{
    public class ResponseBase<T> : ResponseBase
    {
        public T Payload
        {
            get;
            set;
        }
    }   

    public class ResponseBase
    {
        public ResponseBase()
        {
            Message = string.Empty;
            Status = true;
        }
       
        public bool Status { get; set; }

        public string Message
        {
            get;
            set;
        }
    }

}
