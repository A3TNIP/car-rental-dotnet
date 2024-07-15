namespace CarRentalSystem.Application.Base;

public interface IBaseDto<E, D>: IBaseMapper<E, D>
{
    public Guid GetId();
    public string GetName();
}