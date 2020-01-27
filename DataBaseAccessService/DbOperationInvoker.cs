using System;

namespace DataBaseAccessService
{
    internal class CDbOperationInvoker<T>
    {
        private readonly Func<CActionChessDbContext, T> _operation;

        public CDbOperationInvoker(Func<CActionChessDbContext, T> operation)
        {
            _operation = operation;
        }

        public T Invoke(CActionChessDbContext context)
        {
            try
            {
                return _operation.Invoke(context);
            }
            catch (Exception exception)
            {
                return default;
            }
        }
    }
}
