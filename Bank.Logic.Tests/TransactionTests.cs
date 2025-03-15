using Bank.Logic.Abstractions;
using FluentAssertions;
using System;
using Xunit;

namespace Bank.Logic.Tests
{
    public class TransactionTests
    {
        public ITransaction CreateTransaction(TransactionType type, double amount, DateTime date)
        {
            return new Transaction(type, amount, date);
        }

        [Fact]
        public void CreateTransaction_WithValidType_ShouldSetCorrectType()
        {
            var transaction = CreateTransaction(TransactionType.Deposit, 100, DateTime.UtcNow);

            transaction.Type.Should().Be(TransactionType.Deposit, nameof(ITransaction.Type));
        }

        [Fact]
        public void CreateTransaction_WithValidAmount_ShouldSetCorrectAmount()
        {
            var transaction = CreateTransaction(TransactionType.Deposit, 100, DateTime.UtcNow);

            transaction.Amount.Should().Be(100, nameof(ITransaction.Amount));
        }

        [Fact]
        public void CreateTransaction_WithValidDate_ShouldSetCorrectDate()
        {
            var date = DateTime.UtcNow;
            var transaction = CreateTransaction(TransactionType.Deposit, 100, date);

            transaction.Date.Should().BeCloseTo(date, TimeSpan.FromMilliseconds(10), nameof(ITransaction.Date));
        }

        [Fact]
        public void CreateTransaction_Deposit_ShouldValidatePositiveAmount()
        {
            var transaction = CreateTransaction(TransactionType.Deposit, 100, DateTime.UtcNow);

            transaction.Amount.Should().BePositive($"{nameof(ITransaction.Amount)} should be positive for {TransactionType.Deposit}");
        }

        [Fact]
        public void CreateTransaction_Withdraw_ShouldValidateNegativeAmount()
        {
            var transaction = CreateTransaction(TransactionType.Withdraw, -50, DateTime.UtcNow);

            transaction.Amount.Should().BeNegative($"{nameof(ITransaction.Amount)} should be negative for {TransactionType.Withdraw}");
        }

        [Fact]
        public void CreateTransaction_FeeOverdraft_ShouldValidateNegativeAmount()
        {
            var transaction = CreateTransaction(TransactionType.Fee_Overdraft, -35, DateTime.UtcNow);

            transaction.Amount.Should().BeNegative($"{nameof(ITransaction.Amount)} should be negative for {TransactionType.Fee_Overdraft}");
        }

        [Fact]
        public void CreateTransaction_Interest_ShouldValidatePositiveAmount()
        {
            var transaction = CreateTransaction(TransactionType.Interest, 10, DateTime.UtcNow);

            transaction.Amount.Should().BePositive($"{nameof(ITransaction.Amount)} should be positive for {TransactionType.Interest}");
        }

        [Fact]
        public void CreateTransaction_Unknown_ShouldValidatePositiveAmount()
        {
            var transaction = CreateTransaction(TransactionType.Unknown, 0, DateTime.UtcNow);

            transaction.Amount.Should().BePositive($"{nameof(ITransaction.Amount)} should be positive for {TransactionType.Unknown}");
        }

        [Fact]
        public void CreateTransaction_FeeManagement_ShouldValidateNegativeAmount()
        {
            var transaction = CreateTransaction(TransactionType.Fee_Management, -5, DateTime.UtcNow);

            transaction.Amount.Should().BeNegative($"{nameof(ITransaction.Amount)} should be negative for {TransactionType.Fee_Management}");
        }

        [Fact]
        public void CreateTransaction_DepositWithNegativeAmount_ShouldThrowException()
        {
            Action act = () => CreateTransaction(TransactionType.Deposit, -100, DateTime.UtcNow);

            act.Should().Throw<ArgumentOutOfRangeException>($"{nameof(ITransaction.Amount)} should be positive for {TransactionType.Deposit}");
        }

        [Fact]
        public void CreateTransaction_InterestWithNegativeAmount_ShouldThrowException()
        {
            Action act = () => CreateTransaction(TransactionType.Interest, -10, DateTime.UtcNow);

            act.Should().Throw<ArgumentOutOfRangeException>($"{nameof(ITransaction.Amount)} should be positive for {TransactionType.Interest}");
        }

        [Fact]
        public void CreateTransaction_FeeManagementWithPositiveAmount_ShouldThrowException()
        {
            Action act = () => CreateTransaction(TransactionType.Fee_Management, 5, DateTime.UtcNow);

            act.Should().Throw<ArgumentOutOfRangeException>($"{nameof(ITransaction.Amount)} should be negative for {TransactionType.Fee_Management}");
        }

        [Fact]
        public void CreateTransaction_WithdrawWithPositiveAmount_ShouldThrowException()
        {
            Action act = () => CreateTransaction(TransactionType.Withdraw, 50, DateTime.UtcNow);

            act.Should().Throw<ArgumentOutOfRangeException>($"{nameof(ITransaction.Amount)} should be negative for {TransactionType.Withdraw}");
        }

        [Fact]
        public void CreateTransaction_FeeOverdraftWithPositiveAmount_ShouldThrowException()
        {
            Action act = () => CreateTransaction(TransactionType.Fee_Overdraft, 35, DateTime.UtcNow);

            act.Should().Throw<ArgumentOutOfRangeException>($"{nameof(ITransaction.Amount)} should be negative for {TransactionType.Fee_Overdraft}");
        }
    }
}