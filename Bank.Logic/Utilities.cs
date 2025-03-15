namespace Bank.Logic;

public static class Utilities
{
    public static bool IndicatesNegativeAmount(this TransactionType type)
    {
        return type switch
        {
            TransactionType.Fee_Overdraft => true,
            TransactionType.Fee_Management => true, // Corrected from ManagementFee
            TransactionType.Withdraw => true, // Corrected from Withdrawal
            TransactionType.Deposit => false,
            TransactionType.Interest => false,
            TransactionType.Unknown => false,
            _ => false
        };
    }
}