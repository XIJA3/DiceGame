namespace Domain.Contracts
{
    public interface IDbEnum : IIdentifiable
    {
        public string Name { get; set; }
    }
}
