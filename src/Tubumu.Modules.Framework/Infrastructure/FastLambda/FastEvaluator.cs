using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Threading;

namespace Tubumu.Modules.Framework.Infrastructure.FastLambda
{
    public class FastEvaluator : IEvaluator
    {
        private static IExpressionCache<Func<List<object>, object>> s_cache =
            new HashedListCache<Func<List<object>, object>>();

        private DelegateGenerator m_delegateGenerator = new DelegateGenerator();
        private ConstantExtractor m_constantExtrator = new ConstantExtractor();

        private IExpressionCache<Func<List<object>, object>> m_cache;
        private Func<Expression, Func<List<object>, object>> m_creatorDelegate;

        public FastEvaluator()
            : this(s_cache)
        { }

        public FastEvaluator(IExpressionCache<Func<List<object>, object>> cache)
        {
            this.m_cache = cache;
            this.m_creatorDelegate = (key) => this.m_delegateGenerator.Generate(key);
        }

        public object Eval(Expression exp)
        {
            if (exp.NodeType == ExpressionType.Constant)
            {
                return ((ConstantExpression)exp).Value;
            }

            var parameters = this.m_constantExtrator.Extract(exp);
            var func = this.m_cache.Get(exp, this.m_creatorDelegate);
            return func(parameters);
        }
    }        
}
