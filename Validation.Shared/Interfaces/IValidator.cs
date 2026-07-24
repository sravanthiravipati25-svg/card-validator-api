namespace Validation.Shared.Interfaces
{
    /// <summary>
    /// Generic validation contract so future services (IMEI, SSN) follow the same shape.
    /// </summary>
    public interface IValidator<TInput, TResult>
    {
        TResult Validate(TInput input);
    }
}
