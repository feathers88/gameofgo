using System.Collections.Generic;
using System.Runtime.Serialization;
using GoG.Infrastructure.Engine;

namespace GoG.Infrastructure.Services.Engine
{
    /// <summary>
    /// A list of positions response.
    /// </summary>
    [DataContract]
    public class GoSaveSVGResponse : GoResponse
    {
        // This empty constructor is so WCF's DataContractSerializer is able to build an instance of this type.
        public GoSaveSVGResponse()
        {
        }

        public GoSaveSVGResponse(GoResultCode resultCode, string sVGText)
            : base(resultCode)
        {
            SVGText = sVGText;
        }
        
        [DataMember]
        public string SVGText { get; set; }
    }
}