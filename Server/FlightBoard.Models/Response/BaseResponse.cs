using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FlightBoard.Models.Response
{
    [Serializable]
    [DataContract(Name = "BaseResponse")]
    public class BaseResponse
    {
        public BaseResponse()
        {
            TicketResponse = Guid.NewGuid();
            Header = new ResponseHeader();
            Body = new ResponseBody();
        }

        [DataMember(Name = "TicketResponse")]
        public Guid TicketResponse { get; set; }

        [DataMember(Name = "Header")]
        public ResponseHeader Header { get; set; }

        [DataMember(Name = "Body")]
        public dynamic Body { get; set; }

        [DataMember(Name = "InterchangeId")]
        public string InterchangeId { get; set; }
    }

    [Serializable]
    [DataContract(Name = "ResponseHeader")]
    public class ResponseHeader
    {
        [DataMember(Name = "ReturnCode")]
        public string ReturnCode { get; set; }

        [DataMember(Name = "ReturnCodeMessage")]
        public string ReturnCodeMessage { get; set; }
    }


    [Serializable]
    [DataContract(Name = "ResponseBody")]
    public class ResponseBody
    {
        public ResponseBody()
        {
            Items = new Dictionary<string, object>();
        }

        [DataMember(Name = "Items")]
        public IDictionary<string, object> Items { get; set; }
    }
}
