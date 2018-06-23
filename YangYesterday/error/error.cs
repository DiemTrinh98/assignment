using System;

namespace YangYesterday.error
{
    
        public class SpringHeroTransactionException: Exception
        {
            public SpringHeroTransactionException(string message) : base(message)
            {
            }
        }
}