namespace ProiectPSSC.Domain
{
    public record ValidatedOrder(ProductCode productCode, Quantity quantity, Address address, Price price);
}
