using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Tubumu.Modules.Framework.Infrastructure.FastLambda
{
    public class ExpressionHasher : ExpressionVisitor
    {
        public int Hash(Expression exp)
        { 
            this.HashCode = 0;
            this.Visit(exp);
            return this.HashCode;
        }

        public int HashCode { get; protected set; }

        protected virtual ExpressionHasher Hash(int value)
        {
            unchecked { this.HashCode += value; }
            return this;
        }

        protected virtual ExpressionHasher Hash(bool value)
        {
            unchecked { this.HashCode += value ? 1 : 0; }
            return this;
        }

        private static readonly object s_nullValue = new object();

        protected virtual ExpressionHasher Hash(object value)
        {
            value = value ?? s_nullValue;
            unchecked { this.HashCode += value.GetHashCode(); }
            return this;
        }

        protected override Expression Visit(Expression exp)
        {
            if (exp == null) return exp;

            this.Hash((int)exp.NodeType).Hash(exp.Type);
            return base.Visit(exp);
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            this.Hash(b.IsLifted).Hash(b.IsLiftedToNull).Hash(b.Method);
            return base.VisitBinary(b);
        }

        protected override MemberBinding VisitBinding(MemberBinding binding)
        {
            this.Hash(binding.BindingType).Hash(binding.Member);
            return base.VisitBinding(binding);
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            this.Hash(c.Value);
            return base.VisitConstant(c);
        }

        protected override ElementInit VisitElementInitializer(ElementInit initializer)
        {
            this.Hash(initializer.AddMethod);
            return base.VisitElementInitializer(initializer);
        }

        protected override Expression VisitLambda(LambdaExpression lambda)
        {
            foreach (var p in lambda.Parameters)
            {
                this.VisitParameter(p);
            }

            return base.VisitLambda(lambda);
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            this.Hash(m.Member);
            return base.VisitMemberAccess(m);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            this.Hash(m.Method);
            return base.VisitMethodCall(m);
        }

        protected override NewExpression VisitNew(NewExpression nex)
        {
            this.Hash(nex.Constructor);
            if (nex.Members != null)
            {
                foreach (var m in nex.Members) this.Hash(m);
            }

            return base.VisitNew(nex);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            this.Hash(p.Name);
            return base.VisitParameter(p);
        }

        protected override Expression VisitTypeIs(TypeBinaryExpression b)
        {
            this.Hash(b.TypeOperand);
            return base.VisitTypeIs(b);
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            this.Hash(u.IsLifted).Hash(u.IsLiftedToNull).Hash(u.Method);
            return base.VisitUnary(u);
        }
    }
}
