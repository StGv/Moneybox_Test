using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moneybox.App;
using Moneybox.App.Domain.Services;
using Moq;
using System;

namespace MoneyBox.App.Test
{
    [TestClass]
    public class AccountTest
    {
        Mock<INotificationService> _notifierService;
        Account _account;

        [TestInitialize]
        public void SetUp()
        {
            _notifierService = new Mock<INotificationService>();
            _account = new Account(_notifierService.Object)
            {
                User = new User() { Name = "John Smith", Email = "someone@something.com" },
            };
            _account.PayIn(500m);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void test_Witdraw_WhenAccountBalanceIsInsufficient()
        {
            _account.Withdraw(Account.NotificationLimit + 1);

           Assert.IsTrue(_account.Balance == 500m);
        }

        [TestMethod]
        public void test_Witdraw_WhenAccountBalanceIsSufficient_butBelowNotificationLimit()
        {
            var oldBalance = _account.Balance;
            var oldWithdrawn = _account.Withdrawn;
            decimal withdrawal = 48m;

            _account.Withdraw(withdrawal);

            _notifierService.Verify(x => x.NotifyFundsLow(It.IsAny<String>()), Times.Once);
            Assert.IsTrue(_account.Balance == oldBalance - withdrawal);
            Assert.IsTrue(_account.Withdrawn == oldWithdrawn - withdrawal);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void test_PayIn_WhenAmountIsMoreThanPayInLimit()
        {
            var oldPaidIn = _account.PaidIn;

            _account.PayIn(Account.PayInLimit + 1);

            Assert.IsTrue(_account.PaidIn == oldPaidIn);
        }

        [TestMethod]
        public void test_PayIn_WhenAccountBalanceIsSufficient_butBelowNotificationLimit()
        {
            var oldPaidIn = _account.PaidIn;
            var oldBalance = _account.Balance;

            decimal payInAmount = 3001m; //PaidIn == 500m

            _account.PayIn(payInAmount);

            _notifierService.Verify(x => x.NotifyApproachingPayInLimit(It.IsAny<String>()), Times.Once);
            Assert.IsTrue(_account.PaidIn == oldPaidIn + payInAmount);
            Assert.IsTrue(_account.Balance == oldBalance + payInAmount);
        }

    }
}
