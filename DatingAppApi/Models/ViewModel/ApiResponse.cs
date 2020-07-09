using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace DatingAppApi.Models.ViewModel
{
    public class ApiResponse
    {
        [DataMember]
        public int StatusCode { get; set; }
        [DataMember]
        public bool Status { get; set; }
        [DataMember]
        public List<string> Errors { get; set; }
        [DataMember]
        public object Data { get; set; }
        [DataMember]
        public object Token { get; set; }
        public ApiResponse(int _StatusCode, bool _Status, List<string> _Errors, object _Data, string _token)
        {
            this.StatusCode = _StatusCode;
            this.Status = _Status;
            this.Errors = new List<string>();
            this.Errors = _Errors;
            this.Data = _Data;
            this.Token = _token;
        }
    }
}
