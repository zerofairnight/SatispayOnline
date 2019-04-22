using System;
using System.Linq;
using System.Net.Http;

namespace SatispayOnline
{
    public class SatispayException : Exception
    {
        public HttpResponseMessage Response { get; set; }

        public int Code { get; }
        public override string Message { get; }
        public string Wlt { get; }

        public SatispayException()
        {

        }
        
        public SatispayException(int code, string message, string wlt)
        {
            Code = code;
            Message = message;
            wlt = message;
        }
    }

    public class SatispayUnauthorizedException : SatispayException
    {
        public SatispayUnauthorizedException()
        {

        }

        public SatispayUnauthorizedException(int code, string message, string wlt)
            : base(code, message, wlt)
        {

        }
    }

    public class SatispayValidationException : SatispayException
    {
        public SatispayValidationException()
        {

        }

        public SatispayValidationException(int code, string message, string wlt)
            : base(code, message, wlt)
        {

        }
    }
}
