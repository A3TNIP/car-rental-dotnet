namespace CarRentalSystem.Application.Base;

public interface IBaseMapper<TE, TD>
{
    public TD MapToDto(TE entity);
    public TE MapToEntity();
    public TE UpdateEntity(TE entity, TD dto);
}
