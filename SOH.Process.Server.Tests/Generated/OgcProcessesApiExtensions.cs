using System.Text.Json;
using System.Text.Json.Serialization;
using NetTopologySuite.IO.Converters;
using Newtonsoft.Json;

namespace SOH.Process.Server.Generated;

public static class OgcProcessesApiExtensions
{
    public static JsonSerializerSettings CustomJsonSerializerSettings(JsonSerializerSettings options)
    {
        options.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        options.NullValueHandling = NullValueHandling.Ignore;
        options.DefaultValueHandling = DefaultValueHandling.Include;
        options.DateFormatHandling = DateFormatHandling.IsoDateFormat;
        options.Converters.Add(new FeatureCollectionConverter());
        options.Converters.Add(new FeatureConverter());
        options.Converters.Add(new GeometryConverter());
        options.Converters.Add(new AttributesTableConverter());
        return options;
    }
}