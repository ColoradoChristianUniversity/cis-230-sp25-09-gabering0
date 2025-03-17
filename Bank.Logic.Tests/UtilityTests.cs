using Bank.Logic;
using FluentAssertions;
using Xunit;

namespace Bank.Logic.Tests
{
    public class UtilityTests
    {
        [Fact]
        public void IsNegative_ShouldReturnTrue_ForNegativeTransactionTypes()
        {
            bool result = Utilities.IndicatesNegativeAmount(TransactionType.Withdraw);
            result.Should().BeTrue($"Withdraw should indicate a negative amount");
        }

        [Fact]
        public void IsNegative_ShouldReturnFalse_ForPositiveTransactionTypes()
        {
            bool result = Utilities.IndicatesNegativeAmount(TransactionType.Deposit);
            result.Should().BeFalse($"Deposit should not indicate a negative amount");
        }
    }
}