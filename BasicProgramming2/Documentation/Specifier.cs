using System;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Documentation;

public class Specifier<T> : ISpecifier
{
    private Type _type;

    public Specifier()
    {
        _type = typeof(T);
    }

    public string GetApiDescription()
    {
        return _type.GetApiDescription();
    }

    public string[] GetApiMethodNames()
    {
        return _type.GetMethods()
            .Where(x => x.IsApiMethod())
            .Select(x => x.Name)
            .ToArray();
    }

    public string GetApiMethodDescription(string methodName)
    {
        return _type.GetApiMethod(methodName)
            ?.GetApiDescription();
    }

    public string[] GetApiMethodParamNames(string methodName)
    {
        return _type.GetApiMethod(methodName)
            .GetParameters()
            .Select(x => x.Name)
            .ToArray();
    }

    public string GetApiMethodParamDescription(string methodName, string paramName)
    {
        return _type.GetApiMethod(methodName)
            ?.GetParameters()
            .Where(x => x.Name == paramName)
            .FirstOrDefault()
            ?.GetApiDescription();
    }

    public ApiParamDescription GetApiMethodParamFullDescription(string methodName, string paramName)
    {
        var parameter = _type.GetApiMethod(methodName)
            ?.GetParameter(paramName)
            ?.GetFullApiDescription();
        return parameter ??
            new ApiParamDescription
            {
                ParamDescription = new CommonDescription { Name = paramName }
            };
    }

    public ApiMethodDescription GetApiMethodFullDescription(string methodName)
    {
        return _type.GetApiMethod(methodName)
            ?.GetFullApiDescription();
    }
}

public static class MethodInfoExtensions
{
    public static ParameterInfo GetParameter(this MethodInfo info, string name)
    {
        return info?.GetParameters()
            .Where(x => x.Name == name)
            .FirstOrDefault();
    }

    public static bool IsApiMethod(this MethodInfo info)
    {
        return info?.GetCustomAttribute<ApiMethodAttribute>() is not null;
    }

    public static string GetApiDescription(this MethodInfo info)
    {
        return info.GetCustomAttribute<ApiDescriptionAttribute>()
            ?.Description;
    }

    public static ApiMethodDescription GetFullApiDescription(this MethodInfo info)
    {
        if (info is null)
            return null;

        return new ApiMethodDescription
        {
            MethodDescription = new CommonDescription
            {
                Name = info.Name,
                Description = info.GetCustomAttribute<ApiDescriptionAttribute>()
                    ?.Description
            },
            ParamDescriptions = info.GetParameters()
                .Select(x => x.GetFullApiDescription())
                .ToArray(),
            ReturnDescription = info.ReturnParameter
                .GetFullApiDescription()
        };
    }
}

public static class ParameterInfoExtensions
{
    public static string GetApiDescription(this ParameterInfo info)
    {
        return info.GetCustomAttribute<ApiDescriptionAttribute>()
            ?.Description;
    }

    public static ApiParamDescription GetFullApiDescription(this ParameterInfo info)
    {
        if (info is null || info.ParameterType == typeof(void))
            return null;

        var description = new ApiParamDescription
        {
            Required = info.GetCustomAttribute<ApiRequiredAttribute>()
                ?.Required ?? false
        };

        if (!string.IsNullOrEmpty(info.Name))
            description.ParamDescription = new CommonDescription
            {
                Name = info.Name,
                Description = info.GetCustomAttribute<ApiDescriptionAttribute>()
                    ?.Description
            };

        var intValidation = info.GetCustomAttribute<ApiIntValidationAttribute>();
        if (intValidation is not null)
        {
            description.MinValue = intValidation.MinValue;
            description.MaxValue = intValidation.MaxValue;
        }

        return description;
    }
}

public static class TypeExtensions
{
    public static MethodInfo GetApiMethod(this Type type, string name)
    {
        var method = type.GetMethod(name);
        return method.IsApiMethod() ? method : null;
    }

    public static string GetApiDescription(this Type type)
    {
        return type.GetCustomAttribute<ApiDescriptionAttribute>()
            ?.Description;
    }
}