using AutoMapper;

namespace Pragmatic.Design.Core.Mappings;

public interface IAutoMapper
{
    static abstract void ConfigureAutoMapper(IMapperConfigurationExpression mapping);
}
