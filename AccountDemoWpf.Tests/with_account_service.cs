using ReactiveDomain.Tests.Specifications;
using System;
using AccountDemoWpf;

namespace AccountDemoWpf.Tests
{
    // ReSharper disable once InconsistentNaming
    public abstract class with_account_service : MockRepositorySpecification, IDisposable
    {
        private bool _disposed;
        protected AccountSvc AcctSvc;

        static with_account_service()
        {
            Bootstrap.Load();
        }

        protected override void Given()
        {
            base.Given();
            AcctSvc = new AccountSvc(Bus, Repository);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
            }

            _disposed = true;
        }
    }
}
