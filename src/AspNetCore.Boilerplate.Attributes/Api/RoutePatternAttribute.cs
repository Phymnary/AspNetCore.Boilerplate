namespace AspNetCore.Boilerplate.Api;

/// <summary>
///  <para> Mark a static method is a route pattern for endpoint group or a property for that endpoint instant </para>
///  <para> Source generator will take the target to map endpoint route pattern </para>
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
public class RoutePatternAttribute : Attribute;
