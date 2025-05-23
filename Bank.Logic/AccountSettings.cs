namespace Bank.Logic;

// do not edit this file (except for adding ManagementFee)
public record class AccountSettings
{
    public double OverdraftFee { get; set; } = 35.00;
    public double ManagementFee { get; set; } = -5.00; // Added for bonus
}