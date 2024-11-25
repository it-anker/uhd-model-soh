using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using SOH.Process.Server.Models.Ogc;

namespace IO.Swagger.Models;

[DataContract]
public partial class Reference
{
    /// <summary>
    /// Gets or Sets _Ref.
    /// </summary>
    [Required]
    [DataMember(Name = "$ref")]
    public string _Ref { get; set; } = default!;
}