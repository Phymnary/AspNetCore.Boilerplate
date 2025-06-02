using System.Text.Json.Serialization;
using AspNetCore.Boilerplate.Api;

namespace AspNetCore.Boilerplate;

[JsonSerializable(typeof(AspErrorMessage))]
internal partial class AspJsonSerializerContext : JsonSerializerContext;
