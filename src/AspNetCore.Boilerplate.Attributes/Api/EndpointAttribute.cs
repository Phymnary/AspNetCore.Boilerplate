namespace AspNetCore.Boilerplate.Api;


/// <summary>
/// <para>Generate a partial class implement <see cref="AspNetCore.Boilerplate.Api.IEndpoint"/> that needs a HandleAsync method from base class</para>
/// <param name="method">Explicit tell generator what method for RouteBuilder</param>
/// </summary>

[AttributeUsage(AttributeTargets.Class)]
public class EndpointAttribute : Attribute
{
    public EndpointAttribute() { }

    public EndpointAttribute(Method method) { }
    
}
