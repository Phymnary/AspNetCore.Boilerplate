using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using AspNetCore.Boilerplate.Domain;
using static System.Linq.Expressions.Expression;

namespace AspNetCore.Boilerplate.EntityFrameworkCore;

public class EntityUpdateOptions<TEntity>
    where TEntity : class, IEntity
{
    public EntityUpdateOptions(params string[] ignores)
    {
        ignores = [.. ignores, .. SpecialEntityProperties.DefaultMappingIgnores];

        var from = Parameter(typeof(TEntity), "from");
        var to = Parameter(typeof(TEntity), "to");

        List<BinaryExpression> assigns = [];

        foreach (var propertyInfo in typeof(TEntity).GetProperties())
        {
            if (ignores.Contains(propertyInfo.Name) || propertyInfo.Name.EndsWith("Id"))
                continue;

            if (
                propertyInfo is { SetMethod.IsPublic: true, GetMethod.IsPublic: true }
                && propertyInfo.GetCustomAttribute<RequiredMemberAttribute>() is not null
                && IsNotInit(propertyInfo.SetMethod)
            )
                assigns.Add(
                    Assign(Property(to, propertyInfo.Name), Property(from, propertyInfo.Name))
                );
        }

        var body = Block(assigns);
        Run = Lambda<Action<TEntity, TEntity>>(body, from, to).Compile();
    }

    public Action<TEntity, TEntity> Run { get; }

    private static bool IsNotInit(MethodInfo setMethod)
    {
        return !setMethod
            .ReturnParameter.GetRequiredCustomModifiers()
            .Contains(typeof(IsExternalInit));
    }
}
