namespace Bank.Logic;

public static class Utilities
{
    public static bool IndicatesNegativeAmount(this TransactionType type)
    {
        return type switch
        {
            TransactionType.Fee_Overdraft => true,
            TransactionType.Fee_Management => true, // Already true, just confirming
            TransactionType.Withdraw => true,
            TransactionType.Deposit => false,
            TransactionType.Interest => false,
            TransactionType.Unknown => false,
            _ => false
        };
    }
}