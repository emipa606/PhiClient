namespace PhiClient.TransactionSystem;

public enum TransactionResponse
{
    WAITING,

    ACCEPTED,

    DECLINED,

    INTERRUPTED,

    TOOFAST
}