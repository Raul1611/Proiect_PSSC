namespace ProiectPSSC.Domain
{
    public record UnvalidatedOrder(ProductCode productCode, Quantity quantity, Address address, Price price);
}
